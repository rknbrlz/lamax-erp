<%@ Page Title="Order Kanban" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="OrderKanban.aspx.cs"
    Inherits="Feniks.Administrator.OrderKanban" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/sortablejs@1.15.0/Sortable.min.js"></script>

<style>
    body {
        background: #f5f6f8;
    }

    .page-title {
        font-size: 26px;
        font-weight: 700;
        margin: 8px 0 18px 0;
        color: #202124;
    }

    .kanban-board {
        display: flex;
        gap: 18px;
        align-items: stretch;
    }

    .kanban-column {
        flex: 1;
        background: #eef1f5;
        border-radius: 18px;
        padding: 14px;
        min-height: 690px;
    }

    .kanban-column-title {
        font-size: 16px;
        font-weight: 700;
        color: #202124;
        margin-bottom: 12px;
    }

    .kanban-list {
        min-height: 620px;
    }

    .kanban-card {
        position: relative;
        background: #fff;
        padding: 14px 14px 16px 14px;
        border-radius: 14px;
        margin-bottom: 12px;
        box-shadow: 0 4px 14px rgba(0,0,0,.06);
        border: 1px solid rgba(0,0,0,.03);
        cursor: grab;
        transition: box-shadow .15s ease, transform .15s ease;
    }

    .kanban-card:hover {
        box-shadow: 0 8px 20px rgba(0,0,0,.08);
    }

    .kanban-card.sortable-chosen {
        cursor: grabbing;
    }

    .sortable-ghost {
        opacity: .35;
    }

    .sortable-drag {
        transform: rotate(1deg);
    }

    .order-no {
        font-weight: 800;
        color: #111827;
        font-size: 14px;
        margin-bottom: 4px;
    }

    .buyer-name {
        font-size: 14px;
        color: #202124;
        margin-bottom: 2px;
    }

    .order-date {
        font-size: 13px;
        color: #4b5563;
        margin-bottom: 4px;
    }

    .order-total {
        font-size: 18px;
        font-weight: 800;
        color: #111827;
        margin-bottom: 6px;
    }

    .order-breakdown {
        margin-top: 4px;
        font-size: 11px;
        color: #6b7280;
        line-height: 1.45;
    }

    .waiting-tag {
        position: absolute;
        right: 12px;
        bottom: 10px;
        font-size: 11px;
        padding: 4px 9px;
        border-radius: 10px;
        font-weight: 600;
    }

    .waiting-ok {
        background: #e7f7ee;
        color: #059669;
    }

    .waiting-mid {
        background: #fff4e5;
        color: #b45309;
    }

    .waiting-hot {
        background: #ffe4e6;
        color: #dc2626;
    }

    .empty-box {
        background: rgba(255,255,255,.35);
        border: 2px dashed rgba(0,0,0,.08);
        border-radius: 14px;
        min-height: 120px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: #94a3b8;
        font-size: 13px;
    }
</style>

<div class="page-title">Order Kanban Board</div>

