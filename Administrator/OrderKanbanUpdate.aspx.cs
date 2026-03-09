using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class OrderKanbanUpdate : Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "text/plain";

            try
            {
                string orderIdRaw = Request.Form["orderId"];
                string status = (Request.Form["status"] ?? "").Trim().ToUpper();

                int orderId;
                if (!int.TryParse(orderIdRaw, out orderId))
                {
                    Response.StatusCode = 400;
                    Response.Write("Invalid orderId");
                    return;
                }

                if (status != "OPEN" && status != "INPROGRESS" && status != "CLOSED")
                {
                    Response.StatusCode = 400;
                    Response.Write("Invalid status");
                    return;
                }

                using (SqlConnection con = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand("dbo.usp_OrderKanban_UpdateStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@KanbanStatus", status);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                Response.Write("OK");
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                Response.Write("ERROR: " + ex.Message);
            }
        }
    }
}