using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class ProductEdit : Page
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        private int ProductID
        {
            get
            {
                int id;
                return int.TryParse(hfProductID.Value, out id) ? id : 0;
            }
            set
            {
                hfProductID.Value = value.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProductTypes();
                BindStockModes();

                int queryId;
                if (int.TryParse(Request.QueryString["ProductID"], out queryId) && queryId > 0)
                {
                    ProductID = queryId;
                    LoadProduct(queryId);
                    litPageTitle.Text = "Edit Product";
                }
                else
                {
                    ProductID = 0;
                    litPageTitle.Text = "New Product";
                    ApplyStockModeRule();
                }
            }
        }

        private void BindProductTypes()
        {
            ddlProductType.Items.Clear();
            ddlProductType.Items.Add(new ListItem("-- Select --", ""));

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

        private void BindStockModes()
        {
            ddlStockMode.Items.Clear();
            ddlStockMode.Items.Add(new ListItem("Sized Ring (S)", "S"));
            ddlStockMode.Items.Add(new ListItem("Adjustable Ring (A)", "A"));
            ddlStockMode.Items.Add(new ListItem("Normal (N)", "N"));
        }

        private void LoadProduct(int productId)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Product_Get", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductID", productId);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        txtSKU.Text = Convert.ToString(dr["SKU"]);

                        string productTypeId = Convert.ToString(dr["ProductTypeID"]);
                        if (ddlProductType.Items.FindByValue(productTypeId) != null)
                            ddlProductType.SelectedValue = productTypeId;

                        ApplyStockModeRule();

                        string stockMode = Convert.ToString(dr["StockMode"]);
                        if (ddlStockMode.Items.FindByValue(stockMode) != null)
                            ddlStockMode.SelectedValue = stockMode;

                        txtStockAddress.Text = Convert.ToString(dr["StockAddress"]);
                        txtSupplier.Text = Convert.ToString(dr["SupplierID"]);
                    }
                }
            }
        }

        protected void ddlProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyStockModeRule();
        }

        private void ApplyStockModeRule()
        {
            string productTypeText = ddlProductType.SelectedItem != null
                ? ddlProductType.SelectedItem.Text.Trim()
                : string.Empty;

            bool isRing = productTypeText.Equals("Ring", StringComparison.OrdinalIgnoreCase);

            ddlStockMode.Items.Clear();

            if (isRing)
            {
                ddlStockMode.Items.Add(new ListItem("Sized Ring (S)", "S"));
                ddlStockMode.Items.Add(new ListItem("Adjustable Ring (A)", "A"));
                ddlStockMode.Enabled = true;

                if (ddlStockMode.Items.FindByValue("S") != null)
                    ddlStockMode.SelectedValue = "S";
            }
            else
            {
                ddlStockMode.Items.Add(new ListItem("Normal (N)", "N"));
                ddlStockMode.SelectedValue = "N";
                ddlStockMode.Enabled = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = false;

            try
            {
                if (string.IsNullOrWhiteSpace(txtSKU.Text))
                {
                    ShowMessage("SKU is required.", true);
                    return;
                }

                if (string.IsNullOrWhiteSpace(ddlProductType.SelectedValue))
                {
                    ShowMessage("Product Type is required.", true);
                    return;
                }

                string productTypeText = ddlProductType.SelectedItem.Text.Trim();
                string stockMode = ddlStockMode.SelectedValue;

                if (productTypeText.Equals("Ring", StringComparison.OrdinalIgnoreCase))
                {
                    if (stockMode != "S" && stockMode != "A")
                    {
                        ShowMessage("Ring products must use S or A stock mode.", true);
                        return;
                    }
                }
                else
                {
                    stockMode = "N";
                }

                object result;

                using (SqlConnection con = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Product_Save", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@ProductID", ProductID == 0 ? (object)DBNull.Value : ProductID);
                    cmd.Parameters.AddWithValue("@SKU", txtSKU.Text.Trim());
                    cmd.Parameters.AddWithValue("@ProductTypeID", ddlProductType.SelectedValue);
                    cmd.Parameters.AddWithValue("@StockMode", stockMode);
                    cmd.Parameters.AddWithValue("@StockAddress",
                        string.IsNullOrWhiteSpace(txtStockAddress.Text) ? (object)DBNull.Value : txtStockAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@SupplierID",
                        string.IsNullOrWhiteSpace(txtSupplier.Text) ? (object)DBNull.Value : txtSupplier.Text.Trim());
                    cmd.Parameters.AddWithValue("@RecordBy", GetCurrentUserName());

                    con.Open();
                    result = cmd.ExecuteScalar();
                }

                int savedId = 0;
                if (result != null)
                    int.TryParse(result.ToString(), out savedId);

                Response.Redirect("ProductList.aspx?saved=1", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (SqlException ex)
            {
                ShowMessage(ex.Message, true);
            }
            catch (Exception ex)
            {
                ShowMessage("Unexpected error: " + ex.Message, true);
            }
        }

        private string GetCurrentUserName()
        {
            if (Context != null &&
                Context.User != null &&
                Context.User.Identity != null &&
                !string.IsNullOrWhiteSpace(Context.User.Identity.Name))
            {
                return Context.User.Identity.Name;
            }

            return "system";
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Visible = true;
            lblMessage.Text = message;
            lblMessage.CssClass = isError ? "alert alert-danger" : "alert alert-success";
        }
    }
}