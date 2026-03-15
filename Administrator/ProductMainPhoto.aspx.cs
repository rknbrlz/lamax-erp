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
using System.Web.UI;
using DrawingImage = System.Drawing.Image;

namespace Feniks.Administrator
{
    public partial class ProductMainPhoto : Page
    {
        private const int MaxUploadBytesPerFile = 10 * 1024 * 1024;
        private const int OutputCanvas = 2000;
        private const long JpegQuality = 90L;

        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (fuImage != null)
                    fuImage.Attributes["accept"] = ".jpg,.jpeg,.png,image/jpeg,image/png";

                string sku = Request.QueryString["sku"];
                if (!string.IsNullOrWhiteSpace(sku))
                {
                    txtSKU.Text = sku.Trim();
                    BindPreview();
                }
            }

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "hideLoaderAfterPostback",
                "if (typeof hidePageLoader === 'function') { hidePageLoader(); }",
                true
            );
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            ClearMessage();
            BindPreview();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            ClearMessage();

            try
            {
                string sku = txtSKU.Text.Trim();
                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Please enter a SKU.");
                    return;
                }

                if (fuImage == null || !fuImage.HasFile)
                {
                    ShowError("Please select one image.");
                    return;
                }

                string fileName = Path.GetFileName(fuImage.FileName);
                string ext = Path.GetExtension(fileName).ToLowerInvariant();

                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                {
                    ShowError("Only JPG and PNG files are supported.");
                    return;
                }

                if (fuImage.PostedFile.ContentLength > MaxUploadBytesPerFile)
                {
                    ShowError("File is larger than 10 MB.");
                    return;
                }

                byte[] originalData = ReadFully(fuImage.PostedFile.InputStream);
                if (originalData == null || originalData.Length == 0)
                {
                    ShowError("Image could not be read.");
                    return;
                }

                byte[] finalData = BuildAmazonMainImage(
                    originalData,
                    chkAutoWhiteBg.Checked,
                    chkCenterSubject.Checked,
                    chkUseShadow.Checked
                );

                SaveProductPhoto(
                    sku,
                    finalData,
                    "image/jpeg"
                );

                ShowSuccess("Amazon main photo saved successfully.");
                BindPreview();
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ClearMessage();

            try
            {
                string sku = txtSKU.Text.Trim();
                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Please enter a SKU.");
                    return;
                }

                DeleteProductPhoto(sku);
                ShowSuccess("Saved photo deleted.");
                BindPreview();
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        private void BindPreview()
        {
            string sku = txtSKU.Text.Trim();

            if (string.IsNullOrWhiteSpace(sku))
            {
                litPreview.Text = "<div class='empty-box'>Enter a SKU to see the saved photo.</div>";
                SetPreviewLink(null);
                return;
            }

            bool exists = ProductPhotoExists(sku);
            if (!exists)
            {
                litPreview.Text = "<div class='empty-box'>No saved photo for this SKU.</div>";
                SetPreviewLink(null);
                return;
            }

            string url = ResolveUrl("~/Administrator/ProductPhoto.ashx?sku=" + HttpUtility.UrlEncode(sku));

            litPreview.Text =
                "<div class='preview-box'>" +
                "<img class='preview-image' src='" + url + "' alt='' />" +
                "<div class='preview-meta'><strong>SKU:</strong> " + HttpUtility.HtmlEncode(sku) + "<br/><strong>Type:</strong> Amazon Main Photo</div>" +
                "</div>";

            SetPreviewLink(url);
        }

        private void SetPreviewLink(string url)
        {
            if (lnkPreview == null) return;

            if (string.IsNullOrWhiteSpace(url))
            {
                lnkPreview.NavigateUrl = "#";
                lnkPreview.Visible = false;
                return;
            }

            lnkPreview.NavigateUrl = url;
            lnkPreview.Visible = true;
        }

        private bool ProductPhotoExists(string sku)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.T_Product
WHERE SKU = @SKU
  AND Photo IS NOT NULL;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                object result = cmd.ExecuteScalar();
                int count;
                if (result != null && int.TryParse(result.ToString(), out count))
                    return count > 0;

                return false;
            }
        }

        private void SaveProductPhoto(string sku, byte[] imageData, string contentType)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.T_Product
SET Photo = @Photo,
    ContentType = @ContentType
WHERE SKU = @SKU;", con))
            {
                cmd.CommandTimeout = 60;
                cmd.Parameters.Add("@Photo", SqlDbType.VarBinary, -1).Value = imageData;
                cmd.Parameters.Add("@ContentType", SqlDbType.NVarChar, 100).Value = contentType;
                cmd.Parameters.Add("@SKU", SqlDbType.NVarChar, 100).Value = sku;

                con.Open();
                int affected = cmd.ExecuteNonQuery();

                if (affected == 0)
                    throw new Exception("SKU not found in T_Product.");
            }
        }

        private void DeleteProductPhoto(string sku)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.T_Product
