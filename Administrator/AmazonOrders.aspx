<%@ Page Title="Amazon Orders" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AmazonOrders.aspx.cs" Inherits="Feniks.Administrator.AmazonOrders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style>
    body { background:#f5f7fb; }
    .page-wrap { padding:14px; }
    .card-box {
        background:#fff;
        border:1px solid #e5e7eb;
        border-radius:16px;
        padding:18px;
        margin-bottom:16px;
        box-shadow:0 6px 18px rgba(15,23,42,.04);
    }
    .page-title {
        font-size:24px;
        font-weight:800;
        margin:0 0 4px 0;
        color:#111827;
    }
    .page-subtitle {
        color:#6b7280;
        margin-bottom:16px;
    }
    .toolbar {
        display:flex;
        gap:10px;
        flex-wrap:wrap;
        margin-bottom:12px;
    }
    .btn-dark {
        background:#111827;
        color:#fff;
        border:none;
        border-radius:12px;
        padding:10px 14px;
        font-weight:700;
    }
    .btn-soft {
        background:#eef2ff;
        color:#3730a3;
        border:none;
        border-radius:12px;
        padding:10px 14px;
        font-weight:700;
    }
    .btn-green {
        background:#dcfce7;
        color:#166534;
        border:none;
        border-radius:12px;
        padding:8px 12px;
        font-weight:700;
    }
    .table-wrap { overflow:auto; }
    .grid-table {
        width:100%;
        border-collapse:collapse;
        font-size:13px;
    }
    .grid-table th, .grid-table td {
        padding:10px 12px;
        border-bottom:1px solid #eef2f7;
        text-align:left;
        vertical-align:middle;
    }
    .grid-table th {
        background:#f8fafc;
        color:#475569;
        font-weight:700;
    }
    .status-ok {
        display:inline-block;
        background:#dcfce7;
        color:#166534;
        border-radius:999px;
        padding:3px 9px;
        font-size:12px;
        font-weight:700;
    }
    .status-no {
        display:inline-block;
        background:#fee2e2;
        color:#991b1b;
        border-radius:999px;
        padding:3px 9px;
        font-size:12px;
        font-weight:700;
    }
</style>

<div class="page-wrap">
    <div class="card-box">
        <div class="page-title">Amazon Orders</div>
        <div class="page-subtitle">Amazon inbox sync works automatically. Promote to LamaX manually.</div>

        <div class="toolbar">
            <asp:Button ID="btnSync" runat="server" Text="Sync Amazon Inbox" CssClass="btn-dark" OnClick="btnSync_Click" />
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn-soft" OnClick="btnRefresh_Click" />
        </div>

        <asp:Label ID="lblResult" runat="server"></asp:Label>
    </div>

    <div class="card-box">
        <div class="table-wrap">
            <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="false" CssClass="grid-table" GridLines="None" OnRowCommand="gvOrders_RowCommand">
                <Columns>
                    <asp:BoundField DataField="AmazonOrderId" HeaderText="Amazon Order ID" />
                    <asp:BoundField DataField="PurchaseDateUtc" HeaderText="Purchase UTC" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <asp:BoundField DataField="OrderStatus" HeaderText="Status" />
                    <asp:BoundField DataField="OrderTotalAmount" HeaderText="Total" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="OrderTotalCurrency" HeaderText="Currency" />
                    <asp:BoundField DataField="ShippingName" HeaderText="Customer" />
                    <asp:BoundField DataField="ItemCount" HeaderText="Items" />
                    <asp:TemplateField HeaderText="Imported">
                        <ItemTemplate>
                            <%# Convert.ToBoolean(Eval("ImportedToLamax")) ? "<span class='status-ok'>YES</span>" : "<span class='status-no'>NO</span>" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LamaxOrderNumber" HeaderText="LamaX Order No" />
                    <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Button ID="btnPromote" runat="server"
                                Text="Promote"
                                CssClass="btn-green"
                                CommandName="promote"
                                CommandArgument='<%# Eval("AmazonOrderId") %>'
                                Visible='<%# !Convert.ToBoolean(Eval("ImportedToLamax")) %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>