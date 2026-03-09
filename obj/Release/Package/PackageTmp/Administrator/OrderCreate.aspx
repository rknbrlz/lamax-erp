<%@ Page Title="Create Order" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="OrderCreate.aspx.cs" Inherits="Feniks.Administrator.OrderCreate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <meta http-equiv="X-UA-Compatible" content="IE=edge" />

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">

    <style>
        .section-title { margin-top: 15px; }
        .form-label { font-weight: bold; color: #003366; }
        .money { text-align: right; }
        .table td { vertical-align: middle !important; }
        .top-actions { margin: 10px 0; }
        .panel-heading small { color: #777; }
        .disabled { pointer-events: none; }
    </style>

    <div class="container" style="max-width:1200px;">

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert" style="margin-top:10px;">
            <asp:Label ID="lblMsg" runat="server"></asp:Label>
        </asp:Panel>

        <div class="top-actions">
            <asp:LinkButton ID="btnBack" runat="server" CssClass="btn btn-default" OnClick="btnBack_Click">
                <i class="glyphicon glyphicon-repeat"></i> Back
            </asp:LinkButton>

            <asp:LinkButton ID="btnMainMenu" runat="server" CssClass="btn btn-default" OnClick="btnMainMenu_Click" style="margin-left:8px;">
                <i class="glyphicon glyphicon-th-large"></i> Main Menu
            </asp:LinkButton>

            <asp:LinkButton ID="btnOrdersList" runat="server" CssClass="btn btn-default" OnClick="btnOrdersList_Click" style="margin-left:8px;">
                <i class="glyphicon glyphicon-list"></i> Orders List
            </asp:LinkButton>
        </div>

        <div class="panel panel-default">
            <div class="panel-heading">
                <strong>Step 1 — Order Details</strong> <small>Header + customer + products</small>
            </div>
            <div class="panel-body">
                <div class="row">

                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="form-label">Marketplace</label>
                            <asp:DropDownList ID="ddMarketplaces" runat="server" CssClass="form-control"></asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Order Number</label>
                            <asp:TextBox ID="txtOrderNumber" runat="server" CssClass="form-control"
                                MaxLength="50" AutoPostBack="true" OnTextChanged="txtOrderNumber_TextChanged"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Order Date</label>
                            <asp:TextBox ID="txtOrderDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Buyer Name</label>
                            <asp:TextBox ID="txtBuyerName" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="col-md-6">
                        <div class="form-group">
                            <label class="form-label">Ship To</label>
                            <asp:TextBox ID="txtShipTo" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="500"></asp:TextBox>
                        </div>

                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="form-label">Country</label>
                                    <asp:DropDownList ID="ddCountry" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="form-label">State</label>
                                    <asp:DropDownList ID="ddState" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="form-label">E-mail</label>
                                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="80"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="form-label">Phone</label>
                                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="form-group">
                            <label class="form-label">Buyer Message</label>
                            <asp:TextBox ID="txtBuyerNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label class="form-label" style="color:#006600;">Seller Notes</label>
                            <asp:TextBox ID="txtSellerNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" MaxLength="500"></asp:TextBox>
                        </div>
                    </div>

                </div>

                <hr />

                <div class="row">
                    <div class="col-md-4">
                        <label class="form-label">Product Search (SKU)</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtFilterSKU" runat="server" CssClass="form-control" placeholder="type SKU..."></asp:TextBox>
                            <span class="input-group-btn">
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-default" OnClick="btnSearch_Click">
                                    <i class="glyphicon glyphicon-search"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-default" OnClick="btnClear_Click">
                                    <i class="glyphicon glyphicon-remove"></i>
                                </asp:LinkButton>
                            </span>
                        </div>
                    </div>

                    <div class="col-md-8 text-right" style="padding-top:24px;">
                        <asp:LinkButton ID="btnAddSelected" runat="server" CssClass="btn btn-primary"
                            OnClick="btnAddSelected_Click">
                            <i class="glyphicon glyphicon-plus"></i> Add Selected
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnGoStep2" runat="server" CssClass="btn btn-success"
                            OnClick="btnGoStep2_Click" style="margin-left:8px;">
                            Next <i class="glyphicon glyphicon-arrow-right"></i>
                        </asp:LinkButton>
                    </div>
                </div>

                <br />

                <asp:GridView ID="gvProducts" runat="server"
                    CssClass="table table-bordered table-hover table-condensed"
                    AutoGenerateColumns="False"
                    AllowPaging="True"
                    PageSize="10"
                    OnPageIndexChanging="gvProducts_PageIndexChanging"
                    OnRowDataBound="gvProducts_RowDataBound"
                    DataKeyNames="SKU,ProductType,ProductTypeID"
                    GridLines="None">

                    <Columns>

                        <asp:TemplateField HeaderText="" ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkRow" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Photo" ItemStyle-Width="60px" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Image ID="imgPhoto" runat="server" Width="35" Height="35" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Product Type" ItemStyle-Width="140px">
                            <ItemTemplate>
                                <asp:Label ID="lblProductType" runat="server" Text='<%# Eval("ProductType") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-Width="140px" />

                        <asp:TemplateField HeaderText="Ring Size" ItemStyle-Width="160px">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddRingSize" runat="server" CssClass="form-control" Width="150px"></asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Qty" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtQty" runat="server" CssClass="form-control" Width="70px" Text="1"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Item Price" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:TextBox ID="txtItemPrice" runat="server" CssClass="form-control money" Width="100px" Text="0"></asp:TextBox>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                    <EmptyDataRowStyle ForeColor="Red" />
                    <HeaderStyle BackColor="#F4F4F4" />
                </asp:GridView>

                <h4 class="section-title">Added Items</h4>
                <asp:GridView ID="gvAdded" runat="server"
                    CssClass="table table-bordered table-hover table-condensed"
                    AutoGenerateColumns="False"
                    OnRowCommand="gvAdded_RowCommand"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-Width="140px" />
                        <asp:BoundField DataField="RingSize" HeaderText="Ring Size" ItemStyle-Width="120px" />
                        <asp:BoundField DataField="Quantity" HeaderText="Qty" ItemStyle-Width="60px" />
                        <asp:BoundField DataField="ItemPrice" HeaderText="Item Price" ItemStyle-Width="100px" />

                        <asp:TemplateField HeaderText="" ItemStyle-Width="80px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnDel" runat="server"
                                    CssClass="btn btn-xs btn-danger"
                                    CommandName="DeleteRow"
                                    CommandArgument='<%# Eval("OrderCreationDynamicID") %>'
                                    OnClientClick="return confirm('Will be deleted. Are you sure?');">
                                    Delete
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle ForeColor="Red" />
                    <HeaderStyle BackColor="#F4F4F4" />
                </asp:GridView>

            </div>
        </div>

        <asp:Panel ID="pnlStep2" runat="server" Visible="false">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <strong>Step 2 — Payment / Shipping</strong> <small>currency + totals</small>
                </div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label class="form-label">Currency</label>
                                <asp:DropDownList ID="ddCurrency" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <label class="form-label">Shipping Type</label>
                                <asp:DropDownList ID="ddShippingType" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <label class="form-label">Coupon Price</label>
                                <asp:TextBox ID="txtCouponPrice" runat="server" CssClass="form-control money" Text="0"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <label class="form-label">Shipping Price</label>
                                <asp:TextBox ID="txtShippingPrice" runat="server" CssClass="form-control money" Text="0"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label class="form-label">Tax</label>
                                <asp:TextBox ID="txtTax" runat="server" CssClass="form-control money" Text="0"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <label class="form-label">Gift Wrap</label>
                                <asp:TextBox ID="txtGiftWrapPrice" runat="server" CssClass="form-control money" Text="0"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label class="form-label">Gift Message</label>
                                <asp:TextBox ID="txtGiftMessage" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" MaxLength="500"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="text-right">
                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click">
                            <i class="glyphicon glyphicon-floppy-disk"></i> Save
                        </asp:LinkButton>

                        <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" style="margin-left:8px;">
                            Cancel
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:Label ID="lblLoginName" runat="server" Visible="false" />

    </div>

</asp:Content>