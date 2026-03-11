using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class ProductAI : Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["constr"] != null
                ? ConfigurationManager.ConnectionStrings["constr"].ConnectionString
                : "";

        private readonly JavaScriptSerializer _json = new JavaScriptSerializer
        {
            MaxJsonLength = int.MaxValue,
            RecursionLimit = 100
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litSeoScore.Text = "-";
                litVisionSummary.Text = "Henüz analiz yapılmadı.";
                litStrategySummary.Text = "Henüz strategy üretilmedi.";
                litMultiMarketplace.Text = "Henüz çoklu marketplace üretimi yapılmadı.";
                imgProductPreview.Style["display"] = "none";
                litNoImage.Text = "Görsel henüz yüklenmedi.";
                litPreviewMeta.Text = "";
                litSeoLevel.Text = "No score yet";
                litSeoHint.Text = "AI çıktısına göre hesaplanır.";
                ViewState["AI_ALL_RESULTS_JSON"] = "";

                SetRadarVisual(0, 0, 0, 0);
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateInternal(false);
        }

        protected void btnForceRefresh_Click(object sender, EventArgs e)
        {
            GenerateInternal(true);
        }

        protected void btnGenerateAll_Click(object sender, EventArgs e)
        {
            GenerateAllInternal(false);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ClearStatus();

            try
            {
                string sku = (txtSku.Text ?? "").Trim();
                string marketplace = (ddlMarketplace.SelectedValue ?? "").Trim();
                string languageCode = GetLanguageCode(ddlLanguage.SelectedValue);

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Kaydetmek için SKU gerekli.");
                    return;
                }

                ListingResult listing = ReadForm();
                string visionSummaryText = StripHtml(litVisionSummary.Text);
                string strategySummaryText = StripHtml(litStrategySummary.Text);

                SaveListing(sku, marketplace, languageCode, listing, visionSummaryText, strategySummaryText);
                ShowSuccess("AI içerik kaydedildi.");
            }
            catch (Exception ex)
            {
                ShowError("Save hatası: " + Server.HtmlEncode(ex.Message));
            }
        }

        protected void btnSaveAllMarketplaces_Click(object sender, EventArgs e)
        {
            ClearStatus();

            try
            {
                string sku = (txtSku.Text ?? "").Trim();
                string languageCode = GetLanguageCode(ddlLanguage.SelectedValue);

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Tüm marketplace kayıtları için SKU gerekli.");
                    return;
                }

                List<MultiMarketplaceResult> allResults = GetAllResultsFromViewState();
                if (allResults == null || allResults.Count == 0)
                {
                    ShowError("Önce Generate All Marketplaces çalıştırılmalı.");
                    return;
                }

                string visionSummaryText = StripHtml(litVisionSummary.Text);
                int savedCount = 0;

                foreach (var item in allResults)
                {
                    if (item == null || item.Listing == null)
                        continue;

                    string strategySummaryText = item.Strategy != null
                        ? BuildStrategyPlainText(item.Strategy)
                        : "";

                    SaveListing(
                        sku,
                        item.Marketplace ?? "",
                        languageCode,
                        item.Listing,
                        visionSummaryText,
                        strategySummaryText
                    );

                    savedCount++;
                }

                ShowSuccess(savedCount.ToString() + " marketplace sonucu veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                ShowError("Save All DB hatası: " + Server.HtmlEncode(ex.Message));
            }
        }

        protected void btnLoadSaved_Click(object sender, EventArgs e)
        {
            ClearStatus();

            try
            {
                string sku = (txtSku.Text ?? "").Trim();
                string marketplace = (ddlMarketplace.SelectedValue ?? "").Trim();
                string languageCode = GetLanguageCode(ddlLanguage.SelectedValue);

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("Load için SKU gerekli.");
                    return;
                }

                SavedAiContent saved = LoadSavedListing(sku, marketplace, languageCode);
                if (saved == null)
                {
                    ShowError("Kayıtlı içerik bulunamadı.");
                    return;
                }

                FillForm(new ListingResult
                {
                    title = saved.Title,
                    bullet1 = saved.Bullet1,
                    bullet2 = saved.Bullet2,
                    bullet3 = saved.Bullet3,
                    bullet4 = saved.Bullet4,
                    bullet5 = saved.Bullet5,
                    description = saved.Description,
                    keywords = saved.Keywords,
                    tags = saved.Tags,
                    SeoScore = saved.SeoScore
                });

                SetSeoVisual(saved.SeoScore);

                SeoBreakdown radar = CalculateSeoBreakdown(ReadForm(), marketplace, null);
                SetRadarVisual(radar.TitleScore, radar.BulletsScore, radar.KeywordsScore, radar.TagsScore);

                litVisionSummary.Text = !string.IsNullOrWhiteSpace(saved.VisionSummary)
                    ? Server.HtmlEncode(saved.VisionSummary).Replace("\n", "<br/>")
                    : "Kayıtlı vision özeti yok.";

                litStrategySummary.Text = !string.IsNullOrWhiteSpace(saved.StrategySummary)
                    ? Server.HtmlEncode(saved.StrategySummary).Replace("\n", "<br/>")
                    : "Kayıtlı strategy özeti yok.";

                ShowSuccess("Kayıtlı içerik yüklendi.");
            }
            catch (Exception ex)
            {
                ShowError("Load hatası: " + Server.HtmlEncode(ex.Message));
            }
        }

        private void GenerateInternal(bool forceRefresh)
        {
            ClearStatus();

            try
            {
                string sku = (txtSku.Text ?? "").Trim();
                string marketplace = (ddlMarketplace.SelectedValue ?? "").Trim();
                string languageCode = GetLanguageCode(ddlLanguage.SelectedValue);
                string languageName = GetLanguageName(languageCode);

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("SKU boş olamaz.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_connStr))
                {
                    ShowError("Connection string bulunamadı. web.config kontrol et.");
                    return;
                }

                string apiKey = ConfigurationManager.AppSettings["OpenAI:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    ShowError("OpenAI API key tanımlı değil.");
                    return;
                }

                List<ProductImageInfo> images = GetBestProductImages(sku, marketplace, 4);
                LoadProductPreview(images, sku, marketplace);

                if (images == null || images.Count == 0)
                {
                    ShowError("Bu SKU için aktif görsel bulunamadı.");
                    return;
                }

                ProductMeta meta = GetOptionalProductMeta(sku);
                string imageSignature = BuildImageSignature(images);

                VisionResult vision = null;
                bool loadedFromCache = false;

                if (!forceRefresh)
                {
                    vision = LoadVisionCache(sku, imageSignature);
                    loadedFromCache = vision != null;
                }

                if (vision == null)
                {
                    vision = AnalyzeImagesWithAi(apiKey, sku, marketplace, languageName, images, meta);
                    if (vision == null)
                    {
                        ShowError("AI görsel analizi başarısız oldu.");
                        return;
                    }

                    SaveVisionCache(sku, marketplace, languageCode, imageSignature, images.Count, vision);
                }

                KeywordStrategyResult strategy = BuildKeywordStrategyWithAi(apiKey, sku, marketplace, languageName, meta, vision);
                if (strategy == null)
                {
                    ShowError("AI keyword strategy üretimi başarısız oldu.");
                    return;
                }

                ListingResult listing = GenerateListingWithAi(apiKey, sku, marketplace, languageName, meta, vision, strategy);
                if (listing == null)
                {
                    ShowError("AI listing üretimi başarısız oldu.");
                    return;
                }

                NormalizeListingByMarketplace(listing, marketplace, strategy);
                listing.SeoScore = CalculateSeoScore(listing, marketplace, strategy);

                FillForm(listing);
                litVisionSummary.Text = BuildVisionSummaryHtml(vision, images.Count, loadedFromCache, forceRefresh);
                litStrategySummary.Text = BuildStrategySummaryHtml(strategy);
                SetSeoVisual(listing.SeoScore);

                SeoBreakdown radar = CalculateSeoBreakdown(listing, marketplace, strategy);
                SetRadarVisual(radar.TitleScore, radar.BulletsScore, radar.KeywordsScore, radar.TagsScore);

                if (forceRefresh)
                    ShowSuccess("AI içerik üretildi. Görsel analizi zorla yenilendi ve cache güncellendi.");
                else if (loadedFromCache)
                    ShowSuccess("AI içerik üretildi. Görsel analizi cache'den yüklendi.");
                else
                    ShowSuccess("AI içerik üretildi. Görsel analizi yeni oluşturulup cache'e kaydedildi.");
            }
            catch (Exception ex)
            {
                ShowError("Generate hatası: " + Server.HtmlEncode(ex.Message));
            }
        }

        private void GenerateAllInternal(bool forceRefresh)
        {
            ClearStatus();

            try
            {
                string sku = (txtSku.Text ?? "").Trim();
                string languageCode = GetLanguageCode(ddlLanguage.SelectedValue);
                string languageName = GetLanguageName(languageCode);

                if (string.IsNullOrWhiteSpace(sku))
                {
                    ShowError("SKU boş olamaz.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(_connStr))
                {
                    ShowError("Connection string bulunamadı. web.config kontrol et.");
                    return;
                }

                string apiKey = ConfigurationManager.AppSettings["OpenAI:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    ShowError("OpenAI API key tanımlı değil.");
                    return;
                }

                string previewMarketplace = (ddlMarketplace.SelectedValue ?? "").Trim();
                List<ProductImageInfo> images = GetBestProductImages(sku, previewMarketplace, 4);
                LoadProductPreview(images, sku, previewMarketplace);

                if (images == null || images.Count == 0)
                {
                    ShowError("Bu SKU için aktif görsel bulunamadı.");
                    return;
                }

                ProductMeta meta = GetOptionalProductMeta(sku);
                string imageSignature = BuildImageSignature(images);

                VisionResult vision = null;
                bool loadedFromCache = false;

                if (!forceRefresh)
                {
                    vision = LoadVisionCache(sku, imageSignature);
                    loadedFromCache = vision != null;
                }

                if (vision == null)
                {
                    vision = AnalyzeImagesWithAi(apiKey, sku, "Website", languageName, images, meta);
                    if (vision == null)
                    {
                        ShowError("AI görsel analizi başarısız oldu.");
                        return;
                    }

                    SaveVisionCache(sku, "All", languageCode, imageSignature, images.Count, vision);
                }

                litVisionSummary.Text = BuildVisionSummaryHtml(vision, images.Count, loadedFromCache, forceRefresh);

                string[] marketplaces = { "Amazon", "Etsy", "eBay", "Website" };
                List<MultiMarketplaceResult> allResults = new List<MultiMarketplaceResult>();

                foreach (string mp in marketplaces)
                {
                    KeywordStrategyResult strategy = BuildKeywordStrategyWithAi(apiKey, sku, mp, languageName, meta, vision);
                    if (strategy == null)
                        throw new Exception(mp + " için keyword strategy üretilemedi.");

                    ListingResult listing = GenerateListingWithAi(apiKey, sku, mp, languageName, meta, vision, strategy);
                    if (listing == null)
                        throw new Exception(mp + " için listing üretilemedi.");

                    NormalizeListingByMarketplace(listing, mp, strategy);
                    listing.SeoScore = CalculateSeoScore(listing, mp, strategy);

                    allResults.Add(new MultiMarketplaceResult
                    {
                        Marketplace = mp,
                        Strategy = strategy,
                        Listing = listing
                    });
                }

                litMultiMarketplace.Text = BuildMultiMarketplaceHtml(allResults);
                StoreAllResultsInViewState(allResults);

                MultiMarketplaceResult selected = allResults
                    .FirstOrDefault(x => x.Marketplace.Equals(previewMarketplace, StringComparison.OrdinalIgnoreCase))
                    ?? allResults.FirstOrDefault();

                if (selected != null)
                {
                    FillForm(selected.Listing);
                    SetSeoVisual(selected.Listing.SeoScore);
                    litStrategySummary.Text = BuildStrategySummaryHtml(selected.Strategy);

                    SeoBreakdown radar = CalculateSeoBreakdown(selected.Listing, selected.Marketplace, selected.Strategy);
                    SetRadarVisual(radar.TitleScore, radar.BulletsScore, radar.KeywordsScore, radar.TagsScore);
                }

                ShowSuccess("Tüm marketplace içerikleri başarıyla üretildi.");
            }
            catch (Exception ex)
            {
                ShowError("Generate All hatası: " + Server.HtmlEncode(ex.Message));
            }
        }

        private List<ProductImageInfo> GetBestProductImages(string sku, string marketplace, int maxCount)
        {
            const string sql = @"
SELECT TOP (@TopN)
       ProductImageID,
       SKU,
       ImageData,
       ISNULL(ContentType, 'image/jpeg') AS ContentType,
       ISNULL(FileName, '') AS FileName,
       ISNULL(Marketplace, '') AS Marketplace,
       ISNULL(ImageRole, '') AS ImageRole,
       ISNULL(IsPrimary, 0) AS IsPrimary,
       ISNULL(IsActive, 0) AS IsActive,
       ISNULL(SortOrder, 9999) AS SortOrder
FROM dbo.T_ProductImage
WHERE SKU = @SKU
  AND ISNULL(IsActive, 1) = 1
  AND ImageData IS NOT NULL
ORDER BY
    CASE WHEN ISNULL(IsPrimary, 0) = 1 THEN 0 ELSE 1 END,
    CASE
        WHEN UPPER(ISNULL(ImageRole, '')) = 'MAIN' THEN 0
        WHEN UPPER(ISNULL(ImageRole, '')) = 'GALLERY' THEN 1
        ELSE 2
    END,
    CASE
        WHEN ISNULL(Marketplace, '') = @Marketplace THEN 0
        WHEN ISNULL(Marketplace, '') = '' THEN 1
        ELSE 2
    END,
    ISNULL(SortOrder, 9999),
    ProductImageID;";

            List<ProductImageInfo> list = new List<ProductImageInfo>();

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@TopN", maxCount);
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace ?? "");
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new ProductImageInfo
                        {
                            ProductImageID = SafeInt(dr["ProductImageID"]),
                            SKU = SafeString(dr["SKU"]),
                            ImageData = dr["ImageData"] as byte[],
                            ContentType = SafeString(dr["ContentType"]),
                            FileName = SafeString(dr["FileName"]),
                            Marketplace = SafeString(dr["Marketplace"]),
                            ImageRole = SafeString(dr["ImageRole"])
                        });
                    }
                }
            }

            return list;
        }

        private ProductMeta GetOptionalProductMeta(string sku)
        {
            ProductMeta meta = new ProductMeta
            {
                SKU = sku,
                Brand = "Hgerman"
            };

            using (SqlConnection con = new SqlConnection(_connStr))
            {
                con.Open();

                List<string> columns = new List<string>();
                using (SqlCommand colCmd = new SqlCommand(@"
SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'T_Product';", con))
                using (SqlDataReader dr = colCmd.ExecuteReader())
                {
                    while (dr.Read())
                        columns.Add(SafeString(dr["COLUMN_NAME"]));
                }

                if (columns.Count == 0)
                    return meta;

                string productNameCol = FirstExisting(columns, "ProductName", "Name", "Title");
                string materialCol = FirstExisting(columns, "Material", "MaterialName");
                string stoneCol = FirstExisting(columns, "Stone", "StoneType");
                string colorCol = FirstExisting(columns, "Color", "ColorName");
                string categoryCol = FirstExisting(columns, "Category", "CategoryName", "ProductType");
                string genderCol = FirstExisting(columns, "Gender", "TargetGender");
                string brandCol = FirstExisting(columns, "Brand", "BrandName");

                List<string> selectParts = new List<string>();
                if (!string.IsNullOrEmpty(productNameCol)) selectParts.Add("CAST([" + productNameCol + "] AS NVARCHAR(500)) AS ProductName");
                if (!string.IsNullOrEmpty(materialCol)) selectParts.Add("CAST([" + materialCol + "] AS NVARCHAR(500)) AS Material");
                if (!string.IsNullOrEmpty(stoneCol)) selectParts.Add("CAST([" + stoneCol + "] AS NVARCHAR(500)) AS Stone");
                if (!string.IsNullOrEmpty(colorCol)) selectParts.Add("CAST([" + colorCol + "] AS NVARCHAR(500)) AS Color");
                if (!string.IsNullOrEmpty(categoryCol)) selectParts.Add("CAST([" + categoryCol + "] AS NVARCHAR(500)) AS Category");
                if (!string.IsNullOrEmpty(genderCol)) selectParts.Add("CAST([" + genderCol + "] AS NVARCHAR(500)) AS Gender");
                if (!string.IsNullOrEmpty(brandCol)) selectParts.Add("CAST([" + brandCol + "] AS NVARCHAR(500)) AS Brand");

                if (selectParts.Count == 0)
                    return meta;

                string sql = "SELECT TOP 1 " + string.Join(", ", selectParts) + " FROM dbo.T_Product WHERE SKU = @SKU";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@SKU", sku);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                            return meta;

                        if (HasColumn(dr, "ProductName")) meta.ProductName = SafeString(dr["ProductName"]);
                        if (HasColumn(dr, "Material")) meta.Material = SafeString(dr["Material"]);
                        if (HasColumn(dr, "Stone")) meta.Stone = SafeString(dr["Stone"]);
                        if (HasColumn(dr, "Color")) meta.Color = SafeString(dr["Color"]);
                        if (HasColumn(dr, "Category")) meta.Category = SafeString(dr["Category"]);
                        if (HasColumn(dr, "Gender")) meta.Gender = SafeString(dr["Gender"]);
                        if (HasColumn(dr, "Brand") && !string.IsNullOrWhiteSpace(SafeString(dr["Brand"])))
                            meta.Brand = SafeString(dr["Brand"]);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(meta.Brand))
                meta.Brand = "Hgerman";

            return meta;
        }

        private VisionResult AnalyzeImagesWithAi(string apiKey, string sku, string marketplace, string language, List<ProductImageInfo> images, ProductMeta meta)
        {
            string prompt = BuildVisionPrompt(sku, marketplace, language, meta, images.Count);

            List<object> contentItems = new List<object>();
            contentItems.Add(new { type = "input_text", text = prompt });

            foreach (ProductImageInfo img in images)
            {
                if (img.ImageData == null || img.ImageData.Length == 0)
                    continue;

                string base64 = Convert.ToBase64String(img.ImageData);
                string dataUrl = "data:" + img.ContentType + ";base64," + base64;

                contentItems.Add(new
                {
                    type = "input_image",
                    image_url = dataUrl,
                    detail = "high"
                });
            }

            var payload = new
            {
                model = "gpt-4.1-mini",
                input = new object[]
                {
                    new
                    {
                        role = "user",
                        content = contentItems.ToArray()
                    }
                }
            };

            string raw = CallOpenAiResponses(apiKey, payload);
            string cleaned = CleanJson(raw);

            VisionResult result = _json.Deserialize<VisionResult>(cleaned);
            if (result == null)
                throw new Exception("Vision JSON parse edilemedi.");

            NormalizeVisionResult(result);
            return result;
        }

        private KeywordStrategyResult BuildKeywordStrategyWithAi(string apiKey, string sku, string marketplace, string language, ProductMeta meta, VisionResult vision)
        {
            string prompt = BuildKeywordStrategyPrompt(sku, marketplace, language, meta, vision);

            var payload = new
            {
                model = "gpt-4.1-mini",
                input = new object[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "input_text", text = prompt }
                        }
                    }
                }
            };

            string raw = CallOpenAiResponses(apiKey, payload);
            string cleaned = CleanJson(raw);

            KeywordStrategyResult result = _json.Deserialize<KeywordStrategyResult>(cleaned);
            if (result == null)
                throw new Exception("Keyword strategy JSON parse edilemedi.");

            NormalizeKeywordStrategy(result);
            return result;
        }

        private ListingResult GenerateListingWithAi(string apiKey, string sku, string marketplace, string language, ProductMeta meta, VisionResult vision, KeywordStrategyResult strategy)
        {
            string prompt = BuildListingPrompt(sku, marketplace, language, meta, vision, strategy);

            var payload = new
            {
                model = "gpt-4.1-mini",
                input = new object[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new { type = "input_text", text = prompt }
                        }
                    }
                }
            };

            string raw = CallOpenAiResponses(apiKey, payload);
            string cleaned = CleanJson(raw);

            ListingResult result = _json.Deserialize<ListingResult>(cleaned);
            if (result == null)
                throw new Exception("Listing JSON parse edilemedi.");

            NormalizeListingResult(result);
            return result;
        }

        private string CallOpenAiResponses(string apiKey, object payload)
        {
            string requestJson = _json.Serialize(payload);

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(3);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("https://api.openai.com/v1/responses", content).Result;
                string responseText = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("OpenAI API hatası: " + response.StatusCode + " - " + responseText);

                OpenAiResponseEnvelope env = null;
                try
                {
                    env = _json.Deserialize<OpenAiResponseEnvelope>(responseText);
                }
                catch
                {
                }

                if (env != null && !string.IsNullOrWhiteSpace(env.output_text))
                    return env.output_text;

                object rootObj = _json.DeserializeObject(responseText);
                Dictionary<string, object> root = rootObj as Dictionary<string, object>;
                if (root == null)
                    throw new Exception("OpenAI response parse edilemedi. Raw: " + responseText);

                if (root.ContainsKey("output_text") && root["output_text"] != null)
                {
                    string directText = root["output_text"].ToString();
                    if (!string.IsNullOrWhiteSpace(directText))
                        return directText;
                }

                if (root.ContainsKey("output") && root["output"] is object[])
                {
                    object[] outputArr = (object[])root["output"];

                    foreach (object itemObj in outputArr)
                    {
                        Dictionary<string, object> item = itemObj as Dictionary<string, object>;
                        if (item == null) continue;

                        if (item.ContainsKey("content") && item["content"] is object[])
                        {
                            object[] contentArr = (object[])item["content"];

                            foreach (object contentObj in contentArr)
                            {
                                Dictionary<string, object> contentItem = contentObj as Dictionary<string, object>;
                                if (contentItem == null) continue;

                                if (contentItem.ContainsKey("type") &&
                                    contentItem["type"] != null &&
                                    contentItem["type"].ToString() == "output_text")
                                {
                                    if (contentItem.ContainsKey("text") && contentItem["text"] != null)
                                    {
                                        string txt = contentItem["text"].ToString();
                                        if (!string.IsNullOrWhiteSpace(txt))
                                            return txt;
                                    }
                                }

                                if (contentItem.ContainsKey("text") && contentItem["text"] != null)
                                {
                                    string txt = contentItem["text"].ToString();
                                    if (!string.IsNullOrWhiteSpace(txt))
                                        return txt;
                                }
                            }
                        }
                    }
                }

                throw new Exception("OpenAI çıktısı bulundu ama text alanı çıkarılamadı. Raw: " + responseText);
            }
        }

        private string BuildVisionPrompt(string sku, string marketplace, string language, ProductMeta meta, int imageCount)
        {
            string langInstruction = GetLanguageInstruction(language);

            return $@"
You are an e-commerce product vision analyst.

You will receive {imageCount} image(s) of the same SKU.
Analyze all images together as one product.
Return ONLY valid JSON.
Do not add markdown.
Do not wrap in code block.
Do not invent features that are not visible.
Do not infer purity, certification, dimensions, gemstone authenticity, metal type certainty, or handmade process unless clearly visible or explicitly provided in metadata.
If uncertain, leave the field empty or return an empty array.
Use confidence as decimal between 0 and 1.

Focus on:
- what is clearly visible
- likely product type
- likely style language
- visible material appearance
- visible colors
- likely use cases
- whether it naturally feels giftable

{langInstruction}

Context:
- SKU: {sku}
- Brand: {meta.Brand}
- Marketplace: {marketplace}
- ProductName from DB: {meta.ProductName}
- Category from DB: {meta.Category}
- Material from DB: {meta.Material}
- Stone from DB: {meta.Stone}
- Color from DB: {meta.Color}
- Gender from DB: {meta.Gender}

Return exactly this JSON shape:
{{
  ""product_type"": """",
  ""target_audience"": """",
  ""style_keywords"": ["""", """", """", """"],
  ""visible_materials"": ["""", """", """"],
  ""visible_colors"": ["""", """", """"],
  ""use_cases"": ["""", """", """", """"],
  ""giftable"": """",
  ""confidence"": 0.0,
  ""summary"": """"
}}";
        }

        private string BuildKeywordStrategyPrompt(string sku, string marketplace, string language, ProductMeta meta, VisionResult vision)
        {
            string langInstruction = GetLanguageInstruction(language);
            string marketRules = GetMarketplaceRules(marketplace);
            string visionJson = _json.Serialize(vision);

            List<string> seeds = GetMarketplaceSeedKeywords(marketplace, vision, meta);
            string seedList = string.Join(", ", seeds);

            return $@"
You are a senior marketplace SEO strategist for handmade, jewelry, fashion accessory and gift listings.

Your task is NOT to write the final listing yet.
Your task is to create the keyword and positioning strategy that will later be used to generate the listing.

Important rules:
- Use only supported facts from database metadata and image analysis.
- Do not invent material purity, gemstone authenticity, certifications, dimensions, plating, or handmade claims unless explicitly supported.
- Prefer human search intent phrases.
- Think like Etsy + Amazon + eBay marketplace SEO.
- Avoid generic weak keywords when more specific long-tail phrases are possible.
- Avoid high repetition of the same root words.
- The strategy must support readable copy, not keyword stuffing.
- Title keywords should be coverable by tags.
- For Etsy especially, avoid relying on too many single-word tags.
- Favor multi-word phrases with buyer intent.
- Include gift intent only if it naturally fits the product.
- Keep outputs practical and commercially usable.
- Use the seed keywords below to simulate real buyer search behavior and marketplace autosuggest logic.
- Prefer commercially relevant product phrases over vague descriptor phrases.

{langInstruction}

{marketRules}

Brand: {meta.Brand}
SKU: {sku}
ProductName: {meta.ProductName}
Category: {meta.Category}
Material: {meta.Material}
Stone: {meta.Stone}
Color: {meta.Color}
Gender: {meta.Gender}

Seed keywords derived from product vision and metadata:
{seedList}

Image analysis JSON:
{visionJson}

Return ONLY valid JSON in this exact shape:
{{
  ""primary_keyword"": """",
  ""secondary_keywords"": ["""", """", """", """", """"],
  ""long_tail_keywords"": ["""", """", """", """", """"],
  ""title_must_include"": ["""", """", """"],
  ""tags_must_cover"": ["""", """", """", """", """", """", """", """", """", """", """", """", """"],
  ""forbidden_claims"": ["""", """", """"],
  ""tone"": """",
  ""buyer_intent"": """",
  ""differentiation_angle"": """",
  ""seed_keywords_used"": ["""", """", """", """", """", """"]
}}";
        }

        private string BuildListingPrompt(string sku, string marketplace, string language, ProductMeta meta, VisionResult vision, KeywordStrategyResult strategy)
        {
            string langInstruction = GetLanguageInstruction(language);
            string marketRules = GetMarketplaceRules(marketplace);
            string visionJson = _json.Serialize(vision);
            string strategyJson = _json.Serialize(strategy);

            return $@"
You are a senior conversion-focused marketplace copywriter and SEO listing engineer.

Your goal is to generate a high-performing product listing for {marketplace}.
The listing must be commercially strong, readable by humans, and aligned with search intent.

You must obey these rules:
- Return ONLY valid JSON.
- Do not add markdown.
- Do not add explanation.
- Do not wrap anything in code fences.
- Do not invent unsupported details.
- Do not claim sterling silver, solid gold, real gemstone, plated metal, handmade process, healing properties, hypoallergenic features, certifications, dimensions, origin story, or custom options unless explicitly supported by metadata or image analysis.
- If a claim is uncertain, omit it.
- Prefer clear buyer language over decorative fluff.
- Use keywords naturally.
- Avoid keyword stuffing.
- Avoid repetitive wording across title, bullets, description, keywords and tags.
- The title must be readable, not a keyword dump.
- The tags must help cover title intent and buyer intent.
- Use unique keyword roots where possible.
- Favor multi-word keyword phrases over weak single words.
- Keywords and tags must be usable by a real seller immediately.
- If the marketplace is Etsy, the output should feel giftable, elegant and search-smart.
- If the marketplace is Amazon, the bullets should be benefit-driven and scannable.
- If the marketplace is eBay, keep it factual, compact and title-efficient.
- If the marketplace is Website, make it branded, persuasive and clean.

Important SEO intent:
- Blend exact product intent + style intent + audience intent + gift intent where appropriate.
- Support both broad and long-tail discoverability.
- Reflect likely shopper searches without sounding robotic.
- Reuse only the most important terms when necessary.
- Do not overuse the same root word in every tag.

{langInstruction}

{marketRules}

Database metadata:
- Brand: {meta.Brand}
- SKU: {sku}
- ProductName: {meta.ProductName}
- Category: {meta.Category}
- Material: {meta.Material}
- Stone: {meta.Stone}
- Color: {meta.Color}
- Gender: {meta.Gender}

Image analysis JSON:
{visionJson}

Keyword strategy JSON:
{strategyJson}

Output constraints:
- title: marketplace optimized, readable, strong SEO
- bullet1..bullet5: distinct points, no duplicates
- description: well-structured, natural, persuasive, not too vague
- keywords: comma-separated phrases only
- tags: comma-separated phrases only
- tags should be marketplace aware
- do not output empty JSON keys with null values; use empty string only if needed

Return exactly this JSON shape:
{{
  ""title"": """",
  ""bullet1"": """",
  ""bullet2"": """",
  ""bullet3"": """",
  ""bullet4"": """",
  ""bullet5"": """",
  ""description"": """",
  ""keywords"": """",
  ""tags"": """"
}}";
        }

        private List<string> GetMarketplaceSeedKeywords(string marketplace, VisionResult vision, ProductMeta meta)
        {
            var seeds = new List<string>();

            AddSeed(seeds, meta.ProductName);
            AddSeed(seeds, meta.Category);
            AddSeed(seeds, meta.Material);
            AddSeed(seeds, meta.Stone);
            AddSeed(seeds, meta.Color);
            AddSeed(seeds, vision.product_type);
            AddSeed(seeds, vision.target_audience);

            if (vision.style_keywords != null)
            {
                foreach (string s in vision.style_keywords)
                    AddSeed(seeds, s);
            }

            if (vision.visible_colors != null)
            {
                foreach (string s in vision.visible_colors)
                    AddSeed(seeds, s);
            }

            if (vision.visible_materials != null)
            {
                foreach (string s in vision.visible_materials)
                    AddSeed(seeds, s);
            }

            string combined = " " +
                              (meta.ProductName ?? "") + " " +
                              (meta.Category ?? "") + " " +
                              (vision.product_type ?? "") + " " +
                              string.Join(" ", vision.style_keywords ?? new List<string>());

            string lc = combined.ToLowerInvariant();

            if (lc.Contains("ring"))
            {
                AddSeed(seeds, "ring");
                AddSeed(seeds, "statement ring");
                AddSeed(seeds, "gift for women");
            }

            if (lc.Contains("panther"))
            {
                AddSeed(seeds, "panther ring");
                AddSeed(seeds, "panther jewelry");
                AddSeed(seeds, "big cat ring");
            }

            if (lc.Contains("jaguar"))
            {
                AddSeed(seeds, "jaguar ring");
                AddSeed(seeds, "animal ring");
            }

            if (lc.Contains("leopard"))
            {
                AddSeed(seeds, "leopard ring");
                AddSeed(seeds, "animal jewelry");
            }

            if (lc.Contains("cat"))
            {
                AddSeed(seeds, "cat ring");
            }

            if (lc.Contains("boho"))
            {
                AddSeed(seeds, "boho ring");
            }

            if ((marketplace ?? "").Trim().Equals("Etsy", StringComparison.OrdinalIgnoreCase))
            {
                AddSeed(seeds, "giftable jewelry");
                AddSeed(seeds, "animal lover gift");
            }

            return seeds
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(CleanText)
                .Where(x => x.Length >= 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(18)
                .ToList();
        }

        private void AddSeed(List<string> seeds, string value)
        {
            if (seeds == null || string.IsNullOrWhiteSpace(value))
                return;

            string cleaned = CleanText(value);
            if (string.IsNullOrWhiteSpace(cleaned))
                return;

            seeds.Add(cleaned);
        }

        private string GetLanguageInstruction(string language)
        {
            switch ((language ?? "").Trim().ToLowerInvariant())
            {
                case "tr":
                case "turkish":
                    return "Write all output in Turkish.";
                case "pl":
                case "polish":
                    return "Write all output in Polish.";
                default:
                    return "Write all output in English.";
            }
        }

        private string GetLanguageCode(string value)
        {
            string s = (value ?? "").Trim().ToUpperInvariant();

            if (s == "EN" || s == "TR" || s == "PL")
                return s;

            if (s == "ENGLISH") return "EN";
            if (s == "TURKISH") return "TR";
            if (s == "POLISH") return "PL";

            return "EN";
        }

        private string GetLanguageName(string code)
        {
            switch ((code ?? "").Trim().ToUpperInvariant())
            {
                case "TR": return "Turkish";
                case "PL": return "Polish";
                default: return "English";
            }
        }

        private string GetMarketplaceRules(string marketplace)
        {
            switch ((marketplace ?? "").Trim().ToLowerInvariant())
            {
                case "amazon":
                    return @"
Marketplace rules for Amazon:
- Title target length: 150-190 characters where possible, but keep readable.
- Put the strongest buyer-relevant phrase early in the title.
- Avoid decorative filler.
- Bullet points should be scannable, benefit-led, and distinct from each other.
- Each bullet should focus on one major angle: design, look, material appearance, gifting, usage.
- Description should be clean and conversion-friendly.
- Keywords should be comma-separated search phrases.
- Tags can be comma-separated short phrases for internal reuse in LamaX.
- Avoid vague luxury language unless supported by the visuals.
- Avoid overusing the brand name.";

                case "etsy":
                    return @"
Marketplace rules for Etsy:

Title Rules:
- Title target length: 110-140 characters.
- The first 40 characters should contain the strongest search phrase or primary keyword.
- Titles must read naturally and not feel keyword stuffed.
- Use a clean buyer-friendly structure such as:
  MAIN KEYWORD – secondary keyword, style, gift intent.
- Avoid generic phrases like fashion jewelry, casual accessory, elegant wear, trendy style unless there is no better search phrase.

Keyword Rules:
- Prefer real buyer search phrases like panther ring, jaguar ring, animal ring, leopard ring, statement ring when relevant.
- Use keyword phrasing that resembles Etsy search suggestions and shopper intent.
- Favor multi-word phrases over weak single words.
- Balance broad discoverability with niche long-tail intent.

Tag Rules:
- Maximum 13 tags.
- Prefer 2-3 word tags when possible.
- Cover the title intent with tags.
- Avoid excessive repetition of the same root word across all tags.
- Avoid filler tags that do not help search intent.

Conversion Rules:
- Title should improve click-through rate, not only keyword density.
- Description should feel warm, giftable, clear, and easy to skim.
- The product should sound desirable and specific without overclaiming.";

                case "ebay":
                    return @"
Marketplace rules for eBay:
- Title target length: 70-80 characters.
- Prioritize the most commercial keywords first.
- Keep title dense but still readable.
- Bullet points should be short, factual, and non-fluffy.
- Description should be straightforward.
- Avoid storytelling unless it adds buying value.
- Keywords should be concise and directly relevant.
- Tags can be comma-separated short phrases for LamaX internal reuse.";

                case "website":
                    return @"
Marketplace rules for Website:
- Title should be branded, persuasive, and SEO-friendly.
- Prioritize clarity and conversion over platform-style keyword compression.
- Description can be richer and more brand-driven than marketplace listings.
- Bullets should support conversion and buyer confidence.
- Keywords should remain useful for internal SEO planning.
- Tags should help organization, onsite search, and future content mapping.";

                default:
                    return @"
Marketplace rules:
- Keep title readable and SEO-friendly.
- Keep bullets distinct and benefit-aware.
- Use practical description.
- Use clean comma-separated keywords and tags.";
            }
        }

        private int CalculateSeoScore(ListingResult r, string marketplace, KeywordStrategyResult strategy)
        {
            SeoBreakdown b = CalculateSeoBreakdown(r, marketplace, strategy);

            int score = 0;
            score += (int)Math.Round(b.TitleScore * 0.34m, MidpointRounding.AwayFromZero);
            score += (int)Math.Round(b.BulletsScore * 0.21m, MidpointRounding.AwayFromZero);
            score += (int)Math.Round(b.KeywordsScore * 0.20m, MidpointRounding.AwayFromZero);
            score += (int)Math.Round(b.TagsScore * 0.25m, MidpointRounding.AwayFromZero);

            int repetitionPenalty = GetKeywordRepetitionPenalty(r.title, r.description, SplitCsv(r.tags));
            score -= repetitionPenalty;

            string mp = (marketplace ?? "").Trim().ToLowerInvariant();

            if (mp == "etsy")
            {
                if (TitleStartsWellForEtsy(r.title, strategy))
                    score += 4;

                if (SplitCsv(r.tags).Count >= 12)
                    score += 3;

                if (CountMultiWordTags(SplitCsv(r.tags)) >= 10)
                    score += 3;

                if (ContainsWeakGenericEtsyPhrases(r.title))
                    score -= 5;

                score -= GetTagRootRepetitionPenalty(SplitCsv(r.tags));
            }

            if (score > 100) score = 100;
            if (score < 0) score = 0;

            return score;
        }

        private SeoBreakdown CalculateSeoBreakdown(ListingResult r, string marketplace, KeywordStrategyResult strategy)
        {
            SeoBreakdown b = new SeoBreakdown();

            string mp = (marketplace ?? "").Trim().ToLowerInvariant();
            string title = (r.title ?? "").Trim();
            string description = (r.description ?? "").Trim();
            List<string> keywords = SplitCsv(r.keywords);
            List<string> tags = SplitCsv(r.tags);

            int titleScore = 0;
            int titleLen = title.Length;

            if (titleLen >= 25) titleScore += 15;

            if (mp == "amazon")
            {
                if (titleLen >= 150 && titleLen <= 190) titleScore += 45;
                else if (titleLen >= 120) titleScore += 30;
                else if (titleLen >= 80) titleScore += 18;
            }
            else if (mp == "etsy")
            {
                if (titleLen >= 110 && titleLen <= 140) titleScore += 45;
                else if (titleLen >= 90 && titleLen < 110) titleScore += 34;
                else if (titleLen >= 70) titleScore += 22;

                if (TitleStartsWellForEtsy(title, strategy))
                    titleScore += 15;

                if (!ContainsWeakGenericEtsyPhrases(title))
                    titleScore += 8;
            }
            else if (mp == "ebay")
            {
                if (titleLen >= 65 && titleLen <= 80) titleScore += 45;
                else if (titleLen >= 50) titleScore += 30;
                else if (titleLen >= 35) titleScore += 18;
            }
            else
            {
                if (titleLen >= 55 && titleLen <= 160) titleScore += 45;
                else if (titleLen >= 40) titleScore += 30;
                else if (titleLen >= 25) titleScore += 18;
            }

            int titleTagCoverage = CountTitleTagCoverage(title, tags);
            if (titleTagCoverage >= 5) titleScore += 18;
            else if (titleTagCoverage >= 3) titleScore += 12;
            else if (titleTagCoverage >= 1) titleScore += 6;

            if (!string.IsNullOrWhiteSpace(title) && title.Split(' ').Length >= 4)
                titleScore += 8;

            b.TitleScore = Clamp100(titleScore);

            string[] bullets = { r.bullet1, r.bullet2, r.bullet3, r.bullet4, r.bullet5 };
            int filledBullets = bullets.Count(x => !string.IsNullOrWhiteSpace(x));
            int distinctBullets = bullets
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            int bulletsScore = 0;
            bulletsScore += Math.Min(60, filledBullets * 12);

            if (distinctBullets >= 5) bulletsScore += 25;
            else if (distinctBullets >= 4) bulletsScore += 18;
            else if (distinctBullets >= 3) bulletsScore += 10;

            if (bullets.Where(x => !string.IsNullOrWhiteSpace(x)).Any(x => x.Length >= 40))
                bulletsScore += 15;

            b.BulletsScore = Clamp100(bulletsScore);

            int keywordsScore = 0;
            if (keywords.Count >= 10) keywordsScore += 68;
            else if (keywords.Count >= 8) keywordsScore += 54;
            else if (keywords.Count >= 5) keywordsScore += 36;
            else if (keywords.Count >= 3) keywordsScore += 20;

            int multiWordKeywords = keywords.Count(x => x.Contains(" "));
            if (multiWordKeywords >= 6) keywordsScore += 20;
            else if (multiWordKeywords >= 3) keywordsScore += 12;

            if (description.Length >= 220)
                keywordsScore += 10;

            if (mp == "etsy" && strategy != null && !string.IsNullOrWhiteSpace(strategy.primary_keyword))
            {
                if (keywords.Any(x => x.IndexOf(strategy.primary_keyword, StringComparison.OrdinalIgnoreCase) >= 0))
                    keywordsScore += 2;
            }

            b.KeywordsScore = Clamp100(keywordsScore);

            int tagsScore = 0;
            if (mp == "etsy")
            {
                if (tags.Count >= 13) tagsScore += 60;
                else if (tags.Count >= 10) tagsScore += 48;
                else if (tags.Count >= 7) tagsScore += 34;

                int multiWordTags = CountMultiWordTags(tags);
                if (multiWordTags >= 10) tagsScore += 20;
                else if (multiWordTags >= 7) tagsScore += 14;

                int exactishCoverage = CountTitleTagCoverage(title, tags);
                if (exactishCoverage >= 4) tagsScore += 10;
                else if (exactishCoverage >= 2) tagsScore += 6;

                int rootPenalty = GetTagRootRepetitionPenalty(tags);
                tagsScore -= rootPenalty;
            }
            else
            {
                if (tags.Count >= 10) tagsScore += 65;
                else if (tags.Count >= 8) tagsScore += 50;
                else if (tags.Count >= 5) tagsScore += 35;

                int multiWordTags = tags.Count(x => x.Contains(" "));
                if (multiWordTags >= 5) tagsScore += 20;
                else if (multiWordTags >= 3) tagsScore += 10;

                if (CountTitleTagCoverage(title, tags) >= 2)
                    tagsScore += 15;
            }

            b.TagsScore = Clamp100(tagsScore);
            return b;
        }

        private int Clamp100(int value)
        {
            if (value < 0) return 0;
            if (value > 100) return 100;
            return value;
        }

        private int CountTitleTagCoverage(string title, List<string> tags)
        {
            if (string.IsNullOrWhiteSpace(title) || tags == null || tags.Count == 0)
                return 0;

            var titleWords = Regex.Split(title.ToLowerInvariant(), @"[^a-z0-9çğıöşü]+")
                .Where(x => x.Length >= 4)
                .Distinct()
                .ToList();

            int count = 0;

            foreach (string word in titleWords)
            {
                if (tags.Any(t => t.ToLowerInvariant().Contains(word)))
                    count++;
            }

            return count;
        }

        private int GetKeywordRepetitionPenalty(string title, string description, List<string> tags)
        {
            string combined = (title ?? "") + " " + (description ?? "") + " " + string.Join(" ", tags ?? new List<string>());

            var words = Regex.Split(combined.ToLowerInvariant(), @"[^a-z0-9çğıöşü]+")
                .Where(x => x.Length >= 4)
                .ToList();

            var repeated = words
                .GroupBy(x => x)
                .Select(g => new { Word = g.Key, Count = g.Count() })
                .Where(x => x.Count >= 6)
                .Count();

            return Math.Min(10, repeated * 2);
        }

        private bool TitleStartsWellForEtsy(string title, KeywordStrategyResult strategy)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            string start = title.Trim();
            if (start.Length > 45)
                start = start.Substring(0, 45);

            List<string> candidates = new List<string>();

            if (strategy != null)
            {
                if (!string.IsNullOrWhiteSpace(strategy.primary_keyword))
                    candidates.Add(strategy.primary_keyword);

                if (strategy.title_must_include != null)
                    candidates.AddRange(strategy.title_must_include.Where(x => !string.IsNullOrWhiteSpace(x)).Take(3));
            }

            candidates.AddRange(new[]
            {
                "panther ring",
                "jaguar ring",
                "leopard ring",
                "animal ring",
                "statement ring",
                "cat ring"
            });

            return candidates
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLowerInvariant())
                .Distinct()
                .Any(x => start.ToLowerInvariant().Contains(x));
        }

        private bool ContainsWeakGenericEtsyPhrases(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            string s = title.ToLowerInvariant();

            string[] weak =
            {
                "fashion jewelry",
                "casual wear",
                "party wear",
                "casual accessory",
                "stylish accessory",
                "elegant wear",
                "trendy style"
            };

            return weak.Any(x => s.Contains(x));
        }

        private int CountMultiWordTags(List<string> tags)
        {
            return (tags ?? new List<string>())
                .Count(x => !string.IsNullOrWhiteSpace(x) && x.Trim().Contains(" "));
        }

        private int GetTagRootRepetitionPenalty(List<string> tags)
        {
            if (tags == null || tags.Count == 0)
                return 0;

            var words = tags
                .SelectMany(x => Regex.Split((x ?? "").ToLowerInvariant(), @"[^a-z0-9çğıöşü]+"))
                .Where(x => x.Length >= 4)
                .Where(x => x != "gift" && x != "for" && x != "with" && x != "women")
                .ToList();

            int penalty = 0;

            foreach (var g in words.GroupBy(x => x))
            {
                if (g.Count() >= 5)
                    penalty += 2;
            }

            if (penalty > 10)
                penalty = 10;

            return penalty;
        }

        private void NormalizeListingByMarketplace(ListingResult result, string marketplace, KeywordStrategyResult strategy)
        {
            if (result == null) return;

            result.title = CleanText(result.title);
            result.bullet1 = CleanText(result.bullet1);
            result.bullet2 = CleanText(result.bullet2);
            result.bullet3 = CleanText(result.bullet3);
            result.bullet4 = CleanText(result.bullet4);
            result.bullet5 = CleanText(result.bullet5);
            result.description = CleanText(result.description);
            result.keywords = NormalizePhraseCsv(result.keywords);
            result.tags = NormalizePhraseCsv(result.tags);

            string mp = (marketplace ?? "").Trim().ToLowerInvariant();

            if (mp == "etsy")
            {
                result.title = TrimSafe(result.title, 140);
                result.tags = NormalizeEtsyTags(result.tags, strategy);
            }
            else if (mp == "ebay")
            {
                result.title = TrimSafe(result.title, 80);
                result.tags = LimitCsvItems(result.tags, 12, 25);
            }
            else if (mp == "amazon")
            {
                result.title = TrimSafe(result.title, 190);
                result.tags = LimitCsvItems(result.tags, 20, 40);
            }
            else if (mp == "website")
            {
                result.title = TrimSafe(result.title, 160);
                result.tags = LimitCsvItems(result.tags, 20, 40);
            }
        }

        private string NormalizeEtsyTags(string input, KeywordStrategyResult strategy)
        {
            List<string> tags = SplitCsv(input)
                .Select(CleanText)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => x.Length <= 20)
                .Where(x => !IsWeakEtsyTag(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (strategy != null && strategy.tags_must_cover != null)
            {
                foreach (string must in strategy.tags_must_cover)
                {
                    string cleaned = CleanText(must);
                    if (!string.IsNullOrWhiteSpace(cleaned) && cleaned.Length <= 20)
                    {
                        if (!tags.Any(x => x.Equals(cleaned, StringComparison.OrdinalIgnoreCase)))
                            tags.Add(cleaned);
                    }
                }
            }

            tags = tags
                .OrderByDescending(x => x.Contains(" "))
                .ThenBy(x => x.Length)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(13)
                .ToList();

            return string.Join(", ", tags);
        }

        private bool IsWeakEtsyTag(string tag)
        {
            string s = (tag ?? "").Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(s))
                return true;

            string[] weak =
            {
                "fashion jewelry",
                "casual wear",
                "party wear",
                "gift",
                "jewelry",
                "ring",
                "accessory"
            };

            return weak.Contains(s);
        }

        private string BuildImageSignature(List<ProductImageInfo> images)
        {
            if (images == null || images.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (var img in images.OrderBy(x => x.ProductImageID))
            {
                int len = img.ImageData != null ? img.ImageData.Length : 0;

                sb.Append(img.ProductImageID).Append("|")
                  .Append(img.FileName ?? "").Append("|")
                  .Append(img.ContentType ?? "").Append("|")
                  .Append(img.ImageRole ?? "").Append("|")
                  .Append(len).Append(";");
            }

            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        private VisionResult LoadVisionCache(string sku, string imageSignature)
        {
            const string sql = @"
SELECT TOP 1
       ProductType,
       TargetAudience,
       StyleKeywords,
       VisibleMaterials,
       VisibleColors,
       UseCases,
       Giftable,
       Confidence,
       Summary
FROM dbo.T_ProductAIVisionCache
WHERE SKU = @SKU
  AND ImageSignature = @ImageSignature;";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@ImageSignature", imageSignature);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    return new VisionResult
                    {
                        product_type = SafeString(dr["ProductType"]),
                        target_audience = SafeString(dr["TargetAudience"]),
                        style_keywords = SplitPipeOrCsv(SafeString(dr["StyleKeywords"])),
                        visible_materials = SplitPipeOrCsv(SafeString(dr["VisibleMaterials"])),
                        visible_colors = SplitPipeOrCsv(SafeString(dr["VisibleColors"])),
                        use_cases = SplitPipeOrCsv(SafeString(dr["UseCases"])),
                        giftable = SafeString(dr["Giftable"]),
                        confidence = SafeDouble(dr["Confidence"]),
                        summary = SafeString(dr["Summary"])
                    };
                }
            }
        }

        private void SaveVisionCache(string sku, string marketplace, string languageCode, string imageSignature, int imageCount, VisionResult vision)
        {
            string styleKeywords = JoinList(vision.style_keywords);
            string visibleMaterials = JoinList(vision.visible_materials);
            string visibleColors = JoinList(vision.visible_colors);
            string useCases = JoinList(vision.use_cases);
            string visionJson = _json.Serialize(vision);

            const string sql = @"
IF EXISTS (
    SELECT 1
    FROM dbo.T_ProductAIVisionCache
    WHERE SKU = @SKU
      AND ImageSignature = @ImageSignature
)
BEGIN
    UPDATE dbo.T_ProductAIVisionCache
       SET Marketplace = @Marketplace,
           Language = @Language,
           ImageCount = @ImageCount,
           ProductType = @ProductType,
           TargetAudience = @TargetAudience,
           StyleKeywords = @StyleKeywords,
           VisibleMaterials = @VisibleMaterials,
           VisibleColors = @VisibleColors,
           UseCases = @UseCases,
           Giftable = @Giftable,
           Confidence = @Confidence,
           Summary = @Summary,
           VisionJson = @VisionJson,
           UpdatedAt = GETDATE()
     WHERE SKU = @SKU
       AND ImageSignature = @ImageSignature;
END
ELSE
BEGIN
    INSERT INTO dbo.T_ProductAIVisionCache
    (
        SKU, Marketplace, Language, ImageSignature, ImageCount,
        ProductType, TargetAudience, StyleKeywords, VisibleMaterials,
        VisibleColors, UseCases, Giftable, Confidence, Summary, VisionJson,
        CreatedAt
    )
    VALUES
    (
        @SKU, @Marketplace, @Language, @ImageSignature, @ImageCount,
        @ProductType, @TargetAudience, @StyleKeywords, @VisibleMaterials,
        @VisibleColors, @UseCases, @Giftable, @Confidence, @Summary, @VisionJson,
        GETDATE()
    );
END";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace ?? "");
                cmd.Parameters.AddWithValue("@Language", languageCode ?? "");
                cmd.Parameters.AddWithValue("@ImageSignature", imageSignature ?? "");
                cmd.Parameters.AddWithValue("@ImageCount", imageCount);
                cmd.Parameters.AddWithValue("@ProductType", (object)(vision.product_type ?? ""));
                cmd.Parameters.AddWithValue("@TargetAudience", (object)(vision.target_audience ?? ""));
                cmd.Parameters.AddWithValue("@StyleKeywords", (object)styleKeywords);
                cmd.Parameters.AddWithValue("@VisibleMaterials", (object)visibleMaterials);
                cmd.Parameters.AddWithValue("@VisibleColors", (object)visibleColors);
                cmd.Parameters.AddWithValue("@UseCases", (object)useCases);
                cmd.Parameters.AddWithValue("@Giftable", (object)(vision.giftable ?? ""));
                cmd.Parameters.AddWithValue("@Confidence", vision.confidence);
                cmd.Parameters.AddWithValue("@Summary", (object)(vision.summary ?? ""));
                cmd.Parameters.AddWithValue("@VisionJson", (object)visionJson);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveListing(string sku, string marketplace, string languageCode, ListingResult listing, string visionSummary, string strategySummary)
        {
            const string sql = @"
IF EXISTS (
    SELECT 1
    FROM dbo.T_ProductAIContent
    WHERE SKU = @SKU AND Marketplace = @Marketplace AND LanguageCode = @LanguageCode
)
BEGIN
    UPDATE dbo.T_ProductAIContent
       SET Title = @Title,
           Bullet1 = @Bullet1,
           Bullet2 = @Bullet2,
           Bullet3 = @Bullet3,
           Bullet4 = @Bullet4,
           Bullet5 = @Bullet5,
           Description = @Description,
           Keywords = @Keywords,
           Tags = @Tags,
           SeoScore = @SeoScore,
           VisionSummary = @VisionSummary,
           StrategySummary = @StrategySummary,
           UpdatedAt = GETDATE(),
           UpdatedBy = @UserName
     WHERE SKU = @SKU AND Marketplace = @Marketplace AND LanguageCode = @LanguageCode;
END
ELSE
BEGIN
    INSERT INTO dbo.T_ProductAIContent
    (
        SKU, Marketplace, LanguageCode,
        Title, Bullet1, Bullet2, Bullet3, Bullet4, Bullet5,
        Description, Keywords, Tags, SeoScore,
        VisionSummary, StrategySummary, CreatedAt, CreatedBy
    )
    VALUES
    (
        @SKU, @Marketplace, @LanguageCode,
        @Title, @Bullet1, @Bullet2, @Bullet3, @Bullet4, @Bullet5,
        @Description, @Keywords, @Tags, @SeoScore,
        @VisionSummary, @StrategySummary, GETDATE(), @UserName
    );
END";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace);
                cmd.Parameters.AddWithValue("@LanguageCode", languageCode);
                cmd.Parameters.AddWithValue("@Title", (object)(listing.title ?? ""));
                cmd.Parameters.AddWithValue("@Bullet1", (object)(listing.bullet1 ?? ""));
                cmd.Parameters.AddWithValue("@Bullet2", (object)(listing.bullet2 ?? ""));
                cmd.Parameters.AddWithValue("@Bullet3", (object)(listing.bullet3 ?? ""));
                cmd.Parameters.AddWithValue("@Bullet4", (object)(listing.bullet4 ?? ""));
                cmd.Parameters.AddWithValue("@Bullet5", (object)(listing.bullet5 ?? ""));
                cmd.Parameters.AddWithValue("@Description", (object)(listing.description ?? ""));
                cmd.Parameters.AddWithValue("@Keywords", (object)(listing.keywords ?? ""));
                cmd.Parameters.AddWithValue("@Tags", (object)(listing.tags ?? ""));
                cmd.Parameters.AddWithValue("@SeoScore", listing.SeoScore);
                cmd.Parameters.AddWithValue("@VisionSummary", (object)(visionSummary ?? ""));
                cmd.Parameters.AddWithValue("@StrategySummary", (object)(strategySummary ?? ""));
                cmd.Parameters.AddWithValue("@UserName", GetCurrentUserName());

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private SavedAiContent LoadSavedListing(string sku, string marketplace, string languageCode)
        {
            const string sql = @"
SELECT TOP 1 *
FROM dbo.T_ProductAIContent
WHERE SKU = @SKU
  AND Marketplace = @Marketplace
  AND LanguageCode = @LanguageCode;";

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@SKU", sku);
                cmd.Parameters.AddWithValue("@Marketplace", marketplace);
                cmd.Parameters.AddWithValue("@LanguageCode", languageCode);

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    return new SavedAiContent
                    {
                        Title = SafeString(dr["Title"]),
                        Bullet1 = SafeString(dr["Bullet1"]),
                        Bullet2 = SafeString(dr["Bullet2"]),
                        Bullet3 = SafeString(dr["Bullet3"]),
                        Bullet4 = SafeString(dr["Bullet4"]),
                        Bullet5 = SafeString(dr["Bullet5"]),
                        Description = SafeString(dr["Description"]),
                        Keywords = SafeString(dr["Keywords"]),
                        Tags = SafeString(dr["Tags"]),
                        SeoScore = SafeInt(dr["SeoScore"]),
                        VisionSummary = SafeString(dr["VisionSummary"]),
                        StrategySummary = HasColumn(dr, "StrategySummary") ? SafeString(dr["StrategySummary"]) : ""
                    };
                }
            }
        }

        private void FillForm(ListingResult listing)
        {
            txtTitle.Text = listing.title ?? "";
            txtBullet1.Text = listing.bullet1 ?? "";
            txtBullet2.Text = listing.bullet2 ?? "";
            txtBullet3.Text = listing.bullet3 ?? "";
            txtBullet4.Text = listing.bullet4 ?? "";
            txtBullet5.Text = listing.bullet5 ?? "";
            txtDescription.Text = listing.description ?? "";
            txtKeywords.Text = listing.keywords ?? "";
            txtTags.Text = listing.tags ?? "";
        }

        private ListingResult ReadForm()
        {
            return new ListingResult
            {
                title = (txtTitle.Text ?? "").Trim(),
                bullet1 = (txtBullet1.Text ?? "").Trim(),
                bullet2 = (txtBullet2.Text ?? "").Trim(),
                bullet3 = (txtBullet3.Text ?? "").Trim(),
                bullet4 = (txtBullet4.Text ?? "").Trim(),
                bullet5 = (txtBullet5.Text ?? "").Trim(),
                description = (txtDescription.Text ?? "").Trim(),
                keywords = (txtKeywords.Text ?? "").Trim(),
                tags = (txtTags.Text ?? "").Trim(),
                SeoScore = SafeInt(litSeoScore.Text)
            };
        }

        private void LoadProductPreview(List<ProductImageInfo> images, string sku, string marketplace)
        {
            imgProductPreview.ImageUrl = "";
            imgProductPreview.Style["display"] = "none";
            litNoImage.Text = "Görsel henüz yüklenmedi.";
            litPreviewMeta.Text = "";

            int imageCount = images != null ? images.Count : 0;

            if (images == null || images.Count == 0)
            {
                litPreviewMeta.Text =
                    "<span class='meta-pill'>SKU: " + Server.HtmlEncode(sku) + "</span>" +
                    "<span class='meta-pill'>Marketplace: " + Server.HtmlEncode(marketplace) + "</span>" +
                    "<span class='meta-pill'>Images analyzed: 0</span>";
                return;
            }

            ProductImageInfo img = images.FirstOrDefault();
            if (img != null && img.ImageData != null && img.ImageData.Length > 0)
            {
                string contentType = string.IsNullOrWhiteSpace(img.ContentType) ? "image/jpeg" : img.ContentType;
                string base64 = Convert.ToBase64String(img.ImageData);

                imgProductPreview.ImageUrl = "data:" + contentType + ";base64," + base64;
                imgProductPreview.AlternateText = "Product Preview - " + (img.FileName ?? "");
                imgProductPreview.Style["display"] = "inline-block";
                litNoImage.Text = "";
            }

            litPreviewMeta.Text =
                "<span class='meta-pill'>SKU: " + Server.HtmlEncode(sku) + "</span>" +
                "<span class='meta-pill'>Marketplace: " + Server.HtmlEncode(marketplace) + "</span>" +
                "<span class='meta-pill'>Images analyzed: " + imageCount.ToString() + "</span>";
        }

        private string BuildVisionSummaryHtml(VisionResult v, int imageCount, bool loadedFromCache, bool forceRefresh)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='vision-line'><strong>Source:</strong> ")
              .Append(forceRefresh ? "Forced Fresh AI Analysis" : (loadedFromCache ? "Vision Cache" : "Fresh AI Analysis"))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Images analyzed:</strong> ")
              .Append(imageCount.ToString())
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Product type:</strong> ")
              .Append(Server.HtmlEncode(v.product_type))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Audience:</strong> ")
              .Append(Server.HtmlEncode(v.target_audience))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Style:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", v.style_keywords ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Materials:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", v.visible_materials ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Colors:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", v.visible_colors ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Use cases:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", v.use_cases ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='vision-line'><strong>Giftable:</strong> ")
              .Append(Server.HtmlEncode(v.giftable))
              .Append("</div>");

            double conf = v.confidence;
            if (conf > 1) conf = conf / 100.0;
            if (conf > 1) conf = 1;
            if (conf < 0) conf = 0;

            sb.Append("<div class='vision-line'><strong>Confidence:</strong> ")
              .Append(Server.HtmlEncode((conf * 100.0).ToString("0")))
              .Append("%</div>");

            if (!string.IsNullOrWhiteSpace(v.summary))
            {
                sb.Append("<div class='vision-line'><strong>Summary:</strong> ")
                  .Append(Server.HtmlEncode(v.summary))
                  .Append("</div>");
            }

            return sb.ToString();
        }

        private string BuildStrategySummaryHtml(KeywordStrategyResult s)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='strategy-line'><strong>Primary Keyword:</strong> ")
              .Append(Server.HtmlEncode(s.primary_keyword))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Secondary Keywords:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", s.secondary_keywords ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Long Tail Keywords:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", s.long_tail_keywords ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Title Must Include:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", s.title_must_include ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Tags Must Cover:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", s.tags_must_cover ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Forbidden Claims:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", s.forbidden_claims ?? new List<string>())))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Tone:</strong> ")
              .Append(Server.HtmlEncode(s.tone))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Buyer Intent:</strong> ")
              .Append(Server.HtmlEncode(s.buyer_intent))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Differentiation:</strong> ")
              .Append(Server.HtmlEncode(s.differentiation_angle))
              .Append("</div>");

            sb.Append("<div class='strategy-line'><strong>Seed Keywords Used:</strong> ")
              .Append(Server.HtmlEncode(string.Join(", ", s.seed_keywords_used ?? new List<string>())))
              .Append("</div>");

            return sb.ToString();
        }

        private string BuildMultiMarketplaceHtml(List<MultiMarketplaceResult> results)
        {
            if (results == null || results.Count == 0)
                return "Henüz çoklu marketplace üretimi yapılmadı.";

            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='market-tabs'>");

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                string key = Slugify(r.Marketplace);

                sb.Append("<div id='tab_").Append(key).Append("' class='market-tab")
                  .Append(i == 0 ? " active" : "")
                  .Append("' onclick=\"showMarketPanel('").Append(key).Append("')\">")
                  .Append(Server.HtmlEncode(r.Marketplace))
                  .Append("</div>");
            }

            sb.Append("</div>");

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i];
                string key = Slugify(r.Marketplace);

                sb.Append("<div id='panel_").Append(key).Append("' class='market-panel")
                  .Append(i == 0 ? " active" : "")
                  .Append("'>");

                sb.Append("<h4>").Append(Server.HtmlEncode(r.Marketplace)).Append(" Result</h4>");

                sb.Append("<div>")
                  .Append("<span class='market-kpi'>SEO Score: ").Append(r.Listing.SeoScore).Append("</span>")
                  .Append("<span class='market-kpi'>Primary Keyword: ").Append(Server.HtmlEncode(r.Strategy.primary_keyword)).Append("</span>")
                  .Append("<span class='market-kpi'>Tone: ").Append(Server.HtmlEncode(r.Strategy.tone)).Append("</span>")
                  .Append("</div>");

                sb.Append("<div class='market-grid'>");

                AppendMarketField(sb, "Title", r.Listing.title, false);
                AppendMarketField(sb, "Keywords", r.Listing.keywords, false);
                AppendMarketField(sb, "Bullet 1", r.Listing.bullet1, false);
                AppendMarketField(sb, "Bullet 2", r.Listing.bullet2, false);
                AppendMarketField(sb, "Bullet 3", r.Listing.bullet3, false);
                AppendMarketField(sb, "Bullet 4", r.Listing.bullet4, false);

                sb.Append("<div class='market-grid-full'>");
                AppendMarketFieldInner(sb, "Bullet 5", r.Listing.bullet5, false);
                sb.Append("</div>");

                sb.Append("<div class='market-grid-full'>");
                AppendMarketFieldInner(sb, "Description", r.Listing.description, true);
                sb.Append("</div>");

                sb.Append("<div class='market-grid-full'>");
                AppendMarketFieldInner(sb, "Tags", r.Listing.tags, false);
                sb.Append("</div>");

                sb.Append("<div class='market-grid-full'>");
                AppendMarketFieldInner(sb, "Strategy Summary", BuildStrategyPlainText(r.Strategy), true);
                sb.Append("</div>");

                sb.Append("</div>");
                sb.Append("</div>");
            }

            sb.Append("<script type='text/javascript'>setTimeout(function(){showMarketPanel('")
              .Append(Slugify(results.First().Marketplace))
              .Append("');}, 50);</script>");

            return sb.ToString();
        }

        private void AppendMarketField(StringBuilder sb, string label, string value, bool isLong)
        {
            sb.Append("<div class='market-field'>");
            AppendMarketFieldInner(sb, label, value, isLong);
            sb.Append("</div>");
        }

        private void AppendMarketFieldInner(StringBuilder sb, string label, string value, bool isLong)
        {
            sb.Append("<label>").Append(Server.HtmlEncode(label)).Append("</label>")
              .Append("<div class='market-value")
              .Append(isLong ? " long" : "")
              .Append("'>")
              .Append(Server.HtmlEncode(value ?? ""))
              .Append("</div>");
        }

        private string BuildStrategyPlainText(KeywordStrategyResult s)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Primary Keyword: ").Append(s.primary_keyword).Append("\n");
            sb.Append("Secondary Keywords: ").Append(string.Join(", ", s.secondary_keywords ?? new List<string>())).Append("\n");
            sb.Append("Long Tail Keywords: ").Append(string.Join(", ", s.long_tail_keywords ?? new List<string>())).Append("\n");
            sb.Append("Title Must Include: ").Append(string.Join(", ", s.title_must_include ?? new List<string>())).Append("\n");
            sb.Append("Tags Must Cover: ").Append(string.Join(", ", s.tags_must_cover ?? new List<string>())).Append("\n");
            sb.Append("Forbidden Claims: ").Append(string.Join(", ", s.forbidden_claims ?? new List<string>())).Append("\n");
            sb.Append("Tone: ").Append(s.tone).Append("\n");
            sb.Append("Buyer Intent: ").Append(s.buyer_intent).Append("\n");
            sb.Append("Differentiation: ").Append(s.differentiation_angle).Append("\n");
            sb.Append("Seed Keywords Used: ").Append(string.Join(", ", s.seed_keywords_used ?? new List<string>()));

            return sb.ToString();
        }

        private void NormalizeVisionResult(VisionResult result)
        {
            if (result.style_keywords == null) result.style_keywords = new List<string>();
            if (result.visible_materials == null) result.visible_materials = new List<string>();
            if (result.visible_colors == null) result.visible_colors = new List<string>();
            if (result.use_cases == null) result.use_cases = new List<string>();

            result.product_type = NullToEmpty(result.product_type);
            result.target_audience = NullToEmpty(result.target_audience);
            result.giftable = NullToEmpty(result.giftable);
            result.summary = NullToEmpty(result.summary);
        }

        private void NormalizeKeywordStrategy(KeywordStrategyResult result)
        {
            if (result.secondary_keywords == null) result.secondary_keywords = new List<string>();
            if (result.long_tail_keywords == null) result.long_tail_keywords = new List<string>();
            if (result.title_must_include == null) result.title_must_include = new List<string>();
            if (result.tags_must_cover == null) result.tags_must_cover = new List<string>();
            if (result.forbidden_claims == null) result.forbidden_claims = new List<string>();
            if (result.seed_keywords_used == null) result.seed_keywords_used = new List<string>();

            result.primary_keyword = NullToEmpty(result.primary_keyword);
            result.tone = NullToEmpty(result.tone);
            result.buyer_intent = NullToEmpty(result.buyer_intent);
            result.differentiation_angle = NullToEmpty(result.differentiation_angle);

            result.secondary_keywords = NormalizeStringList(result.secondary_keywords);
            result.long_tail_keywords = NormalizeStringList(result.long_tail_keywords);
            result.title_must_include = NormalizeStringList(result.title_must_include);
            result.tags_must_cover = NormalizeStringList(result.tags_must_cover);
            result.forbidden_claims = NormalizeStringList(result.forbidden_claims);
            result.seed_keywords_used = NormalizeStringList(result.seed_keywords_used);
        }

        private List<string> NormalizeStringList(List<string> source)
        {
            return (source ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private void NormalizeListingResult(ListingResult result)
        {
            result.title = NullToEmpty(result.title);
            result.bullet1 = NullToEmpty(result.bullet1);
            result.bullet2 = NullToEmpty(result.bullet2);
            result.bullet3 = NullToEmpty(result.bullet3);
            result.bullet4 = NullToEmpty(result.bullet4);
            result.bullet5 = NullToEmpty(result.bullet5);
            result.description = NullToEmpty(result.description);
            result.keywords = NullToEmpty(result.keywords);
            result.tags = NullToEmpty(result.tags);
        }

        private string CleanJson(string text)
        {
            string s = (text ?? "").Trim();

            if (s.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(7).Trim();

            if (s.StartsWith("```"))
                s = s.Substring(3).Trim();

            if (s.EndsWith("```"))
                s = s.Substring(0, s.Length - 3).Trim();

            return s.Trim();
        }

        private string CleanText(string input)
        {
            string s = (input ?? "").Trim();
            s = Regex.Replace(s, @"\s+", " ");
            s = s.Replace(" ,", ",")
                 .Replace(" .", ".")
                 .Replace(" ;", ";")
                 .Replace(" :", ":");
            return s.Trim();
        }

        private string NormalizePhraseCsv(string input)
        {
            var list = (input ?? "")
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => CleanText(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return string.Join(", ", list);
        }

        private string TrimSafe(string input, int maxLen)
        {
            string s = CleanText(input);
            if (s.Length <= maxLen) return s;

            s = s.Substring(0, maxLen).Trim();

            int lastSpace = s.LastIndexOf(' ');
            if (lastSpace > 20)
                s = s.Substring(0, lastSpace).Trim();

            return s.Trim(' ', ',', ';', '-', '|');
        }

        private string LimitCsvItems(string input, int maxItems, int maxItemLength)
        {
            var list = (input ?? "")
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => CleanText(x))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => x.Length <= maxItemLength)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(maxItems)
                .ToList();

            return string.Join(", ", list);
        }

        private string Slugify(string input)
        {
            string s = (input ?? "").Trim().ToLowerInvariant();
            s = Regex.Replace(s, @"[^a-z0-9]+", "_");
            s = s.Trim('_');
            return string.IsNullOrWhiteSpace(s) ? "market" : s;
        }

        private string FirstExisting(List<string> columns, params string[] names)
        {
            foreach (string name in names)
            {
                string found = columns.FirstOrDefault(x => x.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(found))
                    return found;
            }

            return null;
        }

        private bool HasColumn(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private string JoinList(List<string> items)
        {
            if (items == null || items.Count == 0)
                return "";

            return string.Join("|", items
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private List<string> SplitPipeOrCsv(string input)
        {
            return (input ?? "")
                .Split(new[] { '|', ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private int SafeInt(object o)
        {
            if (o == null || o == DBNull.Value) return 0;

            int val;
            return int.TryParse(o.ToString(), out val) ? val : 0;
        }

        private double SafeDouble(object o)
        {
            if (o == null || o == DBNull.Value) return 0;

            double val;
            return double.TryParse(o.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out val) ? val : 0;
        }

        private string SafeString(object o)
        {
            return o == null || o == DBNull.Value ? "" : o.ToString();
        }

        private string NullToEmpty(string s)
        {
            return s ?? "";
        }

        private List<string> SplitCsv(string input)
        {
            return (input ?? "")
                .Split(new[] { ',', ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private string StripHtml(string html)
        {
            return Regex.Replace(html ?? "", "<.*?>", " ").Trim();
        }

        private string GetCurrentUserName()
        {
            try
            {
                if (Context != null &&
                    Context.User != null &&
                    Context.User.Identity != null &&
                    Context.User.Identity.IsAuthenticated)
                {
                    return Context.User.Identity.Name ?? "system";
                }
            }
            catch
            {
            }

            return "system";
        }

        private void SetSeoVisual(int score)
        {
            if (score <= 0)
            {
                litSeoScore.Text = "-";
                litSeoLevel.Text = "No score yet";
                litSeoHint.Text = "AI çıktısına göre hesaplanır.";
                return;
            }

            litSeoScore.Text = score.ToString();

            if (score >= 90)
            {
                litSeoLevel.Text = "Excellent SEO";
                litSeoHint.Text = "Title, bullets, keywords ve tag yapısı oldukça güçlü görünüyor.";
            }
            else if (score >= 75)
            {
                litSeoLevel.Text = "Strong SEO";
                litSeoHint.Text = "İçerik iyi seviyede. Küçük optimizasyonlarla daha da güçlenebilir.";
            }
            else if (score >= 60)
            {
                litSeoLevel.Text = "Average SEO";
                litSeoHint.Text = "Temel yapı iyi, ancak title/tag/keyword hizasında iyileştirme yapılabilir.";
            }
            else
            {
                litSeoLevel.Text = "Needs Improvement";
                litSeoHint.Text = "Title, açıklama, tag veya keyword alanları daha iyi optimize edilebilir.";
            }
        }

        private void SetRadarVisual(int titleScore, int bulletsScore, int keywordsScore, int tagsScore)
        {
            int t = Clamp100(titleScore);
            int b = Clamp100(bulletsScore);
            int k = Clamp100(keywordsScore);
            int g = Clamp100(tagsScore);

            SetRadarBar(radarTitleBar, t);
            SetRadarBar(radarBulletsBar, b);
            SetRadarBar(radarKeywordsBar, k);
            SetRadarBar(radarTagsBar, g);

            litRadarTitleText.Text = t.ToString() + "%";
            litRadarBulletsText.Text = b.ToString() + "%";
            litRadarKeywordsText.Text = k.ToString() + "%";
            litRadarTagsText.Text = g.ToString() + "%";
        }

        private void SetRadarBar(System.Web.UI.HtmlControls.HtmlGenericControl bar, int score)
        {
            if (bar == null) return;

            bar.Attributes["class"] = "seo-radar-fill " + GetRadarClass(score);
            bar.Style["width"] = Clamp100(score).ToString() + "%";
        }

        private string GetRadarClass(int score)
        {
            score = Clamp100(score);

            if (score >= 75) return "high";
            if (score >= 50) return "medium";
            return "low";
        }

        private void StoreAllResultsInViewState(List<MultiMarketplaceResult> results)
        {
            if (results == null || results.Count == 0)
            {
                ViewState["AI_ALL_RESULTS_JSON"] = "";
                return;
            }

            ViewState["AI_ALL_RESULTS_JSON"] = _json.Serialize(results);
        }

        private List<MultiMarketplaceResult> GetAllResultsFromViewState()
        {
            string raw = ViewState["AI_ALL_RESULTS_JSON"] as string;

            if (string.IsNullOrWhiteSpace(raw))
                return new List<MultiMarketplaceResult>();

            try
            {
                return _json.Deserialize<List<MultiMarketplaceResult>>(raw) ?? new List<MultiMarketplaceResult>();
            }
            catch
            {
                return new List<MultiMarketplaceResult>();
            }
        }

        private void ShowSuccess(string message)
        {
            litStatus.Text = "<div class='status-ok'>" + Server.HtmlEncode(message) + "</div>";
        }

        private void ShowError(string message)
        {
            litStatus.Text = "<div class='status-err'>" + Server.HtmlEncode(message) + "</div>";
        }

        private void ClearStatus()
        {
            litStatus.Text = "";
        }
    }

    public class ProductImageInfo
    {
        public int ProductImageID { get; set; }
        public string SKU { get; set; }
        public byte[] ImageData { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Marketplace { get; set; }
        public string ImageRole { get; set; }
    }

    public class ProductMeta
    {
        public string SKU { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string Material { get; set; }
        public string Stone { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public string Gender { get; set; }
    }

    public class VisionResult
    {
        public string product_type { get; set; }
        public string target_audience { get; set; }
        public List<string> style_keywords { get; set; }
        public List<string> visible_materials { get; set; }
        public List<string> visible_colors { get; set; }
        public List<string> use_cases { get; set; }
        public string giftable { get; set; }
        public double confidence { get; set; }
        public string summary { get; set; }
    }

    public class KeywordStrategyResult
    {
        public string primary_keyword { get; set; }
        public List<string> secondary_keywords { get; set; }
        public List<string> long_tail_keywords { get; set; }
        public List<string> title_must_include { get; set; }
        public List<string> tags_must_cover { get; set; }
        public List<string> forbidden_claims { get; set; }
        public string tone { get; set; }
        public string buyer_intent { get; set; }
        public string differentiation_angle { get; set; }
        public List<string> seed_keywords_used { get; set; }
    }

    public class ListingResult
    {
        public string title { get; set; }
        public string bullet1 { get; set; }
        public string bullet2 { get; set; }
        public string bullet3 { get; set; }
        public string bullet4 { get; set; }
        public string bullet5 { get; set; }
        public string description { get; set; }
        public string keywords { get; set; }
        public string tags { get; set; }
        public int SeoScore { get; set; }
    }

    public class SavedAiContent
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
        public string VisionSummary { get; set; }
        public string StrategySummary { get; set; }
    }

    public class OpenAiResponseEnvelope
    {
        public string output_text { get; set; }
    }

    public class MultiMarketplaceResult
    {
        public string Marketplace { get; set; }
        public KeywordStrategyResult Strategy { get; set; }
        public ListingResult Listing { get; set; }
    }

    public class SeoBreakdown
    {
        public int TitleScore { get; set; }
        public int BulletsScore { get; set; }
        public int KeywordsScore { get; set; }
        public int TagsScore { get; set; }
    }
}