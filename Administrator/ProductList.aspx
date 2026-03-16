<%@ Page Title="Product Management" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ProductList.aspx.cs" Inherits="Feniks.Administrator.ProductList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body {
        background: #f3f5f7;
    }

    .page-wrap {
        padding: 18px;
    }

    .hero-card {
        background: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
        border: 1px solid #d7dee7;
        border-radius: 16px;
        padding: 22px 24px;
        margin-bottom: 16px;
        box-shadow: 0 4px 14px rgba(15, 23, 42, 0.04);
    }

    .hero-flex {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 16px;
        flex-wrap: wrap;
    }

    .hero-title {
        margin: 0;
        font-size: 28px;
        font-weight: 800;
        color: #0f172a;
        letter-spacing: -.2px;
    }

    .hero-subtitle {
        margin: 6px 0 0 0;
        color: #64748b;
        font-size: 13px;
    }

    .btn-header {
        border-radius: 10px !important;
        padding: 10px 16px !important;
        font-weight: 700 !important;
    }

    .btn-dark-modern {
        background: #111827 !important;
        border: 1px solid #111827 !important;
        color: #fff !important;
    }

    .btn-dark-modern:hover,
    .btn-dark-modern:focus {
        background: #000 !important;
        border-color: #000 !important;
        color: #fff !important;
    }

    .card-box {
        background: #fff;
        border: 1px solid #d7dee7;
        border-radius: 16px;
        padding: 18px;
        margin-bottom: 16px;
        box-shadow: 0 4px 14px rgba(15, 23, 42, 0.03);
    }

    .section-title {
        margin: 0 0 14px 0;
        font-size: 13px;
        font-weight: 800;
        color: #0f172a;
        text-transform: uppercase;
        letter-spacing: .45px;
    }

    .filter-label {
        display: block;
        margin-bottom: 6px;
        font-size: 11px;
        font-weight: 800;
        color: #6b7280;
        text-transform: uppercase;
        letter-spacing: .4px;
    }

    .filter-box .form-control {
        height: 42px;
        border-radius: 10px;
        border: 1px solid #ccd5df;
        box-shadow: none;
    }

    .filter-actions {
        display: flex;
        gap: 8px;
        flex-wrap: wrap;
        margin-top: 23px;
    }

    .kpi-row {
        display: grid;
        grid-template-columns: repeat(4, minmax(180px, 1fr));
        gap: 12px;
        margin-bottom: 16px;
    }

    .kpi-card {
        background: #fff;
        border: 1px solid #d7dee7;
        border-radius: 14px;
        padding: 16px;
        box-shadow: 0 4px 12px rgba(15, 23, 42, 0.03);
    }

    .kpi-label {
        font-size: 11px;
        font-weight: 800;
        text-transform: uppercase;
        letter-spacing: .45px;
        color: #6b7280;
        margin-bottom: 8px;
    }

    .kpi-value {
        font-size: 24px;
        font-weight: 800;
        color: #111827;
        line-height: 1.1;
    }

    .kpi-sub {
        margin-top: 6px;
        font-size: 12px;
        color: #94a3b8;
    }

    .summary-bar {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 12px;
        flex-wrap: wrap;
        margin-bottom: 14px;
    }

    .summary-left {
        display: flex;
        gap: 8px;
        flex-wrap: wrap;
        align-items: center;
    }

    .summary-pill {
        display: inline-flex;
        align-items: center;
        padding: 8px 12px;
        border-radius: 10px;
        font-size: 12px;
        font-weight: 800;
        background: #f3f4f6;
        color: #111827;
        border: 1px solid #e5e7eb;
    }

    .summary-pill-dark {
        background: #111827;
        color: #fff;
        border-color: #111827;
    }

    .summary-note {
        font-size: 12px;
        color: #6b7280;
    }

    .grid-shell {
        overflow-x: auto;
        border: 1px solid #d7dee7;
        border-radius: 12px;
    }

    .table-products {
        margin-bottom: 0 !important;
        background: #fff;
        border-collapse: collapse !important;
    }

    .table-products > thead > tr > th {
        background: #eef2f6;
        color: #334155;
        border-top: 0 !important;
        border-bottom: 1px solid #cfd7e2 !important;
        border-right: 1px solid #dbe2ea !important;
        padding: 13px 10px !important;
        font-size: 11px;
        text-transform: uppercase;
        letter-spacing: .45px;
        font-weight: 800;
        vertical-align: middle !important;
        white-space: nowrap;
    }

    .table-products > thead > tr > th:last-child {
        border-right: 0 !important;
    }

    .table-products > tbody > tr > td {
        padding: 12px 10px !important;
        vertical-align: middle !important;
        border-top: 1px solid #dde3ea !important;
        border-right: 1px solid #edf1f5 !important;
    }

    .table-products > tbody > tr > td:last-child {
        border-right: 0 !important;
    }

    .table-products > tbody > tr:nth-child(odd) {
        background: #ffffff;
    }

    .table-products > tbody > tr:nth-child(even) {
        background: #f7f9fb;
    }

    .table-products > tbody > tr:hover {
        background: #eef4fa !important;
    }

    .photo-cell {
        width: 76px;
        text-align: center;
    }

    .photo-link {
        display: inline-block;
        text-decoration: none;
    }

    .photo-frame {
        width: 56px;
        height: 56px;
        border-radius: 10px;
        overflow: hidden;
        border: 1px solid #d8dee7;
        background: #fff;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: transform .15s ease, box-shadow .15s ease, border-color .15s ease;
    }

    .photo-link:hover .photo-frame {
        transform: scale(1.04);
        box-shadow: 0 6px 18px rgba(15, 23, 42, 0.10);
        border-color: #c7d2de;
    }

    .photo-frame img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        display: block;
    }

    .product-main {
        min-width: 220px;
    }

    .sku-line {
        display: block;
        color: #111827;
        font-size: 13px;
        font-weight: 800;
        line-height: 1.3;
        word-break: break-word;
    }

    .type-line {
        display: inline-block;
        margin-top: 6px;
        padding: 4px 9px;
        border-radius: 999px;
        background: #e8eefc;
        color: #2847a1;
        font-size: 11px;
        font-weight: 800;
        letter-spacing: .2px;
    }

    .mode-badge {
        display: inline-block;
        min-width: 34px;
        text-align: center;
        padding: 5px 9px;
        border-radius: 999px;
        font-size: 11px;
        font-weight: 800;
    }

    .mode-s {
        background: #dcfce7;
        color: #166534;
    }

    .mode-a {
        background: #dbeafe;
        color: #1d4ed8;
    }

    .mode-n {
        background: #fef3c7;
        color: #92400e;
    }

    .num-cell {
        text-align: right;
        white-space: nowrap;
        font-size: 13px;
        font-weight: 700;
        color: #111827;
    }

    .money-cell {
        text-align: right;
        white-space: nowrap;
        font-size: 13px;
        font-weight: 800;
        color: #0f172a;
    }

    .mini-muted {
        color: #64748b;
        font-size: 12px;
    }

    .action-compact {
        display: flex;
        gap: 6px;
        justify-content: center;
        align-items: center;
    }

    .btn-icon {
        width: 32px;
        height: 32px;
        padding: 0;
        border-radius: 8px;
        border: 1px solid #d5dbe3;
        background: #f8fafc;
        display: inline-flex;
        align-items: center;
        justify-content: center;
        transition: all .15s ease;
        text-decoration: none !important;
        cursor: pointer;
    }

    .btn-icon:hover,
    .btn-icon:focus {
        background: #eef2f6;
        border-color: #cdd6df;
        outline: none;
        text-decoration: none !important;
    }

    .btn-icon svg {
        width: 16px;
        height: 16px;
        fill: #334155;
        display: block;
    }

    .empty-box {
        text-align: center;
        padding: 34px 14px;
        color: #6b7280;
        font-size: 14px;
        background: #fff;
    }

    .alert {
        border-radius: 12px;
    }

    @media (max-width: 1200px) {
        .kpi-row {
            grid-template-columns: repeat(2, minmax(180px, 1fr));
        }
    }

    @media (max-width: 768px) {
        .hero-title {
            font-size: 24px;
        }

        .photo-frame {
            width: 50px;
            height: 50px;
        }

        .btn-header {
            width: 100%;
        }

        .kpi-row {
            grid-template-columns: 1fr;
        }
    }
