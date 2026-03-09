<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewOrder.aspx.cs" Inherits="Feniks.Administrator.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />  
    <!-- jQuery library -->
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>

<!-- Latest compiled JavaScript -->
<script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
<!-- Bootstrap Modal Dialog -->
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
    <table>
        <tr>
                        <td>
                <center>
                <asp:Panel ID="Panel3" runat="server" DefaultButton="btnBack">
                <button id="btnOneBack" type="button" href="#" runat="server" onserverclick="toOneBack_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-repeat" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                </asp:Panel></center>
            </td>
            <td style="width: 10px"></td>
                        <td>
                            <asp:Button ID="btnMainMenu" runat="server" class="btn btn-default btn-md" OnClick="btnMainMenu_Click" Text="Main Menu" Visible="true" Width="150px" />
                        </td>
                    </tr>
    </table>
<hr />
    <table id="tblStockAddress" runat="server" visible ="true">
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Marketplaces</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddMarketplaces" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Order Number</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtOrderNumber" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="13" placeholder=" --- please enter ---"></asp:TextBox>
            </td>
    </tr>
        <tr>
            <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Order Date</td>
                <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                    <asp:TextBox ID="txtOrderDate" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="15" placeholder=" --- please enter ---" TextMode="Date"></asp:TextBox>
                </td>
        </tr>
        <tr>
            <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Buyer Name</td>
                <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                    <asp:TextBox ID="txtBuyerName" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="15" placeholder=" --- please enter ---" TextMode="Date"></asp:TextBox>
                </td>
        </tr>
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Ship To</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtShipTo" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="15" placeholder=" --- please enter ---" TextMode="Date"></asp:TextBox>
            </td>
    </tr>
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Country</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
            <asp:DropDownList ID="ddCountry" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
        </td>
    </tr>
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">State</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
            <asp:DropDownList ID="ddState" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
        </td>
    </tr>
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">E-mail</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtEmail" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="15" placeholder=" --- please enter ---" TextMode="Date"></asp:TextBox>
            </td>
    </tr>
        <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Phone Number</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtPhoneNumber" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="15" placeholder=" --- please enter ---" TextMode="Date"></asp:TextBox>
            </td>
    </tr>
    </table>
    <hr />
    <table>
        <tr>
            <td>
                <asp:TextBox ID="txtFilterSKU" runat="server" class="form-control" Width="250px" placeholder="SKU" ></asp:TextBox>
            </td>
            <td style="width:5px"></td>
            <td>
            <asp:ImageButton ID="ImageButton8" ImageUrl="~/Images/filter.png" runat="server" Height="15px" style="width: 15px" Visible="True" />
            </td>
                                <td style="width:50px"></td>
            <td style="width:10px"></td>
        </tr>
    </table>
    <br />
    <table>
        <tr>
            <td>
                                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvNewOrder" runat="server" DataSourceID="GridDataSource" CssClass="table table-hover table-bordered" GridLines="None" EnableSortingAndPagingCallbacks="false" AutoGenerateColumns="False" EmptyDataText="SKU Not Found!" OnRowDataBound="gvNewOrder_RowDataBound" OnPageIndexChanging ="gvNewOrder_PageIndexChanging" Font-Size="11px" AllowPaging="True" PageSize="10">
        <Columns>
                                            <asp:TemplateField HeaderText=""> 
            <ItemTemplate>
                <asp:CheckBox ID="chkRow" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
            <asp:BoundField DataField="ProductPhoto" HeaderText="Photo" SortExpression="ProductPhoto" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="SKU" HeaderText="SKU" SortExpression="SKU" ItemStyle-HorizontalAlign="Left" />
                <asp:TemplateField HeaderText="Ring Size">  
                    <ItemTemplate>  
                        <asp:DropDownList ID="ddRingSize" runat="server" CssClass="form-control" AppendDataBoundItems="true">  
                        </asp:DropDownList>  
                    </ItemTemplate>  
                </asp:TemplateField> 
                <asp:TemplateField HeaderText="Quantity">  
                    <ItemTemplate>  
                        <asp:TextBox ID="txtQty" runat="server" DataSourceID="GridDataSource" Visible="true" CssClass="form-control" ForeColor="#CC3300" Width="80px" Text="0"></asp:TextBox>
                    </ItemTemplate>  
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Item Price">  
                    <ItemTemplate>  
                        <asp:TextBox ID="txtItemPrice" runat="server" DataSourceID="GridDataSource" Visible="true" CssClass="form-control" ForeColor="#CC3300" Width="80px" Text="0"></asp:TextBox>
                    </ItemTemplate>  
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Coupon Price">  
                    <ItemTemplate>  
                        <asp:TextBox ID="txtCouponPrice" runat="server" DataSourceID="GridDataSource" Visible="true" CssClass="form-control" ForeColor="#CC3300" Width="80px" Text="0"></asp:TextBox>
                    </ItemTemplate>  
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Tax">  
                    <ItemTemplate>  
                        <asp:TextBox ID="txtTax" runat="server" DataSourceID="GridDataSource" Visible="true" CssClass="form-control" ForeColor="#CC3300" Width="80px" Text="0"></asp:TextBox>
                    </ItemTemplate>  
                </asp:TemplateField>  
                <asp:TemplateField HeaderText="Shipping Price">  
                    <ItemTemplate>  
                        <asp:TextBox ID="txtShippingPrice" runat="server" DataSourceID="GridDataSource" Visible="true" CssClass="form-control" ForeColor="#CC3300" Width="80px" Text="0"></asp:TextBox>
                    </ItemTemplate>  
                </asp:TemplateField>  
                                                    <asp:TemplateField HeaderText="" Visible="False"> 
            <ItemTemplate>
                <asp:Label ID="lblSKU" runat="server" Text='<%# Eval("SKU") %>' Visible="false" ForeColor="#336699"></asp:Label>
                <asp:Label ID="lblType" runat="server" Text='<%# Eval("Type") %>' Visible="false" ForeColor="#336699"></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                                    <%--<PagerSettings Mode="NumericFirstLast" />--%>
            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/> 
                                        <PagerStyle CssClass="pagination-ys" />
    </asp:GridView>
                            </ContentTemplate>
                                                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvNewOrder" EventName="PageIndexChanging" />
                </Triggers>
        </asp:UpdatePanel>
                <asp:SqlDataSource ID="GridDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>"
    SelectCommand="SELECT * FROM V_Product order by SKU asc" FilterExpression="(SKU Like '{0}%'">
    <FilterParameters>        
        <asp:ControlParameter Name="SKU" ControlID="txtFilterSKU" PropertyName="Text" ConvertEmptyStringToNull="false"/>
    </FilterParameters>
