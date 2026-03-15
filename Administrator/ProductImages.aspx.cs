using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using DrawingImage = System.Drawing.Image;

namespace Feniks.Administrator
{
    public partial class ProductImages : Page
    {
        private const int MaxUploadBytesPerFile = 10 * 1024 * 1024;
        private const int MaxLongSide = 1800;
        private const long JpegQuality = 92L;

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

                byte[] finalData = ProcessSingleMainPhoto(
                    originalData,
                    chkAutoWhiteBg.Checked,
                    chkCenterSubject.Checked,
                    chkSoftShadow.Checked
                );

                ReplaceMainPhotoForSku(
                    sku,
                    finalData,
                    Path.GetFileNameWithoutExtension(fileName) + "_main.jpg",
                    "image/jpeg",
                    finalData.Length,
                    User != null && User.Identity != null ? User.Identity.Name : "system"
                );

                ShowSuccess("Main product photo saved successfully.");
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

                DeleteMainPhotoForSku(sku);
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
                return;
            }

            DataRow row = GetMainPhotoBySku(sku);
            if (row == null)
            {
                litPreview.Text = "<div class='empty-box'>No saved photo for this SKU.</div>";
                return;
            }

            int imageId = Convert.ToInt32(row["ProductImageID"]);
            string fileName = Convert.ToString(row["FileName"]);

            litPreview.Text =
                "<img class='preview-image' src='" + ResolveUrl("~/Administrator/ProductPhoto.ashx?id=" + imageId) + "' alt='' />" +
                "<div class='preview-meta'><strong>File:</strong> " + HttpUtility.HtmlEncode(fileName) + "</div>";
        }

        private void ReplaceMainPhotoForSku(string sku, byte[] imageData, string fileName, string contentType, int fileSizeBytes, string createdBy)
        {
            DeleteMainPhotoForSku(sku);

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_Insert", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@ImageData", imageData);
                cmd.Parameters.AddWithValue("@FileName", fileName);
                cmd.Parameters.AddWithValue("@ContentType", contentType);
                cmd.Parameters.AddWithValue("@FileSizeBytes", fileSizeBytes);
                cmd.Parameters.AddWithValue("@SortOrder", 1);
                cmd.Parameters.AddWithValue("@IsPrimary", true);
                cmd.Parameters.AddWithValue("@Marketplace", DBNull.Value);
                cmd.Parameters.AddWithValue("@ImageRole", "MAIN");
                cmd.Parameters.AddWithValue("@CreatedBy", string.IsNullOrWhiteSpace(createdBy) ? "system" : createdBy);

                con.Open();
                cmd.ExecuteScalar();
            }
        }

        private void DeleteMainPhotoForSku(string sku)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(ImageRole, '') = 'MAIN'
  AND ISNULL(FileName, '') NOT LIKE '%_pack_%';", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private DataRow GetMainPhotoBySku(string sku)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 ProductImageID, SKU, FileName, ContentType, SortOrder, IsPrimary
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(ImageRole, '') = 'MAIN'
  AND ISNULL(FileName, '') NOT LIKE '%_pack_%'
