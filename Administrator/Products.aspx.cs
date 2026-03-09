using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class Products : System.Web.UI.Page
    {
        String strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        public string query, constr;
        public SqlCommand com;
        public SqlConnection con;

        public void connection()
        {

            constr = ConfigurationManager.ConnectionStrings["constr"].ToString();
            con = new SqlConnection(constr);
            con.Open();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.AppendHeader("Refresh", "60");

            //Response.AppendHeader("Refresh", "300");

            if (!this.IsPostBack)
            {
                BindData();
                FillGrid();

                DataTable dt = this.FillGrid();
                gvProduct.DataSource = dt;
                gvProduct.DataBind();
            }

        }
        private void BindData()
        {
            //if (this.Page.User.Identity.IsAuthenticated)
            //{
            //    string username = this.Page.User.Identity.Name;
            //    lblLoginName.Text = username;
            //}

            //string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(strConnString))
            //{
            //    using (SqlCommand cmd = new SqlCommand("SELECT * FROM V_Product order by ProductID desc"))
            //    {
            //        using (SqlDataAdapter sda = new SqlDataAdapter())
            //        {
            //            cmd.Connection = con;
            //            sda.SelectCommand = cmd;
            //            using (DataTable dt = new DataTable())
            //            {
            //                sda.Fill(dt);
            //                gvProduct.DataSource = dt;
            //                gvProduct.DataBind();
            //            }
            //        }
            //    }
            //}
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT ProductType FROM T_ProductType Order by ProductType asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddProductTypeFilter.DataSource = cmd.ExecuteReader();
                        ddProductTypeFilter.DataTextField = "ProductType";
                        ddProductTypeFilter.DataValueField = "ProductType";
                        ddProductTypeFilter.DataBind();
                        con.Close();
                    }
                }
                ddProductTypeFilter.Items.Insert(0, new ListItem("--All--", ""));
            }
        }
        private DataTable FillGrid()
        {
            //DataTable dt = new DataTable();
            //string constr = ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString;
            //string sql = "SELECT * FROM V_Books";
            //using (SqlConnection conn = new SqlConnection(constr))
            // {
            //    using (SqlCommand cmd = new SqlCommand(sql))
            //    {
            //        cmd.Connection = conn;
            //        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
            //        {
            //            sda.Fill(dt);
            //        }
            //    }
            // }



            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_Product order by ProductID asc", con))
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    gvProduct.DataSource = dt;
                    gvProduct.DataBind();

                    return dt;
                }
            }

        }
        protected void ddProductTypeFilter_Changed(object sender, EventArgs e)
        {
            string ProductTypex = ddProductTypeFilter.SelectedItem.Value;
            DataTable dt = this.FillGrid();
            DataView dataView = dt.DefaultView;
            if (!string.IsNullOrEmpty(ProductTypex))
            {
                dataView.RowFilter = "ProductType = '" + ProductTypex + "'";
            }
            gvProduct.DataSource = dataView;
            gvProduct.DataBind();
        }
        protected void gvProduct_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            FillGrid();
            gvProduct.PageIndex = e.NewPageIndex;
            gvProduct.DataBind();
        }
        protected void gvProduct_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }
                }

                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;
                    if (!Convert.IsDBNull(dr["Photo"]))
                    {
                        string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr["Photo"]);
                        (e.Row.FindControl("ProductPhoto") as Image).ImageUrl = imageUrl;
                    }
                }
            }
        }
        protected void btnNewProduct_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/NewProduct.aspx");
        }
        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }

        protected void btnPhotoAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/PhotoAddforProduct.aspx");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Products.aspx");
        }
    }
}