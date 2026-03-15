<%@ Page Title="Amazon Main Photo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductMainPhoto.aspx.cs" Inherits="Feniks.Administrator.ProductMainPhoto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }

    .page-wrap { padding:14px; }

    .card-box {
        background:#fff;
        border:1px solid #e8e8e8;
        border-radius:18px;
        padding:20px;
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
        grid-template-columns: 1.2fr 1fr;
        gap:12px;
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
    .upload-input {
        width:100%;
        border:1px solid #d1d5db;
        border-radius:8px;
        padding:9px 10px;
        font-size:13px;
        background:#fff;
    }

    .option-row {
        display:flex;
        flex-wrap:wrap;
        gap:18px;
        margin-top:16px;
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

    .btnx {
        display:inline-block;
        padding:9px 14px;
        border-radius:8px;
        border:1px solid transparent;
        font-size:13px;
        font-weight:700;
        text-decoration:none;
        cursor:pointer;
    }

    .btn-primaryx { background:#2563eb; color:#fff; }
    .btn-lightx   { background:#fff; color:#111827; border-color:#d1d5db; }
    .btn-dangerx  { background:#dc2626; color:#fff; }
    .btn-darkx    { background:#111827; color:#fff; }

    .btnx[disabled],
    .btnx.disabled {
        opacity:.7;
        cursor:not-allowed;
        pointer-events:none;
    }

    .preview-wrap {
        display:flex;
        gap:20px;
        align-items:flex-start;
        flex-wrap:wrap;
    }

    .preview-box {
        width:320px;
    }

    .preview-image {
        width:100%;
        aspect-ratio:1 / 1;
        object-fit:contain;
        background:#fff;
        border:1px solid #e5e7eb;
        border-radius:16px;
        padding:10px;
    }

    .preview-meta {
        margin-top:10px;
        font-size:13px;
        color:#6b7280;
        line-height:1.5;
    }

    .empty-box {
        border:2px dashed #d1d5db;
        border-radius:16px;
        padding:40px 20px;
        text-align:center;
        color:#9ca3af;
        background:#fafafa;
        width:320px;
    }

    .alert {
        margin-bottom:12px;
        border-radius:10px;
    }

    .help-box {
        margin-top:12px;
        padding:12px 14px;
        border-radius:12px;
        background:#f8fafc;
        border:1px solid #e5e7eb;
        color:#475569;
        font-size:12px;
        line-height:1.6;
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

    @media (max-width: 768px) {
        .toolbar-grid { grid-template-columns:1fr; }
    }
</style>

<div class="page-wrap">
    <div class="card-box">
        <div class="page-title">Amazon Main Photo</div>
        <div class="page-subtitle">
            Upload one image for a SKU. LamaX will save one Amazon-style main image directly on the product record.
        </div>

        <asp:Label ID="lblMsg" runat="server" EnableViewState="false" />

        <div class="toolbar-grid">
            <div>
                <label class="field-label">SKU</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="text-input" />
            </div>
            <div>
                <label class="field-label">Image Upload</label>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="upload-input" />
            </div>
        </div>

        <div class="option-row">
            <label><asp:CheckBox ID="chkAutoWhiteBg" runat="server" Checked="true" /> Force clean white background</label>
            <label><asp:CheckBox ID="chkCenterSubject" runat="server" Checked="true" /> Center subject</label>
            <label><asp:CheckBox ID="chkUseShadow" runat="server" /> Add soft shadow</label>
        </div>

        <div class="btn-row">
            <asp:Button ID="btnLoad" runat="server" Text="Load Photo" CssClass="btnx btn-lightx"
                OnClick="btnLoad_Click"
                OnClientClick="return showPageLoader('Loading photo...', 'Please wait while LamaX loads the saved photo.', this);" />

            <asp:Button ID="btnUpload" runat="server" Text="Upload Photo" CssClass="btnx btn-primaryx"
                OnClick="btnUpload_Click"
                OnClientClick="return showPageLoader('Uploading photo...', 'Please wait while LamaX saves the image.', this);" />

            <asp:Button ID="btnDelete" runat="server" Text="Delete Photo" CssClass="btnx btn-dangerx"
                OnClick="btnDelete_Click"
                OnClientClick="return confirm('Delete the saved main photo for this SKU?');" />

            <asp:HyperLink ID="lnkPreview" runat="server" CssClass="btnx btn-darkx" Text="Open Photo" Target="_blank" />
        </div>

        <div class="help-box">
            This version writes directly to T_Product.Photo and uses the existing ProductPhoto handler for preview.
        </div>
    </div>

    <div class="card-box">
        <div class="page-title" style="font-size:22px;">Saved Photo Preview</div>
        <div class="preview-wrap">
            <asp:Literal ID="litPreview" runat="server" />
        </div>
    </div>
</div>

<div id="pageLoader" class="page-loader-overlay">
    <div class="page-loader-box">
        <div class="page-loader-spinner"></div>
        <div id="loaderTitle" class="page-loader-title">Processing...</div>
        <div id="loaderText" class="page-loader-text">Please wait.</div>
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