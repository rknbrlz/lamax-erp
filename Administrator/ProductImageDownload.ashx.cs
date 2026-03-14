using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace Feniks.Administrator
{
    public class ProductImageDownload : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string mode = (context.Request.QueryString["mode"] ?? "").Trim().ToLowerInvariant();

            try
            {
                if (mode == "single")
                {
                    DownloadSingle(context);
                    return;
                }

                if (mode == "marketplacezip")
                {
                    DownloadMarketplaceZip(context);
                    return;
                }

                if (mode == "allpackszip")
                {
                    DownloadAllPacksZip(context);
                    return;
                }

                context.Response.StatusCode = 400;
                context.Response.Write("Invalid download mode.");
            }
            catch (Exception ex)
            {
                context.Response.Clear();
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Download error: " + ex.Message);
            }
            finally
            {
                try { context.Response.End(); } catch { }
            }
        }

        private void DownloadSingle(HttpContext context)
        {
            int productImageId;
            if (!int.TryParse(context.Request.QueryString["id"], out productImageId) || productImageId <= 0)
                throw new Exception("Invalid image id.");

            byte[] imageData = null;
            string fileName = null;
            string contentType = null;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 ImageData, FileName, ContentType
FROM dbo.T_ProductImage
WHERE ProductImageID = @ProductImageID;", con))
            {
                cmd.Parameters.Add("@ProductImageID", SqlDbType.Int).Value = productImageId;
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (dr.Read())
                    {
                        if (dr["ImageData"] != DBNull.Value)
                            imageData = (byte[])dr["ImageData"];

                        if (dr["FileName"] != DBNull.Value)
                            fileName = Convert.ToString(dr["FileName"]);

                        if (dr["ContentType"] != DBNull.Value)
                            contentType = Convert.ToString(dr["ContentType"]);
                    }
                }
            }

            if (imageData == null || imageData.Length == 0)
                throw new Exception("Image not found.");

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "image_" + productImageId + ".jpg";

            if (string.IsNullOrWhiteSpace(contentType))
                contentType = "application/octet-stream";

            context.Response.Clear();
            context.Response.ContentType = contentType;
            context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
            context.Response.BinaryWrite(imageData);
        }

        private void DownloadMarketplaceZip(HttpContext context)
        {
            string sku = (context.Request.QueryString["sku"] ?? "").Trim();
            string marketplace = (context.Request.QueryString["marketplace"] ?? "").Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(sku))
                throw new Exception("SKU is required.");

            if (string.IsNullOrWhiteSpace(marketplace))
                throw new Exception("Marketplace is required.");

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT ProductImageID, FileName, ImageData
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(Marketplace, '') = @Marketplace
  AND ISNULL(FileName, '') LIKE '%_pack_%'
ORDER BY SortOrder ASC, ProductImageID ASC;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0)
                throw new Exception("No generated images found for selected marketplace.");

            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string fileName = Convert.ToString(row["FileName"]);
                        byte[] imageData = row["ImageData"] as byte[];

                        if (imageData == null || imageData.Length == 0)
                            continue;

                        if (string.IsNullOrWhiteSpace(fileName))
                            fileName = "image_" + Convert.ToString(row["ProductImageID"]) + ".jpg";

                        ZipArchiveEntry entry = zip.CreateEntry(fileName, CompressionLevel.Optimal);
                        using (Stream entryStream = entry.Open())
                        {
                            entryStream.Write(imageData, 0, imageData.Length);
                        }
                    }
                }

                string zipName = SafeName(sku) + "_" + marketplace.ToLowerInvariant() + "_listing_pack.zip";

                context.Response.Clear();
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + zipName + "\"");
                context.Response.BinaryWrite(ms.ToArray());
            }
        }

        private void DownloadAllPacksZip(HttpContext context)
        {
            string sku = (context.Request.QueryString["sku"] ?? "").Trim();
            if (string.IsNullOrWhiteSpace(sku))
                throw new Exception("SKU is required.");

            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT ProductImageID, FileName, ImageData, Marketplace
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(FileName, '') LIKE '%_pack_%'
ORDER BY Marketplace ASC, SortOrder ASC, ProductImageID ASC;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            if (dt.Rows.Count == 0)
                throw new Exception("No generated listing packs found.");

            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string fileName = Convert.ToString(row["FileName"]);
                        string marketplace = Convert.ToString(row["Marketplace"]);
                        byte[] imageData = row["ImageData"] as byte[];

                        if (imageData == null || imageData.Length == 0)
                            continue;

                        if (string.IsNullOrWhiteSpace(fileName))
                            fileName = "image_" + Convert.ToString(row["ProductImageID"]) + ".jpg";

                        string entryName = (string.IsNullOrWhiteSpace(marketplace) ? "other" : marketplace.ToLowerInvariant())
                                           + "/" + fileName;

                        ZipArchiveEntry entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
                        using (Stream entryStream = entry.Open())
                        {
                            entryStream.Write(imageData, 0, imageData.Length);
                        }
                    }
                }

                string zipName = SafeName(sku) + "_all_listing_packs.zip";

                context.Response.Clear();
                context.Response.ContentType = "application/zip";
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + zipName + "\"");
                context.Response.BinaryWrite(ms.ToArray());
            }
        }

        private static string SafeName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "file";

            foreach (char c in Path.GetInvalidFileNameChars())
                value = value.Replace(c, '_');

            return value.Replace(' ', '_');
        }
    }
}