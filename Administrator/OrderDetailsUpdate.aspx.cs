using Feniks.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class OrderDetailsUpdate : System.Web.UI.Page
    {
        private readonly string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private const string UsdPlnRateTable = "dbo.T_FxUsdPln";

        private decimal _usdPlnRate = 0m;
        private DateTime _orderDate = DateTime.Today;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindShippingCompanies();
                BindShippingStatus();

                string orderNumber = Request.QueryString["OrderNumber"];
                if (!string.IsNullOrEmpty(orderNumber))
                {
                    LoadOrder(orderNumber);     // sets _orderDate and _usdPlnRate
                    LoadAddressAndContact(orderNumber);
                    LoadItems(orderNumber);
                    LoadNotes(orderNumber);

                    LoadShippingLegs(orderNumber);
                    LoadExpenses(orderNumber);

                    LoadFinancialSummary(orderNumber); // calculator based
                }
            }
        }

        // ---------------- binds ----------------

        private void BindShippingCompanies()
        {
            string sql = "SELECT ShippingCompanyID, ShippingCompany FROM T_ShippingCompany ORDER BY ShippingCompany";
            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                ddInterCompany.DataSource = dt;
                ddInterCompany.DataTextField = "ShippingCompany";
                ddInterCompany.DataValueField = "ShippingCompanyID";
                ddInterCompany.DataBind();
                ddInterCompany.Items.Insert(0, new ListItem("-- Select --", ""));

                ddMainCompany.DataSource = dt;
                ddMainCompany.DataTextField = "ShippingCompany";
                ddMainCompany.DataValueField = "ShippingCompanyID";
                ddMainCompany.DataBind();
                ddMainCompany.Items.Insert(0, new ListItem("-- Select --", ""));
            }
        }

        private void BindShippingStatus()
        {
            string sql = "SELECT ShippingStatusID, ShippingStatus FROM T_ShippingStatus ORDER BY ShippingStatusID";
            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                ddShippingStatus.DataSource = cmd.ExecuteReader();
                ddShippingStatus.DataTextField = "ShippingStatus";
                ddShippingStatus.DataValueField = "ShippingStatusID";
                ddShippingStatus.DataBind();
            }
            ddShippingStatus.Items.Insert(0, new ListItem("--Select Status--", ""));
        }

        // ---------------- core loads ----------------

        private void LoadOrder(string order)
        {
            string sql = @"
SELECT 
    BuyerFullName, 
    Country, 
    TotalPrice, 
    Profit,
    Marketplace,
    ShippingType,
    Currency,
    ShippingStatus,
    OrderDate
FROM dbo.T_OrdersPage
WHERE OrderNumber = @Order";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", order);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return;

                    lblOrderNumber.Text = order;

                    lblBuyer.Text = dr["BuyerFullName"] == DBNull.Value ? "" : dr["BuyerFullName"].ToString();
                    lblCountry.Text = dr["Country"] == DBNull.Value ? "" : dr["Country"].ToString();
                    lblMarketplace.Text = dr["Marketplace"] == DBNull.Value ? "" : dr["Marketplace"].ToString();
                    lblShippingType.Text = dr["ShippingType"] == DBNull.Value ? "" : dr["ShippingType"].ToString();
                    lblCurrency.Text = dr["Currency"] == DBNull.Value ? "" : dr["Currency"].ToString();

                    if (dr["OrderDate"] != DBNull.Value) _orderDate = Convert.ToDateTime(dr["OrderDate"]);
                    else _orderDate = DateTime.Today;

                    lblOrderDate.Text = _orderDate.ToString("yyyy-MM-dd");

                    _usdPlnRate = GetUsdPlnRate(_orderDate);
                    lblUsdPlnRate.Text = (_usdPlnRate > 0 ? _usdPlnRate.ToString("0.0000") : "-");

                    decimal totalUsd = 0m;
                    if (dr["TotalPrice"] != DBNull.Value) decimal.TryParse(dr["TotalPrice"].ToString(), out totalUsd);

                    // Top cards TOTAL (USD+PLN)
                    lblTotalBig.Text = totalUsd.ToString("0.00");
                    lblTotalBigPln.Text = ConvertUsdToPln(totalUsd).ToString("0.00");

                    // Status badge
                    string statusText = dr["ShippingStatus"] == DBNull.Value ? "-" : dr["ShippingStatus"].ToString();
                    lblStatusBadge.Text = string.IsNullOrWhiteSpace(statusText) ? "-" : statusText;
                    ApplyStatusBadgeCss(lblStatusBadge.Text);
                }
            }
        }

        private void LoadAddressAndContact(string order)
        {
            string sql = @"
SELECT TOP 1
    ISNULL(ShipTo,'') AS ShipTo,
    ISNULL(Email,'') AS Email,
    ISNULL(PhoneNumber,'') AS PhoneNumber
FROM dbo.T_Order
WHERE OrderNumber = @Order
ORDER BY OrderID DESC";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", order);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblShipTo.Text = dr["ShipTo"].ToString();
                        lblEmail.Text = dr["Email"].ToString();
                        lblPhone.Text = dr["PhoneNumber"].ToString();
                    }
                    else
                    {
                        lblShipTo.Text = "";
                        lblEmail.Text = "";
                        lblPhone.Text = "";
                    }
                }
            }
        }

        private void LoadItems(string order)
        {
            // ProductType from V_Product (SKU join)
            // UnitCost from T_StockReceipt (latest)
            string sql = @"
SELECT 
    od.SKU,
    od.Quantity,
    od.ItemPrice,
    od.RingSizeID,
    ISNULL(rs.RingSize,'') AS RingSize,
    ISNULL(p.ProductType,'') AS ProductType,
(
    SELECT TOP 1 ISNULL(sr.ItemPrice,0)
    FROM dbo.T_StockReceipt sr
    WHERE sr.SKU = od.SKU
    ORDER BY sr.PurchaseDate DESC, sr.StockReceiptID DESC
) AS UnitCostUsd
FROM dbo.T_OrderDetails od
LEFT JOIN dbo.T_RingSize rs ON rs.RingSizeID = od.RingSizeID
LEFT JOIN dbo.V_Product p ON p.SKU = od.SKU
WHERE od.OrderNumber = @Order
ORDER BY od.OrderDetailsID DESC";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", order);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // add calculated PLN columns
                    dt.Columns.Add("ItemPricePln", typeof(decimal));
                    dt.Columns.Add("UnitCostPln", typeof(decimal));
                    dt.Columns.Add("LineCostUsd", typeof(decimal));
                    dt.Columns.Add("LineCostPln", typeof(decimal));

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow r = dt.Rows[i];

                        decimal itemPrice = r["ItemPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(r["ItemPrice"]);
                        decimal unitCost = r["UnitCostUsd"] == DBNull.Value ? 0m : Convert.ToDecimal(r["UnitCostUsd"]);
                        int qty = r["Quantity"] == DBNull.Value ? 0 : Convert.ToInt32(r["Quantity"]);

                        r["ItemPricePln"] = ConvertUsdToPln(itemPrice);
                        r["UnitCostPln"] = ConvertUsdToPln(unitCost);

                        decimal lineCost = unitCost * qty;
                        r["LineCostUsd"] = lineCost;
                        r["LineCostPln"] = ConvertUsdToPln(lineCost);

                        // RingSize empty -> show blank
                        if (r["RingSize"] == DBNull.Value) r["RingSize"] = "";
                    }

                    gvItems.DataSource = dt;
                    gvItems.DataBind();
                }
            }
        }

        private void LoadNotes(string order)
        {
            string sql = @"
SELECT TOP 1
    ISNULL(BuyerNotes,'') AS BuyerNotes,
    ISNULL(SellerNotes,'') AS SellerNotes,
    ISNULL(GiftMessage,'') AS GiftMessage
FROM dbo.T_Order
WHERE OrderNumber = @Order
ORDER BY OrderID DESC";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", order);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblBuyerNotes.Text = dr["BuyerNotes"].ToString();
                        lblSellerNotes.Text = dr["SellerNotes"].ToString();
                        lblGiftMessage.Text = dr["GiftMessage"].ToString();
                    }
                    else
                    {
                        lblBuyerNotes.Text = "";
                        lblSellerNotes.Text = "";
                        lblGiftMessage.Text = "";
                    }
                }
            }
        }

        // ---------------- shipping legs ----------------

        private void LoadShippingLegs(string order)
        {
            // defaults
            ddInterCompany.SelectedValue = "";
            txtInterTracking.Text = "";
            txtInterShipDate.Text = "";
            txtInterPrice.Text = "0";
            lblInterPricePln.Text = "0.00";

            ddMainCompany.SelectedValue = "";
            txtMainTracking.Text = "";
            txtMainShipDate.Text = "";
            txtMainPrice.Text = "0";
            lblMainPricePln.Text = "0.00";

            string sql = @"
SELECT LegType, ShippingCompanyID, TrackingNumber, ShipDate, ShippingPriceUsd
FROM dbo.T_ShippingLeg
WHERE OrderNumber=@Order";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", order);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        byte leg = Convert.ToByte(dr["LegType"]);
                        string companyId = dr["ShippingCompanyID"] == DBNull.Value ? "" : dr["ShippingCompanyID"].ToString();
                        string tracking = dr["TrackingNumber"] == DBNull.Value ? "" : dr["TrackingNumber"].ToString();
                        string shipDate = dr["ShipDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["ShipDate"]).ToString("yyyy-MM-dd");
                        decimal price = dr["ShippingPriceUsd"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["ShippingPriceUsd"]);

                        if (leg == 1)
                        {
                            ddInterCompany.SelectedValue = companyId;
                            txtInterTracking.Text = tracking;
                            txtInterShipDate.Text = shipDate;
                            txtInterPrice.Text = price.ToString("0.00");
                            lblInterPricePln.Text = ConvertUsdToPln(price).ToString("0.00");
                        }
                        else if (leg == 2)
                        {
                            ddMainCompany.SelectedValue = companyId;
                            txtMainTracking.Text = tracking;
                            txtMainShipDate.Text = shipDate;
                            txtMainPrice.Text = price.ToString("0.00");
                            lblMainPricePln.Text = ConvertUsdToPln(price).ToString("0.00");
                        }
                    }
                }
            }

            // Shipping status from old table (or orders page)
            try
            {
                string sqlStatus = @"SELECT TOP 1 ShippingStatusID, KKID FROM dbo.T_Shipping WHERE OrderNumber=@Order ORDER BY ShippingID DESC";
                using (SqlConnection con = new SqlConnection(strConnString))
                using (SqlCommand cmd = new SqlCommand(sqlStatus, con))
                {
                    cmd.Parameters.AddWithValue("@Order", order);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (dr["ShippingStatusID"] != DBNull.Value)
                                ddShippingStatus.SelectedValue = dr["ShippingStatusID"].ToString();

                            txtKKID.Text = dr["KKID"] == DBNull.Value ? "" : dr["KKID"].ToString();
                        }
                    }
                }
            }
            catch { }
        }

        private void UpsertLeg(string order, int legType, string companyId, string tracking, string shipDateText, string priceText, string recordBy)
        {
            DateTime d;
            DateTime? shipDate = DateTime.TryParse(shipDateText, out d) ? (DateTime?)d.Date : null;

            decimal price;
            if (!decimal.TryParse(priceText, out price)) price = 0m;

            string sqlCheck = "SELECT COUNT(*) FROM dbo.T_ShippingLeg WHERE OrderNumber=@Order AND LegType=@Leg";
            string sqlUpdate = @"
UPDATE dbo.T_ShippingLeg
SET ShippingCompanyID=@Company,
    TrackingNumber=@Tracking,
    ShipDate=@ShipDate,
    ShippingPriceUsd=@Price,
    RecordDate=GETDATE(),
    RecordBy=@By
WHERE OrderNumber=@Order AND LegType=@Leg";

            string sqlInsert = @"
INSERT INTO dbo.T_ShippingLeg (OrderNumber, LegType, ShippingCompanyID, TrackingNumber, ShipDate, ShippingPriceUsd, RecordBy)
VALUES (@Order, @Leg, @Company, @Tracking, @ShipDate, @Price, @By)";

            using (SqlConnection con = new SqlConnection(strConnString))
            {
                con.Open();
                int exists = 0;
                using (SqlCommand c1 = new SqlCommand(sqlCheck, con))
                {
                    c1.Parameters.AddWithValue("@Order", order);
                    c1.Parameters.AddWithValue("@Leg", legType);
                    exists = Convert.ToInt32(c1.ExecuteScalar());
                }

                using (SqlCommand cmd = new SqlCommand(exists > 0 ? sqlUpdate : sqlInsert, con))
                {
                    cmd.Parameters.AddWithValue("@Order", order);
                    cmd.Parameters.AddWithValue("@Leg", legType);

                    if (string.IsNullOrWhiteSpace(companyId)) cmd.Parameters.AddWithValue("@Company", DBNull.Value);
                    else cmd.Parameters.AddWithValue("@Company", Convert.ToInt32(companyId));

                    cmd.Parameters.AddWithValue("@Tracking", (object)(tracking ?? ""));

                    if (shipDate.HasValue) cmd.Parameters.AddWithValue("@ShipDate", shipDate.Value);
                    else cmd.Parameters.AddWithValue("@ShipDate", DBNull.Value);

                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@By", (object)(recordBy ?? ""));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ---------------- expenses ----------------

        private void LoadExpenses(string order)
        {
            string sql = @"
SELECT OrderExtraExpenseID, ExpenseDate, ExpenseType, Description, AmountUsd
FROM dbo.T_OrderExtraExpense
WHERE OrderNumber=@Order
ORDER BY ExpenseDate DESC, OrderExtraExpenseID DESC";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", order);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                dt.Columns.Add("AmountPln", typeof(decimal));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    decimal usd = dt.Rows[i]["AmountUsd"] == DBNull.Value ? 0m : Convert.ToDecimal(dt.Rows[i]["AmountUsd"]);
                    dt.Rows[i]["AmountPln"] = ConvertUsdToPln(usd);
                }

                gvExpenses.DataSource = dt;
                gvExpenses.DataBind();
            }
        }

        protected void btnAddExpense_Click(object sender, EventArgs e)
        {
            string order = lblOrderNumber.Text;
            if (string.IsNullOrWhiteSpace(order)) return;

            decimal amt;
            if (!decimal.TryParse(txtExpenseAmount.Text, out amt)) amt = 0m;

            DateTime d;
            DateTime? ed = DateTime.TryParse(txtExpenseDate.Text, out d) ? (DateTime?)d.Date : null;

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.T_OrderExtraExpense (OrderNumber, ExpenseType, Description, AmountUsd, ExpenseDate, RecordBy)
VALUES (@Order, @Type, @Desc, @Amt, @Date, @By)", con))
            {
                cmd.Parameters.AddWithValue("@Order", order);
                cmd.Parameters.AddWithValue("@Type", ddExpenseType.SelectedValue);
                cmd.Parameters.AddWithValue("@Desc", (object)(txtExpenseDesc.Text ?? ""));
                cmd.Parameters.AddWithValue("@Amt", amt);
                if (ed.HasValue) cmd.Parameters.AddWithValue("@Date", ed.Value);
                else cmd.Parameters.AddWithValue("@Date", DBNull.Value);
                cmd.Parameters.AddWithValue("@By", (object)Context.User.Identity.Name);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            txtExpenseDesc.Text = "";
            txtExpenseAmount.Text = "0";
            txtExpenseDate.Text = "";

            LoadExpenses(order);
            LoadFinancialSummary(order); // refresh profit
        }

    private decimal GetUnitCostSafe(OrderProfitCalculator calc, string sku)
    {
        // calc içindeki GetUnitCostFromStock private; o yüzden 2 seçenek var:
        // 1) GetUnitCostFromStock'u public yap
        // 2) Aynı sorguyu burada yaz
        // Ben 2) yi veriyorum (en hızlı fix):

        const string sql = @"
SELECT TOP 1 ISNULL(ItemPrice,0)
FROM dbo.T_StockReceipt
WHERE SKU = @SKU
ORDER BY PurchaseDate DESC, StockReceiptID DESC";

        decimal cost = 0m;
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            cmd.Parameters.AddWithValue("@SKU", sku);
            con.Open();
            object o = cmd.ExecuteScalar();
            if (o != null && o != DBNull.Value) decimal.TryParse(o.ToString(), out cost);
        }
        return cost;
    }

    protected void gvExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DEL")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection con = new SqlConnection(strConnString))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.T_OrderExtraExpense WHERE OrderExtraExpenseID=@ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                string order = lblOrderNumber.Text;
                LoadExpenses(order);
                LoadFinancialSummary(order);
            }
        }

        // ---------------- financial summary (calculator + PLN) ----------------

        private void LoadFinancialSummary(string orderNumber)
        {
            OrderProfitCalculator calc = new OrderProfitCalculator();
            OrderProfitResult r = calc.Calculate(orderNumber);

            // SALES (Buyer)
            lblItemTotalUsd.Text = r.ItemTotalUsd.ToString("0.00");
            lblItemTotalPln.Text = ConvertUsdToPln(r.ItemTotalUsd).ToString("0.00");

            lblShipBuyerUsd.Text = r.ShippingBuyerUsd.ToString("0.00");
            lblShipBuyerPln.Text = ConvertUsdToPln(r.ShippingBuyerUsd).ToString("0.00");

            lblTaxBuyerUsd.Text = r.TaxBuyerUsd.ToString("0.00");
            lblTaxBuyerPln.Text = ConvertUsdToPln(r.TaxBuyerUsd).ToString("0.00");

            lblGiftUsd.Text = r.GiftWrapUsd.ToString("0.00");
            lblGiftPln.Text = ConvertUsdToPln(r.GiftWrapUsd).ToString("0.00");

            lblGrossUsd.Text = r.GrossSalesUsd.ToString("0.00");
            lblGrossPln.Text = ConvertUsdToPln(r.GrossSalesUsd).ToString("0.00");

            // FEES repeater
            DataTable dtFees = new DataTable();
            dtFees.Columns.Add("Label");
            dtFees.Columns.Add("AmountUsd", typeof(decimal));
            dtFees.Columns.Add("AmountPln");

            for (int i = 0; i < r.FeeLines.Count; i++)
            {
                DataRow row = dtFees.NewRow();
                row["Label"] = r.FeeLines[i].Label;
                row["AmountUsd"] = r.FeeLines[i].AmountUsd;
                row["AmountPln"] = ConvertUsdToPln(r.FeeLines[i].AmountUsd).ToString("0.00");
                dtFees.Rows.Add(row);
            }
            rpFees.DataSource = dtFees;
            rpFees.DataBind();

            lblFeesUsd.Text = r.MarketplaceFeesUsd.ToString("0.00");
            lblFeesPln.Text = ConvertUsdToPln(r.MarketplaceFeesUsd).ToString("0.00");

            // COSTS
            lblProductCostUsd.Text = r.ProductCostUsd.ToString("0.00");
            lblProductCostPln.Text = ConvertUsdToPln(r.ProductCostUsd).ToString("0.00");

            lblShipCostUsd.Text = r.ShippingCostUsd.ToString("0.00");
            lblShipCostPln.Text = ConvertUsdToPln(r.ShippingCostUsd).ToString("0.00");

            lblExtraUsd.Text = r.ExtraExpensesUsd.ToString("0.00");
            lblExtraPln.Text = ConvertUsdToPln(r.ExtraExpensesUsd).ToString("0.00");

            // NET
            lblNetProfitUsd.Text = r.NetProfitUsd.ToString("0.00");
            lblNetProfitPln.Text = ConvertUsdToPln(r.NetProfitUsd).ToString("0.00");

            // Top card NET
            lblNetProfitBig.Text = r.NetProfitUsd.ToString("0.00");
            lblNetProfitBigPln.Text = ConvertUsdToPln(r.NetProfitUsd).ToString("0.00");
        }

        // ---------------- save/delete/back ----------------

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string order = lblOrderNumber.Text;
            if (string.IsNullOrWhiteSpace(order)) return;

            string by = Context.User.Identity.Name;

            // upsert legs
            UpsertLeg(order, 1, ddInterCompany.SelectedValue, txtInterTracking.Text, txtInterShipDate.Text, txtInterPrice.Text, by);
            UpsertLeg(order, 2, ddMainCompany.SelectedValue, txtMainTracking.Text, txtMainShipDate.Text, txtMainPrice.Text, by);

            // keep legacy shipping status in T_Shipping (so your existing list/status logic stays)
            try
            {
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    con.Open();

                    string check = "SELECT COUNT(*) FROM dbo.T_Shipping WHERE OrderNumber=@Order";
                    int exists = 0;
                    using (SqlCommand c = new SqlCommand(check, con))
                    {
                        c.Parameters.AddWithValue("@Order", order);
                        exists = Convert.ToInt32(c.ExecuteScalar());
                    }

                    string sql = exists > 0
                        ? @"UPDATE dbo.T_Shipping SET ShippingStatusID=@St, KKID=@KKID WHERE OrderNumber=@Order"
                        : @"INSERT INTO dbo.T_Shipping (OrderNumber, ShippingStatusID, KKID, ShippingPrice) VALUES (@Order, @St, @KKID, 0)";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Order", order);
                        cmd.Parameters.AddWithValue("@St", string.IsNullOrWhiteSpace(ddShippingStatus.SelectedValue) ? (object)DBNull.Value : ddShippingStatus.SelectedValue);
                        cmd.Parameters.AddWithValue("@KKID", (object)(txtKKID.Text ?? ""));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }

            // refresh OrdersPage (optional)
            try
            {
                using (SqlConnection conxx = new SqlConnection(strConnString))
                using (SqlCommand cmdxx = new SqlCommand("RefreshOrdersPage", conxx))
                {
                    cmdxx.CommandType = CommandType.StoredProcedure;
                    conxx.Open();
                    cmdxx.ExecuteNonQuery();
                }
            }
            catch { }

            // reload
            LoadOrder(order);
            LoadAddressAndContact(order);
            LoadItems(order);
            LoadNotes(order);

            LoadShippingLegs(order);
            LoadExpenses(order);
            LoadFinancialSummary(order);

            Response.Write("<script>alert('Saved!');</script>");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string order = lblOrderNumber.Text;
            if (!string.IsNullOrEmpty(order))
            {
                DeleteOrderRecords(order);
                Response.Redirect("OrderList.aspx");
            }
        }

        private void DeleteOrderRecords(string order)
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                con.Open();

                string sql = @"
