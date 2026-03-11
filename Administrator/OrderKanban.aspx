<%@ Page Title="Order Kanban" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="OrderKanban.aspx.cs"
    Inherits="Feniks.Administrator.OrderKanban" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        body { background:#f5f6f8; }

        .kanban-page {
            padding: 4px 0 10px 0;
        }

        .kanban-hero {
            display:flex;
            align-items:flex-start;
            justify-content:space-between;
            gap:16px;
            margin-bottom:18px;
            flex-wrap:wrap;
        }

        .kanban-title-wrap {
            min-width:0;
        }

        .kanban-title {
            margin:0;
            font-size:28px;
            line-height:1.15;
            font-weight:800;
            color:#111827;
            letter-spacing:-.02em;
        }

        .kanban-subtitle {
            margin-top:6px;
            color:#6b7280;
            font-size:14px;
        }

        .kanban-summary {
            display:flex;
            gap:10px;
            flex-wrap:wrap;
        }

        .summary-pill {
            background:#fff;
            border:1px solid #e5e7eb;
            border-radius:14px;
            padding:10px 14px;
            min-width:110px;
            box-shadow:0 2px 8px rgba(0,0,0,.03);
        }

        .summary-pill-label {
            font-size:12px;
            color:#6b7280;
            margin-bottom:4px;
        }

        .summary-pill-value {
            font-size:20px;
            font-weight:800;
            color:#111827;
            line-height:1;
        }

        .kanban-board {
            display:grid;
            grid-template-columns:repeat(3, minmax(0, 1fr));
            gap:16px;
            align-items:start;
        }

        .kanban-col {
            min-width:0;
            background:#eef2f7;
            border:1px solid #dde5ee;
            border-radius:20px;
            padding:14px;
        }

        .kanban-col-head {
            display:flex;
            align-items:center;
            justify-content:space-between;
            gap:12px;
            margin-bottom:14px;
        }

        .kanban-col-title {
            display:flex;
            align-items:center;
            gap:10px;
            min-width:0;
        }

        .kanban-dot {
            width:12px;
            height:12px;
            border-radius:999px;
            flex-shrink:0;
        }

        .dot-open { background:#f59e0b; }
        .dot-progress { background:#3b82f6; }
        .dot-closed { background:#10b981; }

        .kanban-col-name {
            font-size:16px;
            font-weight:800;
            color:#111827;
            line-height:1.1;
        }

        .kanban-col-badge {
            min-width:30px;
            height:30px;
            padding:0 10px;
            border-radius:999px;
            display:inline-flex;
            align-items:center;
            justify-content:center;
            background:#111827;
            color:#fff;
            font-size:13px;
            font-weight:800;
            white-space:nowrap;
        }

        .kanban-list {
            display:flex;
            flex-direction:column;
            gap:12px;
            min-height:120px;
        }

        .kanban-card {
            background:#fff;
            border:1px solid #e5e7eb;
            border-radius:18px;
            padding:14px;
            box-shadow:0 4px 14px rgba(0,0,0,.04);
            transition:transform .16s ease, box-shadow .16s ease;
            min-width:0;
        }

        .kanban-card:hover {
            transform:translateY(-1px);
            box-shadow:0 8px 20px rgba(0,0,0,.06);
        }

        .kanban-card-top {
            display:flex;
            align-items:flex-start;
            justify-content:space-between;
            gap:10px;
            margin-bottom:10px;
        }

        .order-no {
            font-size:16px;
            font-weight:800;
            color:#111827;
            line-height:1.15;
            word-break:break-word;
        }

        .buyer-name {
            font-size:14px;
            font-weight:700;
            color:#374151;
            margin-top:5px;
            word-break:break-word;
        }

        .waiting-badge {
            flex-shrink:0;
            display:inline-flex;
            align-items:center;
            justify-content:center;
            min-width:76px;
            min-height:30px;
            padding:0 10px;
            border-radius:999px;
            font-size:12px;
            font-weight:800;
            white-space:nowrap;
            border:1px solid transparent;
        }

        .waiting-ok {
            background:#ecfdf5;
            color:#047857;
            border-color:#a7f3d0;
        }

        .waiting-mid {
            background:#fffbeb;
            color:#b45309;
            border-color:#fde68a;
        }

        .waiting-hot {
            background:#fef2f2;
            color:#b91c1c;
            border-color:#fecaca;
        }

        .kanban-meta {
            display:grid;
            grid-template-columns:1fr;
            gap:8px;
            margin-bottom:10px;
        }

        .meta-row {
            display:flex;
            align-items:flex-start;
            justify-content:space-between;
            gap:10px;
        }

        .meta-label {
            font-size:12px;
            color:#6b7280;
            line-height:1.3;
            flex-shrink:0;
        }

        .meta-value {
            font-size:13px;
            color:#111827;
            font-weight:700;
            text-align:right;
            line-height:1.35;
            min-width:0;
            word-break:break-word;
        }

        .order-total {
            font-size:18px;
            font-weight:800;
            color:#111827;
            margin:4px 0 8px 0;
            line-height:1.2;
            word-break:break-word;
        }

        .order-breakdown {
            font-size:12px;
            color:#6b7280;
            line-height:1.5;
            word-break:break-word;
            overflow-wrap:anywhere;
            border-top:1px dashed #e5e7eb;
            padding-top:10px;
        }

        .empty-card {
            background:#fff;
            border:1px dashed #cbd5e1;
            border-radius:18px;
            padding:18px;
            color:#6b7280;
            text-align:center;
            font-size:14px;
        }

        @media (max-width: 1199px) {
            .kanban-board {
                grid-template-columns:repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 767px) {
            .kanban-page {
                padding-top:2px;
            }

            .kanban-title {
                font-size:22px;
            }

            .kanban-subtitle {
                font-size:13px;
            }

            .kanban-board {
                grid-template-columns:1fr;
                gap:12px;
            }

            .kanban-col {
                padding:12px;
                border-radius:16px;
            }

            .kanban-card {
                border-radius:16px;
                padding:12px;
            }

            .kanban-card-top {
                flex-direction:column;
                align-items:flex-start;
            }

            .waiting-badge {
                min-width:0;
            }

            .meta-row {
                flex-direction:column;
                gap:3px;
            }

            .meta-value {
                text-align:left;
            }

            .order-total {
                font-size:17px;
            }

            .summary-pill {
                flex:1 1 calc(50% - 10px);
                min-width:0;
            }
        }
    </style>

    <div class="kanban-page">
        <div class="kanban-hero">
            <div class="kanban-title-wrap">
                <h1 class="kanban-title">Order Kanban Board</h1>
                <div class="kanban-subtitle">
                    Open, in progress ve closed siparişleri tek ekranda takip et.
                </div>
            </div>

            <div class="kanban-summary">
                <div class="summary-pill">
                    <div class="summary-pill-label">Open</div>
                    <div class="summary-pill-value">
                        <asp:Literal ID="litSummaryOpen" runat="server" />
                    </div>
                </div>
                <div class="summary-pill">
                    <div class="summary-pill-label">In Progress</div>
                    <div class="summary-pill-value">
                        <asp:Literal ID="litSummaryProgress" runat="server" />
                    </div>
                </div>
                <div class="summary-pill">
                    <div class="summary-pill-label">Closed</div>
                    <div class="summary-pill-value">
                        <asp:Literal ID="litSummaryClosed" runat="server" />
                    </div>
                </div>
            </div>
        </div>

        <div class="kanban-board">

            <!-- OPEN -->
            <section class="kanban-col">
                <div class="kanban-col-head">
                    <div class="kanban-col-title">
                        <span class="kanban-dot dot-open"></span>
                        <span class="kanban-col-name">Open</span>
                    </div>
                    <span class="kanban-col-badge">
                        <asp:Literal ID="litBadgeOpen" runat="server" />
                    </span>
                </div>

                <div class="kanban-list">
                    <asp:Repeater ID="rptOpen" runat="server" OnItemDataBound="rptOpen_ItemDataBound">
                        <ItemTemplate>
                            <article class="kanban-card">
                                <div class="kanban-card-top">
                                    <div>
                                        <div class="order-no">#<%# Eval("OrderNumber") %></div>
                                        <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                                    </div>
                                    <asp:Literal ID="litWaiting" runat="server"></asp:Literal>
                                </div>

                                <div class="kanban-meta">
                                    <div class="meta-row">
                                        <span class="meta-label">Date</span>
                                        <span class="meta-value"><%# Eval("OrderDateText") %></span>
                                    </div>
                                </div>

                                <div class="order-total"><%# Eval("OrderTotalText") %></div>
                                <div class="order-breakdown"><%# Eval("BreakdownText") %></div>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:PlaceHolder ID="phOpenEmpty" runat="server" Visible="false">
                        <div class="empty-card">Bu kolonda sipariş yok.</div>
                    </asp:PlaceHolder>
                </div>
            </section>

            <!-- IN PROGRESS -->
            <section class="kanban-col">
                <div class="kanban-col-head">
                    <div class="kanban-col-title">
                        <span class="kanban-dot dot-progress"></span>
                        <span class="kanban-col-name">In Progress</span>
                    </div>
                    <span class="kanban-col-badge">
                        <asp:Literal ID="litBadgeProgress" runat="server" />
                    </span>
                </div>

                <div class="kanban-list">
                    <asp:Repeater ID="rptProgress" runat="server" OnItemDataBound="rptProgress_ItemDataBound">
                        <ItemTemplate>
                            <article class="kanban-card">
                                <div class="kanban-card-top">
                                    <div>
                                        <div class="order-no">#<%# Eval("OrderNumber") %></div>
                                        <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                                    </div>
                                    <asp:Literal ID="litWaiting" runat="server"></asp:Literal>
                                </div>

                                <div class="kanban-meta">
                                    <div class="meta-row">
                                        <span class="meta-label">Date</span>
                                        <span class="meta-value"><%# Eval("OrderDateText") %></span>
                                    </div>
                                </div>

                                <div class="order-total"><%# Eval("OrderTotalText") %></div>
                                <div class="order-breakdown"><%# Eval("BreakdownText") %></div>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:PlaceHolder ID="phProgressEmpty" runat="server" Visible="false">
                        <div class="empty-card">Bu kolonda sipariş yok.</div>
                    </asp:PlaceHolder>
                </div>
            </section>

            <!-- CLOSED -->
            <section class="kanban-col">
                <div class="kanban-col-head">
                    <div class="kanban-col-title">
                        <span class="kanban-dot dot-closed"></span>
                        <span class="kanban-col-name">Closed</span>
                    </div>
                    <span class="kanban-col-badge">
                        <asp:Literal ID="litBadgeClosed" runat="server" />
                    </span>
                </div>

                <div class="kanban-list">
                    <asp:Repeater ID="rptClosed" runat="server" OnItemDataBound="rptClosed_ItemDataBound">
                        <ItemTemplate>
                            <article class="kanban-card">
                                <div class="kanban-card-top">
                                    <div>
                                        <div class="order-no">#<%# Eval("OrderNumber") %></div>
                                        <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                                    </div>
                                    <asp:Literal ID="litWaiting" runat="server"></asp:Literal>
                                </div>

                                <div class="kanban-meta">
                                    <div class="meta-row">
                                        <span class="meta-label">Date</span>
                                        <span class="meta-value"><%# Eval("OrderDateText") %></span>
                                    </div>
                                </div>

                                <div class="order-total"><%# Eval("OrderTotalText") %></div>
                                <div class="order-breakdown"><%# Eval("BreakdownText") %></div>
                            </article>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:PlaceHolder ID="phClosedEmpty" runat="server" Visible="false">
                        <div class="empty-card">Bu kolonda sipariş yok.</div>
                    </asp:PlaceHolder>
                </div>
            </section>

        </div>
    </div>

</asp:Content>