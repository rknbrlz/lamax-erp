using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class OrderDelete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindOrdersGrid();
        }

        private void BindOrdersGrid()
        {
            using (SqlConnection conn = new SqlConnection("Your_Connection_String"))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT TOP (1000) Marketplace, OrderNumber, BuyerFullName, Country
                  FROM dbo.T_OrdersPage", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvOrders.DataSource = dt;
                gvOrders.DataBind();
            }
        }
    }
}