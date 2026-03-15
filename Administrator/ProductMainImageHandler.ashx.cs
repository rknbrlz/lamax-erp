using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace Feniks.Administrator
{
    public class ProductMainImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string sku = (context.Request["sku"] ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(sku))
            {
                ReturnPlaceholder(context);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1
                       Photo,
                       ContentType
                FROM dbo.T_ProductAmazonMainImage
                WHERE SKU = @SKU
                  AND IsActive = 1
                  AND Photo IS NOT NULL
                ORDER BY AmazonMainImageID DESC", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        byte[] bytes = dr["Photo"] as byte[];
                        string contentType = Convert.ToString(dr["ContentType"]);

                        if (bytes != null && bytes.Length > 0)
                        {
                            context.Response.Clear();
                            context.Response.Buffer = true;
                            context.Response.ContentType = string.IsNullOrWhiteSpace(contentType)
                                ? "image/jpeg"
                                : contentType;

                            context.Response.Cache.SetCacheability(HttpCacheability.Public);
                            context.Response.Cache.SetMaxAge(TimeSpan.FromMinutes(30));
                            context.Response.BinaryWrite(bytes);
                            return;
                        }
                    }
                }
            }

            ReturnPlaceholder(context);
        }

        private void ReturnPlaceholder(HttpContext context)
        {
            string filePath = context.Server.MapPath("~/Content/no-image.png");

            context.Response.Clear();
            context.Response.Buffer = true;

            if (File.Exists(filePath))
            {
                context.Response.ContentType = "image/png";
                context.Response.WriteFile(filePath);
            }
            else
            {
                context.Response.ContentType = "image/gif";
                byte[] gifBytes = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==");
                context.Response.BinaryWrite(gifBytes);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}