using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Feniks
{
    public class ProductImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();

            int imageId;
            if (!int.TryParse(context.Request.QueryString["id"], out imageId) || imageId <= 0)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Invalid image id");
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductImage_Get", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductImageID", imageId);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (!dr.Read())
                        {
                            context.Response.StatusCode = 404;
                            context.Response.ContentType = "text/plain";
                            context.Response.Write("Image not found");
                            return;
                        }

                        if (dr["ImageData"] == DBNull.Value)
                        {
                            context.Response.StatusCode = 404;
                            context.Response.ContentType = "text/plain";
                            context.Response.Write("Image data not found");
                            return;
                        }

                        byte[] bytes = (byte[])dr["ImageData"];
                        string contentType = "image/jpeg";

                        if (dr["ContentType"] != DBNull.Value)
                        {
                            string dbType = Convert.ToString(dr["ContentType"]);
                            if (!string.IsNullOrWhiteSpace(dbType) &&
                                dbType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                            {
                                contentType = dbType;
                            }
                        }

                        context.Response.Buffer = true;
                        context.Response.ContentType = contentType;
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
                        context.Response.BinaryWrite(bytes);
                        context.Response.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                context.Response.Write("Handler error: " + ex.Message);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}