SET Photo = NULL,
    ContentType = NULL
WHERE SKU = @SKU;", con))
            {
                cmd.CommandTimeout = 60;
                cmd.Parameters.Add("@SKU", SqlDbType.NVarChar, 100).Value = sku;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ClearMessage()
        {
            lblMsg.Text = "";
            lblMsg.CssClass = "";
        }

        private void ShowError(string message)
        {
            lblMsg.CssClass = "alert alert-danger";
            lblMsg.Text = HttpUtility.HtmlEncode(message);
        }

        private void ShowSuccess(string message)
        {
            lblMsg.CssClass = "alert alert-success";
            lblMsg.Text = HttpUtility.HtmlEncode(message);
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

        private static byte[] BuildAmazonMainImage(byte[] inputBytes, bool forceWhiteBackground, bool centerSubject, bool useShadow)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (DrawingImage sourceImage = DrawingImage.FromStream(inputStream))
            {
                FixOrientation(sourceImage);

                int srcW = sourceImage.Width;
                int srcH = sourceImage.Height;

                using (Bitmap src = new Bitmap(srcW, srcH, PixelFormat.Format24bppRgb))
                {
                    using (Graphics g = Graphics.FromImage(src))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.Clear(Color.White);
                        g.DrawImage(sourceImage, 0, 0, srcW, srcH);
                    }

                    Rectangle bounds = centerSubject ? FindSubjectBoundsFast(src) : new Rectangle(0, 0, src.Width, src.Height);
                    if (bounds.Width < 2 || bounds.Height < 2)
                        bounds = new Rectangle(0, 0, src.Width, src.Height);

                    using (Bitmap canvas = new Bitmap(OutputCanvas, OutputCanvas, PixelFormat.Format24bppRgb))
                    {
                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.Clear(Color.White);

                            double scale = Math.Min((OutputCanvas * 0.88) / bounds.Width, (OutputCanvas * 0.88) / bounds.Height);
                            int drawW = Math.Max(1, (int)Math.Round(bounds.Width * scale));
                            int drawH = Math.Max(1, (int)Math.Round(bounds.Height * scale));
                            int x = (OutputCanvas - drawW) / 2;
                            int y = (OutputCanvas - drawH) / 2;

                            if (useShadow)
                            {
                                using (SolidBrush sb = new SolidBrush(Color.FromArgb(18, 0, 0, 0)))
                                {
                                    g.FillEllipse(sb, x + 8, y + drawH - 12, drawW - 16, 20);
                                }
                            }

                            g.DrawImage(
                                src,
                                new Rectangle(x, y, drawW, drawH),
                                bounds,
                                GraphicsUnit.Pixel
                            );
                        }

                        if (forceWhiteBackground)
                        {
                            // final white flatten
                            using (Bitmap flattened = new Bitmap(OutputCanvas, OutputCanvas, PixelFormat.Format24bppRgb))
                            using (Graphics g = Graphics.FromImage(flattened))
                            {
                                g.Clear(Color.White);
                                g.DrawImage(canvas, 0, 0, OutputCanvas, OutputCanvas);

                                using (MemoryStream output = new MemoryStream())
                                {
                                    SaveAsJpeg(flattened, output, JpegQuality);
                                    return output.ToArray();
                                }
                            }
                        }
                        else
                        {
                            using (MemoryStream output = new MemoryStream())
                            {
                                SaveAsJpeg(canvas, output, JpegQuality);
                                return output.ToArray();
                            }
                        }
                    }
                }
            }
        }

        private static Rectangle FindSubjectBoundsFast(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            int left = w - 1;
            int top = h - 1;
            int right = 0;
            int bottom = 0;
            bool found = false;

            int step = Math.Max(1, Math.Min(w, h) / 600);

            for (int y = 0; y < h; y += step)
            {
                for (int x = 0; x < w; x += step)
                {
                    Color c = bmp.GetPixel(x, y);

                    if (c.R > 245 && c.G > 245 && c.B > 245)
                        continue;

                    found = true;
                    if (x < left) left = x;
                    if (y < top) top = y;
                    if (x > right) right = x;
                    if (y > bottom) bottom = y;
                }
            }

            if (!found)
                return new Rectangle(0, 0, w, h);

            int padX = Math.Max(8, (right - left + 1) / 20);
            int padY = Math.Max(8, (bottom - top + 1) / 20);

            left = Math.Max(0, left - padX);
            top = Math.Max(0, top - padY);
            right = Math.Min(w - 1, right + padX);
            bottom = Math.Min(h - 1, bottom + padY);

            return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
        }

        private static void SaveAsJpeg(Bitmap bitmap, Stream output, long jpegQuality)
        {
            ImageCodecInfo jpgCodec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
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
    }
}