using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Feniks.Services
{
    public class EtsyOrderSyncService
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private readonly EtsyApiClient _client = new EtsyApiClient();

        public void Sync(bool onlyOpenOrders = true)
        {
            int offset = 0;
            int limit = 100;

            while (true)
            {
                JObject receiptsRoot = _client.GetReceipts(limit, offset, onlyOpenOrders);
                JArray results = receiptsRoot["results"] as JArray;

                if (results == null || results.Count == 0)
                    break;

                foreach (JObject receipt in results)
                {
                    SaveReceipt(receipt);

                    long receiptId = receipt["receipt_id"] != null ? (long)receipt["receipt_id"] : 0;
                    if (receiptId > 0)
                    {
                        JObject txRoot = _client.GetReceiptTransactions(receiptId);
                        JArray txResults = txRoot["results"] as JArray;

                        if (txResults != null)
                        {
                            foreach (JObject tx in txResults)
                                SaveReceiptTransaction(receiptId, tx);
                        }
                    }
                }

                if (results.Count < limit)
                    break;

                offset += limit;
            }

            SaveSyncStateSuccess();
        }

        private void SaveReceipt(JObject r)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_EtsyUpsertInboxOrder", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                string countryIso = ReadString(r, "country_iso");
                decimal? grandTotal = ReadDecimal(r, "grandtotal", "grand_total");
                decimal? subtotal = ReadDecimal(r, "subtotal");
                decimal? taxCost = ReadDecimal(r, "total_tax_cost", "tax_cost");
                decimal? shippingCost = ReadDecimal(r, "total_shipping_cost", "shipping_cost");
                decimal? discountAmt = ReadDecimal(r, "discount_amt", "discount_amount");
                long? createdTs = ReadLong(r, "create_timestamp", "created_timestamp");
                long? updatedTs = ReadLong(r, "update_timestamp", "updated_timestamp");

                cmd.Parameters.AddWithValue("@ShopId", _client.ShopId);
                cmd.Parameters.AddWithValue("@ReceiptId", ReadLong(r, "receipt_id") ?? 0);
                cmd.Parameters.AddWithValue("@ReceiptNumber", (object)ReadString(r, "receipt_id") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerUserId", (object)ReadLong(r, "buyer_user_id") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerName", (object)BuildBuyerName(r) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BuyerEmail", (object)ReadString(r, "buyer_email") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstLine", (object)ReadString(r, "first_line") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SecondLine", (object)ReadString(r, "second_line") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object)ReadString(r, "city") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@StateRegion", (object)ReadString(r, "state") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ZipCode", (object)ReadString(r, "zip") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryIso", (object)countryIso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrencyCode", (object)ReadString(r, "currency_code") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GrandTotal", (object)grandTotal ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Subtotal", (object)subtotal ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TaxCost", (object)taxCost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ShippingCost", (object)shippingCost ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiscountAmt", (object)discountAmt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@WasPaid", ReadBool(r, "was_paid"));
                cmd.Parameters.AddWithValue("@WasShipped", ReadBool(r, "was_shipped"));
                cmd.Parameters.AddWithValue("@StatusText", (object)ReadString(r, "status") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedTimestamp", (object)createdTs ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedTimestamp", (object)updatedTs ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedAtUtc", (object)UnixToUtc(createdTs) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedAtUtc", (object)UnixToUtc(updatedTs) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RawJson", r.ToString(Formatting.None));

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveReceiptTransaction(long receiptId, JObject tx)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_EtsyUpsertInboxOrderItem", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                cmd.Parameters.AddWithValue("@TransactionId", ReadLong(tx, "transaction_id") ?? 0);
                cmd.Parameters.AddWithValue("@ListingId", (object)ReadLong(tx, "listing_id") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Title", (object)ReadString(tx, "title") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sku", (object)ReadSku(tx) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Quantity", (object)ReadDecimal(tx, "quantity") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Price", (object)ReadDecimal(tx, "price") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CurrencyCode", (object)ReadString(tx, "currency_code") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@VariationsJson", (object)ReadNestedJson(tx, "variations") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PersonalizationJson", (object)ReadNestedJson(tx, "personalization") ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RawJson", tx.ToString(Formatting.None));

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void PromoteToLamax(long receiptId)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_Etsy_PromoteInboxOrderToLamax", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ReceiptId", receiptId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveSyncStateSuccess()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.T_EtsySyncState WHERE ShopId = @ShopId)
BEGIN
    UPDATE dbo.T_EtsySyncState
    SET LastSyncUtc = GETUTCDATE(),
        LastSuccessUtc = GETUTCDATE(),
        LastError = NULL,
        UpdatedAtUtc = GETUTCDATE()
    WHERE ShopId = @ShopId;
END
ELSE
BEGIN
    INSERT INTO dbo.T_EtsySyncState(ShopId, LastSyncUtc, LastSuccessUtc, LastError)
    VALUES(@ShopId, GETUTCDATE(), GETUTCDATE(), NULL);
END", con))
            {
                cmd.Parameters.AddWithValue("@ShopId", _client.ShopId);
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
IF EXISTS (SELECT 1 FROM dbo.T_EtsySyncState WHERE ShopId = @ShopId)
BEGIN
    UPDATE dbo.T_EtsySyncState
    SET LastSyncUtc = GETUTCDATE(),
        LastError = @Msg,
        UpdatedAtUtc = GETUTCDATE()
    WHERE ShopId = @ShopId;
END
ELSE
BEGIN
    INSERT INTO dbo.T_EtsySyncState(ShopId, LastSyncUtc, LastError)
    VALUES(@ShopId, GETUTCDATE(), @Msg);
END

INSERT INTO dbo.T_EtsySyncLog(ShopId, LogType, Message)
VALUES(@ShopId, 'ERROR', @Msg);", con))
                {
                    cmd.Parameters.AddWithValue("@ShopId", _client.ShopId);
                    cmd.Parameters.AddWithValue("@Msg", message ?? "");
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
            }
        }

        private string BuildBuyerName(JObject r)
        {
            string name = ReadString(r, "name");
            if (!string.IsNullOrWhiteSpace(name))
                return name;

            string first = ReadString(r, "first_name");
            string last = ReadString(r, "last_name");
            return (first + " " + last).Trim();
        }

        private string ReadSku(JObject tx)
        {
            JToken sku = tx["sku"];
            if (sku == null) return null;

            if (sku.Type == JTokenType.Array)
            {
                JArray arr = (JArray)sku;
                if (arr.Count > 0) return arr[0].ToString();
                return null;
            }

            return sku.ToString();
        }

        private string ReadNestedJson(JObject obj, string name)
        {
            return obj[name] != null ? obj[name].ToString(Formatting.None) : null;
        }

        private string ReadString(JObject obj, params string[] names)
        {
            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                    return obj[name].ToString();
            }
            return null;
        }

        private long? ReadLong(JObject obj, params string[] names)
        {
            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                {
                    long v;
                    if (long.TryParse(obj[name].ToString(), out v))
                        return v;
                }
            }
            return null;
        }

        private decimal? ReadDecimal(JObject obj, params string[] names)
        {
            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                {
                    decimal v;
                    if (decimal.TryParse(obj[name].ToString(), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out v))
                        return v;
                }
            }
            return null;
        }

        private bool ReadBool(JObject obj, params string[] names)
        {
            foreach (string name in names)
            {
                if (obj[name] != null && obj[name].Type != JTokenType.Null)
                {
                    bool v;
                    if (bool.TryParse(obj[name].ToString(), out v))
                        return v;

                    if (obj[name].ToString() == "1") return true;
                    if (obj[name].ToString() == "0") return false;
                }
            }
            return false;
        }

        private DateTime? UnixToUtc(long? ts)
        {
            if (!ts.HasValue) return null;
            try
            {
                return DateTimeOffset.FromUnixTimeSeconds(ts.Value).UtcDateTime;
            }
            catch
            {
                return null;
            }
        }
    }
}