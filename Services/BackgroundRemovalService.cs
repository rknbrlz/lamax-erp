using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace Feniks.Services
{
    public static class BackgroundRemovalService
    {
        public static bool IsEnabled
        {
            get
            {
                return string.Equals(
                    ConfigurationManager.AppSettings["BgRemoval:Enabled"],
                    "true",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string Provider
        {
            get
            {
                return (ConfigurationManager.AppSettings["BgRemoval:Provider"] ?? "PHOTOROOM")
                    .Trim()
                    .ToUpperInvariant();
            }
        }

        public static string ApiKey
        {
            get
            {
                return (ConfigurationManager.AppSettings["BgRemoval:ApiKey"] ?? "").Trim();
            }
        }

        public static int TimeoutSeconds
        {
            get
            {
                int value;
                if (int.TryParse(ConfigurationManager.AppSettings["BgRemoval:TimeoutSeconds"], out value) && value > 0)
                    return value;

                return 120;
            }
        }

        public static byte[] RemoveBackground(byte[] imageBytes, string fileName, out string contentType)
        {
            contentType = "image/png";

            if (imageBytes == null || imageBytes.Length == 0)
                throw new Exception("Image bytes are empty.");

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "upload.jpg";

            if (!IsEnabled)
                return imageBytes;

            if (string.IsNullOrWhiteSpace(ApiKey))
                throw new Exception("Background removal API key is empty.");

            switch (Provider)
            {
                case "PHOTOROOM":
                    return RemoveWithPhotoroom(imageBytes, fileName, out contentType);

                default:
                    throw new Exception("Unsupported background removal provider: " + Provider);
            }
        }

        private static byte[] RemoveWithPhotoroom(byte[] imageBytes, string fileName, out string contentType)
        {
            contentType = "image/png";

            string url = "https://sdk.photoroom.com/v1/segment";
            string boundary = "----LamaXBoundary" + DateTime.UtcNow.Ticks.ToString("x");

            byte[] requestBody = BuildMultipartBody(
                boundary,
                "image_file",
                fileName,
                imageBytes,
                GetMimeType(fileName)
            );

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = TimeoutSeconds * 1000;
            request.ReadWriteTimeout = TimeoutSeconds * 1000;
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Headers["x-api-key"] = ApiKey;
            request.ContentLength = requestBody.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(requestBody, 0, requestBody.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (MemoryStream ms = new MemoryStream())
                {
                    if (responseStream == null)
                        throw new Exception("Background removal API returned an empty response stream.");

                    responseStream.CopyTo(ms);

                    contentType = string.IsNullOrWhiteSpace(response.ContentType)
                        ? "image/png"
                        : response.ContentType;

                    return ms.ToArray();
                }
            }
            catch (WebException webEx)
            {
                string apiError = ReadWebException(webEx);
                throw new Exception("Photoroom API error: " + apiError, webEx);
            }
        }

        private static byte[] BuildMultipartBody(
            string boundary,
            string fieldName,
            string fileName,
            byte[] fileBytes,
            string mimeType)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] lineBreak = Encoding.UTF8.GetBytes("\r\n");
                byte[] boundaryBytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
                byte[] trailerBytes = Encoding.UTF8.GetBytes("--" + boundary + "--\r\n");

                ms.Write(boundaryBytes, 0, boundaryBytes.Length);

                string header =
                    "Content-Disposition: form-data; name=\"" + fieldName + "\"; filename=\"" + fileName + "\"\r\n" +
                    "Content-Type: " + mimeType + "\r\n\r\n";

                byte[] headerBytes = Encoding.UTF8.GetBytes(header);
                ms.Write(headerBytes, 0, headerBytes.Length);

                ms.Write(fileBytes, 0, fileBytes.Length);
                ms.Write(lineBreak, 0, lineBreak.Length);

                ms.Write(trailerBytes, 0, trailerBytes.Length);

                return ms.ToArray();
            }
        }

        private static string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();

            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".webp":
                    return "image/webp";
                case ".heic":
                    return "image/heic";
                default:
                    return "application/octet-stream";
            }
        }

        private static string ReadWebException(WebException webEx)
        {
            if (webEx == null)
                return "Unknown API error.";

            if (webEx.Response == null)
                return webEx.Message;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)webEx.Response)
                using (Stream stream = response.GetResponseStream())
                {
                    if (stream == null)
                        return response.StatusCode + " - " + response.StatusDescription;

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string body = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(body))
                            return response.StatusCode + " - " + response.StatusDescription;

                        return response.StatusCode + " - " + body;
                    }
                }
            }
            catch
            {
                return webEx.Message;
            }
        }
    }
}