</asp:SqlDataSource>
            </td>
            </tr>
        <tr>
            <td>
                                                    <asp:Button ID="btnAddList" runat="server" class="btn btn-primary btn-md" OnClick="btnAddList_Click" Text="Add" Visible="true" Width="120px" />
                <br id="space1" runat="server" visible="false"/>
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
        <asp:GridView ID="gvNewOrder2" runat="server" CssClass="table table-hover table-bordered" GridLines="None" OnRowCommand="gvNewOrder2_RowCommand"
    AutoGenerateColumns="false" Font-Size="10px">
    <Columns>
        <asp:BoundField DataField="ProductPhoto" HeaderText="ProductPhoto" SortExpression="ProductPhoto" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Type" HeaderText="Type" SortExpression="Type" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="SKU" HeaderText="SKU" SortExpression="SKU" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField DataField="RingSize" HeaderText="Ring Size" SortExpression="RingSize" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" SortExpression="Quantity" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="ItemPrice" HeaderText="Item Price" SortExpression="ItemPrice" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="CouponPrice" HeaderText="Coupon Price" SortExpression="CouponPrice" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="Tax" HeaderText="Tax" SortExpression="Tax" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="ShippingPrice" HeaderText="Shipping Price" SortExpression="ShippingPrice" ItemStyle-HorizontalAlign="Left" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkUpdate" Text="Delete" runat="server" OnClientClick="return confirm('Are you sure?');" CommandName="DeleteRow" CommandArgument='<%# Bind("SatCreationDynamicID") %>'/>
                        </ItemTemplate>
                    </asp:TemplateField>
    </Columns>
                        <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                                    <PagerSettings Mode="NumericFirstLast" />
                                        <PagerStyle CssClass="pagination-ys" />
</asp:GridView>
                                </ContentTemplate>
        </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <table id="tblSaveButton" runat="server" visible="false">
        <tr>
            <td style="width: 200px; height:22px; color:#003366; font-family:Arial, Helvetica, sans-serif; font-weight: bold;" ></td>
            <td  style="width:1000px"></td>
            <td>
                <asp:Button ID="btnSatCreationSave" runat="server" class="btn btn-success" style="width:100px" Text="Save" OnClick="btnNewOrderSave_Click" />
            </td>
            <td style="width:30px"></td>
            <td>
                <asp:Button ID="btnSatCreationCancel" runat="server" class="btn btn-danger" style="width:100px" Text="Cancel" OnClick="btnNewOrderCancel_Click" />
            </td>
        </tr>
    </table>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