DELETE FROM dbo.T_ShippingLeg        WHERE OrderNumber = @Order;
DELETE FROM dbo.T_OrderExtraExpense  WHERE OrderNumber = @Order;
DELETE FROM dbo.T_Shipping           WHERE OrderNumber = @Order;
DELETE FROM dbo.T_PackExpenses       WHERE OrderNumber = @Order;
DELETE FROM dbo.T_OrderDetails       WHERE OrderNumber = @Order;
DELETE FROM dbo.T_Order              WHERE OrderNumber = @Order;
DELETE FROM dbo.T_OrdersPage         WHERE OrderNumber = @Order;";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Order", order);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("OrderList.aspx");
        }

        // ---------------- helpers ----------------

        private void ApplyStatusBadgeCss(string statusText)
        {
            lblStatusBadge.CssClass = "badge-pill b-default";
            string s = (statusText ?? "").Trim().ToLowerInvariant();

            if (s == "preparing") lblStatusBadge.CssClass = "badge-pill b-preparing";
            else if (s == "packaged") lblStatusBadge.CssClass = "badge-pill b-packaged";
            else if (s == "ready") lblStatusBadge.CssClass = "badge-pill b-ready";
            else if (s == "pre-shipping") lblStatusBadge.CssClass = "badge-pill b-pre-shipping";
            else if (s == "final shipping") lblStatusBadge.CssClass = "badge-pill b-final";
            else if (s == "cancel") lblStatusBadge.CssClass = "badge-pill b-cancel";
        }

        private decimal ConvertUsdToPln(decimal usd)
        {
            if (_usdPlnRate <= 0) return 0m;
            return Math.Round(usd * _usdPlnRate, 2);
        }

        private decimal GetUsdPlnRate(DateTime rateDate)
        {
            string sql = @"
SELECT TOP 1 UsdPln
FROM " + UsdPlnRateTable + @"
WHERE RateDate <= @D
ORDER BY RateDate DESC";

            try
            {
                using (SqlConnection con = new SqlConnection(strConnString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@D", rateDate.Date);
                    con.Open();
                    object o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                    {
                        decimal r;
                        if (decimal.TryParse(o.ToString(), out r)) return r;
                    }
                }
            }
            catch { }

            return 0m;
        }
    }
}
