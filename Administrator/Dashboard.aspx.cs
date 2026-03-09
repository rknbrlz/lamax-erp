using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Services;

namespace Feniks.Administrator
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e) { }

        // =========================
        // DTOs
        // =========================
        public class DashboardAllDto
        {
            public KpiCardsDto Kpi { get; set; }
            public List<TrendPointDto> Trend { get; set; }
            public List<MarketDto> Marketplace { get; set; }
            public List<CountryDto> Country { get; set; }
            public List<StateDto> USStates { get; set; }
            public List<RepeatCustomerDto> RepeatCustomers { get; set; }
            public List<TopProductDto> TopProducts { get; set; }
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

        public class StateDto
        {
            public int StateID { get; set; }
            public string State { get; set; }
            public string StateShort { get; set; }
            public int OrdersCount { get; set; }
            public decimal RevenuePLN { get; set; }
        }

        public class RepeatCustomerDto
        {
            public string CustomerKey { get; set; }
            public string BuyerFullName { get; set; }
            public string Email { get; set; }
            public int OrdersCount { get; set; }
            public decimal RevenuePLN { get; set; }
            public DateTime FirstOrder { get; set; }
            public DateTime LastOrder { get; set; }
        }

        public class TopProductDto
        {
            public int? ProductTypeID { get; set; }
            public string ProductType { get; set; }
            public string SKU { get; set; }
            public int Qty { get; set; }
            public decimal RevenuePLN { get; set; }
            public string ImageName { get; set; }
            public string ContentType { get; set; }
        }

        [WebMethod]
        public static string TestDbConn()
        {
            string cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (var con = new SqlConnection(cs))
            using (var cmd = new SqlCommand("SELECT SUSER_SNAME() AS LoginName, DB_NAME() AS DbName", con))
            {
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    r.Read();
                    return "OK | Login=" + r["LoginName"] + " | DB=" + r["DbName"];
                }
            }
        }


        // =========================
        // Data access
        // =========================
        // 🔥 BURASI ÖNEMLİ: Senin sistemin Login'de "constr" kullanıyor.
        // Dashboard da aynı connection string'i kullanmalı.
        private static string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        private static DateTime? ParseDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.Date;
            return null;
        }

        [WebMethod]
        public static DashboardAllDto GetDashboardAll(string period, string fromDate, string toDate)
        {
            var dto = new DashboardAllDto
            {
                Kpi = new KpiCardsDto(),
                Trend = new List<TrendPointDto>(),
                Marketplace = new List<MarketDto>(),
                Country = new List<CountryDto>(),
                USStates = new List<StateDto>(),
                RepeatCustomers = new List<RepeatCustomerDto>(),
                TopProducts = new List<TopProductDto>()
            };

            try
            {
                using (var con = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("dbo.Dashboard_GetAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Period", (object)(period ?? "MONTH"));

                    var fd = ParseDate(fromDate);
                    var td = ParseDate(toDate);
                    cmd.Parameters.AddWithValue("@FromDate", (object)fd ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", (object)td ?? DBNull.Value);

                    // US otomatik bulunsun
                    cmd.Parameters.AddWithValue("@USCountryID", DBNull.Value);

                    cmd.Parameters.AddWithValue("@TopProducts", 30);
                    cmd.Parameters.AddWithValue("@TopRepeat", 50);
                    cmd.Parameters.AddWithValue("@TopStates", 30);

                    con.Open();

                    using (var r = cmd.ExecuteReader())
                    {
                        // #1 KPI
                        if (r.Read())
                        {
                            dto.Kpi.OrdersCount = r["OrdersCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["OrdersCount"]);
                            dto.Kpi.ItemsCount = r["ItemsCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["ItemsCount"]);
                            dto.Kpi.RevenueOriginal = r["RevenueOriginal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenueOriginal"]);
                            dto.Kpi.RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"]);
                            dto.Kpi.AOV_Original = r["AOV_Original"] == DBNull.Value ? 0 : Convert.ToDecimal(r["AOV_Original"]);
                            dto.Kpi.AOV_PLN = r["AOV_PLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["AOV_PLN"]);
                        }

                        // #2 Trend
                        if (r.NextResult())
                        {
                            while (r.Read())
                            {
                                var key = Convert.ToDateTime(r["PeriodKey"]);
                                dto.Trend.Add(new TrendPointDto
                                {
                                    Label = key.ToString("yyyy-MM-dd HH:mm"),
                                    RevenueOriginal = r["RevenueOriginal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenueOriginal"]),
                                    RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"]),
                                    OrdersCount = r["OrdersCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["OrdersCount"])
                                });
                            }
                        }

                        // #3 Marketplace
                        if (r.NextResult())
                        {
                            while (r.Read())
                            {
                                dto.Marketplace.Add(new MarketDto
                                {
                                    MarketplaceID = r["MarketplaceID"] == DBNull.Value ? 0 : Convert.ToInt32(r["MarketplaceID"]),
                                    Marketplace = r["Marketplace"]?.ToString(),
                                    OrdersCount = r["OrdersCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["OrdersCount"]),
                                    RevenueOriginal = r["RevenueOriginal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenueOriginal"]),
                                    RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"])
                                });
                            }
                        }

                        // #4 Country
                        if (r.NextResult())
                        {
                            while (r.Read())
                            {
                                dto.Country.Add(new CountryDto
                                {
                                    CountryID = r["CountryID"] == DBNull.Value ? 0 : Convert.ToInt32(r["CountryID"]),
                                    Country = r["Country"]?.ToString(),
                                    OrdersCount = r["OrdersCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["OrdersCount"]),
                                    RevenueOriginal = r["RevenueOriginal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenueOriginal"]),
                                    RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"])
                                });
                            }
                        }

                        // #5 US States
                        if (r.NextResult())
                        {
                            while (r.Read())
                            {
                                dto.USStates.Add(new StateDto
                                {
                                    StateID = r["StateID"] == DBNull.Value ? 0 : Convert.ToInt32(r["StateID"]),
                                    State = r["State"]?.ToString(),
                                    StateShort = r["StateShort"]?.ToString(),
                                    OrdersCount = r["OrdersCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["OrdersCount"]),
                                    RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"])
                                });
                            }
                        }

                        // #6 Repeat
                        if (r.NextResult())
                        {
                            while (r.Read())
                            {
                                dto.RepeatCustomers.Add(new RepeatCustomerDto
                                {
                                    CustomerKey = r["CustomerKey"]?.ToString(),
                                    BuyerFullName = r["BuyerFullName"]?.ToString(),
                                    Email = r["Email"]?.ToString(),
                                    OrdersCount = r["OrdersCount"] == DBNull.Value ? 0 : Convert.ToInt32(r["OrdersCount"]),
                                    RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"]),
                                    FirstOrder = r["FirstOrder"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["FirstOrder"]),
                                    LastOrder = r["LastOrder"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["LastOrder"])
                                });
                            }
                        }

                        // #7 Top Products
                        if (r.NextResult())
                        {
                            while (r.Read())
                            {
                                dto.TopProducts.Add(new TopProductDto
                                {
                                    ProductTypeID = r["ProductTypeID"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["ProductTypeID"]),
                                    ProductType = r["ProductType"]?.ToString(),
                                    SKU = r["SKU"]?.ToString(),
                                    Qty = r["Qty"] == DBNull.Value ? 0 : Convert.ToInt32(r["Qty"]),
                                    RevenuePLN = r["RevenuePLN"] == DBNull.Value ? 0 : Convert.ToDecimal(r["RevenuePLN"]),
                                    ImageName = r["ImageName"]?.ToString(),
                                    ContentType = r["ContentType"]?.ToString()
                                });
                            }
                        }
                    }
                }

                return dto;
            }
            catch (SqlException ex)
            {
                // 🔎 Hata mesajını net görelim (şimdilik)
                throw new Exception("SQL ERROR: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("APP ERROR: " + ex.Message);
            }
        }
    }
}
