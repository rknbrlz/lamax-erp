<%@ Page Title="Menu for Admin" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="MenuforAdmin.aspx.cs" Inherits="Feniks.Admin.MenuforAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <!-- Bootstrap 3 -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>

    <style>
        body { background:#f5f6f8; }

        /* ✅ Sarı alandaki üst boşluğu azalt */
        .page-wrap { padding-top: 0px; padding-bottom: 25px; }
        .topbar { margin-bottom: 12px; margin-top: 10px; }

        .rate-badge {
            background:#fff; border:1px solid #e5e5e5; color:#666; font-weight:700;
            padding:8px 10px; display:inline-block; margin-left:8px; border-radius:14px;
        }
        .rate-badge .v { color:#999; font-weight:700; }

        .hero {
            background:#fff; border:1px solid #e6e6e6; border-radius:12px;
            padding:18px 18px; margin-top: 10px; margin-bottom: 15px;
            box-shadow: 0 1px 2px rgba(0,0,0,.03);
        }
        .hero h2 { margin:0; font-size:22px; color:#666; font-weight:700; }
        .hero small { color:#999; }

        .menu-card {
            background:#fff; border:1px solid #e6e6e6; border-radius:12px;
            padding:18px; margin-bottom: 15px;
            box-shadow: 0 1px 2px rgba(0,0,0,.03);
            transition: transform .12s ease, box-shadow .12s ease;
            min-height: 170px;
        }
        .menu-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 18px rgba(0,0,0,.06);
        }
        .menu-title { font-weight:700; color:#333; margin-top: 0; }
        .menu-desc { color:#888; font-size: 12px; margin-bottom: 12px; }
        .menu-ico { font-size:38px; }
        .menu-btn { margin-top: 12px; }

        .alert-soft {
            border-radius:12px; border:1px solid #f0e1a6;
            background: #fff8d9;
        }
        .alert-soft .glyphicon { margin-right: 6px; }

        /* FX small grid */
        .fx-grid .cell { margin-bottom: 10px; }
        .fx-grid small { color:#999; }
        .fx-grid strong { font-size:16px; color:#333; }
        .fx-meta { font-size:12px; color:#999; margin-top: 6px; }

        /* Counter card */
        .kpi-row { margin-top: 6px; }
        .kpi-item { padding-right: 8px; }
        .kpi-label { color:#999; font-size:12px; text-transform:uppercase; letter-spacing:.4px; }
        .kpi-value { font-size:40px; font-weight:800; color:#333; line-height:1.1; }
        .kpi-sub { color:#999; font-size:12px; margin-top: 8px; }
        @media (max-width: 767px){
            .kpi-value { font-size:36px; }
        }

        /* GridView tweaks */
        .table.table-condensed td, .table.table-condensed th { font-size: 12px; }
    </style>

    <div class="container-fluid page-wrap">

        <!-- TOP BAR -->
        <div class="row topbar">
            <div class="col-xs-12 text-right">
                <span class="rate-badge">USD/PLN: <span class="v"><asp:Label ID="lblUSD" runat="server" /></span></span>
                <span class="rate-badge">EUR/PLN: <span class="v"><asp:Label ID="lblEUR" runat="server" /></span></span>
                <span class="rate-badge">CAD/PLN: <span class="v"><asp:Label ID="lblCAD" runat="server" /></span></span>
                <span class="rate-badge">PLN/TRY: <span class="v"><asp:Label ID="lblPLN" runat="server" /></span></span>
            </div>
        </div>

        <!-- ✅ ALERT (KALDIRILDI) -->
        <div id="divInfo" runat="server" visible="false" class="row" style="display:none;">
            <div class="col-xs-12">
                <div class="alert alert-soft">
                    <strong>
                        <i class="glyphicon glyphicon-alert" style="color:#f0ad4e;"></i>
                        Pending orders in action:
                    </strong>
                    <asp:Label ID="lblOpenQty" runat="server" Font-Bold="True"></asp:Label>
                </div>
            </div>
        </div>

        <!-- ✅ HERO (KALDIRILDI) -->
        <asp:Panel ID="pnlHero" runat="server" Visible="false">
            <div class="row">
                <div class="col-xs-12">
                    <div class="hero">
                        <h2>Welcome Hgerman! <small>Admin Panel</small></h2>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- ROW 1 -->
        <div class="row">

            <!-- ORDER COUNTERS -->
            <div class="col-sm-6 col-md-3">
                <div class="menu-card">
                    <div class="clearfix">
                        <div class="pull-left">
                            <h4 class="menu-title">Orders Counter</h4>
                            <div class="menu-desc">All-time totals (today included)</div>
                        </div>
                        <div class="pull-right text-primary menu-ico">
                            <i class="glyphicon glyphicon-dashboard"></i>
                        </div>
                    </div>

                    <div class="row kpi-row">
                        <div class="col-xs-6 kpi-item">
                            <div class="kpi-label">Total Orders</div>
                            <div class="kpi-value">
                                <asp:Label ID="lblTotalOrders" runat="server" Text="0" />
                            </div>
                        </div>
                        <div class="col-xs-6 kpi-item">
                            <div class="kpi-label">Total Qty</div>
                            <div class="kpi-value">
                                <asp:Label ID="lblTotalQty" runat="server" Text="0" />
                            </div>
                        </div>
                    </div>

                    <div class="kpi-sub">
                        Updated: <asp:Label ID="lblOrdersCounterUpdated" runat="server" Text="-" />
                    </div>

                    <asp:HiddenField ID="hfTotalOrders" runat="server" Value="0" />
                    <asp:HiddenField ID="hfTotalQty" runat="server" Value="0" />
                </div>
            </div>

            <!-- ✅ CURRENCY CONVERTER CARD (ortalanmış) -->
            <div class="col-sm-6 col-md-3">
                <div class="menu-card">
                    <div class="clearfix">
                        <div class="pull-left">
                            <h4 class="menu-title">Currency Converter</h4>
                            <div class="menu-desc">USD • PLN • EUR • TRY • CAD</div>
                        </div>
                        <div class="pull-right text-info menu-ico">
                            <i class="glyphicon glyphicon-random"></i>
                        </div>
                    </div>

                    <div class="form-group" style="margin-top:8px;">
                        <label style="font-size:12px;color:#999;margin-bottom:4px;">Amount</label>
                        <asp:TextBox ID="txtFxAmount" runat="server" CssClass="form-control"
                            placeholder="e.g. 125.50" />
                        <asp:RequiredFieldValidator ID="rfvFxAmount" runat="server"
                            ControlToValidate="txtFxAmount"
                            ErrorMessage="Amount required"
                            Display="Dynamic" ForeColor="#b30000" Font-Size="11px"
                            ValidationGroup="FxGroup" />
                    </div>

                    <div class="row">
                        <div class="col-xs-6">
                            <label style="font-size:12px;color:#999;margin-bottom:4px;">From</label>
                            <asp:DropDownList ID="ddlFxFrom" runat="server" CssClass="form-control">
                                <asp:ListItem Text="USD" Value="USD" />
                                <asp:ListItem Text="PLN" Value="PLN" />
                                <asp:ListItem Text="EUR" Value="EUR" />
                                <asp:ListItem Text="TRY" Value="TRY" />
                                <asp:ListItem Text="CAD" Value="CAD" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-xs-6">
                            <label style="font-size:12px;color:#999;margin-bottom:4px;">To</label>
                            <asp:DropDownList ID="ddlFxTo" runat="server" CssClass="form-control">
                                <asp:ListItem Text="PLN" Value="PLN" />
                                <asp:ListItem Text="USD" Value="USD" />
                                <asp:ListItem Text="EUR" Value="EUR" />
                                <asp:ListItem Text="TRY" Value="TRY" />
                                <asp:ListItem Text="CAD" Value="CAD" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <asp:Button ID="btnFxConvert" runat="server" Text="Convert"
                        CssClass="btn btn-default btn-block menu-btn" Style="margin-top:10px;"
                        OnClick="btnFxConvert_Click"
                        ValidationGroup="FxGroup" />

                    <div style="margin-top:8px;">
                        <asp:Label ID="lblFxConvertResult" runat="server" Text="-" Font-Bold="true" />
                        <div class="fx-meta">
                            Rate date: <asp:Label ID="lblFxConvertRateDate" runat="server" Text="-" />
                        </div>
                        <asp:Label ID="lblFxConvertMsg" runat="server" Visible="false"
                            Style="display:block;margin-top:6px;font-size:12px;" />
                    </div>
                </div>
            </div>

            <!-- ✅ FX RATES CARD (NBP) -> Stock yerine taşındı -->
            <div class="col-sm-6 col-md-3">
                <div class="menu-card">
                    <div class="clearfix">
                        <div class="pull-left">
                            <h4 class="menu-title">FX Rates (NBP)</h4>
                            <div class="menu-desc">USDPLN • EURPLN • GBPPLN • TRYPLN</div>
                        </div>
                        <div class="pull-right text-warning menu-ico">
                            <i class="glyphicon glyphicon-transfer"></i>
                        </div>
                    </div>

                    <div class="row fx-grid" style="margin-top:4px;">
                        <div class="col-xs-6 cell">
                            <small>USDPLN</small><br />
                            <strong><asp:Label ID="lblUsdPln" runat="server" Text="-" /></strong>
                        </div>
                        <div class="col-xs-6 cell">
                            <small>EURPLN</small><br />
                            <strong><asp:Label ID="lblEurPln" runat="server" Text="-" /></strong>
                        </div>
                        <div class="col-xs-6 cell">
                            <small>GBPPLN</small><br />
                            <strong><asp:Label ID="lblGbpPln" runat="server" Text="-" /></strong>
                        </div>
                        <div class="col-xs-6 cell">
                            <small>TRYPLN</small><br />
                            <strong><asp:Label ID="lblTryPln" runat="server" Text="-" /></strong>
                        </div>
                    </div>

                    <div class="fx-meta">
                        Date: <asp:Label ID="lblFxDate" runat="server" Text="-" />
                        &nbsp;•&nbsp;
                        Updated: <asp:Label ID="lblFxUpdated" runat="server" Text="-" />
                    </div>

                    <asp:Button ID="btnUpdateFx" runat="server" Text="Update from NBP"
                        CssClass="btn btn-default btn-block menu-btn"
                        OnClick="btnUpdateFx_Click"
                        CausesValidation="false" />

                    <asp:Label ID="lblFxMsg" runat="server" Visible="false"
                        Style="display:block;margin-top:8px;font-size:12px;" />
                </div>
            </div>

            <!-- PAYMENTS CARD -->
            <div class="col-sm-6 col-md-3">
                <div class="menu-card">
                    <div class="clearfix">
                        <div class="pull-left">
                            <h4 class="menu-title">Payments</h4>
                            <div class="menu-desc">Amazon • Etsy • eBay • Website</div>
                        </div>
                        <div class="pull-right text-success menu-ico">
                            <i class="glyphicon glyphicon-credit-card"></i>
                        </div>
                    </div>

                    <div class="form-group" style="margin-top:8px;">
                        <label style="font-size:12px;color:#999;margin-bottom:4px;">Marketplace</label>
                        <asp:DropDownList ID="ddlPayMarketplace" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Amazon" Value="Amazon" />
                            <asp:ListItem Text="Etsy" Value="Etsy" />
                            <asp:ListItem Text="eBay" Value="eBay" />
                            <asp:ListItem Text="Website" Value="Website" />
                        </asp:DropDownList>
                    </div>

                    <div class="row">
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label style="font-size:12px;color:#999;margin-bottom:4px;">Date</label>
                                <asp:TextBox ID="txtPayDate" runat="server" CssClass="form-control" placeholder="yyyy-mm-dd" />
                                <asp:RequiredFieldValidator ID="rfvPayDate" runat="server"
                                    ControlToValidate="txtPayDate" ErrorMessage="Date required"
                                    Display="Dynamic" ForeColor="#b30000" Font-Size="11px"
                                    ValidationGroup="PayGroup" />
                            </div>
                        </div>
                        <div class="col-xs-6">
                            <div class="form-group">
                                <label style="font-size:12px;color:#999;margin-bottom:4px;">Amount</label>
                                <asp:TextBox ID="txtPayAmount" runat="server" CssClass="form-control" placeholder="e.g. 1250.50" />
                                <asp:RequiredFieldValidator ID="rfvPayAmount" runat="server"
                                    ControlToValidate="txtPayAmount" ErrorMessage="Amount required"
                                    Display="Dynamic" ForeColor="#b30000" Font-Size="11px"
                                    ValidationGroup="PayGroup" />
                            </div>
                        </div>
                    </div>

                    <asp:Button ID="btnPayAdd" runat="server" Text="Add Payment"
                        CssClass="btn btn-default btn-block menu-btn"
                        OnClick="btnPayAdd_ServerClick"
                        ValidationGroup="PayGroup" />

                    <asp:Label ID="lblPayMsg" runat="server" Visible="false"
                        Style="display:block;margin-top:8px;font-size:12px;" />

                    <div style="margin-top:10px;">
                        <asp:GridView ID="gvPayments" runat="server"
                            CssClass="table table-condensed table-striped"
                            AutoGenerateColumns="False" GridLines="None" ShowHeader="true">
                            <Columns>
                                <asp:BoundField DataField="PayDate" HeaderText="Date" />
                                <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" />
                                <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" HtmlEncode="false" />
                            </Columns>
                        </asp:GridView>

                        <div class="fx-meta">
                            Last update: <asp:Label ID="lblPayUpdated" runat="server" Text="-" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <!-- ROW 2 -->
        <div class="row">

            <!-- ✅ Dashboard & Reports (KALDIRILDI) -->
            <asp:Panel ID="pnlDashboardReports" runat="server" Visible="false">
                <div class="col-sm-6 col-md-3">
                    <div class="menu-card">
                        <div class="clearfix">
                            <div class="pull-left">
                                <h4 class="menu-title">Dashboard & Reports</h4>
                                <div class="menu-desc">KPIs, charts, marketplace breakdowns</div>
                            </div>
                            <div class="pull-right text-info menu-ico">
                                <i class="glyphicon glyphicon-stats"></i>
                            </div>
                        </div>

                        <asp:Button ID="btnDashboardReports" runat="server" Text="Open"
                            CssClass="btn btn-default btn-block menu-btn"
                            OnClick="toDashboard_click"
                            CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>


        </div>

        <!-- hidden fields -->
        <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
        <asp:Label ID="lblRole" runat="server" Text="Label" Visible="False"></asp:Label>

    </div>

    <!-- Counter animation -->
    <script type="text/javascript">
        (function () {
            function animateCounter(el, to) {
                var duration = 900; // ms
                var startTime = null;
                el.textContent = "0";

                function step(ts) {
                    if (!startTime) startTime = ts;
                    var p = Math.min((ts - startTime) / duration, 1);
                    var val = Math.floor(to * p);
                    el.textContent = val.toLocaleString();
                    if (p < 1) requestAnimationFrame(step);
                }
                requestAnimationFrame(step);
            }

            document.addEventListener("DOMContentLoaded", function () {
                var lblOrders = document.getElementById("<%= lblTotalOrders.ClientID %>");
                var lblQty = document.getElementById("<%= lblTotalQty.ClientID %>");
                var hfOrders = document.getElementById("<%= hfTotalOrders.ClientID %>");
                var hfQty = document.getElementById("<%= hfTotalQty.ClientID %>");

                if (lblOrders && hfOrders) {
                    var t1 = parseInt(hfOrders.value || "0", 10);
                    if (isNaN(t1)) t1 = 0;
                    animateCounter(lblOrders, t1);
                }

                if (lblQty && hfQty) {
                    var t2 = parseInt(hfQty.value || "0", 10);
                    if (isNaN(t2)) t2 = 0;
                    animateCounter(lblQty, t2);
                }
            });
        })();
    </script>

</asp:Content>