<%@ Page Title="Amazon Main Image" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ProductAmazonMainImage.aspx.cs"
    Inherits="Feniks.Administrator.ProductAmazonMainImage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }

    .page-wrap {
        padding: 16px;
    }

    .card-box {
        background: #fff;
        border: 1px solid #e7e7e7;
        border-radius: 18px;
        padding: 22px;
        margin-bottom: 18px;
        box-shadow: 0 4px 14px rgba(0,0,0,.04);
    }

    .page-title {
        font-size: 26px;
        font-weight: 800;
        color: #1f2937;
        margin: 0 0 6px 0;
    }

    .page-subtitle {
        color: #6b7280;
        margin-bottom: 20px;
    }

    .section-title {
        font-size: 16px;
        font-weight: 700;
        color: #1f2937;
        margin-bottom: 14px;
    }

    .field-label {
        font-weight: 700;
        color: #374151;
        margin-bottom: 6px;
        display: block;
    }

    .form-control {
        border-radius: 12px;
        min-height: 42px;
    }

    .help-text {
        color: #6b7280;
        font-size: 12px;
        margin-top: 6px;
    }

    .btn-main {
        border: none;
        border-radius: 12px;
        padding: 10px 18px;
        font-weight: 700;
    }

    .btn-primary2 {
        background: #111827;
        color: #fff;
    }

    .btn-primary2:hover {
        background: #000;
        color: #fff;
    }

    .btn-soft {
        background: #eef2f7;
        color: #111827;
    }

    .btn-soft:hover {
        background: #e5e7eb;
        color: #111827;
    }

    .preview-box {
        border: 1px dashed #d1d5db;
        border-radius: 18px;
        background: #fafafa;
        min-height: 340px;
        display: flex;
        align-items: center;
        justify-content: center;
        overflow: hidden;
        padding: 16px;
    }

    .preview-box img {
        max-width: 100%;
        max-height: 520px;
        border-radius: 12px;
        background: #fff;
        box-shadow: 0 2px 12px rgba(0,0,0,.04);
    }

    .status-label {
        display: inline-block;
        padding: 6px 12px;
        border-radius: 999px;
        background: #eef6ff;
        color: #1d4ed8;
        font-weight: 700;
        font-size: 12px;
        margin-top: 12px;
    }

    .meta-box {
        background: #f9fafb;
        border: 1px solid #e5e7eb;
        border-radius: 14px;
        padding: 14px 16px;
        margin-top: 14px;
        color: #374151;
    }

    .validation-block {
        margin-top: 12px;
    }

    .loading-overlay {
        position: fixed;
        inset: 0;
        background: rgba(255,255,255,.78);
        z-index: 99999;
        display: none;
        align-items: center;
        justify-content: center;
        backdrop-filter: blur(2px);
    }

    .loading-box {
        background: #ffffff;
        border: 1px solid #e5e7eb;
        border-radius: 18px;
        padding: 24px 28px;
        min-width: 280px;
        text-align: center;
        box-shadow: 0 12px 30px rgba(0,0,0,.08);
    }

    .loading-spinner {
        width: 42px;
        height: 42px;
        border: 4px solid #e5e7eb;
        border-top: 4px solid #111827;
        border-radius: 50%;
        margin: 0 auto 14px auto;
        animation: spin 0.8s linear infinite;
    }

    .loading-title {
        font-size: 16px;
        font-weight: 800;
        color: #111827;
        margin-bottom: 4px;
    }

    .loading-text {
        color: #6b7280;
        font-size: 13px;
    }

    @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
    }

    @media (max-width: 767px) {
        .page-title {
            font-size: 22px;
        }
    }
</style>

<script type="text/javascript">
    function showLoading(message) {
        var overlay = document.getElementById('loadingOverlay');
        var text = document.getElementById('loadingText');

        if (text && message) {
            text.innerHTML = message;
        }

        if (overlay) {
            overlay.style.display = 'flex';
        }

        return true;
    }
</script>

<div id="loadingOverlay" class="loading-overlay">
    <div class="loading-box">
        <div class="loading-spinner"></div>
        <div class="loading-title">Processing...</div>
        <div id="loadingText" class="loading-text">Preparing Amazon main image.</div>
    </div>
</div>

<div class="page-wrap">

    <div class="card-box">
        <div class="page-title">Amazon Main Image Upload</div>
        <div class="page-subtitle">
            Upload one product photo, process it with OpenAI, convert it to Amazon-ready square format, and save it to database.
        </div>

        <div class="row">
            <div class="col-md-6">
                <div class="section-title">Image Settings</div>

                <label class="field-label">SKU</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" MaxLength="50"
                    placeholder="Enter SKU"></asp:TextBox>

                <div class="help-text">
                    The system checks your SKU in T_Product before saving.
                </div>

                <br />

                <label class="field-label">Product Photo</label>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />

                <div class="help-text">
                    Supported: JPG, JPEG, PNG, WEBP, BMP. Final output will be saved as 2000x2000 JPEG.
                </div>

                <br />

                <div class="row">
                    <div class="col-sm-6">
                        <label class="field-label">Canvas Size</label>
                        <asp:TextBox ID="txtCanvasSize" runat="server" CssClass="form-control" Text="2000" />
                    </div>
                    <div class="col-sm-6">
                        <label class="field-label">Product Fill Ratio (%)</label>
                        <asp:TextBox ID="txtFillRatio" runat="server" CssClass="form-control" Text="85" />
                    </div>
                </div>

                <div class="help-text" style="margin-top:8px;">
                    OpenAI will clean the background first. Then the app will create the final Amazon-style square image.
                </div>

                <br />

                <asp:Button ID="btnSave" runat="server" Text="Convert with OpenAI & Save"
                    CssClass="btn btn-main btn-primary2"
                    OnClick="btnSave_Click"
                    OnClientClick="return showLoading('OpenAI is cleaning the background and preparing the final image...');" />

                <asp:Button ID="btnLoadCurrent" runat="server" Text="Load Current"
                    CssClass="btn btn-main btn-soft"
                    OnClick="btnLoadCurrent_Click"
                    OnClientClick="return showLoading('Loading current image from database...');" />

                <asp:Button ID="btnBack" runat="server" Text="Back"
                    CssClass="btn btn-main btn-soft"
                    OnClick="btnBack_Click" />

                <div class="validation-block">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </div>
            </div>

            <div class="col-md-6">
                <div class="section-title">Preview</div>

                <div class="preview-box">
                    <asp:Image ID="imgPreview" runat="server" Visible="false" />
                    <asp:Label ID="lblPreviewEmpty" runat="server" Text="No preview yet." ForeColor="#9ca3af"></asp:Label>
                </div>

                <asp:Panel ID="pnlMeta" runat="server" Visible="false" CssClass="meta-box">
                    <div><strong>SKU:</strong> <asp:Literal ID="litSku" runat="server"></asp:Literal></div>
                    <div><strong>Original File:</strong> <asp:Literal ID="litOriginalFile" runat="server"></asp:Literal></div>
                    <div><strong>Output:</strong> <asp:Literal ID="litOutputInfo" runat="server"></asp:Literal></div>
                    <div><strong>Saved Image:</strong> <asp:HyperLink ID="lnkCurrentImage" runat="server" Target="_blank">Open current image</asp:HyperLink></div>
                    <div>
                        <span class="status-label">OpenAI cleaned · White background · Square canvas · DB saved</span>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

</div>

</asp:Content>