using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Feniks.Services
{
    public class EtsyApiClient
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private readonly string _apiKey = ConfigurationManager.AppSettings["EtsyApiKey"];
        private readonly string _sharedSecret = ConfigurationManager.AppSettings["EtsySharedSecret"];
        private readonly string _shopName = ConfigurationManager.AppSettings["EtsyShopName"];

        private long? _cachedShopId;

        public string ShopName
        {
            get { return (_shopName ?? "").Trim(); }
        }

        public long ShopId
        {
            get
            {
                if (_cachedShopId.HasValue)
                    return _cachedShopId.Value;

                string accessToken = GetValidAccessToken();
                _cachedShopId = ResolveShopIdWithAccessToken(accessToken);
                return _cachedShopId.Value;
            }
        }

        public string BuildAuthorizeUrl(string state, string codeChallenge)
        {
            string redirectUri = ConfigurationManager.AppSettings["EtsyRedirectUri"];
            string scope = "transactions_r shops_r";

            return "https://www.etsy.com/oauth/connect" +
                   "?response_type=code" +
                   "&redirect_uri=" + HttpUtility.UrlEncode(redirectUri) +
                   "&scope=" + HttpUtility.UrlEncode(scope) +
                   "&client_id=" + HttpUtility.UrlEncode(_apiKey) +
                   "&state=" + HttpUtility.UrlEncode(state) +
                   "&code_challenge=" + HttpUtility.UrlEncode(codeChallenge) +
                   "&code_challenge_method=S256";
        }

        public static string GenerateCodeVerifier()
        {
            byte[] bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);

            return Base64UrlEncode(bytes);
        }

        public static string GenerateCodeChallenge(string verifier)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(verifier));
                return Base64UrlEncode(hash);
            }
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public JObject ExchangeCodeForToken(string code, string codeVerifier)
        {
            string redirectUri = ConfigurationManager.AppSettings["EtsyRedirectUri"];
            string url = "https://api.etsy.com/v3/public/oauth/token";

            JObject body = new JObject
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = _apiKey,
                ["redirect_uri"] = redirectUri,
                ["code"] = code,
                ["code_verifier"] = codeVerifier
            };

            string response = SendJson(url, "POST", body.ToString());
            return JObject.Parse(response);
        }

        public JObject RefreshToken(string refreshToken)
        {
            string url = "https://api.etsy.com/v3/public/oauth/token";

            JObject body = new JObject
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _apiKey,
                ["refresh_token"] = refreshToken
            };

            string response = SendJson(url, "POST", body.ToString());
            return JObject.Parse(response);
        }

        public void SaveToken(JObject tokenJson)
        {
            string accessToken = (string)tokenJson["access_token"];
            string refreshToken = (string)tokenJson["refresh_token"];
            string tokenType = (string)tokenJson["token_type"];
            int expiresIn = tokenJson["expires_in"] != null ? (int)tokenJson["expires_in"] : 3600;
            string scope = tokenJson["scope"] != null ? tokenJson["scope"].ToString() : "";

            if (string.IsNullOrWhiteSpace(accessToken))
                throw new Exception("Etsy access token boş döndü.");

            long shopId = ResolveShopIdWithAccessToken(accessToken);
            DateTime expiresAtUtc = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_EtsyUpsertToken", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ShopId", shopId);
                cmd.Parameters.AddWithValue("@AccessToken", (object)accessToken ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RefreshToken", (object)refreshToken ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TokenType", (object)tokenType ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ExpiresAtUtc", expiresAtUtc);
                cmd.Parameters.AddWithValue("@Scope", (object)scope ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            _cachedShopId = shopId;
        }

        public string GetValidAccessToken()
        {
            string accessToken = null;
            string refreshToken = null;
            DateTime? expiresAtUtc = null;

            // Bootstrap için ShopId kullanmadan en son tokenı al
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 AccessToken, RefreshToken, ExpiresAtUtc
                FROM dbo.T_EtsyAuthToken
                ORDER BY UpdatedAtUtc DESC, EtsyAuthTokenID DESC", con))
            {
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        accessToken = dr["AccessToken"] as string;
                        refreshToken = dr["RefreshToken"] as string;

                        if (dr["ExpiresAtUtc"] != DBNull.Value)
                            expiresAtUtc = Convert.ToDateTime(dr["ExpiresAtUtc"]);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(accessToken) &&
                expiresAtUtc.HasValue &&
                expiresAtUtc.Value > DateTime.UtcNow)
            {
                return accessToken;
            }

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new Exception("Etsy token bulunamadı. Önce Connect Etsy yap.");

            JObject refreshed = RefreshToken(refreshToken);

            // refresh sonrası DB'ye tekrar düzgün kaydet
            SaveToken(refreshed);

            string newAccessToken = (string)refreshed["access_token"];
            if (string.IsNullOrWhiteSpace(newAccessToken))
                throw new Exception("Etsy access token yenilenemedi.");

            return newAccessToken;
        }

        public long ResolveShopIdWithAccessToken(string accessToken)
        {
            string shopName = ShopName;

            if (string.IsNullOrWhiteSpace(shopName))
                throw new Exception("web.config içinde EtsyShopName tanımlı değil.");

            string url = "https://api.etsy.com/v3/application/shops/" + HttpUtility.UrlEncode(shopName);
            string json = SendRequest(url, "GET", null, true, accessToken);

            JObject obj = JObject.Parse(json);

            JToken token = obj["shop_id"];
            if (token == null || token.Type == JTokenType.Null)
                throw new Exception("Etsy shop id çözümlenemedi. Shop name kontrol et: " + shopName);

            long shopId;
            if (!long.TryParse(token.ToString(), out shopId))
                throw new Exception("Etsy shop id sayısal formatta değil.");

            return shopId;
        }

        public JObject GetReceipts(int limit, int offset, bool onlyOpen)
        {
            string url = "https://api.etsy.com/v3/application/shops/" + ShopId + "/receipts" +
                         "?was_paid=true" +
                         (onlyOpen ? "&was_shipped=false" : "") +
                         "&limit=" + limit +
                         "&offset=" + offset +
                         "&sort_on=created&sort_order=desc";

            string json = SendAuthorized(url, "GET");
            return JObject.Parse(json);
        }

        public JObject GetReceiptTransactions(long receiptId)
        {
            string url = "https://api.etsy.com/v3/application/shops/" + ShopId + "/receipts/" + receiptId + "/transactions";
            string json = SendAuthorized(url, "GET");
            return JObject.Parse(json);
        }

        private string SendAuthorized(string url, string method)
        {
            string accessToken = GetValidAccessToken();
            return SendRequest(url, method, null, true, accessToken);
        }

        private string SendJson(string url, string method, string jsonBody)
        {
            return SendRequest(url, method, jsonBody, false, null);
        }

        private string SendRequest(string url, string method, string body, bool useAuth, string accessToken)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method;
            req.ContentType = "application/json";
            req.Accept = "application/json";
            req.Headers["x-api-key"] = _apiKey;

            if (useAuth)
                req.Headers["Authorization"] = "Bearer " + accessToken;

            if (!string.IsNullOrWhiteSpace(body))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                using (Stream stream = req.GetRequestStream())
                    stream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    return sr.ReadToEnd();
            }
            catch (WebException ex)
            {
                string raw = "";
                if (ex.Response != null)
                {
                    using (var sr = new StreamReader(ex.Response.GetResponseStream()))
                        raw = sr.ReadToEnd();
                }

                throw new Exception("Etsy API error: " + raw, ex);
            }
        }
    }
}