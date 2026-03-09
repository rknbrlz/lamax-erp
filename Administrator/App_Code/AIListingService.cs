using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feniks.Administrator
{
    public class AIListingResult
    {
        public string Title { get; set; }
        public string Bullet1 { get; set; }
        public string Bullet2 { get; set; }
        public string Bullet3 { get; set; }
        public string Bullet4 { get; set; }
        public string Bullet5 { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Tags { get; set; }
        public int SeoScore { get; set; }
        public string PromptText { get; set; }
        public string RawResponse { get; set; }
        public string ModelName { get; set; }
        public string LanguageCode { get; set; }
    }

    public class ProductPromptData
    {
        public string SKU { get; set; }
        public string ProductTypeID { get; set; }
        public string MaterialID { get; set; }
        public string ColorID { get; set; }
        public string BandTypeID { get; set; }
        public string StoneStatusID { get; set; }
        public string Stone1 { get; set; }
        public string Personalized { get; set; }
        public string Weight { get; set; }
        public string Diameter { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public int ImageCount { get; set; }
        public bool HasPrimaryImage { get; set; }
    }

    public class AIListingService
    {
        private readonly string _connectionString;
        private readonly string _apiKey;
        private readonly string _model;

        public AIListingService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            _apiKey = ConfigurationManager.AppSettings["OpenAI:ApiKey"];
            _model = ConfigurationManager.AppSettings["OpenAI:Model"] ?? "gpt-4.1-mini";
        }

        public async Task<AIListingResult> GenerateAsync(string sku, string marketplace, string languageCode)
        {
            ProductPromptData product = GetProductData(sku);
            string prompt = BuildPrompt(product, marketplace, languageCode);

            string raw = await CallOpenAI(prompt);

            AIListingResult result = ParseResponse(raw);
            result.PromptText = prompt;
            result.RawResponse = raw;
            result.ModelName = _model;
            result.LanguageCode = languageCode;

            return result;
        }

        private ProductPromptData GetProductData(string sku)
        {
            ProductPromptData data = new ProductPromptData();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1
    SKU,
    CONVERT(NVARCHAR(100), ProductTypeID) AS ProductTypeID,
    CONVERT(NVARCHAR(100), MaterialID) AS MaterialID,
    CONVERT(NVARCHAR(100), ColorID) AS ColorID,
    CONVERT(NVARCHAR(100), BandTypeID) AS BandTypeID,
    CONVERT(NVARCHAR(100), StoneStatusID) AS StoneStatusID,
    CONVERT(NVARCHAR(200), Stone1) AS Stone1,
    CONVERT(NVARCHAR(50), Personalized) AS Personalized,
    CONVERT(NVARCHAR(50), Weight) AS Weight,
    CONVERT(NVARCHAR(50), Diameter) AS Diameter,
    CONVERT(NVARCHAR(50), Length) AS Length,
    CONVERT(NVARCHAR(50), Width) AS Width
FROM dbo.T_Product
WHERE SKU = @SKU
ORDER BY ProductID DESC;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        throw new Exception("SKU bulunamadı: " + sku);

                    data.SKU = Convert.ToString(dr["SKU"]);
                    data.ProductTypeID = Convert.ToString(dr["ProductTypeID"]);
                    data.MaterialID = Convert.ToString(dr["MaterialID"]);
                    data.ColorID = Convert.ToString(dr["ColorID"]);
                    data.BandTypeID = Convert.ToString(dr["BandTypeID"]);
                    data.StoneStatusID = Convert.ToString(dr["StoneStatusID"]);
                    data.Stone1 = Convert.ToString(dr["Stone1"]);
                    data.Personalized = Convert.ToString(dr["Personalized"]);
                    data.Weight = Convert.ToString(dr["Weight"]);
                    data.Diameter = Convert.ToString(dr["Diameter"]);
                    data.Length = Convert.ToString(dr["Length"]);
                    data.Width = Convert.ToString(dr["Width"]);
                }
            }

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    COUNT(*) AS ImageCount,
    SUM(CASE WHEN IsPrimary = 1 THEN 1 ELSE 0 END) AS PrimaryCount
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND IsActive = 1;", con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        data.ImageCount = dr["ImageCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ImageCount"]);
                        int primaryCount = dr["PrimaryCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PrimaryCount"]);
                        data.HasPrimaryImage = primaryCount > 0;
                    }
                }
            }

            return data;
        }

        private string BuildPrompt(ProductPromptData p, string marketplace, string languageCode)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("You are an expert e-commerce SEO copywriter.");
            sb.AppendLine("Generate marketplace-optimized product listing content.");
            sb.AppendLine("Write naturally, persuasively, and keyword-rich.");
            sb.AppendLine("Do not use fake claims.");
            sb.AppendLine("Do not mention unsupported materials if not provided.");
            sb.AppendLine("Output JSON only.");
            sb.AppendLine();
            sb.AppendLine("Required JSON schema:");
            sb.AppendLine("{");
            sb.AppendLine("  \"title\": \"...\",");
            sb.AppendLine("  \"bullet1\": \"...\",");
            sb.AppendLine("  \"bullet2\": \"...\",");
            sb.AppendLine("  \"bullet3\": \"...\",");
            sb.AppendLine("  \"bullet4\": \"...\",");
            sb.AppendLine("  \"bullet5\": \"...\",");
            sb.AppendLine("  \"description\": \"...\",");
            sb.AppendLine("  \"keywords\": \"comma separated keywords\",");
            sb.AppendLine("  \"tags\": \"comma separated tags\",");
            sb.AppendLine("  \"seoScore\": 0");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("Marketplace: " + marketplace);
            sb.AppendLine("Language: " + languageCode);
            sb.AppendLine("Brand: Hgerman");
            sb.AppendLine("SKU: " + p.SKU);
            sb.AppendLine("ProductTypeID: " + p.ProductTypeID);
            sb.AppendLine("MaterialID: " + p.MaterialID);
            sb.AppendLine("ColorID: " + p.ColorID);
            sb.AppendLine("BandTypeID: " + p.BandTypeID);
            sb.AppendLine("StoneStatusID: " + p.StoneStatusID);
            sb.AppendLine("Stone1: " + p.Stone1);
            sb.AppendLine("Personalized: " + p.Personalized);
            sb.AppendLine("Weight: " + p.Weight);
            sb.AppendLine("Diameter: " + p.Diameter);
            sb.AppendLine("Length: " + p.Length);
            sb.AppendLine("Width: " + p.Width);
            sb.AppendLine("ImageCount: " + p.ImageCount);
            sb.AppendLine("HasPrimaryImage: " + p.HasPrimaryImage);
            sb.AppendLine();

            if (marketplace == "AMAZON")
            {
                sb.AppendLine("Amazon rules:");
                sb.AppendLine("- Title max 200 chars");
                sb.AppendLine("- 5 concise bullets");
                sb.AppendLine("- Description readable and conversion-focused");
                sb.AppendLine("- Keywords should be backend search style");
                sb.AppendLine("- tags can be empty");
            }
            else if (marketplace == "ETSY")
            {
                sb.AppendLine("Etsy rules:");
                sb.AppendLine("- Title must be SEO rich and natural");
                sb.AppendLine("- Bullets can still be generated for internal use");
                sb.AppendLine("- Tags should be Etsy-friendly comma separated short phrases");
                sb.AppendLine("- Description should feel handmade and attractive");
            }
            else if (marketplace == "EBAY")
            {
                sb.AppendLine("eBay rules:");
                sb.AppendLine("- Title should be keyword rich and readable");
                sb.AppendLine("- Description clear and practical");
                sb.AppendLine("- Keywords should reflect buyer search terms");
            }

            return sb.ToString();
        }

        private async Task<string> CallOpenAI(string prompt)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new Exception("OpenAI API key tanımlı değil.");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _apiKey);

                var body = new
                {
                    model = _model,
                    input = prompt
                };

                string json = JsonConvert.SerializeObject(body);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://api.openai.com/v1/responses", content);
                string responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    throw new Exception("OpenAI hata: " + responseText);

                return responseText;
            }
        }

        private AIListingResult ParseResponse(string raw)
        {
            JObject root = JObject.Parse(raw);

            string outputText = "";

            JToken output = root["output"];
            if (output != null && output.Type == JTokenType.Array)
            {
                foreach (JToken item in output)
                {
                    JToken content = item["content"];
                    if (content != null && content.Type == JTokenType.Array)
                    {
                        foreach (JToken c in content)
                        {
                            if (Convert.ToString(c["type"]) == "output_text")
                            {
                                outputText += Convert.ToString(c["text"]);
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(outputText))
                throw new Exception("AI cevabı parse edilemedi.");

            AIListingResult result;
            try
            {
                result = JsonConvert.DeserializeObject<AIListingResult>(outputText);
            }
            catch
            {
                int first = outputText.IndexOf("{");
                int last = outputText.LastIndexOf("}");
                if (first >= 0 && last > first)
                {
                    string jsonPart = outputText.Substring(first, last - first + 1);
                    result = JsonConvert.DeserializeObject<AIListingResult>(jsonPart);
                }
                else
                {
                    throw new Exception("AI JSON cevabı çözülemedi.");
                }
            }

            if (result == null)
                throw new Exception("AI sonucu boş döndü.");

            return result;
        }

        public void SaveResult(string sku, string marketplace, AIListingResult result, string userName)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductAIContent_Upsert", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace);
                cmd.Parameters.AddWithValue("@Title", (object)result.Title ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bullet1", (object)result.Bullet1 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bullet2", (object)result.Bullet2 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bullet3", (object)result.Bullet3 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bullet4", (object)result.Bullet4 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Bullet5", (object)result.Bullet5 ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)result.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Keywords", (object)result.Keywords ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Tags", (object)result.Tags ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SeoScore", result.SeoScore);
                cmd.Parameters.AddWithValue("@LanguageCode", (object)result.LanguageCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModelName", (object)result.ModelName ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PromptText", (object)result.PromptText ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RawResponse", (object)result.RawResponse ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserName", (object)userName ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public AIListingResult GetSavedResult(string sku, string marketplace)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.sp_ProductAIContent_Get", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    return new AIListingResult
                    {
                        Title = Convert.ToString(dr["Title"]),
                        Bullet1 = Convert.ToString(dr["Bullet1"]),
                        Bullet2 = Convert.ToString(dr["Bullet2"]),
                        Bullet3 = Convert.ToString(dr["Bullet3"]),
                        Bullet4 = Convert.ToString(dr["Bullet4"]),
                        Bullet5 = Convert.ToString(dr["Bullet5"]),
                        Description = Convert.ToString(dr["Description"]),
                        Keywords = Convert.ToString(dr["Keywords"]),
                        Tags = Convert.ToString(dr["Tags"]),
                        SeoScore = dr["SeoScore"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SeoScore"]),
                        LanguageCode = Convert.ToString(dr["LanguageCode"]),
                        ModelName = Convert.ToString(dr["ModelName"]),
                        PromptText = Convert.ToString(dr["PromptText"]),
                        RawResponse = Convert.ToString(dr["RawResponse"])
                    };
                }
            }
        }
    }
}