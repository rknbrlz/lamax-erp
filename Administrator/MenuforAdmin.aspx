<%@ Page Title="Menu for Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MenuforAdmin.aspx.cs" Inherits="Feniks.Admin.MenuforAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body {
        background: #f3f5f7;
    }

    .admin-page {
        padding: 0 14px 14px 14px;
        margin-top: 0 !important;
    }

    .page-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 12px;
        flex-wrap: wrap;
        margin: 0 0 10px 0;
        padding-top: 0;
    }

    .page-title-wrap {
        margin: 0;
        padding: 0;
    }

    .page-title-wrap h1 {
        margin: 0;
        font-size: 24px;
        font-weight: 700;
        color: #162033;
        letter-spacing: -.2px;
        line-height: 1.1;
    }

    .page-title-wrap p {
        margin: 4px 0 0 0;
        font-size: 12px;
        color: #6b7280;
    }

    .top-rate-strip {
        display: flex;
        gap: 8px;
        flex-wrap: wrap;
        margin: 0;
        padding: 0;
    }

    .top-rate-item {
        min-width: 104px;
        background: #ffffff;
        border: 1px solid #e4e7ec;
        border-radius: 8px;
        padding: 8px 10px;
    }

    .top-rate-label {
        font-size: 10px;
        font-weight: 700;
        color: #6b7280;
        text-transform: uppercase;
        letter-spacing: .35px;
        margin-bottom: 2px;
    }

    .top-rate-value {
        font-size: 15px;
        font-weight: 700;
        color: #162033;
        line-height: 1.1;
    }

    .info-banner {
        background: #fffbea;
        border: 1px solid #efe2a6;
        color: #7a6410;
        padding: 9px 12px;
        border-radius: 8px;
        margin-bottom: 12px;
        font-size: 12px;
        font-weight: 600;
    }

    .etsy-banner {
        background: #f6f8ff;
        border: 1px solid #dbe3ff;
        color: #304674;
        padding: 9px 12px;
        border-radius: 8px;
        margin-bottom: 12px;
        font-size: 12px;
        font-weight: 600;
    }

    .row-tight {
        margin-bottom: 12px;
    }

    .panel-card {
        background: #ffffff;
        border: 1px solid #e5e7eb;
        border-radius: 10px;
        padding: 14px;
        height: 100%;
    }

    .panel-title {
        margin: 0;
        font-size: 17px;
        font-weight: 700;
        color: #162033;
        line-height: 1.15;
    }

    .panel-subtitle {
        margin: 3px 0 10px 0;
        font-size: 12px;
        color: #6b7280;
        line-height: 1.35;
    }

    .mini-kpi-grid {
        display: grid;
        grid-template-columns: repeat(2, minmax(0,1fr));
        gap: 10px;
    }

    .mini-kpi-box {
        border: 1px solid #e5e7eb;
        background: #fafbfc;
        border-radius: 8px;
        padding: 12px;
    }

    .mini-kpi-label {
        font-size: 10px;
        color: #6b7280;
        text-transform: uppercase;
        font-weight: 700;
        letter-spacing: .35px;
        margin-bottom: 6px;
    }

    .mini-kpi-value {
        font-size: 36px;
        line-height: 1;
        font-weight: 700;
        color: #162033;
    }

    .meta-text {
        margin-top: 10px;
        font-size: 11px;
        color: #6b7280;
    }

    .form-grid {
        display: grid;
        grid-template-columns: 1.2fr .85fr .85fr auto;
        gap: 10px;
        align-items: end;
    }

    .form-grid-pay {
        display: grid;
        grid-template-columns: 1fr .8fr .8fr auto;
        gap: 10px;
        align-items: end;
        margin-bottom: 10px;
    }

    .field label {
        display: block;
        margin-bottom: 4px;
        font-size: 10px;
        font-weight: 700;
        color: #4b5563;
        text-transform: uppercase;
        letter-spacing: .35px;
    }

    .input-modern {
        width: 100%;
        height: 38px;
        border: 1px solid #d7dde6;
        border-radius: 7px;
        padding: 8px 10px;
        background: #ffffff;
        color: #162033;
        font-size: 13px;
        transition: .15s ease;
        outline: none;
    }

    .input-modern:focus {
        border-color: #94a3b8;
        box-shadow: 0 0 0 3px rgba(148,163,184,.10);
    }

    .textarea-modern {
        width: 100%;
        min-height: 120px;
        max-height: 120px;
        border: 1px solid #d7dde6;
        border-radius: 7px;
        padding: 10px;
        background: #ffffff;
        color: #162033;
        font-size: 13px;
        resize: none;
        outline: none;
        transition: .15s ease;
    }

    .textarea-modern:focus {
        border-color: #94a3b8;
        box-shadow: 0 0 0 3px rgba(148,163,184,.10);
    }

    .btn-main {
        height: 38px;
        border: none;
        border-radius: 7px;
        padding: 0 14px;
        background: #162033;
        color: #ffffff;
        font-size: 13px;
        font-weight: 700;
        white-space: nowrap;
    }

    .btn-main:hover,
    .btn-main:focus {
        background: #0f172a;
        color: #ffffff;
    }

    .btn-light-nav {
        height: 36px;
        border: 1px solid #d8dee7;
        border-radius: 7px;
        padding: 0 12px;
        background: #ffffff;
        color: #162033;
        font-size: 13px;
        font-weight: 600;
    }

    .btn-light-nav:hover,
    .btn-light-nav:focus {
        background: #f8fafc;
        color: #162033;
    }

    .btn-etsy-nav {
        height: 36px;
        border: 1px solid #cfd8ff;
        border-radius: 7px;
        padding: 0 12px;
        background: #f7f9ff;
        color: #233a70;
        font-size: 13px;
        font-weight: 700;
    }

    .btn-etsy-nav:hover,
    .btn-etsy-nav:focus {
        background: #edf2ff;
        color: #1d315f;
    }

    .result-box {
        margin-top: 10px;
        border: 1px solid #e5e7eb;
        background: #fafbfc;
        border-radius: 8px;
        padding: 12px;
        min-height: 70px;
    }

    .result-big {
        font-size: 22px;
        font-weight: 700;
        color: #162033;
        line-height: 1.15;
    }

    .result-meta {
        margin-top: 4px;
        font-size: 11px;
        color: #6b7280;
    }

    .fx-grid {
        display: grid;
        grid-template-columns: repeat(4, minmax(0,1fr));
        gap: 10px;
    }

    .fx-box {
        border: 1px solid #e5e7eb;
        background: #fafbfc;
        border-radius: 8px;
        padding: 12px;
        text-align: center;
    }

    .fx-code {
        font-size: 10px;
        color: #6b7280;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: .35px;
        margin-bottom: 6px;
    }

    .fx-value {
        font-size: 26px;
        line-height: 1;
        font-weight: 700;
        color: #162033;
    }

    .toolbar-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        gap: 10px;
        flex-wrap: wrap;
        margin-top: 10px;
    }

    .toolbar-meta {
        font-size: 11px;
        color: #6b7280;
    }

    .msg-label {
        display: inline-block;
        margin-top: 8px;
        font-size: 11px;
        font-weight: 700;
    }

    .table-wrap {
        border: 1px solid #e5e7eb;
        border-radius: 8px;
        overflow: hidden;
        background: #fff;
    }

    .table-clean {
        width: 100%;
        border-collapse: collapse;
    }

    .table-clean th {
        background: #f8fafc;
        color: #4b5563;
        font-size: 10px;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: .3px;
        padding: 9px 10px;
        border-bottom: 1px solid #e5e7eb;
    }

    .table-clean td {
        padding: 9px 10px;
        border-bottom: 1px solid #eef2f7;
        font-size: 12px;
        color: #162033;
        line-height: 1.3;
    }

    .table-clean tr:last-child td {
        border-bottom: none;
    }

    .table-clean tr:nth-child(even) td {
        background: #fcfcfd;
    }

    .nav-wrap {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }

    .nav-divider {
        height: 1px;
        background: #e8edf3;
        margin: 12px 0 10px 0;
    }

    .nav-section-title {
        font-size: 11px;
        font-weight: 700;
        color: #6b7280;
        text-transform: uppercase;
        letter-spacing: .35px;
        margin-bottom: 8px;
    }

    .hidden-block,
    .hero-box {
        display: none;
    }

    .compact-panel {
        padding-top: 12px;
        padding-bottom: 12px;
    }

    @media (max-width: 1199px) {
        .fx-grid {
            grid-template-columns: repeat(2, minmax(0,1fr));
        }
    }

    @media (max-width: 991px) {
        .form-grid,
        .form-grid-pay {
            grid-template-columns: 1fr 1fr;
        }

        .form-grid > div:last-child,
        .form-grid-pay > div:last-child {
            grid-column: 1 / -1;
        }
    }

    @media (max-width: 767px) {
        .admin-page {
            padding: 0 10px 10px 10px;
        }

        .page-title-wrap h1 {
            font-size: 21px;
        }

        .mini-kpi-grid,
        .form-grid,
        .form-grid-pay,
        .fx-grid {
            grid-template-columns: 1fr;
        }

        .mini-kpi-value {
            font-size: 30px;
        }

        .fx-value {
            font-size: 24px;
        }

        .top-rate-item {
            min-width: 95px;
        }

        .textarea-modern {
            min-height: 100px;
            max-height: 100px;
        }
    }
