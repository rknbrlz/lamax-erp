<%@ Page Title="Order Kanban" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="OrderKanban.aspx.cs"
    Inherits="Feniks.Administrator.OrderKanban" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background: #f5f7fb; }

    .kanban-page {
        padding: 18px 18px 28px 18px;
    }

    .kanban-topbar {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 16px;
        flex-wrap: wrap;
        margin-bottom: 18px;
    }

    .kanban-title-wrap h1 {
        margin: 0;
        font-size: 24px;
        line-height: 1.15;
        font-weight: 800;
        color: #0f172a;
        letter-spacing: .2px;
    }

    .kanban-title-wrap .sub {
        margin-top: 6px;
        font-size: 13px;
        color: #64748b;
    }

    .kanban-stats {
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
    }

    .kanban-stat {
        min-width: 110px;
        background: #fff;
        border: 1px solid #e2e8f0;
        border-radius: 16px;
        padding: 10px 14px;
        box-shadow: 0 4px 16px rgba(15,23,42,.04);
    }

    .kanban-stat .label {
        font-size: 12px;
        color: #64748b;
        display: block;
        margin-bottom: 4px;
    }

    .kanban-stat .value {
        font-size: 18px;
        font-weight: 800;
        color: #0f172a;
    }

    .kanban-board {
        display: grid;
        grid-template-columns: repeat(3, minmax(280px, 1fr));
        gap: 16px;
        align-items: start;
    }

    .kanban-column {
        background: #eaf0f6;
        border: 1px solid #d8e1eb;
        border-radius: 24px;
        padding: 14px;
        min-height: 620px;
        transition: background .18s ease, border-color .18s ease, box-shadow .18s ease;
        position: relative;
    }

    .kanban-column.drag-over {
        background: #e6f0ff;
        border-color: #7aa7ff;
        box-shadow: 0 0 0 3px rgba(59,130,246,.10) inset;
    }

    .column-head {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        margin-bottom: 14px;
    }

    .column-title {
        display: flex;
        align-items: center;
        gap: 10px;
        font-size: 17px;
        font-weight: 800;
        color: #0f172a;
    }

    .column-dot {
        width: 11px;
        height: 11px;
        border-radius: 50%;
        display: inline-block;
    }

    .dot-open { background: #f59e0b; }
    .dot-progress { background: #3b82f6; }
    .dot-closed { background: #10b981; }

    .column-count {
        min-width: 34px;
        height: 34px;
        border-radius: 999px;
        background: #0f172a;
        color: #fff;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 0 10px;
        font-size: 14px;
        font-weight: 800;
    }

    .card-list {
        min-height: 460px;
    }

    .kanban-card {
        background: #fff;
        border: 1px solid #e5e7eb;
        border-radius: 20px;
        padding: 14px 14px 12px 14px;
        margin-bottom: 12px;
        box-shadow: 0 4px 14px rgba(15,23,42,.04);
        cursor: grab;
        user-select: none;
        transition: transform .16s ease, box-shadow .16s ease, opacity .16s ease;
        position: relative;
    }

    .kanban-card:hover {
        transform: translateY(-2px);
        box-shadow: 0 12px 24px rgba(15,23,42,.08);
    }

    .kanban-card.dragging {
        opacity: .85;
        transform: rotate(1deg) scale(1.02);
        box-shadow: 0 18px 35px rgba(15,23,42,.18);
        cursor: grabbing;
        z-index: 20;
    }

    .kanban-card.saving::after {
        content: 'Saving...';
        position: absolute;
        top: 12px;
        right: 12px;
        font-size: 11px;
        font-weight: 800;
        color: #2563eb;
        background: #eff6ff;
        border: 1px solid #bfdbfe;
        border-radius: 999px;
        padding: 4px 8px;
    }

    .card-top {
        display: flex;
        align-items: flex-start;
        justify-content: space-between;
        gap: 10px;
    }

    .card-title-block {
        min-width: 0;
    }

    .order-no {
        margin: 0;
        font-size: 17px;
        line-height: 1.2;
        font-weight: 900;
        color: #0f172a;
        word-break: break-word;
    }

    .buyer-name {
        margin-top: 6px;
        font-size: 14px;
        font-weight: 700;
        color: #1e293b;
        word-break: break-word;
    }

    .waiting-pill {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        white-space: nowrap;
        border-radius: 999px;
        padding: 8px 14px;
        font-size: 12px;
        font-weight: 800;
        border: 1px solid transparent;
    }

    /* Open / In Progress */
    .waiting-open-good {
        color: #a16207;
        background: #fffbeb;
        border-color: #fde68a;
    }

    .waiting-open-warn {
        color: #b45309;
        background: #fff7ed;
        border-color: #fdba74;
    }

    .waiting-open-hot {
        color: #b91c1c;
        background: #fef2f2;
        border-color: #fca5a5;
    }

    /* Closed */
    .waiting-closed-fast {
        color: #065f46;
        background: #ecfdf5;
        border-color: #86efac;
    }

    .waiting-closed-mid {
        color: #1d4ed8;
        background: #eff6ff;
        border-color: #93c5fd;
    }

    .waiting-closed-slow {
        color: #6b7280;
        background: #f3f4f6;
        border-color: #d1d5db;
    }

    .meta-row {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        margin-top: 10px;
        font-size: 13px;
        color: #64748b;
    }

    .meta-row strong {
        color: #0f172a;
        font-weight: 800;
    }

    .amount {
        margin-top: 10px;
        font-size: 18px;
        font-weight: 900;
        color: #0f172a;
    }

    .divider {
        margin: 10px 0 9px 0;
        border-top: 1px dashed #d9e2ec;
    }

    .breakdown {
        font-size: 12px;
        color: #64748b;
        line-height: 1.5;
        word-break: break-word;
    }

    .empty-box {
        border: 1px dashed #cbd5e1;
        border-radius: 18px;
        background: rgba(255,255,255,.85);
        padding: 18px;
        text-align: center;
        color: #64748b;
        font-size: 14px;
    }

    .drop-placeholder {
        height: 118px;
        border-radius: 20px;
        border: 2px dashed #93c5fd;
        background: rgba(255,255,255,.75);
        margin-bottom: 12px;
    }

    .toast-wrap {
        position: fixed;
        right: 18px;
        bottom: 18px;
        z-index: 9999;
        display: flex;
        flex-direction: column;
        gap: 10px;
    }

    .toast-item {
        min-width: 240px;
        max-width: 360px;
        background: #0f172a;
        color: #fff;
        border-radius: 14px;
        padding: 12px 14px;
        font-size: 13px;
        font-weight: 700;
        box-shadow: 0 16px 32px rgba(15,23,42,.22);
        opacity: 0;
        transform: translateY(8px);
        animation: toastIn .2s ease forwards;
    }

    .toast-item.error {
        background: #991b1b;
    }

    .toast-item.success {
        background: #065f46;
    }

    @keyframes toastIn {
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    @media (max-width: 1200px) {
        .kanban-board {
            grid-template-columns: 1fr;
        }

        .kanban-column {
            min-height: auto;
        }

        .card-list {
            min-height: 120px;
        }
    }
</style>

<div class="kanban-page">
    <div class="kanban-topbar">
        <div class="kanban-title-wrap">
            <h1>Order Kanban Board</h1>
            <div class="sub">Manage open, in progress, and closed orders in one Trello-style board.</div>
        </div>

        <div class="kanban-stats">
            <div class="kanban-stat">
                <span class="label">Open</span>
                <span class="value" id="statOpen"><asp:Literal ID="litSummaryOpen" runat="server" /></span>
            </div>
            <div class="kanban-stat">
                <span class="label">In Progress</span>
                <span class="value" id="statProgress"><asp:Literal ID="litSummaryProgress" runat="server" /></span>
            </div>
            <div class="kanban-stat">
                <span class="label">Closed</span>
                <span class="value" id="statClosed"><asp:Literal ID="litSummaryClosed" runat="server" /></span>
            </div>
        </div>
    </div>

    <div class="kanban-board">

        <!-- OPEN -->
        <div class="kanban-column" data-status="OPEN">
            <div class="column-head">
                <div class="column-title">
                    <span class="column-dot dot-open"></span>
                    <span>Open</span>
                </div>
                <div class="column-count" id="countOpen">
                    <asp:Literal ID="litBadgeOpen" runat="server" />
                </div>
            </div>

            <div class="card-list" data-status="OPEN" id="listOpen">
                <asp:Repeater ID="rptOpen" runat="server" OnItemDataBound="rptOpen_ItemDataBound">
                    <ItemTemplate>
                        <div class="kanban-card"
                             draggable="true"
                             data-order="<%# Eval("OrderNumber") %>"
                             data-status="OPEN">
                            <div class="card-top">
                                <div class="card-title-block">
                                    <div class="order-no">#<%# Eval("OrderNumber") %></div>
                                    <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                                </div>
                                <asp:Literal ID="litWaiting" runat="server" />
                            </div>

                            <div class="meta-row">
                                <span>Date</span>
                                <strong><%# Eval("OrderDateText") %></strong>
                            </div>

                            <div class="amount"><%# Eval("OrderTotalText") %></div>

                            <div class="divider"></div>

                            <div class="breakdown"><%# Eval("BreakdownText") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phOpenEmpty" runat="server" Visible="false">
                    <div class="empty-box">No orders in this column.</div>
                </asp:PlaceHolder>
            </div>
        </div>

        <!-- IN PROGRESS -->
        <div class="kanban-column" data-status="INPROGRESS">
            <div class="column-head">
                <div class="column-title">
                    <span class="column-dot dot-progress"></span>
                    <span>In Progress</span>
                </div>
                <div class="column-count" id="countProgress">
                    <asp:Literal ID="litBadgeProgress" runat="server" />
                </div>
            </div>

            <div class="card-list" data-status="INPROGRESS" id="listProgress">
                <asp:Repeater ID="rptProgress" runat="server" OnItemDataBound="rptProgress_ItemDataBound">
                    <ItemTemplate>
                        <div class="kanban-card"
                             draggable="true"
                             data-order="<%# Eval("OrderNumber") %>"
                             data-status="INPROGRESS">
                            <div class="card-top">
                                <div class="card-title-block">
                                    <div class="order-no">#<%# Eval("OrderNumber") %></div>
                                    <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                                </div>
                                <asp:Literal ID="litWaiting" runat="server" />
                            </div>

                            <div class="meta-row">
                                <span>Date</span>
                                <strong><%# Eval("OrderDateText") %></strong>
                            </div>

                            <div class="amount"><%# Eval("OrderTotalText") %></div>

                            <div class="divider"></div>

                            <div class="breakdown"><%# Eval("BreakdownText") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phProgressEmpty" runat="server" Visible="false">
                    <div class="empty-box">No orders in this column.</div>
                </asp:PlaceHolder>
            </div>
        </div>

        <!-- CLOSED -->
        <div class="kanban-column" data-status="CLOSED">
            <div class="column-head">
                <div class="column-title">
                    <span class="column-dot dot-closed"></span>
                    <span>Closed</span>
                </div>
                <div class="column-count" id="countClosed">
                    <asp:Literal ID="litBadgeClosed" runat="server" />
                </div>
            </div>

            <div class="card-list" data-status="CLOSED" id="listClosed">
                <asp:Repeater ID="rptClosed" runat="server" OnItemDataBound="rptClosed_ItemDataBound">
                    <ItemTemplate>
                        <div class="kanban-card"
                             draggable="true"
                             data-order="<%# Eval("OrderNumber") %>"
                             data-status="CLOSED">
                            <div class="card-top">
                                <div class="card-title-block">
                                    <div class="order-no">#<%# Eval("OrderNumber") %></div>
                                    <div class="buyer-name"><%# Eval("BuyerFullName") %></div>
                                </div>
                                <asp:Literal ID="litWaiting" runat="server" />
                            </div>

                            <div class="meta-row">
                                <span>Date</span>
                                <strong><%# Eval("OrderDateText") %></strong>
                            </div>

                            <div class="amount"><%# Eval("OrderTotalText") %></div>

                            <div class="divider"></div>

                            <div class="breakdown"><%# Eval("BreakdownText") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="phClosedEmpty" runat="server" Visible="false">
                    <div class="empty-box">No orders in this column.</div>
                </asp:PlaceHolder>
            </div>
        </div>

    </div>
</div>

<div class="toast-wrap" id="toastWrap"></div>

<script type="text/javascript">
    (function () {
        var draggedCard = null;
        var sourceList = null;
        var sourceNextSibling = null;
        var sourceStatus = null;
        var placeholder = document.createElement("div");
        placeholder.className = "drop-placeholder";

        function init() {
            bindCards();
            bindLists();
            refreshAllCounts();
            refreshAllEmptyStates();
        }

        function bindCards() {
            var cards = document.querySelectorAll(".kanban-card");
            for (var i = 0; i < cards.length; i++) {
                cards[i].addEventListener("dragstart", onDragStart);
                cards[i].addEventListener("dragend", onDragEnd);
            }
        }

        function bindLists() {
            var lists = document.querySelectorAll(".card-list");
            for (var i = 0; i < lists.length; i++) {
                lists[i].addEventListener("dragover", onDragOver);
                lists[i].addEventListener("dragenter", onDragEnter);
                lists[i].addEventListener("dragleave", onDragLeave);
                lists[i].addEventListener("drop", onDrop);
            }
        }

        function onDragStart(e) {
            draggedCard = e.currentTarget;
            sourceList = draggedCard.parentElement;
            sourceNextSibling = draggedCard.nextElementSibling;
            sourceStatus = draggedCard.getAttribute("data-status");

            draggedCard.classList.add("dragging");
            e.dataTransfer.effectAllowed = "move";
            e.dataTransfer.setData("text/plain", draggedCard.getAttribute("data-order"));
        }

        function onDragEnd() {
            if (draggedCard) {
                draggedCard.classList.remove("dragging");
            }
            removePlaceholder();
            clearDragOver();
        }

        function onDragEnter(e) {
            var col = e.currentTarget.closest(".kanban-column");
            if (col) col.classList.add("drag-over");
        }

        function onDragLeave(e) {
            var col = e.currentTarget.closest(".kanban-column");
            if (!col) return;

            var rect = col.getBoundingClientRect();
            var x = e.clientX;
            var y = e.clientY;

            if (x <= rect.left || x >= rect.right || y <= rect.top || y >= rect.bottom) {
                col.classList.remove("drag-over");
            }
        }

        function onDragOver(e) {
            e.preventDefault();
            if (!draggedCard) return;

            var list = e.currentTarget;
            var col = list.closest(".kanban-column");
            if (col) col.classList.add("drag-over");

            var afterElement = getDragAfterElement(list, e.clientY);

            if (afterElement == null) {
                list.appendChild(placeholder);
            } else {
                list.insertBefore(placeholder, afterElement);
            }
        }

        function onDrop(e) {
            e.preventDefault();
            if (!draggedCard) return;

            var targetList = e.currentTarget;
            var targetStatus = targetList.getAttribute("data-status");
            var orderNumber = draggedCard.getAttribute("data-order");
            var oldList = sourceList;
            var oldStatus = sourceStatus;

            targetList.insertBefore(draggedCard, placeholder);
            removePlaceholder();
            clearDragOver();

            draggedCard.setAttribute("data-status", targetStatus);
            draggedCard.classList.add("saving");

            refreshAllCounts();
            refreshAllEmptyStates();

            if (oldStatus === targetStatus) {
                draggedCard.classList.remove("saving");
                return;
            }

            saveKanbanStatus(orderNumber, targetStatus, function () {
                draggedCard.classList.remove("saving");
                showToast("Order moved to " + getStatusText(targetStatus), "success");
            }, function (msg) {
                rollbackCard(oldList, oldStatus);
                draggedCard.classList.remove("saving");
                showToast(msg || "Status update failed.", "error");
            });
        }

        function rollbackCard(oldList, oldStatus) {
            if (!draggedCard || !oldList) return;

            if (sourceNextSibling && sourceNextSibling.parentNode === oldList) {
                oldList.insertBefore(draggedCard, sourceNextSibling);
            } else {
                oldList.appendChild(draggedCard);
            }

            draggedCard.setAttribute("data-status", oldStatus);
            refreshAllCounts();
            refreshAllEmptyStates();
        }

        function saveKanbanStatus(orderNumber, newStatus, onSuccess, onError) {
            var xhr = new XMLHttpRequest();
            xhr.open("POST", "OrderKanban.aspx/UpdateKanbanStatus", true);
            xhr.setRequestHeader("Content-Type", "application/json; charset=utf-8");

            xhr.onreadystatechange = function () {
                if (xhr.readyState !== 4) return;

                if (xhr.status === 200) {
                    try {
                        var result = JSON.parse(xhr.responseText);
                        if (result && result.d && result.d.success) {
                            if (onSuccess) onSuccess();
                        } else {
                            if (onError) onError(result && result.d ? result.d.message : "Update failed.");
                        }
                    } catch (e) {
                        if (onError) onError("Response parse error.");
                    }
                } else {
                    if (onError) onError("Server error.");
                }
            };

            xhr.send(JSON.stringify({
                orderNumber: orderNumber,
                newStatus: newStatus
            }));
        }

        function getDragAfterElement(container, y) {
            var draggableElements = [].slice.call(container.querySelectorAll(".kanban-card:not(.dragging)"));
            var closest = { offset: Number.NEGATIVE_INFINITY, element: null };

            for (var i = 0; i < draggableElements.length; i++) {
                var child = draggableElements[i];
                var box = child.getBoundingClientRect();
                var offset = y - box.top - box.height / 2;

                if (offset < 0 && offset > closest.offset) {
                    closest = { offset: offset, element: child };
                }
            }

            return closest.element;
        }

        function removePlaceholder() {
            if (placeholder.parentNode) {
                placeholder.parentNode.removeChild(placeholder);
            }
        }

        function clearDragOver() {
            var cols = document.querySelectorAll(".kanban-column");
            for (var i = 0; i < cols.length; i++) {
                cols[i].classList.remove("drag-over");
            }
        }

        function refreshAllCounts() {
            setCount("OPEN", "countOpen", "statOpen");
            setCount("INPROGRESS", "countProgress", "statProgress");
            setCount("CLOSED", "countClosed", "statClosed");
        }

        function setCount(status, badgeId, statId) {
            var count = document.querySelectorAll('.kanban-card[data-status="' + status + '"]').length;
            var badge = document.getElementById(badgeId);
            var stat = document.getElementById(statId);

            if (badge) badge.textContent = count;
            if (stat) stat.textContent = count;
        }

        function refreshAllEmptyStates() {
            toggleEmpty("listOpen");
            toggleEmpty("listProgress");
            toggleEmpty("listClosed");
        }

        function toggleEmpty(listId) {
            var list = document.getElementById(listId);
            if (!list) return;

            var cards = list.querySelectorAll(".kanban-card");
            var empty = list.querySelector(".empty-box");

            if (!empty) return;
            empty.style.display = cards.length === 0 ? "block" : "none";
        }

        function getStatusText(status) {
            if (status === "OPEN") return "Open";
            if (status === "INPROGRESS") return "In Progress";
            if (status === "CLOSED") return "Closed";
            return status;
        }

        function showToast(message, type) {
            var wrap = document.getElementById("toastWrap");
            if (!wrap) return;

            var item = document.createElement("div");
            item.className = "toast-item " + (type || "");
            item.textContent = message;
            wrap.appendChild(item);

            setTimeout(function () {
                item.style.opacity = "0";
                item.style.transform = "translateY(8px)";
                setTimeout(function () {
                    if (item.parentNode) item.parentNode.removeChild(item);
                }, 200);
            }, 2500);
        }

        init();
    })();
</script>

</asp:Content>