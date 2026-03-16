<%@ Page Title="Amazon Main Image" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ProductAmazonMainImage.aspx.cs"
    Inherits="Feniks.Administrator.ProductAmazonMainImage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body {
        background: #f4f6f8;
    }

    .page-wrap {
        padding: 18px;
    }

    .page-card {
        background: #fff;
        border: 1px solid #e7eaee;
        border-radius: 18px;
        padding: 24px;
        box-shadow: 0 8px 24px rgba(15, 23, 42, .04);
    }

    .page-title {
        font-size: 22px;
        font-weight: 800;
        color: #172033;
        margin: 0 0 6px 0;
    }

    .page-subtitle {
        color: #667085;
        margin-bottom: 24px;
    }

    .grid-main {
        display: grid;
        grid-template-columns: 1.05fr 1fr;
        gap: 26px;
        align-items: start;
    }

    .section-title {
        font-size: 16px;
        font-weight: 700;
        color: #1f2937;
        margin: 0 0 14px 0;
    }

    .field {
        margin-bottom: 18px;
    }

    .field label {
        display: block;
        font-weight: 700;
        color: #1f2937;
        margin-bottom: 8px;
    }

    .field input[type=text],
    .field .form-control {
        width: 100%;
        height: 44px;
        border: 1px solid #d8dee6;
        border-radius: 12px;
        padding: 10px 14px;
        outline: none;
        box-shadow: none;
        transition: border-color .2s ease, box-shadow .2s ease;
        background: #fff;
    }

    .field input[type=text]:focus,
    .field .form-control:focus {
        border-color: #b8c2d1;
        box-shadow: 0 0 0 3px rgba(15, 23, 42, .04);
    }

    .help-text {
        margin-top: 8px;
        font-size: 12px;
        color: #667085;
        line-height: 1.5;
    }

    .row-2 {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 14px;
    }

    .action-bar {
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
        margin-top: 8px;
    }

    .btn-main,
    .btn-sub,
    .btn-back {
        min-height: 42px;
        padding: 0 16px;
        border-radius: 12px;
        font-weight: 700;
        border: 0;
        transition: all .2s ease;
    }

    .btn-main {
        background: #0f172a;
        color: #fff;
    }

    .btn-main:hover {
        opacity: .92;
    }

    .btn-sub {
        background: #eef2f6;
        color: #172033;
    }

    .btn-sub:hover {
        background: #e7ecf2;
    }

    .btn-back {
        background: #f4f6f8;
        color: #344054;
    }

    .btn-back:hover {
        background: #eceff3;
    }

    .message-box {
        margin-top: 14px;
    }

    .msg-success,
    .msg-error {
        display: block;
        border-radius: 12px;
        padding: 12px 14px;
        font-size: 13px;
        font-weight: 600;
    }

    .msg-success {
        background: #ecfdf3;
        color: #027a48;
        border: 1px solid #abefc6;
    }

    .msg-error {
        background: #fef3f2;
        color: #b42318;
        border: 1px solid #fecdca;
    }

    .preview-title {
        font-size: 16px;
        font-weight: 700;
        color: #1f2937;
        margin: 0 0 14px 0;
    }

    .preview-box {
        min-height: 380px;
        border: 1px dashed #d7dde5;
        border-radius: 18px;
        background: #fafbfc;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 18px;
        position: relative;
        overflow: hidden;
    }

    .preview-box img {
        max-width: 100%;
        max-height: 520px;
        border-radius: 12px;
        box-shadow: 0 8px 22px rgba(15, 23, 42, .08);
        background: #fff;
    }

    .preview-empty {
        color: #98a2b3;
        font-size: 15px;
    }

    .meta-box {
        margin-top: 16px;
        border: 1px solid #e7eaee;
        border-radius: 14px;
        background: #fff;
        padding: 14px 16px;
    }

    .meta-row {
        display: grid;
        grid-template-columns: 130px 1fr;
        gap: 10px;
        padding: 6px 0;
        border-bottom: 1px solid #f1f3f5;
    }

    .meta-row:last-child {
        border-bottom: 0;
        padding-bottom: 0;
    }

    .meta-key {
        font-weight: 700;
        color: #344054;
    }

    .meta-value {
        color: #475467;
        word-break: break-word;
    }

    .cost-note {
        margin-top: 16px;
        border: 1px solid #d9e6ff;
        background: #f5f9ff;
        border-radius: 14px;
        padding: 12px 14px;
        color: #1849a9;
        font-size: 12px;
        line-height: 1.55;
    }

    .loading-overlay {
        position: fixed;
        inset: 0;
        background: rgba(15, 23, 42, 0.22);
        backdrop-filter: blur(2px);
        z-index: 99999;
        display: none;
        align-items: center;
        justify-content: center;
        padding: 20px;
    }

    .loading-card {
        width: 100%;
        max-width: 420px;
        background: #ffffff;
        border: 1px solid #e7eaee;
        border-radius: 22px;
        box-shadow: 0 20px 60px rgba(15, 23, 42, .18);
        padding: 28px 24px;
        text-align: center;
    }

    .loading-spinner {
        width: 62px;
        height: 62px;
        margin: 0 auto 18px auto;
        border-radius: 50%;
        border: 5px solid #e9eef5;
        border-top-color: #0f172a;
        animation: lamaxSpin 0.9s linear infinite;
    }

    .loading-title {
        font-size: 20px;
        font-weight: 800;
        color: #172033;
        margin-bottom: 8px;
    }

    .loading-text {
        font-size: 14px;
        color: #667085;
        line-height: 1.6;
        margin-bottom: 16px;
    }

    .loading-steps {
        text-align: left;
        background: #f8fafc;
        border: 1px solid #edf2f7;
        border-radius: 14px;
        padding: 14px 16px;
    }

    .loading-step {
        font-size: 13px;
        color: #475467;
        padding: 5px 0;
    }

    .loading-step strong {
        color: #111827;
    }

    @keyframes lamaxSpin {
        to {
            transform: rotate(360deg);
        }
    }

    @media (max-width: 1100px) {
        .grid-main {
            grid-template-columns: 1fr;
        }
    }

    @media (max-width: 640px) {
        .row-2 {
            grid-template-columns: 1fr;
        }

        .page-wrap {
            padding: 12px;
        }

        .page-card {
            padding: 16px;
            border-radius: 14px;
        }
    }