</style>

<div class="page-wrap">

    <div class="hero-card">
        <div class="hero-flex">
            <div>
                <h1 class="hero-title">Product Management</h1>
                <p class="hero-subtitle">Review products, stock, sales, variants and latest unit prices from a single screen.</p>
            </div>

            <div>
                <a href="ProductEdit.aspx" class="btn btn-header btn-dark-modern">+ New Product</a>
            </div>
        </div>
    </div>

    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert alert-info" Style="display:block;"></asp:Label>

    <div class="kpi-row">
        <div class="kpi-card">
            <div class="kpi-label">Total Products</div>
            <div class="kpi-value"><asp:Literal ID="litKpiProducts" runat="server" Text="0"></asp:Literal></div>
            <div class="kpi-sub">Filtered result count</div>
        </div>

        <div class="kpi-card">
            <div class="kpi-label">Total Stock Qty</div>
            <div class="kpi-value"><asp:Literal ID="litKpiStockQty" runat="server" Text="0"></asp:Literal></div>
            <div class="kpi-sub">Sum of on-hand stock</div>
        </div>

        <div class="kpi-card">
            <div class="kpi-label">Total Sales Qty</div>
            <div class="kpi-value"><asp:Literal ID="litKpiSalesQty" runat="server" Text="0"></asp:Literal></div>
            <div class="kpi-sub">Historical sold quantity</div>
        </div>

        <div class="kpi-card">
            <div class="kpi-label">Average Unit Price</div>
            <div class="kpi-value"><asp:Literal ID="litKpiAvgPrice" runat="server" Text="0.00"></asp:Literal></div>
            <div class="kpi-sub">Latest purchase-based value</div>
        </div>
    </div>

    <div class="card-box filter-box">
        <h3 class="section-title">Search Filters</h3>

        <div class="row">
            <div class="col-md-4">
                <label class="filter-label">SKU</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" placeholder="Enter SKU or part of SKU"></asp:TextBox>
            </div>

            <div class="col-md-3">
                <label class="filter-label">Product Type</label>
                <asp:DropDownList ID="ddlProductType" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>

            <div class="col-md-3">
                <label class="filter-label">Stock Mode</label>
                <asp:DropDownList ID="ddlStockMode" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Sized Ring (S)" Value="S" />
                    <asp:ListItem Text="Adjustable Ring (A)" Value="A" />
                    <asp:ListItem Text="Normal (N)" Value="N" />
                </asp:DropDownList>
            </div>

            <div class="col-md-2">
                <div class="filter-actions">
                    <asp:Button ID="btnSearch" runat="server" Text="Search"
                        CssClass="btn btn-dark-modern btn-header"
                        OnClick="btnSearch_Click" />

                    <asp:Button ID="btnClear" runat="server" Text="Clear"
                        CssClass="btn btn-default btn-header"
                        OnClick="btnClear_Click" CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>

    <div class="card-box">
        <div class="summary-bar">
            <div class="summary-left">
                <span class="summary-pill summary-pill-dark">
                    Total:&nbsp;<asp:Literal ID="litResultCount" runat="server" Text="0"></asp:Literal>
                </span>
                <span class="summary-pill">Photo Source: Amazon Main Image</span>
            </div>

            <div class="summary-note">
                Sharp grid lines, zebra rows and compact actions are enabled.
            </div>
        </div>

        <div class="grid-shell">
            <asp:GridView ID="gvProducts" runat="server"
                AutoGenerateColumns="false"
                CssClass="table table-products"
                GridLines="None"
                EmptyDataText="No product found."
                DataKeyNames="ProductID,SKU"
                OnRowCommand="gvProducts_RowCommand"
                OnRowDataBound="gvProducts_RowDataBound">

                <EmptyDataTemplate>
                    <div class="empty-box">
                        No product found.
                    </div>
                </EmptyDataTemplate>

                <Columns>

                    <asp:TemplateField HeaderText="Photo">
                        <ItemStyle CssClass="photo-cell" Width="76px" />
                        <ItemTemplate>
                            <a class="photo-link"
                               href='<%# "ProductAmazonMainImage.aspx?sku=" + Server.UrlEncode(Convert.ToString(Eval("SKU"))) %>'
                               title="Open product images">
                                <span class="photo-frame">
                                    <img src='<%# ResolveUrl("~/Administrator/ProductMainImageHandler.ashx?sku=" + Server.UrlEncode(Convert.ToString(Eval("SKU"))) + "&v=" + Convert.ToString(Eval("ProductID"))) %>'
                                         alt="Product Photo" />
                                </span>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="ProductID" HeaderText="ID">
                        <ItemStyle CssClass="num-cell" Width="70px" />
                    </asp:BoundField>

                    <asp:TemplateField HeaderText="Product">
                        <ItemTemplate>
                            <div class="product-main">
                                <span class="sku-line"><%# Eval("SKU") %></span>
                                <span class="type-line"><%# Eval("ProductType") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Mode">
                        <ItemStyle Width="90px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="lblMode" runat="server"
                                Text='<%# Eval("StockMode") %>'
                                CssClass="mode-badge"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="StockQty" HeaderText="Stock Qty" DataFormatString="{0:N0}">
                        <ItemStyle CssClass="num-cell" Width="110px" />
                    </asp:BoundField>

                    <asp:BoundField DataField="SalesQty" HeaderText="Sales Qty" DataFormatString="{0:N0}">
                        <ItemStyle CssClass="num-cell" Width="110px" />
                    </asp:BoundField>

                    <asp:BoundField DataField="VariantCount" HeaderText="Variant Count" DataFormatString="{0:N0}">
                        <ItemStyle CssClass="num-cell" Width="120px" />
                    </asp:BoundField>

                    <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:N2}">
                        <ItemStyle CssClass="money-cell" Width="110px" />
                    </asp:BoundField>

                    <asp:BoundField DataField="RecordBy" HeaderText="Record By">
                        <ItemStyle CssClass="mini-muted" Width="120px" />
                    </asp:BoundField>

                    <asp:BoundField DataField="RecordDate" HeaderText="Created" DataFormatString="{0:yyyy-MM-dd HH:mm}">
                        <ItemStyle CssClass="mini-muted" Width="145px" />
                    </asp:BoundField>

                    <asp:TemplateField HeaderText="Actions">
                        <ItemStyle Width="90px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <div class="action-compact">
                                <asp:LinkButton ID="btnEdit" runat="server"
                                    CommandName="EditProduct"
                                    CommandArgument='<%# Eval("ProductID") %>'
                                    CssClass="btn-icon"
                                    ToolTip="Edit Product">
                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zm2.92 2.83H5v-.92l8.06-8.06.92.92L5.92 20.08zM20.71 7.04a1.003 1.003 0 000-1.42l-2.34-2.34a1.003 1.003 0 00-1.42 0l-1.83 1.83 3.75 3.75 1.84-1.82z"></path>
                                    </svg>
                                </asp:LinkButton>

                                <a class="btn-icon"
                                   title="Images"
                                   href='<%# "ProductAmazonMainImage.aspx?sku=" + Server.UrlEncode(Convert.ToString(Eval("SKU"))) %>'>
                                    <svg viewBox="0 0 24 24" aria-hidden="true">
                                        <path d="M21 19V5c0-1.1-.9-2-2-2H5C3.9 3 3 3.9 3 5v14c0 1.1.9 2 2 2h14c1.1 0 2-.9 2-2zm-2 0H5V5h14v14zM8.5 13.5l2.5 3.01L14.5 12l4.5 6H6l2.5-4.5z"></path>
                                    </svg>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>

</asp:Content>