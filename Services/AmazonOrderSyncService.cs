using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;

namespace Feniks.Services
{
    public class AmazonOrderSyncService
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private readonly AmazonSpApiClient _client = new AmazonSpApiClient();
        private readonly TelegramService _telegram = new TelegramService();

        public void Sync(bool autoPromote = true, bool sendTelegram = true)
        {
            DateTime lastSyncUtc = GetLastSyncUtc();

            string nextToken = null;

            while (true)
            {
                JObject ordersRoot = _client.GetOrders(lastSyncUtc, nextToken);
                JArray orders = ordersRoot["payload"] != null ? ordersRoot["payload"]["Orders"] as JArray : null;

                if (orders == null || orders.Count == 0)
                    break;

                foreach (JObject order in orders)
                {
                    string amazonOrderId = ReadString(order, "AmazonOrderId");
                    if (string.IsNullOrWhiteSpace(amazonOrderId))
                        continue;

                    bool isNewOrder = !InboxOrderExists(amazonOrderId);

                    SaveOrder(order);
                    SyncOrderItems(amazonOrderId);

                    if (autoPromote)
                    {
                        try
                        {
                            PromoteToLamax(amazonOrderId);
                        }
                        catch
                        {
                            // promote hatası sync'i komple patlatmasın
                        }
                    }

                    if (sendTelegram && isNewOrder)
                    {
                        try
                        {
                            SendTelegramForOrder(amazonOrderId);
                        }
                        catch (Exception ex)
                        {
                            SaveSyncLog("ERROR", "Telegram send failed for order " + amazonOrderId + ". " + ex.Message);
                        }
                    }
                }

                nextToken = ordersRoot["payload"] != null && ordersRoot["payload"]["NextToken"] != null
                    ? ordersRoot["payload"]["NextToken"].ToString()
                    : null;

                if (string.IsNullOrWhiteSpace(nextToken))
                    break;
            }

            SaveSyncStateSuccess();
        }

        public void SyncOrderItems(string amazonOrderId)
        {
            string nextToken = null;

            while (true)
            {
                JObject itemsRoot = _client.GetOrderItems(amazonOrderId, nextToken);
                JArray items = itemsRoot["payload"] != null ? itemsRoot["payload"]["OrderItems"] as JArray : null;

                if (items == null || items.Count == 0)
                    break;

                foreach (JObject item in items)
                    SaveOrderItem(amazonOrderId, item);

                nextToken = itemsRoot["payload"] != null && itemsRoot["payload"]["NextToken"] != null
                    ? itemsRoot["payload"]["NextToken"].ToString()
                    : null;

                if (string.IsNullOrWhiteSpace(nextToken))
                    break;
            }
        }

        private void SaveOrder(JObject o)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_AmazonUpsertInboxOrder", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                JObject shippingAddress = o["ShippingAddress"] as JObject;
                JObject orderTotal = o["OrderTotal"] as JObject;
                JObject buyerInfo = o["BuyerInfo"] as JObject;

