using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace Feniks.Administrator
{
    public class ProductPhoto : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Buffer = true;

            string sku = context.Request.QueryString["sku"];
            if (string.IsNullOrWhiteSpace(sku))
            {
                WritePlaceholder(context);
                return;
            }

            byte[] photoBytes = null;
            string contentType = null;

            string cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string sql = @"SELECT TOP 1 Photo, ContentType
                               FROM dbo.T_Product
                               WHERE SKU = @SKU";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@SKU", sku);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (dr["Photo"] != DBNull.Value)
                                photoBytes = (byte[])dr["Photo"];

                            if (dr["ContentType"] != DBNull.Value)
                                contentType = dr["ContentType"].ToString();
                        }
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
            context.Response.End();
        }

        private void WritePlaceholder(HttpContext context)
        {
            context.Response.ContentType = "image/svg+xml";
            context.Response.Write(
                "<svg xmlns='http://www.w3.org/2000/svg' width='90' height='90'>" +
                "<rect width='100%' height='100%' fill='#f2f2f2'/>" +
                "<text x='50%' y='50%' dominant-baseline='middle' text-anchor='middle' fill='#999' font-size='12'>No Photo</text>" +
                "</svg>"
            );
        }

        public bool IsReusable => true;
    }
}
