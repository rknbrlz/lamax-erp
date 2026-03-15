using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Feniks
{
    public class ProductAmazonMainImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string sku = (context.Request.QueryString["sku"] ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(sku))
            {
                context.Response.StatusCode = 400;
                context.Response.Write("SKU is required.");
                return;
            }

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand("dbo.SP_ProductAmazonMainImage_GetBySKU", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SKU", sku);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        string contentType = dr["ContentType"] == DBNull.Value
                            ? "image/jpeg"
                            : Convert.ToString(dr["ContentType"]);

                        byte[] bytes = dr["Photo"] as byte[];

                        if (bytes == null || bytes.Length == 0)
                        {
                            context.Response.StatusCode = 404;
                            context.Response.Write("Image data not found.");
                            return;
                        }

                        context.Response.Clear();
                        context.Response.ContentType = contentType;
                        context.Response.BinaryWrite(bytes);
                        context.Response.End();
                        return;
                    }
                }
            }

            context.Response.StatusCode = 404;
            context.Response.Write("Image not found.");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}