                cmd.Parameters.AddWithValue("@AmazonOrderId", (object)ReadString(o, "AmazonOrderId") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PurchaseDateUtc", (object)ReadDate(o, "PurchaseDate") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@LastUpdateDateUtc", (object)ReadDate(o, "LastUpdateDate") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OrderStatus", (object)ReadString(o, "OrderStatus") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FulfillmentChannel", (object)ReadString(o, "FulfillmentChannel") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SalesChannel", (object)ReadString(o, "SalesChannel") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShipServiceLevel", (object)ReadString(o, "ShipServiceLevel") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OrderTotalAmount", (object)ReadAmount(orderTotal) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@OrderTotalCurrency", (object)ReadString(orderTotal, "CurrencyCode") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MarketplaceId", (object)ReadString(o, "MarketplaceId") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", (object)ReadString(o, "BuyerEmail") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerName", (object)ReadString(buyerInfo, "BuyerName") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingName", (object)ReadString(shippingAddress, "Name") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingPhone", (object)ReadString(shippingAddress, "Phone") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddressLine1", (object)ReadString(shippingAddress, "AddressLine1") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddressLine2", (object)ReadString(shippingAddress, "AddressLine2") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AddressLine3", (object)ReadString(shippingAddress, "AddressLine3") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object)ReadString(shippingAddress, "City") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@County", (object)ReadString(shippingAddress, "County") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@District", (object)ReadString(shippingAddress, "District") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StateOrRegion", (object)ReadString(shippingAddress, "StateOrRegion") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PostalCode", (object)ReadString(shippingAddress, "PostalCode") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryCode", (object)ReadString(shippingAddress, "CountryCode") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPrime", ReadBool(o, "IsPrime"));
                cmd.Parameters.AddWithValue("@IsBusinessOrder", ReadBool(o, "IsBusinessOrder"));
                cmd.Parameters.AddWithValue("@RawJson", o.ToString(Formatting.None));

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveOrderItem(string amazonOrderId, JObject item)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_AmazonUpsertInboxOrderItem", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                JObject itemPrice = item["ItemPrice"] as JObject;
                JObject itemTax = item["ItemTax"] as JObject;
                JObject shippingPrice = item["ShippingPrice"] as JObject;
                JObject shippingTax = item["ShippingTax"] as JObject;
                JObject promotionDiscount = item["PromotionDiscount"] as JObject;

                cmd.Parameters.AddWithValue("@AmazonOrderId", amazonOrderId);
                cmd.Parameters.AddWithValue("@AmazonOrderItemId", (object)ReadString(item, "OrderItemId") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ASIN", (object)ReadString(item, "ASIN") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SellerSKU", (object)ReadString(item, "SellerSKU") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Title", (object)ReadString(item, "Title") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@QuantityOrdered", (object)ReadDecimal(item, "QuantityOrdered") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@QuantityShipped", (object)ReadDecimal(item, "QuantityShipped") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ItemPriceAmount", (object)ReadAmount(itemPrice) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ItemPriceCurrency", (object)ReadString(itemPrice, "CurrencyCode") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ItemTaxAmount", (object)ReadAmount(itemTax) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingPriceAmount", (object)ReadAmount(shippingPrice) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingTaxAmount", (object)ReadAmount(shippingTax) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PromotionDiscountAmount", (object)ReadAmount(promotionDiscount) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RawJson", item.ToString(Formatting.None));

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void PromoteToLamax(string amazonOrderId)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_Amazon_PromoteInboxOrderToLamax", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AmazonOrderId", amazonOrderId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool InboxOrderExists(string amazonOrderId)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.T_AmazonOrderInbox
WHERE AmazonOrderId = @AmazonOrderId;", con))
            {
                cmd.Parameters.AddWithValue("@AmazonOrderId", amazonOrderId);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void SendTelegramForOrder(string amazonOrderId)
        {
            if (!_telegram.IsConfigured)
                return;

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1
    O.AmazonOrderId,
    O.PurchaseDateUtc,
    O.OrderStatus,
    O.OrderTotalAmount,
    O.OrderTotalCurrency,
    O.SalesChannel,
    O.LamaxOrderNumber,
    I.SellerSKU,
    I.Title,
    I.QuantityOrdered
FROM dbo.T_AmazonOrderInbox O
LEFT JOIN dbo.T_AmazonOrderInboxItem I
    ON O.AmazonOrderId = I.AmazonOrderId
WHERE O.AmazonOrderId = @AmazonOrderId
  AND ISNULL(O.IsTelegramNotified, 0) = 0
ORDER BY I.AmazonOrderInboxItemID;", con))
            {
                cmd.Parameters.AddWithValue("@AmazonOrderId", amazonOrderId);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return;

                    string orderId = Convert.ToString(dr["AmazonOrderId"]);
                    string orderStatus = Convert.ToString(dr["OrderStatus"]);
                    string salesChannel = Convert.ToString(dr["SalesChannel"]);
                    string lamaxOrderNo = dr["LamaxOrderNumber"] == DBNull.Value ? "" : Convert.ToString(dr["LamaxOrderNumber"]);
                    string sellerSku = dr["SellerSKU"] == DBNull.Value ? "" : Convert.ToString(dr["SellerSKU"]);
                    string title = dr["Title"] == DBNull.Value ? "" : Convert.ToString(dr["Title"]);
                    string currency = dr["OrderTotalCurrency"] == DBNull.Value ? "" : Convert.ToString(dr["OrderTotalCurrency"]);
                    string total = dr["OrderTotalAmount"] == DBNull.Value ? "" : Convert.ToDecimal(dr["OrderTotalAmount"]).ToString("N2", CultureInfo.InvariantCulture);
                    string qty = dr["QuantityOrdered"] == DBNull.Value ? "" : Convert.ToDecimal(dr["QuantityOrdered"]).ToString("N0", CultureInfo.InvariantCulture);

                    string purchaseDateText = "";
                    if (dr["PurchaseDateUtc"] != DBNull.Value)
                    {
                        DateTime dt = Convert.ToDateTime(dr["PurchaseDateUtc"]).ToLocalTime();
                        purchaseDateText = dt.ToString("yyyy-MM-dd HH:mm");
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("🛒 <b>New Amazon Order</b>");
                    sb.AppendLine("");
                    sb.AppendLine("<b>Order No:</b> " + orderId);
                    if (!string.IsNullOrWhiteSpace(lamaxOrderNo))
                        sb.AppendLine("<b>LamaX No:</b> " + lamaxOrderNo);
                    sb.AppendLine("<b>Status:</b> " + orderStatus);
                    sb.AppendLine("<b>Date:</b> " + purchaseDateText);
                    sb.AppendLine("<b>Channel:</b> " + salesChannel);
                    sb.AppendLine("<b>Total:</b> " + total + " " + currency);

                    if (!string.IsNullOrWhiteSpace(sellerSku))
                        sb.AppendLine("<b>SKU:</b> " + sellerSku);

                    if (!string.IsNullOrWhiteSpace(qty))
                        sb.AppendLine("<b>Qty:</b> " + qty);

                    if (!string.IsNullOrWhiteSpace(title))
                        sb.AppendLine("<b>Title:</b> " + title);

                    _telegram.SendMessage(sb.ToString());
                }
            }

            using (SqlConnection con2 = new SqlConnection(_cs))
            using (SqlCommand cmd2 = new SqlCommand(@"
UPDATE dbo.T_AmazonOrderInbox
SET
    IsTelegramNotified = 1,
    TelegramNotifiedAtUtc = GETUTCDATE()
WHERE AmazonOrderId = @AmazonOrderId;", con2))
            {
                cmd2.Parameters.AddWithValue("@AmazonOrderId", amazonOrderId);
                con2.Open();
                cmd2.ExecuteNonQuery();
            }
        }

        private DateTime GetLastSyncUtc()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 LastSuccessUtc
FROM dbo.T_AmazonSyncState
WHERE MarketplaceId = @MarketplaceId;", con))
            {
                cmd.Parameters.AddWithValue("@MarketplaceId", _client.MarketplaceId);
                con.Open();

                object obj = cmd.ExecuteScalar();

                if (obj == null || obj == DBNull.Value)
                    return DateTime.UtcNow.AddDays(-30);

                return Convert.ToDateTime(obj).ToUniversalTime().AddMinutes(-10);
            }
        }

        private void SaveSyncStateSuccess()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.T_AmazonSyncState WHERE MarketplaceId = @MarketplaceId)
BEGIN
    UPDATE dbo.T_AmazonSyncState
    SET
        LastSyncUtc = GETUTCDATE(),
        LastSuccessUtc = GETUTCDATE(),
        LastError = NULL,
        UpdatedAtUtc = GETUTCDATE()
    WHERE MarketplaceId = @MarketplaceId;
END
ELSE
BEGIN
    INSERT INTO dbo.T_AmazonSyncState (MarketplaceId, LastSyncUtc, LastSuccessUtc, LastError, UpdatedAtUtc)
    VALUES (@MarketplaceId, GETUTCDATE(), GETUTCDATE(), NULL, GETUTCDATE());
END

INSERT INTO dbo.T_AmazonSyncLog(MarketplaceId, LogType, Message, CreatedAtUtc)
VALUES(@MarketplaceId, 'INFO', 'Amazon order sync completed.', GETUTCDATE());", con))
            {
                cmd.Parameters.AddWithValue("@MarketplaceId", _client.MarketplaceId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void SaveSyncError(string message)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_cs))
                using (SqlCommand cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.T_AmazonSyncState WHERE MarketplaceId = @MarketplaceId)
BEGIN
    UPDATE dbo.T_AmazonSyncState
    SET
        LastSyncUtc = GETUTCDATE(),
        LastError = @Msg,
        UpdatedAtUtc = GETUTCDATE()
    WHERE MarketplaceId = @MarketplaceId;
END
ELSE
BEGIN
    INSERT INTO dbo.T_AmazonSyncState (MarketplaceId, LastSyncUtc, LastError, UpdatedAtUtc)
    VALUES (@MarketplaceId, GETUTCDATE(), @Msg, GETUTCDATE());
END

INSERT INTO dbo.T_AmazonSyncLog(MarketplaceId, LogType, Message, CreatedAtUtc)
VALUES(@MarketplaceId, 'ERROR', @Msg, GETUTCDATE());", con))
                {
                    cmd.Parameters.AddWithValue("@MarketplaceId", _client.MarketplaceId);
                    cmd.Parameters.AddWithValue("@Msg", (object)message ?? DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private void SaveSyncLog(string logType, string message)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.T_AmazonSyncLog(MarketplaceId, LogType, Message, CreatedAtUtc)
VALUES(@MarketplaceId, @LogType, @Message, GETUTCDATE());", con))
            {
                cmd.Parameters.AddWithValue("@MarketplaceId", _client.MarketplaceId);
                cmd.Parameters.AddWithValue("@LogType", logType);
                cmd.Parameters.AddWithValue("@Message", message ?? "");
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string ReadString(JObject obj, params string[] names)
        {
            if (obj == null) return null;

            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                    return obj[name].ToString();
            }

            return null;
        }

        private decimal? ReadAmount(JObject obj)
        {
            if (obj == null || obj["Amount"] == null || obj["Amount"].Type == JTokenType.Null)
                return null;

            decimal value;
            if (decimal.TryParse(obj["Amount"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return value;

            return null;
        }

        private decimal? ReadDecimal(JObject obj, params string[] names)
        {
            if (obj == null) return null;

            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                {
                    decimal value;
                    if (decimal.TryParse(obj[name].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        return value;
                }
            }

            return null;
        }

        private DateTime? ReadDate(JObject obj, params string[] names)
        {
            if (obj == null) return null;

            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                {
                    DateTime dt;
                    if (DateTime.TryParse(obj[name].ToString(), null, DateTimeStyles.AdjustToUniversal, out dt))
                        return dt.ToUniversalTime();
                }
            }

            return null;
        }

        private bool ReadBool(JObject obj, params string[] names)
        {
            if (obj == null) return false;

            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                {
                    bool value;
                    if (bool.TryParse(obj[name].ToString(), out value))
                        return value;
                }
            }

            return false;
        }
    }
}