</style>

<div class="admin-page">

    <div class="page-header">
        <div class="page-title-wrap">
            <h1>Admin Overview</h1>
            <p>Compact control panel for daily operations</p>
        </div>

        <div class="top-rate-strip">
            <div class="top-rate-item">
                <div class="top-rate-label">USD/PLN</div>
                <div class="top-rate-value"><asp:Label ID="lblUSD" runat="server" Text="0.0000"></asp:Label></div>
            </div>
            <div class="top-rate-item">
                <div class="top-rate-label">EUR/PLN</div>
                <div class="top-rate-value"><asp:Label ID="lblEUR" runat="server" Text="0.0000"></asp:Label></div>
            </div>
            <div class="top-rate-item">
                <div class="top-rate-label">CAD/PLN</div>
                <div class="top-rate-value"><asp:Label ID="lblCAD" runat="server" Text="0.0000"></asp:Label></div>
            </div>
            <div class="top-rate-item">
                <div class="top-rate-label">PLN/TRY</div>
                <div class="top-rate-value"><asp:Label ID="lblPLN" runat="server" Text="0.0000"></asp:Label></div>
            </div>
        </div>
    </div>

    <div id="divInfo" runat="server" class="info-banner">
        Pending orders in action:
        <asp:Label ID="lblOpenQty" runat="server" Text="0"></asp:Label>
    </div>

    <asp:Panel ID="pnlEtsyInfo" runat="server" CssClass="etsy-banner" Visible="false">
        Etsy inbox pending:
        <asp:Label ID="lblEtsyPendingCount" runat="server" Text="0"></asp:Label>
        &nbsp;&nbsp;•&nbsp;&nbsp;
        Last sync:
        <asp:Label ID="lblEtsyLastSync" runat="server" Text="-"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="pnlHero" runat="server" CssClass="hero-box">
        <div>hidden</div>
    </asp:Panel>

    <div class="row row-tight">
        <div class="col-md-3">
            <div class="panel-card compact-panel">
                <h2 class="panel-title">Orders</h2>
                <div class="panel-subtitle">All-time summary</div>

                <div class="mini-kpi-grid">
                    <div class="mini-kpi-box">
                        <div class="mini-kpi-label">Total Orders</div>
                        <div class="mini-kpi-value">
                            <asp:Label ID="lblTotalOrders" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>

                    <div class="mini-kpi-box">
                        <div class="mini-kpi-label">Total Qty</div>
                        <div class="mini-kpi-value">
                            <asp:Label ID="lblTotalQty" runat="server" Text="0"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="meta-text">
                    Updated:
                    <asp:Label ID="lblOrdersCounterUpdated" runat="server" Text="-"></asp:Label>
                </div>

                <asp:HiddenField ID="hfTotalOrders" runat="server" />
                <asp:HiddenField ID="hfTotalQty" runat="server" />
            </div>
        </div>

        <div class="col-md-5">
            <div class="panel-card compact-panel">
                <h2 class="panel-title">Currency Converter</h2>
                <div class="panel-subtitle">Quick amount conversion</div>

                <div class="form-grid">
                    <div class="field">
                        <label>Amount</label>
                        <asp:TextBox ID="txtFxAmount" runat="server" CssClass="input-modern" placeholder="0.00"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvFxAmount" runat="server"
                            ControlToValidate="txtFxAmount"
                            ErrorMessage="Amount is required"
                            Display="Dynamic"
                            ForeColor="#b30000"
                            ValidationGroup="fx"></asp:RequiredFieldValidator>
                    </div>

                    <div class="field">
                        <label>From</label>
                        <asp:DropDownList ID="ddlFxFrom" runat="server" CssClass="input-modern">
                            <asp:ListItem Text="USD" Value="USD"></asp:ListItem>
                            <asp:ListItem Text="PLN" Value="PLN"></asp:ListItem>
                            <asp:ListItem Text="EUR" Value="EUR"></asp:ListItem>
                            <asp:ListItem Text="TRY" Value="TRY"></asp:ListItem>
                            <asp:ListItem Text="CAD" Value="CAD"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="field">
                        <label>To</label>
                        <asp:DropDownList ID="ddlFxTo" runat="server" CssClass="input-modern">
                            <asp:ListItem Text="PLN" Value="PLN"></asp:ListItem>
                            <asp:ListItem Text="USD" Value="USD"></asp:ListItem>
                            <asp:ListItem Text="EUR" Value="EUR"></asp:ListItem>
                            <asp:ListItem Text="TRY" Value="TRY"></asp:ListItem>
                            <asp:ListItem Text="CAD" Value="CAD"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="field">
                        <asp:Button ID="btnFxConvert" runat="server" Text="Convert" CssClass="btn-main" ValidationGroup="fx" OnClick="btnFxConvert_Click" />
                    </div>
                </div>

                <div class="result-box">
                    <div class="result-big">
                        <asp:Label ID="lblFxConvertResult" runat="server" Text="-"></asp:Label>
                    </div>
                    <div class="result-meta">
                        Rate date:
                        <asp:Label ID="lblFxConvertRateDate" runat="server" Text="-"></asp:Label>
                    </div>
                    <asp:Label ID="lblFxConvertMsg" runat="server" CssClass="msg-label" Visible="false"></asp:Label>
                </div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="panel-card compact-panel">
                <h2 class="panel-title">Navigation</h2>
                <div class="panel-subtitle">Quick actions</div>

                <div class="nav-section-title">Core</div>
                <div class="nav-wrap">
                    <asp:Button ID="btnDashboardReports" runat="server" Text="Dashboard" CssClass="btn-light-nav" OnClick="toDashboard_click" />
                    <asp:Button ID="btnNavProducts" runat="server" Text="Products" CssClass="btn-light-nav" OnClick="toProducts_click" />
                    <asp:Button ID="btnNavOrders" runat="server" Text="Orders" CssClass="btn-light-nav" OnClick="toOrders_click" />
                    <asp:Button ID="btnNavNewOrder" runat="server" Text="New Order" CssClass="btn-light-nav" OnClick="btnNewOrder_Click" />
                </div>

                <div class="nav-divider"></div>

                <div class="nav-section-title">Etsy</div>
                <div class="nav-wrap">
                    <asp:Button ID="btnNavEtsyInbox" runat="server" Text="Etsy Inbox" CssClass="btn-etsy-nav" OnClick="toEtsyInbox_click" />
                    <asp:Button ID="btnNavEtsyConnect" runat="server" Text="Connect Etsy" CssClass="btn-etsy-nav" OnClick="toEtsyConnect_click" />
                    <asp:Button ID="btnNavEtsySync" runat="server" Text="Sync Etsy" CssClass="btn-etsy-nav" OnClick="toEtsySync_click" />
                </div>

                <div style="height:10px;"></div>

                <h2 class="panel-title" style="font-size:15px;">FX Snapshot</h2>
                <div class="panel-subtitle" style="margin-bottom:8px;">Latest stored rates</div>

                <div class="fx-grid">
                    <div class="fx-box">
                        <div class="fx-code">USDPLN</div>
                        <div class="fx-value"><asp:Label ID="lblUsdPln" runat="server" Text="-"></asp:Label></div>
                    </div>
                    <div class="fx-box">
                        <div class="fx-code">EURPLN</div>
                        <div class="fx-value"><asp:Label ID="lblEurPln" runat="server" Text="-"></asp:Label></div>
                    </div>
                    <div class="fx-box">
                        <div class="fx-code">GBPPLN</div>
                        <div class="fx-value"><asp:Label ID="lblGbpPln" runat="server" Text="-"></asp:Label></div>
                    </div>
                    <div class="fx-box">
                        <div class="fx-code">TRYPLN</div>
                        <div class="fx-value"><asp:Label ID="lblTryPln" runat="server" Text="-"></asp:Label></div>
                    </div>
                </div>

                <div class="toolbar-row">
                    <div class="toolbar-meta">
                        <asp:Label ID="lblFxDate" runat="server" Text="-"></asp:Label>
                        &nbsp;&nbsp;•&nbsp;&nbsp;
                        <asp:Label ID="lblFxUpdated" runat="server" Text="-"></asp:Label>
                    </div>

                    <asp:Button ID="btnUpdateFx" runat="server" Text="Update FX" CssClass="btn-main" OnClick="btnUpdateFx_Click" />
                </div>

                <asp:Label ID="lblFxMsg" runat="server" CssClass="msg-label" Visible="false"></asp:Label>
            </div>
        </div>
    </div>

    <div class="row row-tight">
        <div class="col-md-4">
            <div class="panel-card compact-panel">
                <h2 class="panel-title">Admin Note</h2>
                <div class="panel-subtitle">Short reminder area</div>

                <asp:TextBox
                    ID="txtAdminNote"
                    runat="server"
                    TextMode="MultiLine"
                    CssClass="textarea-modern"
                    MaxLength="4000"
                    placeholder="Write your note here..."></asp:TextBox>

                <div class="toolbar-row">
                    <div class="toolbar-meta">
                        Last update:
                        <asp:Label ID="lblAdminNoteUpdated" runat="server" Text="-"></asp:Label>
                    </div>

                    <asp:Button
                        ID="btnSaveAdminNote"
                        runat="server"
                        Text="Save Note"
                        CssClass="btn-main"
                        OnClick="btnSaveAdminNote_ServerClick" />
                </div>

                <asp:Label ID="lblAdminNoteMsg" runat="server" CssClass="msg-label" Visible="false"></asp:Label>
            </div>
        </div>

        <div class="col-md-8">
            <div class="panel-card compact-panel">
                <h2 class="panel-title">Payments</h2>
                <div class="panel-subtitle">Recent marketplace payment entries</div>

                <div class="form-grid-pay">
                    <div class="field">
                        <label>Marketplace</label>
                        <asp:DropDownList ID="ddlPayMarketplace" runat="server" CssClass="input-modern">
                            <asp:ListItem Text="Amazon" Value="Amazon"></asp:ListItem>
                            <asp:ListItem Text="Etsy" Value="Etsy"></asp:ListItem>
                            <asp:ListItem Text="eBay" Value="eBay"></asp:ListItem>
                            <asp:ListItem Text="Website" Value="Website"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="field">
                        <label>Date</label>
                        <asp:TextBox ID="txtPayDate" runat="server" CssClass="input-modern"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPayDate" runat="server"
                            ControlToValidate="txtPayDate"
                            ErrorMessage="Date is required"
                            Display="Dynamic"
                            ForeColor="#b30000"
                            ValidationGroup="pay"></asp:RequiredFieldValidator>
                    </div>

                    <div class="field">
                        <label>Amount</label>
                        <asp:TextBox ID="txtPayAmount" runat="server" CssClass="input-modern" placeholder="0.00"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPayAmount" runat="server"
                            ControlToValidate="txtPayAmount"
                            ErrorMessage="Amount is required"
                            Display="Dynamic"
                            ForeColor="#b30000"
                            ValidationGroup="pay"></asp:RequiredFieldValidator>
                    </div>

                    <div class="field">
                        <asp:Button ID="btnPayAdd" runat="server" Text="Save Payment" CssClass="btn-main" ValidationGroup="pay" OnClick="btnPayAdd_ServerClick" />
                    </div>
                </div>

                <asp:Label ID="lblPayMsg" runat="server" CssClass="msg-label" Visible="false"></asp:Label>

                <div class="table-wrap" style="margin-top:8px;">
                    <asp:GridView ID="gvPayments" runat="server" AutoGenerateColumns="false" CssClass="table-clean" GridLines="None">
                        <Columns>
                            <asp:BoundField DataField="PayDate" HeaderText="Date" />
                            <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" />
                            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:0.00}" HtmlEncode="false" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div style="padding:12px;color:#6b7280;font-size:12px;">No payments found.</div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

                <div class="toolbar-meta" style="margin-top:10px;">
                    Last update:
                    <asp:Label ID="lblPayUpdated" runat="server" Text="-"></asp:Label>
                </div>
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlDashboardReports" runat="server" Style="display:none;"></asp:Panel>

    <div class="hidden-block">
        <asp:Label ID="lblLoginName" runat="server" Text=""></asp:Label>
        <asp:Label ID="lblRole" runat="server" Text=""></asp:Label>

        <asp:Button ID="btnHeroProducts" runat="server" Text="Products" OnClick="toProducts_click" />
        <asp:Button ID="btnHeroOrders" runat="server" Text="Orders" OnClick="toOrders_click" />
        <asp:Button ID="btnHeroNewOrder" runat="server" Text="New Order" OnClick="btnNewOrder_Click" />
        <asp:Button ID="btnHeroDashboard" runat="server" Text="Dashboard" OnClick="toDashboard_click" />
    </div>

</div>

</asp:Content>