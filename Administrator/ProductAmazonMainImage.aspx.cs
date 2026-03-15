using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class ProductAmazonMainImage : Page
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        private string OpenAiApiKey
        {
            get { return ConfigurationManager.AppSettings["OpenAI:ApiKey"] ?? string.Empty; }
        }

        private string OpenAiImageEditEndpoint
        {
            get
            {
                string endpoint = ConfigurationManager.AppSettings["OpenAI:ImageEditEndpoint"];
                if (string.IsNullOrWhiteSpace(endpoint))
                    return "https://api.openai.com/v1/images/edits";

                return endpoint;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetPreview();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMessage();

                string sku = (txtSKU.Text ?? string.Empty).Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("SKU is required.");
                    return;
                }

                if (!SkuExists(sku))
                {
                    ShowError("SKU was not found in T_Product.");
                    return;
                }

                if (!fuImage.HasFile)
                {
                    ShowError("Please select one image file.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(OpenAiApiKey))
                {
                    ShowError("OpenAI API key is missing in Web.config.");
                    return;
                }

                int canvasSize = ParsePositiveInt(txtCanvasSize.Text, 2000);
                int fillRatio = ParsePositiveInt(txtFillRatio.Text, 85);

                if (canvasSize < 1000) canvasSize = 1000;
                if (canvasSize > 4000) canvasSize = 4000;
                if (fillRatio < 60) fillRatio = 60;
                if (fillRatio > 95) fillRatio = 95;

                string extension = Path.GetExtension(fuImage.FileName ?? string.Empty).ToLowerInvariant();
                string[] allowed = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };

                if (!allowed.Contains(extension))
                {
                    ShowError("Unsupported file type. Please upload JPG, JPEG, PNG, BMP or WEBP.");
                    return;
                }

                byte[] uploadedBytes;
                using (BinaryReader br = new BinaryReader(fuImage.PostedFile.InputStream))
                {
                    uploadedBytes = br.ReadBytes(fuImage.PostedFile.ContentLength);
                }

                // 1) OpenAI ile arka planı temizlet
                byte[] openAiProcessedPng = EditImageWithOpenAi(
                    uploadedBytes,
                    GetMimeTypeFromExtension(extension),
                    Path.GetFileName(fuImage.FileName)
                );

                // 2) Son Amazon canvas çıktısını lokal olarak üret
                ProcessedImageResult result = BuildAmazonMainImageFromProcessed(openAiProcessedPng, canvasSize, fillRatio);

                // 3) DB save
                SaveImageToDatabase(
                    sku: sku,
                    originalFileName: Path.GetFileName(fuImage.FileName),
                    storedFileName: sku + "_amazon_main.jpg",
                    contentType: "image/jpeg",
                    width: result.Width,
                    height: result.Height,
                    sizeBytes: result.Bytes.Length,
                    imageBytes: result.Bytes,
                    userName: GetCurrentUserName()
                );

                BindPreviewFromBytes(result.Bytes);
                BindMeta(
                    sku,
                    Path.GetFileName(fuImage.FileName),
                    result.Width + " x " + result.Height + " | " + FormatBytes(result.Bytes.Length)
                );

                ShowSuccess("Image processed with OpenAI and saved successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        protected void btnLoadCurrent_Click(object sender, EventArgs e)
        {
            try
            {
                ClearMessage();

                string sku = (txtSKU.Text ?? string.Empty).Trim().ToUpperInvariant();

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Please enter SKU first.");
                    return;
                }

                using (SqlConnection con = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand("dbo.SP_ProductAmazonMainImage_GetBySKU", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SKU", sku);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            byte[] bytes = dr["Photo"] as byte[];
                            string originalFile = dr["OriginalFileName"] == DBNull.Value ? "" : Convert.ToString(dr["OriginalFileName"]);
                            int width = dr["ImageWidth"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ImageWidth"]);
                            int height = dr["ImageHeight"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ImageHeight"]);
                            int sizeBytes = dr["ImageSizeBytes"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ImageSizeBytes"]);

                            if (bytes != null && bytes.Length > 0)
                            {
                                BindPreviewFromBytes(bytes);
                                BindMeta(sku, originalFile, width + " x " + height + " | " + FormatBytes(sizeBytes));
                                ShowSuccess("Current saved image loaded.");
                                return;
                            }
                        }
                    }
                }

                ResetPreview();
                ShowError("No saved Amazon main image found for this SKU.");
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }

        private bool SkuExists(string sku)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.T_Product WHERE SKU = @SKU", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private byte[] EditImageWithOpenAi(byte[] sourceBytes, string mimeType, string fileName)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OpenAiApiKey);

                using (MultipartFormDataContent form = new MultipartFormDataContent())
                {
                    // Model
                    form.Add(new StringContent("gpt-image-1"), "model");

                    // Prompt
                    string prompt =
                        "Edit this product photo for an Amazon main image workflow. " +
                        "Keep the product exactly the same. " +
                        "Remove distractions, clean the background, and make the background pure white. " +
                        "Do not add extra objects, props, text, watermark, hands, shadows, decorations, or scene elements. " +
                        "Preserve the jewelry details and realistic shape. " +
                        "Return a clean isolated product image suitable for a marketplace main photo.";

                    form.Add(new StringContent(prompt, Encoding.UTF8), "prompt");

                    // Output settings
                    form.Add(new StringContent("1024x1024"), "size");
                    form.Add(new StringContent("high"), "quality");
                    form.Add(new StringContent("png"), "output_format");

                    // Image
                    ByteArrayContent imageContent = new ByteArrayContent(sourceBytes);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);
                    form.Add(imageContent, "image", fileName);

                    HttpResponseMessage response = client.PostAsync(OpenAiImageEditEndpoint, form).GetAwaiter().GetResult();
                    string responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("OpenAI API failed: " + response.StatusCode + " - " + responseText);
                    }

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.MaxJsonLength = int.MaxValue;

                    OpenAiImageEditResponse parsed = serializer.Deserialize<OpenAiImageEditResponse>(responseText);

                    if (parsed == null || parsed.data == null || parsed.data.Length == 0 || string.IsNullOrWhiteSpace(parsed.data[0].b64_json))
                    {
                        throw new Exception("OpenAI response did not include image data.");
                    }

                    return Convert.FromBase64String(parsed.data[0].b64_json);
                }
            }
        }

        private ProcessedImageResult BuildAmazonMainImageFromProcessed(byte[] processedImageBytes, int canvasSize, int fillRatio)
        {
            using (MemoryStream inputMs = new MemoryStream(processedImageBytes))
            using (Image rawImage = Image.FromStream(inputMs))
            using (Bitmap source = new Bitmap(rawImage))
            {
                Rectangle cropRect = DetectSubjectBounds(source);

                using (Bitmap cropped = CropBitmap(source, cropRect))
                {
                    int targetInnerSize = (int)Math.Round(canvasSize * (fillRatio / 100.0));
                    if (targetInnerSize < 1) targetInnerSize = (int)Math.Round(canvasSize * 0.85);

                    double scaleX = (double)targetInnerSize / cropped.Width;
                    double scaleY = (double)targetInnerSize / cropped.Height;
                    double scale = Math.Min(scaleX, scaleY);

                    int drawWidth = Math.Max(1, (int)Math.Round(cropped.Width * scale));
                    int drawHeight = Math.Max(1, (int)Math.Round(cropped.Height * scale));

                    using (Bitmap canvas = new Bitmap(canvasSize, canvasSize, PixelFormat.Format24bppRgb))
                    {
                        canvas.SetResolution(72, 72);

                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            g.Clear(Color.White);
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            int x = (canvasSize - drawWidth) / 2;
                            int y = (canvasSize - drawHeight) / 2;

                            g.DrawImage(cropped, new Rectangle(x, y, drawWidth, drawHeight));
                        }

                        using (MemoryStream outputMs = new MemoryStream())
                        {
                            SaveJpeg(canvas, outputMs, 95L);

                            return new ProcessedImageResult
                            {
                                Width = canvas.Width,
                                Height = canvas.Height,
                                Bytes = outputMs.ToArray()
                            };
                        }
                    }
                }
            }
        }

        private Rectangle DetectSubjectBounds(Bitmap bmp)
        {
            int minX = bmp.Width;
            int minY = bmp.Height;
            int maxX = 0;
            int maxY = 0;
            bool found = false;

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);

                    bool isBackground =
                        (c.A == 0) ||
                        (c.R >= 245 && c.G >= 245 && c.B >= 245);

                    if (!isBackground)
                    {
                        found = true;

                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            if (!found)
            {
                return new Rectangle(0, 0, bmp.Width, bmp.Height);
            }

            int padX = Math.Max(12, (int)Math.Round((maxX - minX + 1) * 0.05));
            int padY = Math.Max(12, (int)Math.Round((maxY - minY + 1) * 0.05));

            minX = Math.Max(0, minX - padX);
            minY = Math.Max(0, minY - padY);
            maxX = Math.Min(bmp.Width - 1, maxX + padX);
            maxY = Math.Min(bmp.Height - 1, maxY + padY);

            return Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
        }

        private Bitmap CropBitmap(Bitmap source, Rectangle cropArea)
        {
            Bitmap target = new Bitmap(cropArea.Width, cropArea.Height, PixelFormat.Format32bppArgb);
            target.SetResolution(72, 72);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.Clear(Color.White);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(
                    source,
                    new Rectangle(0, 0, cropArea.Width, cropArea.Height),
                    cropArea,
                    GraphicsUnit.Pixel
                );
            }

            return target;
        }

        private void SaveImageToDatabase(
            string sku,
            string originalFileName,
            string storedFileName,
            string contentType,
            int width,
            int height,
            int sizeBytes,
            byte[] imageBytes,
            string userName)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.SP_ProductAmazonMainImage_Save", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@OriginalFileName", (object)originalFileName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StoredFileName", (object)storedFileName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ContentType", contentType);
                cmd.Parameters.AddWithValue("@ImageWidth", width);
                cmd.Parameters.AddWithValue("@ImageHeight", height);
                cmd.Parameters.AddWithValue("@ImageSizeBytes", sizeBytes);
                cmd.Parameters.AddWithValue("@Photo", imageBytes);
                cmd.Parameters.AddWithValue("@UserName", (object)userName ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string GetMimeTypeFromExtension(string extension)
        {
            switch ((extension ?? string.Empty).ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".bmp":
                    return "image/bmp";
                case ".webp":
                    return "image/webp";
                default:
                    return "application/octet-stream";
            }
        }

        private void SaveJpeg(Image image, Stream output, long quality)
        {
            ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

            if (jpgEncoder == null)
            {
                image.Save(output, ImageFormat.Jpeg);
                return;
            }

            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            image.Save(output, jpgEncoder, encoderParameters);
        }

        private void BindPreviewFromBytes(byte[] imageBytes)
        {
            imgPreview.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
            imgPreview.Visible = true;
            lblPreviewEmpty.Visible = false;
        }

        private void BindMeta(string sku, string originalFileName, string outputInfo)
        {
            litSku.Text = Server.HtmlEncode(sku);
            litOriginalFile.Text = Server.HtmlEncode(originalFileName);
            litOutputInfo.Text = Server.HtmlEncode(outputInfo);
            lnkCurrentImage.NavigateUrl = "~/ProductAmazonMainImageHandler.ashx?sku=" + Server.UrlEncode(sku);
            pnlMeta.Visible = true;
        }

        private void ResetPreview()
        {
            imgPreview.ImageUrl = string.Empty;
            imgPreview.Visible = false;
            lblPreviewEmpty.Visible = true;
            pnlMeta.Visible = false;
        }

        private int ParsePositiveInt(string text, int defaultValue)
        {
            int value;
            if (int.TryParse((text ?? string.Empty).Trim(), out value) && value > 0)
                return value;

            return defaultValue;
        }

        private string GetCurrentUserName()
        {
            if (Context != null && Context.User != null && Context.User.Identity != null && Context.User.Identity.IsAuthenticated)
                return Context.User.Identity.Name;

            return "system";
        }

        private string FormatBytes(int byteCount)
        {
            decimal size = byteCount;
            string[] units = { "B", "KB", "MB", "GB" };
            int unit = 0;

            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return string.Format("{0:0.##} {1}", size, units[unit]);
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = "<div class='alert alert-success' style='border-radius:12px;'>" + Server.HtmlEncode(message) + "</div>";
        }

        private void ShowError(string message)
        {
            lblMessage.Text = "<div class='alert alert-danger' style='border-radius:12px;'>" + Server.HtmlEncode(message) + "</div>";
        }

        private void ClearMessage()
        {
            lblMessage.Text = string.Empty;
        }

        private class ProcessedImageResult
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public byte[] Bytes { get; set; }
        }

        private class OpenAiImageEditResponse
        {
            public OpenAiImageData[] data { get; set; }
        }

        private class OpenAiImageData
        {
            public string b64_json { get; set; }
        }
    }
}