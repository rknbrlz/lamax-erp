using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Feniks.Administrator
{
    public class DashboardApi : IHttpHandler
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var action = (context.Request["action"] ?? "").Trim().ToLowerInvariant();
            var period = (context.Request["period"] ?? "MONTH").Trim().ToUpperInvariant();
            var fromDate = (context.Request["fromDate"] ?? "").Trim();
            var toDate = (context.Request["toDate"] ?? "").Trim();

            try
            {
                if (action != "getall")
                    throw new Exception("Unsupported action.");

                var fx = GetLatestUsdPln();

                DateTime rangeFrom, rangeToExcl;
                GetRange(period, fromDate, toDate, out rangeFrom, out rangeToExcl);

                var sales = GetSalesAll(period, fromDate, toDate, fx.UsdPlnRate);
                var pay = GetPaymentsAll(fx.UsdPlnRate, rangeFrom, rangeToExcl);
                var custKpi = GetCustomerKpiFromOrders(rangeFrom, rangeToExcl);
                var costs = GetCostsAll(rangeFrom, rangeToExcl, fx.UsdPlnRate);

                var ordersAnalytics = GetOrdersAnalytics(fx.UsdPlnRate, rangeFrom, rangeToExcl);

                // NEW: Product analytics (1 + 2)
                var productAnalytics = GetProductAnalytics(rangeFrom, rangeToExcl, fx.UsdPlnRate, sales.TopProducts);

                var result = new
                {
                    Fx = fx,

                    // sales
                    Kpi = sales.Kpi,
                    Trend = sales.Trend,
                    Marketplace = sales.Marketplace,
                    Country = sales.Country,
                    TopProducts = sales.TopProducts,
                    RepeatCustomers = sales.RepeatCustomers,

                    // payments
                    PaymentsKpi = pay.PaymentsKpi,
                    PaymentsMarketplace = pay.PaymentsMarketplace,
                    PaymentsList = pay.PaymentsList,

                    // customers
                    CustomerKpi = custKpi,

                    // costs
                    CostsKpi = costs.CostsKpi,
                    OrderCosts = costs.OrderCosts,
                    ShippingLegs = costs.ShippingLegs,
                    ExtraExpenses = costs.ExtraExpenses,

                    // orders
                    OrdersAnalytics = ordersAnalytics,

                    // products
                    ProductAnalytics = productAnalytics
                };

                var json = new JavaScriptSerializer().Serialize(result);
                context.Response.Write(json);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write(new JavaScriptSerializer().Serialize(new { error = ex.Message }));
            }
        }

        // =========================
        // Safe readers
        // =========================
        private static decimal ReadDecimalSafe(IDataRecord r, string fieldName, decimal defaultValue = 0m)
        {
            try
            {
                int idx = r.GetOrdinal(fieldName);
                if (idx < 0) return defaultValue;
                if (r.IsDBNull(idx)) return defaultValue;
                return Convert.ToDecimal(r.GetValue(idx));
            }
            catch { return defaultValue; }
        }

        private static int ReadIntSafe(IDataRecord r, string fieldName, int defaultValue = 0)
        {
            try
            {
                int idx = r.GetOrdinal(fieldName);
                if (idx < 0) return defaultValue;
                if (r.IsDBNull(idx)) return defaultValue;
                return Convert.ToInt32(r.GetValue(idx));
            }
            catch { return defaultValue; }
        }

        private static string ReadStringSafe(IDataRecord r, string fieldName, string defaultValue = "")
        {
            try
            {
                int idx = r.GetOrdinal(fieldName);
                if (idx < 0) return defaultValue;
                if (r.IsDBNull(idx)) return defaultValue;
                return r.GetValue(idx).ToString();
            }
            catch { return defaultValue; }
        }

        // =========================
        // Column / Object helpers
        // =========================
        private static bool ColumnExists(SqlConnection con, string tableName, string columnName)
        {
            using (var cmd = new SqlCommand(@"
SELECT 1
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME=@t AND COLUMN_NAME=@c;", con))
            {
                cmd.Parameters.AddWithValue("@t", tableName);
                cmd.Parameters.AddWithValue("@c", columnName);
                var o = cmd.ExecuteScalar();
                return o != null && o != DBNull.Value;
            }
        }

        private static string PickFirstExisting(SqlConnection con, string tableName, params string[] candidates)
        {
            foreach (var c in candidates)
                if (ColumnExists(con, tableName, c))
                    return c;
            return null;
        }

        private static bool ObjectExists(SqlConnection con, string fullName)
        {
            using (var cmd = new SqlCommand(@"
SELECT 1
FROM sys.objects o
WHERE o.object_id = OBJECT_ID(@n)
  AND o.type IN ('U','V');", con))
            {
                cmd.Parameters.AddWithValue("@n", fullName);
                var o = cmd.ExecuteScalar();
                return o != null && o != DBNull.Value;
            }
        }

        // =========================
        // FX
        // =========================
        public class FxDto
        {
            public decimal UsdPlnRate { get; set; }
            public string RateDate { get; set; }
        }

        private FxDto GetLatestUsdPln()
        {
            var fx = new FxDto { UsdPlnRate = 0m, RateDate = "-" };

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT TOP (1) RateDate, UsdPln
FROM dbo.T_FxUsdPln
ORDER BY RateDate DESC;", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        fx.RateDate = Convert.ToDateTime(r["RateDate"]).ToString("yyyy-MM-dd");
                        fx.UsdPlnRate = (r["UsdPln"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["UsdPln"], CultureInfo.InvariantCulture);
                    }
                }
            }

            return fx;
        }

        // =========================
        // Sales (SP)
        // =========================
        private class SalesAll
        {
            public KpiCardsDto Kpi { get; set; } = new KpiCardsDto();
            public List<TrendPointDto> Trend { get; set; } = new List<TrendPointDto>();
            public List<MarketDto> Marketplace { get; set; } = new List<MarketDto>();
            public List<CountryDto> Country { get; set; } = new List<CountryDto>();
            public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
            public List<RepeatCustomerDto> RepeatCustomers { get; set; } = new List<RepeatCustomerDto>();
        }

        public class KpiCardsDto
        {
            public int OrdersCount { get; set; }
            public int ItemsCount { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
            public decimal AOV_Original { get; set; }
            public decimal AOV_PLN { get; set; }
        }

        public class TrendPointDto
        {
            public string Label { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
            public int OrdersCount { get; set; }
        }

        public class MarketDto
        {
            public int MarketplaceID { get; set; }
            public string Marketplace { get; set; }
            public int OrdersCount { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
        }

        public class CountryDto
        {
            public int CountryID { get; set; }
            public string Country { get; set; }
            public int OrdersCount { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
        }

        public class TopProductDto
        {
            public int? ProductTypeID { get; set; }
            public string ProductType { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
        }

        public class RepeatCustomerDto
        {
            public string CustomerKey { get; set; }
            public string BuyerFullName { get; set; }
            public string Email { get; set; }
            public int OrdersCount { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
            public DateTime FirstOrder { get; set; }
            public DateTime LastOrder { get; set; }
            public string FirstOrderText { get; set; }
            public string LastOrderText { get; set; }
        }

        private SalesAll GetSalesAll(string period, string fromDate, string toDate, decimal usdPlnRate)
        {
            var dto = new SalesAll();

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("dbo.Dashboard_GetAll", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Period", (object)(period ?? "MONTH"));
                var fd = ParseDateMulti(fromDate);
                var td = ParseDateMulti(toDate);
                cmd.Parameters.AddWithValue("@FromDate", (object)fd ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", (object)td ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@USCountryID", DBNull.Value);
                cmd.Parameters.AddWithValue("@TopProducts", 30);
                cmd.Parameters.AddWithValue("@TopRepeat", 50);
                cmd.Parameters.AddWithValue("@TopStates", 30);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    // Resultset 1: KPI
                    if (r.Read())
                    {
                        dto.Kpi.OrdersCount = ReadIntSafe(r, "OrdersCount");
                        dto.Kpi.ItemsCount = ReadIntSafe(r, "ItemsCount");
                        dto.Kpi.RevenueOriginal = ReadDecimalSafe(r, "RevenueOriginal");
                        dto.Kpi.RevenuePLN = ReadDecimalSafe(r, "RevenuePLN");
                        dto.Kpi.AOV_Original = ReadDecimalSafe(r, "AOV_Original");
                        dto.Kpi.AOV_PLN = ReadDecimalSafe(r, "AOV_PLN");
                    }

                    // Resultset 2: Trend
                    if (r.NextResult())
                    {
                        while (r.Read())
                        {
                            var key = Convert.ToDateTime(r["PeriodKey"]);
                            dto.Trend.Add(new TrendPointDto
                            {
                                Label = key.ToString("yyyy-MM-dd"),
                                RevenueOriginal = ReadDecimalSafe(r, "RevenueOriginal"),
                                RevenuePLN = ReadDecimalSafe(r, "RevenuePLN"),
                                OrdersCount = ReadIntSafe(r, "OrdersCount")
                            });
                        }
                    }

                    // Resultset 3: Marketplace
                    if (r.NextResult())
                    {
                        while (r.Read())
                        {
                            dto.Marketplace.Add(new MarketDto
                            {
                                MarketplaceID = ReadIntSafe(r, "MarketplaceID"),
                                Marketplace = ReadStringSafe(r, "Marketplace"),
                                OrdersCount = ReadIntSafe(r, "OrdersCount"),
                                RevenueOriginal = ReadDecimalSafe(r, "RevenueOriginal"),
                                RevenuePLN = ReadDecimalSafe(r, "RevenuePLN")
                            });
                        }
                    }

                    // Resultset 4: Country
                    if (r.NextResult())
                    {
                        while (r.Read())
                        {
                            dto.Country.Add(new CountryDto
                            {
                                CountryID = ReadIntSafe(r, "CountryID"),
                                Country = ReadStringSafe(r, "Country"),
                                OrdersCount = ReadIntSafe(r, "OrdersCount"),
                                RevenueOriginal = ReadDecimalSafe(r, "RevenueOriginal"),
                                RevenuePLN = ReadDecimalSafe(r, "RevenuePLN")
                            });
                        }
                    }

                    // Resultset 5: (States) -> skip
                    if (r.NextResult()) { while (r.Read()) { } }

                    // Resultset 6: Repeat customers
                    if (r.NextResult())
                    {
                        while (r.Read())
                        {
                            decimal revPln = ReadDecimalSafe(r, "RevenuePLN");
                            decimal revUsd = ReadDecimalSafe(r, "RevenueOriginal");
                            if (revUsd == 0m && usdPlnRate > 0m && revPln > 0m) revUsd = revPln / usdPlnRate;

                            DateTime first = (r["FirstOrder"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(r["FirstOrder"]);
                            DateTime last = (r["LastOrder"] == DBNull.Value) ? DateTime.MinValue : Convert.ToDateTime(r["LastOrder"]);

                            dto.RepeatCustomers.Add(new RepeatCustomerDto
                            {
                                CustomerKey = ReadStringSafe(r, "CustomerKey"),
                                BuyerFullName = ReadStringSafe(r, "BuyerFullName"),
                                Email = ReadStringSafe(r, "Email"),
                                OrdersCount = ReadIntSafe(r, "OrdersCount"),
                                RevenueOriginal = revUsd,
                                RevenuePLN = revPln,
                                FirstOrder = first,
                                LastOrder = last,
                                FirstOrderText = (first == DateTime.MinValue) ? "-" : first.ToString("yyyy-MM-dd"),
                                LastOrderText = (last == DateTime.MinValue) ? "-" : last.ToString("yyyy-MM-dd")
                            });
                        }
                    }

                    // Resultset 7: Top products
                    if (r.NextResult())
                    {
                        while (r.Read())
                        {
                            decimal revPln = ReadDecimalSafe(r, "RevenuePLN");
                            decimal revUsd = ReadDecimalSafe(r, "RevenueOriginal");
                            if (revUsd == 0m && usdPlnRate > 0m && revPln > 0m) revUsd = revPln / usdPlnRate;

                            dto.TopProducts.Add(new TopProductDto
                            {
                                ProductTypeID = (r["ProductTypeID"] == DBNull.Value) ? (int?)null : Convert.ToInt32(r["ProductTypeID"]),
                                ProductType = ReadStringSafe(r, "ProductType"),
                                SKU = ReadStringSafe(r, "SKU"),
                                Qty = ReadIntSafe(r, "Qty"),
                                RevenueOriginal = revUsd,
                                RevenuePLN = revPln
                            });
                        }
                    }
                }
            }

            return dto;
        }

        // =========================
        // Orders Analytics DTOs
        // =========================
        public class OrdersPointDto
        {
            public string Label { get; set; }
            public int OrdersCount { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
            public decimal AOV_Original { get; set; }
            public decimal AOV_PLN { get; set; }
        }

        public class StackDatasetDto
        {
            public string Label { get; set; }
            public List<int> Data { get; set; } = new List<int>();
        }

        public class MarketStackDto
        {
            public List<string> Labels { get; set; } = new List<string>();
            public List<StackDatasetDto> Datasets { get; set; } = new List<StackDatasetDto>();
        }

        public class HeatmapWeekDto
        {
            public string WeekLabel { get; set; }
            public List<int> Cells { get; set; } = new List<int>();
        }

        public class HeatmapDto
        {
            public List<HeatmapWeekDto> Weeks { get; set; } = new List<HeatmapWeekDto>();
        }

        public class OrdersAnalyticsDto
        {
            public int TotalOrders { get; set; }
            public decimal AvgPerDay { get; set; }
            public string PeakDayText { get; set; }
            public int PeakDayCount { get; set; }

            public decimal TotalRevenueUsd { get; set; }
            public decimal TotalRevenuePln { get; set; }

            public List<OrdersPointDto> Yearly { get; set; } = new List<OrdersPointDto>();
            public List<OrdersPointDto> Monthly { get; set; } = new List<OrdersPointDto>();
            public List<OrdersPointDto> Weekly { get; set; } = new List<OrdersPointDto>();

            public MarketStackDto MarketStack30 { get; set; } = new MarketStackDto();

            public string TopCountryName { get; set; }
            public int TopCountryOrders { get; set; }

            public HeatmapDto Heatmap35 { get; set; } = new HeatmapDto();
        }

        // =========================
        // Orders Analytics (YOUR FULL)
        // =========================
        private OrdersAnalyticsDto GetOrdersAnalytics(decimal usdPlnRate, DateTime selectedFrom, DateTime selectedToExclusive)
        {
            var dto = new OrdersAnalyticsDto();

            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                using (var cmd = new SqlCommand(@"
;WITH base AS (
    SELECT
        CAST(oa.OrderDate AS date) AS Dt,
        oa.OrderNumber,
        CAST(ISNULL(oa.OrderTotalOriginal,0) AS decimal(18,6)) AS RevUsd,
        CAST(ISNULL(oa.OrderTotalPLN,0)      AS decimal(18,6)) AS RevPln
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
),
dayAgg AS (
    SELECT Dt, COUNT(DISTINCT OrderNumber) AS OrdersCount
    FROM base
    GROUP BY Dt
)
SELECT
    (SELECT COUNT(DISTINCT OrderNumber) FROM base) AS TotalOrders,
    (SELECT ISNULL(SUM(RevUsd),0) FROM base) AS TotalRevUsd,
    (SELECT ISNULL(SUM(RevPln),0) FROM base) AS TotalRevPln,
    (SELECT TOP (1) Dt FROM dayAgg ORDER BY OrdersCount DESC, Dt DESC) AS PeakDay,
    (SELECT TOP (1) OrdersCount FROM dayAgg ORDER BY OrdersCount DESC, Dt DESC) AS PeakCount;", con))
                {
                    cmd.Parameters.AddWithValue("@f", selectedFrom.Date);
                    cmd.Parameters.AddWithValue("@t", selectedToExclusive.Date);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            dto.TotalOrders = (r["TotalOrders"] == DBNull.Value) ? 0 : Convert.ToInt32(r["TotalOrders"]);
                            dto.TotalRevenueUsd = (r["TotalRevUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["TotalRevUsd"]);
                            dto.TotalRevenuePln = (r["TotalRevPln"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["TotalRevPln"]);

                            var pd = (r["PeakDay"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["PeakDay"]);
                            dto.PeakDayText = pd.HasValue ? pd.Value.ToString("yyyy-MM-dd") : "-";
                            dto.PeakDayCount = (r["PeakCount"] == DBNull.Value) ? 0 : Convert.ToInt32(r["PeakCount"]);
                        }
                    }
                }

                var days = (selectedToExclusive.Date - selectedFrom.Date).TotalDays;
                dto.AvgPerDay = (days > 0) ? (dto.TotalOrders / (decimal)days) : 0m;

                Func<DateTime, DateTime, string, List<OrdersPointDto>> getSeries = (from, toEx, mode) =>
                {
                    string keyExpr, labelFmt;
                    if (mode == "MONTH")
                    {
                        keyExpr = "DATEFROMPARTS(YEAR(oa.OrderDate), MONTH(oa.OrderDate), 1)";
                        labelFmt = "yyyy-MM";
                    }
                    else
                    {
                        keyExpr = "CAST(oa.OrderDate AS date)";
                        labelFmt = "MM-dd";
                    }

                    var list = new List<OrdersPointDto>();

                    using (var cmd = new SqlCommand($@"
;WITH base AS (
    SELECT
        {keyExpr} AS PeriodKey,
        oa.OrderNumber,
        CAST(ISNULL(oa.OrderTotalOriginal,0) AS decimal(18,6)) AS RevUsd,
        CAST(ISNULL(oa.OrderTotalPLN,0)      AS decimal(18,6)) AS RevPln
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
),
agg AS (
    SELECT
        PeriodKey,
        COUNT(DISTINCT OrderNumber) AS OrdersCount,
        SUM(RevUsd) AS RevenueUsd,
        SUM(RevPln) AS RevenuePln
    FROM base
    GROUP BY PeriodKey
)
SELECT PeriodKey, OrdersCount, RevenueUsd, RevenuePln
FROM agg
ORDER BY PeriodKey;", con))
                    {
                        cmd.Parameters.AddWithValue("@f", from.Date);
                        cmd.Parameters.AddWithValue("@t", toEx.Date);

                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                var pk = (r["PeriodKey"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["PeriodKey"]);
                                int oc = (r["OrdersCount"] == DBNull.Value) ? 0 : Convert.ToInt32(r["OrdersCount"]);
                                decimal usd = (r["RevenueUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["RevenueUsd"]);
                                decimal pln = (r["RevenuePln"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["RevenuePln"]);

                                decimal aovUsd = (oc > 0) ? (usd / oc) : 0m;
                                decimal aovPln = (oc > 0) ? (pln / oc) : 0m;

                                list.Add(new OrdersPointDto
                                {
                                    Label = pk.HasValue ? pk.Value.ToString(labelFmt) : "-",
                                    OrdersCount = oc,
                                    RevenueOriginal = usd,
                                    RevenuePLN = pln,
                                    AOV_Original = aovUsd,
                                    AOV_PLN = aovPln
                                });
                            }
                        }
                    }

                    return list;
                };

                var today = DateTime.Now.Date;

                var yearFrom = new DateTime(today.Year, today.Month, 1).AddMonths(-11);
                var yearToEx = new DateTime(today.Year, today.Month, 1).AddMonths(1);
                dto.Yearly = getSeries(yearFrom, yearToEx, "MONTH");

                var mFrom = today.AddDays(-29);
                var mToEx = today.AddDays(1);
                dto.Monthly = getSeries(mFrom, mToEx, "DAY");

                var wFrom = today.AddDays(-6);
                var wToEx = today.AddDays(1);
                dto.Weekly = getSeries(wFrom, wToEx, "DAY");

                // Marketplace stacked (last 30 days)
                var labelDays = new List<DateTime>();
                for (int i = 0; i < 30; i++) labelDays.Add(mFrom.AddDays(i));

                dto.MarketStack30.Labels = labelDays.Select(x => x.ToString("MM-dd")).ToList();

                var rows = new List<Tuple<DateTime, int, int>>();
                using (var cmd = new SqlCommand(@"
;WITH base AS (
    SELECT CAST(oa.OrderDate AS date) AS Dt, oa.MarketplaceID, oa.OrderNumber
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
)
SELECT Dt, MarketplaceID, COUNT(DISTINCT OrderNumber) AS OrdersCount
FROM base
GROUP BY Dt, MarketplaceID
ORDER BY Dt, MarketplaceID;", con))
                {
                    cmd.Parameters.AddWithValue("@f", mFrom.Date);
                    cmd.Parameters.AddWithValue("@t", mToEx.Date);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var dt = (r["Dt"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["Dt"]);
                            int mid = (r["MarketplaceID"] == DBNull.Value) ? 0 : Convert.ToInt32(r["MarketplaceID"]);
                            int cnt = (r["OrdersCount"] == DBNull.Value) ? 0 : Convert.ToInt32(r["OrdersCount"]);
                            if (dt.HasValue && mid > 0) rows.Add(Tuple.Create(dt.Value.Date, mid, cnt));
                        }
                    }
                }

                var mktNames = new Dictionary<int, string>();
                using (var cmd = new SqlCommand(@"SELECT MarketplaceID, Marketplace FROM dbo.T_Marketplace;", con))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int id = r["MarketplaceID"] == DBNull.Value ? 0 : Convert.ToInt32(r["MarketplaceID"]);
                        string name = r["Marketplace"] == DBNull.Value ? "" : r["Marketplace"].ToString();
                        if (id > 0 && !mktNames.ContainsKey(id)) mktNames[id] = name;
                    }
                }

                var allMktIds = rows.Select(x => x.Item2).Distinct().OrderBy(x => x).ToList();
                foreach (var mid in allMktIds)
                {
                    var ds = new StackDatasetDto { Label = mktNames.ContainsKey(mid) ? mktNames[mid] : ("Marketplace " + mid) };
                    foreach (var dday in labelDays)
                    {
                        int v = rows.Where(x => x.Item2 == mid && x.Item1 == dday.Date).Select(x => x.Item3).FirstOrDefault();
                        ds.Data.Add(v);
                    }
                    dto.MarketStack30.Datasets.Add(ds);
                }

                // Top Country (last 30 days)
                using (var cmd = new SqlCommand(@"
;WITH base AS (
    SELECT CAST(oa.OrderDate AS date) AS Dt, oa.OrderNumber, oa.CountryID
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
),
agg AS (
    SELECT CountryID, COUNT(DISTINCT OrderNumber) AS OrdersCount
    FROM base
    WHERE CountryID IS NOT NULL
    GROUP BY CountryID
)
SELECT TOP (1)
    a.CountryID, a.OrdersCount, c.Country
FROM agg a
LEFT JOIN dbo.T_Country c ON c.CountryID = a.CountryID
ORDER BY a.OrdersCount DESC, a.CountryID;", con))
                {
                    cmd.Parameters.AddWithValue("@f", mFrom.Date);
                    cmd.Parameters.AddWithValue("@t", mToEx.Date);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            dto.TopCountryOrders = (r["OrdersCount"] == DBNull.Value) ? 0 : Convert.ToInt32(r["OrdersCount"]);
                            dto.TopCountryName = (r["Country"] == DBNull.Value) ? "-" : r["Country"].ToString();
                            if (string.IsNullOrWhiteSpace(dto.TopCountryName)) dto.TopCountryName = "-";
                        }
                        else
                        {
                            dto.TopCountryName = "-";
                            dto.TopCountryOrders = 0;
                        }
                    }
                }

                // Heatmap (last 35 days -> 5 weeks x 7 days)
                var hmEnd = today.AddDays(1);
                var hmStart = today.AddDays(-34);

                int shift = ((int)hmStart.DayOfWeek + 6) % 7;
                var alignedStart = hmStart.AddDays(-shift);

                var dayCounts = new Dictionary<DateTime, int>();
                using (var cmd = new SqlCommand(@"
SELECT CAST(oa.OrderDate AS date) AS Dt, COUNT(DISTINCT oa.OrderNumber) AS OrdersCount
FROM dbo.V_OrderAgg oa
WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
GROUP BY CAST(oa.OrderDate AS date);", con))
                {
                    cmd.Parameters.AddWithValue("@f", alignedStart.Date);
                    cmd.Parameters.AddWithValue("@t", hmEnd.Date);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var dt = (r["Dt"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["Dt"]);
                            var cnt = (r["OrdersCount"] == DBNull.Value) ? 0 : Convert.ToInt32(r["OrdersCount"]);
                            if (dt.HasValue) dayCounts[dt.Value.Date] = cnt;
                        }
                    }
                }

                for (int w = 0; w < 5; w++)
                {
                    var weekStart = alignedStart.AddDays(w * 7);
                    var weekEnd = weekStart.AddDays(6);

                    var week = new HeatmapWeekDto
                    {
                        WeekLabel = weekStart.ToString("MM-dd") + ".." + weekEnd.ToString("MM-dd"),
                        Cells = new List<int>()
                    };

                    for (int d = 0; d < 7; d++)
                    {
                        var day = weekStart.AddDays(d).Date;
                        int v = dayCounts.ContainsKey(day) ? dayCounts[day] : 0;

                        if (day < hmStart.Date || day >= hmEnd.Date) v = 0;

                        week.Cells.Add(v);
                    }

                    dto.Heatmap35.Weeks.Add(week);
                }
            }

            return dto;
        }

        // =========================
        // Payments
        // =========================
        private class PaymentsAll
        {
            public PaymentsKpiDto PaymentsKpi { get; set; } = new PaymentsKpiDto();
            public List<PaymentsMarketplaceDto> PaymentsMarketplace { get; set; } = new List<PaymentsMarketplaceDto>();
            public List<PaymentRowDto> PaymentsList { get; set; } = new List<PaymentRowDto>();
        }

        public class PaymentsKpiDto
        {
            public decimal YearTotalUSD { get; set; }
            public decimal YearTotalPLN { get; set; }
            public decimal MonthTotalUSD { get; set; }
            public decimal MonthTotalPLN { get; set; }
            public decimal PeriodTotalUSD { get; set; }
            public decimal PeriodTotalPLN { get; set; }
        }

        public class PaymentsMarketplaceDto
        {
            public string Marketplace { get; set; }
            public decimal TotalUSD { get; set; }
            public decimal TotalPLN { get; set; }
            public int CountPayments { get; set; }
        }

        public class PaymentRowDto
        {
            public string PayDateText { get; set; }
            public string Marketplace { get; set; }
            public decimal AmountUSD { get; set; }
            public decimal AmountPLN { get; set; }
            public string CreatedAtText { get; set; }
        }

        private PaymentsAll GetPaymentsAll(decimal usdPlnRate, DateTime rangeFrom, DateTime rangeToExcl)
        {
            var dto = new PaymentsAll();

            var now = DateTime.Now;
            var yearFrom = new DateTime(now.Year, 1, 1);
            var yearTo = yearFrom.AddYears(1);

            var monthFrom = new DateTime(now.Year, now.Month, 1);
            var monthTo = monthFrom.AddMonths(1);

            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                Func<DateTime, DateTime, decimal> sumUsd = (f, t) =>
                {
                    using (var cmd = new SqlCommand(@"
SELECT ISNULL(SUM(Amount),0)
FROM dbo.T_MarketplacePayments
WHERE PayDate >= @f AND PayDate < @t;", con))
                    {
                        cmd.Parameters.AddWithValue("@f", f.Date);
                        cmd.Parameters.AddWithValue("@t", t.Date);
                        return Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                };

                var yearUsd = sumUsd(yearFrom, yearTo);
                dto.PaymentsKpi.YearTotalUSD = yearUsd;
                dto.PaymentsKpi.YearTotalPLN = yearUsd * usdPlnRate;

                var monthUsd = sumUsd(monthFrom, monthTo);
                dto.PaymentsKpi.MonthTotalUSD = monthUsd;
                dto.PaymentsKpi.MonthTotalPLN = monthUsd * usdPlnRate;

                var periodUsd = sumUsd(rangeFrom, rangeToExcl);
                dto.PaymentsKpi.PeriodTotalUSD = periodUsd;
                dto.PaymentsKpi.PeriodTotalPLN = periodUsd * usdPlnRate;

                using (var cmd = new SqlCommand(@"
SELECT Marketplace,
       ISNULL(SUM(Amount),0) AS TotalUSD,
       COUNT(*) AS CountPayments
FROM dbo.T_MarketplacePayments
WHERE PayDate >= @f AND PayDate < @t
GROUP BY Marketplace
ORDER BY TotalUSD DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                    cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var totalUsd = r["TotalUSD"] == DBNull.Value ? 0m : Convert.ToDecimal(r["TotalUSD"]);
                            dto.PaymentsMarketplace.Add(new PaymentsMarketplaceDto
                            {
                                Marketplace = r["Marketplace"]?.ToString(),
                                TotalUSD = totalUsd,
                                TotalPLN = totalUsd * usdPlnRate,
                                CountPayments = r["CountPayments"] == DBNull.Value ? 0 : Convert.ToInt32(r["CountPayments"])
                            });
                        }
                    }
                }

                using (var cmd = new SqlCommand(@"
SELECT TOP (500)
    PayDate, Marketplace, Amount, CreatedAt
FROM dbo.T_MarketplacePayments
WHERE PayDate >= @f AND PayDate < @t
ORDER BY PayDate DESC, PaymentId DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                    cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var payDate = (r["PayDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["PayDate"]);
                            var created = (r["CreatedAt"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["CreatedAt"]);
                            var amtUsd = (r["Amount"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["Amount"]);

                            dto.PaymentsList.Add(new PaymentRowDto
                            {
                                PayDateText = payDate.HasValue ? payDate.Value.ToString("yyyy-MM-dd") : "-",
                                Marketplace = r["Marketplace"]?.ToString(),
                                AmountUSD = amtUsd,
                                AmountPLN = amtUsd * usdPlnRate,
                                CreatedAtText = created.HasValue ? created.Value.ToString("yyyy-MM-dd HH:mm") : ""
                            });
                        }
                    }
                }
            }

            return dto;
        }

        // =========================
        // Costs DTOs
        // =========================
        private class CostsAll
        {
            public CostsKpiDto CostsKpi { get; set; } = new CostsKpiDto();

            public List<OrderCostDto> OrderCosts { get; set; } = new List<OrderCostDto>();
            public List<ShippingLegDto> ShippingLegs { get; set; } = new List<ShippingLegDto>();
            public List<ExtraExpenseDto> ExtraExpenses { get; set; } = new List<ExtraExpenseDto>();
        }

        public class CostsKpiDto
        {
            public decimal ShippingUSD { get; set; }
            public decimal ShippingPLN { get; set; }
            public decimal ExtraUSD { get; set; }
            public decimal ExtraPLN { get; set; }
            public decimal TotalUSD { get; set; }
            public decimal TotalPLN { get; set; }
        }

        public class OrderCostDto
        {
            public string OrderNumber { get; set; }
            public string OrderDateText { get; set; }
            public decimal ShippingUsd { get; set; }
            public decimal ShippingPln { get; set; }
            public decimal ExtraUsd { get; set; }
            public decimal ExtraPln { get; set; }
            public decimal TotalUsd { get; set; }
            public decimal TotalPln { get; set; }
        }

        public class ShippingLegDto
        {
            public string OrderNumber { get; set; }
            public string ShipDateText { get; set; }
            public string Company { get; set; }
            public string Tracking { get; set; }
            public string LegTypeText { get; set; }
            public decimal PriceUsd { get; set; }
            public decimal PricePln { get; set; }
        }

        public class ExtraExpenseDto
        {
            public string OrderNumber { get; set; }
            public string ExpenseDateText { get; set; }
            public string Description { get; set; }
            public decimal AmountUsd { get; set; }
            public decimal AmountPln { get; set; }
        }

        // =========================
        // Costs (YOUR FULL)
        // =========================
        private CostsAll GetCostsAll(DateTime rangeFrom, DateTime rangeToExcl, decimal usdPlnRate)
        {
            var dto = new CostsAll();

            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                using (var cmd = new SqlCommand(@"
;WITH OrdersInPeriod AS (
    SELECT
        oa.OrderNumber,
        CAST(oa.OrderDate AS date) AS OrderDt
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
),
Ship AS (
    SELECT
        sl.OrderNumber,
        SUM(ISNULL(sl.ShippingPriceUsd,0)) AS ShippingUsd
    FROM dbo.T_ShippingLeg sl
    GROUP BY sl.OrderNumber
),
Extra AS (
    SELECT
        ee.OrderNumber,
        SUM(ISNULL(ee.AmountUsd,0)) AS ExtraUsd
    FROM dbo.T_OrderExtraExpense ee
    GROUP BY ee.OrderNumber
)
SELECT
    o.OrderNumber,
    o.OrderDt,
    ISNULL(s.ShippingUsd,0) AS ShippingUsd,
    ISNULL(e.ExtraUsd,0) AS ExtraUsd
FROM OrdersInPeriod o
LEFT JOIN Ship s ON s.OrderNumber = o.OrderNumber
LEFT JOIN Extra e ON e.OrderNumber = o.OrderNumber
ORDER BY o.OrderDt DESC, o.OrderNumber DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                    cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);

                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var ord = r["OrderNumber"]?.ToString();
                            var odt = (r["OrderDt"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["OrderDt"]);
                            var shipUsd = (r["ShippingUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["ShippingUsd"]);
                            var extraUsd = (r["ExtraUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["ExtraUsd"]);
                            var totalUsd = shipUsd + extraUsd;

                            dto.OrderCosts.Add(new OrderCostDto
                            {
                                OrderNumber = ord,
                                OrderDateText = odt.HasValue ? odt.Value.ToString("yyyy-MM-dd") : "-",
                                ShippingUsd = shipUsd,
                                ShippingPln = shipUsd * usdPlnRate,
                                ExtraUsd = extraUsd,
                                ExtraPln = extraUsd * usdPlnRate,
                                TotalUsd = totalUsd,
                                TotalPln = totalUsd * usdPlnRate
                            });

                            dto.CostsKpi.ShippingUSD += shipUsd;
                            dto.CostsKpi.ExtraUSD += extraUsd;
                        }
                    }
                }

                dto.CostsKpi.TotalUSD = dto.CostsKpi.ShippingUSD + dto.CostsKpi.ExtraUSD;
                dto.CostsKpi.ShippingPLN = dto.CostsKpi.ShippingUSD * usdPlnRate;
                dto.CostsKpi.ExtraPLN = dto.CostsKpi.ExtraUSD * usdPlnRate;
                dto.CostsKpi.TotalPLN = dto.CostsKpi.TotalUSD * usdPlnRate;

                // Shipping legs detail
                string shipTable = "T_ShippingLeg";
                string colShipDate = PickFirstExisting(con, shipTable, "ShipDate", "CreatedAt", "InsertDate", "Date");
                string colTracking = PickFirstExisting(con, shipTable, "Tracking", "TrackingNumber", "TrackingNo");
                string colLegType = PickFirstExisting(con, shipTable, "LegType", "LegTypeID", "LegTypeId");
                string colPriceUsd = PickFirstExisting(con, shipTable, "ShippingPriceUsd", "PriceUsd", "CostUsd", "AmountUsd");
                string colCompanyId = PickFirstExisting(con, shipTable, "ShippingCompanyID", "CompanyID", "ShippingCompanyId");
                string colCompanyTxt = PickFirstExisting(con, shipTable, "Company", "CompanyName", "Carrier");

                bool hasCompanyId = !string.IsNullOrEmpty(colCompanyId);
                bool hasCompanyTxt = !string.IsNullOrEmpty(colCompanyTxt);

                var sqlShip =
@";WITH OrdersInPeriod AS (
    SELECT oa.OrderNumber
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
)
SELECT
    sl.OrderNumber,
    " + (colShipDate != null ? ("CAST(sl." + colShipDate + @" AS datetime) AS ShipDt") : "NULL AS ShipDt") + @",
    " + (hasCompanyId ? "sc.ShippingCompany AS Company" : (hasCompanyTxt ? ("sl." + colCompanyTxt + " AS Company") : "NULL AS Company")) + @",
    " + (colTracking != null ? ("sl." + colTracking + " AS Tracking") : "NULL AS Tracking") + @",
    " + (colLegType != null ? ("CAST(sl." + colLegType + " AS varchar(50)) AS LegTypeText") : "NULL AS LegTypeText") + @",
    " + (colPriceUsd != null ? ("CAST(ISNULL(sl." + colPriceUsd + ",0) AS decimal(18,6)) AS PriceUsd") : "CAST(0 AS decimal(18,6)) AS PriceUsd") + @"
FROM dbo.T_ShippingLeg sl
INNER JOIN OrdersInPeriod o ON o.OrderNumber = sl.OrderNumber
" + (hasCompanyId ? "LEFT JOIN dbo.T_ShippingCompany sc ON sc.ShippingCompanyID = sl." + colCompanyId : "") + @"
ORDER BY sl.OrderNumber, ShipDt;";

                using (var cmdShip = new SqlCommand(sqlShip, con))
                {
                    cmdShip.Parameters.AddWithValue("@f", rangeFrom.Date);
                    cmdShip.Parameters.AddWithValue("@t", rangeToExcl.Date);

                    using (var r = cmdShip.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var ord = r["OrderNumber"]?.ToString();
                            var shipDt = (r["ShipDt"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["ShipDt"]);
                            var company = (r["Company"] == DBNull.Value) ? "" : r["Company"].ToString();
                            var tracking = (r["Tracking"] == DBNull.Value) ? "" : r["Tracking"].ToString();
                            var legTypeText = (r["LegTypeText"] == DBNull.Value) ? "" : r["LegTypeText"].ToString();
                            var priceUsd = (r["PriceUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["PriceUsd"]);

                            dto.ShippingLegs.Add(new ShippingLegDto
                            {
                                OrderNumber = ord,
                                ShipDateText = shipDt.HasValue ? shipDt.Value.ToString("yyyy-MM-dd") : "-",
                                Company = company,
                                Tracking = tracking,
                                LegTypeText = legTypeText,
                                PriceUsd = priceUsd,
                                PricePln = priceUsd * usdPlnRate
                            });
                        }
                    }
                }

                // Extra expenses detail
                string extraTable = "T_OrderExtraExpense";
                string colExtraDate = PickFirstExisting(con, extraTable, "ExpenseDate", "CreatedAt", "InsertDate", "Date");
                string colExtraDesc = PickFirstExisting(con, extraTable, "Description", "ExpenseType", "Title", "Note", "Notes", "Reason");
                string colExtraAmt = PickFirstExisting(con, extraTable, "AmountUsd", "Amount", "CostUsd", "PriceUsd");

                var sqlExtra =
@";WITH OrdersInPeriod AS (
    SELECT oa.OrderNumber
    FROM dbo.V_OrderAgg oa
    WHERE oa.OrderDate >= @f AND oa.OrderDate < @t
)
SELECT
    ee.OrderNumber,
    " + (colExtraDate != null ? ("CAST(ee." + colExtraDate + @" AS datetime) AS ExDt") : "NULL AS ExDt") + @",
    " + (colExtraDesc != null ? ("CAST(ee." + colExtraDesc + @" AS nvarchar(400)) AS ExDesc") : "NULL AS ExDesc") + @",
    " + (colExtraAmt != null ? ("CAST(ISNULL(ee." + colExtraAmt + ",0) AS decimal(18,6)) AS AmountUsd") : "CAST(0 AS decimal(18,6)) AS AmountUsd") + @"
FROM dbo.T_OrderExtraExpense ee
INNER JOIN OrdersInPeriod o ON o.OrderNumber = ee.OrderNumber
ORDER BY ee.OrderNumber, ExDt;";

                using (var cmdExtra = new SqlCommand(sqlExtra, con))
                {
                    cmdExtra.Parameters.AddWithValue("@f", rangeFrom.Date);
                    cmdExtra.Parameters.AddWithValue("@t", rangeToExcl.Date);

                    using (var r = cmdExtra.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var ord = r["OrderNumber"]?.ToString();
                            var exDt = (r["ExDt"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["ExDt"]);
                            var desc = (r["ExDesc"] == DBNull.Value) ? "" : r["ExDesc"].ToString();
                            var amtUsd = (r["AmountUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["AmountUsd"]);

                            dto.ExtraExpenses.Add(new ExtraExpenseDto
                            {
                                OrderNumber = ord,
                                ExpenseDateText = exDt.HasValue ? exDt.Value.ToString("yyyy-MM-dd") : "-",
                                Description = string.IsNullOrWhiteSpace(desc) ? "-" : desc,
                                AmountUsd = amtUsd,
                                AmountPln = amtUsd * usdPlnRate
                            });
                        }
                    }
                }
            }

            return dto;
        }

        // =========================
        // Customers KPI
        // =========================
        public class CustomerKpiDto
        {
            public int UniqueCustomers { get; set; }
            public int NewCustomers { get; set; }
            public int ReturningCustomers { get; set; }
            public decimal ReturningRatePct { get; set; }
        }

        private CustomerKpiDto GetCustomerKpiFromOrders(DateTime from, DateTime toExclusive)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
;WITH valid AS (
    SELECT
        NULLIF(LTRIM(RTRIM(Email)), '') AS Email,
        CAST(OrderDate AS date) AS OrderDt
    FROM dbo.T_Order
    WHERE Email IS NOT NULL AND LTRIM(RTRIM(Email)) <> ''
      AND OrderDate IS NOT NULL
),
periodCustomers AS (
    SELECT DISTINCT Email
    FROM valid
    WHERE OrderDt >= @f AND OrderDt < @t
),
firstOrder AS (
    SELECT Email, MIN(OrderDt) AS FirstDt
    FROM valid
    GROUP BY Email
),
newCustomers AS (
    SELECT fo.Email
    FROM firstOrder fo
    WHERE fo.FirstDt >= @f AND fo.FirstDt < @t
)
SELECT
    (SELECT COUNT(*) FROM periodCustomers) AS UniqueCustomers,
    (SELECT COUNT(*) FROM newCustomers nc WHERE nc.Email IN (SELECT Email FROM periodCustomers)) AS NewCustomersInPeriod;", con))
            {
                cmd.Parameters.AddWithValue("@f", from.Date);
                cmd.Parameters.AddWithValue("@t", toExclusive.Date);

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return new CustomerKpiDto();

                    int unique = r["UniqueCustomers"] == DBNull.Value ? 0 : Convert.ToInt32(r["UniqueCustomers"]);
                    int newC = r["NewCustomersInPeriod"] == DBNull.Value ? 0 : Convert.ToInt32(r["NewCustomersInPeriod"]);
                    int returning = Math.Max(0, unique - newC);
                    decimal rate = (unique > 0) ? (100m * returning / unique) : 0m;

                    return new CustomerKpiDto
                    {
                        UniqueCustomers = unique,
                        NewCustomers = newC,
                        ReturningCustomers = returning,
                        ReturningRatePct = rate
                    };
                }
            }
        }

        // =========================
        // PRODUCT ANALYTICS (NEW: 1 + 2)
        // =========================
        public class ProductTypeSummaryDto
        {
            public string ProductType { get; set; }
            public int Qty { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
            public int UniqueSkus { get; set; }
            public int OrdersCount { get; set; }
        }

        public class TopSkuDto
        {
            public string ProductType { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public decimal RevenueOriginal { get; set; }
            public decimal RevenuePLN { get; set; }
        }

        public class ParetoDto
        {
            public List<string> Labels { get; set; } = new List<string>();
            public List<decimal> RevenuePLN { get; set; } = new List<decimal>();
            public List<decimal> CumPct { get; set; } = new List<decimal>();
        }

        public class StackDatasetDto2
        {
            public string Label { get; set; }
            public List<int> Data { get; set; } = new List<int>();
        }

        public class TypeMarketplaceDto
        {
            public List<string> Labels { get; set; } = new List<string>();          // ProductType
            public List<StackDatasetDto2> Datasets { get; set; } = new List<StackDatasetDto2>(); // Marketplaces
        }

        public class ProductAnalyticsDto
        {
            public int TotalQty { get; set; }
            public int DistinctTypes { get; set; }
            public int UniqueSkus { get; set; }

            public string TopTypeName { get; set; }
            public int TopTypeQty { get; set; }

            public bool IsPartial { get; set; }
            public List<ProductTypeSummaryDto> TypeSummary { get; set; } = new List<ProductTypeSummaryDto>();

            public List<TopSkuDto> TopSkus { get; set; } = new List<TopSkuDto>();
            public ParetoDto Pareto { get; set; } = new ParetoDto();
            public TypeMarketplaceDto TypeMarketplace { get; set; } = new TypeMarketplaceDto();
        }

        private ProductAnalyticsDto GetProductAnalytics(DateTime rangeFrom, DateTime rangeToExcl, decimal usdPlnRate, List<TopProductDto> topProducts)
        {
            var dto = new ProductAnalyticsDto();

            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                // Candidate sources for order-item level data (view/table)
                var candidates = new[]
                {
                    "dbo.V_OrderItemAgg",
                    "dbo.V_OrderItem",
                    "dbo.V_OrderItems",
                    "dbo.V_OrderDetail",
                    "dbo.V_OrderDetails",
                    "dbo.T_OrderItem",
                    "dbo.T_OrderItems",
                    "dbo.T_OrderDetail",
                    "dbo.T_OrderDetails"
                };

                string src = candidates.FirstOrDefault(x => ObjectExists(con, x));

                // Marketplace names
                Dictionary<int, string> mktNames = new Dictionary<int, string>();
                using (var cmd = new SqlCommand(@"SELECT MarketplaceID, Marketplace FROM dbo.T_Marketplace;", con))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int id = r["MarketplaceID"] == DBNull.Value ? 0 : Convert.ToInt32(r["MarketplaceID"]);
                        string name = r["Marketplace"] == DBNull.Value ? "" : r["Marketplace"].ToString();
                        if (id > 0 && !mktNames.ContainsKey(id)) mktNames[id] = name;
                    }
                }

                if (!string.IsNullOrEmpty(src))
                {
                    string shortName = src.Replace("dbo.", "");

                    string colOrderDate = PickFirstExisting(con, shortName, "OrderDate", "CreatedAt", "InsertDate", "Date");
                    string colOrderNo = PickFirstExisting(con, shortName, "OrderNumber", "OrderNo", "OrderId", "OrderID");
                    string colSku = PickFirstExisting(con, shortName, "SKU", "Sku", "SellerSku", "ProductSku");
                    string colType = PickFirstExisting(con, shortName, "ProductType", "TypeName", "ProductTypeName");
                    string colQty = PickFirstExisting(con, shortName, "Qty", "Quantity", "ItemQty");

                    string colRevUsd = PickFirstExisting(con, shortName,
                        "RevenueOriginal", "LineTotalOriginal", "TotalOriginal", "AmountOriginal", "LineTotalUsd", "TotalUsd", "AmountUsd");

                    string colRevPln = PickFirstExisting(con, shortName,
                        "RevenuePLN", "LineTotalPLN", "TotalPLN", "AmountPLN", "LineTotalPln", "TotalPln", "AmountPln");

                    bool ok = !string.IsNullOrEmpty(colOrderDate) && !string.IsNullOrEmpty(colType) && !string.IsNullOrEmpty(colQty);

                    if (ok)
                    {
                        // 1) Product type summary
                        string sqlType = @"
;WITH base AS (
    SELECT
        CAST(s." + colOrderDate + @" AS date) AS OrderDt,
        " + (string.IsNullOrEmpty(colOrderNo) ? "NULL" : ("CAST(s." + colOrderNo + " AS nvarchar(80))")) + @" AS OrderNumber,
        " + (string.IsNullOrEmpty(colSku) ? "NULL" : ("CAST(s." + colSku + " AS nvarchar(80))")) + @" AS SKU,
        CAST(s." + colType + @" AS nvarchar(120)) AS ProductType,
        CAST(ISNULL(s." + colQty + @",0) AS int) AS Qty,
        " + (string.IsNullOrEmpty(colRevUsd) ? "CAST(0 AS decimal(18,6))" : ("CAST(ISNULL(s." + colRevUsd + ",0) AS decimal(18,6))")) + @" AS RevUsd,
        " + (string.IsNullOrEmpty(colRevPln) ? "CAST(0 AS decimal(18,6))" : ("CAST(ISNULL(s." + colRevPln + ",0) AS decimal(18,6))")) + @" AS RevPln
    FROM " + src + @" s
    WHERE s." + colOrderDate + @" >= @f AND s." + colOrderDate + @" < @t
),
agg AS (
    SELECT
        NULLIF(LTRIM(RTRIM(ProductType)),'') AS ProductType,
        SUM(Qty) AS Qty,
        SUM(RevUsd) AS RevenueUsd,
        SUM(RevPln) AS RevenuePln,
        COUNT(DISTINCT NULLIF(LTRIM(RTRIM(SKU)),'')) AS UniqueSkus,
        COUNT(DISTINCT NULLIF(LTRIM(RTRIM(OrderNumber)),'')) AS OrdersCount
    FROM base
    GROUP BY NULLIF(LTRIM(RTRIM(ProductType)),'')
)
SELECT ISNULL(ProductType,'-') AS ProductType, Qty, RevenueUsd, RevenuePln, UniqueSkus, OrdersCount
FROM agg
ORDER BY Qty DESC, ProductType;";

                        using (var cmd = new SqlCommand(sqlType, con))
                        {
                            cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                            cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);

                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    string type = (r["ProductType"] == DBNull.Value) ? "-" : r["ProductType"].ToString();
                                    int qty = (r["Qty"] == DBNull.Value) ? 0 : Convert.ToInt32(r["Qty"]);
                                    decimal pln = (r["RevenuePln"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["RevenuePln"]);
                                    decimal usd = (r["RevenueUsd"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["RevenueUsd"]);
                                    if (usd == 0m && usdPlnRate > 0m && pln > 0m) usd = pln / usdPlnRate;

                                    int uniqSku = (r["UniqueSkus"] == DBNull.Value) ? 0 : Convert.ToInt32(r["UniqueSkus"]);
                                    int orders = (r["OrdersCount"] == DBNull.Value) ? 0 : Convert.ToInt32(r["OrdersCount"]);

                                    dto.TypeSummary.Add(new ProductTypeSummaryDto
                                    {
                                        ProductType = string.IsNullOrWhiteSpace(type) ? "-" : type,
                                        Qty = qty,
                                        RevenueOriginal = usd,
                                        RevenuePLN = pln,
                                        UniqueSkus = uniqSku,
                                        OrdersCount = orders
                                    });

                                    dto.TotalQty += qty;
                                }
                            }
                        }

                        dto.DistinctTypes = dto.TypeSummary.Count;

                        // Distinct SKUs
                        if (!string.IsNullOrEmpty(colSku))
                        {
                            string sqlSku = @"
SELECT COUNT(DISTINCT NULLIF(LTRIM(RTRIM(CAST(s." + colSku + @" AS nvarchar(80)))),'')) 
FROM " + src + @" s
WHERE s." + colOrderDate + @" >= @f AND s." + colOrderDate + @" < @t;";

                            using (var cmd = new SqlCommand(sqlSku, con))
                            {
                                cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                                cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);
                                dto.UniqueSkus = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                        }
                        else dto.UniqueSkus = dto.TypeSummary.Sum(x => x.UniqueSkus);

                        var topType = dto.TypeSummary.OrderByDescending(x => x.Qty).FirstOrDefault();
                        dto.TopTypeName = topType?.ProductType;
                        dto.TopTypeQty = topType?.Qty ?? 0;

                        // 2) Top SKUs + Pareto
                        if (!string.IsNullOrEmpty(colSku))
                        {
                            string sqlTopSku = @"
;WITH base AS (
    SELECT
        " + (string.IsNullOrEmpty(colOrderNo) ? "NULL" : ("CAST(s." + colOrderNo + " AS nvarchar(80))")) + @" AS OrderNumber,
        NULLIF(LTRIM(RTRIM(CAST(s." + colSku + @" AS nvarchar(80)))), '') AS SKU,
        NULLIF(LTRIM(RTRIM(CAST(s." + colType + @" AS nvarchar(120)))), '') AS ProductType,
        CAST(ISNULL(s." + colQty + @",0) AS int) AS Qty,
        " + (string.IsNullOrEmpty(colRevUsd) ? "CAST(0 AS decimal(18,6))" : ("CAST(ISNULL(s." + colRevUsd + ",0) AS decimal(18,6))")) + @" AS RevUsd,
        " + (string.IsNullOrEmpty(colRevPln) ? "CAST(0 AS decimal(18,6))" : ("CAST(ISNULL(s." + colRevPln + ",0) AS decimal(18,6))")) + @" AS RevPln
    FROM " + src + @" s
    WHERE s." + colOrderDate + @" >= @f AND s." + colOrderDate + @" < @t
      AND s." + colSku + @" IS NOT NULL
),
agg AS (
    SELECT
        ISNULL(ProductType,'-') AS ProductType,
        SKU,
        SUM(Qty) AS Qty,
        SUM(RevUsd) AS RevenueUsd,
        SUM(RevPln) AS RevenuePln
    FROM base
    WHERE SKU IS NOT NULL AND SKU <> ''
    GROUP BY ISNULL(ProductType,'-'), SKU
)
SELECT TOP (@top)
    ProductType, SKU, Qty, RevenueUsd, RevenuePln
FROM agg
ORDER BY RevenuePln DESC, RevenueUsd DESC, Qty DESC;";

                            var topN = 30;
                            using (var cmd = new SqlCommand(sqlTopSku, con))
                            {
                                cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                                cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);
                                cmd.Parameters.AddWithValue("@top", topN);

                                using (var r = cmd.ExecuteReader())
                                {
                                    while (r.Read())
                                    {
                                        string type = r["ProductType"] == DBNull.Value ? "-" : r["ProductType"].ToString();
                                        string sku = r["SKU"] == DBNull.Value ? "" : r["SKU"].ToString();
                                        int qty = r["Qty"] == DBNull.Value ? 0 : Convert.ToInt32(r["Qty"]);
                                        decimal pln = r["RevenuePln"] == DBNull.Value ? 0m : Convert.ToDecimal(r["RevenuePln"]);
                                        decimal usd = r["RevenueUsd"] == DBNull.Value ? 0m : Convert.ToDecimal(r["RevenueUsd"]);
                                        if (usd == 0m && usdPlnRate > 0m && pln > 0m) usd = pln / usdPlnRate;

                                        dto.TopSkus.Add(new TopSkuDto
                                        {
                                            ProductType = string.IsNullOrWhiteSpace(type) ? "-" : type,
                                            SKU = sku,
                                            Qty = qty,
                                            RevenueOriginal = usd,
                                            RevenuePLN = pln
                                        });
                                    }
                                }
                            }

                            var sumPln = dto.TopSkus.Sum(x => x.RevenuePLN);
                            if (sumPln > 0m)
                            {
                                decimal run = 0m;
                                foreach (var x in dto.TopSkus)
                                {
                                    run += x.RevenuePLN;
                                    dto.Pareto.Labels.Add(string.IsNullOrWhiteSpace(x.SKU) ? "-" : x.SKU);
                                    dto.Pareto.RevenuePLN.Add(x.RevenuePLN);
                                    dto.Pareto.CumPct.Add(Math.Round(100m * run / sumPln, 2));
                                }
                            }
                        }

                        // 3) Type × Marketplace stacked (needs OrderNumber)
                        if (!string.IsNullOrEmpty(colOrderNo))
                        {
                            string sqlTM = @"
;WITH items AS (
    SELECT
        CAST(s." + colOrderDate + @" AS date) AS OrderDt,
        NULLIF(LTRIM(RTRIM(CAST(s." + colOrderNo + @" AS nvarchar(80)))), '') AS OrderNumber,
        ISNULL(NULLIF(LTRIM(RTRIM(CAST(s." + colType + @" AS nvarchar(120)))), ''), '-') AS ProductType,
        CAST(ISNULL(s." + colQty + @",0) AS int) AS Qty
    FROM " + src + @" s
    WHERE s." + colOrderDate + @" >= @f AND s." + colOrderDate + @" < @t
),
j AS (
    SELECT
        i.ProductType,
        oa.MarketplaceID,
        SUM(i.Qty) AS Qty
    FROM items i
    INNER JOIN dbo.V_OrderAgg oa ON oa.OrderNumber = i.OrderNumber
    GROUP BY i.ProductType, oa.MarketplaceID
)
SELECT ProductType, MarketplaceID, Qty
FROM j
ORDER BY ProductType, MarketplaceID;";

                            var rows = new List<Tuple<string, int, int>>();
                            using (var cmd = new SqlCommand(sqlTM, con))
                            {
                                cmd.Parameters.AddWithValue("@f", rangeFrom.Date);
                                cmd.Parameters.AddWithValue("@t", rangeToExcl.Date);

                                using (var r = cmd.ExecuteReader())
                                {
                                    while (r.Read())
                                    {
                                        string type = r["ProductType"] == DBNull.Value ? "-" : r["ProductType"].ToString();
                                        int mid = r["MarketplaceID"] == DBNull.Value ? 0 : Convert.ToInt32(r["MarketplaceID"]);
                                        int qty = r["Qty"] == DBNull.Value ? 0 : Convert.ToInt32(r["Qty"]);
                                        if (mid > 0) rows.Add(Tuple.Create(type, mid, qty));
                                    }
                                }
                            }

                            var types = rows.Select(x => x.Item1).Distinct().OrderBy(x => x).ToList();
                            var mids = rows.Select(x => x.Item2).Distinct().OrderBy(x => x).ToList();

                            dto.TypeMarketplace.Labels = types;

                            foreach (var mid in mids)
                            {
                                var ds = new StackDatasetDto2 { Label = mktNames.ContainsKey(mid) ? mktNames[mid] : ("Marketplace " + mid) };
                                foreach (var t in types)
                                {
                                    int v = rows.Where(x => x.Item2 == mid && x.Item1 == t).Select(x => x.Item3).FirstOrDefault();
                                    ds.Data.Add(v);
                                }
                                dto.TypeMarketplace.Datasets.Add(ds);
                            }
                        }

                        dto.IsPartial = false;
                        return dto;
                    }
                }

                // ===== Fallback (partial): from TopProducts =====
                dto.IsPartial = true;

                var safe = (topProducts ?? new List<TopProductDto>())
                    .Where(x => !string.IsNullOrWhiteSpace(x.ProductType))
                    .ToList();

                var grouped = safe
                    .GroupBy(x => x.ProductType.Trim())
                    .Select(g => new ProductTypeSummaryDto
                    {
                        ProductType = g.Key,
                        Qty = g.Sum(z => z.Qty),
                        RevenuePLN = g.Sum(z => z.RevenuePLN),
                        RevenueOriginal = g.Sum(z => z.RevenueOriginal),
                        UniqueSkus = g.Select(z => (z.SKU ?? "").Trim()).Where(s => s != "").Distinct().Count(),
                        OrdersCount = 0
                    })
                    .OrderByDescending(x => x.Qty)
                    .ToList();

                dto.TypeSummary = grouped;
                dto.TotalQty = grouped.Sum(x => x.Qty);
                dto.DistinctTypes = grouped.Count;
                dto.UniqueSkus = safe.Select(x => (x.SKU ?? "").Trim()).Where(s => s != "").Distinct().Count();

                var top2 = grouped.FirstOrDefault();
                dto.TopTypeName = top2?.ProductType;
                dto.TopTypeQty = top2?.Qty ?? 0;

                dto.TopSkus = safe
                    .Where(x => !string.IsNullOrWhiteSpace(x.SKU))
                    .GroupBy(x => new { Type = x.ProductType.Trim(), Sku = x.SKU.Trim() })
                    .Select(g => new TopSkuDto
                    {
                        ProductType = g.Key.Type,
                        SKU = g.Key.Sku,
                        Qty = g.Sum(z => z.Qty),
                        RevenuePLN = g.Sum(z => z.RevenuePLN),
                        RevenueOriginal = g.Sum(z => z.RevenueOriginal)
                    })
                    .OrderByDescending(x => x.RevenuePLN)
                    .Take(30)
                    .ToList();

                var sumPln2 = dto.TopSkus.Sum(x => x.RevenuePLN);
                if (sumPln2 > 0m)
                {
                    decimal run2 = 0m;
                    foreach (var x in dto.TopSkus)
                    {
                        run2 += x.RevenuePLN;
                        dto.Pareto.Labels.Add(x.SKU);
                        dto.Pareto.RevenuePLN.Add(x.RevenuePLN);
                        dto.Pareto.CumPct.Add(Math.Round(100m * run2 / sumPln2, 2));
                    }
                }

                return dto;
            }
        }

        // =========================
        // Date helpers
        // =========================
        private static DateTime? ParseDateMulti(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim();

            DateTime dt;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt.Date;
            if (DateTime.TryParseExact(s, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt.Date;
            if (DateTime.TryParseExact(s, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt.Date;
            if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt)) return dt.Date;
            return null;
        }

        private static void GetRange(string period, string fromDate, string toDate, out DateTime from, out DateTime toExclusive)
        {
            var now = DateTime.Now.Date;

            if (string.IsNullOrWhiteSpace(period)) period = "MONTH";
            period = period.Trim().ToUpperInvariant();

            if (period == "TODAY") { from = now; toExclusive = now.AddDays(1); return; }
            if (period == "MONTH") { from = new DateTime(now.Year, now.Month, 1); toExclusive = from.AddMonths(1); return; }
            if (period == "YEAR") { from = new DateTime(now.Year, 1, 1); toExclusive = from.AddYears(1); return; }

            var fd = ParseDateMulti(fromDate) ?? new DateTime(now.Year, now.Month, 1);
            var td = ParseDateMulti(toDate) ?? now;
            if (td < fd) { var tmp = fd; fd = td; td = tmp; }

            from = fd.Date;
            toExclusive = td.Date.AddDays(1);
        }
    }
}