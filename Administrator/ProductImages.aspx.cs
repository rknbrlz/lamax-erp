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
using Feniks.Services;

namespace Feniks.Administrator
{
    public partial class ProductImages : Page
    {
        private const int MaxUploadBytesPerFile = 10 * 1024 * 1024;
        private const long JpegQuality = 84L;

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
            ClearMessage();
            BindImages();
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
                    ShowError("Please select at least one image.");
                    return;
                }

                int nextSortOrder = GetInitialSortOrder(sku);
                bool firstShouldBePrimary = chkIsPrimary.Checked;
                bool autoWhiteBg = chkAutoWhiteBg.Checked;
                bool centerSubject = chkCenterSubject.Checked;
                bool softShadow = chkSoftShadow.Checked;
                bool useAiBgRemoval = chkUseAiBgRemoval != null && chkUseAiBgRemoval.Checked;

                string preset = (ddlPreset.SelectedValue ?? "AUTO").Trim().ToUpperInvariant();
                string imageRole = string.IsNullOrWhiteSpace(ddlImageRole.SelectedValue)
                    ? "GALLERY"
                    : ddlImageRole.SelectedValue.Trim().ToUpperInvariant();

                int insertedCount = 0;
                List<string> info = new List<string>();

                foreach (HttpPostedFile postedFile in imageFiles)
                {
                    string originalFileName = Path.GetFileName(postedFile.FileName);
                    string ext = Path.GetExtension(originalFileName).ToLowerInvariant();

                    if (!IsAllowedExtension(ext))
                    {
                        info.Add(originalFileName + ": skipped, invalid extension.");
                        continue;
                    }

                    if (postedFile.ContentLength > MaxUploadBytesPerFile)
                    {
                        info.Add(originalFileName + ": skipped, file is larger than 10 MB.");
                        continue;
                    }

                    byte[] originalData = ReadFully(postedFile.InputStream);
                    if (originalData == null || originalData.Length == 0)
                    {
                        info.Add(originalFileName + ": skipped, file could not be read.");
                        continue;
                    }

                    byte[] baseInputData = originalData;
                    string providerResultType = null;

                    if (useAiBgRemoval)
                    {
                        try
                        {
                            baseInputData = BackgroundRemovalService.RemoveBackground(
                                originalData,
                                originalFileName,
                                out providerResultType
                            );
                        }
                        catch (Exception apiEx)
                        {
                            info.Add(originalFileName + ": AI background removal failed, local processing used (" + apiEx.Message + ")");
                            baseInputData = originalData;
                        }
                    }

                    byte[] finalData;
                    string finalContentType;
                    string finalFileName;

                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || providerResultType == "image/png")
                    {
                        finalData = ProcessMarketplaceImage(
                            baseInputData,
                            imageRole,
                            preset,
                            autoWhiteBg,
                            centerSubject,
                            softShadow
                        );

                        finalContentType = "image/jpeg";
                        finalFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".jpg";
                    }
                    else
                    {
                        finalData = baseInputData;
                        finalContentType = string.IsNullOrWhiteSpace(providerResultType)
                            ? GetSafeContentType(ext, postedFile.ContentType)
                            : providerResultType;

                        finalFileName = originalFileName;
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
                    ShowError("No images were uploaded. " + string.Join(" | ", info));
                    return;
                }

