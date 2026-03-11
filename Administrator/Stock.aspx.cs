using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class Stock : Page
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
                BindLocations();
                BindTransferLocations();
                BindGrid();
            }
        }

        private void BindProductTypes()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT ProductTypeID, ProductType FROM dbo.T_ProductType ORDER BY ProductType", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlProductType.DataSource = dt;
                ddlProductType.DataTextField = "ProductType";
                ddlProductType.DataValueField = "ProductTypeID";
                ddlProductType.DataBind();
                ddlProductType.Items.Insert(0, new ListItem("All", ""));
            }
        }

        private void BindLocations()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT LocationID, LocationName FROM dbo.T_StockLocation WHERE IsActive = 1 ORDER BY LocationName", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlLocation.DataSource = dt;
                ddlLocation.DataTextField = "LocationName";
                ddlLocation.DataValueField = "LocationID";
                ddlLocation.DataBind();
                ddlLocation.Items.Insert(0, new ListItem("All", ""));
            }
        }

        private void BindTransferLocations()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("SELECT LocationID, LocationName FROM dbo.T_StockLocation WHERE IsActive = 1 ORDER BY LocationName", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlTransferLocation.DataSource = dt;
                ddlTransferLocation.DataTextField = "LocationName";
                ddlTransferLocation.DataValueField = "LocationID";
                ddlTransferLocation.DataBind();
                ddlTransferLocation.Items.Insert(0, new ListItem("Select", ""));
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Stock_Search", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SKUContains",
                    string.IsNullOrWhiteSpace(txtSKU.Text) ? (object)DBNull.Value : txtSKU.Text.Trim());

                cmd.Parameters.AddWithValue("@StockMode",
                    string.IsNullOrWhiteSpace(ddlStockMode.SelectedValue) ? (object)DBNull.Value : ddlStockMode.SelectedValue);

                cmd.Parameters.AddWithValue("@ProductTypeID",
                    string.IsNullOrWhiteSpace(ddlProductType.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlProductType.SelectedValue));

                cmd.Parameters.AddWithValue("@LocationID",
                    string.IsNullOrWhiteSpace(ddlLocation.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlLocation.SelectedValue));

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvStock.DataSource = dt;
                    gvStock.DataBind();

                    litCount.Text = "<div class='muted-count'>Rows: " + dt.Rows.Count + "</div>";
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ClearMessage();
            pnlOperation.Visible = false;
            BindGrid();
        }

        protected void gvStock_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rowIndex;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out rowIndex))
                return;

            GridViewRow row = gvStock.Rows[rowIndex];

            int variantId = Convert.ToInt32(gvStock.DataKeys[rowIndex].Values["VariantID"]);
            int locationId = Convert.ToInt32(gvStock.DataKeys[rowIndex].Values["LocationID"]);
            string productSku = Convert.ToString(gvStock.DataKeys[rowIndex].Values["ProductSKU"]);
            string variantSku = Convert.ToString(gvStock.DataKeys[rowIndex].Values["VariantSKU"]);
            string variantName = Convert.ToString(gvStock.DataKeys[rowIndex].Values["VariantName"]);
            string locationName = Convert.ToString(gvStock.DataKeys[rowIndex].Values["LocationName"]);

            OpenOperationPanel(
                e.CommandName,
                variantId,
                locationId,
                productSku,
                string.IsNullOrWhiteSpace(variantName) ? variantSku : variantName,
                locationName
            );
        }

        private void OpenOperationPanel(string commandName, int variantId, int locationId, string productSku, string variantName, string locationName)
        {
            hfVariantID.Value = variantId.ToString();
            hfLocationID.Value = locationId.ToString();

            txtOpProductSku.Text = productSku;
            txtOpVariant.Text = variantName;
            txtOpLocation.Text = locationName;

            txtQty.Text = "1";
            txtUnitCost.Text = "";
            txtRefNo.Text = "";
            txtNote.Text = "";
            txtCurrency.Text = "PLN";
            ddlTransferLocation.SelectedIndex = 0;

            pnlOperation.Visible = true;

            if (commandName == "OpenReceive")
            {
                hfAction.Value = "RECEIVE";
                litOpTitle.Text = "Receive Stock";
                ddlTransferLocation.Enabled = false;
            }
            else if (commandName == "OpenAdjust")
            {
                hfAction.Value = "ADJUST";
                litOpTitle.Text = "Adjust Stock";
                ddlTransferLocation.Enabled = false;
            }
            else if (commandName == "OpenTransfer")
            {
                hfAction.Value = "TRANSFER";
                litOpTitle.Text = "Transfer Stock";
                ddlTransferLocation.Enabled = true;
            }
        }

        protected void btnSaveOperation_Click(object sender, EventArgs e)
        {
            ClearMessage();

            try
            {
                int variantId = Convert.ToInt32(hfVariantID.Value);
                int locationId = Convert.ToInt32(hfLocationID.Value);
                decimal qty = ParseDecimal(txtQty.Text);
                decimal? unitCost = string.IsNullOrWhiteSpace(txtUnitCost.Text) ? (decimal?)null : ParseDecimal(txtUnitCost.Text);
                string currency = string.IsNullOrWhiteSpace(txtCurrency.Text) ? null : txtCurrency.Text.Trim().ToUpper();
                string refNo = string.IsNullOrWhiteSpace(txtRefNo.Text) ? null : txtRefNo.Text.Trim();
                string note = string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim();
                string createdBy = string.IsNullOrWhiteSpace(User.Identity.Name) ? "system" : User.Identity.Name;

                if (hfAction.Value == "RECEIVE")
                {
                    ExecuteReceive(variantId, locationId, qty, unitCost, currency, refNo, note, createdBy);
                    ShowSuccess("Stock received successfully.");
                }
                else if (hfAction.Value == "ADJUST")
                {
                    ExecuteAdjust(variantId, locationId, qty, unitCost, currency, refNo, note, createdBy);
                    ShowSuccess("Stock adjusted successfully.");
                }
                else if (hfAction.Value == "TRANSFER")
                {
                    if (string.IsNullOrWhiteSpace(ddlTransferLocation.SelectedValue))
                        throw new Exception("Please select target location.");

                    int toLocationId = Convert.ToInt32(ddlTransferLocation.SelectedValue);
                    ExecuteTransfer(variantId, locationId, toLocationId, qty, refNo, note, createdBy);
                    ShowSuccess("Stock transferred successfully.");
                }

                pnlOperation.Visible = false;
                BindGrid();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        protected void btnCancelOperation_Click(object sender, EventArgs e)
        {
            pnlOperation.Visible = false;
            ClearMessage();
        }

        private void ExecuteReceive(int variantId, int locationId, decimal qty, decimal? unitCost, string currency, string refNo, string note, string createdBy)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Stock_Receive", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VariantID", variantId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@UnitCost", (object)unitCost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Currency", (object)currency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RefNo", (object)refNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object)createdBy ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ExecuteAdjust(int variantId, int locationId, decimal qtyDelta, decimal? unitCost, string currency, string refNo, string note, string createdBy)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Stock_Adjust", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VariantID", variantId);
                cmd.Parameters.AddWithValue("@LocationID", locationId);
                cmd.Parameters.AddWithValue("@QtyDelta", qtyDelta);
                cmd.Parameters.AddWithValue("@UnitCost", (object)unitCost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Currency", (object)currency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RefNo", (object)refNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object)createdBy ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ExecuteTransfer(int variantId, int fromLocationId, int toLocationId, decimal qty, string refNo, string note, string createdBy)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Stock_Transfer", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VariantID", variantId);
                cmd.Parameters.AddWithValue("@FromLocationID", fromLocationId);
                cmd.Parameters.AddWithValue("@ToLocationID", toLocationId);
                cmd.Parameters.AddWithValue("@Qty", qty);
                cmd.Parameters.AddWithValue("@RefNo", (object)refNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Note", (object)note ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", (object)createdBy ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private decimal ParseDecimal(string value)
        {
            decimal result;
            string normalized = (value ?? "").Trim().Replace(",", ".");
            if (!decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                throw new Exception("Qty / amount is not valid.");

            return result;
        }

        public string GetModeBadge(object stockModeObj)
        {
            string mode = Convert.ToString(stockModeObj);

            if (mode == "S")
                return "<span class='mode-pill mode-s'>S</span>";

            if (mode == "A")
                return "<span class='mode-pill mode-a'>A</span>";

            return "<span class='mode-pill mode-n'>N</span>";
        }

        public string GetAvailableHtml(object availableObj)
        {
            decimal qty = 0;
            decimal.TryParse(Convert.ToString(availableObj), out qty);

            if (qty < 0)
                return "<span class='qty-neg'>" + qty.ToString("0.####") + "</span>";

            if (qty > 0)
                return "<span class='qty-ok'>" + qty.ToString("0.####") + "</span>";

            return "<span class='qty-zero'>0</span>";
        }

        private void ShowSuccess(string message)
        {
            litMessage.Text = "<div class='msg-ok'>" + Server.HtmlEncode(message) + "</div>";
        }

        private void ShowError(string message)
        {
            litMessage.Text = "<div class='msg-err'>" + Server.HtmlEncode(message) + "</div>";
        }

        private void ClearMessage()
        {
            litMessage.Text = "";
        }
    }
}