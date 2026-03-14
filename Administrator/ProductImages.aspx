<%@ Page Title="Product Images" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductImages.aspx.cs" Inherits="Feniks.Administrator.ProductImages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }

    .page-wrap { padding:14px; }

    .card-box {
        background:#fff;
        border:1px solid #e8e8e8;
        border-radius:18px;
        padding:18px;
        margin-bottom:16px;
        box-shadow:0 4px 12px rgba(0,0,0,.03);
    }

    .page-title {
        font-size:28px;
        font-weight:800;
        margin:0 0 4px 0;
        color:#111827;
    }

    .page-subtitle {
        color:#6b7280;
        margin-bottom:18px;
        font-size:13px;
    }

    .toolbar-grid {
        display:grid;
        grid-template-columns:repeat(6, 1fr);
        gap:10px;
        align-items:end;
    }

    .field-label {
        display:block;
        font-size:11px;
        font-weight:800;
        color:#374151;
        margin-bottom:6px;
        text-transform:uppercase;
        letter-spacing:.4px;
    }

    .text-input,
    .select-input,
    .upload-input {
        width:100%;
        border:1px solid #d1d5db;
        border-radius:8px;
        padding:9px 10px;
        font-size:13px;
        background:#fff;
    }

    .dropzone {
        margin-top:12px;
        border:2px dashed #d1d5db;
        border-radius:14px;
        padding:18px;
        text-align:center;
        color:#6b7280;
        background:#fafafa;
    }

    .dropzone strong {
        display:block;
        font-size:16px;
        color:#111827;
        margin-bottom:4px;
    }

    .option-row {
        display:flex;
        flex-wrap:wrap;
        gap:18px;
        margin-top:14px;
        margin-bottom:12px;
    }

    .option-row label {
        font-size:13px;
        color:#374151;
        font-weight:600;
    }

    .btn-row {
        display:flex;
        flex-wrap:wrap;
        gap:8px;
        margin-top:10px;
    }

    .btnx,
    .btn-link-mini {
        display:inline-block;
        padding:9px 14px;
        border-radius:8px;
        border:1px solid transparent;
        font-size:13px;
        font-weight:700;
        text-decoration:none !important;
        cursor:pointer;
    }

    .btn-primaryx { background:#2563eb; color:#fff !important; }
    .btn-successx { background:#16a34a; color:#fff !important; }
    .btn-lightx   { background:#fff; color:#111827 !important; border-color:#d1d5db; }
    .btn-darkx    { background:#111827; color:#fff !important; }
    .btn-dangerx  { background:#dc2626; color:#fff !important; }

    .btnx[disabled],
    .btnx.disabled {
        opacity:.7;
        cursor:not-allowed;
        pointer-events:none;
    }

    .help-note {
        margin-top:10px;
        font-size:12px;
        color:#6b7280;
    }

    .section-title {
        font-size:16px;
        font-weight:800;
        color:#111827;
        margin-bottom:14px;
    }

    .image-grid {
        display:grid;
        grid-template-columns:repeat(auto-fill, minmax(190px, 1fr));
        gap:14px;
    }

    .image-card {
        border:1px solid #dfe3e8;
        border-radius:14px;
        padding:10px;
        background:#fff;
        position:relative;
    }

    .image-card.primary {
        border-color:#22c55e;
        box-shadow:0 0 0 2px rgba(34,197,94,.12);
    }

    .image-thumb {
        width:100%;
        aspect-ratio:1 / 1;
        object-fit:contain;
        background:#fff;
        border-radius:10px;
        border:1px solid #eef0f2;
    }

    .image-name {
        margin-top:10px;
        font-size:12px;
        font-weight:700;
        color:#111827;
        word-break:break-word;
        min-height:34px;
    }

    .badge-row {
        display:flex;
        flex-wrap:wrap;
        gap:6px;
        margin-top:8px;
        margin-bottom:8px;
    }

    .badge-mini {
        display:inline-block;
        border-radius:999px;
        padding:3px 8px;
        font-size:10px;
        font-weight:800;
        background:#eef2ff;
        color:#4338ca;
    }

    .badge-green {
        background:#ecfdf5;
        color:#15803d;
    }

    .badge-gray {
        background:#f3f4f6;
        color:#4b5563;
    }

    .meta-text {
        font-size:11px;
        color:#6b7280;
        margin-bottom:8px;
    }

    .card-actions {
        display:flex;
        gap:6px;
        flex-wrap:wrap;
    }

    .btn-mini {
        border:0;
        border-radius:6px;
        padding:5px 8px;
        font-size:11px;
        font-weight:700;
        cursor:pointer;
        color:#fff;
        text-decoration:none !important;
    }

    .btn-mini-green { background:#22c55e; }
    .btn-mini-red   { background:#ef4444; }
    .btn-mini-dark  { background:#111827; }

    .packs-grid {
        display:grid;
        grid-template-columns:repeat(auto-fill, minmax(320px, 1fr));
        gap:14px;
    }

    .pack-card {
        border:1px solid #e5e7eb;
        border-radius:14px;
        padding:14px;
        background:#fff;
    }

    .pack-title {
        font-size:22px;
        font-weight:800;
        color:#111827;
        margin-bottom:3px;
    }

    .pack-meta {
        font-size:11px;
        color:#6b7280;
        margin-bottom:10px;
    }

    .pack-actions {
        display:flex;
        gap:8px;
        flex-wrap:wrap;
        margin-bottom:12px;
    }

    .pack-item {
        display:flex;
        gap:10px;
        align-items:center;
        margin-bottom:10px;
        padding-bottom:10px;
        border-bottom:1px solid #f1f5f9;
    }

    .pack-thumb {
        width:64px;
        height:64px;
        object-fit:contain;
        background:#fff;
        border:1px solid #e5e7eb;
        border-radius:8px;
        flex:0 0 64px;
    }

    .pack-item-title {
        font-size:12px;
        font-weight:800;
        color:#111827;
        margin-bottom:4px;
    }

    .pack-item-file {
        font-size:11px;
        color:#6b7280;
        margin-bottom:6px;
        word-break:break-word;
    }

    .pack-item-actions {
        display:flex;
        gap:6px;
        flex-wrap:wrap;
    }

    .alert {
        margin-bottom:12px;
        border-radius:10px;
    }

    .page-loader-overlay {
        position:fixed;
        inset:0;
        background:rgba(255,255,255,.78);
        backdrop-filter:blur(2px);
        z-index:99999;
        display:none;
        align-items:center;
        justify-content:center;
    }

    .page-loader-overlay.show {
        display:flex;
    }

    .page-loader-box {
        min-width:320px;
        max-width:90%;
        background:#ffffff;
        border:1px solid #e5e7eb;
        border-radius:18px;
        box-shadow:0 16px 40px rgba(0,0,0,.12);
        padding:26px 28px;
        text-align:center;
    }

    .page-loader-spinner {
        width:54px;
        height:54px;
        margin:0 auto 16px auto;
        border:5px solid #e5e7eb;
        border-top-color:#2563eb;
        border-radius:50%;
        animation:lamaxSpin .9s linear infinite;
    }

    .page-loader-title {
        font-size:20px;
        font-weight:800;
        color:#111827;
        margin-bottom:6px;
    }

    .page-loader-text {
        font-size:13px;
        color:#6b7280;
        line-height:1.5;
    }

    @keyframes lamaxSpin {
        from { transform:rotate(0deg); }
        to { transform:rotate(360deg); }
    }

    @media (max-width: 1200px) {
        .toolbar-grid { grid-template-columns:repeat(3, 1fr); }
    }

    @media (max-width: 768px) {
        .toolbar-grid { grid-template-columns:repeat(1, 1fr); }
    }
</style>

<div class="page-wrap">
    <div class="card-box">
        <div class="page-title">Product Images</div>
        <div class="page-subtitle">
            Upload up to 4 source images, clean the background to white, generate marketplace packs, and download generated files.
        </div>

        <asp:Label ID="lblMsg" runat="server" EnableViewState="false" />

        <div class="toolbar-grid">
            <div>
                <label class="field-label">SKU</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="text-input" />
            </div>

            <div>
                <label class="field-label">Marketplace</label>
                <asp:DropDownList ID="ddlMarketplace" runat="server" CssClass="select-input">
                    <asp:ListItem Text="Default" Value="" />
                    <asp:ListItem Text="Amazon" Value="AMAZON" />
                    <asp:ListItem Text="Etsy" Value="ETSY" />
                    <asp:ListItem Text="eBay" Value="EBAY" />
                    <asp:ListItem Text="Website" Value="WEBSITE" />
                </asp:DropDownList>
            </div>

            <div>
                <label class="field-label">Image Role</label>
                <asp:DropDownList ID="ddlImageRole" runat="server" CssClass="select-input">
                    <asp:ListItem Text="MAIN" Value="MAIN" />
                    <asp:ListItem Text="ANGLE" Value="ANGLE" />
                    <asp:ListItem Text="DETAIL" Value="DETAIL" />
                    <asp:ListItem Text="HAND" Value="HAND" />
                    <asp:ListItem Text="SIDE" Value="SIDE" />
                    <asp:ListItem Text="GALLERY" Value="GALLERY" />
                </asp:DropDownList>
            </div>

            <div>
                <label class="field-label">Preset</label>
                <asp:DropDownList ID="ddlPreset" runat="server" CssClass="select-input">
                    <asp:ListItem Text="Auto" Value="AUTO" />
                    <asp:ListItem Text="Amazon Clean White" Value="AMAZON" />
                    <asp:ListItem Text="Etsy Premium" Value="ETSY" />
                    <asp:ListItem Text="eBay Standard" Value="EBAY" />
                    <asp:ListItem Text="Website Premium" Value="WEBSITE" />
                </asp:DropDownList>
            </div>

            <div>
                <label class="field-label">Start Sort Order</label>
                <asp:TextBox ID="txtSortOrder" runat="server" CssClass="text-input" />
            </div>

            <div>
                <label class="field-label">Image Upload</label>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="upload-input" AllowMultiple="true" />
            </div>
        </div>

        <div class="dropzone">
            <strong>Drop files here or click</strong>
            Upload up to 4 source images. Then let LamaX build Amazon / Etsy / eBay / Website image packs.
        </div>

        <div class="option-row">
            <label><asp:CheckBox ID="chkIsPrimary" runat="server" /> Make first uploaded image primary</label>
            <label><asp:CheckBox ID="chkAutoWhiteBg" runat="server" Checked="true" /> Auto clean light background to white</label>
            <label><asp:CheckBox ID="chkCenterSubject" runat="server" Checked="true" /> Center subject automatically</label>
            <label><asp:CheckBox ID="chkSoftShadow" runat="server" /> Add soft shadow</label>
            <label><asp:CheckBox ID="chkDeleteOldPacks" runat="server" Checked="true" /> Replace existing generated packs</label>
        </div>

        <div class="btn-row">
            <asp:Button ID="btnLoad" runat="server" Text="Load Images" CssClass="btnx btn-lightx"
                OnClick="btnLoad_Click"
                OnClientClick="return showPageLoader('Loading images...', 'Please wait while LamaX loads source images and generated packs.', this);" />

            <asp:Button ID="btnUpload" runat="server" Text="Upload Image(s)" CssClass="btnx btn-primaryx"
                OnClick="btnUpload_Click"
                OnClientClick="return showPageLoader('Uploading images...', 'Your files are being uploaded and optimized. This may take a few seconds.', this);" />

            <asp:Button ID="btnGeneratePacks" runat="server" Text="Generate Listing Packs" CssClass="btnx btn-successx"
                OnClick="btnGeneratePacks_Click"
                OnClientClick="return showPageLoader('Generating listing packs...', 'LamaX is cleaning backgrounds and building Amazon, Etsy, eBay, and Website image sets.', this);" />

            <asp:Button ID="btnLoadPacks" runat="server" Text="Load Packs" CssClass="btnx btn-lightx"
                OnClick="btnLoadPacks_Click"
                OnClientClick="return showPageLoader('Loading packs...', 'Please wait while generated listing packs are loaded.', this);" />

            <asp:HyperLink ID="lnkDownloadAllPacks" runat="server" CssClass="btn-link-mini btn-darkx" Text="Download All Packs ZIP" />
        </div>

        <div class="help-note">
            Source images are improved first. Generated Listing Packs create channel-specific derivative images and store them in database.
        </div>
    </div>

    <div class="card-box">
        <div class="section-title">Source Images</div>

        <asp:Repeater ID="rptImages" runat="server" OnItemCommand="rptImages_ItemCommand">
            <HeaderTemplate>
                <div class="image-grid">
            </HeaderTemplate>
            <ItemTemplate>
                <div class='image-card <%# Convert.ToBoolean(Eval("IsPrimary")) ? "primary" : "" %>'>
                    <img class="image-thumb" src='<%# ResolveUrl("~/Administrator/ProductPhoto.ashx?id=" + Eval("ProductImageID")) %>' alt="" />
                    <div class="image-name"><%# Eval("FileName") %></div>

                    <div class="badge-row">
                        <%# Convert.ToBoolean(Eval("IsPrimary")) ? "<span class=\"badge-mini badge-green\">PRIMARY</span>" : "" %>
                        <span class="badge-mini"><%# Eval("ImageRole") %></span>
                        <span class="badge-mini badge-gray"><%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Marketplace"))) ? "DEFAULT" : Eval("Marketplace").ToString() %></span>
                    </div>

                    <div class="meta-text">
                        Sort: <%# Eval("SortOrder") %><br />
                        ID: <%# Eval("ProductImageID") %>
                    </div>

                    <div class="card-actions">
                        <asp:Button ID="btnPrimary" runat="server" Text="Make Primary" CommandName="makeprimary" CommandArgument='<%# Eval("ProductImageID") %>' CssClass="btn-mini btn-mini-green" />
                        <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="deleteimg" CommandArgument='<%# Eval("ProductImageID") %>' CssClass="btn-mini btn-mini-red" OnClientClick="return confirm('Delete this image?');" />
                        <a class="btn-mini btn-mini-dark" href='<%# ResolveUrl("~/Administrator/ProductImageDownload.ashx?mode=single&id=" + Eval("ProductImageID")) %>' target="_blank">Download</a>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>

    <div class="card-box">
        <div class="section-title">Generated Listing Packs</div>
        <asp:Literal ID="litPacks" runat="server" />
    </div>
</div>

<div id="pageLoader" class="page-loader-overlay">
    <div class="page-loader-box">
        <div class="page-loader-spinner"></div>
        <div id="loaderTitle" class="page-loader-title">Processing images...</div>
        <div id="loaderText" class="page-loader-text">
            Please wait. LamaX is cleaning backgrounds, optimizing images, and preparing your listing pack.
        </div>
    </div>
</div>

<script type="text/javascript">
    function showPageLoader(title, text, clickedButton) {
        try {
            var overlay = document.getElementById("pageLoader");
            var loaderTitle = document.getElementById("loaderTitle");
            var loaderText = document.getElementById("loaderText");

            if (loaderTitle && title) loaderTitle.innerText = title;
            if (loaderText && text) loaderText.innerText = text;

            if (overlay) overlay.classList.add("show");

            var buttons = document.querySelectorAll(".btnx");
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].setAttribute("disabled", "disabled");
                buttons[i].classList.add("disabled");
            }

            if (clickedButton) {
                if (clickedButton.value !== undefined) clickedButton.value = "Please wait...";
                if (clickedButton.innerText !== undefined) clickedButton.innerText = "Please wait...";
            }
        } catch (e) { }

        return true;
    }

    function hidePageLoader() {
        try {
            var overlay = document.getElementById("pageLoader");
            if (overlay) overlay.classList.remove("show");

            var buttons = document.querySelectorAll(".btnx");
            for (var i = 0; i < buttons.length; i++) {
                buttons[i].removeAttribute("disabled");
                buttons[i].classList.remove("disabled");
            }
        } catch (e) { }
    }

    window.addEventListener("pageshow", function () {
        hidePageLoader();
    });
</script>

</asp:Content>