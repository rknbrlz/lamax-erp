using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Feniks.Administrator
{
    public class ProductPhoto : IHttpHandler
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
            context.Response.Clear();
            context.Response.Buffer = true;

            try
            {
                int productImageId;
                string rawId =
                    context.Request.QueryString["id"] ??
                    context.Request.QueryString["ProductImageID"] ??
                    context.Request.QueryString["ImageID"] ??
                    context.Request.QueryString["pid"];

                if (int.TryParse(rawId, out productImageId) && productImageId > 0)
                {
                    ServeProductImageById(context, productImageId);
                    return;
                }

                string sku = context.Request.QueryString["sku"];
                if (!string.IsNullOrWhiteSpace(sku))
                {
                    ServeProductPhotoBySku(context, sku.Trim());
                    return;
                }

                WritePlaceholder(context);
            }
            catch
            {
                WritePlaceholder(context);
            }
            finally
            {
                try { context.Response.End(); } catch { }
            }
        }

        private void ServeProductImageById(HttpContext context, int productImageId)
        {
            byte[] imageData = null;
            string contentType = null;
            string fileName = null;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 ImageData, ContentType, FileName
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

                        if (dr["ContentType"] != DBNull.Value)
                            contentType = Convert.ToString(dr["ContentType"]);

                        if (dr["FileName"] != DBNull.Value)
                            fileName = Convert.ToString(dr["FileName"]);
                    }
                }
            }

            if (imageData == null || imageData.Length == 0)
            {
                WritePlaceholder(context);
                return;
            }

            if (string.IsNullOrWhiteSpace(contentType))
                contentType = GuessContentType(fileName);

            bool download = string.Equals(context.Request.QueryString["download"], "1", StringComparison.OrdinalIgnoreCase);

            context.Response.ContentType = contentType;
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            if (download)
            {
                string safeFileName = string.IsNullOrWhiteSpace(fileName) ? ("image_" + productImageId + ".jpg") : fileName;
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + safeFileName + "\"");
            }

            context.Response.BinaryWrite(imageData);
        }

        private void ServeProductPhotoBySku(HttpContext context, string sku)
        {
            byte[] photoBytes = null;
            string contentType = null;

            using (SqlConnection con = new SqlConnection(ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 Photo, ContentType
FROM dbo.T_Product
WHERE SKU = @SKU;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (dr.Read())
                    {
                        if (dr["Photo"] != DBNull.Value)
                            photoBytes = (byte[])dr["Photo"];

                        if (dr["ContentType"] != DBNull.Value)
                            contentType = Convert.ToString(dr["ContentType"]);
                    }
                }
            }

            if (photoBytes == null || photoBytes.Length == 0)
            {
                WritePlaceholder(context);
                return;
            }

            if (string.IsNullOrWhiteSpace(contentType))
                contentType = "image/jpeg";

            context.Response.ContentType = contentType;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromDays(7));
            context.Response.Cache.SetSlidingExpiration(true);
            context.Response.BinaryWrite(photoBytes);
        }

        private static string GuessContentType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "image/jpeg";

            string ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            switch (ext)
            {
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".webp": return "image/webp";
                case ".gif": return "image/gif";
                default: return "image/jpeg";
            }
        }

        private static void WritePlaceholder(HttpContext context)
        {
            context.Response.ContentType = "image/svg+xml";
            context.Response.Write(
                "<svg xmlns='http://www.w3.org/2000/svg' width='300' height='300'>" +
                "<rect width='100%' height='100%' fill='#f3f4f6'/>" +
                "<text x='50%' y='50%' dominant-baseline='middle' text-anchor='middle' " +
                "font-family='Arial' font-size='18' fill='#9ca3af'>No Photo</text>" +
                "</svg>");
        }
    }
}