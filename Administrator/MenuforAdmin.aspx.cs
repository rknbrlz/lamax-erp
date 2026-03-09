using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Feniks.Admin
{
    public partial class MenuforAdmin : System.Web.UI.Page
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EnsureAuthenticated();

                // ✅ Sarı alanlar kapalı: Alert + Hero + 2 kart
                divInfo.Visible = false;
                pnlHero.Visible = false;
                //pnlOrderManagement.Visible = false;
                pnlDashboardReports.Visible = false;

                // Order counters
                LoadOrderCounters();

                // Load FX (DB)
                LoadLatestFxRates();

                // If old, refresh silently
                EnsureFxRatesFresh();

                // Reload FX after refresh
                LoadLatestFxRates();

                // Top bar synced with FX table
                SyncTopBarWithLatestFx();

                // Payments
                InitPaymentsUi();
                LoadLatestPayments();
            }
        }

        private void EnsureAuthenticated()
        {
            if (!this.Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            lblLoginName.Text = this.Page.User.Identity.Name ?? "Label";

            if (lblLoginName.Text == "Label")
            {
                Response.Redirect("~/Login.aspx");
            }
        }

        private void OpenOrderQuantityValue()
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT TOP (1) OpenOrderQty FROM V_OpenOrderQuantity", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);

                lblOpenQty.Text = (dt.Rows.Count > 0) ? dt.Rows[0]["OpenOrderQty"].ToString() : "0";
            }
        }

        // =========================================================
        // ORDER COUNTERS
        // =========================================================
        private void LoadOrderCounters()
        {
            long totalOrders = 0;
            long totalQty = 0;

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT
                    (SELECT COUNT(DISTINCT OrderNumber) FROM dbo.T_Order) AS TotalOrders,
                    (SELECT CAST(ISNULL(SUM(COALESCE(Quantity,0)),0) AS bigint) FROM dbo.T_OrderDetails) AS TotalQty;
            ", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        totalOrders = (r["TotalOrders"] == DBNull.Value) ? 0 : Convert.ToInt64(r["TotalOrders"]);
                        totalQty = (r["TotalQty"] == DBNull.Value) ? 0 : Convert.ToInt64(r["TotalQty"]);
                    }
                }
            }

            lblTotalOrders.Text = totalOrders.ToString();
            lblTotalQty.Text = totalQty.ToString();

            hfTotalOrders.Value = totalOrders.ToString();
            hfTotalQty.Value = totalQty.ToString();

            lblOrdersCounterUpdated.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        // =========================================================
        // FX (NBP -> DB)
        // =========================================================
        private void LoadLatestFxRates()
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (1)
                    RateDate, UsdPln, EurPln, GbpPln, TryPln,
                    ISNULL(UpdatedAt, CreatedAt) AS LastUpdate
                FROM dbo.T_FxUsdPln
                ORDER BY RateDate DESC;", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        SetFxUi("-", "-", "-", "-", "-", "-");
                        return;
                    }

                    var d = Convert.ToDateTime(r["RateDate"]).ToString("yyyy-MM-dd");
                    var u = Convert.ToDateTime(r["LastUpdate"]).ToString("yyyy-MM-dd HH:mm");

                    string usd = (r["UsdPln"] == DBNull.Value) ? "-" : Convert.ToDecimal(r["UsdPln"]).ToString("0.0000", CultureInfo.InvariantCulture);
                    string eur = SafeDec(r, "EurPln");
                    string gbp = SafeDec(r, "GbpPln");
                    string trY = SafeDec(r, "TryPln");

                    SetFxUi(usd, eur, gbp, trY, d, u);
                }
            }
        }

        private string SafeDec(IDataRecord r, string col)
        {
            try
            {
                var v = r[col];
                if (v == null || v == DBNull.Value) return "-";
                return Convert.ToDecimal(v).ToString("0.0000", CultureInfo.InvariantCulture);
            }
            catch { return "-"; }
        }

        private void SetFxUi(string usd, string eur, string gbp, string trY, string date, string updated)
        {
            lblUsdPln.Text = usd;
            lblEurPln.Text = eur;
            lblGbpPln.Text = gbp;
            lblTryPln.Text = trY;

            lblFxDate.Text = date;
            lblFxUpdated.Text = updated;
        }

        private void SyncTopBarWithLatestFx()
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (1) RateDate, UsdPln, EurPln, CadPln, TryPln
                FROM dbo.T_FxUsdPln
                ORDER BY RateDate DESC;", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        lblUSD.Text = "0.0000";
                        lblEUR.Text = "0.0000";
                        lblCAD.Text = "0.0000";
                        lblPLN.Text = "0.0000";
                        return;
                    }

                    lblUSD.Text = (r["UsdPln"] == DBNull.Value) ? "0.0000"
                        : Convert.ToDecimal(r["UsdPln"]).ToString("0.0000", CultureInfo.InvariantCulture);

                    lblEUR.Text = (r["EurPln"] == DBNull.Value) ? "0.0000"
                        : Convert.ToDecimal(r["EurPln"]).ToString("0.0000", CultureInfo.InvariantCulture);

                    lblCAD.Text = (r["CadPln"] == DBNull.Value) ? "0.0000"
                        : Convert.ToDecimal(r["CadPln"]).ToString("0.0000", CultureInfo.InvariantCulture);

                    if (r["TryPln"] == DBNull.Value)
                    {
                        lblPLN.Text = "0.0000";
                    }
                    else
                    {
                        decimal tryPln = Convert.ToDecimal(r["TryPln"]);
                        lblPLN.Text = (tryPln > 0) ? (1m / tryPln).ToString("0.0000", CultureInfo.InvariantCulture) : "0.0000";
                    }
                }
            }
        }

        protected void btnUpdateFx_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateFxFromNbp();

                lblFxMsg.Visible = true;
                lblFxMsg.Style["color"] = "#2d862d";
                lblFxMsg.Text = "FX rates updated successfully.";

                LoadLatestFxRates();
                SyncTopBarWithLatestFx();
            }
            catch (Exception ex)
            {
                lblFxMsg.Visible = true;
                lblFxMsg.Style["color"] = "#b30000";
                lblFxMsg.Text = "FX update failed: " + ex.Message;
            }
        }

        private void EnsureFxRatesFresh()
        {
            DateTime? lastDate = null;

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT MAX(RateDate) FROM dbo.T_FxUsdPln", con))
            {
                con.Open();
                var o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value) lastDate = Convert.ToDateTime(o);
            }

            if (!lastDate.HasValue || lastDate.Value.Date < DateTime.UtcNow.Date.AddDays(-2))
            {
                UpdateFxFromNbp();
            }
        }

        private void UpdateFxFromNbp()
        {
            string json;
            using (var wc = new WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                json = wc.DownloadString("https://api.nbp.pl/api/exchangerates/tables/A?format=json");
            }

            var ser = new JavaScriptSerializer();
            var tables = ser.Deserialize<NbpTable[]>(json);

            if (tables == null || tables.Length == 0)
                throw new Exception("NBP response empty.");

            var t = tables[0];
            var rateDate = DateTime.ParseExact(t.effectiveDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;

            decimal usd = t.rates.First(x => x.code == "USD").mid;
            decimal eur = t.rates.First(x => x.code == "EUR").mid;
            decimal gbp = t.rates.First(x => x.code == "GBP").mid;
            decimal trY = t.rates.First(x => x.code == "TRY").mid;
            decimal cad = t.rates.First(x => x.code == "CAD").mid;

            bool hasAll = HasColumns_EurGbpTryCad();

            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                if (!hasAll)
                    throw new Exception("DB columns missing. Ensure EurPln/GbpPln/TryPln/CadPln exist in dbo.T_FxUsdPln.");

                using (var cmd = new SqlCommand(@"
                    MERGE dbo.T_FxUsdPln AS tgt
                    USING (SELECT @d AS RateDate) src
                    ON tgt.RateDate = src.RateDate
                    WHEN MATCHED THEN
                        UPDATE SET
                            UsdPln=@u, EurPln=@e, GbpPln=@g, TryPln=@t, CadPln=@c,
                            Source='NBP',
                            UpdatedAt=SYSUTCDATETIME()
                    WHEN NOT MATCHED THEN
                        INSERT (RateDate, UsdPln, EurPln, GbpPln, TryPln, CadPln, Source, CreatedAt)
                        VALUES (@d,@u,@e,@g,@t,@c,'NBP',SYSUTCDATETIME());", con))
                {
                    cmd.Parameters.AddWithValue("@d", rateDate);
                    cmd.Parameters.AddWithValue("@u", usd);
                    cmd.Parameters.AddWithValue("@e", eur);
                    cmd.Parameters.AddWithValue("@g", gbp);
                    cmd.Parameters.AddWithValue("@t", trY);
                    cmd.Parameters.AddWithValue("@c", cad);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool HasColumns_EurGbpTryCad()
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM sys.columns
                WHERE object_id = OBJECT_ID('dbo.T_FxUsdPln')
                  AND name IN ('EurPln','GbpPln','TryPln','CadPln');", con))
            {
                con.Open();
                var cnt = Convert.ToInt32(cmd.ExecuteScalar());
                return cnt == 4;
            }
        }

        private class NbpTable
        {
            public string effectiveDate { get; set; }
            public NbpRate[] rates { get; set; }
        }

        private class NbpRate
        {
            public string code { get; set; }
            public decimal mid { get; set; }
        }

        // =========================================================
        // Currency Converter
        // =========================================================
        protected void btnFxConvert_Click(object sender, EventArgs e)
        {
            lblFxConvertMsg.Visible = false;

            try
            {
                var raw = (txtFxAmount.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(raw))
                    throw new Exception("Amount is empty.");

                decimal amount;
                if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out amount) &&
                    !decimal.TryParse(raw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                {
                    throw new Exception("Invalid amount format.");
                }

                if (amount < 0) throw new Exception("Amount cannot be negative.");

                var from = ddlFxFrom.SelectedValue;
                var to = ddlFxTo.SelectedValue;

                EnsureFxRatesFresh();

                var rateFromPln = GetPlnRate(from);
                var rateToPln = GetPlnRate(to);

                if (rateFromPln <= 0 || rateToPln <= 0)
                    throw new Exception("Rates not available.");

                decimal amountPln = amount * rateFromPln;
                decimal result = amountPln / rateToPln;

                lblFxConvertResult.Text = $"{result:0.00} {to}";
                lblFxConvertRateDate.Text = GetLatestFxRateDateText();

                SyncTopBarWithLatestFx();
            }
            catch (Exception ex)
            {
                lblFxConvertMsg.Visible = true;
                lblFxConvertMsg.Style["color"] = "#b30000";
                lblFxConvertMsg.Text = "Convert failed: " + ex.Message;
            }
        }

        private decimal GetPlnRate(string currencyCode)
        {
            currencyCode = (currencyCode ?? "").Trim().ToUpperInvariant();
            if (currencyCode == "PLN") return 1m;

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (1) RateDate, UsdPln, EurPln, TryPln, CadPln
                FROM dbo.T_FxUsdPln
                ORDER BY RateDate DESC;", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        throw new Exception("No FX data in DB. Please update from NBP.");

                    object val;
                    switch (currencyCode)
                    {
                        case "USD": val = SafeGet(r, "UsdPln"); break;
                        case "EUR": val = SafeGet(r, "EurPln"); break;
                        case "TRY": val = SafeGet(r, "TryPln"); break;
                        case "CAD": val = SafeGet(r, "CadPln"); break;
                        default: throw new Exception("Unsupported currency: " + currencyCode);
                    }

                    if (val == null || val == DBNull.Value)
                        throw new Exception("Rate missing for " + currencyCode);

                    return Convert.ToDecimal(val, CultureInfo.InvariantCulture);
                }
            }
        }

        private object SafeGet(IDataRecord r, string col)
        {
            try { return r[col]; } catch { return DBNull.Value; }
        }

        private string GetLatestFxRateDateText()
        {
            try
            {
                using (var con = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("SELECT TOP (1) RateDate FROM dbo.T_FxUsdPln ORDER BY RateDate DESC;", con))
                {
                    con.Open();
                    var o = cmd.ExecuteScalar();
                    if (o == null || o == DBNull.Value) return "-";
                    return Convert.ToDateTime(o).ToString("yyyy-MM-dd");
                }
            }
            catch { return "-"; }
        }

        // =========================================================
        // PAYMENTS
        // =========================================================
        private void InitPaymentsUi()
        {
            try { txtPayDate.Attributes["type"] = "date"; } catch { }

            if (string.IsNullOrWhiteSpace(txtPayDate.Text))
                txtPayDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void LoadLatestPayments()
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
                SELECT TOP (8)
                    CONVERT(varchar(10), PayDate, 23) AS PayDate,
                    Marketplace,
                    Amount
                FROM dbo.T_MarketplacePayments
                ORDER BY PayDate DESC, PaymentId DESC;", con))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                gvPayments.DataSource = dt;
                gvPayments.DataBind();
            }

            lblPayUpdated.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        protected void btnPayAdd_ServerClick(object sender, EventArgs e)
        {
            lblPayMsg.Visible = false;

            try
            {
                var marketplace = (ddlPayMarketplace.SelectedValue ?? "").Trim();
                if (string.IsNullOrWhiteSpace(marketplace))
                    throw new Exception("Marketplace is empty.");

                var dateRaw = (txtPayDate.Text ?? "").Trim();
                DateTime payDate;

                if (!DateTime.TryParseExact(dateRaw, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out payDate))
                {
                    if (!DateTime.TryParse(dateRaw, CultureInfo.CurrentCulture, DateTimeStyles.None, out payDate))
                        throw new Exception("Invalid date format.");
                }

                var amountRaw = (txtPayAmount.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(amountRaw))
                    throw new Exception("Amount is empty.");

                decimal amount;
                if (!decimal.TryParse(amountRaw, NumberStyles.Any, CultureInfo.CurrentCulture, out amount) &&
                    !decimal.TryParse(amountRaw.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                {
                    throw new Exception("Invalid amount format.");
                }

                if (amount <= 0) throw new Exception("Amount must be greater than 0.");

                using (var con = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(@"
                    INSERT INTO dbo.T_MarketplacePayments (Marketplace, PayDate, Amount)
                    VALUES (@m, @d, @a);", con))
                {
                    cmd.Parameters.AddWithValue("@m", marketplace);
                    cmd.Parameters.AddWithValue("@d", payDate.Date);
                    cmd.Parameters.AddWithValue("@a", amount);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                lblPayMsg.Visible = true;
                lblPayMsg.Style["color"] = "#2d862d";
                lblPayMsg.Text = "Payment saved.";

                txtPayAmount.Text = "";

                LoadLatestPayments();
            }
            catch (Exception ex)
            {
                lblPayMsg.Visible = true;
                lblPayMsg.Style["color"] = "#b30000";
                lblPayMsg.Text = "Save failed: " + ex.Message;
            }
        }

        // =========================================================
        // NAVIGATION
        // =========================================================
        protected void toProducts_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }

        protected void toOrders_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/OrderList.aspx");
        }

        protected void btnNewOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/OrderCreate.aspx");
        }

        // Stock Management UI’dan kaldırıldı, istersen bunu da silebilirsin:
        // protected void toStockManagement_click(object sender, EventArgs e)
        // {
        //     Response.Redirect("~/Administrator/StockManagement.aspx");
        // }

        protected void toDashboard_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Dashboard.aspx");
        }
    }
}