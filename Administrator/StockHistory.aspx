<%@ Page Title="Stock History" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="StockHistory.aspx.cs" Inherits="Feniks.Administrator.StockHistory" %>

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

    .qty-in{
        color:#0b7a35;
        font-weight:800;
    }

    .qty-out{
        color:#c62828;
        font-weight:800;
    }

    .qty-zero{
        color:#111827;
        font-weight:800;
    }

    .tag{
        display:inline-block;
        border-radius:999px;
        padding:4px 10px;
        font-size:12px;
        font-weight:700;
    }

    .tag-receipt { background:#e8f7ec; color:#2e7d32; }
    .tag-adjust  { background:#fff5db; color:#9a6b00; }
    .tag-transfer{ background:#e8f1ff; color:#1565c0; }
    .tag-sale    { background:#f3e8ff; color:#7e22ce; }
    .tag-return  { background:#ecfeff; color:#0f766e; }
    .tag-other   { background:#f3f4f6; color:#374151; }

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

    .table > thead > tr > th{
        background:#fafafa;
        border-bottom:1px solid #ddd;
        font-weight:700;
        white-space:nowrap;
    }

    .table > tbody > tr > td{
        vertical-align:middle !important;
    }

    .cell-note{
        max-width:280px;
        white-space:normal;
        word-break:break-word;
    }
</style>

<div class="page-wrap">

    <div class="card-box">
        <div class="page-title">Stock History</div>
        <div class="page-subtitle">
            Tüm stok hareketlerini variant, lokasyon ve işlem tipi bazında izleyebilirsin.
        </div>

        <div class="row">
            <div class="col-md-3">
                <label>SKU contains</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Location</label>
                <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Txn Type</label>
                <asp:DropDownList ID="ddlTxnType" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="RECEIVE" Value="RECEIVE" />
                    <asp:ListItem Text="ADJUST" Value="ADJUST" />
                    <asp:ListItem Text="TRANSFER" Value="TRANSFER" />
                    <asp:ListItem Text="SALE" Value="SALE" />
                    <asp:ListItem Text="RETURN" Value="RETURN" />
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <label>Date From</label>
                <asp:TextBox ID="txtDateFrom" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-2">
                <label>Date To</label>
                <asp:TextBox ID="txtDateTo" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-1">
                <label>&nbsp;</label><br />
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-main" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>

    <div class="card-box">
        <div class="toolbar-top">
            <asp:Literal ID="litCount" runat="server" />
        </div>

        <asp:GridView ID="gvHistory" runat="server"
            AutoGenerateColumns="false"
            CssClass="table table-bordered table-hover"
            GridLines="None">
            <Columns>
                <asp:BoundField DataField="TxnDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                <asp:TemplateField HeaderText="Txn Type">
                    <ItemTemplate>
                        <%# GetTxnTypeBadge(Eval("TxnType")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="ProductSKU" HeaderText="Product SKU" />
                <asp:BoundField DataField="VariantSKU" HeaderText="Variant SKU" />
                <asp:BoundField DataField="ProductType" HeaderText="Product Type" />
                <asp:BoundField DataField="VariantName" HeaderText="Variant" />
                <asp:BoundField DataField="LocationName" HeaderText="Location" />
                <asp:TemplateField HeaderText="Qty">
                    <ItemTemplate>
                        <%# GetQtyHtml(Eval("QtyDelta")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="UnitCost" HeaderText="Unit Cost" DataFormatString="{0:0.####}" />
                <asp:BoundField DataField="Currency" HeaderText="Cur" />
                <asp:BoundField DataField="RefNo" HeaderText="Ref No" />
                <asp:BoundField DataField="CreatedBy" HeaderText="Created By" />
                <asp:TemplateField HeaderText="Note">
                    <ItemTemplate>
                        <div class="cell-note"><%# Server.HtmlEncode(Convert.ToString(Eval("Note"))) %></div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</div>
</asp:Content>