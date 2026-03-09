using Feniks.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class OrderList : System.Web.UI.Page
    {
        private readonly string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        // ✅ NEW SP
        private const string OrdersPagedSp = "dbo.GetOrdersPage_Paged_v2";

        private const int UnprocessedTopN = 10;

        // Main grid mode: DONE (3,4) default; PENDING (1,2)
        private string MainMode
        {
            get { return (ViewState["MainMode"] as string) ?? "DONE"; }
            set { ViewState["MainMode"] = value; }
        }

        // FX cache (postback'lerde stabil olsun diye ViewState'de tutuyoruz)
        private Dictionary<string, decimal> FxCache
        {
            get
            {
                var obj = ViewState["FxCache"] as Dictionary<string, decimal>;
                if (obj == null)
                {
                    obj = new Dictionary<string, decimal>();
                    ViewState["FxCache"] = obj;
                }
                return obj;
            }
            set { ViewState["FxCache"] = value; }
        }

        private string KpiPeriod
        {
            get { return (ViewState["KpiPeriod"] as string) ?? "MONTH"; }
            set { ViewState["KpiPeriod"] = value; }
        }

        // =========================================================
        // ✅ DBNull-safe converters (CRASH FIX)
        // =========================================================
        private static int ToInt(object o)
        {
            if (o == null || o == DBNull.Value) return 0;

            int n;
            var s = o.ToString().Trim();
            if (string.IsNullOrWhiteSpace(s)) return 0;

            if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out n)) return n;
            if (int.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out n)) return n;

            return 0;
        }

        private static decimal ToDec(object o)
        {
            if (o == null || o == DBNull.Value) return 0m;

            // ✅ If it is already numeric, NEVER go through ToString() (culture-safe)
            try
            {
                if (o is decimal dec) return dec;
                if (o is double dbl) return Convert.ToDecimal(dbl);
                if (o is float flt) return Convert.ToDecimal(flt);
                if (o is int i) return i;
                if (o is long l) return l;
                if (o is short srt) return srt;
                if (o is byte b) return b;
            }
            catch { /* ignore and fallback to string parse */ }

            var s = o.ToString().Trim();
            if (string.IsNullOrWhiteSpace(s)) return 0m;

            // ✅ If string uses comma as decimal separator (and no dot), normalize first
            // Example: "3,885527"  -> "3.885527"
            if (s.Contains(",") && !s.Contains("."))
            {
                // remove spaces
                s = s.Replace(" ", "");

                // treat comma as decimal separator only when it looks like decimal format
                // e.g. 1 to 6 digits after comma
                int idx = s.LastIndexOf(',');
                if (idx > 0 && (s.Length - idx - 1) >= 1 && (s.Length - idx - 1) <= 6)
                {
                    s = s.Replace(",", ".");
                }
            }

            decimal d;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;

            // fallback: current culture
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;

            // last resort: force dot
            if (decimal.TryParse(s.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out d)) return d;

            return 0m;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindFilters();

                KpiPeriod = "MONTH";
                ApplyKpiTabStyles();
                ApplyPeriodToDateBoxes(KpiPeriod);

                // ✅ main grid default = DONE
                MainMode = "DONE";
                UpdateMainModeButtonText();

                gvOrders.PageIndex = 0;
                BindGridPaged();
            }
        }

        // ---------------- navigation/buttons ----------------

        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

        protected void btnCreateOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/OrderCreate.aspx");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            gvOrders.PageIndex = 0;
            BindGridPaged();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddMarketplace.SelectedValue = "";
            ddStatus.SelectedValue = "";
            txtSearch.Text = "";
            txtFrom.Text = "";
            txtTo.Text = "";

            KpiPeriod = "ALL";
            ApplyKpiTabStyles();

            gvOrders.PageIndex = 0;
            BindGridPaged();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            gvOrders.PageIndex = 0;
            BindGridPaged();
        }

        protected void Filters_Changed(object sender, EventArgs e)
        {
            if (sender == txtFrom || sender == txtTo)
            {
                KpiPeriod = "CUSTOM";
                ApplyKpiTabStyles();
            }

            gvOrders.PageIndex = 0;
            BindGridPaged();
        }

        // ✅ Toggle main grid: DONE <-> PENDING
        protected void btnMainModeToggle_Click(object sender, EventArgs e)
        {
            MainMode = (MainMode == "DONE") ? "PENDING" : "DONE";
            UpdateMainModeButtonText();

            gvOrders.PageIndex = 0;
            BindGridPaged();
            upMain.Update();
        }

        private void UpdateMainModeButtonText()
        {
            if (btnMainModeToggle == null) return;

            // Buton “ana grid’de diğerini göster” diye dursun
            btnMainModeToggle.Text = (MainMode == "DONE")
                ? "Show Pending in main grid"
                : "Show Done in main grid";
        }

        // ---------------- filter binding ----------------

        private void BindFilters()
        {
            BindMarketplace();
            BindStatus();
        }

        private void BindMarketplace()
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT MarketplaceID, Marketplace
                FROM dbo.T_Marketplace
                ORDER BY Marketplace", con))
            {
                con.Open();
                ddMarketplace.DataSource = cmd.ExecuteReader();
                ddMarketplace.DataTextField = "Marketplace";
                ddMarketplace.DataValueField = "MarketplaceID";
                ddMarketplace.DataBind();
            }
            ddMarketplace.Items.Insert(0, new ListItem("--All--", ""));
        }

        private void BindStatus()
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ShippingStatusID, ShippingStatus
                FROM dbo.T_ShippingStatus
                ORDER BY ShippingStatusID", con))
            {
                con.Open();
                ddStatus.DataSource = cmd.ExecuteReader();
                ddStatus.DataTextField = "ShippingStatus";
                ddStatus.DataValueField = "ShippingStatusID";
                ddStatus.DataBind();
            }
            ddStatus.Items.Insert(0, new ListItem("--All--", ""));
        }

        // ---------------- grid paging ----------------

        protected void gvOrders_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOrders.PageIndex = e.NewPageIndex;
            BindGridPaged();
        }

        private void BindGridPaged()
        {
            // ✅ Top grid always: OrderStatus 1,2
            BindUnprocessed();

            // ✅ Main grid: depends on mode
            string mainOrderStatusCsv = (MainMode == "DONE") ? "3,4" : "1,2";

            DataTable dt = GetOrdersPageFromDb(
                gvOrders.PageIndex,
                gvOrders.PageSize,
                out int totalCount,
                orderStatusCsv: mainOrderStatusCsv
            );

            gvOrders.VirtualItemCount = totalCount;
            lblTotalCount.Text = totalCount.ToString(CultureInfo.InvariantCulture);

            gvOrders.DataSource = dt;
            gvOrders.DataBind();

            SetKpis();

            lblLastUpdated.Text = "Updated: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            ApplyKpiTabStyles();
        }

        private void BindUnprocessed()
        {
            // ✅ ÜST TABLO: TÜM FİLTRELERDEN BAĞIMSIZ
            DataTable dt = GetOrdersPageFromDb_UnprocessedTop(
                topN: UnprocessedTopN,
                out int totalCount
            );

            lblUnprocessedCount.Text = totalCount.ToString(CultureInfo.InvariantCulture);

            gvUnprocessed.DataSource = dt;
            gvUnprocessed.DataBind();
        }

        private DataTable GetOrdersPageFromDb_UnprocessedTop(int topN, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(OrdersPagedSp, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // paging: ilk sayfa + topN
                cmd.Parameters.AddWithValue("@PageIndex", 0);
                cmd.Parameters.AddWithValue("@PageSize", topN);

                // ✅ filtreler tamamen kapalı
                cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                cmd.Parameters.AddWithValue("@MarketplaceID", DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingStatusID", DBNull.Value);
                cmd.Parameters.AddWithValue("@Search", DBNull.Value);

                // ✅ sadece açık siparişler
                cmd.Parameters.AddWithValue("@OrderStatusCsv", "1,2");

                DataSet ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    totalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

                return (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
            }
        }

        private DataTable GetOrdersPageFromDb(
            int pageIndex,
            int pageSize,
            out int totalCount,
            string orderStatusCsv,
            bool ignoreDdStatus = false
        )
        {
            totalCount = 0;

            DateTime fromD, toD;
            DateTime? from = DateTime.TryParse(txtFrom.Text, out fromD) ? fromD.Date : (DateTime?)null;
            DateTime? to = DateTime.TryParse(txtTo.Text, out toD) ? toD.Date : (DateTime?)null;

            if (from.HasValue && to.HasValue && from.Value > to.Value)
            {
                DateTime? tmp = from; from = to; to = tmp;
            }

            int id;
            int? marketplaceId = int.TryParse(ddMarketplace.SelectedValue, out id) ? (int?)id : null;

            int st;
            int? shippingStatusId = null;
            if (!ignoreDdStatus)
                shippingStatusId = int.TryParse(ddStatus.SelectedValue, out st) ? (int?)st : null;

            string search = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim();

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(OrdersPagedSp, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                cmd.Parameters.AddWithValue("@FromDate", (object)from ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", (object)to ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@MarketplaceID", (object)marketplaceId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingStatusID", (object)shippingStatusId ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Search", (object)search ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@OrderStatusCsv", (object)orderStatusCsv ?? DBNull.Value);

                DataSet ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    totalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

                return (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
            }
        }

        // ---------------- row formatting & net profit ----------------

        protected void gvOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            Label lblRaw = e.Row.FindControl("lblStatusRaw") as Label;
            Label lblPill = e.Row.FindControl("lblStatusPill") as Label;
            string status = (lblRaw != null ? lblRaw.Text : "") ?? "";

            if (lblPill != null)
            {
                if (status.Equals("Preparing", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-preparing";
                else if (status.Equals("Packaged", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-packaged";
                else if (status.Equals("Ready", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-ready";
                else if (status.Equals("Pre-Shipping", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-preship";
                else if (status.Equals("Waiting for Decision", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-wait";
                else if (status.Equals("Size Revision", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-size";
                else if (status.Equals("Awaiting from Supplier", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-supplier";
                else if (status.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-cancel";
                else if (status.Equals("Final Shipping", StringComparison.OrdinalIgnoreCase))
                    lblPill.CssClass = "status-pill status-final";
                else
                    lblPill.CssClass = "status-pill status-other";
            }

            if (status.Equals("Final Shipping", StringComparison.OrdinalIgnoreCase))
                e.Row.CssClass = (e.Row.CssClass + " row-final-shipping").Trim();

            // Net profit
            Label lblOrderHidden = e.Row.FindControl("lblOrderNumberHidden") as Label;
            Label lblNetUsd = e.Row.FindControl("lblNetProfitUsd") as Label;
            Label lblNetPln = e.Row.FindControl("lblNetProfitPln") as Label;

            if (lblOrderHidden == null || lblNetUsd == null || lblNetPln == null) return;

            string orderNo = lblOrderHidden.Text ?? "";
            if (string.IsNullOrWhiteSpace(orderNo)) return;

            OrderProfitCalculator calc = new OrderProfitCalculator();
            OrderProfitResult r = calc.Calculate(orderNo);

            // FX
            DateTime orderDate = LocalToday();
            object oDate = DataBinder.Eval(e.Row.DataItem, "OrderDate");
            if (oDate != null && oDate != DBNull.Value)
                orderDate = Convert.ToDateTime(oDate);

            object wdObj = DataBinder.Eval(e.Row.DataItem, "WaitingDays");
            int wd = (wdObj != null && wdObj != DBNull.Value) ? Convert.ToInt32(wdObj) : 0;

            if (wd >= 14) e.Row.Style["border-left"] = "6px solid #d9534f"; // 14+ gün
            else if (wd >= 7) e.Row.Style["border-left"] = "6px solid #f0ad4e"; // 7-13 gün
            else if (wd >= 3) e.Row.Style["border-left"] = "6px solid #5bc0de"; // 3-6 gün

            decimal fx = GetUsdPlnRateCached(orderDate);
            decimal pln = Math.Round(r.NetProfitUsd * fx, 2);

            lblNetUsd.Text = "$ " + r.NetProfitUsd.ToString("0.00", CultureInfo.InvariantCulture);
            lblNetPln.Text = pln.ToString("0.00", CultureInfo.InvariantCulture);

            lblNetUsd.CssClass = (r.NetProfitUsd > 0) ? "profit-pos" : "profit-zero";
        }

        // ---------------- KPI summary ----------------

        private class KpiSummary
        {
            public int OrdersCount { get; set; }
            public decimal RevenueUsd { get; set; }
            public decimal ProfitUsd { get; set; }
            public decimal RevenuePln { get; set; }
            public decimal ProfitPln { get; set; }
            public decimal AvgUsdPlnWeighted { get; set; }
        }

        private void SetKpis()
        {
            DateTime fromD, toD;
            DateTime? from = DateTime.TryParse(txtFrom.Text, out fromD) ? fromD.Date : (DateTime?)null;
            DateTime? to = DateTime.TryParse(txtTo.Text, out toD) ? toD.Date : (DateTime?)null;

            KpiSummary kpi = GetKpiFromDb(KpiPeriod, from, to);

            lblKpiOrders.Text = kpi.OrdersCount.ToString(CultureInfo.InvariantCulture);
            lblKpiRevenue.Text = "$ " + kpi.RevenueUsd.ToString("0.00", CultureInfo.InvariantCulture);
            lblKpiProfit.Text = "$ " + kpi.ProfitUsd.ToString("0.00", CultureInfo.InvariantCulture);
            lblKpiRevenuePln.Text = "≈ " + kpi.RevenuePln.ToString("0.00", CultureInfo.InvariantCulture) + " zł";
            lblKpiProfitPln.Text = "≈ " + kpi.ProfitPln.ToString("0.00", CultureInfo.InvariantCulture) + " zł";

            lblKpiFx.Text = (kpi.AvgUsdPlnWeighted > 0m)
                ? kpi.AvgUsdPlnWeighted.ToString("0.0000", CultureInfo.InvariantCulture)
                : "-";

            lblKpiPeriodText.Text = PeriodLabel(KpiPeriod);
        }

        // ✅ CRASH FIX: DBNull-safe reads here
        private KpiSummary GetKpiFromDb(string period, DateTime? from, DateTime? to)
        {
            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand("dbo.GetOrdersKpiSummary_DailyFx", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@Period", SqlDbType.VarChar, 10).Value = (object)(period ?? "MONTH");

                cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value =
                    from.HasValue ? (object)from.Value : DBNull.Value;

                cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value =
                    to.HasValue ? (object)to.Value : DBNull.Value;

                string mpText = (ddMarketplace != null && ddMarketplace.SelectedValue != "")
                    ? ddMarketplace.SelectedItem.Text
                    : null;

                cmd.Parameters.Add("@Marketplace", SqlDbType.VarChar, 50).Value =
                    string.IsNullOrWhiteSpace(mpText) ? (object)DBNull.Value : mpText;

                string stText = (ddStatus != null && ddStatus.SelectedValue != "")
                    ? ddStatus.SelectedItem.Text
                    : null;

                cmd.Parameters.Add("@ShippingStatus", SqlDbType.VarChar, 50).Value =
                    string.IsNullOrWhiteSpace(stText) ? (object)DBNull.Value : stText;

                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value =
                    string.IsNullOrWhiteSpace(txtSearch.Text) ? (object)DBNull.Value : txtSearch.Text.Trim();

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new KpiSummary
                        {
                            OrdersCount = ToInt(dr["OrdersCount"]),
                            RevenueUsd = ToDec(dr["RevenueUsd"]),
                            ProfitUsd = ToDec(dr["ProfitUsd"]),
                            RevenuePln = ToDec(dr["RevenuePln"]),
                            ProfitPln = ToDec(dr["ProfitPln"]),
                            AvgUsdPlnWeighted = ToDec(dr["AvgUsdPln_Weighted"])
                        };
                    }
                }
            }

            return new KpiSummary();
        }

        protected void btnKpiPeriod_Command(object sender, CommandEventArgs e)
        {
            string period = (e.CommandArgument ?? "").ToString().Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(period)) return;

            KpiPeriod = period;

            if (KpiPeriod == "ALL")
            {
                txtFrom.Text = "";
                txtTo.Text = "";
            }
            else if (KpiPeriod == "TODAY")
            {
                var t = DateTime.Today;
                txtFrom.Text = t.ToString("yyyy-MM-dd");
                txtTo.Text = t.ToString("yyyy-MM-dd");
            }
            else if (KpiPeriod == "MONTH")
            {
                DateTime d1 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                txtFrom.Text = d1.ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else if (KpiPeriod == "YEAR")
            {
                DateTime d1 = new DateTime(DateTime.Today.Year, 1, 1);
                txtFrom.Text = d1.ToString("yyyy-MM-dd");
                txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else if (KpiPeriod == "CUSTOM")
            {
                if (string.IsNullOrWhiteSpace(txtFrom.Text))
                    txtFrom.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
                if (string.IsNullOrWhiteSpace(txtTo.Text))
                    txtTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            }

            ApplyKpiTabStyles();
            gvOrders.PageIndex = 0;

            BindGridPaged();
            upMain.Update();
        }

        private void ApplyPeriodToDateBoxes(string period)
        {
            DateTime today = LocalToday();

            if (period == "ALL")
            {
                txtFrom.Text = "";
                txtTo.Text = "";
            }
            else if (period == "TODAY")
            {
                txtFrom.Text = today.ToString("yyyy-MM-dd");
                txtTo.Text = today.ToString("yyyy-MM-dd");
            }
            else if (period == "MONTH")
            {
                DateTime d1 = new DateTime(today.Year, today.Month, 1);
                txtFrom.Text = d1.ToString("yyyy-MM-dd");
                txtTo.Text = today.ToString("yyyy-MM-dd");
            }
            else if (period == "YEAR")
            {
                DateTime d1 = new DateTime(today.Year, 1, 1);
                txtFrom.Text = d1.ToString("yyyy-MM-dd");
                txtTo.Text = today.ToString("yyyy-MM-dd");
            }
            else if (period == "CUSTOM")
            {
                if (string.IsNullOrWhiteSpace(txtFrom.Text))
                    txtFrom.Text = today.AddDays(-7).ToString("yyyy-MM-dd");
                if (string.IsNullOrWhiteSpace(txtTo.Text))
                    txtTo.Text = today.ToString("yyyy-MM-dd");
            }
        }

        private void ApplyKpiTabStyles()
        {
            SetActive(btnKpiAll, KpiPeriod == "ALL");
            SetActive(btnKpiToday, KpiPeriod == "TODAY");
            SetActive(btnKpiMonth, KpiPeriod == "MONTH");
            SetActive(btnKpiYear, KpiPeriod == "YEAR");
            SetActive(btnKpiCustom, KpiPeriod == "CUSTOM");
        }

        private void SetActive(LinkButton b, bool active)
        {
            if (b == null) return;
            b.CssClass = active ? "kpi-tab active" : "kpi-tab";
        }

        private string PeriodLabel(string period)
        {
            switch ((period ?? "").ToUpperInvariant())
            {
                case "ALL": return "All Records";
                case "TODAY": return "Today";
                case "MONTH": return "This Month";
                case "YEAR": return "This Year";
                case "CUSTOM": return "Custom";
                default: return "";
            }
        }

        // ---------------- helpers: FX + local today ----------------

        private DateTime LocalToday()
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz).Date;
            }
            catch
            {
                return DateTime.Today;
            }
        }

        private decimal GetUsdPlnRateCached(DateTime date)
        {
            string key = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var cache = FxCache;

            if (cache.ContainsKey(key))
                return cache[key];

            decimal fx = 0m;
            string sql = @"
SELECT TOP 1 UsdPln
FROM dbo.T_FxUsdPln
WHERE RateDate <= @D
ORDER BY RateDate DESC";

            using (SqlConnection con = new SqlConnection(strConnString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@D", date.Date);
                con.Open();
                object o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value)
                    fx = Convert.ToDecimal(o, CultureInfo.InvariantCulture);
            }

            cache[key] = fx;
            FxCache = cache;
            return fx;
        }
    }
}