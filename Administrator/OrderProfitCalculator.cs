using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Feniks.Services
{
    public class FeeLine
    {
        public string Label { get; set; }
        public decimal AmountUsd { get; set; }

        public FeeLine() { }
        public FeeLine(string label, decimal amountUsd)
        {
            Label = label;
            AmountUsd = amountUsd;
        }
    }

    public class OrderProfitResult
    {
        public string Marketplace { get; set; }

        // Sales (Buyer)
        public decimal ItemTotalUsd { get; set; }
        public decimal ShippingBuyerUsd { get; set; }
        public decimal TaxBuyerUsd { get; set; }
        public decimal GiftWrapUsd { get; set; }

        public decimal GrossSalesUsd
        {
            get { return ItemTotalUsd + ShippingBuyerUsd + TaxBuyerUsd + GiftWrapUsd; }
        }

        // Fees
        public decimal MarketplaceFeesUsd { get; set; }
        public List<FeeLine> FeeLines { get; set; }

        // Costs
        public decimal ProductCostUsd { get; set; }
        public decimal ShippingCostUsd { get; set; }   // intermediate + main
        public decimal ExtraExpensesUsd { get; set; }

        public List<FeeLine> ShippingLines { get; set; }
        public List<FeeLine> ExtraExpenseLines { get; set; }

        public decimal TotalCostsUsd
        {
            get { return ProductCostUsd + ShippingCostUsd + ExtraExpensesUsd; }
        }

        // Net profit: (Item + ShipBuyer + GiftWrap) - fees - costs
        // TaxBuyer is informative only.
        public decimal NetProfitUsd
        {
            get { return (ItemTotalUsd + ShippingBuyerUsd + GiftWrapUsd) - MarketplaceFeesUsd - TotalCostsUsd; }
        }

        public OrderProfitResult()
        {
            Marketplace = "";
            FeeLines = new List<FeeLine>();
            ShippingLines = new List<FeeLine>();
            ExtraExpenseLines = new List<FeeLine>();
        }
    }

    public class OrderProfitCalculator
    {
        private readonly string _cs;

        public OrderProfitCalculator()
        {
            _cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        }

        public OrderProfitResult Calculate(string orderNumber)
        {
            OrderProfitResult res = new OrderProfitResult();

            // 1) Header
            OrderHeader hdr = LoadOrderHeader(orderNumber);
            res.Marketplace = hdr.Marketplace;
            res.ShippingBuyerUsd = hdr.ShippingBuyer;
            res.TaxBuyerUsd = hdr.Tax;
            res.GiftWrapUsd = hdr.GiftWrap;

            // 2) Items (ItemTotal + ProductCost)
            List<OrderItem> items = LoadOrderItems(orderNumber);

            decimal itemTotal = 0m;
            decimal productCost = 0m;

            for (int i = 0; i < items.Count; i++)
            {
                OrderItem it = items[i];
                itemTotal += it.Quantity * it.ItemPrice;

                // *** Unit cost: sadece stoktaki en son ItemPrice alınacak. WeightPrice ile çarpma yok.
                decimal unitCost = GetUnitCostFromStock(it.Sku);
                productCost += unitCost * it.Quantity;
            }

            res.ItemTotalUsd = itemTotal;
            res.ProductCostUsd = productCost;

            // 3) Fees (rule-based estimate)
            List<FeeLine> feeLines;
            decimal fees = CalculateMarketplaceFees(
                res.Marketplace,
                res.ItemTotalUsd,
                res.ShippingBuyerUsd,
                res.GrossSalesUsd,
                out feeLines);

            res.MarketplaceFeesUsd = fees;
            res.FeeLines = feeLines;

            // 4) Shipping legs (intermediate+main)
            ShippingCost ship = LoadShippingLegCosts(orderNumber);
            res.ShippingCostUsd = ship.Total;
            res.ShippingLines = ship.Lines;

            // 5) Extra expenses
            ExtraCost extra = LoadExtraExpenses(orderNumber);
            res.ExtraExpensesUsd = extra.Total;
            res.ExtraExpenseLines = extra.Lines;

            return res;
        }

        // ---------------- internal models ----------------

        private class OrderHeader
        {
            public string Marketplace;
            public decimal ShippingBuyer;
            public decimal Tax;
            public decimal GiftWrap;

            public OrderHeader()
            {
                Marketplace = "";
                ShippingBuyer = 0m;
                Tax = 0m;
                GiftWrap = 0m;
            }
        }

        private class OrderItem
        {
            public string Sku;
            public int Quantity;
            public decimal ItemPrice;

            public OrderItem()
            {
                Sku = "";
                Quantity = 0;
                ItemPrice = 0m;
            }
        }

        private class ShippingCost
        {
            public decimal Total;
            public List<FeeLine> Lines;

            public ShippingCost()
            {
                Total = 0m;
                Lines = new List<FeeLine>();
            }
        }

        private class ExtraCost
        {
            public decimal Total;
            public List<FeeLine> Lines;

            public ExtraCost()
            {
                Total = 0m;
                Lines = new List<FeeLine>();
            }
        }

        // ---------------- loaders ----------------

        private OrderHeader LoadOrderHeader(string orderNumber)
        {
            // marketplace from OrdersPage, buyer shipping/tax/giftwrap from T_Order
            const string sql = @"
SELECT TOP 1
    ISNULL(op.Marketplace,'') AS Marketplace,
    ISNULL(o.ShippingPrice,0) AS ShippingBuyer,
    ISNULL(o.Tax,0) AS TaxBuyer,
    ISNULL(o.GiftWrapPrice,0) AS GiftWrap
FROM dbo.T_Order o
LEFT JOIN dbo.T_OrdersPage op ON op.OrderNumber = o.OrderNumber
WHERE o.OrderNumber = @Order
ORDER BY o.OrderID DESC";

            OrderHeader h = new OrderHeader();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", orderNumber ?? "");
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return h;

                    h.Marketplace = dr["Marketplace"]?.ToString() ?? "";
                    h.ShippingBuyer = dr["ShippingBuyer"] != DBNull.Value ? Convert.ToDecimal(dr["ShippingBuyer"]) : 0m;
                    h.Tax = dr["TaxBuyer"] != DBNull.Value ? Convert.ToDecimal(dr["TaxBuyer"]) : 0m;
                    h.GiftWrap = dr["GiftWrap"] != DBNull.Value ? Convert.ToDecimal(dr["GiftWrap"]) : 0m;
                }
            }

            return h;
        }

        private List<OrderItem> LoadOrderItems(string orderNumber)
        {
            const string sql = @"
SELECT SKU, ISNULL(Quantity,0) AS Quantity, ISNULL(ItemPrice,0) AS ItemPrice
FROM dbo.T_OrderDetails
WHERE OrderNumber = @Order";

            List<OrderItem> list = new List<OrderItem>();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", orderNumber ?? "");
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        OrderItem it = new OrderItem();
                        it.Sku = dr["SKU"] == DBNull.Value ? "" : dr["SKU"].ToString();
                        it.Quantity = dr["Quantity"] != DBNull.Value ? Convert.ToInt32(dr["Quantity"]) : 0;
                        it.ItemPrice = dr["ItemPrice"] != DBNull.Value ? Convert.ToDecimal(dr["ItemPrice"]) : 0m;
                        list.Add(it);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Unit cost: stok giriş tablosundan SKU bazında "son" ItemPrice'ı getirir.
        /// Önemli: artık WeightPrice ile çarpma yok. Sadece ItemPrice kullanılır.
        /// </summary>
        private decimal GetUnitCostFromStock(string sku)
        {
            const string sql = @"
SELECT TOP 1
    ISNULL(ItemPrice,0) AS UnitCost
FROM dbo.T_StockReceipt
WHERE SKU = @SKU
ORDER BY PurchaseDate DESC, StockReceiptID DESC";

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku ?? "");
                con.Open();

                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                    return 0m;

                // SQL DECIMAL/VARCHAR olabilir; güvenli parse
                decimal cost;
                if (decimal.TryParse(o.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out cost))
                    return cost;

                // fallback: try local parse (örnek: comma decimal)
                if (decimal.TryParse(o.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out cost))
                    return cost;

                return 0m;
            }
        }


        private ShippingCost LoadShippingLegCosts(string orderNumber)
        {
            // Prefer new table. If no rows exist, fallback to old T_Shipping.ShippingPrice.
            const string sqlLegs = @"
SELECT LegType, ISNULL(ShippingPriceUsd,0) AS Price
FROM dbo.T_ShippingLeg
WHERE OrderNumber=@Order";

            ShippingCost sc = new ShippingCost();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(sqlLegs, con))
            {
                cmd.Parameters.AddWithValue("@Order", orderNumber ?? "");
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        byte leg = dr["LegType"] != DBNull.Value ? Convert.ToByte(dr["LegType"]) : (byte)0;
                        decimal price = dr["Price"] != DBNull.Value ? Convert.ToDecimal(dr["Price"]) : 0m;
                        sc.Total += price;

                        string label = (leg == 1) ? "Intermediate Shipping" : "Main Shipping";
                        sc.Lines.Add(new FeeLine(label, price));
                    }
                }
            }

            if (sc.Lines.Count == 0)
            {
                // fallback old
                const string sqlOld = @"
SELECT TOP 1 ISNULL(ShippingPrice,0)
FROM dbo.T_Shipping
WHERE OrderNumber=@Order
ORDER BY ShippingID DESC";

                using (SqlConnection con = new SqlConnection(_cs))
                using (SqlCommand cmd = new SqlCommand(sqlOld, con))
                {
                    cmd.Parameters.AddWithValue("@Order", orderNumber ?? "");
                    con.Open();
                    object o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                    {
                        decimal price = Convert.ToDecimal(o);
                        sc.Total = price;
                        if (price > 0m) sc.Lines.Add(new FeeLine("Main Shipping", price));
                    }
                }
            }

            return sc;
        }

        private ExtraCost LoadExtraExpenses(string orderNumber)
        {
            const string sql = @"
SELECT ExpenseType, ISNULL(Description,'') AS Description, ISNULL(AmountUsd,0) AS AmountUsd
FROM dbo.T_OrderExtraExpense
WHERE OrderNumber=@Order";

            ExtraCost ec = new ExtraCost();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Order", orderNumber ?? "");
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string type = dr["ExpenseType"] == DBNull.Value ? "Other" : dr["ExpenseType"].ToString();
                        string desc = dr["Description"] == DBNull.Value ? "" : dr["Description"].ToString();
                        decimal amt = dr["AmountUsd"] != DBNull.Value ? Convert.ToDecimal(dr["AmountUsd"]) : 0m;

                        ec.Total += amt;
                        ec.Lines.Add(new FeeLine(type + (string.IsNullOrWhiteSpace(desc) ? "" : (": " + desc)), amt));
                    }
                }
            }

            return ec;
        }

        // ---------------- fee rules ----------------

        private decimal CalculateMarketplaceFees(
            string marketplace,
            decimal itemTotal,
            decimal shippingBuyer,
            decimal grossOrderTotal,
            out List<FeeLine> lines)
        {
            lines = new List<FeeLine>();
            string mp = (marketplace ?? "").Trim().ToLowerInvariant();

            // hgerman.shop -> no marketplace fees
            if (mp.Contains("hgerman") || mp.Contains("shop"))
                return 0m;

            // ETSY example (based on your screenshot):
            // - Transaction fee: 6.5% item, 6.5% shipping
            // - Processing: 6.5% order total + fixed (adjustable)
            if (mp.Contains("etsy"))
            {
                decimal rate = 0.065m;
                decimal fixedFee = 0.25m;

                decimal feeItem = itemTotal * rate;
                decimal feeShip = shippingBuyer * rate;
                decimal feeProc = (grossOrderTotal * rate) + fixedFee;

                lines.Add(new FeeLine("Etsy Transaction Fee (Item)", feeItem));
                lines.Add(new FeeLine("Etsy Transaction Fee (Shipping)", feeShip));
                lines.Add(new FeeLine("Etsy Processing Fee", feeProc));

                return feeItem + feeShip + feeProc;
            }

            // AMAZON example:
            // - Referral fee item: 15% of item price
            // - Shipping commission: 15% of shipping
            if (mp.Contains("amazon"))
            {
                decimal rate = 0.15m;
                decimal feeItem = itemTotal * rate;
                decimal feeShip = shippingBuyer * rate;

                lines.Add(new FeeLine("Amazon Referral Fee (Item)", feeItem));
                lines.Add(new FeeLine("Amazon Shipping Commission", feeShip));

                return feeItem + feeShip;
            }

            // EBAY example:
            // - Final Value Fee ~ 15% (item+ship)
            // - International Fee ~ 1.55% (item+ship)
            // - Fixed per order ~ 0.40
            if (mp.Contains("ebay"))
            {
                decimal baseAmt = itemTotal + shippingBuyer;
                decimal finalValue = baseAmt * 0.15m;
                decimal intl = baseAmt * 0.0155m;
                decimal fixedFee = 0.40m;

                lines.Add(new FeeLine("eBay Final Value Fee", finalValue));
                lines.Add(new FeeLine("eBay International Fee", intl));
                lines.Add(new FeeLine("eBay Fixed Fee", fixedFee));

                return finalValue + intl + fixedFee;
            }

            return 0m;
        }
    }
}
