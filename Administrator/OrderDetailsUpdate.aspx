<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderDetailsUpdate.aspx.cs" Inherits="Feniks.Administrator.OrderDetailsUpdate" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Order Details</title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <style>
        body { background:#f5f6f8; }
        .page-title { font-weight:700; margin-top:15px; }
        .card { background:#fff; border:1px solid #e6e6e6; border-radius:10px; padding:15px; margin-bottom:15px; }
        .metric-title { font-size:12px; color:#777; letter-spacing:.5px; text-transform:uppercase; }
        .metric-value { font-size:26px; font-weight:700; margin-top:4px; }
        .metric-sub { color:#888; font-size:12px; margin-top:3px; }
        .pln-sub { color:#666; font-size:12px; margin-top:6px; }
        .badge-pill { border-radius:999px; padding:7px 12px; font-weight:600; display:inline-block; }
        .b-preparing { background:#ffe9b5; color:#7a4b00; }
        .b-packaged { background:#d9edf7; color:#0b4f6c; }
        .b-ready { background:#d9edf7; color:#0b4f6c; }
        .b-pre-shipping { background:#cfe2ff; color:#084298; }
        .b-final { background:#d1f1df; color:#0a3d1c; }
        .b-cancel { background:#ffd6d6; color:#7a0b0b; }
        .b-default { background:#eee; color:#555; }

        .thumb { width:70px; height:70px; object-fit:cover; border-radius:8px; border:1px solid #eee; background:#fafafa; }
        .table > tbody > tr > td { vertical-align:middle; }
        .section-title { font-weight:700; margin:0 0 10px 0; }
        .note-box { background:#fafafa; border:1px dashed #ddd; border-radius:8px; padding:10px; min-height:42px; white-space:pre-wrap; }
        .top-actions { margin-top:15px; }
        .muted { color:#777; font-size:12px; }
        .kv th { width:180px; }

        .money-usd{ font-weight:800; }
        .money-pln{ color:#666; font-size:11px; margin-top:2px; }

        .mini-card{
            background:#fff; border:1px solid #eee; border-radius:10px; padding:14px;
            box-shadow: 0 6px 18px rgba(0,0,0,.06);
            margin-bottom:12px;
        }
        .mini-title{ font-size:12px; color:#777; text-transform:uppercase; letter-spacing:.4px; }
        .mini-value{ font-size:26px; font-weight:800; margin-top:6px; }
    </style>
</head>

<body>
<form id="form1" runat="server">
<div class="container-fluid" style="max-width:1400px;">

    <div class="row">
        <div class="col-xs-12">
            <div class="top-actions pull-right">
                <asp:Button ID="btnBack" runat="server" Text="Back" CssClass="btn btn-default" OnClick="btnBack_Click" />
                <asp:Button ID="btnSave" runat="server" Text="Update" CssClass="btn btn-success" OnClick="btnSave_Click" />
<asp:Button ID="btnDelete" runat="server" CssClass="btn btn-danger" Text="Delete Order"
    OnClick="btnDelete_Click"
    CausesValidation="false"
    UseSubmitBehavior="false"
    OnClientClick="confirmDelete(); return false;" />
            </div>

            <h2 class="page-title">Order Details - <asp:Label ID="lblOrderNumber" runat="server" /></h2>
            <div class="muted">
                Order Date: <asp:Label ID="lblOrderDate" runat="server" />
                &nbsp;|&nbsp; USD→PLN: <asp:Label ID="lblUsdPlnRate" runat="server" Text="-" />
            </div>
        </div>
    </div>

    <!-- Top cards -->
    <div class="row" style="margin-top:10px;">
        <div class="col-md-4">
            <div class="mini-card">
                <div class="mini-title">Total</div>
                <div class="mini-value">$ <asp:Label ID="lblTotalBig" runat="server" /></div>
                <div class="money-pln">≈ <asp:Label ID="lblTotalBigPln" runat="server" /> zł</div>
                <div class="metric-sub">Order total amount</div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="mini-card">
                <div class="mini-title">Net Profit</div>
                <div class="mini-value">$ <asp:Label ID="lblNetProfitBig" runat="server" /></div>
                <div class="money-pln">≈ <asp:Label ID="lblNetProfitBigPln" runat="server" /> zł</div>
                <div class="metric-sub">Calculated from Sales - Fees - Costs</div>
            </div>
        </div>

        <div class="col-md-4">
            <div class="mini-card">
                <div class="mini-title">Shipping Status</div>
                <div style="margin-top:10px;">
                    <asp:Label ID="lblStatusBadge" runat="server" CssClass="badge-pill b-default" Text="-" />
                </div>
                <div class="metric-sub" style="margin-top:10px;">Current workflow stage</div>
            </div>
        </div>
    </div>

    <!-- Order info -->
    <div class="card">
        <h4 class="section-title">Order Information</h4>
        <table class="table kv">
            <tr><th>Marketplace</th><td><asp:Label ID="lblMarketplace" runat="server" /></td></tr>
            <tr><th>Buyer</th><td><asp:Label ID="lblBuyer" runat="server" /></td></tr>
            <tr><th>Country</th><td><asp:Label ID="lblCountry" runat="server" /></td></tr>
            <tr><th>Shipping Type</th><td><asp:Label ID="lblShippingType" runat="server" /></td></tr>
            <tr><th>Currency</th><td><asp:Label ID="lblCurrency" runat="server" /></td></tr>
        </table>
    </div>

    <!-- Address -->
    <div class="card">
        <h4 class="section-title">Address & Contact</h4>
        <table class="table kv">
            <tr><th>Ship To</th><td><asp:Label ID="lblShipTo" runat="server" /></td></tr>
            <tr><th>Email</th><td><asp:Label ID="lblEmail" runat="server" /></td></tr>
            <tr><th>Phone</th><td><asp:Label ID="lblPhone" runat="server" /></td></tr>
        </table>
    </div>

    <!-- Items -->
    <div class="card">
        <h4 class="section-title">Items</h4>

        <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False"
            CssClass="table table-bordered table-hover"
            EmptyDataText="No item found for this order."
>
            <Columns>
                <asp:TemplateField HeaderText="Photo" ItemStyle-Width="90px">
                    <ItemTemplate>
                        <img class="thumb"
                             src='<%# ResolveUrl("~/Administrator/ProductPhoto.ashx?sku=" + HttpUtility.UrlEncode(Eval("SKU").ToString())) %>'
                             alt="photo" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="ProductType" HeaderText="Type" ItemStyle-Width="120px" />
                <asp:BoundField DataField="SKU" HeaderText="SKU" />

                <asp:BoundField DataField="Quantity" HeaderText="Qty" ItemStyle-Width="80px" />

                <asp:TemplateField HeaderText="Item Price">
                    <ItemTemplate>
                        <div class="money-usd">$ <%# Eval("ItemPrice","{0:0.00}") %></div>
                        <div class="money-pln">≈ <%# Eval("ItemPricePln","{0:0.00}") %> zł</div>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <HeaderStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <asp:BoundField DataField="RingSize" HeaderText="Ring Size" ItemStyle-Width="140px" />

                <asp:TemplateField HeaderText="Unit Cost">
                    <ItemTemplate>
                        <div class="money-usd">$ <%# Eval("UnitCostUsd","{0:0.00}") %></div>
                        <div class="money-pln">≈ <%# Eval("UnitCostPln","{0:0.00}") %> zł</div>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <HeaderStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Line Cost">
                    <ItemTemplate>
                        <div class="money-usd">$ <%# Eval("LineCostUsd","{0:0.00}") %></div>
                        <div class="money-pln">≈ <%# Eval("LineCostPln","{0:0.00}") %> zł</div>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                    <HeaderStyle HorizontalAlign="Right" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Financial Summary -->
    <div class="card">
        <h4 class="section-title">Financial Summary</h4>

        <div class="row">
            <div class="col-md-4">
                <h5>Sales (Buyer)</h5>
                <table class="table table-condensed">
                    <tr><td>Item Total</td><td>
                        $ <asp:Label ID="lblItemTotalUsd" runat="server" />
                        <div class="money-pln">≈ <asp:Label ID="lblItemTotalPln" runat="server" /> zł</div>
                    </td></tr>

                    <tr><td>Shipping (Buyer)</td><td>
                        $ <asp:Label ID="lblShipBuyerUsd" runat="server" />
                        <div class="money-pln">≈ <asp:Label ID="lblShipBuyerPln" runat="server" /> zł</div>
                    </td></tr>

                    <tr><td>Tax (Buyer)</td><td>
                        $ <asp:Label ID="lblTaxBuyerUsd" runat="server" />
                        <div class="money-pln">≈ <asp:Label ID="lblTaxBuyerPln" runat="server" /> zł</div>
                    </td></tr>

                    <tr><td>Gift Wrap</td><td>
                        $ <asp:Label ID="lblGiftUsd" runat="server" />
                        <div class="money-pln">≈ <asp:Label ID="lblGiftPln" runat="server" /> zł</div>
                    </td></tr>

                    <tr style="font-weight:bold">
                        <td>Gross Sales</td>
                        <td>
                            $ <asp:Label ID="lblGrossUsd" runat="server" />
                            <div class="money-pln">≈ <asp:Label ID="lblGrossPln" runat="server" /> zł</div>
                        </td>
                    </tr>
                </table>
            </div>

            <div class="col-md-4">
                <h5>Marketplace Fees (Estimated)</h5>

                <asp:Repeater ID="rpFees" runat="server">
                    <ItemTemplate>
                        <div>
                            <%# Eval("Label") %><br />
                            <span class="text-danger">-$ <%# Eval("AmountUsd","{0:0.00}") %></span>
                            <div class="money-pln text-muted">≈ -<%# Eval("AmountPln") %> zł</div>
                        </div>
                        <hr />
                    </ItemTemplate>
                </asp:Repeater>

                <strong>Total Fees</strong><br />
                -$ <asp:Label ID="lblFeesUsd" runat="server" />
                <div class="money-pln">≈ -<asp:Label ID="lblFeesPln" runat="server" /> zł</div>
            </div>

            <div class="col-md-4">
                <h5>Costs</h5>

                Product Cost<br />
                -$ <asp:Label ID="lblProductCostUsd" runat="server" />
                <div class="money-pln">≈ -<asp:Label ID="lblProductCostPln" runat="server" /> zł</div>
                <hr />

                Shipping Cost (Legs)<br />
                -$ <asp:Label ID="lblShipCostUsd" runat="server" />
                <div class="money-pln">≈ -<asp:Label ID="lblShipCostPln" runat="server" /> zł</div>
                <hr />

                Extra Expenses<br />
                -$ <asp:Label ID="lblExtraUsd" runat="server" />
                <div class="money-pln">≈ -<asp:Label ID="lblExtraPln" runat="server" /> zł</div>
            </div>
        </div>

        <hr />

        <div style="text-align:right">
            <h3>
                NET PROFIT<br />
                $ <asp:Label ID="lblNetProfitUsd" runat="server" />
                <div class="money-pln">≈ <asp:Label ID="lblNetProfitPln" runat="server" /> zł</div>
            </h3>
        </div>
    </div>

    <!-- Notes -->
    <div class="card">
        <h4 class="section-title">Notes</h4>
        <div class="row">
            <div class="col-md-4">
                <div class="metric-title">Buyer Notes</div>
                <div class="note-box"><asp:Label ID="lblBuyerNotes" runat="server" /></div>
            </div>
            <div class="col-md-4">
                <div class="metric-title">Seller Notes</div>
                <div class="note-box"><asp:Label ID="lblSellerNotes" runat="server" /></div>
            </div>
            <div class="col-md-4">
                <div class="metric-title">Gift Message</div>
                <div class="note-box"><asp:Label ID="lblGiftMessage" runat="server" /></div>
            </div>
        </div>
    </div>

    <!-- Shipping (Two Legs) -->
    <div class="card">
        <h4 class="section-title">Shipping - Two Legs</h4>

        <div class="row">
            <div class="col-md-6">
                <h5>Intermediate Shipping</h5>
                <label>Company</label>
                <asp:DropDownList ID="ddInterCompany" runat="server" CssClass="form-control"></asp:DropDownList>

                <label style="margin-top:10px;">Tracking Number</label>
                <asp:TextBox ID="txtInterTracking" runat="server" CssClass="form-control" />

                <label style="margin-top:10px;">Ship Date</label>
                <asp:TextBox ID="txtInterShipDate" runat="server" CssClass="form-control" TextMode="Date" />

                <label style="margin-top:10px;">Price (USD)</label>
                <asp:TextBox ID="txtInterPrice" runat="server" CssClass="form-control" />
                <div class="muted" style="margin-top:6px;">≈ <asp:Label ID="lblInterPricePln" runat="server" Text="0.00" /> zł</div>
            </div>

            <div class="col-md-6">
                <h5>Main Shipping</h5>
                <label>Company</label>
                <asp:DropDownList ID="ddMainCompany" runat="server" CssClass="form-control"></asp:DropDownList>

                <label style="margin-top:10px;">Tracking Number</label>
                <asp:TextBox ID="txtMainTracking" runat="server" CssClass="form-control" />

                <label style="margin-top:10px;">Ship Date</label>
                <asp:TextBox ID="txtMainShipDate" runat="server" CssClass="form-control" TextMode="Date" />

                <label style="margin-top:10px;">Price (USD)</label>
                <asp:TextBox ID="txtMainPrice" runat="server" CssClass="form-control" />
                <div class="muted" style="margin-top:6px;">≈ <asp:Label ID="lblMainPricePln" runat="server" Text="0.00" /> zł</div>
            </div>
        </div>

        <hr />

        <h5>Workflow Status</h5>
        <div class="row">
            <div class="col-md-6">
                <label>Shipping Status</label>
                <asp:DropDownList ID="ddShippingStatus" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>
            <div class="col-md-6">
                <label>Legacy KKID (optional)</label>
                <asp:TextBox ID="txtKKID" runat="server" CssClass="form-control" />
            </div>
        </div>
    </div>

    <!-- Extra expenses -->
    <div class="card">
        <h4 class="section-title">Extra Expenses</h4>

        <div class="row">
            <div class="col-md-2">
                <label>Type</label>
                <asp:DropDownList ID="ddExpenseType" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Customs Fee" Value="CustomsFee" />
                    <asp:ListItem Text="Replacement" Value="Replacement" />
                    <asp:ListItem Text="Reship" Value="Reship" />
                    <asp:ListItem Text="Refund Fee" Value="RefundFee" />
                    <asp:ListItem Text="Other" Value="Other" />
                </asp:DropDownList>
            </div>
            <div class="col-md-5">
                <label>Description</label>
                <asp:TextBox ID="txtExpenseDesc" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-2">
                <label>Amount (USD)</label>
                <asp:TextBox ID="txtExpenseAmount" runat="server" CssClass="form-control" Text="0" />
            </div>
            <div class="col-md-2">
                <label>Date</label>
                <asp:TextBox ID="txtExpenseDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-1">
                <label>&nbsp;</label>
                <asp:Button ID="btnAddExpense" runat="server" CssClass="btn btn-primary btn-block" Text="Add" OnClick="btnAddExpense_Click" />
            </div>
        </div>

        <div style="margin-top:12px;">
            <asp:GridView ID="gvExpenses" runat="server" AutoGenerateColumns="False"
                CssClass="table table-bordered table-hover"
                OnRowCommand="gvExpenses_RowCommand"
                EmptyDataText="No extra expenses.">
                <Columns>
                    <asp:BoundField DataField="ExpenseDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" ItemStyle-Width="120px" />
                    <asp:BoundField DataField="ExpenseType" HeaderText="Type" ItemStyle-Width="120px" />
                    <asp:BoundField DataField="Description" HeaderText="Description" />
                    <asp:TemplateField HeaderText="Amount">
                        <ItemTemplate>
                            <div class="money-usd">$ <%# Eval("AmountUsd","{0:0.00}") %></div>
                            <div class="money-pln">≈ <%# Eval("AmountPln","{0:0.00}") %> zł</div>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Right" />
                        <HeaderStyle HorizontalAlign="Right" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDel" runat="server" Text="Delete"
                                CssClass="btn btn-xs btn-danger"
                                CommandName="DEL"
                                CommandArgument='<%# Eval("OrderExtraExpenseID") %>'
                                OnClientClick="return confirm('Delete this expense?');"></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="90px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</div>
</form>

<script>
    function confirmDelete() {
        Swal.fire({
            title: 'Are you sure?',
            text: 'Are you sure you want to delete this order all records?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, Delete!',
            cancelButtonText: 'No, Cancel',
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                __doPostBack('<%= btnDelete.UniqueID %>', '');
            }
        });
    }
</script>
</body>
</html>