                ShowSuccess(insertedCount + " image(s) uploaded. " + string.Join(" | ", info));
                BindImages();
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        protected void btnSaveOrder_Click(object sender, EventArgs e)
        {
            ClearMessage();

            try
            {
                string raw = hfSortOrder.Value;

                if (string.IsNullOrWhiteSpace(raw))
                {
                    string sku = txtSKU.Text.Trim();
                    if (string.IsNullOrWhiteSpace(sku))
                    {
                        ShowError("Please enter a SKU.");
                        return;
                    }

                    raw = BuildCurrentSortOrderFromDatabase(sku);
                }

                if (string.IsNullOrWhiteSpace(raw))
                {
                    ShowError("No images found to save.");
                    return;
                }

                string[] pairs = raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                int updated = 0;

                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();

                    foreach (string pair in pairs)
                    {
                        string[] parts = pair.Split(':');
                        if (parts.Length != 2) continue;

                        int imageId;
                        int sortOrder;

                        if (!int.TryParse(parts[0], out imageId)) continue;
                        if (!int.TryParse(parts[1], out sortOrder)) continue;

                        using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_UpdateSort", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@ProductImageID", imageId);
                            cmd.Parameters.AddWithValue("@SortOrder", sortOrder);
                            cmd.ExecuteNonQuery();
                            updated++;
                        }
                    }
                }

                ShowSuccess(updated + " image order(s) saved.");
                BindImages();
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        protected void rptImages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ClearMessage();

            try
            {
                int imageId;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out imageId))
                {
                    ShowError("Invalid image id.");
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

                    NormalizeSortAfterDeleteOrPrimary(txtSKU.Text.Trim());
                    ShowSuccess("Primary image updated.");
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

                    NormalizeSortAfterDeleteOrPrimary(txtSKU.Text.Trim());
                    ShowSuccess("Image deleted.");
                    BindImages();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
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

        private string BuildCurrentSortOrderFromDatabase(string sku)
        {
            List<string> parts = new List<string>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ProductImageID, SortOrder
                FROM dbo.T_ProductImage
                WHERE SKU = @SKU
                ORDER BY SortOrder, ProductImageID", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    int index = 1;
                    while (dr.Read())
                    {
                        int imageId = Convert.ToInt32(dr["ProductImageID"]);
                        parts.Add(imageId + ":" + index);
                        index++;
                    }
                }
            }

            return string.Join(",", parts);
        }

        private void NormalizeSortAfterDeleteOrPrimary(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return;

            List<int> ids = new List<int>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ProductImageID
                FROM dbo.T_ProductImage
                WHERE SKU = @SKU
                ORDER BY IsPrimary DESC, SortOrder, ProductImageID", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ids.Add(Convert.ToInt32(dr["ProductImageID"]));
                    }
                }
            }

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                for (int i = 0; i < ids.Count; i++)
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_UpdateSort", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@ProductImageID", ids[i]);
                        cmd.Parameters.AddWithValue("@SortOrder", i + 1);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void InsertImage(
            string sku,
            byte[] imageData,
            string fileName,
            string contentType,
            int fileSizeBytes,
            int sortOrder,
            bool isPrimary,
            string marketplace,
            string imageRole,
            string createdBy)
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

        private static byte[] ProcessMarketplaceImage(
            byte[] inputBytes,
            string imageRole,
            string preset,
            bool autoWhiteBg,
            bool centerSubject,
            bool softShadow)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            using (DrawingImage originalImage = DrawingImage.FromStream(inputStream))
            {
                FixOrientation(originalImage);

                Bitmap sourceBitmap = new Bitmap(originalImage);
                try
                {
                    Bitmap working = new Bitmap(sourceBitmap);
                    try
                    {
                        if (autoWhiteBg)
                        {
                            CleanLightBackgroundFromEdges(working, 242, 28);
                            ReplaceNearWhitePixels(working, 245, 18);
                        }

                        Rectangle subjectBounds = GetSubjectBounds(working, 248, 10);
                        if (subjectBounds.Width < 5 || subjectBounds.Height < 5)
                        {
                            subjectBounds = new Rectangle(0, 0, working.Width, working.Height);
                        }

                        Bitmap cropped = CropBitmap(working, subjectBounds);
                        try
                        {
                            CanvasOptions options = ResolveCanvasOptions(imageRole, preset);

                            Bitmap finalBitmap = RenderStudioCanvas(
                                cropped,
                                options,
                                softShadow,
                                centerSubject
                            );

                            try
                            {
                                using (MemoryStream output = new MemoryStream())
                                {
                                    SaveAsJpeg(finalBitmap, output, JpegQuality);
                                    return output.ToArray();
                                }
                            }
                            finally
                            {
                                finalBitmap.Dispose();
                            }
                        }
                        finally
                        {
                            cropped.Dispose();
                        }
                    }
                    finally
                    {
                        working.Dispose();
                    }
                }
                finally
                {
                    sourceBitmap.Dispose();
                }
            }
        }

        private static CanvasOptions ResolveCanvasOptions(string imageRole, string preset)
        {
            string role = (imageRole ?? "").Trim().ToUpperInvariant();
            string p = (preset ?? "AUTO").Trim().ToUpperInvariant();

            if (p == "AMAZON")
            {
                return new CanvasOptions
                {
                    CanvasWidth = 2000,
                    CanvasHeight = 2000,
                    SubjectFillRatio = role == "MAIN" ? 0.88 : 0.80,
                    ForceSquare = true
                };
            }

            if (p == "ETSY")
            {
                return new CanvasOptions
                {
                    CanvasWidth = 2000,
                    CanvasHeight = 2000,
                    SubjectFillRatio = role == "MAIN" ? 0.82 : 0.76,
                    ForceSquare = true
                };
            }

            if (p == "WEBSITE")
            {
                return new CanvasOptions
                {
                    CanvasWidth = 1800,
                    CanvasHeight = 1800,
                    SubjectFillRatio = 0.78,
                    ForceSquare = true
                };
            }

            if (p == "SQUARE")
            {
                return new CanvasOptions
                {
                    CanvasWidth = 2000,
                    CanvasHeight = 2000,
                    SubjectFillRatio = 0.84,
                    ForceSquare = true
                };
            }

            if (role == "MAIN")
            {
                return new CanvasOptions
                {
                    CanvasWidth = 2000,
                    CanvasHeight = 2000,
                    SubjectFillRatio = 0.86,
                    ForceSquare = true
                };
            }

            return new CanvasOptions
            {
                CanvasWidth = 2000,
                CanvasHeight = 2000,
                SubjectFillRatio = 0.78,
                ForceSquare = true
            };
        }

        private static Bitmap RenderStudioCanvas(Bitmap subjectBitmap, CanvasOptions options, bool addShadow, bool centerSubject)
        {
            int canvasW = options.CanvasWidth;
            int canvasH = options.CanvasHeight;

            Bitmap canvas = new Bitmap(canvasW, canvasH);
            SafeSetResolution(canvas, 96, 96);

            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                Size scaled = FitToCanvas(subjectBitmap.Width, subjectBitmap.Height, canvasW, canvasH, options.SubjectFillRatio);

                int x = centerSubject ? (canvasW - scaled.Width) / 2 : 0;
                int y = centerSubject ? (canvasH - scaled.Height) / 2 : 0;

                if (addShadow)
                {
                    DrawSoftShadow(
                        g,
                        x + (scaled.Width / 10),
                        y + scaled.Height - Math.Max(14, scaled.Height / 24),
                        (int)(scaled.Width * 0.68),
                        Math.Max(14, scaled.Height / 12)
                    );
                }

                g.DrawImage(subjectBitmap, new Rectangle(x, y, scaled.Width, scaled.Height));
            }

            return canvas;
        }

        private static Size FitToCanvas(int sourceWidth, int sourceHeight, int canvasWidth, int canvasHeight, double fillRatio)
        {
            double maxW = canvasWidth * fillRatio;
            double maxH = canvasHeight * fillRatio;

            double ratioX = maxW / sourceWidth;
            double ratioY = maxH / sourceHeight;
            double ratio = Math.Min(ratioX, ratioY);

            int newW = Math.Max(1, (int)Math.Round(sourceWidth * ratio));
            int newH = Math.Max(1, (int)Math.Round(sourceHeight * ratio));

            return new Size(newW, newH);
        }

        private static void DrawSoftShadow(Graphics g, int x, int y, int width, int height)
        {
            for (int i = 18; i >= 1; i--)
            {
                int alpha = Math.Max(1, i);
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0)))
                {
                    Rectangle r = new Rectangle(
                        x - i,
                        y - (i / 2),
                        width + (i * 2),
                        height + i
                    );

                    g.FillEllipse(brush, r);
                }
            }
        }

        private static Rectangle GetSubjectBounds(Bitmap bitmap, byte whiteThreshold, int tolerance)
        {
            int minX = bitmap.Width;
            int minY = bitmap.Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (!LooksLikeWhite(c, whiteThreshold, tolerance))
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            if (maxX < minX || maxY < minY)
                return Rectangle.Empty;

            int padX = Math.Max(8, (maxX - minX) / 20);
            int padY = Math.Max(8, (maxY - minY) / 20);

            minX = Math.Max(0, minX - padX);
            minY = Math.Max(0, minY - padY);
            maxX = Math.Min(bitmap.Width - 1, maxX + padX);
            maxY = Math.Min(bitmap.Height - 1, maxY + padY);

            return Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
        }

        private static Bitmap CropBitmap(Bitmap source, Rectangle rect)
        {
            Bitmap cropped = new Bitmap(rect.Width, rect.Height);
            SafeSetResolution(cropped, source.HorizontalResolution, source.VerticalResolution);

            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.Clear(Color.White);
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                g.DrawImage(source, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            }

            return cropped;
        }

        private static void SaveAsJpeg(Bitmap bitmap, Stream output, long quality)
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

        private static void CleanLightBackgroundFromEdges(Bitmap bitmap, byte minChannel, int tolerance)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            bool[,] visited = new bool[width, height];
            Queue<Point> queue = new Queue<Point>();

            for (int x = 0; x < width; x++)
            {
                EnqueueIfBackground(bitmap, visited, queue, x, 0, minChannel, tolerance);
                EnqueueIfBackground(bitmap, visited, queue, x, height - 1, minChannel, tolerance);
            }

            for (int y = 0; y < height; y++)
            {
                EnqueueIfBackground(bitmap, visited, queue, 0, y, minChannel, tolerance);
                EnqueueIfBackground(bitmap, visited, queue, width - 1, y, minChannel, tolerance);
            }

            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0, 0, 1, -1 };

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                bitmap.SetPixel(p.X, p.Y, Color.White);

                for (int i = 0; i < 4; i++)
                {
                    int nx = p.X + dx[i];
                    int ny = p.Y + dy[i];

                    if (nx < 0 || ny < 0 || nx >= width || ny >= height)
                        continue;

                    if (visited[nx, ny])
                        continue;

                    EnqueueIfBackground(bitmap, visited, queue, nx, ny, minChannel, tolerance);
                }
            }
        }

        private static void ReplaceNearWhitePixels(Bitmap bitmap, byte whiteThreshold, int tolerance)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    if (LooksLikeWhite(c, whiteThreshold, tolerance))
                    {
                        bitmap.SetPixel(x, y, Color.White);
                    }
                }
            }
        }

        private static void EnqueueIfBackground(Bitmap bitmap, bool[,] visited, Queue<Point> queue, int x, int y, byte minChannel, int tolerance)
        {
            if (visited[x, y])
                return;

            Color c = bitmap.GetPixel(x, y);
            if (LooksLikeBackground(c, minChannel, tolerance))
            {
                visited[x, y] = true;
                queue.Enqueue(new Point(x, y));
            }
        }

        private static bool LooksLikeBackground(Color c, byte minChannel, int tolerance)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));

            bool brightEnough = c.R >= minChannel && c.G >= minChannel && c.B >= minChannel;
            bool lowColorCast = (max - min) <= tolerance;

            return brightEnough && lowColorCast;
        }

        private static bool LooksLikeWhite(Color c, byte whiteThreshold, int tolerance)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));

            return c.R >= whiteThreshold &&
                   c.G >= whiteThreshold &&
                   c.B >= whiteThreshold &&
                   (max - min) <= tolerance;
        }

        private void ClearMessage()
        {
            lblMsg.Text = "";
            lblMsg.CssClass = "";
        }

        private void ShowError(string message)
        {
            lblMsg.CssClass = "alert alert-danger";
            lblMsg.Text = Server.HtmlEncode(message);
        }

        private void ShowSuccess(string message)
        {
            lblMsg.CssClass = "alert alert-success";
            lblMsg.Text = Server.HtmlEncode(message);
        }

        private sealed class CanvasOptions
        {
            public int CanvasWidth { get; set; }
            public int CanvasHeight { get; set; }
            public double SubjectFillRatio { get; set; }
            public bool ForceSquare { get; set; }
        }
    }
}