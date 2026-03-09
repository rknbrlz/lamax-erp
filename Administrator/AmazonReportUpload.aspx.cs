using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class AmazonAccountingReport : Page
    {
        private static string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (fuTxt == null || !fuTxt.HasFile)
            {
                lblMsg.Text = "<span class='msg-err'>Please upload a TXT report.</span>";
                return;
            }

            string ext = Path.GetExtension(fuTxt.FileName ?? "").ToLowerInvariant();
            if (ext != ".txt" && ext != ".tsv" && ext != ".csv")
            {
                // Amazon bazen .txt verir, bazen tab-delimited. Yine de izin verelim:
                // lblMsg.Text = "<span class='msg-err'>Only .txt is allowed.</span>";
                // return;
            }

            try
            {
                string content;
                using (var sr = new StreamReader(fuTxt.PostedFile.InputStream, Encoding.UTF8, true))
                    content = sr.ReadToEnd();

                // Amazon raporları genelde TAB-delimited olur.
                // Eğer tab yoksa virgül dene.
                char delim = content.Contains("\t") ? '\t' : ',';

                var src = ParseDelimited(content, delim);
                if (src.Rows.Count == 0)
                    throw new Exception("No rows found in the uploaded file.");

                // order-id kolonunu bul
                int cOrderId = FindColumn(src, new[] { "order-id", "order id" });
                if (cOrderId < 0)
                    throw new Exception("Column 'order-id' not found.");

                // order-id listesi
                var orderIds = src.AsEnumerable()
                                  .Select(r => SafeStr(r[cOrderId]).Trim())
                                  .Where(x => x.Length > 0)
                                  .Distinct()
                                  .ToList();

                // Delivery/Tracking map
                var trackingMap = LoadTrackingNumbers(orderIds);

                // Output: sabit kolonlu tablo
                var outDt = BuildAccountingExport(src, trackingMap);

                // Preview (first 25)
                gvPreview.DataSource = outDt.AsEnumerable().Take(25).CopyToDataTable();
                gvPreview.DataBind();

                // TSV indir
                var tsv = ToDelimited(outDt, '\t');
                var fileName = "AmazonAccounting_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".tsv";
                DownloadText(tsv, fileName, "text/tab-separated-values");

                lblMsg.Text = "<span class='msg-ok'>Generated.</span>";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "<span class='msg-err'>ERROR: " + Server.HtmlEncode(ex.Message) + "</span>";
            }
        }

        // =========================
        // OUTPUT BUILDER (SENİN İSTEDİĞİN FORMAT)
        // =========================
        private DataTable BuildAccountingExport(DataTable src, Dictionary<string, string> trackingMap)
        {
            int cOrderId = FindColumn(src, new[] { "order-id", "order id" });
            int cBuyer = FindColumn(src, new[] { "buyer-name", "buyer name" });
            int cSku = FindColumn(src, new[] { "sku" });
            int cProd = FindColumn(src, new[] { "product-name", "product name" });
            int cQty = FindColumn(src, new[] { "quantity-purchased", "quantity purchased", "qty", "quantity" });
            int cCurr = FindColumn(src, new[] { "currency" });

            int cItemPrice = FindColumn(src, new[] { "item-price", "item price" });
            int cItemTax = FindColumn(src, new[] { "item-tax", "item tax" });
            int cShipPrice = FindColumn(src, new[] { "shipping-price", "shipping price" });
            int cShipTax = FindColumn(src, new[] { "shipping-tax", "shipping tax" });

            // Amazon header bazen "order-total" / "order total"
            int cOrderTot = FindColumn(src, new[] { "order total", "order-total", "order_total" });

            int cPurchase = FindColumn(src, new[] { "purchase-date", "purchase date" });
            int cRecipient = FindColumn(src, new[] { "recipient-name", "recipient name" });
            int cAddr1 = FindColumn(src, new[] { "ship-address-1", "ship address 1", "ship-address1" });
            int cCity = FindColumn(src, new[] { "ship-city", "ship city" });
            int cState = FindColumn(src, new[] { "ship-state", "ship state" });
            int cPostal = FindColumn(src, new[] { "ship-postal-code", "ship postal code", "postal-code", "zip" });
            int cCountry = FindColumn(src, new[] { "ship-country", "ship country" });
            int cChannel = FindColumn(src, new[] { "sales-channel", "sales channel" });

            // CC599C durum (bazı raporlarda ayrı kolon olur)
            int cDocStatus = FindColumn(src, new[] { "cc599c", "document status", "cc599c document status" });

            if (cOrderId < 0)
                throw new Exception("order-id column not found.");

            // ✅ Output tabloda sabit kolonlar + çok satırlı header
            var dt = new DataTable("AccountingExport");

            dt.Columns.Add("order-id", typeof(string));
            dt.Columns.Add("Delivery Number\n(Shipping Tracking Number)", typeof(string));
            dt.Columns.Add("CC599C\nDocument Status", typeof(string));
            dt.Columns.Add("purchase-date", typeof(string));          // date only
            dt.Columns.Add("purchase-date ", typeof(string));         // timestamp (aynı isim görünümü için boşluk)
            dt.Columns.Add("buyer-name", typeof(string));
            dt.Columns.Add("sku", typeof(string));
            dt.Columns.Add("product-name", typeof(string));
            dt.Columns.Add("quantity-purchased", typeof(string));
            dt.Columns.Add("currency", typeof(string));
            dt.Columns.Add("item-price", typeof(string));
            dt.Columns.Add("item-tax", typeof(string));
            dt.Columns.Add("shipping-price", typeof(string));
            dt.Columns.Add("shipping-tax", typeof(string));
            dt.Columns.Add("Order Total", typeof(string));
            dt.Columns.Add("Average Exchange Rate\n(NBP Table A) day -1 ", typeof(string));
            dt.Columns.Add("Order Total\n(PLN)", typeof(string));
            dt.Columns.Add("recipient-name", typeof(string));
            dt.Columns.Add("ship-address-1", typeof(string));
            dt.Columns.Add("ship-city", typeof(string));
            dt.Columns.Add("ship-state", typeof(string));
            dt.Columns.Add("ship-postal-code", typeof(string));
            dt.Columns.Add("ship-country", typeof(string));
            dt.Columns.Add("sales-channel", typeof(string));

            // Excel’de TR/PL gibi virgül gösterimi için pl-PL
            var ci = CultureInfo.GetCultureInfo("pl-PL");

            decimal sumUsd = 0m;
            decimal sumPln = 0m;

            foreach (DataRow r in src.Rows)
            {
                var orderId = SafeStr(r[cOrderId]).Trim();
                if (orderId.Length == 0) continue;

                trackingMap.TryGetValue(orderId, out string tracking);

                string purchaseRaw = (cPurchase >= 0) ? SafeStr(r[cPurchase]).Trim() : "";
                DateTime? purchaseDt = TryParseDate(purchaseRaw);
                string purchaseDateOnly = purchaseDt.HasValue ? purchaseDt.Value.ToString("yyyy-MM-dd") : "";
                string purchaseTimestamp = purchaseRaw;

                decimal orderTotalUsd = (cOrderTot >= 0) ? (TryParseDecimal(SafeStr(r[cOrderTot])) ?? 0m) : 0m;

                // FX = purchase date -1 day
                decimal fx = 0m;
                if (purchaseDt.HasValue)
                {
                    var fxRow = GetFxUsdPln_DayMinus1(purchaseDt.Value.Date);
                    if (fxRow.HasValue) fx = fxRow.Value.Rate;
                }

                decimal orderTotalPln = (fx > 0m) ? Math.Round(orderTotalUsd * fx, 2) : 0m;

                sumUsd += orderTotalUsd;
                sumPln += orderTotalPln;

                string docStatus = (cDocStatus >= 0) ? SafeStr(r[cDocStatus]).Trim() : "";

                dt.Rows.Add(
                    orderId,
                    tracking ?? "",
                    docStatus,
                    purchaseDateOnly,
                    purchaseTimestamp,
                    (cBuyer >= 0) ? SafeStr(r[cBuyer]).Trim() : "",
                    (cSku >= 0) ? SafeStr(r[cSku]).Trim() : "",
                    (cProd >= 0) ? SafeStr(r[cProd]).Trim() : "",
                    (cQty >= 0) ? SafeStr(r[cQty]).Trim() : "",
                    (cCurr >= 0) ? SafeStr(r[cCurr]).Trim() : "USD",
                    (cItemPrice >= 0) ? SafeStr(r[cItemPrice]).Trim() : "",
                    (cItemTax >= 0) ? SafeStr(r[cItemTax]).Trim() : "",
                    (cShipPrice >= 0) ? SafeStr(r[cShipPrice]).Trim() : "",
                    (cShipTax >= 0) ? SafeStr(r[cShipTax]).Trim() : "",
                    orderTotalUsd.ToString("N2", ci),
                    (fx > 0m) ? fx.ToString("N4", ci) : "",
                    (fx > 0m) ? orderTotalPln.ToString("N2", ci) : "",
                    (cRecipient >= 0) ? SafeStr(r[cRecipient]).Trim() : "",
                    (cAddr1 >= 0) ? SafeStr(r[cAddr1]).Trim() : "",
                    (cCity >= 0) ? SafeStr(r[cCity]).Trim() : "",
                    (cState >= 0) ? SafeStr(r[cState]).Trim() : "",
                    (cPostal >= 0) ? SafeStr(r[cPostal]).Trim() : "",
                    (cCountry >= 0) ? SafeStr(r[cCountry]).Trim() : "",
                    (cChannel >= 0) ? SafeStr(r[cChannel]).Trim() : ""
                );
            }

            // Total satırı (senin örnekteki gibi sadece total kolonlarını dolduruyoruz)
            var totalRow = dt.NewRow();
            totalRow["Order Total"] = sumUsd.ToString("N2", ci);
            totalRow["Order Total\n(PLN)"] = sumPln.ToString("N2", ci);
            dt.Rows.Add(totalRow);

            return dt;
        }

        // =========================
        // DB: Tracking numbers
        // =========================
        private Dictionary<string, string> LoadTrackingNumbers(List<string> orderIds)
        {
            // orderIds -> TrackingNumber
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (orderIds == null || orderIds.Count == 0) return map;

            // SQL Server param limit vs için chunk
            const int chunkSize = 800;

            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                for (int i = 0; i < orderIds.Count; i += chunkSize)
                {
                    var chunk = orderIds.Skip(i).Take(chunkSize).ToList();

                    var sb = new StringBuilder();
                    sb.Append("SELECT OrderNumber, TrackingNumber FROM dbo.T_ShippingLeg WHERE OrderNumber IN (");
                    for (int p = 0; p < chunk.Count; p++)
                    {
                        if (p > 0) sb.Append(",");
                        sb.Append("@o" + p);
                    }
                    sb.Append(");");

                    using (var cmd = new SqlCommand(sb.ToString(), con))
                    {
                        for (int p = 0; p < chunk.Count; p++)
                            cmd.Parameters.AddWithValue("@o" + p, chunk[p]);

                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                var ord = (r["OrderNumber"] == DBNull.Value) ? "" : r["OrderNumber"].ToString();
                                var trk = (r["TrackingNumber"] == DBNull.Value) ? "" : r["TrackingNumber"].ToString();

                                ord = (ord ?? "").Trim();
                                trk = (trk ?? "").Trim();

                                if (ord.Length == 0) continue;

                                // aynı order için birden çok satır varsa dolu tracking’i tercih et
                                if (!map.ContainsKey(ord) || (map[ord].Length == 0 && trk.Length > 0))
                                    map[ord] = trk;
                            }
                        }
                    }
                }
            }

            return map;
        }

        // =========================
        // DB: FX day-1
        // =========================
        private struct FxRow
        {
            public DateTime RateDate;
            public decimal Rate;
        }

        private FxRow? GetFxUsdPln_DayMinus1(DateTime purchaseDate)
        {
            var d = purchaseDate.AddDays(-1);

            // NBP bazen hafta sonu yok; o yüzden geriye doğru en yakın rate’i alalım (day-1 hedef, yoksa <= day-1)
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT TOP (1) RateDate, UsdPln
FROM dbo.T_FxUsdPln
WHERE RateDate <= @d
ORDER BY RateDate DESC;", con))
            {
                cmd.Parameters.AddWithValue("@d", d.Date);
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        var rd = (r["RateDate"] == DBNull.Value) ? (DateTime?)null : Convert.ToDateTime(r["RateDate"]);
                        var rate = (r["UsdPln"] == DBNull.Value) ? 0m : Convert.ToDecimal(r["UsdPln"], CultureInfo.InvariantCulture);

                        if (rd.HasValue && rate > 0m)
                            return new FxRow { RateDate = rd.Value.Date, Rate = rate };
                    }
                }
            }
            return null;
        }

        // =========================
        // TXT parsing
        // =========================
        private static DataTable ParseDelimited(string content, char delimiter)
        {
            var dt = new DataTable();
            if (string.IsNullOrWhiteSpace(content)) return dt;

            using (var sr = new StringReader(content))
            {
                string headerLine = sr.ReadLine();
                if (headerLine == null) return dt;

                var headers = SplitLine(headerLine, delimiter).Select(x => x.Trim()).ToList();
                for (int i = 0; i < headers.Count; i++)
                {
                    var name = headers[i];
                    if (string.IsNullOrWhiteSpace(name)) name = "col" + i;

                    // duplicate header fix
                    string baseName = name;
                    int k = 1;
                    while (dt.Columns.Contains(name))
                    {
                        name = baseName + "_" + k;
                        k++;
                    }
                    dt.Columns.Add(name, typeof(string));
                }

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Trim().Length == 0) continue;
                    var parts = SplitLine(line, delimiter);
                    var row = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        row[i] = (i < parts.Count) ? parts[i] : "";
                    }
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }

        private static List<string> SplitLine(string line, char delimiter)
        {
            var res = new List<string>();
            if (line == null) return res;

            var sb = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    // double quote escape
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                    continue;
                }

                if (!inQuotes && c == delimiter)
                {
                    res.Add(sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(c);
            }

            res.Add(sb.ToString());
            return res;
        }

        // =========================
        // Helpers
        // =========================
        private static int FindColumn(DataTable dt, IEnumerable<string> names)
        {
            if (dt == null) return -1;
            var set = new HashSet<string>(names.Select(x => (x ?? "").Trim().ToLowerInvariant()));

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var col = (dt.Columns[i].ColumnName ?? "").Trim().ToLowerInvariant();
                if (set.Contains(col)) return i;
            }

            // bazen kolon isimlerinde \r gibi char olur
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var col = (dt.Columns[i].ColumnName ?? "").Trim().ToLowerInvariant().Replace("\r", "").Replace("\n", "");
                if (set.Contains(col)) return i;
            }

            return -1;
        }

        private static string SafeStr(object o)
        {
            return (o == null || o is DBNull) ? "" : o.ToString();
        }

        private static DateTime? TryParseDate(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim();

            // ISO timestamp olabilir
            if (DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto))
                return dto.DateTime.Date;

            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt.Date;

            if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.None, out dt))
                return dt.Date;

            return null;
        }

        private static decimal? TryParseDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Trim();

            // Amazon bazen "5,36" bazen "5.36"
            // önce invariant dene
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v)) return v;

            // virgül -> nokta
            var s2 = s.Replace(",", ".");
            if (decimal.TryParse(s2, NumberStyles.Any, CultureInfo.InvariantCulture, out v)) return v;

            // pl-PL
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.GetCultureInfo("pl-PL"), out v)) return v;

            return null;
        }

        private static string ToDelimited(DataTable dt, char delimiter)
        {
            var sb = new StringBuilder();

            // header
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (i > 0) sb.Append(delimiter);
                sb.Append(EscapeDelimited(dt.Columns[i].ColumnName, delimiter));
            }
            sb.AppendLine();

            foreach (DataRow r in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (i > 0) sb.Append(delimiter);
                    var s = (r[i] == null || r[i] is DBNull) ? "" : r[i].ToString();
                    sb.Append(EscapeDelimited(s, delimiter));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static string EscapeDelimited(string s, char delimiter)
        {
            if (s == null) s = "";
            bool mustQuote = s.Contains(delimiter.ToString()) || s.Contains("\"") || s.Contains("\n") || s.Contains("\r");
            if (!mustQuote) return s;
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        private void DownloadText(string content, string fileName, string contentType)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "utf-8";
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(content);
            Response.Flush();
            Response.End();
        }
    }
}