</style>

<script type="text/javascript">
    var lamaxProcessingStarted = false;

    function showLamaXLoader() {
        var overlay = document.getElementById('lamaxLoadingOverlay');
        if (overlay) {
            overlay.style.display = 'flex';
        }
    }

    function hideLamaXLoader() {
        var overlay = document.getElementById('lamaxLoadingOverlay');
        if (overlay) {
            overlay.style.display = 'none';
        }
    }

    function startAmazonImageProcessing() {
        if (lamaxProcessingStarted) {
            return false;
        }

        if (typeof (Page_ClientValidate) === 'function') {
            if (!Page_ClientValidate()) {
                return false;
            }
        }

        lamaxProcessingStarted = true;
        showLamaXLoader();

        var btn = document.getElementById('<%= btnSave.ClientID %>');
        if (btn) {
            btn.disabled = true;
            if (btn.value !== undefined) btn.value = 'Processing...';
            if (btn.innerText !== undefined) btn.innerText = 'Processing...';
        }

        window.setTimeout(function () {
            __doPostBack('<%= btnSave.UniqueID %>', '');
        }, 150);

        return false;
    }

    function previewSelectedFile(input) {
        var file = input.files && input.files[0];
        if (!file) return;

        var reader = new FileReader();
        reader.onload = function (e) {
            var img = document.getElementById('<%= imgPreview.ClientID %>');
            var empty = document.getElementById('<%= lblPreviewEmpty.ClientID %>');

            if (img) {
                img.src = e.target.result;
                img.style.display = 'block';
            }

            if (empty) {
                empty.style.display = 'none';
            }
        };
        reader.readAsDataURL(file);
    }

    window.addEventListener('pageshow', function () {
        hideLamaXLoader();
        lamaxProcessingStarted = false;
    });

    window.addEventListener('load', function () {
        hideLamaXLoader();
        lamaxProcessingStarted = false;
    });
</script>

