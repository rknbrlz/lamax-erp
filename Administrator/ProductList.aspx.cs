using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class ProductList : Page
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProductTypes();
                BindGrid();

                if (!string.IsNullOrWhiteSpace(Request.QueryString["saved"]))
                {
                    ShowMessage("Product saved successfully.", false);
                }
            }
        }

        private void BindProductTypes()
        {
            ddlProductType.Items.Clear();
            ddlProductType.Items.Add(new ListItem("All", ""));

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_ProductType_List", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ddlProductType.Items.Add(new ListItem(
                            Convert.ToString(dr["ProductType"]),
                            Convert.ToString(dr["ProductTypeID"])
                        ));
                    }
                }
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Product_List", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SKUContains",
                    string.IsNullOrWhiteSpace(txtSKU.Text) ? (object)DBNull.Value : txtSKU.Text.Trim());

                cmd.Parameters.AddWithValue("@ProductTypeID",
                    string.IsNullOrWhiteSpace(ddlProductType.SelectedValue) ? (object)DBNull.Value : ddlProductType.SelectedValue);

                cmd.Parameters.AddWithValue("@StockMode",
                    string.IsNullOrWhiteSpace(ddlStockMode.SelectedValue) ? (object)DBNull.Value : ddlStockMode.SelectedValue);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvProducts.DataSource = dt;
                gvProducts.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditProduct")
            {
                Response.Redirect("ProductEdit.aspx?ProductID=" + e.CommandArgument, false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            Label lblMode = (Label)e.Row.FindControl("lblMode");
            if (lblMode == null)
                return;

            string mode = lblMode.Text.Trim().ToUpperInvariant();

            switch (mode)
            {
                case "S":
                    lblMode.CssClass = "mode-badge mode-s";
                    break;
                case "A":
                    lblMode.CssClass = "mode-badge mode-a";
                    break;
                default:
                    lblMode.CssClass = "mode-badge mode-n";
                    break;
            }
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Visible = true;
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
        }
    }
}