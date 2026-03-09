using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class OrderCreate : System.Web.UI.Page
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        // 0 chk | 1 photo | 2 product type | 3 sku | 4 ring size | 5 qty | 6 price
        private const int COL_RING_SIZE = 4;

        // T_RingSize: "No Ring" = 14
        private const int NO_RING_SIZE_ID = 14;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.Page.User.Identity.IsAuthenticated)
                    lblLoginName.Text = this.Page.User.Identity.Name;

                BindDropdowns();
                BindProducts();
                BindAdded();
                HideMessage();

                UpdateNextButtonState();
            }
            else
            {
                UpdateNextButtonState();
            }
        }

        // -------------------- UI MESSAGE --------------------

        private void ShowMessage(string html, string bootstrapClass)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "alert " + bootstrapClass;
            lblMsg.Text = html;
        }

        private void HideMessage()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }

        // -------------------- BINDING --------------------

        private void BindDropdowns()
        {
            BindDropDown(ddMarketplaces, "SELECT MarketplaceID, Marketplace FROM T_Marketplace ORDER BY Marketplace", "Marketplace", "MarketplaceID", addChoose: true);
            BindDropDown(ddCountry, "SELECT CountryID, Country FROM T_Country ORDER BY Country", "Country", "CountryID", addChoose: true);
            BindDropDown(ddState, "SELECT StateID, State FROM V_State ORDER BY State", "State", "StateID", addChoose: true);

            BindDropDown(ddCurrency, "SELECT CurrencyID, Currency FROM T_Currency ORDER BY CurrencyID", "Currency", "CurrencyID", addChoose: false);
            SelectIfExists(ddCurrency, "USD");

            BindDropDown(ddShippingType, "SELECT ShippingTypeID, ShippingType FROM T_ShippingType ORDER BY ShippingTypeID", "ShippingType", "ShippingTypeID", addChoose: false);
            SelectIfExists(ddShippingType, "Standard");
        }

        private void BindDropDown(DropDownList ddl, string sql, string textField, string valueField, bool addChoose)
        {
            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                ddl.DataSource = cmd.ExecuteReader();
                ddl.DataTextField = textField;
                ddl.DataValueField = valueField;
                ddl.DataBind();
            }

            if (addChoose)
                ddl.Items.Insert(0, new ListItem("--- please choose ---", "0"));
        }

        private void SelectIfExists(DropDownList ddl, string text)
        {
            var item = ddl.Items.FindByText(text);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private void BindProducts()
        {
            HideMessage();

            string sku = (txtFilterSKU.Text ?? "").Trim();

            string sql = @"
SELECT * 
FROM V_Product
WHERE (@SKU = '' OR SKU LIKE '%'+@SKU+'%')
ORDER BY SKU ASC";

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                var dt = new DataTable();
                da.Fill(dt);

                gvProducts.DataSource = dt;
                gvProducts.DataBind();

                // hiç Ring yoksa RingSize kolonunu header dahil gizle
                bool anyRing = false;
                if (dt.Columns.Contains("ProductType"))
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        string pt = (r["ProductType"] ?? "").ToString();
                        if (IsRingProduct(pt)) { anyRing = true; break; }
                    }
                }

                if (gvProducts.Columns.Count > COL_RING_SIZE)
                    gvProducts.Columns[COL_RING_SIZE].Visible = anyRing;
            }
        }

        private void BindAdded()
        {
            string orderNo = (txtOrderNumber.Text ?? "").Trim();

            string sql = @"
SELECT * 
FROM V_OrderCreationDynamic
WHERE OrderNumber = @OrderNumber
ORDER BY OrderCreationDynamicID DESC";

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@OrderNumber", orderNo);
                var dt = new DataTable();
                da.Fill(dt);

                gvAdded.DataSource = dt;
                gvAdded.DataBind();
            }

            UpdateNextButtonState();
        }

        private DataTable GetRingSizes()
        {
            if (Session["RingSizesDT"] is DataTable cached) return cached;

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("SELECT RingSizeID, RingSize FROM T_RingSize ORDER BY RingSizeID", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                Session["RingSizesDT"] = dt;
                return dt;
            }
        }

        // -------------------- NEXT BUTTON --------------------

        private void UpdateNextButtonState()
        {
            bool hasOrderNumber = !string.IsNullOrWhiteSpace(txtOrderNumber.Text);
            bool hasAtLeastOneItem = (gvAdded != null && gvAdded.Rows.Count > 0);

            btnGoStep2.Enabled = (hasOrderNumber && hasAtLeastOneItem);
            btnGoStep2.CssClass = btnGoStep2.Enabled ? "btn btn-success" : "btn btn-success disabled";
        }

        // -------------------- HELPERS --------------------

        private bool IsRingProduct(string productType)
        {
            string t = (productType ?? "").Trim().ToLowerInvariant();
            return t.Contains("ring");
        }

        private int SafeInt(string s)
        {
            int.TryParse((s ?? "").Trim(), out int v);
            return v;
        }

        private decimal SafeDec(string s)
        {
            string x = (s ?? "").Trim().Replace(",", ".");
            decimal.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal v);
            return v;
        }

        // -------------------- NAV BUTTONS --------------------

        protected void btnBack_Click(object sender, EventArgs e) => Response.Redirect("~/Administrator/Orders.aspx");
        protected void btnMainMenu_Click(object sender, EventArgs e) => Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        protected void btnOrdersList_Click(object sender, EventArgs e) => Response.Redirect("~/Administrator/OrdersList.aspx");

        // -------------------- EVENTS --------------------

        protected void txtOrderNumber_TextChanged(object sender, EventArgs e)
        {
            BindAdded();
            UpdateNextButtonState();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvProducts.PageIndex = 0;
            BindProducts();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtFilterSKU.Text = "";
            gvProducts.PageIndex = 0;
            BindProducts();
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProducts.PageIndex = e.NewPageIndex;
            BindProducts();
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var dr = (DataRowView)e.Row.DataItem;

            if (dr.Row.Table.Columns.Contains("Photo") && !Convert.IsDBNull(dr["Photo"]))
            {
                string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr["Photo"]);
                (e.Row.FindControl("imgPhoto") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;
            }

            var dd = e.Row.FindControl("ddRingSize") as DropDownList;
            if (dd != null && dd.Items.Count == 0)
            {
                dd.DataSource = GetRingSizes();
                dd.DataTextField = "RingSize";
                dd.DataValueField = "RingSizeID";
                dd.DataBind();
                dd.Items.Insert(0, new ListItem("--Please Select--", "0"));
            }

            string productType = gvProducts.DataKeys[e.Row.RowIndex]["ProductType"]?.ToString();
            bool isRing = IsRingProduct(productType);

            if (!isRing)
            {
                if (dd != null)
                {
                    dd.Enabled = false;
                    var item = dd.Items.FindByValue(NO_RING_SIZE_ID.ToString());
                    if (item != null)
                    {
                        dd.ClearSelection();
                        item.Selected = true;
                    }
                }

                // karışık listede ring olmayan satırın ring size hücresini gizle
                if (gvProducts.Columns.Count > COL_RING_SIZE)
                    e.Row.Cells[COL_RING_SIZE].Style["display"] = "none";
            }
        }

        protected void btnAddSelected_Click(object sender, EventArgs e)
        {
            HideMessage();

            string orderNo = (txtOrderNumber.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(orderNo))
            {
                ShowMessage("Order Number boş olamaz.", "alert-warning");
                UpdateNextButtonState();
                return;
            }

            int addedCount = 0;
            int skippedCount = 0;
            var warn = new StringBuilder();

            foreach (GridViewRow row in gvProducts.Rows)
            {
                var chk = row.FindControl("chkRow") as CheckBox;
                if (chk == null || !chk.Checked) continue;

                string sku = gvProducts.DataKeys[row.RowIndex]["SKU"]?.ToString();
                string productType = gvProducts.DataKeys[row.RowIndex]["ProductType"]?.ToString();
                int productTypeId = SafeInt(gvProducts.DataKeys[row.RowIndex]["ProductTypeID"]?.ToString());

                bool isRing = IsRingProduct(productType);

                var ddRing = row.FindControl("ddRingSize") as DropDownList;
                var txtQty = row.FindControl("txtQty") as TextBox;
                var txtPrice = row.FindControl("txtItemPrice") as TextBox;

                int selectedRingSizeId = SafeInt(ddRing?.SelectedValue);
                decimal qty = SafeDec(txtQty?.Text);
                decimal price = SafeDec(txtPrice?.Text);

                if (qty <= 0)
                {
                    skippedCount++;
                    warn.Append($"<div>SKU <b>{sku}</b>: Qty 0 olamaz.</div>");
                    continue;
                }

                if (isRing && selectedRingSizeId == 0)
                {
                    skippedCount++;
                    warn.Append($"<div>SKU <b>{sku}</b> (Ring): Ring Size seçmelisin.</div>");
                    continue;
                }

                int finalRingSizeId = isRing ? selectedRingSizeId : NO_RING_SIZE_ID;

                InsertDynamicLine(orderNo, sku, productTypeId, finalRingSizeId, qty, price);

                addedCount++;
                chk.Checked = false;
            }

            BindAdded();
            UpdateNextButtonState();

            if (addedCount > 0 && skippedCount == 0)
                ShowMessage($"<b>{addedCount}</b> ürün eklendi.", "alert-success");
            else if (addedCount > 0 && skippedCount > 0)
                ShowMessage($"<b>{addedCount}</b> ürün eklendi, <b>{skippedCount}</b> ürün atlandı.<hr/>{warn}", "alert-warning");
            else if (addedCount == 0 && skippedCount > 0)
                ShowMessage($"Ürün eklenemedi. Kontrol et:<hr/>{warn}", "alert-warning");
        }

        private void InsertDynamicLine(string orderNumber, string sku, int productTypeId, int ringSizeId, decimal qty, decimal price)
        {
            string sql = @"
IF EXISTS (
    SELECT 1 FROM T_OrderCreationDynamic
    WHERE OrderNumber=@OrderNumber
      AND SKU=@SKU
      AND RingSizeID = @RingSizeID
)
BEGIN
    UPDATE T_OrderCreationDynamic
    SET Quantity = Quantity + @Quantity,
        ItemPrice = @ItemPrice
    WHERE OrderNumber=@OrderNumber
      AND SKU=@SKU
      AND RingSizeID = @RingSizeID
END
ELSE
BEGIN
    INSERT INTO T_OrderCreationDynamic (OrderNumber, SKU, ProductTypeID, RingSizeID, Quantity, ItemPrice)
    VALUES (@OrderNumber, @SKU, @ProductTypeID, @RingSizeID, @Quantity, @ItemPrice)
END";

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@ProductTypeID", productTypeId);
                cmd.Parameters.AddWithValue("@RingSizeID", ringSizeId);
                cmd.Parameters.AddWithValue("@Quantity", qty);
                cmd.Parameters.AddWithValue("@ItemPrice", price);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void gvAdded_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteRow") return;

            int id = SafeInt(e.CommandArgument?.ToString());
            if (id <= 0) return;

            using (var con = new SqlConnection(_cs))
            using (var cmd = new SqlCommand("DELETE FROM T_OrderCreationDynamic WHERE OrderCreationDynamicID=@ID", con))
            {
                cmd.Parameters.AddWithValue("@ID", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            BindAdded();
            UpdateNextButtonState();
        }

        protected void btnGoStep2_Click(object sender, EventArgs e)
        {
            UpdateNextButtonState();
            if (!btnGoStep2.Enabled)
            {
                ShowMessage("Next için: Order Number dolu olmalı ve en az 1 ürün eklenmeli.", "alert-info");
                return;
            }

            pnlStep2.Visible = true;
            HideMessage();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            HideMessage();

            UpdateNextButtonState();
            if (!btnGoStep2.Enabled)
            {
                ShowMessage("Kaydetmek için: Order Number dolu olmalı ve en az 1 ürün eklenmeli.", "alert-warning");
                return;
            }

            using (var con = new SqlConnection(_cs))
            {
                con.Open();
                using (var tr = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(@"
INSERT INTO T_Order
(OrderNumber,OrderDate,MarketplaceID,ShipTo,BuyerFullName,CountryID,StateID,Email,PhoneNumber,
 RecordDate,RecordBy,CoupunPrice,ShippingPrice,Tax,GiftWrapPrice,GiftMessage,BuyerNotes,SellerNotes,Currency,ShippingTypeID)
VALUES
(@OrderNumber,@OrderDate,@MarketplaceID,@ShipTo,@BuyerFullName,@CountryID,@StateID,@Email,@PhoneNumber,
 @RecordDate,@RecordBy,@CouponPrice,@ShippingPrice,@Tax,@GiftWrapPrice,@GiftMessage,@BuyerNotes,@SellerNotes,@Currency,@ShippingTypeID)
", con, tr))
                        {
                            cmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@OrderDate", txtOrderDate.Text);
                            cmd.Parameters.AddWithValue("@MarketplaceID", ddMarketplaces.SelectedValue);
                            cmd.Parameters.AddWithValue("@ShipTo", txtShipTo.Text);
                            cmd.Parameters.AddWithValue("@BuyerFullName", txtBuyerName.Text);
                            cmd.Parameters.AddWithValue("@CountryID", ddCountry.SelectedValue);
                            cmd.Parameters.AddWithValue("@StateID", ddState.SelectedValue);
                            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                            cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text);
                            cmd.Parameters.AddWithValue("@RecordDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@RecordBy", lblLoginName.Text);

                            cmd.Parameters.AddWithValue("@CouponPrice", SafeDec(txtCouponPrice.Text));
                            cmd.Parameters.AddWithValue("@ShippingPrice", SafeDec(txtShippingPrice.Text));
                            cmd.Parameters.AddWithValue("@Tax", SafeDec(txtTax.Text));
                            cmd.Parameters.AddWithValue("@GiftWrapPrice", SafeDec(txtGiftWrapPrice.Text));
                            cmd.Parameters.AddWithValue("@GiftMessage", txtGiftMessage.Text);
                            cmd.Parameters.AddWithValue("@BuyerNotes", txtBuyerNotes.Text);
                            cmd.Parameters.AddWithValue("@SellerNotes", txtSellerNotes.Text);
                            cmd.Parameters.AddWithValue("@Currency", ddCurrency.SelectedValue);
                            cmd.Parameters.AddWithValue("@ShippingTypeID", ddShippingType.SelectedValue);

                            cmd.ExecuteNonQuery();
                        }

                        ExecSp(con, tr, "InsertOrderDetailsData");
                        ExecSp(con, tr, "OrderTableMainDataUpdateNew");
                        ExecSp(con, tr, "OrderInsertTOShippingTable");
                        ExecSp(con, tr, "RefreshOrdersPage");

                        using (var cmd = new SqlCommand("INSERT INTO T_PackExpenses (OrderNumber) VALUES(@OrderNumber)", con, tr))
                        {
                            cmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }

                        tr.Commit();
                        Response.Redirect("~/Administrator/OrderList.aspx", true);
                    }
                    catch (Exception ex)
                    {
                        tr.Rollback();
                        ShowMessage("Save sırasında hata oluştu: " + Server.HtmlEncode(ex.Message), "alert-danger");
                    }
                }
            }
        }

        private void ExecSp(SqlConnection con, SqlTransaction tr, string spName)
        {
            using (var cmd = new SqlCommand(spName, con, tr))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/OrderList.aspx");
        }
    }
}