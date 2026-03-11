<%@ Page Title="Stock" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Stock.aspx.cs" Inherits="Feniks.Administrator.Stock" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }
    .page-wrap { padding:14px; }

    .card-box{
        background:#fff;
        border:1px solid #e8e8e8;
        border-radius:18px;
        padding:22px;
        margin-bottom:18px;
        box-shadow:0 4px 12px rgba(0,0,0,.03);
    }

    .page-title{
        font-size:24px;
        font-weight:800;
        margin:0 0 6px 0;
        color:#1f2937;
    }

    .page-subtitle{
        color:#6b7280;
        margin-bottom:18px;
    }

    .form-control{
        border-radius:10px;
        height:42px;
        box-shadow:none !important;
    }

    .btn-main{
        background:#2f6fab;
        color:#fff;
        border:none;
        height:42px;
        border-radius:10px;
        padding:0 22px;
        font-weight:600;
    }

    .btn-main:hover{ color:#fff; opacity:.95; }

    .mode-pill{
        display:inline-block;
        min-width:28px;
        text-align:center;
        padding:4px 9px;
        border-radius:999px;
        font-size:12px;
        font-weight:700;
    }

    .mode-s { background:#e8f7ec; color:#2e7d32; }
    .mode-a { background:#e8f1ff; color:#1565c0; }
    .mode-n { background:#fff5db; color:#9a6b00; }

    .qty-ok{ color:#0b7a35; font-weight:800; }
    .qty-zero{ color:#111827; font-weight:800; }
    .qty-neg{ color:#c62828; font-weight:800; }

    .btn-xs2{
        padding:4px 10px;
        border:none;
        border-radius:6px;
        color:#fff !important;
        font-size:12px;
        text-decoration:none !important;
        display:inline-block;
        margin-right:4px;
    }

    .btn-receive{ background:#4caf50; }
    .btn-adjust{ background:#f0ad4e; }
    .btn-transfer{ background:#5bc0de; }

    .small-note{
        color:#6b7280;
        font-size:12px;
        margin-top:8px;
    }

    .table > thead > tr > th{
        background:#fafafa;
        border-bottom:1px solid #ddd;
        font-weight:700;
    }

    .toolbar-top{
        display:flex;
        justify-content:space-between;
        align-items:center;
        gap:10px;
        flex-wrap:wrap;
        margin-bottom:12px;
    }

    .muted-count{
        color:#6b7280;
        font-size:13px;
    }

    .msg-ok{
        background:#ecfdf3;
        border:1px solid #bbf7d0;
        color:#166534;
        padding:10px 12px;
        border-radius:10px;
        margin-bottom:12px;
    }

    .msg-err{
        background:#fef2f2;
        border:1px solid #fecaca;
        color:#991b1b;
        padding:10px 12px;
        border-radius:10px;
        margin-bottom:12px;
    }

    .op-panel{
        background:#fafafa;
        border:1px dashed #d7dce2;
        border-radius:14px;
        padding:16px;
        margin-top:16px;
    }

    .op-title{
        font-size:16px;
        font-weight:700;
        color:#1f2937;
        margin-bottom:14px;
    }

    .hidden-field{
        display:none;
    }
</style>

<div class="page-wrap">

    <div class="card-box">
        <div class="page-title">Stock Management</div>
        <div class="page-subtitle">
            Sized rings size bazlı görünür. Adjustable ve normal ürünler tek satır olarak yönetilir.
        </div>

        <asp:Literal ID="litMessage" runat="server" />

        <div class="row">
            <div class="col-md-3">
                <label>SKU contains</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Stock Mode</label>
                <asp:DropDownList ID="ddlStockMode" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Sized Ring (S)" Value="S" />
                    <asp:ListItem Text="Adjustable (A)" Value="A" />
                    <asp:ListItem Text="Normal (N)" Value="N" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <label>Product Type</label>
                <asp:DropDownList ID="ddlProductType" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Location</label>
                <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>&nbsp;</label><br />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-main" OnClick="btnSearch_Click" />
            </div>
        </div>

        <div class="small-note">
            <span class="mode-pill mode-s">S</span> = size bazlı,
            <span class="mode-pill mode-a">A</span> = adjustable,
            <span class="mode-pill mode-n">N</span> = normal ürün.
        </div>
    </div>

    <div class="card-box">
        <div class="toolbar-top">
            <asp:Literal ID="litCount" runat="server" />
        </div>

        <asp:GridView ID="gvStock" runat="server"
            AutoGenerateColumns="false"
            CssClass="table table-bordered table-hover"
            GridLines="None"
            DataKeyNames="VariantID,LocationID,ProductID,ProductSKU,VariantSKU,VariantName,LocationName"
            OnRowCommand="gvStock_RowCommand">
            <Columns>
                <asp:BoundField DataField="ProductSKU" HeaderText="Product SKU" />
                <asp:BoundField DataField="VariantSKU" HeaderText="Variant SKU" />
                <asp:BoundField DataField="ProductID" HeaderText="ProductID" />
                <asp:BoundField DataField="ProductType" HeaderText="Product Type" />
                <asp:TemplateField HeaderText="Mode">
                    <ItemTemplate>
                        <%# GetModeBadge(Eval("StockMode")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="VariantName" HeaderText="Variant" />
                <asp:BoundField DataField="LocationName" HeaderText="Location" />
                <asp:TemplateField HeaderText="OnHand">
                    <ItemTemplate>
                        <span class="qty-zero"><%# Eval("OnHandQty", "{0:0.####}") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ReservedQty" HeaderText="Reserved" DataFormatString="{0:0.####}" />
                <asp:TemplateField HeaderText="Available">
                    <ItemTemplate>
                        <%# GetAvailableHtml(Eval("AvailableQty")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnReceiveRow" runat="server"
                            CommandName="OpenReceive"
                            CommandArgument='<%# Container.DataItemIndex %>'
                            CssClass="btn-xs2 btn-receive">Receive</asp:LinkButton>

                        <asp:LinkButton ID="btnAdjustRow" runat="server"
                            CommandName="OpenAdjust"
                            CommandArgument='<%# Container.DataItemIndex %>'
                            CssClass="btn-xs2 btn-adjust">Adjust</asp:LinkButton>

                        <asp:LinkButton ID="btnTransferRow" runat="server"
                            CommandName="OpenTransfer"
                            CommandArgument='<%# Container.DataItemIndex %>'
                            CssClass="btn-xs2 btn-transfer">Transfer</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <asp:Panel ID="pnlOperation" runat="server" CssClass="op-panel" Visible="false">
            <div class="op-title">
                <asp:Literal ID="litOpTitle" runat="server" />
            </div>

            <asp:HiddenField ID="hfAction" runat="server" />
            <asp:HiddenField ID="hfVariantID" runat="server" />
            <asp:HiddenField ID="hfLocationID" runat="server" />

            <div class="row">
                <div class="col-md-3">
                    <label>Product SKU</label>
                    <asp:TextBox ID="txtOpProductSku" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-3">
                    <label>Variant</label>
                    <asp:TextBox ID="txtOpVariant" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-2">
                    <label>Current Location</label>
                    <asp:TextBox ID="txtOpLocation" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-2">
                    <label>Qty</label>
                    <asp:TextBox ID="txtQty" runat="server" CssClass="form-control" Text="1" />
                </div>
                <div class="col-md-2">
                    <label>Currency</label>
                    <asp:TextBox ID="txtCurrency" runat="server" CssClass="form-control" Text="PLN" />
                </div>
            </div>

            <div class="row" style="margin-top:12px;">
                <div class="col-md-2">
                    <label>Unit Cost</label>
                    <asp:TextBox ID="txtUnitCost" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <label>Ref No</label>
                    <asp:TextBox ID="txtRefNo" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-4">
                    <label>Note</label>
                    <asp:TextBox ID="txtNote" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-3">
                    <label>Transfer To Location</label>
                    <asp:DropDownList ID="ddlTransferLocation" runat="server" CssClass="form-control" />
                </div>
            </div>

            <div style="margin-top:14px;">
                <asp:Button ID="btnSaveOperation" runat="server" Text="Save" CssClass="btn-main" OnClick="btnSaveOperation_Click" />
                <asp:Button ID="btnCancelOperation" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="btnCancelOperation_Click" CausesValidation="false" />
            </div>
        </asp:Panel>
    </div>

</div>
</asp:Content>