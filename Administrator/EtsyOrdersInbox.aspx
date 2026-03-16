<%@ Page Title="Etsy Orders Inbox" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EtsyOrdersInbox.aspx.cs" Inherits="Feniks.Administrator.EtsyOrdersInbox" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f7fb; }
    .page-wrap { padding:18px; }
    .card-box{
        background:#fff;border:1px solid #e5e7eb;border-radius:18px;padding:20px;margin-bottom:18px;
        box-shadow:0 6px 18px rgba(15,23,42,.04);
    }
    .title-row{
        display:flex;align-items:center;justify-content:space-between;gap:12px;flex-wrap:wrap;
    }
    .page-title{font-size:26px;font-weight:800;color:#0f172a;margin:0;}
    .page-sub{color:#64748b;margin-top:4px;}
    .action-row{display:flex;gap:10px;flex-wrap:wrap;}
    .mini{
        display:inline-block;background:#f8fafc;border:1px solid #e2e8f0;border-radius:999px;
        padding:8px 12px;font-size:12px;color:#334155;margin-right:8px;
    }
    .grid td,.grid th{vertical-align:middle !important;}
    .grid th{background:#f8fafc;}
    .status-pill{
        display:inline-block;padding:4px 10px;border-radius:999px;font-size:12px;font-weight:700;
        background:#eef2ff;color:#3730a3;
    }
</style>

<div class="page-wrap">

    <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert"></asp:Panel>

    <div class="card-box">
        <div class="title-row">
            <div>
                <h1 class="page-title">Etsy Orders Inbox</h1>
                <div class="page-sub">Etsy’den gelen siparişleri burada gör, istersen LamaX’a aktar.</div>
            </div>

            <div class="action-row">
                <asp:HyperLink ID="lnkConnect" runat="server" NavigateUrl="~/Administrator/EtsyOAuthStart.aspx" CssClass="btn btn-default">
                    Connect Etsy
                </asp:HyperLink>

                <asp:Button ID="btnSyncNow" runat="server" Text="Sync Now" CssClass="btn btn-primary" OnClick="btnSyncNow_Click" />
                <asp:Button ID="btnImportSelected" runat="server" Text="Import Selected to LamaX" CssClass="btn btn-success" OnClick="btnImportSelected_Click" />
            </div>
        </div>

        <div style="margin-top:14px;">
            <span class="mini">Last sync: <asp:Literal ID="litLastSync" runat="server" /></span>
            <span class="mini">Last success: <asp:Literal ID="litLastSuccess" runat="server" /></span>
            <span class="mini">Last error: <asp:Literal ID="litLastError" runat="server" /></span>
        </div>
    </div>

    <div class="card-box">
        <asp:GridView ID="gvInbox" runat="server"
            AutoGenerateColumns="false"
            CssClass="table table-bordered table-hover grid"
            DataKeyNames="ReceiptId"
            OnRowCommand="gvInbox_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkRow" runat="server" />
                    </ItemTemplate>
                    <ItemStyle Width="40px" />
                </asp:TemplateField>

                <asp:BoundField DataField="ReceiptId" HeaderText="Receipt ID" />
                <asp:BoundField DataField="BuyerName" HeaderText="Buyer" />
                <asp:BoundField DataField="BuyerEmail" HeaderText="Email" />
                <asp:BoundField DataField="CountryIso" HeaderText="Country" />
                <asp:BoundField DataField="CurrencyCode" HeaderText="Currency" />
                <asp:BoundField DataField="GrandTotal" HeaderText="Total" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="ShippingCost" HeaderText="Shipping" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="TaxCost" HeaderText="Tax" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="ItemCount" HeaderText="Items" />

                <asp:TemplateField HeaderText="Paid">
                    <ItemTemplate>
                        <%# (Convert.ToBoolean(Eval("WasPaid")) ? "<span class='status-pill'>Paid</span>" : "<span class='status-pill'>No</span>") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Shipped">
                    <ItemTemplate>
                        <%# (Convert.ToBoolean(Eval("WasShipped")) ? "<span class='status-pill'>Shipped</span>" : "<span class='status-pill'>Open</span>") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Imported">
                    <ItemTemplate>
                        <%# (Convert.ToBoolean(Eval("ImportedToLamax")) ? "<span class='status-pill'>Imported</span>" : "<span class='status-pill'>Pending</span>") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="LamaxOrderNumber" HeaderText="LamaX Order No" />
                <asp:BoundField DataField="CreatedAtUtc" HeaderText="Created UTC" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                <asp:ButtonField Text="Import" CommandName="ImportRow" ButtonType="Button" ControlStyle-CssClass="btn btn-success btn-sm" />
            </Columns>
        </asp:GridView>
    </div>

</div>
</asp:Content>