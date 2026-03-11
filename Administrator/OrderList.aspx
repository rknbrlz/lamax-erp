<%@ Page Title="OrderList" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="OrderList.aspx.cs" Inherits="Feniks.Administrator.OrderList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f6f6f6; }

    .orderlist-page{
        padding:4px 0 12px 0;
    }

    .page-header-row{
        display:flex;
        align-items:flex-start;
        justify-content:space-between;
        gap:12px;
        margin-top:8px;
        flex-wrap:wrap;
    }

    .title-wrap{
        min-width:0;
    }

    .title-wrap h2{
        margin:0;
        font-weight:800;
        letter-spacing:.2px;
        font-size:28px;
        line-height:1.15;
        color:#1f2937;
    }

    .subtitle{
        color:#777;
        margin-top:4px;
        font-size:12px;
        line-height:1.4;
    }

    .top-actions{
        display:flex;
        align-items:center;
        justify-content:flex-end;
        gap:10px;
        flex-wrap:wrap;
        min-width:0;
    }

    .kpi-tabs{
        display:inline-flex;
        gap:0;
        border:1px solid #e5e5e5;
        border-radius:999px;
        overflow:hidden;
        background:#fff;
        box-shadow:0 6px 16px rgba(0,0,0,.06);
        flex-wrap:nowrap;
        max-width:100%;
    }

    .kpi-tab{
        padding:8px 14px;
        font-weight:700;
        font-size:12px;
        color:#555;
        background:#fff;
        border:0;
        outline:none;
        text-decoration:none !important;
        display:inline-block;
        white-space:nowrap;
    }

    .kpi-tab:hover{ background:#f6f6f6; }
    .kpi-tab.active{ background:#0d6efd; color:#fff; }
    .kpi-tab + .kpi-tab{ border-left:1px solid #e5e5e5; }

    .kpi-grid{
        display:grid;
        grid-template-columns:repeat(4, minmax(0,1fr));
        gap:16px;
        margin-top:14px;
        margin-bottom:12px;
    }

    .kpi-card{
        background:#fff;
        border:1px solid #eee;
        border-radius:14px;
        box-shadow:0 6px 18px rgba(0,0,0,.06);
        padding:16px 18px;
        min-width:0;
    }

    .kpi-title{
        color:#666;
        font-weight:700;
        font-size:12px;
        margin-bottom:6px;
        display:flex;
        justify-content:space-between;
        align-items:center;
        gap:8px;
        min-width:0;
    }

    .kpi-value{
        font-size:28px;
        font-weight:900;
        line-height:1.1;
        margin-bottom:4px;
        word-break:break-word;
    }

    .kpi-sub{
        color:#888;
        font-size:12px;
        line-height:1.4;
    }

    .filters{
        background:#fff;
        border:1px solid #eee;
        border-radius:14px;
        box-shadow:0 6px 18px rgba(0,0,0,.06);
        padding:14px;
        margin:8px 0 14px 0;
    }

    .filters .form-control{ height:38px; }

    .filters .row{
        margin-left:-8px;
        margin-right:-8px;
    }

    .filters [class^="col-"],
    .filters [class*=" col-"]{
        padding-left:8px;
        padding-right:8px;
        margin-bottom:10px;
    }

    .filter-label{
        display:block;
        font-weight:700;
        font-size:12px;
        color:#555;
        margin-bottom:6px;
    }

    .grid-wrap{
        background:#fff;
        border:1px solid #eee;
        border-radius:14px;
        box-shadow:0 6px 18px rgba(0,0,0,.06);
        padding:10px 10px 2px 10px;
        overflow:hidden;
    }

    .grid-scroll{
        width:100%;
        overflow-x:auto;
        -webkit-overflow-scrolling:touch;
    }

    .grid-scroll .table{
        min-width:980px;
        margin-bottom:0;
    }

    .table>thead>tr>th{
        background:#fafafa;
        border-bottom:1px solid #eee;
        white-space:nowrap;
    }

    .money-usd{
        font-weight:800;
        white-space:nowrap;
    }

    .money-pln{
        color:#666;
        font-size:11px;
        margin-top:2px;
        white-space:nowrap;
    }

    .profit-pos{ color:#0a7a2f; font-weight:800; }
    .profit-zero{ color:#999; font-weight:700; }

    .status-pill{
        display:inline-block;
        padding:6px 10px;
        border-radius:999px;
        font-weight:800;
        font-size:12px;
        border:1px solid rgba(0,0,0,.06);
        line-height:1.2;
        white-space:nowrap;
    }

    .status-preparing{ background:#fff3cd; color:#8a6d3b; }
    .status-final{ background:#dff0d8; color:#2e6b2f; }
    .status-other{ background:#eee; color:#444; }
    .status-ready{ background:#d9edf7; color:#31708f; }
    .status-packaged{ background:#dff0d8; color:#2e6b2f; }
    .status-cancel{ background:#f2dede; color:#a94442; }
    .status-wait{ background:#eee; color:#555; }
    .status-size{ background:#fcf8e3; color:#8a6d3b; }
    .status-supplier{ background:#f5f5f5; color:#555; }
    .status-preship{ background:#e7f1ff; color:#2f5fb3; }

    .row-final-shipping td{ background:#e9f7ee !important; }

    .pagination-ys{
        padding:8px 0 4px 0;
    }

    .pagination-ys table > tbody > tr > td{
        display:inline;
    }

    .pagination-ys table > tbody > tr > td > a,
    .pagination-ys table > tbody > tr > td > span{
        float:left;
        padding:8px 12px;
        margin-left:-1px;
        border:1px solid #ddd;
        background:#fff;
        color:#0d6efd;
        text-decoration:none;
    }

    .pagination-ys table > tbody > tr > td > span{
        background:#f5f5f5;
        color:#999;
    }

    .pagination-ys table > tbody > tr > td:first-child > a,
    .pagination-ys table > tbody > tr > td:first-child > span{
        border-top-left-radius:8px;
        border-bottom-left-radius:8px;
    }

    .pagination-ys table > tbody > tr > td:last-child > a,
    .pagination-ys table > tbody > tr > td:last-child > span{
        border-top-right-radius:8px;
        border-bottom-right-radius:8px;
    }

    #loadingOverlay{
        position:fixed;
        top:0;
        left:0;
        width:100%;
        height:100%;
        background:rgba(255,255,255,0.72);
        z-index:9999;
        display:none;
        align-items:center;
        justify-content:center;
        font-size:20px;
        font-weight:bold;
        color:#333;
        text-align:center;
        padding:20px;
    }

    .todo-wrap{
        background:#fff;
        border:1px solid #eee;
        border-radius:14px;
        box-shadow:0 6px 18px rgba(0,0,0,.06);
        padding:14px;
        margin:6px 0 14px 0;
    }

    .todo-head{
        display:flex;
        align-items:flex-start;
        justify-content:space-between;
        gap:12px;
        flex-wrap:wrap;
    }

    .todo-title{
        font-weight:900;
        font-size:14px;
        color:#1f2937;
    }

    .todo-sub{
        color:#777;
        font-size:12px;
        margin-top:2px;
        line-height:1.4;
    }

    .todo-actions{
        display:flex;
        gap:8px;
        align-items:center;
        flex-wrap:wrap;
    }

    .total-text{
        margin:6px 2px 10px 2px;
    }

    @media (max-width: 1199px){
        .kpi-grid{
            grid-template-columns:repeat(2, minmax(0,1fr));
        }
    }

    @media (max-width: 991px){
        .page-header-row{
            flex-direction:column;
            align-items:stretch;
        }

        .top-actions{
            justify-content:flex-start;
        }

        .kpi-tabs{
            overflow-x:auto;
            overflow-y:hidden;
            -webkit-overflow-scrolling:touch;
            max-width:100%;
        }

        .filters .btn-block{
            display:block;
            width:100%;
        }
    }

    @media (max-width: 767px){
        .orderlist-page{
            padding-top:2px;
        }

        .title-wrap h2{
            font-size:22px;
        }

        .subtitle{
            font-size:12px;
        }

        .top-actions{
            flex-direction:column;
            align-items:stretch;
        }

        .top-actions .btn,
        .todo-actions .btn{
            width:100%;
        }

        .kpi-tabs{
            width:100%;
            display:flex;
        }

        .kpi-grid{
            grid-template-columns:1fr;
            gap:12px;
        }

        .kpi-card{
            padding:14px;
        }

        .kpi-value{
            font-size:24px;
        }

        .filters{
            padding:12px;
        }

        .grid-wrap{
            padding:8px 8px 2px 8px;
        }

        .pagination-ys table > tbody > tr > td > a,
        .pagination-ys table > tbody > tr > td > span{
            padding:7px 10px;
            font-size:12px;
        }
    }
</style>

<div id="loadingOverlay">Loading, please wait...</div>

<script type="text/javascript">
    function lamaxShowLoading() {
        var el = document.getElementById("loadingOverlay");
        if (el) el.style.display = "flex";
    }

    function lamaxHideLoading() {
        var el = document.getElementById("loadingOverlay");
        if (el) el.style.display = "none";
    }

    function initOrderListLoading() {
        if (typeof (Sys) === "undefined" ||
            !Sys.WebForms ||
            !Sys.WebForms.PageRequestManager) {
            return;
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (!prm) return;

        prm.remove_beginRequest(lamaxShowLoading);
        prm.remove_endRequest(lamaxHideLoading);

        prm.add_beginRequest(lamaxShowLoading);
        prm.add_endRequest(lamaxHideLoading);
    }

    if (window.addEventListener) {
        window.addEventListener("load", initOrderListLoading);
    } else if (window.attachEvent) {
        window.attachEvent("onload", initOrderListLoading);
    }
</script>

<div class="orderlist-page">
    <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>

            <div class="page-header-row">
                <div class="title-wrap">
                    <h2>Orders</h2>
                    <div class="subtitle">Top: OrderStatus 1-2 | Bottom: OrderStatus 3-4</div>
                </div>

                <div class="top-actions">
                    <asp:Button ID="btnMainMenu" runat="server" Text="← Main Menu"
                        CssClass="btn btn-default" OnClick="btnMainMenu_Click" />

                    <asp:Button ID="btnCreateOrder" runat="server" Text="+ Create Order"
                        CssClass="btn btn-success" OnClick="btnCreateOrder_Click" />

                    <div class="kpi-tabs">
                        <asp:LinkButton ID="btnKpiAll" runat="server" CssClass="kpi-tab"
                            CausesValidation="false" OnCommand="btnKpiPeriod_Command" CommandArgument="ALL">All</asp:LinkButton>
                        <asp:LinkButton ID="btnKpiToday" runat="server" CssClass="kpi-tab"
                            CausesValidation="false" OnCommand="btnKpiPeriod_Command" CommandArgument="TODAY">Today</asp:LinkButton>
                        <asp:LinkButton ID="btnKpiMonth" runat="server" CssClass="kpi-tab"
                            CausesValidation="false" OnCommand="btnKpiPeriod_Command" CommandArgument="MONTH">Month</asp:LinkButton>
                        <asp:LinkButton ID="btnKpiYear" runat="server" CssClass="kpi-tab"
                            CausesValidation="false" OnCommand="btnKpiPeriod_Command" CommandArgument="YEAR">Year</asp:LinkButton>
                        <asp:LinkButton ID="btnKpiCustom" runat="server" CssClass="kpi-tab"
                            CausesValidation="false" OnCommand="btnKpiPeriod_Command" CommandArgument="CUSTOM">Custom</asp:LinkButton>
                    </div>

                    <asp:Label ID="lblLastUpdated" runat="server" CssClass="text-muted"></asp:Label>

                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn btn-primary"
                        OnClick="btnRefresh_Click" />
                </div>
            </div>

            <!-- KPI cards -->
            <div class="kpi-grid">
                <div class="kpi-card">
                    <div class="kpi-title">
                        <span>Orders</span>
                        <asp:Label ID="lblKpiPeriodText" runat="server" CssClass="text-muted"></asp:Label>
                    </div>
                    <div class="kpi-value"><asp:Label ID="lblKpiOrders" runat="server" Text="0" /></div>
                    <div class="kpi-sub">Count</div>
                </div>

                <div class="kpi-card">
                    <div class="kpi-title"><span>Revenue</span></div>
                    <div class="kpi-value"><asp:Label ID="lblKpiRevenue" runat="server" Text="$ 0.00" /></div>
                    <div class="kpi-sub"><asp:Label ID="lblKpiRevenuePln" runat="server" Text="≈ 0.00 zł" /></div>
                </div>

                <div class="kpi-card">
                    <div class="kpi-title"><span>Profit (old)</span></div>
                    <div class="kpi-value"><asp:Label ID="lblKpiProfit" runat="server" Text="$ 0.00" /></div>
                    <div class="kpi-sub"><asp:Label ID="lblKpiProfitPln" runat="server" Text="≈ 0.00 zł" /></div>
                </div>

                <div class="kpi-card">
                    <div class="kpi-title"><span>USD→PLN</span></div>
                    <div class="kpi-value"><asp:Label ID="lblKpiFx" runat="server" Text="-" /></div>
                    <div class="kpi-sub">Revenue-weighted avg (period)</div>
                </div>
            </div>

            <!-- Unprocessed / Pending -->
            <div class="todo-wrap">
                <div class="todo-head">
                    <div>
                        <div class="todo-title">Unprocessed Orders</div>
                        <div class="todo-sub">
                            Showing <asp:Label ID="lblUnprocessedCount" runat="server" Text="0"></asp:Label>
                            order(s) where <b>OrderStatus IN (1,2)</b>
                        </div>
                    </div>

                    <div class="todo-actions">
                        <asp:LinkButton ID="btnMainModeToggle" runat="server"
                            CssClass="btn btn-default btn-sm"
                            CausesValidation="false"
                            OnClick="btnMainModeToggle_Click">
                            Show Pending in main grid
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="grid-wrap" style="padding:10px; margin-top:10px;">
                    <div class="grid-scroll">
                        <asp:GridView ID="gvUnprocessed" runat="server"
                            CssClass="table table-bordered table-hover"
                            AutoGenerateColumns="False"
                            AllowPaging="False"
                            OnRowDataBound="gvOrders_RowDataBound"
                            EmptyDataText="No unprocessed orders found."
                            GridLines="None" BorderStyle="None">

                            <Columns>
                                <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:yyyy-MM-dd}" />

                                <asp:TemplateField HeaderText="Order Number">
                                    <ItemTemplate>
                                        <a href='OrderDetailsUpdate.aspx?OrderNumber=<%# Eval("OrderNumber") %>' target="_blank" style="font-weight:800;">
                                            <%# Eval("OrderNumber") %>
                                        </a>
                                        <asp:Label ID="lblOrderNumberHidden" runat="server" Text='<%# Eval("OrderNumber") %>' Visible="false"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" />
                                <asp:BoundField DataField="BuyerFullName" HeaderText="Buyer" />
                                <asp:BoundField DataField="Country" HeaderText="Country" />
                                <asp:BoundField DataField="ShippingType" HeaderText="Ship Type" />

                                <asp:TemplateField HeaderText="Waiting">
                                    <ItemTemplate>
                                        <span style="font-weight:900;">
                                            <%# Eval("WaitingDays") %> d
                                        </span>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" Width="90px" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Order Total">
                                    <ItemTemplate>
                                        <div class="money-usd">$ <%# Eval("TotalPrice", "{0:0.00}") %></div>
                                        <div class="money-pln">(<%# Eval("TotalPricePln", "{0:0.00}") %> zł)</div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle HorizontalAlign="Right" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Net Profit">
                                    <ItemTemplate>
                                        <asp:Label ID="lblNetProfitUsd" runat="server" CssClass="profit-zero" Text="$ 0.00"></asp:Label>
                                        <div class="money-pln">
                                            (<asp:Label ID="lblNetProfitPln" runat="server" Text="0.00"></asp:Label> zł)
                                        </div>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <HeaderStyle HorizontalAlign="Right" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatusRaw" runat="server" Text='<%# Eval("ShippingStatus") %>' Visible="false" />
                                        <asp:Label ID="lblStatusPill" runat="server" CssClass="status-pill status-other"
                                            Text='<%# Eval("ShippingStatus") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <!-- Filters -->
            <div class="filters">
                <asp:Panel ID="pnlFilters" runat="server" DefaultButton="btnApplyFilters">
                    <div class="row">

                        <div class="col-md-2 col-sm-6">
                            <label class="filter-label">Marketplace</label>
                            <asp:DropDownList ID="ddMarketplace" runat="server" CssClass="form-control"
                                AutoPostBack="true" OnSelectedIndexChanged="Filters_Changed" />
                        </div>

                        <div class="col-md-2 col-sm-6">
                            <label class="filter-label">Status (Shipping)</label>
                            <asp:DropDownList ID="ddStatus" runat="server" CssClass="form-control"
                                AutoPostBack="true" OnSelectedIndexChanged="Filters_Changed" />
                        </div>

                        <div class="col-md-3 col-sm-12">
                            <label class="filter-label">Search (Order/Buyer/Country)</label>
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control"
                                AutoPostBack="true" OnTextChanged="Filters_Changed"
                                placeholder="e.g. 3960... / Jeff / United..." />
                        </div>

                        <div class="col-md-2 col-sm-6">
                            <label class="filter-label">From</label>
                            <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" TextMode="Date"
                                AutoPostBack="true" OnTextChanged="Filters_Changed" />
                        </div>

                        <div class="col-md-2 col-sm-6">
                            <label class="filter-label">To</label>
                            <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" TextMode="Date"
                                AutoPostBack="true" OnTextChanged="Filters_Changed" />
                        </div>

                        <div class="col-md-1 col-sm-12">
                            <label class="filter-label">&nbsp;</label>
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-default btn-block"
                                OnClick="btnClear_Click" />
                        </div>

                    </div>

                    <div style="display:none">
                        <asp:Button ID="btnApplyFilters" runat="server" Text="Apply" OnClick="btnApplyFilters_Click" />
                    </div>
                </asp:Panel>
            </div>

            <div class="text-muted total-text">
                Total (main grid): <asp:Label ID="lblTotalCount" runat="server" Text="0"></asp:Label>
            </div>

            <!-- Main grid -->
            <div class="grid-wrap">
                <div class="grid-scroll">
                    <asp:GridView ID="gvOrders" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        AllowPaging="True"
                        AllowCustomPaging="True"
                        PageSize="20"
                        OnPageIndexChanging="gvOrders_PageIndexChanging"
                        OnRowDataBound="gvOrders_RowDataBound"
                        EmptyDataText="No orders found."
                        GridLines="None" BorderStyle="None">

                        <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last" />
                        <PagerStyle CssClass="pagination-ys" />

                        <Columns>
                            <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:yyyy-MM-dd}" />

                            <asp:TemplateField HeaderText="Order Number">
                                <ItemTemplate>
                                    <a href='OrderDetailsUpdate.aspx?OrderNumber=<%# Eval("OrderNumber") %>' target="_blank" style="font-weight:800;">
                                        <%# Eval("OrderNumber") %>
                                    </a>
                                    <asp:Label ID="lblOrderNumberHidden" runat="server" Text='<%# Eval("OrderNumber") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" />
                            <asp:BoundField DataField="BuyerFullName" HeaderText="Buyer" />
                            <asp:BoundField DataField="Country" HeaderText="Country" />
                            <asp:BoundField DataField="ShippingType" HeaderText="Ship Type" />

                            <asp:TemplateField HeaderText="Order Total">
                                <ItemTemplate>
                                    <div class="money-usd">$ <%# Eval("TotalPrice", "{0:0.00}") %></div>
                                    <div class="money-pln">(<%# Eval("TotalPricePln", "{0:0.00}") %> zł)</div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle HorizontalAlign="Right" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Net Profit">
                                <ItemTemplate>
                                    <asp:Label ID="lblNetProfitUsd" runat="server" CssClass="profit-zero" Text="$ 0.00"></asp:Label>
                                    <div class="money-pln">
                                        (<asp:Label ID="lblNetProfitPln" runat="server" Text="0.00"></asp:Label> zł)
                                    </div>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" />
                                <HeaderStyle HorizontalAlign="Right" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatusRaw" runat="server" Text='<%# Eval("ShippingStatus") %>' Visible="false" />
                                    <asp:Label ID="lblStatusPill" runat="server" CssClass="status-pill status-other"
                                        Text='<%# Eval("ShippingStatus") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnMainMenu" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnCreateOrder" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnClear" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="gvOrders" EventName="PageIndexChanging" />

            <asp:AsyncPostBackTrigger ControlID="btnKpiAll" EventName="Command" />
            <asp:AsyncPostBackTrigger ControlID="btnKpiToday" EventName="Command" />
            <asp:AsyncPostBackTrigger ControlID="btnKpiMonth" EventName="Command" />
            <asp:AsyncPostBackTrigger ControlID="btnKpiYear" EventName="Command" />
            <asp:AsyncPostBackTrigger ControlID="btnKpiCustom" EventName="Command" />

            <asp:AsyncPostBackTrigger ControlID="ddMarketplace" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddStatus" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtSearch" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtFrom" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtTo" EventName="TextChanged" />

            <asp:AsyncPostBackTrigger ControlID="btnMainModeToggle" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>
</div>

</asp:Content>