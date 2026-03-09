<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/Site.Master"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="Feniks.Administrator.Dashboard" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Dashboard
</asp:Content>

<asp:Content ID="cHead" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Dashboard local styles */
        .card{
            background:#fff; border-radius:12px; padding:16px; margin-bottom:16px;
            box-shadow:0 1px 3px rgba(0,0,0,.08);
            border:1px solid rgba(0,0,0,.04);
        }
        .card-title{ font-size:12px; color:#777; text-transform:uppercase; letter-spacing:.3px; margin-bottom:6px; }
        .card-value{ font-size:24px; font-weight:800; line-height:1.15; }
        .card-value.pay-value{ font-weight:400; }
        .filter-label { font-weight:700; font-size:12px; color:#333; }
        .muted { color:#888; font-size:12px; }
        .table th { font-size:12px; text-transform:uppercase; color:#555; }

        .chart-wrap { height:320px; }
        .chart-wrap canvas { width:100% !important; height:100% !important; }
        .chart-wrap-sm { height:240px; }
        .chart-wrap-sm canvas { width:100% !important; height:100% !important; }

        .chart-wrap-map { height:420px; }
        .chart-wrap-map canvas { width:100% !important; height:100% !important; }

        .section-title { font-weight:900; margin:0 0 12px 0; color:#333; }

        .nav-tabs>li>a { font-weight:800; }
        .tab-pane { padding-top:14px; }

        .pill{
            display:inline-block; padding:2px 8px; border-radius:999px; border:1px solid #eee;
            background:#fafafa; color:#777; font-size:11px; margin-left:6px;
        }

        /* Heatmap */
        .hm-table { width:100%; border-collapse:separate; border-spacing:6px; table-layout:fixed; }
        .hm-table th { font-size:11px; color:#666; text-transform:uppercase; letter-spacing:.25px; }
        .hm-cell{
            border:1px solid #eee; border-radius:10px; padding:10px 6px; text-align:center;
            background:#fafafa; font-weight:800; font-size:13px;
        }
        .hm-week { font-size:11px; color:#777; font-weight:800; width:110px; }
        .hm-legend { font-size:12px; color:#777; }
        .hm-note { font-size:12px; color:#888; }

        .loading{
            position:fixed; left:0; top:0; right:0; bottom:0;
            background:rgba(0,0,0,.25); display:none; z-index:9999;
        }
        .loading .box{
            position:absolute; left:50%; top:50%; transform:translate(-50%,-50%);
            background:#fff; padding:18px 22px; border-radius:12px;
            box-shadow:0 8px 26px rgba(0,0,0,.25); font-weight:800;
        }

        /* ✅ Accounting iframe */
        #tabAccounting iframe { background:#fff; }
    </style>

    <!-- Chart.js -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js"></script>

    <!-- MAP -->
    <script src="https://cdn.jsdelivr.net/npm/topojson-client@3/dist/topojson-client.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/chartjs-chart-geo@4.3.2/build/index.umd.min.js"></script>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="loading" id="loading"><div class="box">Loading...</div></div>

    <!-- FILTER -->
    <div class="card">
        <div class="row">
            <div class="col-md-2">
                <div class="filter-label">Period</div>
                <select id="ddlPeriod" class="form-control">
                    <option value="TODAY">Today</option>
                    <option value="MONTH" selected>Month</option>
                    <option value="YEAR">Year</option>
                    <option value="CUSTOM">Custom</option>
                </select>
            </div>
            <div class="col-md-2">
                <div class="filter-label">From</div>
                <input type="text" id="txtFrom" class="form-control" placeholder="dd.MM.yyyy or yyyy-MM-dd" />
            </div>
            <div class="col-md-2">
                <div class="filter-label">To</div>
                <input type="text" id="txtTo" class="form-control" placeholder="dd.MM.yyyy or yyyy-MM-dd" />
            </div>
            <div class="col-md-2" style="padding-top:20px;">
                <button type="button" id="btnApply" class="btn btn-primary btn-block" style="font-weight:800;">Apply</button>
            </div>
            <div class="col-md-4" style="padding-top:26px;">
                <div class="muted text-right hidden-sm hidden-xs" id="lblFxNote">USD/PLN: - (rate date: -)</div>
            </div>
        </div>
    </div>

    <!-- TABS -->
    <ul class="nav nav-tabs" role="tablist" id="dashTabs">
        <li role="presentation" class="active"><a href="#tabOverview" role="tab" data-toggle="tab">Overview</a></li>
        <li role="presentation"><a href="#tabPayments" role="tab" data-toggle="tab">Payments</a></li>
        <li role="presentation"><a href="#tabOrders" role="tab" data-toggle="tab">Orders</a></li>
        <li role="presentation"><a href="#tabProducts" role="tab" data-toggle="tab">Product</a></li>
        <li role="presentation"><a href="#tabCustomers" role="tab" data-toggle="tab">Customers</a></li>
        <li role="presentation"><a href="#tabCosts" role="tab" data-toggle="tab">Costs</a></li>

        <!-- ✅ NEW TAB -->
        <li role="presentation"><a href="#tabAccounting" role="tab" data-toggle="tab">Accounting Reports</a></li>
    </ul>

    <div class="tab-content">

        <!-- OVERVIEW -->
        <div role="tabpanel" class="tab-pane active" id="tabOverview">
            <h4 class="section-title">Sales KPIs</h4>
            <div class="row">
                <div class="col-md-3"><div class="card">
                    <div class="card-title">Revenue</div>
                    <div id="kpiRevenuePair" class="card-value">-</div>
                    <div class="muted">USD | PLN</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Orders</div>
                    <div id="kpiOrders" class="card-value">-</div>
                    <div class="muted">Count</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Items</div>
                    <div id="kpiItems" class="card-value">-</div>
                    <div class="muted">Qty</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">AOV</div>
                    <div id="kpiAovPair" class="card-value">-</div>
                    <div class="muted">USD | PLN</div>
                </div></div>
            </div>

            <h4 class="section-title">Trend</h4>
            <div class="card">
                <div class="muted" style="margin-bottom:8px;">Revenue shown as USD + PLN (two lines). Orders as bars.</div>
                <div class="chart-wrap"><canvas id="chartTrend"></canvas></div>
            </div>
        </div>

        <!-- PAYMENTS -->
        <div role="tabpanel" class="tab-pane" id="tabPayments">
            <h4 class="section-title">Payments KPIs (Amounts entered as USD)</h4>
            <div class="row">
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Payments - Year</div>
                    <div id="kpiPayYear" class="card-value pay-value">-</div>
                    <div class="muted">Current year</div>
                </div></div>
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Payments - Month</div>
                    <div id="kpiPayMonth" class="card-value pay-value">-</div>
                    <div class="muted">Current month</div>
                </div></div>
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Payments - Selected Period</div>
                    <div id="kpiPayPeriod" class="card-value pay-value">-</div>
                    <div class="muted">Filtered range</div>
                </div></div>
            </div>

            <h4 class="section-title">Payments by Marketplace</h4>
            <div class="card">
                <div class="muted">Bars PLN, tooltip shows USD | PLN</div>
                <div class="chart-wrap-sm"><canvas id="chartPayMarketplace"></canvas></div>
            </div>

            <h4 class="section-title">Payments List (Selected Period)</h4>
            <div class="card">
                <div class="muted" style="margin-bottom:10px;">Marketplace payouts (from <code>T_MarketplacePayments</code>).</div>
                <div class="table-responsive">
                    <table class="table table-striped" id="tblPaymentsList">
                        <thead>
                        <tr>
                            <th style="width:140px;">Pay Date</th>
                            <th style="width:180px;">Marketplace</th>
                            <th class="text-right" style="width:160px;">Amount USD</th>
                            <th class="text-right" style="width:160px;">Amount PLN</th>
                            <th style="width:180px;">CreatedAt</th>
                        </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- ORDERS -->
        <div role="tabpanel" class="tab-pane" id="tabOrders">
            <h4 class="section-title">Orders Analytics</h4>

            <div class="row">
                <div class="col-md-3"><div class="card">
                    <div class="card-title">Total Orders (Selected)</div>
                    <div id="kpiOrdersSelTotal" class="card-value">-</div>
                    <div class="muted" id="kpiOrdersSelTotalSub">-</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Avg / Day (Selected)</div>
                    <div id="kpiOrdersSelAvg" class="card-value">-</div>
                    <div class="muted">orders/day</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Peak Day (Selected)</div>
                    <div id="kpiOrdersSelPeakDay" class="card-value">-</div>
                    <div class="muted">highest day</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Peak Orders</div>
                    <div id="kpiOrdersSelPeakCount" class="card-value">-</div>
                    <div class="muted">max daily orders</div>
                </div></div>
            </div>

            <div class="row">
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Top Country (Last 30 Days)</div>
                    <div id="kpiTopCountryName" class="card-value">-</div>
                    <div class="muted"><span id="kpiTopCountryCount">-</span> orders</div>
                </div></div>

                <div class="col-md-8"><div class="card">
                    <div class="card-title">Orders Heatmap (Last 35 Days)</div>
                    <div class="hm-note" style="margin-bottom:8px;">Rows are weeks, columns are days (Mon–Sun). Values are order counts.</div>
                    <div class="table-responsive">
                        <table class="hm-table" id="tblHeatmap">
                            <thead>
                            <tr>
                                <th class="hm-week">Week</th>
                                <th>Mon</th><th>Tue</th><th>Wed</th><th>Thu</th><th>Fri</th><th>Sat</th><th>Sun</th>
                            </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <div class="hm-legend">Tip: This shows “busy patterns” clearly (e.g., Friday spikes).</div>
                </div></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Orders Map (Selected Period)</h4>
                <div class="muted" style="margin-bottom:8px;">
                    Choropleth map based on <b>OrdersCount by Country</b>.
                </div>
                <div class="chart-wrap-map"><canvas id="chartOrdersMap"></canvas></div>
                <div class="muted" id="mapNote" style="margin-top:8px;"></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Yearly (Last 12 months)</h4>
                <div class="muted">Bars: Orders • Line: Revenue PLN</div>
                <div class="chart-wrap"><canvas id="chartOrdersYear"></canvas></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Monthly (Last 30 days)</h4>
                <div class="muted">Bars: Orders • Line: AOV PLN</div>
                <div class="chart-wrap"><canvas id="chartOrdersMonth"></canvas></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Weekly (Last 7 days)</h4>
                <div class="muted">Bars: Orders • Line: Revenue PLN</div>
                <div class="chart-wrap-sm"><canvas id="chartOrdersWeek"></canvas></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Marketplace Share (Last 30 days)</h4>
                <div class="muted">Stacked bars: daily orders split by marketplace</div>
                <div class="chart-wrap"><canvas id="chartOrdersMarketStack30"></canvas></div>
            </div>

            <div class="row">
                <div class="col-md-6"><div class="card">
                    <h4 style="margin-top:0;">Sales by Marketplace</h4>
                    <div class="muted">Bars PLN, tooltip shows USD | PLN</div>
                    <div class="chart-wrap-sm"><canvas id="chartMarketplace"></canvas></div>
                </div></div>

                <div class="col-md-6"><div class="card">
                    <h4 style="margin-top:0;">Sales by Country</h4>
                    <div class="muted">Bars PLN, tooltip shows USD | PLN</div>
                    <div class="chart-wrap-sm"><canvas id="chartCountry"></canvas></div>
                </div></div>
            </div>
        </div>

        <!-- PRODUCT -->
        <div role="tabpanel" class="tab-pane" id="tabProducts">
            <!-- senin verdiğin Product bloğu aynen (kısaltmadım, birebir bıraktım) -->
            <h4 class="section-title">Product KPIs</h4>
            <div class="row">
                <div class="col-md-3"><div class="card">
                    <div class="card-title">Total Qty</div>
                    <div id="kpiProdTotalQty" class="card-value">-</div>
                    <div class="muted" id="kpiProdPartial">-</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Distinct Types</div>
                    <div id="kpiProdDistinctTypes" class="card-value">-</div>
                    <div class="muted">Selected period</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Unique SKUs</div>
                    <div id="kpiProdUniqueSkus" class="card-value">-</div>
                    <div class="muted">Selected period</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Top Type</div>
                    <div id="kpiProdTopType" class="card-value">-</div>
                    <div class="muted"><span id="kpiProdTopTypeQty">-</span> qty</div>
                </div></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Product Types (Selected Period)</h4>
                <div class="muted" style="margin-bottom:10px;">
                    Qty + Revenue + Unique SKU + Orders. <span class="pill" id="pillProdPartial">-</span>
                </div>
                <div class="table-responsive">
                    <table class="table table-striped" id="tblProductTypes">
                        <thead>
                        <tr>
                            <th style="width:24%;">Type</th>
                            <th class="text-right" style="width:10%;">Qty</th>
                            <th class="text-right" style="width:18%;">Revenue USD</th>
                            <th class="text-right" style="width:18%;">Revenue PLN</th>
                            <th class="text-right" style="width:12%;">Unique SKU</th>
                            <th class="text-right" style="width:12%;">Orders</th>
                        </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>

            <div class="row">
                <div class="col-md-7">
                    <div class="card">
                        <h4 style="margin-top:0;">Top SKUs + Pareto</h4>
                        <div class="muted" style="margin-bottom:10px;">
                            Bars: Revenue PLN • Line: Cumulative % (Pareto)
                        </div>
                        <div class="chart-wrap"><canvas id="chartSkuPareto"></canvas></div>
                    </div>
                </div>

                <div class="col-md-5">
                    <div class="card">
                        <h4 style="margin-top:0;">Top SKUs (Selected Period)</h4>
                        <div class="muted" style="margin-bottom:10px;">Sorted by Revenue PLN desc.</div>
                        <div class="table-responsive">
                            <table class="table table-striped" id="tblTopSkus">
                                <thead>
                                <tr>
                                    <th style="width:34%;">Type</th>
                                    <th>SKU</th>
                                    <th class="text-right" style="width:12%;">Qty</th>
                                    <th class="text-right" style="width:22%;">Rev USD</th>
                                    <th class="text-right" style="width:22%;">Rev PLN</th>
                                </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Product Type × Marketplace (Selected Period)</h4>
                <div class="muted">Stacked bars: Qty per ProductType split by Marketplace</div>
                <div class="chart-wrap"><canvas id="chartTypeMarketplace"></canvas></div>
            </div>

            <div class="card">
                <h4 style="margin-top:0;">Top Products</h4>
                <div class="muted" style="margin-bottom:10px;">Revenue shown as USD and PLN.</div>
                <div class="table-responsive">
                    <table class="table table-striped" id="tblTopProducts">
                        <thead>
                        <tr>
                            <th style="width:18%;">Type</th>
                            <th>SKU</th>
                            <th class="text-right" style="width:10%;">Qty</th>
                            <th class="text-right" style="width:18%;">Revenue USD</th>
                            <th class="text-right" style="width:18%;">Revenue PLN</th>
                        </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- CUSTOMERS -->
        <div role="tabpanel" class="tab-pane" id="tabCustomers">
            <h4 class="section-title">Customers</h4>

            <div class="row">
                <div class="col-md-3"><div class="card">
                    <div class="card-title">Unique Customers</div>
                    <div id="kpiCustUnique" class="card-value">-</div>
                    <div class="muted">Selected period</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">New Customers</div>
                    <div id="kpiCustNew" class="card-value">-</div>
                    <div class="muted">First order in selected period</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Returning Customers</div>
                    <div id="kpiCustReturning" class="card-value">-</div>
                    <div class="muted">Unique - New</div>
                </div></div>

                <div class="col-md-3"><div class="card">
                    <div class="card-title">Returning Rate</div>
                    <div id="kpiCustReturnRate" class="card-value">-</div>
                    <div class="muted">Returning / Unique</div>
                </div></div>
            </div>

            <div class="row">
                <div class="col-md-4"><div class="card">
                    <h4 style="margin-top:0;">New vs Returning</h4>
                    <div class="muted">Selected period</div>
                    <div class="chart-wrap-sm" style="height:260px;"><canvas id="chartNewReturning"></canvas></div>
                </div></div>

                <div class="col-md-8"><div class="card">
                    <h4 style="margin-top:0;">Top Repeat Customers</h4>
                    <div class="muted" style="margin-bottom:10px;">
                        Top 50 by revenue. <span class="pill" id="pillCustomersNote">-</span>
                    </div>

                    <div class="table-responsive">
                        <table class="table table-striped" id="tblRepeatCustomers">
                            <thead>
                            <tr>
                                <th>Customer</th>
                                <th>Email</th>
                                <th class="text-right" style="width:10%;">Orders</th>
                                <th class="text-right" style="width:18%;">Revenue USD</th>
                                <th class="text-right" style="width:18%;">Revenue PLN</th>
                                <th style="width:16%;">First</th>
                                <th style="width:16%;">Last</th>
                            </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div></div>
            </div>
        </div>

        <!-- COSTS -->
        <div role="tabpanel" class="tab-pane" id="tabCosts">
            <h4 class="section-title">Costs KPIs (Selected Period Orders)</h4>
            <div class="row">
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Shipping Total</div>
                    <div id="kpiShipPair" class="card-value">-</div>
                    <div class="muted">USD | PLN</div>
                </div></div>
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Extra Total</div>
                    <div id="kpiExtraPair" class="card-value">-</div>
                    <div class="muted">USD | PLN</div>
                </div></div>
                <div class="col-md-4"><div class="card">
                    <div class="card-title">Total Costs</div>
                    <div id="kpiCostPair" class="card-value">-</div>
                    <div class="muted">Shipping + Extra</div>
                </div></div>
            </div>

            <h4 class="section-title">Order Costs (Shipping + Extra)</h4>
            <div class="card">
                <div class="muted" style="margin-bottom:10px;">
                    Click <b>+</b> to view Shipping Legs and Extra Expenses detail under the order.
                </div>

                <div class="table-responsive">
                    <table class="table table-striped" id="tblOrderCosts">
                        <thead>
                        <tr>
                            <th style="width:56px;"></th>
                            <th style="width:160px;">OrderNumber</th>
                            <th style="width:120px;">OrderDate</th>
                            <th class="text-right" style="width:150px;">Shipping USD</th>
                            <th class="text-right" style="width:150px;">Shipping PLN</th>
                            <th class="text-right" style="width:150px;">Extra USD</th>
                            <th class="text-right" style="width:150px;">Extra PLN</th>
                            <th class="text-right" style="width:150px;">Total USD</th>
                            <th class="text-right" style="width:150px;">Total PLN</th>
                        </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- ✅ ACCOUNTING REPORTS (NEW) -->
        <div role="tabpanel" class="tab-pane" id="tabAccounting">
            <h4 class="section-title">Accounting Reports</h4>

            <div class="card">
                <div class="muted" style="margin-bottom:10px;">
                    Upload Amazon TXT report → LamaX will enrich Delivery Number from <code>T_ShippingLeg</code>
                    and NBP day-1 from <code>T_FxUsdPln</code>, then create the accounting Excel.
                </div>

                <!-- ✅ Change src if you placed the page elsewhere -->
                <iframe src="/Administrator/AmazonReportUpload.aspx"
                        style="width:100%;height:820px;border:0;border-radius:12px;"></iframe>
            </div>
        </div>

    </div>
</asp:Content>

<asp:Content ID="cScripts" ContentPlaceHolderID="ScriptsContent" runat="server">
<script>
    function showLoading(on) { $("#loading").toggle(!!on); }

    function moneyPL(x) { var n = Number(x || 0); if (isNaN(n)) n = 0; return n.toLocaleString('pl-PL', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); }
    function moneyUS(x) { var n = Number(x || 0); if (isNaN(n)) n = 0; return n.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }); }
    function pairUsdPln(usd, pln) { return moneyUS(usd) + " USD  |  " + moneyPL(pln) + " PLN"; }
    function safeText(s) { return (s === null || s === undefined) ? "" : String(s); }
    function escHtml(s) { s = safeText(s); return s.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#039;"); }

    var trendChart = null, marketplaceChart = null, countryChart = null, payMarketplaceChart = null, newReturningChart = null;
    var chartOrdersYear = null, chartOrdersMonth = null, chartOrdersWeek = null, chartOrdersMarketStack30 = null;
    var ordersMapChart = null, worldGeoCache = null;
    var skuParetoChart = null, typeMarketplaceChart = null;
    var costDetail = { ship: {}, extra: {} };

    $("#btnApply").click(function () { loadDashboard(); });

    // ✅ Kritik fix: Tab açılınca Chart.js resize (boş görünme problemi biter)
    $('#dashTabs a[data-toggle="tab"]').on('shown.bs.tab', function () {
        safeResizeAllCharts();
    });

    function safeResize(ch) {
        try {
            if (ch && typeof ch.resize === "function") ch.resize();
        } catch (e) { }
    }
    function safeResizeAllCharts() {
        safeResize(trendChart);
        safeResize(marketplaceChart);
        safeResize(countryChart);
        safeResize(payMarketplaceChart);
        safeResize(newReturningChart);
        safeResize(chartOrdersYear);
        safeResize(chartOrdersMonth);
        safeResize(chartOrdersWeek);
        safeResize(chartOrdersMarketStack30);
        safeResize(ordersMapChart);
        safeResize(skuParetoChart);
        safeResize(typeMarketplaceChart);
    }

    function buildComboBarLine(canvasId, labels, bars, line, barLabel, lineLabel, lineAxisTitle) {
        return new Chart(document.getElementById(canvasId), {
            data: {
                labels: labels, datasets: [
                    { type: 'bar', label: barLabel, data: bars, yAxisID: 'yBar' },
                    { type: 'line', label: lineLabel, data: line, tension: 0.25, yAxisID: 'yLine' }
                ]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false },
                plugins: { legend: { position: 'bottom' } },
                scales: {
                    yBar: { beginAtZero: true, title: { display: true, text: 'Orders' } },
                    yLine: { beginAtZero: true, position: 'right', grid: { drawOnChartArea: false }, title: { display: true, text: lineAxisTitle } }
                }
            }
        });
    }

    function renderHeatmap(hm) {
        var $tb = $("#tblHeatmap tbody"); $tb.empty();
        if (!hm || !hm.Weeks || hm.Weeks.length === 0) {
            $tb.append("<tr><td colspan='8' class='muted'>No data.</td></tr>");
            return;
        }
        hm.Weeks.forEach(function (w) {
            var tr = "<tr>";
            tr += "<td class='hm-week'>" + escHtml(w.WeekLabel || "-") + "</td>";
            var cells = (w.Cells || []);
            for (var i = 0; i < 7; i++) {
                var v = (cells[i] || 0);
                tr += "<td><div class='hm-cell'>" + v + "</div></td>";
            }
            tr += "</tr>";
            $tb.append(tr);
        });
    }

    function normCountryName(n) {
        n = (n || "").toString().trim();
        var k = n.toLowerCase();
        var map = {
            "usa": "United States of America",
            "united states": "United States of America",
            "u.s.": "United States of America",
            "u.s.a.": "United States of America",
            "uk": "United Kingdom",
            "england": "United Kingdom",
            "czechia": "Czech Republic",
            "türkiye": "Turkey",
            "turkiye": "Turkey"
        };
        return map[k] || n;
    }

    async function getWorldFeatures() {
        if (worldGeoCache) return worldGeoCache;
        var url = "https://cdn.jsdelivr.net/npm/world-atlas@2/countries-50m.json";
        var resp = await fetch(url);
        var topo = await resp.json();
        var countries = ChartGeo.topojson.feature(topo, topo.objects.countries).features;
        worldGeoCache = countries;
        return countries;
    }

    async function renderOrdersMap(countryRows) {
        try {
            var rows = (countryRows || []).slice();
            var dict = {}, total = 0;
            rows.forEach(function (x) {
                var name = normCountryName(x.Country || "");
                var cnt = Number(x.OrdersCount || 0);
                if (!name) return;
                dict[name] = (dict[name] || 0) + cnt;
                total += cnt;
            });

            var features = await getWorldFeatures();
            var data = features.map(function (f) {
                var nm = (f && f.properties && f.properties.name) ? f.properties.name : "";
                return { feature: f, value: (dict[nm] || 0) };
            });

            var matched = 0;
            Object.keys(dict).forEach(function (k) {
                var ok = features.some(function (f) { return (f.properties && f.properties.name) === k; });
                if (ok) matched++;
            });
            $("#mapNote").text("Map: " + matched + " country name matched • Total orders: " + total);

            if (ordersMapChart) ordersMapChart.destroy();

            ordersMapChart = new Chart(document.getElementById("chartOrdersMap"), {
                type: "choropleth",
                data: { labels: features.map(f => f.properties.name), datasets: [{ label: "Orders", data: data, outline: features }] },
                options: {
                    responsive: true, maintainAspectRatio: false,
                    plugins: {
                        legend: { display: false },
                        tooltip: {
                            callbacks: {
                                label: function (ctx) {
                                    var v = (ctx.raw && ctx.raw.value) ? ctx.raw.value : 0;
                                    var nm = (ctx.raw && ctx.raw.feature && ctx.raw.feature.properties) ? ctx.raw.feature.properties.name : "";
                                    return (nm || "-") + ": " + v + " orders";
                                }
                            }
                        }
                    },
                    scales: {
                        projection: { axis: "x", projection: "equalEarth" },
                        color: { axis: "x", quantize: 6, legend: { position: "bottom", align: "right" } }
                    }
                }
            });
        } catch (e) {
            $("#mapNote").text("Map error: " + (e && e.message ? e.message : e));
            try { if (ordersMapChart) ordersMapChart.destroy(); } catch (_) { }
        }
    }

    // ✅ Cost expand (aynı)
    $(document).on("click", ".btnToggleCost", function () {
        var $btn = $(this);
        var order = $btn.data("order");
        var rowId = "det_" + order;

        var $existing = $("#" + rowId);
        if ($existing.length) {
            $existing.find(".collapse").collapse("toggle");
            return;
        }

        var shipRows = (costDetail.ship[order] || []);
        var extraRows = (costDetail.extra[order] || []);

        var shipHtml = "";
        if (shipRows.length === 0) {
            shipHtml = "<div class='muted'>No shipping legs.</div>";
        } else {
            shipHtml += "<div class='table-responsive'><table class='table table-condensed table-striped'>";
            shipHtml += "<thead><tr><th style='width:120px;'>ShipDate</th><th>Company</th><th>Tracking</th><th style='width:90px;'>LegType</th><th class='text-right' style='width:130px;'>Price USD</th><th class='text-right' style='width:130px;'>Price PLN</th></tr></thead><tbody>";
            shipRows.forEach(function (x) {
                shipHtml += "<tr><td>" + escHtml(x.ShipDateText || "-") + "</td><td>" + escHtml(x.Company || "-") + "</td><td>" + escHtml(x.Tracking || "") + "</td><td>" + escHtml(x.LegTypeText || "") + "</td><td class='text-right'>" + moneyUS(x.PriceUsd || 0) + "</td><td class='text-right'>" + moneyPL(x.PricePln || 0) + "</td></tr>";
            });
            shipHtml += "</tbody></table></div>";
        }

        var extraHtml = "";
        if (extraRows.length === 0) {
            extraHtml = "<div class='muted'>No extra expenses.</div>";
        } else {
            extraHtml += "<div class='table-responsive'><table class='table table-condensed table-striped'>";
            extraHtml += "<thead><tr><th style='width:140px;'>Date</th><th>Description / Type</th><th class='text-right' style='width:130px;'>Amount USD</th><th class='text-right' style='width:130px;'>Amount PLN</th></tr></thead><tbody>";
            extraRows.forEach(function (x) {
                extraHtml += "<tr><td>" + escHtml(x.ExpenseDateText || "-") + "</td><td>" + escHtml(x.Description || "-") + "</td><td class='text-right'>" + moneyUS(x.AmountUsd || 0) + "</td><td class='text-right'>" + moneyPL(x.AmountPln || 0) + "</td></tr>";
            });
            extraHtml += "</tbody></table></div>";
        }

        var $tr = $btn.closest("tr");
        var html = "<tr id='" + rowId + "'><td colspan='9' style='padding:0 8px 12px 8px; border-top:0;'><div class='collapse in' id='collapse_" + order + "'><div style='background:#fbfbfb;border:1px solid #eee;border-radius:10px;padding:12px;'><h5 style='margin:8px 0;font-weight:900;'>Shipping Legs</h5>" + shipHtml + "<h5 style='margin:8px 0;font-weight:900;'>Extra Expenses</h5>" + extraHtml + "</div></div></td></tr>";
        $tr.after(html);

        var $col = $("#collapse_" + order);
        $col.on("shown.bs.collapse", function () { $btn.text("-"); });
        $col.on("hidden.bs.collapse", function () { $btn.text("+"); });
        $btn.text("-");
    });

    function loadDashboard() {
        var p = $("#ddlPeriod").val();
        var f = $("#txtFrom").val();
        var t = $("#txtTo").val();

        showLoading(true);

        $.ajax({
            type: "GET",
            url: "/Administrator/DashboardApi.ashx",
            data: { action: "getall", period: p, fromDate: f, toDate: t },
            dataType: "json",
            success: function (d) {

                var fx = d.Fx || {};
                if (fx.UsdPlnRate) {
                    $("#lblFxNote").text("USD/PLN: " + moneyPL(fx.UsdPlnRate) + " (rate date: " + (fx.RateDate || "-") + ")");
                    $("#pillCustomersNote").text("USDPLN " + moneyPL(fx.UsdPlnRate) + " • " + (fx.RateDate || "-"));
                } else {
                    $("#lblFxNote").text("USD/PLN: - (rate date: -)");
                    $("#pillCustomersNote").text("-");
                }

                var k = d.Kpi || {};
                $("#kpiRevenuePair").text(pairUsdPln(k.RevenueOriginal || 0, k.RevenuePLN || 0));
                $("#kpiOrders").text(k.OrdersCount || 0);
                $("#kpiItems").text(k.ItemsCount || 0);
                $("#kpiAovPair").text(pairUsdPln(k.AOV_Original || 0, k.AOV_PLN || 0));

                var pk = d.PaymentsKpi || {};
                $("#kpiPayYear").text(pairUsdPln(pk.YearTotalUSD || 0, pk.YearTotalPLN || 0));
                $("#kpiPayMonth").text(pairUsdPln(pk.MonthTotalUSD || 0, pk.MonthTotalPLN || 0));
                $("#kpiPayPeriod").text(pairUsdPln(pk.PeriodTotalUSD || 0, pk.PeriodTotalPLN || 0));

                // Payments list
                var pTb = $("#tblPaymentsList tbody"); pTb.empty();
                (d.PaymentsList || []).forEach(function (x) {
                    pTb.append("<tr><td>" + safeText(x.PayDateText || "-") + "</td><td>" + safeText(x.Marketplace || "-") + "</td><td class='text-right'>" + moneyUS(x.AmountUSD || 0) + "</td><td class='text-right'>" + moneyPL(x.AmountPLN || 0) + "</td><td>" + safeText(x.CreatedAtText || "") + "</td></tr>");
                });

                // Top Products table
                var tb = $("#tblTopProducts tbody"); tb.empty();
                (d.TopProducts || []).forEach(function (x) {
                    tb.append("<tr><td>" + safeText(x.ProductType || "-") + "</td><td>" + safeText(x.SKU || "") + "</td><td class='text-right'>" + (x.Qty || 0) + "</td><td class='text-right'>" + moneyUS(x.RevenueOriginal || 0) + "</td><td class='text-right'>" + moneyPL(x.RevenuePLN || 0) + "</td></tr>");
                });

                // Trend
                var tr = d.Trend || [];
                var tLabels = tr.map(x => x.Label);
                var tRevUsd = tr.map(x => x.RevenueOriginal);
                var tRevPln = tr.map(x => x.RevenuePLN);
                var tOrders = tr.map(x => x.OrdersCount);

                if (trendChart) trendChart.destroy();
                trendChart = new Chart(document.getElementById("chartTrend"), {
                    data: {
                        labels: tLabels, datasets: [
                            { type: 'line', label: 'Revenue (USD)', data: tRevUsd, tension: 0.25, yAxisID: 'yUsd' },
                            { type: 'line', label: 'Revenue (PLN)', data: tRevPln, tension: 0.25, yAxisID: 'yPln' },
                            { type: 'bar', label: 'Orders', data: tOrders, yAxisID: 'yOrders' }
                        ]
                    },
                    options: {
                        responsive: true, maintainAspectRatio: false,
                        interaction: { mode: 'index', intersect: false },
                        scales: {
                            yUsd: { position: 'left', title: { display: true, text: 'USD' }, beginAtZero: true },
                            yPln: { position: 'right', grid: { drawOnChartArea: false }, title: { display: true, text: 'PLN' }, beginAtZero: true },
                            yOrders: { display: false, beginAtZero: true }
                        }
                    }
                });

                // Sales by marketplace
                var m = d.Marketplace || [];
                if (marketplaceChart) marketplaceChart.destroy();
                marketplaceChart = new Chart(document.getElementById("chartMarketplace"), {
                    type: 'bar',
                    data: { labels: m.map(x => x.Marketplace), datasets: [{ label: 'Sales (PLN)', data: m.map(x => x.RevenuePLN) }] },
                    options: {
                        indexAxis: 'y', responsive: true, maintainAspectRatio: false,
                        plugins: { legend: { display: false }, tooltip: { callbacks: { label: (ctx) => pairUsdPln((m[ctx.dataIndex] || {}).RevenueOriginal || 0, (m[ctx.dataIndex] || {}).RevenuePLN || 0) } } }
                    }
                });

                // Sales by country (+ OrdersCount for map)
                var c = d.Country || [];
                if (countryChart) countryChart.destroy();
                countryChart = new Chart(document.getElementById("chartCountry"), {
                    type: 'bar',
                    data: { labels: c.map(x => x.Country), datasets: [{ label: 'Sales (PLN)', data: c.map(x => x.RevenuePLN) }] },
                    options: {
                        indexAxis: 'y', responsive: true, maintainAspectRatio: false,
                        plugins: { legend: { display: false }, tooltip: { callbacks: { label: (ctx) => pairUsdPln((c[ctx.dataIndex] || {}).RevenueOriginal || 0, (c[ctx.dataIndex] || {}).RevenuePLN || 0) } } }
                    }
                });

                // Map
                renderOrdersMap(c);

                // Payments by marketplace
                var pm = d.PaymentsMarketplace || [];
                if (payMarketplaceChart) payMarketplaceChart.destroy();
                payMarketplaceChart = new Chart(document.getElementById("chartPayMarketplace"), {
                    type: 'bar',
                    data: { labels: pm.map(x => x.Marketplace), datasets: [{ label: 'Payments (PLN)', data: pm.map(x => x.TotalPLN) }] },
                    options: {
                        indexAxis: 'y', responsive: true, maintainAspectRatio: false,
                        plugins: { legend: { display: false }, tooltip: { callbacks: { label: (ctx) => pairUsdPln((pm[ctx.dataIndex] || {}).TotalUSD || 0, (pm[ctx.dataIndex] || {}).TotalPLN || 0) } } }
                    }
                });

                // Customers
                var ck = d.CustomerKpi || {};
                $("#kpiCustUnique").text(ck.UniqueCustomers || 0);
                $("#kpiCustNew").text(ck.NewCustomers || 0);
                $("#kpiCustReturning").text(ck.ReturningCustomers || 0);
                $("#kpiCustReturnRate").text((Number(ck.ReturningRatePct || 0).toFixed(1)) + "%");

                var nv = [ck.NewCustomers || 0, ck.ReturningCustomers || 0];
                if (newReturningChart) newReturningChart.destroy();
                newReturningChart = new Chart(document.getElementById("chartNewReturning"), {
                    type: 'doughnut',
                    data: { labels: ['New', 'Returning'], datasets: [{ data: nv }] },
                    options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
                });

                var rc = d.RepeatCustomers || [];
                var tbc = $("#tblRepeatCustomers tbody"); tbc.empty();
                rc.forEach(function (x) {
                    tbc.append("<tr><td>" + safeText(x.BuyerFullName || x.CustomerKey || "-") + "</td><td>" + safeText(x.Email || "") + "</td><td class='text-right'>" + (x.OrdersCount || 0) + "</td><td class='text-right'>" + moneyUS(x.RevenueOriginal || 0) + "</td><td class='text-right'>" + moneyPL(x.RevenuePLN || 0) + "</td><td>" + safeText(x.FirstOrderText || "-") + "</td><td>" + safeText(x.LastOrderText || "-") + "</td></tr>");
                });

                // Costs
                var ckpi = d.CostsKpi || {};
                $("#kpiShipPair").text(pairUsdPln(ckpi.ShippingUSD || 0, ckpi.ShippingPLN || 0));
                $("#kpiExtraPair").text(pairUsdPln(ckpi.ExtraUSD || 0, ckpi.ExtraPLN || 0));
                $("#kpiCostPair").text(pairUsdPln(ckpi.TotalUSD || 0, ckpi.TotalPLN || 0));

                costDetail.ship = {}; (d.ShippingLegs || []).forEach(function (x) { var key = x.OrderNumber || ""; (costDetail.ship[key] = costDetail.ship[key] || []).push(x); });
                costDetail.extra = {}; (d.ExtraExpenses || []).forEach(function (x) { var key = x.OrderNumber || ""; (costDetail.extra[key] = costDetail.extra[key] || []).push(x); });

                var ctb = $("#tblOrderCosts tbody"); ctb.empty();
                (d.OrderCosts || []).forEach(function (x) {
                    var order = safeText(x.OrderNumber || "");
                    var hasShip = (costDetail.ship[order] && costDetail.ship[order].length > 0);
                    var hasExtra = (costDetail.extra[order] && costDetail.extra[order].length > 0);
                    var canExpand = (hasShip || hasExtra);

                    ctb.append(
                        "<tr>" +
                        "<td>" + (canExpand ? ("<button type='button' class='btn btn-default btn-xs btnToggleCost' data-order='" + escHtml(order) + "'>+</button>") : "") + "</td>" +
                        "<td>" + escHtml(order || "-") + "</td>" +
                        "<td>" + escHtml(x.OrderDateText || "-") + "</td>" +
                        "<td class='text-right'>" + moneyUS(x.ShippingUsd || 0) + "</td>" +
                        "<td class='text-right'>" + moneyPL(x.ShippingPln || 0) + "</td>" +
                        "<td class='text-right'>" + moneyUS(x.ExtraUsd || 0) + "</td>" +
                        "<td class='text-right'>" + moneyPL(x.ExtraPln || 0) + "</td>" +
                        "<td class='text-right'>" + moneyUS(x.TotalUsd || 0) + "</td>" +
                        "<td class='text-right'>" + moneyPL(x.TotalPln || 0) + "</td>" +
                        "</tr>"
                    );
                });

                // Orders analytics
                var oa = d.OrdersAnalytics || {};
                $("#kpiOrdersSelTotal").text(oa.TotalOrders || 0);
                $("#kpiOrdersSelTotalSub").text(pairUsdPln(oa.TotalRevenueUsd || 0, oa.TotalRevenuePln || 0));
                $("#kpiOrdersSelAvg").text((Number(oa.AvgPerDay || 0)).toFixed(2));
                $("#kpiOrdersSelPeakDay").text(oa.PeakDayText || "-");
                $("#kpiOrdersSelPeakCount").text(oa.PeakDayCount || 0);

                $("#kpiTopCountryName").text(oa.TopCountryName || "-");
                $("#kpiTopCountryCount").text(oa.TopCountryOrders || 0);

                renderHeatmap(oa.Heatmap35);

                var yy = oa.Yearly || [];
                if (chartOrdersYear) chartOrdersYear.destroy();
                chartOrdersYear = buildComboBarLine("chartOrdersYear", yy.map(x => x.Label), yy.map(x => x.OrdersCount), yy.map(x => x.RevenuePLN), "Orders", "Revenue (PLN)", "PLN");

                var mm = oa.Monthly || [];
                if (chartOrdersMonth) chartOrdersMonth.destroy();
                chartOrdersMonth = buildComboBarLine("chartOrdersMonth", mm.map(x => x.Label), mm.map(x => x.OrdersCount), mm.map(x => x.AOV_PLN), "Orders", "AOV (PLN)", "PLN");

                var ww = oa.Weekly || [];
                if (chartOrdersWeek) chartOrdersWeek.destroy();
                chartOrdersWeek = buildComboBarLine("chartOrdersWeek", ww.map(x => x.Label), ww.map(x => x.OrdersCount), ww.map(x => x.RevenuePLN), "Orders", "Revenue (PLN)", "PLN");

                var stack = oa.MarketStack30 || {};
                var stackLabels = stack.Labels || [];
                var datasets = (stack.Datasets || []).map(ds => ({ label: ds.Label, data: ds.Data || [], stack: 'mkt' }));

                if (chartOrdersMarketStack30) chartOrdersMarketStack30.destroy();
                chartOrdersMarketStack30 = new Chart(document.getElementById("chartOrdersMarketStack30"), {
                    type: 'bar',
                    data: { labels: stackLabels, datasets: datasets },
                    options: { responsive: true, maintainAspectRatio: false, interaction: { mode: 'index', intersect: false }, plugins: { legend: { position: 'bottom' } }, scales: { x: { stacked: true }, y: { stacked: true, beginAtZero: true } } }
                });

                // Product analytics
                var pa = d.ProductAnalytics || {};
                $("#kpiProdTotalQty").text(pa.TotalQty || 0);
                $("#kpiProdDistinctTypes").text(pa.DistinctTypes || 0);
                $("#kpiProdUniqueSkus").text(pa.UniqueSkus || 0);
                $("#kpiProdTopType").text(pa.TopTypeName || "-");
                $("#kpiProdTopTypeQty").text(pa.TopTypeQty || 0);

                var partialText = (pa.IsPartial ? "Partial data (fallback)" : "Full data");
                $("#kpiProdPartial").text(partialText);
                $("#pillProdPartial").text(partialText);

                var ptb = $("#tblProductTypes tbody"); ptb.empty();
                var ts = (pa.TypeSummary || []);
                if (!ts || ts.length === 0) {
                    ptb.append("<tr><td colspan='6' class='muted'>No data.</td></tr>");
                } else {
                    ts.forEach(function (x) {
                        ptb.append("<tr><td>" + safeText(x.ProductType || "-") + "</td><td class='text-right'>" + (x.Qty || 0) + "</td><td class='text-right'>" + moneyUS(x.RevenueOriginal || 0) + "</td><td class='text-right'>" + moneyPL(x.RevenuePLN || 0) + "</td><td class='text-right'>" + (x.UniqueSkus || 0) + "</td><td class='text-right'>" + (x.OrdersCount || 0) + "</td></tr>");
                    });
                }

                var topSkus = (pa.TopSkus || []);
                var skuTb = $("#tblTopSkus tbody"); skuTb.empty();
                if (!topSkus || topSkus.length === 0) {
                    skuTb.append("<tr><td colspan='5' class='muted'>No data.</td></tr>");
                } else {
                    topSkus.forEach(function (x) {
                        skuTb.append("<tr><td>" + safeText(x.ProductType || "-") + "</td><td>" + safeText(x.SKU || "") + "</td><td class='text-right'>" + (x.Qty || 0) + "</td><td class='text-right'>" + moneyUS(x.RevenueOriginal || 0) + "</td><td class='text-right'>" + moneyPL(x.RevenuePLN || 0) + "</td></tr>");
                    });
                }

                var pareto = pa.Pareto || {};
                var pLabels = (pareto.Labels || []);
                var pRev = (pareto.RevenuePLN || []);
                var pCum = (pareto.CumPct || []);

                if (skuParetoChart) skuParetoChart.destroy();
                skuParetoChart = new Chart(document.getElementById("chartSkuPareto"), {
                    data: {
                        labels: pLabels, datasets: [
                            { type: 'bar', label: 'Revenue (PLN)', data: pRev, yAxisID: 'yRev' },
                            { type: 'line', label: 'Cumulative %', data: pCum, yAxisID: 'yPct', tension: 0.25 }
                        ]
                    },
                    options: {
                        responsive: true, maintainAspectRatio: false,
                        interaction: { mode: 'index', intersect: false },
                        plugins: {
                            legend: { position: 'bottom' }, tooltip: {
                                callbacks: {
                                    label: function (ctx) {
                                        if (ctx.dataset.yAxisID === 'yPct') return "Cum: " + Number(ctx.raw || 0).toFixed(1) + "%";
                                        return "Rev: " + moneyPL(ctx.raw || 0) + " PLN";
                                    }
                                }
                            }
                        },
                        scales: {
                            yRev: { beginAtZero: true, title: { display: true, text: 'PLN' } },
                            yPct: { beginAtZero: true, position: 'right', min: 0, max: 100, grid: { drawOnChartArea: false }, title: { display: true, text: '%' } }
                        }
                    }
                });

                var tm = pa.TypeMarketplace || {};
                var tmLabels = (tm.Labels || []);
                var tmDatasets = (tm.Datasets || []).map(function (ds) { return { label: ds.Label, data: ds.Data || [], stack: 'mkt' }; });

                if (typeMarketplaceChart) typeMarketplaceChart.destroy();
                typeMarketplaceChart = new Chart(document.getElementById("chartTypeMarketplace"), {
                    type: 'bar',
                    data: { labels: tmLabels, datasets: tmDatasets },
                    options: {
                        responsive: true, maintainAspectRatio: false, interaction: { mode: 'index', intersect: false }, plugins: { legend: { position: 'bottom' } },
                        scales: { x: { stacked: true }, y: { stacked: true, beginAtZero: true, title: { display: true, text: 'Qty' } } }
                    }
                });

                // ✅ En sonda: ilk load’da aktif olmayan tablar için de ölçüleri düzelt
                setTimeout(safeResizeAllCharts, 50);
            },
            error: function (xhr) {
                var msg = "ERROR: " + xhr.status;
                try { if (xhr.responseText) msg += "\n" + xhr.responseText; } catch (e) { }
                alert(msg);
            },
            complete: function () { showLoading(false); }
        });
    }

    $(document).ready(function () {
        loadDashboard();
    });
</script>
</asp:Content>