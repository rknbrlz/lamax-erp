<%@ Page Title="Product AI" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ProductAI.aspx.cs" Inherits="Feniks.Administrator.ProductAI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }

    .page-wrap { padding:14px; }

    .card-box {
        background:#fff;
        border:1px solid #e8e8e8;
        border-radius:18px;
        padding:22px;
        margin-bottom:18px;
        box-shadow:0 4px 12px rgba(0,0,0,.03);
    }

    .page-title {
        font-size:24px;
        font-weight:800;
        margin:0 0 6px 0;
        color:#1f2937;
    }

    .page-subtitle {
        color:#6b7280;
        margin-bottom:18px;
    }

    .lbl {
        display:block;
        font-weight:700;
        margin-bottom:6px;
        color:#111827;
    }

    .form-control {
        border-radius:8px;
        height:42px;
    }

    textarea.form-control {
        height:auto;
        min-height:90px;
    }

    .top-actions {
        display:flex;
        gap:10px;
        align-items:end;
        flex-wrap:wrap;
        margin-top:26px;
    }

    .score-badge {
        width:104px;
        height:104px;
        border-radius:50%;
        background:linear-gradient(180deg,#f8fbff 0%,#eef4ff 100%);
        display:flex;
        align-items:center;
        justify-content:center;
        font-weight:800;
        font-size:30px;
        color:#111827;
        border:1px solid #dbe4ff;
        box-shadow:0 10px 24px rgba(37,99,235,.10);
        margin:0 auto 14px auto;
    }

    .mini-box {
        background:#f8fafc;
        border:1px dashed #d1d5db;
        border-radius:12px;
        padding:14px;
        min-height:240px;
    }

    .status-ok {
        background:#dff0d8;
        color:#3c763d;
        border:1px solid #c7e3be;
        padding:12px 14px;
        border-radius:8px;
        display:inline-block;
        margin-top:8px;
    }

    .status-err {
        background:#f2dede;
        color:#a94442;
        border:1px solid #ebcccc;
        padding:12px 14px;
        border-radius:8px;
        display:inline-block;
        margin-top:8px;
    }

    .section-label {
        font-weight:700;
        margin-bottom:8px;
        color:#111827;
    }

    .vision-line,
    .strategy-line {
        margin:4px 0;
        color:#374151;
        line-height:1.5;
    }

    .preview-wrap {
        display:flex;
        align-items:center;
        justify-content:center;
        min-height:190px;
    }

    .product-preview {
        max-width:100%;
        max-height:180px;
        border-radius:12px;
        border:1px solid #e5e7eb;
        box-shadow:0 2px 8px rgba(0,0,0,.05);
        background:#fff;
        padding:6px;
    }

    .preview-empty {
        color:#6b7280;
        font-size:13px;
        text-align:center;
        padding-top:12px;
    }

    .preview-meta {
        margin-top:12px;
        display:flex;
        flex-direction:column;
        gap:6px;
    }

    .meta-pill {
        display:inline-block;
        padding:6px 10px;
        border-radius:999px;
        background:#eef2ff;
        border:1px solid #dbe4ff;
        color:#374151;
        font-size:12px;
        font-weight:600;
        margin-right:6px;
        margin-bottom:6px;
    }

    .seo-box {
        display:flex;
        align-items:center;
        justify-content:center;
        min-height:240px;
    }

    .seo-card {
        width:100%;
        min-height:210px;
        background:#ffffff;
        border:1px solid #e5e7eb;
        border-radius:16px;
        padding:18px 16px;
        display:flex;
        flex-direction:column;
        align-items:center;
        justify-content:center;
        text-align:center;
        box-shadow:0 8px 20px rgba(15,23,42,.04);
    }

    .seo-level {
        font-size:14px;
        font-weight:700;
        color:#1f2937;
        margin-bottom:6px;
    }

    .seo-note {
        color:#6b7280;
        font-size:12px;
        line-height:1.5;
        max-width:220px;
    }

    .seo-mini-stats {
        margin-top:10px;
        display:flex;
        flex-wrap:wrap;
        justify-content:center;
        gap:8px;
    }

    .seo-pill {
        display:inline-block;
        padding:6px 10px;
        border-radius:999px;
        background:#f3f4f6;
        border:1px solid #e5e7eb;
        color:#374151;
        font-size:11px;
        font-weight:700;
    }

    .strategy-box {
        background:#f8fafc;
        border:1px dashed #d1d5db;
        border-radius:12px;
        padding:14px;
        min-height:180px;
    }

    .multi-wrap {
        margin-top:10px;
    }

    .market-tabs {
        display:flex;
        flex-wrap:wrap;
        gap:8px;
        margin-bottom:14px;
    }

    .market-tab {
        display:inline-block;
        padding:8px 14px;
        border-radius:999px;
        background:#eef2ff;
        border:1px solid #dbe4ff;
        color:#1f2937;
        font-weight:700;
        cursor:pointer;
        user-select:none;
    }

    .market-tab.active {
        background:#2563eb;
        border-color:#2563eb;
        color:#fff;
    }

    .market-panel {
        display:none;
        border:1px solid #e5e7eb;
        border-radius:14px;
        padding:16px;
        background:#fcfcfd;
        margin-bottom:14px;
    }

    .market-panel.active {
        display:block;
    }

    .market-panel h4 {
        margin-top:0;
        margin-bottom:12px;
        font-weight:800;
        color:#111827;
    }

    .market-grid {
        display:grid;
        grid-template-columns:1fr 1fr;
        gap:14px;
    }

    .market-grid-full {
        grid-column:1 / -1;
    }

    .market-field label {
        display:block;
        font-weight:700;
        margin-bottom:6px;
    }

    .market-value {
        background:#fff;
        border:1px solid #e5e7eb;
        border-radius:10px;
        padding:10px 12px;
        min-height:42px;
        white-space:pre-wrap;
        color:#1f2937;
    }

    .market-value.long {
        min-height:130px;
    }

    .multi-actions {
        margin-top:14px;
        display:flex;
        gap:10px;
        flex-wrap:wrap;
    }

    .market-kpi {
        display:inline-block;
        padding:6px 10px;
        border-radius:999px;
        background:#ecfeff;
        border:1px solid #bae6fd;
        color:#0f172a;
        font-size:12px;
        font-weight:700;
        margin-right:8px;
        margin-bottom:8px;
    }

    .seo-radar-card {
        width:100%;
        margin-top:14px;
        background:#0f172a;
        border-radius:18px;
        padding:16px 16px 12px 16px;
        color:#fff;
        box-shadow:0 8px 20px rgba(15,23,42,.10);
    }

    .seo-radar-title {
        font-size:15px;
        font-weight:800;
        margin-bottom:12px;
        color:#fff;
    }

    .seo-radar-row {
        display:grid;
        grid-template-columns:72px 1fr 46px;
        gap:10px;
        align-items:center;
        margin-bottom:10px;
    }

    .seo-radar-label {
        font-size:13px;
        font-weight:700;
        color:#e5e7eb;
    }

    .seo-radar-track {
        position:relative;
        height:14px;
        background:rgba(255,255,255,.12);
        border-radius:999px;
        overflow:hidden;
        border:1px solid rgba(255,255,255,.08);
    }

    .seo-radar-fill {
        display:block;
        height:100%;
        width:0%;
        border-radius:999px;
        transition:width .35s ease;
    }

    .seo-radar-fill.low {
        background:linear-gradient(90deg,#ef4444 0%,#f87171 100%);
    }

    .seo-radar-fill.medium {
        background:linear-gradient(90deg,#f59e0b 0%,#fbbf24 100%);
    }

    .seo-radar-fill.high {
        background:linear-gradient(90deg,#22c55e 0%,#4ade80 100%);
    }

    .seo-radar-value {
        font-size:12px;
        font-weight:800;
        color:#f9fafb;
        text-align:right;
    }

    .ai-loading-overlay {
        position:fixed;
        inset:0;
        background:rgba(15,23,42,.28);
        z-index:99999;
        display:none;
        align-items:center;
        justify-content:center;
        backdrop-filter:blur(2px);
    }

    .ai-loading-card {
        width:340px;
        max-width:92vw;
        background:#fff;
        border-radius:18px;
        padding:26px 22px;
        text-align:center;
        box-shadow:0 24px 48px rgba(0,0,0,.18);
        border:1px solid #e5e7eb;
    }

    .ai-spinner {
        width:52px;
        height:52px;
        margin:0 auto 14px auto;
        border-radius:50%;
        border:4px solid #dbeafe;
        border-top-color:#2563eb;
        animation:spinAi 0.9s linear infinite;
    }

    .ai-loading-title {
        font-size:18px;
        font-weight:800;
        color:#111827;
        margin-bottom:6px;
    }

    .ai-loading-text {
        color:#6b7280;
        font-size:13px;
        line-height:1.5;
    }

    @keyframes spinAi {
        to { transform:rotate(360deg); }
    }

    @media (max-width: 991px) {
        .top-actions { margin-top:12px; }
        .market-grid { grid-template-columns:1fr; }
    }
</style>

<script type="text/javascript">
    function showMarketPanel(key) {
        var tabs = document.querySelectorAll('.market-tab');
        var panels = document.querySelectorAll('.market-panel');

        for (var i = 0; i < tabs.length; i++) tabs[i].classList.remove('active');
        for (var j = 0; j < panels.length; j++) panels[j].classList.remove('active');

        var tab = document.getElementById('tab_' + key);
        var panel = document.getElementById('panel_' + key);

        if (tab) tab.classList.add('active');
        if (panel) panel.classList.add('active');
    }

    function startAiLoading(message) {
        var overlay = document.getElementById('aiLoadingOverlay');
        var text = document.getElementById('aiLoadingText');

        if (text && message) text.innerHTML = message;
        if (overlay) overlay.style.display = 'flex';

        return true;
    }

    function stopAiLoading() {
        var overlay = document.getElementById('aiLoadingOverlay');
        if (overlay) overlay.style.display = 'none';
    }

    window.addEventListener('pageshow', function () {
        stopAiLoading();
    });
</script>

<div id="aiLoadingOverlay" class="ai-loading-overlay">
    <div class="ai-loading-card">
        <div class="ai-spinner"></div>
        <div class="ai-loading-title">Please wait</div>
        <div id="aiLoadingText" class="ai-loading-text">AI içerik hazırlanıyor...</div>
    </div>
</div>

<div class="page-wrap">

    <div class="card-box">
        <div class="page-title">AI Listing Generator</div>
        <div class="page-subtitle">SKU bazlı Amazon / Etsy / eBay / Website listing üretimi (foto destekli)</div>

        <div class="row">
            <div class="col-md-3">
                <label class="lbl">SKU</label>
                <asp:TextBox ID="txtSku" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-3">
                <label class="lbl">Marketplace</label>
                <asp:DropDownList ID="ddlMarketplace" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Amazon" Value="Amazon" />
                    <asp:ListItem Text="Etsy" Value="Etsy" />
                    <asp:ListItem Text="eBay" Value="eBay" />
                    <asp:ListItem Text="Website" Value="Website" />
                </asp:DropDownList>
            </div>

            <div class="col-md-2">
                <label class="lbl">Language</label>
                <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="form-control">
                    <asp:ListItem Text="English" Value="EN" />
                    <asp:ListItem Text="Turkish" Value="TR" />
                    <asp:ListItem Text="Polish" Value="PL" />
                </asp:DropDownList>
            </div>

            <div class="col-md-4">
                <div class="top-actions">
                    <asp:Button ID="btnLoadSaved" runat="server" Text="Load Saved"
                        CssClass="btn btn-default"
                        OnClick="btnLoadSaved_Click"
                        OnClientClick="return startAiLoading('Kayıtlı içerik yükleniyor...');" />

                    <asp:Button ID="btnGenerate" runat="server" Text="Generate with AI"
                        CssClass="btn btn-primary"
                        OnClick="btnGenerate_Click"
                        OnClientClick="return startAiLoading('AI tek marketplace için içerik üretiyor...');" />

                    <asp:Button ID="btnGenerateAll" runat="server" Text="Generate All Marketplaces"
                        CssClass="btn btn-info"
                        OnClick="btnGenerateAll_Click"
                        OnClientClick="return startAiLoading('Tüm marketplace içerikleri üretiliyor... Bu işlem biraz sürebilir.');" />

                    <asp:Button ID="btnForceRefresh" runat="server" Text="Force Refresh"
                        CssClass="btn btn-warning"
                        OnClick="btnForceRefresh_Click"
                        OnClientClick="return startAiLoading('Görsel analiz cache bypass edilerek yeniden üretiliyor...');" />

                    <asp:Button ID="btnSave" runat="server" Text="Save"
                        CssClass="btn btn-success"
                        OnClick="btnSave_Click"
                        OnClientClick="return startAiLoading('Geçerli marketplace içeriği kaydediliyor...');" />

                    <asp:Button ID="btnSaveAllMarketplaces" runat="server" Text="Save All to DB"
                        CssClass="btn btn-success"
                        OnClick="btnSaveAllMarketplaces_Click"
                        OnClientClick="return startAiLoading('Tüm marketplace sonuçları veritabanına kaydediliyor...');" />
                </div>
            </div>
        </div>

        <div style="margin-top:8px;">
            <asp:Literal ID="litStatus" runat="server" />
        </div>
    </div>

    <div class="card-box">
        <div class="row">
            <div class="col-md-3">
                <div class="section-label">Product Preview</div>
                <div class="mini-box">
                    <div class="preview-wrap">
                        <asp:Image ID="imgProductPreview" runat="server" CssClass="product-preview" Style="display:none;" />
                    </div>

                    <div class="preview-empty">
                        <asp:Literal ID="litNoImage" runat="server" Text="Görsel henüz yüklenmedi." />
                    </div>

                    <div class="preview-meta">
                        <asp:Literal ID="litPreviewMeta" runat="server" />
                    </div>
                </div>
            </div>

            <div class="col-md-6">
                <div class="section-label">AI Image Understanding</div>
                <div class="mini-box">
                    <asp:Literal ID="litVisionSummary" runat="server" Text="Henüz analiz yapılmadı." />
                </div>
            </div>

            <div class="col-md-3">
                <div class="section-label">SEO Score</div>
                <div class="mini-box seo-box">
                    <div class="seo-card">
                        <div class="score-badge">
                            <asp:Literal ID="litSeoScore" runat="server" Text="-" />
                        </div>

                        <div class="seo-level">
                            <asp:Literal ID="litSeoLevel" runat="server" Text="No score yet" />
                        </div>

                        <div class="seo-note">
                            <asp:Literal ID="litSeoHint" runat="server" Text="AI çıktısına göre hesaplanır." />
                        </div>

                        <div class="seo-mini-stats">
                            <span class="seo-pill">Title</span>
                            <span class="seo-pill">Bullets</span>
                            <span class="seo-pill">Keywords</span>
                            <span class="seo-pill">Tags</span>
                        </div>
                    </div>
                </div>

                <div class="seo-radar-card">
                    <div class="seo-radar-title">SEO Radar</div>

                    <div class="seo-radar-row">
                        <div class="seo-radar-label">Title</div>
                        <div class="seo-radar-track">
                            <div id="radarTitleBar" runat="server" class="seo-radar-fill"></div>
                        </div>
                        <div class="seo-radar-value">
                            <asp:Literal ID="litRadarTitleText" runat="server" Text="0%" />
                        </div>
                    </div>

                    <div class="seo-radar-row">
                        <div class="seo-radar-label">Bullets</div>
                        <div class="seo-radar-track">
                            <div id="radarBulletsBar" runat="server" class="seo-radar-fill"></div>
                        </div>
                        <div class="seo-radar-value">
                            <asp:Literal ID="litRadarBulletsText" runat="server" Text="0%" />
                        </div>
                    </div>

                    <div class="seo-radar-row">
                        <div class="seo-radar-label">Keywords</div>
                        <div class="seo-radar-track">
                            <div id="radarKeywordsBar" runat="server" class="seo-radar-fill"></div>
                        </div>
                        <div class="seo-radar-value">
                            <asp:Literal ID="litRadarKeywordsText" runat="server" Text="0%" />
                        </div>
                    </div>

                    <div class="seo-radar-row" style="margin-bottom:0;">
                        <div class="seo-radar-label">Tags</div>
                        <div class="seo-radar-track">
                            <div id="radarTagsBar" runat="server" class="seo-radar-fill"></div>
                        </div>
                        <div class="seo-radar-value">
                            <asp:Literal ID="litRadarTagsText" runat="server" Text="0%" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="card-box">
        <div class="section-label">AI Keyword Strategy</div>
        <div class="strategy-box">
            <asp:Literal ID="litStrategySummary" runat="server" Text="Henüz strategy üretilmedi." />
        </div>
    </div>

    <div class="card-box">
        <div class="row">
            <div class="col-md-12">
                <label class="lbl">Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="row" style="margin-top:14px;">
            <div class="col-md-6">
                <label class="lbl">Bullet 1</label>
                <asp:TextBox ID="txtBullet1" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label class="lbl">Bullet 2</label>
                <asp:TextBox ID="txtBullet2" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="row" style="margin-top:14px;">
            <div class="col-md-6">
                <label class="lbl">Bullet 3</label>
                <asp:TextBox ID="txtBullet3" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-6">
                <label class="lbl">Bullet 4</label>
                <asp:TextBox ID="txtBullet4" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="row" style="margin-top:14px;">
            <div class="col-md-12">
                <label class="lbl">Bullet 5</label>
                <asp:TextBox ID="txtBullet5" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="row" style="margin-top:14px;">
            <div class="col-md-12">
                <label class="lbl">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control"
                    TextMode="MultiLine" Rows="8" />
            </div>
        </div>

        <div class="row" style="margin-top:14px;">
            <div class="col-md-6">
                <label class="lbl">Keywords</label>
                <asp:TextBox ID="txtKeywords" runat="server" CssClass="form-control"
                    TextMode="MultiLine" Rows="4" />
            </div>
            <div class="col-md-6">
                <label class="lbl">Tags</label>
                <asp:TextBox ID="txtTags" runat="server" CssClass="form-control"
                    TextMode="MultiLine" Rows="4" />
            </div>
        </div>
    </div>

    <div class="card-box">
        <div class="section-label">Multi Marketplace Results</div>
        <div class="multi-wrap">
            <asp:Literal ID="litMultiMarketplace" runat="server" Text="Henüz çoklu marketplace üretimi yapılmadı." />
        </div>
    </div>

</div>

</asp:Content>