ORDER BY ProductImageID DESC;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
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

        private static byte[] ProcessSingleMainPhoto(byte[] inputBytes, bool autoWhiteBg, bool centerSubject, bool softShadow)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (DrawingImage sourceImage = DrawingImage.FromStream(inputStream))
            {
                FixOrientation(sourceImage);

                Size scaled = GetScaledSize(sourceImage.Width, sourceImage.Height, MaxLongSide);

                using (Bitmap working = new Bitmap(scaled.Width, scaled.Height, PixelFormat.Format32bppArgb))
                {
                    SafeSetResolution(working, sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

                    using (Graphics g = Graphics.FromImage(working))
                    {
                        g.CompositingMode = CompositingMode.SourceCopy;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.Clear(Color.White);
                        g.DrawImage(sourceImage, 0, 0, scaled.Width, scaled.Height);
                    }

                    bool[,] bgMask = null;

                    if (autoWhiteBg)
                    {
                        bgMask = DetectBackgroundMask(working, 242, 34);
                        ApplyWhiteBackgroundCleanup(working, bgMask, 2);
                    }

                    Rectangle subjectBounds = centerSubject
                        ? GetSubjectBounds(working, bgMask)
                        : new Rectangle(0, 0, working.Width, working.Height);

                    if (subjectBounds.Width <= 1 || subjectBounds.Height <= 1)
                        subjectBounds = new Rectangle(0, 0, working.Width, working.Height);

                    using (Bitmap crop = CropBitmap(working, subjectBounds))
                    using (Bitmap canvas = new Bitmap(1800, 1800, PixelFormat.Format24bppRgb))
                    {
                        SafeSetResolution(canvas, working.HorizontalResolution, working.VerticalResolution);

                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.Clear(Color.White);

                            Size fit = FitToCanvas(crop.Width, crop.Height, 1800, 1800, 0.84);
                            int x = (1800 - fit.Width) / 2;
                            int y = (1800 - fit.Height) / 2;

                            if (softShadow)
                                DrawSoftShadow(g, x, y, fit.Width, fit.Height);

                            g.DrawImage(crop, x, y, fit.Width, fit.Height);
                        }

                        using (MemoryStream output = new MemoryStream())
                        {
                            SaveAsJpeg(canvas, output, JpegQuality);
                            return output.ToArray();
                        }
                    }
                }
            }
        }

        private static bool[,] DetectBackgroundMask(Bitmap bmp, int edgeThreshold, int tolerance)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            bool[,] visited = new bool[w, h];
            bool[,] bg = new bool[w, h];
            System.Collections.Generic.Queue<Point> q = new System.Collections.Generic.Queue<Point>();

            Action<int, int> enqueue = (x, y) =>
            {
                if (x < 0 || y < 0 || x >= w || y >= h) return;
                if (visited[x, y]) return;
                visited[x, y] = true;
                q.Enqueue(new Point(x, y));
            };

            for (int x = 0; x < w; x++)
            {
                enqueue(x, 0);
                enqueue(x, h - 1);
            }

            for (int y = 0; y < h; y++)
            {
                enqueue(0, y);
                enqueue(w - 1, y);
            }

            while (q.Count > 0)
            {
                Point p = q.Dequeue();
                Color c = bmp.GetPixel(p.X, p.Y);

                if (!IsLikelyBackground(c, edgeThreshold, tolerance))
                    continue;

                bg[p.X, p.Y] = true;

                enqueue(p.X - 1, p.Y);
                enqueue(p.X + 1, p.Y);
                enqueue(p.X, p.Y - 1);
                enqueue(p.X, p.Y + 1);
            }

            return bg;
        }

        private static bool IsLikelyBackground(Color c, int threshold, int tolerance)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));
            int spread = max - min;
            int avg = (c.R + c.G + c.B) / 3;

            bool nearWhite = c.R >= threshold && c.G >= threshold && c.B >= threshold;
            bool brightNeutral = avg >= threshold - 4 && spread <= tolerance;
            bool lightGray = avg >= threshold - 10 && spread <= tolerance / 2;

            return nearWhite || brightNeutral || lightGray;
        }

        private static void ApplyWhiteBackgroundCleanup(Bitmap bmp, bool[,] bgMask, int featherRadius)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (bgMask[x, y])
                    {
                        bmp.SetPixel(x, y, Color.White);
                        continue;
                    }

                    int neighborBgCount = CountBackgroundNeighbors(bgMask, x, y);
                    if (neighborBgCount > 0)
                    {
                        Color c = bmp.GetPixel(x, y);

                        int blendedR = BlendChannel(c.R, 255, neighborBgCount, 8);
                        int blendedG = BlendChannel(c.G, 255, neighborBgCount, 8);
                        int blendedB = BlendChannel(c.B, 255, neighborBgCount, 8);

                        bmp.SetPixel(x, y, Color.FromArgb(blendedR, blendedG, blendedB));
                    }
                }
            }

            if (featherRadius > 0)
                SoftFeatherEdges(bmp, bgMask, featherRadius);
        }

        private static void SoftFeatherEdges(Bitmap bmp, bool[,] bgMask, int radius)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            Bitmap clone = (Bitmap)bmp.Clone();

            for (int y = radius; y < h - radius; y++)
            {
                for (int x = radius; x < w - radius; x++)
                {
                    if (bgMask[x, y]) continue;

                    int bgNeighbors = 0;
                    int total = 0;

                    for (int oy = -radius; oy <= radius; oy++)
                    {
                        for (int ox = -radius; ox <= radius; ox++)
                        {
                            total++;
                            if (bgMask[x + ox, y + oy]) bgNeighbors++;
                        }
                    }

                    if (bgNeighbors <= 0) continue;

                    Color c = clone.GetPixel(x, y);
                    double ratio = Math.Min(0.35, (double)bgNeighbors / total * 0.55);

                    int r = (int)Math.Round(c.R + (255 - c.R) * ratio);
                    int g = (int)Math.Round(c.G + (255 - c.G) * ratio);
                    int b = (int)Math.Round(c.B + (255 - c.B) * ratio);

                    bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            clone.Dispose();
        }

        private static int CountBackgroundNeighbors(bool[,] bgMask, int x, int y)
        {
            int w = bgMask.GetLength(0);
            int h = bgMask.GetLength(1);
            int count = 0;

            for (int oy = -1; oy <= 1; oy++)
            {
                for (int ox = -1; ox <= 1; ox++)
                {
                    if (ox == 0 && oy == 0) continue;
                    int nx = x + ox;
                    int ny = y + oy;
                    if (nx < 0 || ny < 0 || nx >= w || ny >= h) continue;
                    if (bgMask[nx, ny]) count++;
                }
            }

            return count;
        }

        private static int BlendChannel(int value, int target, int count, int maxCount)
        {
            double ratio = Math.Min(0.40, (double)count / maxCount * 0.50);
            return (int)Math.Round(value + (target - value) * ratio);
        }

        private static Rectangle GetSubjectBounds(Bitmap bmp, bool[,] bgMask)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            int left = w - 1;
            int top = h - 1;
            int right = 0;
            int bottom = 0;
            bool found = false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (bgMask != null && bgMask[x, y])
                        continue;

                    Color c = bmp.GetPixel(x, y);
                    if (c.R >= 248 && c.G >= 248 && c.B >= 248)
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

            int padX = Math.Max(10, (right - left + 1) / 16);
            int padY = Math.Max(10, (bottom - top + 1) / 16);

            left = Math.Max(0, left - padX);
            top = Math.Max(0, top - padY);
            right = Math.Min(w - 1, right + padX);
            bottom = Math.Min(h - 1, bottom + padY);

            return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
        }

        private static Bitmap CropBitmap(Bitmap source, Rectangle rect)
        {
            Bitmap target = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            SafeSetResolution(target, source.HorizontalResolution, source.VerticalResolution);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.White);
                g.DrawImage(source, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            }

            return target;
        }

        private static Size FitToCanvas(int width, int height, int canvasWidth, int canvasHeight, double fillRatio)
        {
            double usableWidth = canvasWidth * fillRatio;
            double usableHeight = canvasHeight * fillRatio;
            double ratio = Math.Min(usableWidth / width, usableHeight / height);

            return new Size(
                Math.Max(1, (int)Math.Round(width * ratio)),
                Math.Max(1, (int)Math.Round(height * ratio))
            );
        }

        private static void DrawSoftShadow(Graphics g, int x, int y, int width, int height)
        {
            Rectangle shadowRect = new Rectangle(x + 4, y + 18, width, height);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddRectangle(shadowRect);

                for (int i = 0; i < 18; i += 4)
                {
                    int alpha = Math.Max(1, 22 - i);
                    using (Pen pen = new Pen(Color.FromArgb(alpha, 0, 0, 0), 1 + i))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
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

        private static Size GetScaledSize(int width, int height, int maxLongSide)
        {
            if (width <= 0 || height <= 0)
                return new Size(1, 1);

            int longSide = Math.Max(width, height);
            if (longSide <= maxLongSide)
                return new Size(width, height);

            double ratio = (double)maxLongSide / longSide;

            return new Size(
                Math.Max(1, (int)Math.Round(width * ratio)),
                Math.Max(1, (int)Math.Round(height * ratio))
            );
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
                bmp.SetResolution(horizontal > 0 ? horizontal : 96, vertical > 0 ? vertical : 96);
            }
            catch
            {
                bmp.SetResolution(96, 96);
            }
        }
    }
}