<%@ Page Title="Stock | lamaX" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Stock.aspx.cs" Inherits="Feniks.Administrator.Stock" %>

<asp:Content ID="cTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Stock | lamaX
</asp:Content>

<asp:Content ID="cHead" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        /* Stock page only */
        .page-wrap{ max-width:1250px; margin:22px auto; }
        .card{ background:#fff; border-radius:14px; box-shadow:0 10px 24px rgba(0,0,0,.08); padding:16px; margin-bottom:14px; }
        .title{ font-weight:800; font-size:18px; margin:0 0 10px; }
        .muted{ color:#7a7f87; }
        .grid th{ background:#fafafa; }
        .qty { font-weight:800; }
        .qty.bad { color:#b30000; }
        .qty.ok { color:#1a7f37; }
        .btn-xs{ padding:3px 8px; }
        .help{ font-size:12px; color:#7a7f87; margin-top:6px; }
        .pill{ display:inline-block; padding:2px 8px; border-radius:999px; font-size:12px; background:#eef2ff; color:#334155; }
        .pill-a{ background:#ecfeff; color:#155e75; }
        .pill-s{ background:#f0fdf4; color:#166534; }
        .pill-n{ background:#fef9c3; color:#854d0e; }
        .modal .form-group{ margin-bottom:10px; }
    </style>
</asp:Content>

<asp:Content ID="cMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="page-wrap">

        <div class="card">
            <div class="title">Stock (Internal)</div>

            <div class="row">
                <div class="col-md-3">
                    <label>SKU contains</label>
                    <asp:TextBox ID="txtSku" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-2">
                    <label>Stock Mode</label>
                    <asp:DropDownList ID="ddlStockMode" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-3">
                    <label>Product Type</label>
                    <asp:DropDownList ID="ddlProductType" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-2">
                    <label>Location</label>
                    <asp:DropDownList ID="ddlLocationFilter" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-2" style="padding-top:25px;">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-block"
                        OnClick="btnSearch_Click" />
                </div>
            </div>

            <div class="help">
                Sized rings (<span class="pill pill-s">S</span>) show rows by size.
                Adjustable rings (<span class="pill pill-a">A</span>) show a single row (Adjustable).
                Normal products (<span class="pill pill-n">N</span>) show a single row.
                Inactive variants are hidden.
            </div>
        </div>

        <div class="card">
            <asp:Label ID="lblMsg" runat="server" CssClass="muted" />
            <div class="table-responsive">
                <asp:GridView ID="gv" runat="server" CssClass="table table-bordered table-hover grid"
                    AutoGenerateColumns="False" OnRowCommand="gv_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="SKU" HeaderText="SKU" />
                        <asp:BoundField DataField="ProductID" HeaderText="ProductID" />
                        <asp:BoundField DataField="ProductType" HeaderText="Product Type" />

                        <asp:TemplateField HeaderText="Mode">
                            <ItemTemplate>
                                <span class='<%# ModeCss(Eval("StockMode")) %>'><%# Eval("StockMode") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="VariantName" HeaderText="Variant (Size/Type)" />
                        <asp:BoundField DataField="LocationName" HeaderText="Location" />

                        <asp:TemplateField HeaderText="OnHand">
                            <ItemTemplate>
                                <span class="qty"><%# Eval("OnHandQty","{0:0.####}") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Reserved">
                            <ItemTemplate>
                                <span class="muted"><%# Eval("ReservedQty","{0:0.####}") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Available">
                            <ItemTemplate>
                                <span class='<%# (Convert.ToDecimal(Eval("AvailableQty"))<0) ? "qty bad" : "qty ok" %>'>
                                    <%# Eval("AvailableQty","{0:0.####}") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnReceive" runat="server" CssClass="btn btn-success btn-xs"
                                    CommandName="RECEIVE" CommandArgument='<%# Eval("VariantID") + "|" + Eval("LocationID") %>'>
                                    Receive
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnAdjust" runat="server" CssClass="btn btn-warning btn-xs"
                                    CommandName="ADJUST" CommandArgument='<%# Eval("VariantID") + "|" + Eval("LocationID") %>'>
                                    Adjust
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnTransfer" runat="server" CssClass="btn btn-info btn-xs"
                                    CommandName="TRANSFER" CommandArgument='<%# Eval("VariantID") + "|" + Eval("LocationID") %>'>
                                    Transfer
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>

                    <EmptyDataTemplate>
                        <div class="muted" style="padding:12px;">No records.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- ===== Modal: Receive ===== -->
        <div id="mdlReceive" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Receive Stock</h4>
              </div>
              <div class="modal-body">

                <asp:HiddenField ID="hfReceiveVariantID" runat="server" />
                <asp:HiddenField ID="hfReceiveLocationID" runat="server" />

                <div class="form-group">
                    <label>Qty</label>
                    <asp:TextBox ID="txtReceiveQty" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Unit Cost (optional)</label>
                    <asp:TextBox ID="txtReceiveUnitCost" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Currency (optional)</label>
                    <asp:TextBox ID="txtReceiveCurrency" runat="server" CssClass="form-control" Text="USD" />
                </div>

                <div class="form-group">
                    <label>RefNo (optional)</label>
                    <asp:TextBox ID="txtReceiveRefNo" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Note</label>
                    <asp:TextBox ID="txtReceiveNote" runat="server" CssClass="form-control" />
                </div>

              </div>
              <div class="modal-footer">
                <asp:Button ID="btnReceiveSave" runat="server" Text="Save" CssClass="btn btn-success"
                    OnClick="btnReceiveSave_Click" />
                <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
              </div>
            </div>
          </div>
        </div>

        <!-- ===== Modal: Adjust ===== -->
        <div id="mdlAdjust" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Adjust Stock</h4>
              </div>
              <div class="modal-body">

                <asp:HiddenField ID="hfAdjustVariantID" runat="server" />
                <asp:HiddenField ID="hfAdjustLocationID" runat="server" />

                <div class="form-group">
                    <label>Qty Delta (use negative to decrease)</label>
                    <asp:TextBox ID="txtAdjustDelta" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Note</label>
                    <asp:TextBox ID="txtAdjustNote" runat="server" CssClass="form-control" />
                </div>

              </div>
              <div class="modal-footer">
                <asp:Button ID="btnAdjustSave" runat="server" Text="Save" CssClass="btn btn-warning"
                    OnClick="btnAdjustSave_Click" />
                <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
              </div>
            </div>
          </div>
        </div>

        <!-- ===== Modal: Transfer ===== -->
        <div id="mdlTransfer" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Transfer Stock</h4>
              </div>
              <div class="modal-body">

                <asp:HiddenField ID="hfTransferVariantID" runat="server" />
                <asp:HiddenField ID="hfTransferFromLocationID" runat="server" />

                <div class="form-group">
                    <label>To Location</label>
                    <asp:DropDownList ID="ddlTransferToLocation" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Qty</label>
                    <asp:TextBox ID="txtTransferQty" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label>Note</label>
                    <asp:TextBox ID="txtTransferNote" runat="server" CssClass="form-control" />
                </div>

              </div>
              <div class="modal-footer">
                <asp:Button ID="btnTransferSave" runat="server" Text="Save" CssClass="btn btn-info"
                    OnClick="btnTransferSave_Click" />
                <button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>
              </div>
            </div>
          </div>
        </div>

    </div>

</asp:Content>

<asp:Content ID="cScripts" ContentPlaceHolderID="ScriptsContent" runat="server">
    <script>
        function openModal(id) { $('#' + id).modal('show'); }
    </script>
</asp:Content>