<div class="kanban-board">

    <!-- OPEN -->
    <div class="kanban-column">
        <div class="kanban-column-title">
            Open (<span class="open-count"><asp:Literal ID="litBadgeOpen" runat="server"></asp:Literal></span>)
        </div>

        <div id="openList" class="kanban-list">
            <asp:Repeater ID="rptOpen" runat="server" OnItemDataBound="rptOpen_ItemDataBound">
                <ItemTemplate>
                    <div class="kanban-card" data-id='<%# Eval("OrderID") %>'>
                        <div class="order-no">#<%# Eval("OrderNumber") %></div>
                        <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                        <div class="order-date">Date: <%# Eval("OrderDateText") %></div>

                        <div class="order-total">
                            <%# Eval("OrderTotalText") %>
                        </div>

                        <div class="order-breakdown">
                            <%# Eval("BreakdownText") %>
                        </div>

                        <asp:Literal ID="litWaiting" runat="server"></asp:Literal>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="phOpenEmpty" runat="server" Visible="false">
                <div class="empty-box">Bu kolonda sipariş yok.</div>
            </asp:PlaceHolder>
        </div>
    </div>

    <!-- IN PROGRESS -->
    <div class="kanban-column">
        <div class="kanban-column-title">
            In Progress (<span class="progress-count"><asp:Literal ID="litBadgeProgress" runat="server"></asp:Literal></span>)
        </div>

        <div id="progressList" class="kanban-list">
            <asp:Repeater ID="rptProgress" runat="server" OnItemDataBound="rptProgress_ItemDataBound">
                <ItemTemplate>
                    <div class="kanban-card" data-id='<%# Eval("OrderID") %>'>
                        <div class="order-no">#<%# Eval("OrderNumber") %></div>
                        <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                        <div class="order-date">Date: <%# Eval("OrderDateText") %></div>

                        <div class="order-total">
                            <%# Eval("OrderTotalText") %>
                        </div>

                        <div class="order-breakdown">
                            <%# Eval("BreakdownText") %>
                        </div>

                        <asp:Literal ID="litWaiting" runat="server"></asp:Literal>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="phProgressEmpty" runat="server" Visible="false">
                <div class="empty-box">Bu kolonda sipariş yok.</div>
            </asp:PlaceHolder>
        </div>
    </div>

    <!-- CLOSED -->
    <div class="kanban-column">
        <div class="kanban-column-title">
            Closed (<span class="closed-count"><asp:Literal ID="litBadgeClosed" runat="server"></asp:Literal></span>)
        </div>

        <div id="closedList" class="kanban-list">
            <asp:Repeater ID="rptClosed" runat="server" OnItemDataBound="rptClosed_ItemDataBound">
                <ItemTemplate>
                    <div class="kanban-card" data-id='<%# Eval("OrderID") %>'>
                        <div class="order-no">#<%# Eval("OrderNumber") %></div>
                        <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                        <div class="order-date">Date: <%# Eval("OrderDateText") %></div>

                        <div class="order-total">
                            <%# Eval("OrderTotalText") %>
                        </div>

                        <div class="order-breakdown">
                            <%# Eval("BreakdownText") %>
                        </div>

                        <asp:Literal ID="litWaiting" runat="server"></asp:Literal>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:PlaceHolder ID="phClosedEmpty" runat="server" Visible="false">
                <div class="empty-box">Bu kolonda sipariş yok.</div>
            </asp:PlaceHolder>
        </div>
    </div>

</div>

<script type="text/javascript">
    function getStatusFromListId(listId) {
        if (listId === 'openList') return 'OPEN';
        if (listId === 'progressList') return 'INPROGRESS';
        if (listId === 'closedList') return 'CLOSED';
        return '';
    }

    function refreshCounts() {
        $('.open-count').text($('#openList .kanban-card').length);
        $('.progress-count').text($('#progressList .kanban-card').length);
        $('.closed-count').text($('#closedList .kanban-card').length);
    }

    function bindSortable(listId) {
        var el = document.getElementById(listId);
        if (!el) return;

        new Sortable(el, {
            group: {
                name: 'kanbanOrders',
                pull: true,
                put: true
            },
            animation: 180,
            sort: true,
            ghostClass: 'sortable-ghost',
            chosenClass: 'sortable-chosen',
            dragClass: 'sortable-drag',
            forceFallback: true,
            fallbackOnBody: true,
            swapThreshold: 0.65,

            onEnd: function (evt) {
                var orderId = evt.item.getAttribute('data-id');
                var newStatus = getStatusFromListId(evt.to.id);

                if (!orderId || !newStatus) return;

                $.ajax({
                    type: 'POST',
                    url: 'OrderKanbanUpdate.aspx',
                    data: {
                        orderId: orderId,
                        status: newStatus
                    },
                    success: function () {
                        refreshCounts();
                    },
                    error: function () {
                        alert('Kanban güncelleme başarısız.');
                        location.reload();
                    }
                });
            }
        });
    }

    $(function () {
        bindSortable('openList');
        bindSortable('progressList');
        bindSortable('closedList');
        refreshCounts();
    });
</script>

</asp:Content>