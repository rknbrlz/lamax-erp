using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Feniks.Services
{
    public class AmazonSpApiClient
    {
        private readonly string _lwaClientId;
        private readonly string _lwaClientSecret;
        private readonly string _refreshToken;
        private readonly string _marketplaceId;
        private readonly string _endpointHost;

        public AmazonSpApiClient()
        {
            _lwaClientId = GetRequiredAppSetting("AmazonLwaClientId");
            _lwaClientSecret = GetRequiredAppSetting("AmazonLwaClientSecret");
            _refreshToken = GetRequiredAppSetting("AmazonRefreshToken");
            _marketplaceId = GetRequiredAppSetting("AmazonMarketplaceId");
            _endpointHost = GetRequiredAppSetting("AmazonSpApiHost");
        }

        public string MarketplaceId
        {
            get { return _marketplaceId; }
        }

        public JObject GetOrders(DateTime lastUpdatedAfterUtc, string nextToken = null)
        {
            string path = "/orders/v0/orders";
            string query;

            if (!string.IsNullOrWhiteSpace(nextToken))
            {
                query = "NextToken=" + UrlEncode(nextToken);
            }
            else
            {
                query =
                    "MarketplaceIds=" + UrlEncode(_marketplaceId) +
                    "&LastUpdatedAfter=" + UrlEncode(lastUpdatedAfterUtc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)) +
                    "&MaxResultsPerPage=100";
            }

            return SendSignedGet(path, query);
        }

        public JObject GetOrderItems(string amazonOrderId, string nextToken = null)
        {
            if (string.IsNullOrWhiteSpace(amazonOrderId))
                throw new ArgumentException("amazonOrderId is required.", "amazonOrderId");

            string path = "/orders/v0/orders/" + Uri.EscapeDataString(amazonOrderId) + "/orderItems";
            string query = string.IsNullOrWhiteSpace(nextToken)
                ? ""
                : "NextToken=" + UrlEncode(nextToken);

            return SendSignedGet(path, query);
        }

        public string GetAccessToken()
        {
            string url = "https://api.amazon.com/auth/o2/token";

            string postData =
                "grant_type=refresh_token" +
                "&refresh_token=" + UrlEncode(_refreshToken) +
                "&client_id=" + UrlEncode(_lwaClientId) +
                "&client_secret=" + UrlEncode(_lwaClientSecret);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Accept = "application/json";
            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;

            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = bytes.Length;

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string json = sr.ReadToEnd();
                    JObject obj = JObject.Parse(json);

                    string accessToken = obj["access_token"] != null
                        ? obj["access_token"].ToString()
                        : "";

                    if (string.IsNullOrWhiteSpace(accessToken))
                        throw new Exception("Amazon token response does not contain access_token. Response=" + json);

                    return accessToken;
                }
            }
            catch (WebException ex)
            {
                string errorText = ReadWebException(ex);
                throw new Exception("Amazon token request failed. Response=" + errorText, ex);
            }
        }

        private JObject SendSignedGet(string path, string queryString)
        {
            string accessToken = GetAccessToken();

            string host = NormalizeHost(_endpointHost);
            string url = "https://" + host + path;

            if (!string.IsNullOrWhiteSpace(queryString))
                url += "?" + queryString;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Timeout = 120000;
            request.ReadWriteTimeout = 120000;

            request.Headers["x-amz-access-token"] = accessToken;

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string json = sr.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(json))
                        return new JObject();

                    return JObject.Parse(json);
                }
            }
            catch (WebException ex)
            {
                string errorText = ReadWebException(ex);
                throw new Exception(
                    "Amazon GET failed. Url=" + url + " Response=" + errorText,
                    ex
                );
            }
        }

        private static string ReadWebException(WebException ex)
        {
            if (ex == null)
                return "";

            if (ex.Response == null)
                return ex.Message ?? "";

            try
            {
                using (var response = ex.Response)
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch
            {
                return ex.Message ?? "";
            }
        }

        private static string NormalizeHost(string host)
        {
            host = (host ?? "").Trim();

            if (host.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                host = host.Substring("https://".Length);

            if (host.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                host = host.Substring("http://".Length);

            host = host.TrimEnd('/');

            return host;
        }

        private static string GetRequiredAppSetting(string key)
        {
            string value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value))
                throw new ConfigurationErrorsException("Missing appSetting: " + key);

            return value.Trim();
        }

        private static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value ?? "")
                .Replace("+", "%20")
                .Replace("*", "%2A")
                .Replace("%7e", "~");
        }
    }
}