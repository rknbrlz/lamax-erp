using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DrawingImage = System.Drawing.Image;

namespace Feniks.Administrator
{
    public partial class ProductImages : Page
    {
        private const int MaxUploadBytesPerFile = 10 * 1024 * 1024; // 10 MB
        private const int MaxLongSide = 2000;
        private const long JpegQuality = 82L;
        private const int MainCanvasSize = 2000;

        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fuImage.Attributes["multiple"] = "multiple";
                fuImage.Attributes["accept"] = ".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp";

                string sku = Request.QueryString["sku"];
                if (!string.IsNullOrWhiteSpace(sku))
                {
                    txtSKU.Text = sku.Trim();
                    BindImages();
                }
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            BindImages();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            lblMsg.CssClass = "";

            try
            {
                string sku = txtSKU.Text.Trim();
                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("SKU giriniz.");
                    return;
                }

                HttpFileCollection files = Request.Files;
                List<HttpPostedFile> imageFiles = new List<HttpPostedFile>();

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];
                    if (file == null || file.ContentLength <= 0) continue;

                    if (!string.IsNullOrWhiteSpace(file.FileName))
                        imageFiles.Add(file);
                }

                if (imageFiles.Count == 0)
                {
                    ShowError("Lütfen en az bir görsel seçiniz.");
                    return;
                }

                int nextSortOrder = GetInitialSortOrder(sku);
                bool firstShouldBePrimary = chkIsPrimary.Checked;
                int insertedCount = 0;
                List<string> info = new List<string>();

                foreach (HttpPostedFile postedFile in imageFiles)
                {
                    string originalFileName = Path.GetFileName(postedFile.FileName);
                    string ext = Path.GetExtension(originalFileName).ToLowerInvariant();

                    if (!IsAllowedExtension(ext))
                    {
                        info.Add(originalFileName + ": uzantı geçersiz, atlandı.");
                        continue;
                    }

                    if (postedFile.ContentLength <= 0)
                    {
                        info.Add(originalFileName + ": boş dosya, atlandı.");
                        continue;
                    }

                    if (postedFile.ContentLength > MaxUploadBytesPerFile)
                    {
                        info.Add(originalFileName + ": 10 MB üstü, atlandı.");
                        continue;
                    }

                    byte[] originalData = ReadFully(postedFile.InputStream);
                    if (originalData == null || originalData.Length == 0)
                    {
                        info.Add(originalFileName + ": dosya okunamadı, atlandı.");
                        continue;
                    }

                    string imageRole = string.IsNullOrWhiteSpace(ddlImageRole.SelectedValue)
                        ? "GALLERY"
                        : ddlImageRole.SelectedValue.Trim().ToUpperInvariant();

                    bool isMainLike = imageRole == "MAIN";

                    byte[] finalData = originalData;
                    string finalContentType = GetSafeContentType(ext, postedFile.ContentType);
                    string finalFileName = originalFileName;

                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
                    {
                        finalData = isMainLike
                            ? OptimizeToSquareCanvas(originalData, ext, MainCanvasSize, JpegQuality)
                            : OptimizeByLongSide(originalData, ext, MaxLongSide, JpegQuality);

                        finalContentType = (ext == ".png") ? "image/png" : "image/jpeg";
                    }
                    else if (ext == ".webp")
                    {
                        finalData = originalData;
                        finalContentType = "image/webp";
                    }

                    bool isPrimaryForThisImage = insertedCount == 0 && firstShouldBePrimary;

                    InsertImage(
                        sku,
                        finalData,
                        finalFileName,
                        finalContentType,
                        finalData.Length,
                        nextSortOrder,
                        isPrimaryForThisImage,
                        string.IsNullOrWhiteSpace(ddlMarketplace.SelectedValue) ? null : ddlMarketplace.SelectedValue,
                        imageRole,
                        User != null && User.Identity != null ? User.Identity.Name : "system"
                    );

                    insertedCount++;
                    info.Add(originalFileName + ": " + FormatSize(originalData.Length) + " → " + FormatSize(finalData.Length));
                    nextSortOrder++;
                }

                if (insertedCount == 0)
                {
                    ShowError("Hiçbir görsel yüklenemedi. " + string.Join(" | ", info));
                    return;
                }

                ShowSuccess(insertedCount + " görsel yüklendi. " + string.Join(" | ", info));
                BindImages();
            }
            catch (Exception ex)
            {
                ShowError("Hata: " + ex.Message);
            }
        }

        protected void rptImages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            lblMsg.Text = "";
            lblMsg.CssClass = "";

            try
            {
                int imageId;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out imageId))
                {
                    ShowError("Geçersiz görsel ID.");
                    return;
                }

                if (e.CommandName == "makeprimary")
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_SetPrimary", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductImageID", imageId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowSuccess("Primary görsel güncellendi.");
                    BindImages();
                }
                else if (e.CommandName == "deleteimg")
                {
                    using (SqlConnection con = new SqlConnection(ConnectionString))
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_Delete", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductImageID", imageId);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowSuccess("Görsel silindi.");
                    BindImages();
                }
            }
            catch (Exception ex)
            {
                ShowError("Hata: " + ex.Message);
            }
        }

        private void BindImages()
        {
            string sku = txtSKU.Text.Trim();

            if (string.IsNullOrWhiteSpace(sku))
            {
                rptImages.DataSource = null;
                rptImages.DataBind();
                return;
            }

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_ListBySKU", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SKU", sku);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            rptImages.DataSource = dt;
            rptImages.DataBind();
        }

        private int GetInitialSortOrder(string sku)
        {
            int parsedSort;
            if (int.TryParse(txtSortOrder.Text.Trim(), out parsedSort) && parsedSort > 0)
                return parsedSort;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ISNULL(MAX(SortOrder), 0) + 1
                FROM dbo.T_ProductImage
                WHERE SKU = @SKU", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                object result = cmd.ExecuteScalar();
                int next;
                if (result != null && int.TryParse(result.ToString(), out next) && next > 0)
                    return next;
            }

            return 1;
        }

        private void InsertImage(string sku, byte[] imageData, string fileName, string contentType,
            int fileSizeBytes, int sortOrder, bool isPrimary, string marketplace, string imageRole, string createdBy)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_Insert", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@ImageData", imageData);
                cmd.Parameters.AddWithValue("@FileName", fileName);
                cmd.Parameters.AddWithValue("@ContentType", contentType);
                cmd.Parameters.AddWithValue("@FileSizeBytes", fileSizeBytes);
                cmd.Parameters.AddWithValue("@SortOrder", sortOrder);
                cmd.Parameters.AddWithValue("@IsPrimary", isPrimary);
                cmd.Parameters.AddWithValue("@Marketplace", string.IsNullOrWhiteSpace(marketplace) ? (object)DBNull.Value : marketplace);
                cmd.Parameters.AddWithValue("@ImageRole", string.IsNullOrWhiteSpace(imageRole) ? (object)DBNull.Value : imageRole);
                cmd.Parameters.AddWithValue("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "system" : createdBy);

                con.Open();
                cmd.ExecuteScalar();
            }
        }

        private static bool IsAllowedExtension(string ext)
        {
            string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };
            return allowed.Contains(ext);
        }

        private static byte[] ReadFully(Stream input)
        {
            if (input == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private static byte[] OptimizeByLongSide(byte[] inputBytes, string ext, int maxLongSide, long jpegQuality)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (DrawingImage originalImage = DrawingImage.FromStream(inputStream))
            {
                FixOrientation(originalImage);

                Size newSize = GetScaledSize(originalImage.Width, originalImage.Height, maxLongSide);

                using (Bitmap resizedBitmap = new Bitmap(newSize.Width, newSize.Height))
                {
                    SafeSetResolution(resizedBitmap, originalImage.HorizontalResolution, originalImage.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(resizedBitmap))
                    {
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.Clear(Color.White);
                        g.DrawImage(originalImage, 0, 0, newSize.Width, newSize.Height);
                    }

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        SaveByExtension(resizedBitmap, outputStream, ext, jpegQuality);
                        return outputStream.ToArray();
                    }
                }
            }
        }

        private static byte[] OptimizeToSquareCanvas(byte[] inputBytes, string ext, int canvasSize, long jpegQuality)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (DrawingImage originalImage = DrawingImage.FromStream(inputStream))
            {
                FixOrientation(originalImage);

                Size fitSize = GetScaledSize(originalImage.Width, originalImage.Height, canvasSize);
                int x = (canvasSize - fitSize.Width) / 2;
                int y = (canvasSize - fitSize.Height) / 2;

                using (Bitmap canvas = new Bitmap(canvasSize, canvasSize))
                {
                    SafeSetResolution(canvas, originalImage.HorizontalResolution, originalImage.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.Clear(Color.White);
                        g.DrawImage(originalImage, x, y, fitSize.Width, fitSize.Height);
                    }

                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        SaveByExtension(canvas, outputStream, ext, jpegQuality);
                        return outputStream.ToArray();
                    }
                }
            }
        }

        private static void SaveByExtension(Bitmap bitmap, Stream output, string ext, long jpegQuality)
        {
            if (ext == ".png")
            {
                bitmap.Save(output, ImageFormat.Png);
                return;
            }

            ImageCodecInfo jpgCodec = GetEncoder(ImageFormat.Jpeg);
            if (jpgCodec == null)
            {
                bitmap.Save(output, ImageFormat.Jpeg);
                return;
            }

            using (EncoderParameters encoderParams = new EncoderParameters(1))
            {
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);
                bitmap.Save(output, jpgCodec, encoderParams);
            }
        }

        private static Size GetScaledSize(int width, int height, int maxLongSide)
        {
            if (width <= 0 || height <= 0)
                return new Size(1, 1);

            int longSide = Math.Max(width, height);
            if (longSide <= maxLongSide)
                return new Size(width, height);

            double ratio = (double)maxLongSide / longSide;
            int newWidth = Math.Max(1, (int)Math.Round(width * ratio));
            int newHeight = Math.Max(1, (int)Math.Round(height * ratio));

            return new Size(newWidth, newHeight);
        }

        private static void FixOrientation(DrawingImage img)
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

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
        }

        private static string GetSafeContentType(string ext, string postedContentType)
        {
            if (!string.IsNullOrWhiteSpace(postedContentType) &&
                postedContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                return postedContentType;
            }

            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".webp":
                    return "image/webp";
                default:
                    return "application/octet-stream";
            }
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            if (bytes < 1024 * 1024) return (bytes / 1024d).ToString("0.0") + " KB";
            return (bytes / 1024d / 1024d).ToString("0.00") + " MB";
        }

        private void ShowError(string message)
        {
            lblMsg.CssClass = "alert alert-danger";
            lblMsg.Text = message;
        }

        private void ShowSuccess(string message)
        {
            lblMsg.CssClass = "alert alert-success";
            lblMsg.Text = message;
        }
    }
}