<div class="page-wrap">
    <div class="page-card">
        <h1 class="page-title">Amazon Main Image Upload</h1>
        <div class="page-subtitle">
            Upload one product photo, clean it economically, convert it to Amazon-ready square format, and save it to database.
        </div>

        <div class="grid-main">
            <div>
                <div class="section-title">Image Settings</div>

                <div class="field">
                    <label for="<%= txtSKU.ClientID %>">SKU</label>
                    <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" placeholder="Enter SKU" />
                    <div class="help-text">The system checks your SKU in T_Product before saving.</div>
                </div>

                <div class="field">
                    <label for="<%= fuImage.ClientID %>">Product Photo</label>
                    <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" onchange="previewSelectedFile(this);" />
                    <div class="help-text">
                        Supported: JPG, JPEG, PNG, WEBP, BMP. Final output will be saved as square JPEG.
                    </div>
                </div>

                <div class="row-2">
                    <div class="field">
                        <label for="<%= txtCanvasSize.ClientID %>">Canvas Size</label>
                        <asp:TextBox ID="txtCanvasSize" runat="server" CssClass="form-control" Text="2000" />
                    </div>

                    <div class="field">
                        <label for="<%= txtFillRatio.ClientID %>">Product Fill Ratio (%)</label>
                        <asp:TextBox ID="txtFillRatio" runat="server" CssClass="form-control" Text="85" />
                    </div>
                </div>

                <div class="help-text">
                    This version uses a lower-cost image edit flow and blocks accidental duplicate processing for the same SKU and same file.
                </div>

                <div class="action-bar">
                <asp:Button ID="btnSave" runat="server" Text="Convert & Save"
                    CssClass="btn-main"
                    OnClick="btnSave_Click"
                    UseSubmitBehavior="false"
                    OnClientClick="return startAmazonImageProcessing();" />

                    <asp:Button ID="btnLoadCurrent" runat="server" Text="Load Current"
                        CssClass="btn-sub"
                        OnClick="btnLoadCurrent_Click" />

                    <asp:Button ID="btnBack" runat="server" Text="Back"
                        CssClass="btn-back"
                        CausesValidation="false"
                        OnClick="btnBack_Click" />
                </div>

                <div class="message-box">
                    <asp:Label ID="lblMessage" runat="server" />
                </div>

                <div class="cost-note">
                    Optimized for lower image processing cost while keeping the Amazon-ready output flow.
                </div>
            </div>

            <div>
                <div class="preview-title">Preview</div>

                <div class="preview-box">
                    <asp:Image ID="imgPreview" runat="server" Style="display:none;" />
                    <asp:Label ID="lblPreviewEmpty" runat="server" CssClass="preview-empty" Text="No preview yet." />
                </div>

                <asp:Panel ID="pnlMeta" runat="server" Visible="false" CssClass="meta-box">
                    <div class="meta-row">
                        <div class="meta-key">SKU</div>
                        <div class="meta-value"><asp:Literal ID="litSku" runat="server" /></div>
                    </div>
                    <div class="meta-row">
                        <div class="meta-key">Original File</div>
                        <div class="meta-value"><asp:Literal ID="litOriginalFile" runat="server" /></div>
                    </div>
                    <div class="meta-row">
                        <div class="meta-key">Output</div>
                        <div class="meta-value"><asp:Literal ID="litOutputInfo" runat="server" /></div>
                    </div>
                    <div class="meta-row">
                        <div class="meta-key">Saved Image</div>
                        <div class="meta-value">
                            <asp:HyperLink ID="lnkCurrentImage" runat="server" Target="_blank" Text="Open current image" />
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</div>

<div id="lamaxLoadingOverlay" class="loading-overlay">
    <div class="loading-card">
        <div class="loading-spinner"></div>
        <div class="loading-title">Processing image...</div>
        <div class="loading-text">
            Please wait while LamaX prepares your Amazon main image.
        </div>

        <div class="loading-steps">
            <div class="loading-step"><strong>Step 1:</strong> Uploading image</div>
            <div class="loading-step"><strong>Step 2:</strong> Cleaning background with AI</div>
            <div class="loading-step"><strong>Step 3:</strong> Building square Amazon canvas</div>
            <div class="loading-step"><strong>Step 4:</strong> Saving image to database</div>
        </div>
    </div>
</div>

</asp:Content>