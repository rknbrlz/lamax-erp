using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Feniks
{
    public class ProductImageHandler : IHttpHandler
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Buffer = true;

            try
            {
                int imageId;
                if (!int.TryParse(context.Request.QueryString["id"], out imageId) || imageId <= 0)
                {
                    WriteTextError(context, 400, "Invalid image id.");
                    return;
                }

                bool isThumb = ParseBool(context.Request.QueryString["thumb"]);
                int width = ParseInt(context.Request.QueryString["w"], 0);
                int height = ParseInt(context.Request.QueryString["h"], 0);

                if (isThumb)
                {
                    if (width <= 0 && height <= 0)
                        width = 400;

                    width = NormalizeSize(width, 50, 2000);
                    height = NormalizeSize(height, 0, 2000);
                }

                string cacheKey = BuildCacheKey(imageId, isThumb, width, height);
                CachedImageResult cached = HttpRuntime.Cache[cacheKey] as CachedImageResult;

                if (cached != null)
                {
                    if (HandleNotModified(context, cached.LastModifiedUtc))
                        return;

                    WriteImageResponse(context, cached.Bytes, cached.ContentType, cached.LastModifiedUtc);
                    return;
                }

                DbImageResult dbImage = GetImageFromDatabase(imageId);
                if (dbImage == null || dbImage.Bytes == null || dbImage.Bytes.Length == 0)
                {
                    WriteTextError(context, 404, "Image not found.");
                    return;
                }

                byte[] outputBytes = dbImage.Bytes;
                string outputContentType = GetSafeImageContentType(dbImage.ContentType);

                if (isThumb)
                {
                    outputBytes = CreateThumbnail(dbImage.Bytes, width, height);
                    outputContentType = "image/jpeg";
                }

                DateTime lastModifiedUtc = dbImage.CreatedOnUtc ?? DateTime.UtcNow;

                CachedImageResult item = new CachedImageResult
                {
                    Bytes = outputBytes,
                    ContentType = outputContentType,
                    LastModifiedUtc = lastModifiedUtc
                };

                HttpRuntime.Cache.Insert(
                    cacheKey,
                    item,
                    null,
                    Cache.NoAbsoluteExpiration,
                    isThumb ? TimeSpan.FromMinutes(60) : TimeSpan.FromMinutes(20)
                );

                if (HandleNotModified(context, lastModifiedUtc))
                    return;

                WriteImageResponse(context, outputBytes, outputContentType, lastModifiedUtc);
            }
            catch (Exception ex)
            {
                WriteTextError(context, 500, "Handler error: " + ex.Message);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private DbImageResult GetImageFromDatabase(int imageId)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_Get", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductImageID", imageId);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!dr.Read())
                        return null;

                    DbImageResult result = new DbImageResult();

                    result.Bytes = dr["ImageData"] == DBNull.Value ? null : (byte[])dr["ImageData"];
                    result.ContentType = dr["ContentType"] == DBNull.Value ? "image/jpeg" : Convert.ToString(dr["ContentType"]);

                    if (HasColumn(dr, "CreatedOn") && dr["CreatedOn"] != DBNull.Value)
                    {
                        DateTime createdOn = Convert.ToDateTime(dr["CreatedOn"]);
                        result.CreatedOnUtc = createdOn.Kind == DateTimeKind.Utc
                            ? createdOn
                            : createdOn.ToUniversalTime();
                    }
                    else
                    {
                        result.CreatedOnUtc = DateTime.UtcNow;
                    }

                    return result;
                }
            }
        }

        private byte[] CreateThumbnail(byte[] originalBytes, int reqWidth, int reqHeight)
        {
            using (MemoryStream input = new MemoryStream(originalBytes))
            using (Image original = Image.FromStream(input))
            {
                FixOrientation(original);

                Size targetSize = CalculateTargetSize(original.Width, original.Height, reqWidth, reqHeight);

                using (Bitmap canvas = new Bitmap(targetSize.Width, targetSize.Height))
                {
                    SafeSetResolution(canvas, original.HorizontalResolution, original.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.Clear(Color.White);
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.DrawImage(original, 0, 0, targetSize.Width, targetSize.Height);
                    }

                    using (MemoryStream output = new MemoryStream())
                    {
                        SaveJpeg(canvas, output, 82L);
                        return output.ToArray();
                    }
                }
            }
        }

        private static Size CalculateTargetSize(int originalWidth, int originalHeight, int reqWidth, int reqHeight)
        {
            if (originalWidth <= 0 || originalHeight <= 0)
                return new Size(1, 1);

            if (reqWidth > 0 && reqHeight > 0)
            {
                double ratioX = (double)reqWidth / originalWidth;
                double ratioY = (double)reqHeight / originalHeight;
                double ratio = Math.Min(ratioX, ratioY);

                int w = Math.Max(1, (int)Math.Round(originalWidth * ratio));
                int h = Math.Max(1, (int)Math.Round(originalHeight * ratio));
                return new Size(w, h);
            }

            if (reqWidth > 0)
            {
                double ratio = (double)reqWidth / originalWidth;
                int h = Math.Max(1, (int)Math.Round(originalHeight * ratio));
                return new Size(reqWidth, h);
            }

            if (reqHeight > 0)
            {
                double ratio = (double)reqHeight / originalHeight;
                int w = Math.Max(1, (int)Math.Round(originalWidth * ratio));
                return new Size(w, reqHeight);
            }

            return new Size(originalWidth, originalHeight);
        }

        private static void SaveJpeg(Bitmap bitmap, Stream output, long quality)
        {
            ImageCodecInfo jpgCodec = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

            if (jpgCodec == null)
            {
                bitmap.Save(output, ImageFormat.Jpeg);
                return;
            }

            using (EncoderParameters encoderParams = new EncoderParameters(1))
            {
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                bitmap.Save(output, jpgCodec, encoderParams);
            }
        }

        private static void FixOrientation(Image img)
        {
            try
            {
                const int ExifOrientationId = 0x0112;
                if (!img.PropertyIdList.Contains(ExifOrientationId))
                    return;

                PropertyItem prop = img.GetPropertyItem(ExifOrientationId);
                int orientation = BitConverter.ToUInt16(prop.Value, 0);

                switch (orientation)
                {
                    case 2: img.RotateFlip(RotateFlipType.RotateNoneFlipX); break;
                    case 3: img.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                    case 4: img.RotateFlip(RotateFlipType.Rotate180FlipX); break;
                    case 5: img.RotateFlip(RotateFlipType.Rotate90FlipX); break;
                    case 6: img.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                    case 7: img.RotateFlip(RotateFlipType.Rotate270FlipX); break;
                    case 8: img.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                }

                img.RemovePropertyItem(ExifOrientationId);
            }
            catch
            {
            }
        }

        private static void SafeSetResolution(Bitmap bmp, float horizontal, float vertical)
        {
            try
            {
                if (horizontal <= 0 || vertical <= 0)
                {
                    bmp.SetResolution(96, 96);
                    return;
                }

                bmp.SetResolution(horizontal, vertical);
            }
            catch
            {
                bmp.SetResolution(96, 96);
            }
        }

        private static bool HandleNotModified(HttpContext context, DateTime lastModifiedUtc)
        {
            string ifModifiedSince = context.Request.Headers["If-Modified-Since"];
            DateTime parsed;

            if (!string.IsNullOrWhiteSpace(ifModifiedSince) && DateTime.TryParse(ifModifiedSince, out parsed))
            {
                DateTime imsUtc = parsed.ToUniversalTime();

                if (Math.Abs((imsUtc - lastModifiedUtc).TotalSeconds) < 1)
                {
                    context.Response.StatusCode = 304;
                    return true;
                }
            }

            return false;
        }

        private static void WriteImageResponse(HttpContext context, byte[] bytes, string contentType, DateTime lastModifiedUtc)
        {
            context.Response.ContentType = contentType;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
            context.Response.Cache.SetSlidingExpiration(false);
            context.Response.Cache.SetLastModified(lastModifiedUtc);
            context.Response.Cache.SetValidUntilExpires(true);

            context.Response.BinaryWrite(bytes);
        }

        private static void WriteTextError(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "text/plain";
            context.Response.Write(message);
        }

        private static string BuildCacheKey(int imageId, bool isThumb, int width, int height)
        {
            return "productimg:" + imageId + ":" + (isThumb ? "t" : "o") + ":" + width + "x" + height;
        }

        private static int ParseInt(string input, int defaultValue)
        {
            int value;
            return int.TryParse(input, out value) ? value : defaultValue;
        }

        private static bool ParseBool(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim().ToLowerInvariant();
            return input == "1" || input == "true" || input == "yes";
        }

        private static int NormalizeSize(int value, int min, int max)
        {
            if (value < min) return min == 0 ? 0 : min;
            if (value > max) return max;
            return value;
        }

        private static string GetSafeImageContentType(string dbType)
        {
            if (string.IsNullOrWhiteSpace(dbType))
                return "image/jpeg";

            dbType = dbType.Trim().ToLowerInvariant();

            if (dbType == "image/jpeg" || dbType == "image/jpg" || dbType == "image/png" || dbType == "image/webp")
                return dbType;

            return "image/jpeg";
        }

        private static bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(dr.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private sealed class DbImageResult
        {
            public byte[] Bytes { get; set; }
            public string ContentType { get; set; }
            public DateTime? CreatedOnUtc { get; set; }
        }

        private sealed class CachedImageResult
        {
            public byte[] Bytes { get; set; }
            public string ContentType { get; set; }
            public DateTime LastModifiedUtc { get; set; }
        }
    }
}