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
                        ddlProductType.Items.Add(
                            new ListItem(
                                Convert.ToString(dr["ProductType"]),
                                Convert.ToString(dr["ProductTypeID"])
                            )
                        );
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

                cmd.Parameters.AddWithValue(
                    "@SKUContains",
                    string.IsNullOrWhiteSpace(txtSKU.Text)
                        ? (object)DBNull.Value
                        : txtSKU.Text.Trim()
                );

                cmd.Parameters.AddWithValue(
                    "@ProductTypeID",
                    string.IsNullOrWhiteSpace(ddlProductType.SelectedValue)
                        ? (object)DBNull.Value
                        : ddlProductType.SelectedValue
                );

                cmd.Parameters.AddWithValue(
                    "@StockMode",
                    string.IsNullOrWhiteSpace(ddlStockMode.SelectedValue)
                        ? (object)DBNull.Value
                        : ddlStockMode.SelectedValue
                );

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvProducts.DataSource = dt;
                gvProducts.DataBind();

                litResultCount.Text = dt.Rows.Count.ToString("N0");
                BindKpis(dt);
            }
        }

        private void BindKpis(DataTable dt)
        {
            litKpiProducts.Text = dt.Rows.Count.ToString("N0");
            litKpiStockQty.Text = SumColumn(dt, "StockQty").ToString("N0");
            litKpiSalesQty.Text = SumColumn(dt, "SalesQty").ToString("N0");
            litKpiAvgPrice.Text = AvgColumn(dt, "UnitPrice").ToString("N2");
        }

        private decimal SumColumn(DataTable dt, string columnName)
        {
            decimal total = 0m;

            if (dt == null || !dt.Columns.Contains(columnName))
                return total;

            foreach (DataRow row in dt.Rows)
            {
                total += SafeToDecimal(row[columnName]);
            }

            return total;
        }

        private decimal AvgColumn(DataTable dt, string columnName)
        {
            decimal total = 0m;
            int count = 0;

            if (dt == null || !dt.Columns.Contains(columnName))
                return 0m;

            foreach (DataRow row in dt.Rows)
            {
                total += SafeToDecimal(row[columnName]);
                count++;
            }

            if (count == 0)
                return 0m;

            return total / count;
        }

        private decimal SafeToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            decimal result;
            return decimal.TryParse(Convert.ToString(value), out result) ? result : 0m;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSKU.Text = string.Empty;

            if (ddlProductType.Items.Count > 0)
                ddlProductType.SelectedIndex = 0;

            if (ddlStockMode.Items.Count > 0)
                ddlStockMode.SelectedIndex = 0;

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

            Label lblMode = e.Row.FindControl("lblMode") as Label;
            if (lblMode != null)
            {
                string mode = (lblMode.Text ?? string.Empty).Trim().ToUpperInvariant();

                if (mode == "S")
                {
                    lblMode.CssClass = "mode-badge mode-s";
                    lblMode.Text = "S";
                }
                else if (mode == "A")
                {
                    lblMode.CssClass = "mode-badge mode-a";
                    lblMode.Text = "A";
                }
                else
                {
                    lblMode.CssClass = "mode-badge mode-n";
                    lblMode.Text = "N";
                }
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