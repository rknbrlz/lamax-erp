<%@ Page Title="Product Images" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductImages.aspx.cs" Inherits="Feniks.Administrator.ProductImages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
<link rel="stylesheet" href="https://code.jquery.com/ui/1.13.2/themes/base/jquery-ui.css" />

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
        font-size:26px;
        font-weight:800;
        margin:0 0 6px 0;
        color:#1f2937;
    }

    .page-subtitle {
        color:#6b7280;
        margin-bottom:18px;
    }

    .filter-grid {
        display:grid;
        grid-template-columns:repeat(6, minmax(150px,1fr));
        gap:14px;
        align-items:end;
    }

    .filter-grid .field label {
        display:block;
        margin-bottom:6px;
        font-size:12px;
        font-weight:700;
        color:#374151;
        text-transform:uppercase;
        letter-spacing:.3px;
    }

    .drop-zone {
        margin-top:16px;
        border:2px dashed #d1d5db;
        border-radius:18px;
        background:#fafafa;
        padding:26px;
        text-align:center;
        transition:all .2s ease;
        cursor:pointer;
    }

    .drop-zone:hover {
        background:#f3f4f6;
        border-color:#9ca3af;
    }

    .drop-title {
        font-size:18px;
        font-weight:800;
        color:#111827;
        margin-bottom:6px;
    }

    .drop-note {
        color:#6b7280;
        font-size:13px;
        line-height:1.5;
    }

    .option-row {
        display:flex;
        flex-wrap:wrap;
        gap:18px;
        margin-top:16px;
        align-items:center;
    }

    .option-row span {
        display:flex;
        align-items:center;
    }

    .option-row label {
        font-weight:600;
        color:#374151;
        margin-left:6px;
        margin-right:12px;
        margin-bottom:0;
    }

    .action-row {
        display:flex;
        flex-wrap:wrap;
        gap:10px;
        margin-top:16px;
    }

    .help-note {
        margin-top:12px;
        color:#6b7280;
        font-size:12px;
        line-height:1.6;
    }

    .alert {
        margin-top:16px;
        margin-bottom:0;
        border-radius:12px;
    }

    .preview-title {
        font-size:18px;
        font-weight:800;
        color:#111827;
        margin-bottom:14px;
    }

    .img-grid {
        display:grid;
        grid-template-columns:repeat(auto-fill, minmax(240px,1fr));
        gap:16px;
    }

    .img-card {
        background:#fff;
        border:1px solid #e5e7eb;
        border-radius:18px;
        overflow:hidden;
        box-shadow:0 4px 12px rgba(0,0,0,.03);
        transition:all .2s ease;
        position:relative;
        cursor:move;
    }

    .img-card:hover {
        transform:translateY(-2px);
        box-shadow:0 8px 20px rgba(0,0,0,.06);
    }

    .img-card.primary-card {
        border:2px solid #16a34a;
        box-shadow:0 0 10px rgba(22,163,74,0.22);
    }

    .img-thumb {
        height:220px;
        background:linear-gradient(180deg,#f9fafb 0%,#f3f4f6 100%);
        display:flex;
        align-items:center;
        justify-content:center;
        overflow:hidden;
    }

    .img-thumb img {
        width:100%;
        height:100%;
        object-fit:contain;
        background:#fff;
        transition:transform .25s ease;
        pointer-events:none;
    }

    .img-card:hover .img-thumb img {
        transform:scale(1.04);
    }

    .img-body {
        padding:14px;
        cursor:default;
    }

    .img-name {
        font-size:14px;
        font-weight:700;
        color:#111827;
        line-height:1.4;
        min-height:40px;
        word-break:break-word;
        margin-bottom:10px;
    }

    .badge-row {
        display:flex;
        flex-wrap:wrap;
        gap:6px;
        margin-bottom:10px;
    }

    .soft-badge {
        display:inline-block;
        padding:5px 9px;
        border-radius:999px;
        font-size:11px;
        font-weight:700;
        letter-spacing:.2px;
        background:#eef2ff;
        color:#3730a3;
    }

    .soft-badge.primary {
        background:#dcfce7;
        color:#166534;
    }

    .soft-badge.market {
        background:#ecfeff;
        color:#155e75;
    }

    .img-meta {
        font-size:12px;
        color:#6b7280;
        line-height:1.6;
        margin-bottom:12px;
    }

    .img-actions {
        display:flex;
        gap:8px;
        flex-wrap:wrap;
    }

    .drag-hint {
        position:absolute;
        top:10px;
        right:10px;
        background:rgba(17,24,39,.78);
        color:#fff;
        border-radius:999px;
        padding:4px 8px;
        font-size:11px;
        font-weight:700;
        z-index:3;
        pointer-events:none;
    }

    .sort-badge {
        position:absolute;
        top:10px;
        left:10px;
        background:#111827;
        color:#fff;
        border-radius:999px;
        padding:4px 8px;
        font-size:11px;
        font-weight:700;
        z-index:3;
        pointer-events:none;
    }

    .sortable-placeholder {
        border:2px dashed #93c5fd;
        background:#eff6ff;
        border-radius:18px;
        min-height:260px;
        visibility:visible !important;
    }

    .drag-handle {
        position:absolute;
        bottom:10px;
        right:10px;
        background:#334155;
        color:#fff;
        border-radius:10px;
        padding:5px 8px;
        font-size:11px;
        font-weight:700;
        z-index:3;
        cursor:grab;
    }

    .drag-handle:active {
        cursor:grabbing;
    }

    @media (max-width:1200px) {
        .filter-grid { grid-template-columns:repeat(3, minmax(150px,1fr)); }
    }

    @media (max-width:768px) {
        .filter-grid { grid-template-columns:1fr; }
        .action-row { flex-direction:column; }
        .action-row .btn { width:100%; }
    }
</style>

<div class="page-wrap">
    <div class="card-box">
        <div class="page-title">Product Images</div>
        <div class="page-subtitle">Upload, optimize, whiten, center, shadow and reorder product photos by SKU.</div>

        <div class="filter-grid">
            <div class="field">
                <label>SKU</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" />
            </div>

            <div class="field">
                <label>Marketplace</label>
                <asp:DropDownList ID="ddlMarketplace" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Default" Value="" />
                    <asp:ListItem Text="Etsy" Value="ETSY" />
                    <asp:ListItem Text="Amazon" Value="AMAZON" />
                    <asp:ListItem Text="eBay" Value="EBAY" />
                    <asp:ListItem Text="Website" Value="WEBSITE" />
                </asp:DropDownList>
            </div>

            <div class="field">
                <label>Image Role</label>
                <asp:DropDownList ID="ddlImageRole" runat="server" CssClass="form-control">
                    <asp:ListItem Text="MAIN" Value="MAIN" />
                    <asp:ListItem Text="GALLERY" Value="GALLERY" />
                    <asp:ListItem Text="DETAIL" Value="DETAIL" />
                    <asp:ListItem Text="SIDE" Value="SIDE" />
                    <asp:ListItem Text="ANGLE" Value="ANGLE" />
                    <asp:ListItem Text="PACKAGING" Value="PACKAGING" />
                    <asp:ListItem Text="LIFESTYLE" Value="LIFESTYLE" />
                </asp:DropDownList>
            </div>

            <div class="field">
                <label>Preset</label>
                <asp:DropDownList ID="ddlPreset" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Auto" Value="AUTO" />
                    <asp:ListItem Text="Amazon" Value="AMAZON" />
                    <asp:ListItem Text="Etsy" Value="ETSY" />
                    <asp:ListItem Text="Website" Value="WEBSITE" />
                    <asp:ListItem Text="Square Studio" Value="SQUARE" />
                </asp:DropDownList>
            </div>

            <div class="field">
                <label>Start Sort Order</label>
                <asp:TextBox ID="txtSortOrder" runat="server" CssClass="form-control" TextMode="Number" />
            </div>

            <div class="field">
                <label>Image Upload</label>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
            </div>
        </div>

        <div class="drop-zone" onclick="document.getElementById('<%= fuImage.ClientID %>').click();">
            <div class="drop-title">Drop files here or click</div>
            <div class="drop-note">
                JPG, JPEG, PNG, WEBP · multiple selection supported · recommended limit: 10 MB per file<br />
                Auto white background, center alignment, soft shadow and marketplace crop presets included.
            </div>
        </div>

        <div class="option-row">
            <span>
                <asp:CheckBox ID="chkIsPrimary" runat="server" />
                <label for="<%= chkIsPrimary.ClientID %>">Make first uploaded image primary</label>
            </span>

            <span>
                <asp:CheckBox ID="chkUseAiBgRemoval" runat="server" />
                <label for="<%= chkUseAiBgRemoval.ClientID %>">Use AI background removal API</label>
            </span>

            <span>
                <asp:CheckBox ID="chkAutoWhiteBg" runat="server" Checked="true" />
                <label for="<%= chkAutoWhiteBg.ClientID %>">Auto clean light background to white</label>
            </span>

            <span>
                <asp:CheckBox ID="chkCenterSubject" runat="server" Checked="true" />
                <label for="<%= chkCenterSubject.ClientID %>">Center subject automatically</label>
            </span>

            <span>
                <asp:CheckBox ID="chkSoftShadow" runat="server" Checked="true" />
                <label for="<%= chkSoftShadow.ClientID %>">Add soft shadow</label>
            </span>
        </div>

        <div class="action-row">
            <asp:Button ID="btnLoad" runat="server" Text="Load Images" CssClass="btn btn-default" OnClick="btnLoad_Click" />
            <asp:Button ID="btnUpload" runat="server" Text="Upload Image(s)" CssClass="btn btn-primary" OnClick="btnUpload_Click" />
            <asp:Button ID="btnSaveOrder" runat="server" Text="Save Order" CssClass="btn btn-info" OnClientClick="prepareSortOrderBeforePostback(); return true;" OnClick="btnSaveOrder_Click" />
        </div>

        <div class="help-note">
            Drag cards using the HANDLE area, then click <strong>Save Order</strong>. You can also click Save Order without dragging; current visible order will still be saved.
        </div>

        <asp:Label ID="lblMsg" runat="server" EnableViewState="false"></asp:Label>
        <asp:HiddenField ID="hfSortOrder" runat="server" />
    </div>

    <div class="card-box">
        <div class="preview-title">Selected files preview</div>

        <asp:Repeater ID="rptImages" runat="server" OnItemCommand="rptImages_ItemCommand">
            <HeaderTemplate>
                <div id="imageSortable" class="img-grid">
            </HeaderTemplate>
            <ItemTemplate>
                <div class='img-card <%# Convert.ToBoolean(Eval("IsPrimary")) ? "primary-card" : "" %>' data-id='<%# Eval("ProductImageID") %>'>
                    <div class="sort-badge">#<%# Eval("SortOrder") %></div>
                    <div class="drag-hint">DRAG</div>

                    <div class="img-thumb">
                        <img src='/ProductImageHandler.ashx?id=<%# Eval("ProductImageID") %>&thumb=1&w=420&h=420'
                             alt='<%# Eval("FileName") %>' />
                    </div>

                    <div class="img-body">
                        <div class="img-name"><%# Eval("FileName") %></div>

                        <div class="badge-row">
                            <%# Convert.ToBoolean(Eval("IsPrimary")) ? "<span class='soft-badge primary'>PRIMARY</span>" : "" %>
                            <span class="soft-badge"><%# Eval("ImageRole") %></span>
                            <span class="soft-badge market"><%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Marketplace"))) ? "DEFAULT" : Eval("Marketplace").ToString() %></span>
                        </div>

                        <div class="img-meta">
                            ID: <%# Eval("ProductImageID") %>
                        </div>

                        <div class="img-actions">
                            <asp:LinkButton ID="btnMakePrimary" runat="server"
                                CssClass="btn btn-success btn-xs"
                                CommandName="makeprimary"
                                CommandArgument='<%# Eval("ProductImageID") %>'>
                                Make Primary
                            </asp:LinkButton>

                            <asp:LinkButton ID="btnDelete" runat="server"
                                CssClass="btn btn-danger btn-xs"
                                CommandName="deleteimg"
                                CommandArgument='<%# Eval("ProductImageID") %>'
                                OnClientClick="return confirm('Delete this image?');">
                                Delete
                            </asp:LinkButton>
                        </div>
                    </div>

                    <div class="drag-handle">HANDLE</div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>

<script>
    function updateVisibleSortBadges() {
        $("#imageSortable .img-card").each(function (index) {
            $(this).find(".sort-badge").first().text("#" + (index + 1));
        });
    }

    function prepareSortOrderBeforePostback() {
        var items = [];
        $("#imageSortable .img-card").each(function (index) {
            items.push($(this).attr("data-id") + ":" + (index + 1));
        });
        $("#<%= hfSortOrder.ClientID %>").val(items.join(","));
        updateVisibleSortBadges();
    }

    function bindImageSortable() {
        var $grid = $("#imageSortable");

        if (!$grid.length || typeof $grid.sortable !== "function") {
            return;
        }

        try {
            if ($grid.hasClass("ui-sortable")) {
                $grid.sortable("destroy");
            }
        } catch (e) { }

        $grid.sortable({
            items: ".img-card",
            handle: ".drag-handle",
            placeholder: "sortable-placeholder",
            tolerance: "pointer",
            forcePlaceholderSize: true,
            scroll: true,
            start: function () {
                $("body").addClass("sorting-active");
            },
            stop: function () {
                $("body").removeClass("sorting-active");
                prepareSortOrderBeforePostback();
            },
            update: function () {
                prepareSortOrderBeforePostback();
            }
        });

        $grid.disableSelection();
        prepareSortOrderBeforePostback();
    }

    $(document).ready(function () {
        bindImageSortable();
        updateVisibleSortBadges();
    });

    Sys.Application.add_load(function () {
        bindImageSortable();
        updateVisibleSortBadges();
    });
</script>

</asp:Content>