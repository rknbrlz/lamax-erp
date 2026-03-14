using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DrawingImage = System.Drawing.Image;

namespace Feniks.Administrator
{
    public partial class ProductImages : Page
    {
        private const int MaxUploadBytesPerFile = 10 * 1024 * 1024; // 10 MB
        private const int MaxLongSide = 2200;
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
                {
                    fuImage.Attributes["multiple"] = "multiple";
                    fuImage.Attributes["accept"] = ".jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp";
                }

                string sku = Request.QueryString["sku"];
                if (!string.IsNullOrWhiteSpace(sku))
                {
                    txtSKU.Text = sku.Trim();
                    BindImages();
                    BindPacks();
                }
            }

            ClientScript.RegisterStartupScript(
                this.GetType(),
                "hideLoaderAfterPostback",
                "if (typeof hidePageLoader === 'function') { hidePageLoader(); }",
                true
            );

            SetDownloadLinks();
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            ClearMessage();
            BindImages();
            BindPacks();
            SetDownloadLinks();
        }

        protected void btnLoadPacks_Click(object sender, EventArgs e)
        {
            ClearMessage();
            BindPacks();
            SetDownloadLinks();
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

                int existingCount = GetImageCountBySku(sku);

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

                if (existingCount + imageFiles.Count > 4)
                {
                    ShowError("This version supports maximum 4 source images per SKU.");
                    return;
                }

                int nextSortOrder = GetInitialSortOrder(sku);
                bool firstShouldBePrimary = chkIsPrimary.Checked;

                string imageRole = string.IsNullOrWhiteSpace(ddlImageRole.SelectedValue)
                    ? "MAIN"
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

                    string finalContentType = GetSafeContentType(ext, postedFile.ContentType);
                    string finalFileName = originalFileName;

                    bool isPrimaryForThisImage = (existingCount == 0 && insertedCount == 0 && firstShouldBePrimary);

                    InsertImage(
                        sku,
                        originalData,
                        finalFileName,
                        finalContentType,
                        originalData.Length,
                        nextSortOrder,
                        isPrimaryForThisImage,
                        string.IsNullOrWhiteSpace(ddlMarketplace.SelectedValue) ? null : ddlMarketplace.SelectedValue,
                        imageRole,
                        User != null && User.Identity != null ? User.Identity.Name : "system"
                    );

                    insertedCount++;
                    info.Add(originalFileName + ": " + FormatSize(originalData.Length));
                    nextSortOrder++;
                }

                if (insertedCount == 0)
                {
                    ShowError("No images were uploaded. " + string.Join(" | ", info));
                    return;
                }

                ShowSuccess(insertedCount + " image(s) uploaded successfully. " + string.Join(" | ", info));
                BindImages();
                BindPacks();
                SetDownloadLinks();
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        protected void btnGeneratePacks_Click(object sender, EventArgs e)
        {
            ClearMessage();

            try
            {
                string sku = txtSKU.Text.Trim();
                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Please enter SKU.");
                    return;
                }

                DataTable sourceImages = GetSourceImagesForPack(sku);
                if (sourceImages.Rows.Count == 0)
                {
                    ShowError("No source images found for this SKU.");
                    return;
                }

                if (chkDeleteOldPacks.Checked)
                    DeleteGeneratedPackImages(sku);

                Dictionary<string, SourceImageItem> sourceMap = BuildSourceRoleMap(sourceImages);
                List<ChannelPackSpec> channels = BuildChannelSpecs();

                foreach (ChannelPackSpec channel in channels)
                {
                    int nextSort = GetInitialSortOrder(sku);

                    foreach (ChannelImageSpec imageSpec in channel.Images)
                    {
                        SourceImageItem source = ResolveSourceForRole(sourceMap, imageSpec.SourceRoleFallbacks);
                        if (source == null || source.ImageData == null || source.ImageData.Length == 0)
                            continue;

                        byte[] processed = ProcessMarketplaceImage(
                            source.ImageData,
                            imageSpec.OutputRole,
                            imageSpec.Options,
                            ".jpg"
                        );

                        string fileName = BuildGeneratedFileName(sku, channel.Marketplace, imageSpec.OutputRole);

                        InsertImage(
                            sku,
                            processed,
                            fileName,
                            "image/jpeg",
                            processed.Length,
                            nextSort,
                            false,
                            channel.Marketplace,
                            imageSpec.OutputRole,
                            User != null && User.Identity != null ? User.Identity.Name : "system"
                        );

                        nextSort++;
                    }
                }

                ShowSuccess("Generated listing packs created successfully.");
                BindImages();
                BindPacks();
                SetDownloadLinks();
            }
            catch (Exception ex)
            {
                ShowError("Error while generating packs: " + ex.Message);
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
                    ShowError("Invalid image ID.");
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

                    ShowSuccess("Primary image updated.");
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

                    ShowSuccess("Image deleted.");
                }

                BindImages();
                BindPacks();
                SetDownloadLinks();
            }
            catch (Exception ex)
            {
                ShowError("Error: " + ex.Message);
            }
        }

        private void ClearMessage()
        {
            if (lblMsg != null)
            {
                lblMsg.Text = "";
                lblMsg.CssClass = "";
            }
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

        private void SetDownloadLinks()
        {
            if (lnkDownloadAllPacks == null)
                return;

            string sku = txtSKU.Text.Trim();
            if (string.IsNullOrWhiteSpace(sku))
            {
                lnkDownloadAllPacks.NavigateUrl = "#";
                lnkDownloadAllPacks.Visible = false;
                return;
            }

            lnkDownloadAllPacks.NavigateUrl =
                ResolveUrl("~/Administrator/ProductImageDownload.ashx?mode=allpackszip&sku=" + HttpUtility.UrlEncode(sku));
            lnkDownloadAllPacks.Target = "_blank";
            lnkDownloadAllPacks.Visible = true;
        }

        private int GetImageCountBySku(string sku)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(FileName, '') NOT LIKE '%_pack_%';", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                object result = cmd.ExecuteScalar();
                int count;
                if (result != null && int.TryParse(result.ToString(), out count))
                    return count;

                return 0;
            }
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
WHERE SKU = @SKU;", con))
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

            DataView dv = dt.DefaultView;
            dv.RowFilter = "ISNULL(FileName, '') NOT LIKE '%_pack_%'";
            dv.Sort = "IsPrimary DESC, SortOrder ASC, ProductImageID ASC";

            rptImages.DataSource = dv;
            rptImages.DataBind();
        }

        private void BindPacks()
        {
            string sku = txtSKU.Text.Trim();
            if (string.IsNullOrWhiteSpace(sku))
            {
                litPacks.Text = "<div class='meta-text'>Enter SKU and load packs.</div>";
                return;
            }

            DataTable dt = GetGeneratedPackImages(sku);
            if (dt.Rows.Count == 0)
            {
                litPacks.Text = "<div class='meta-text'>No generated listing packs found yet.</div>";
                return;
            }

            StringBuilder sb = new StringBuilder();

            var groups = dt.AsEnumerable()
                .GroupBy(r => Convert.ToString(r["Marketplace"]).ToUpperInvariant())
                .OrderBy(g => g.Key);

            sb.Append("<div class='packs-grid'>");

            foreach (var g in groups)
            {
                string marketplace = g.Key;

                sb.Append("<div class='pack-card'>");
                sb.AppendFormat("<div class='pack-title'>{0} Listing Pack</div>", HttpUtility.HtmlEncode(ToDisplayMarketplace(marketplace)));
                sb.AppendFormat("<div class='pack-meta'>{0} · {1}</div>",
                    HttpUtility.HtmlEncode(marketplace),
                    DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));

                sb.Append("<div class='pack-actions'>");
                sb.AppendFormat(
                    "<a class='btn-link-mini btn-darkx' target='_blank' href='{0}'>Download {1} ZIP</a>",
                    ResolveUrl("~/Administrator/ProductImageDownload.ashx?mode=marketplacezip&sku=" + HttpUtility.UrlEncode(sku) + "&marketplace=" + HttpUtility.UrlEncode(marketplace)),
                    HttpUtility.HtmlEncode(ToDisplayMarketplace(marketplace))
                );
                sb.Append("</div>");

                foreach (DataRow row in g.OrderBy(x => Convert.ToInt32(x["SortOrder"])))
                {
                    int imageId = Convert.ToInt32(row["ProductImageID"]);
                    string role = Convert.ToString(row["ImageRole"]);
                    string fileName = Convert.ToString(row["FileName"]);

                    sb.Append("<div class='pack-item'>");
                    sb.AppendFormat(
                        "<img class='pack-thumb' src='{0}' alt='' />",
                        ResolveUrl("~/Administrator/ProductPhoto.ashx?id=" + imageId)
                    );
                    sb.Append("<div style='flex:1;'>");
                    sb.AppendFormat("<div class='pack-item-title'>{0}</div>", HttpUtility.HtmlEncode(role));
                    sb.AppendFormat("<div class='pack-item-file'>{0}</div>", HttpUtility.HtmlEncode(fileName));
                    sb.Append("<div class='pack-item-actions'>");
                    sb.AppendFormat(
                        "<a class='btn-mini btn-mini-dark' target='_blank' href='{0}'>Download</a>",
                        ResolveUrl("~/Administrator/ProductImageDownload.ashx?mode=single&id=" + imageId)
                    );
                    sb.Append("</div>");
                    sb.Append("</div>");
                    sb.Append("</div>");
                }

                sb.Append("</div>");
            }

            sb.Append("</div>");
            litPacks.Text = sb.ToString();
        }

        private DataTable GetSourceImagesForPack(string sku)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT ProductImageID, SKU, ImageData, FileName, ContentType, SortOrder, IsPrimary, Marketplace, ImageRole
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(FileName, '') NOT LIKE '%_pack_%'
ORDER BY IsPrimary DESC, SortOrder ASC, ProductImageID ASC;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private DataTable GetGeneratedPackImages(string sku)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT ProductImageID, SKU, FileName, SortOrder, IsPrimary, Marketplace, ImageRole
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(FileName, '') LIKE '%_pack_%'
ORDER BY Marketplace, SortOrder ASC, ProductImageID ASC;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }

        private void DeleteGeneratedPackImages(string sku)
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(FileName, '') LIKE '%_pack_%';", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private ImageProcessOptions BuildSourceOptions()
        {
            return new ImageProcessOptions
            {
                CanvasSize = 2000,
                FillRatio = 0.82,
                AutoWhiteBackground = chkAutoWhiteBg.Checked,
                CenterSubject = chkCenterSubject.Checked,
                SoftShadow = chkSoftShadow.Checked,
                EdgeThreshold = 242,
                ColorTolerance = 34,
                FeatherRadius = 2,
                ShadowOpacity = 28,
                ShadowBlur = 22,
                ShadowOffsetY = 26,
                BackgroundColor = Color.White
            };
        }

        private List<ChannelPackSpec> BuildChannelSpecs()
        {
            return new List<ChannelPackSpec>
            {
                new ChannelPackSpec
                {
                    Marketplace = "WEBSITE",
                    Images = new List<ChannelImageSpec>
                    {
                        MakeSpec("MAIN",   new[] { "MAIN", "ANGLE", "DETAIL", "SIDE" }, 1800, 0.84, true),
                        MakeSpec("HAND",   new[] { "HAND", "ANGLE", "DETAIL", "SIDE" }, 1800, 0.82, true),
                        MakeSpec("DETAIL", new[] { "DETAIL", "ANGLE", "MAIN", "SIDE" }, 1800, 0.90, false),
                        MakeSpec("SIDE",   new[] { "SIDE", "ANGLE", "DETAIL", "MAIN" }, 1800, 0.84, false)
                    }
                },
                new ChannelPackSpec
                {
                    Marketplace = "EBAY",
                    Images = new List<ChannelImageSpec>
                    {
                        MakeSpec("MAIN",   new[] { "MAIN", "ANGLE", "DETAIL", "SIDE" }, 1600, 0.83, false),
                        MakeSpec("ANGLE",  new[] { "ANGLE", "SIDE", "DETAIL", "MAIN" }, 1600, 0.84, false),
                        MakeSpec("DETAIL", new[] { "DETAIL", "ANGLE", "MAIN", "SIDE" }, 1600, 0.90, false),
                        MakeSpec("SIDE",   new[] { "SIDE", "ANGLE", "DETAIL", "MAIN" }, 1600, 0.84, false)
                    }
                },
                new ChannelPackSpec
                {
                    Marketplace = "ETSY",
                    Images = new List<ChannelImageSpec>
                    {
                        MakeSpec("MAIN",   new[] { "MAIN", "ANGLE", "DETAIL", "SIDE" }, 2000, 0.80, false),
                        MakeSpec("HAND",   new[] { "HAND", "ANGLE", "DETAIL", "SIDE" }, 2000, 0.82, true),
                        MakeSpec("DETAIL", new[] { "DETAIL", "ANGLE", "MAIN", "SIDE" }, 2000, 0.92, false),
                        MakeSpec("SIDE",   new[] { "SIDE", "ANGLE", "DETAIL", "MAIN" }, 2000, 0.84, false)
                    }
                },
                new ChannelPackSpec
                {
                    Marketplace = "AMAZON",
                    Images = new List<ChannelImageSpec>
                    {
                        MakeSpec("MAIN",   new[] { "MAIN", "ANGLE", "DETAIL", "SIDE" }, 2000, 0.88, false),
                        MakeSpec("ANGLE",  new[] { "ANGLE", "SIDE", "DETAIL", "MAIN" }, 2000, 0.86, false),
                        MakeSpec("DETAIL", new[] { "DETAIL", "ANGLE", "MAIN", "SIDE" }, 2000, 0.92, false),
                        MakeSpec("SIDE",   new[] { "SIDE", "ANGLE", "DETAIL", "MAIN" }, 2000, 0.86, false)
                    }
                }
            };
        }

        private ChannelImageSpec MakeSpec(string outputRole, string[] fallbacks, int canvas, double fillRatio, bool shadow)
        {
            return new ChannelImageSpec
            {
                OutputRole = outputRole,
                SourceRoleFallbacks = fallbacks,
                Options = new ImageProcessOptions
                {
                    CanvasSize = canvas,
                    FillRatio = fillRatio,
                    AutoWhiteBackground = true,
                    CenterSubject = true,
                    SoftShadow = shadow,
                    EdgeThreshold = 242,
                    ColorTolerance = 34,
                    FeatherRadius = 2,
                    ShadowOpacity = shadow ? 26 : 0,
                    ShadowBlur = shadow ? 20 : 0,
                    ShadowOffsetY = shadow ? 20 : 0,
                    BackgroundColor = Color.White
                }
            };
        }

        private Dictionary<string, SourceImageItem> BuildSourceRoleMap(DataTable dt)
        {
            Dictionary<string, SourceImageItem> map = new Dictionary<string, SourceImageItem>(StringComparer.OrdinalIgnoreCase);
            List<SourceImageItem> all = new List<SourceImageItem>();

            foreach (DataRow row in dt.Rows)
            {
                SourceImageItem item = new SourceImageItem
                {
                    ProductImageID = Convert.ToInt32(row["ProductImageID"]),
                    SKU = Convert.ToString(row["SKU"]),
                    ImageData = row["ImageData"] as byte[],
                    FileName = Convert.ToString(row["FileName"]),
                    Marketplace = Convert.ToString(row["Marketplace"]),
                    ImageRole = NormalizeRole(Convert.ToString(row["ImageRole"])),
                    SortOrder = row["SortOrder"] == DBNull.Value ? 0 : Convert.ToInt32(row["SortOrder"]),
                    IsPrimary = row["IsPrimary"] != DBNull.Value && Convert.ToBoolean(row["IsPrimary"])
                };

                all.Add(item);

                if (!map.ContainsKey(item.ImageRole))
                    map[item.ImageRole] = item;
            }

            SourceImageItem primary = all.FirstOrDefault(x => x.IsPrimary) ?? all.FirstOrDefault();
            if (primary != null && !map.ContainsKey("MAIN")) map["MAIN"] = primary;

            SourceImageItem second = all.Skip(1).FirstOrDefault() ?? primary;
            SourceImageItem third = all.Skip(2).FirstOrDefault() ?? second ?? primary;
            SourceImageItem fourth = all.Skip(3).FirstOrDefault() ?? third ?? second ?? primary;

            if (second != null && !map.ContainsKey("ANGLE")) map["ANGLE"] = second;
            if (third != null && !map.ContainsKey("DETAIL")) map["DETAIL"] = third;
            if (fourth != null && !map.ContainsKey("SIDE")) map["SIDE"] = fourth;
            if (!map.ContainsKey("HAND") && second != null) map["HAND"] = second;

            return map;
        }

        private SourceImageItem ResolveSourceForRole(Dictionary<string, SourceImageItem> sourceMap, string[] fallbacks)
        {
            if (fallbacks == null || fallbacks.Length == 0) return null;

            foreach (string role in fallbacks)
            {
                SourceImageItem item;
                if (sourceMap.TryGetValue(role, out item) && item != null)
                    return item;
            }

            return null;
        }

        private static string BuildGeneratedFileName(string sku, string marketplace, string role)
        {
            return string.Format("{0}_pack_{1}_{2}.jpg",
                SafeName(sku),
                SafeName(marketplace).ToLowerInvariant(),
                SafeName(role).ToLowerInvariant());
        }

        private static string SafeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "x";

            char[] invalid = Path.GetInvalidFileNameChars();
            StringBuilder sb = new StringBuilder(value.Length);

            foreach (char c in value.Trim())
            {
                if (invalid.Contains(c) || char.IsWhiteSpace(c))
                    sb.Append('_');
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private static string ToDisplayMarketplace(string marketplace)
        {
            switch ((marketplace ?? "").ToUpperInvariant())
            {
                case "AMAZON": return "Amazon";
                case "ETSY": return "Etsy";
                case "EBAY": return "eBay";
                case "WEBSITE": return "Website";
                default: return marketplace;
            }
        }

        private static string NormalizeRole(string value)
        {
            value = (value ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(value)) return "GALLERY";
            return value;
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

        private static byte[] ProcessMarketplaceImage(
            byte[] inputBytes,
            string imageRole,
            ImageProcessOptions options,
            string outputExt)
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

                    if (options.AutoWhiteBackground)
                    {
                        bgMask = DetectBackgroundMask(working, options.EdgeThreshold, options.ColorTolerance);
                        ApplyWhiteBackgroundCleanup(working, bgMask, options.FeatherRadius);
                    }

                    Rectangle subjectBounds = GetSubjectBounds(working, bgMask);
                    if (subjectBounds.Width <= 1 || subjectBounds.Height <= 1)
                        subjectBounds = new Rectangle(0, 0, working.Width, working.Height);

                    using (Bitmap crop = CropBitmap(working, subjectBounds))
                    using (Bitmap canvas = new Bitmap(options.CanvasSize, options.CanvasSize, PixelFormat.Format24bppRgb))
                    {
                        SafeSetResolution(canvas, working.HorizontalResolution, working.VerticalResolution);

                        using (Graphics g = Graphics.FromImage(canvas))
                        {
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.Clear(options.BackgroundColor);

                            Size fit = FitToCanvas(crop.Width, crop.Height, options.CanvasSize, options.CanvasSize, options.FillRatio);
                            int x = (options.CanvasSize - fit.Width) / 2;
                            int y = (options.CanvasSize - fit.Height) / 2;

                            if (options.SoftShadow)
                                DrawSoftShadow(g, x, y, fit.Width, fit.Height, options);

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
            Queue<Point> q = new Queue<Point>();

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
                    if (IsNearWhite(c, 248))
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

        private static bool IsNearWhite(Color c, int threshold)
        {
            return c.R >= threshold && c.G >= threshold && c.B >= threshold;
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

            int outW = Math.Max(1, (int)Math.Round(width * ratio));
            int outH = Math.Max(1, (int)Math.Round(height * ratio));

            return new Size(outW, outH);
        }

        private static void DrawSoftShadow(Graphics g, int x, int y, int width, int height, ImageProcessOptions options)
        {
            Rectangle shadowRect = new Rectangle(
                x + Math.Max(2, options.ShadowBlur / 5),
                y + options.ShadowOffsetY,
                width,
                height);

            using (GraphicsPath path = RoundedRect(shadowRect, Math.Max(16, width / 10)))
            {
                for (int i = 0; i < options.ShadowBlur; i += 4)
                {
                    int alpha = Math.Max(1, options.ShadowOpacity - i);
                    using (Pen pen = new Pen(Color.FromArgb(alpha, 0, 0, 0), 1 + i))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            if (diameter > bounds.Width) diameter = bounds.Width;
            if (diameter > bounds.Height) diameter = bounds.Height;

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();

            return path;
        }

        private static void SaveAsJpeg(Bitmap bitmap, Stream output, long jpegQuality)
        {
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

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
        }

        private static string GetSafeContentType(string ext, string postedContentType)
        {
            if (!string.IsNullOrWhiteSpace(postedContentType) &&
                postedContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return postedContentType;

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
            if (bytes < 1024 * 1024) return (bytes / 1024d).ToString("0.0", CultureInfo.InvariantCulture) + " KB";
            return (bytes / 1024d / 1024d).ToString("0.00", CultureInfo.InvariantCulture) + " MB";
        }

        private sealed class ImageProcessOptions
        {
            public int CanvasSize { get; set; }
            public double FillRatio { get; set; }
            public bool AutoWhiteBackground { get; set; }
            public bool CenterSubject { get; set; }
            public bool SoftShadow { get; set; }
            public int EdgeThreshold { get; set; }
            public int ColorTolerance { get; set; }
            public int FeatherRadius { get; set; }
            public int ShadowOpacity { get; set; }
            public int ShadowBlur { get; set; }
            public int ShadowOffsetY { get; set; }
            public Color BackgroundColor { get; set; }
        }

        private sealed class ChannelPackSpec
        {
            public string Marketplace { get; set; }
            public List<ChannelImageSpec> Images { get; set; }
        }

        private sealed class ChannelImageSpec
        {
            public string OutputRole { get; set; }
            public string[] SourceRoleFallbacks { get; set; }
            public ImageProcessOptions Options { get; set; }
        }

        private sealed class SourceImageItem
        {
            public int ProductImageID { get; set; }
            public string SKU { get; set; }
            public byte[] ImageData { get; set; }
            public string FileName { get; set; }
            public string Marketplace { get; set; }
            public string ImageRole { get; set; }
            public int SortOrder { get; set; }
            public bool IsPrimary { get; set; }
        }
    }
}