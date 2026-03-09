<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderDetails.aspx.cs" Inherits="Feniks.Administrator.OrderDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />  
    <!-- jQuery library -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Latest compiled JavaScript -->
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <!-- Bootstrap Modal Dialog -->
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">
    <style>
    .vl {
    border-left: 1px solid grey;
    height: 10px;
        }
    </style>
    <br />
    <br />
<div class="container">
  <div class="panel panel-default">
    <div class="panel-body">
    <table>
        <tr>
                                    <td>
                <center>
                                <button id="btnOneBack" type="button" href="#" runat="server" onserverclick="toOneBack_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-repeat" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
               </center>
            </td>
            <td style="width: 10px"></td>
            <td>
                    <asp:Button ID="btnMainMenu" runat="server" class="btn btn-default btn-md" OnClick="btnMainMenu_Click" Text="Main Menu" Visible="true" Width="150px" />
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
            <center>
                <img runat="server" visible="false" src="/Administrator/imagesnew/Status1.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Status1"/>
                <img runat="server" visible="false" src="/Administrator/imagesnew/Status234.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Status234"/>
                <img runat="server" visible="false" src="/Administrator/imagesnew/Status5.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Status5"/>
                <img runat="server" visible="false" src="/Administrator/imagesnew/Status6.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Status6"/>
                <img runat="server" visible="false" src="/Administrator/imagesnew/Status7.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Status7"/>
                <img runat="server" visible="false" src="/Administrator/imagesnew/Status8.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Status8"/>
            </center>
    <br />
    <center>
        <div class="container">
            <div class="panel panel-default">
                <div class="panel-body">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label15" runat="server" Text="Order ID: #"></asp:Label>
                            </td>
                            <td style="width: 5px;"></td>
                            <td>
                                <asp:Label ID="lblOrderNumber" runat="server" Text="Label" Font-Bold="True"></asp:Label>
                            </td>
                            <td style="width: 5px;"></td>
                            <td>
                                <div class="vl"></div>
                            </td>
                            <td style="width: 5px;"></td>
                            <td>
                                <asp:Label ID="Label18" runat="server" Text="Order Date:"></asp:Label>
                            </td>
                            <td style="width: 5px;"></td>
                                                        <td>
                                <asp:Label ID="lblOrderDate" runat="server" Text="Label" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                    </table>
        <strong style="text-align: center">
            
        </strong>
                    </div>
                </div>
            </div>
    </center>
    <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                <table>
                    <tr>
                        <td>
                            <center>
                            <table>
                                <tr>
                                    <td>
                                        <center>
                                        <img runat="server" visible="true" src="/Administrator/imagesnew/8666609_user_icon.png" style="text-align: center; vertical-align: middle; height: 65px;" id="Img1"/>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 25px;"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <center>
                                        <asp:Label ID="lblBuyerFullName" runat="server" Text="Label" ForeColor="#666666"></asp:Label>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 5px;"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <center>
                                        <asp:Label ID="lblemail" runat="server" Text="Label" ForeColor="#666666" Font-Size="X-Small"></asp:Label>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 5px;"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <center>
                                        <asp:Label ID="lblPhoneNumber" runat="server" Text="Label" ForeColor="#666666" Font-Size="X-Small"></asp:Label>
                                        </center>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 5px;"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <center>
                                        <img runat="server" visible="true" src="/Administrator/imagesnew/stargrey.png" style="text-align: center; vertical-align: middle; height: 15px;" id="ImgStarGrey"/>
                                        <img runat="server" visible="true" src="/Administrator/imagesnew/star.png" style="text-align: center; vertical-align: middle; height: 15px;" id="ImgStar"/>
                                        </center>
                                    </td>
                                </tr>
                            </table>
                            </center>
                        </td>
                        <td style="width: 100px;"></td>
                        <td>
                            <table style="vertical-align: top; text-align: left;">
                             <tr>
                                 <td>
                                     <asp:Label ID="Label2" runat="server" Text="Ship To (Address)" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                 </td>
                             </tr>
                             <tr>
                                <td style="height: 5px;"></td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="lblShipTo" runat="server" Text="Ship To (Address)" ForeColor="#666666" Font-Size="X-Small" Font-Bold="False"></asp:Label>
                                 </td>
                             </tr>
                             <tr>
                                <td style="height: 30px;"></td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="Label1" runat="server" Text="Country" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td style="width: 25px;"></td>
                                                                  <td>
                                     <asp:Label ID="Label4" runat="server" Text="State" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                 </td>
                                 <td style="width: 15px;"></td>
                                 <td style="width: 15px;"></td>
                                 <td style="width: 100px;"></td>
                                                                  <td>
                                     <asp:Label ID="Label25" runat="server" Text="Marketplace" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                 </td>
                             </tr>
                             <tr>
                                <td style="height: 5px;"></td>
                             </tr>
                             <tr>
                                 <td>
                                     <asp:Label ID="lblCountry" runat="server" Text="Ship To (Address)" ForeColor="#666666" Font-Size="X-Small" Font-Bold="False"></asp:Label>
                                 </td>
                                 <td style="width: 25px;"></td>
                                                                  <td>
                                     <asp:Label ID="lblState" runat="server" Text="Ship To (Address)" ForeColor="#666666" Font-Size="X-Small" Font-Bold="False"></asp:Label>
                                 </td>
                                 <td style="width: 15px;"></td>
                                 <td>
                                 <asp:Label ID="lblStateShort" runat="server" Text="Ship To (Address)" ForeColor="#666666" Font-Size="X-Small" Font-Bold="False"></asp:Label>
                                 </td>
                                 <td style="width: 100px;"></td>
                                 <td>
                                 <asp:Label ID="lblMarketplace" runat="server" Text="Ship To (Address)" ForeColor="#666666" Font-Size="X-Small" Font-Bold="False"></asp:Label>
                                 </td>
                             </tr>
                         </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
        <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                    <asp:GridView ID="gvOrderDetail" runat="server" class="table table-bordered table-condensed table-responsive table-hover" OnRowDataBound="gvOrderDetail_RowDataBound" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Order is not available." Font-Size="12px" AllowPaging="True" PageSize="20" BorderStyle="None">
        <Columns>
                        <asp:TemplateField>
            <ItemTemplate>
            <asp:Image ID="ProductPhoto" runat="server" Height="30px" Width="30px" CssClass = "pic" onclick="ShowFull(this)" ondoubleclick="ShowZoomin(this)" style = "cursor:pointer;margin:10px"/>
            </ItemTemplate>
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="SKU" Visible="True" >
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="ProductType" Visible="True" >
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="RingSize" HeaderText="Ring Size" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="RingSize" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Quantity" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Item Price" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="100px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="$" Visible="true" ForeColor="#666666" Font-Size="Small" Font-Bold="false"></asp:Label> 
                        </td>
                        <td style="width:100px">
                            <asp:Label ID="lblItemPrice" runat="server" Text='<%# Eval("ItemPrice") %>' Visible="true" ForeColor="#666666" Font-Size="Small" Font-Bold="false"></asp:Label> 
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="100px" />
            </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
    </asp:GridView>
                </div>
            </div>
            </div>
        <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                <table>
                    <tr>
                        <td style="width:300px">
                            <asp:Label ID="Label5" runat="server" Text="Item Total" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label6" runat="server" Text="$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblItemTotal" runat="server" Text="120" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label7" runat="server" Text="Coupon Total" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label8" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblCouponPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label10" runat="server" Text="Subtotal" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label11" runat="server" Text="$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblSubTotal" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label13" runat="server" Text="Shipping Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label14" runat="server" Text="$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblShippingPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label19" runat="server" Text="Gift Wrapping" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label20" runat="server" Text="$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblGiftWrapPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label16" runat="server" Text="Tax" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label17" runat="server" Text="$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblTax" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                        <td>
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label22" runat="server" Text="Order Total" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                        </td>
                        <td style="width:710px">
                        <td style="text-align: right">
                            <asp:Label ID="Label23" runat="server" Text="$" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblOrderTotal" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                        </td>
                    </tr>
                </table>
                </div>
            </div>
            </div>
        <div class="container">
            <div class="panel panel-default">
                <div class="panel-body">
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label9" runat="server" Text="Gift Message" Font-Size="Small" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblGiftMessage" runat="server" Text="Label" Font-Size="Small" Font-Bold="False" ForeColor="#FF3300"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label12" runat="server" Text="Buyer Message" Font-Size="Small" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblBuyerMsg" runat="server" Text="Label" Font-Size="Small" Font-Bold="False" ForeColor="#FF3300"></asp:Label>               
                            </td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="sasd" runat="server" Text="Seller Message" Font-Size="Small" Font-Bold="True"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblSellerMsg" runat="server" Text="Label" Font-Size="Small" Font-Bold="False" ForeColor="#FF3300"></asp:Label>               
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
            <div class="container">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label21" runat="server" Text="Carrier" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                                <td style="width:20px"></td>
                                <td>
                                    <asp:Label ID="lblCarrier" runat="server" Text="20" ForeColor="#333333" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:50px"></td>
                                <td>
                                    <asp:Label ID="Label26" runat="server" Text="Tracking ID" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                                <td style="width:20px"></td>
                                <td>
                                    <asp:Label ID="lblTrackingNumber" runat="server" Text="20" ForeColor="#333333" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="height:20px"></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label24" runat="server" Text="Ship Date" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                                <td style="width:20px"></td>
                                <td>
                                    <asp:Label ID="lblShipDate" runat="server" Text="20" ForeColor="#333333" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:50px"></td>
                                <td>
                                    <asp:Label ID="Label27" runat="server" Text="KK ID" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                                <td style="width:20px"></td>
                                <td>
                                    <asp:Label ID="lblKKID" runat="server" Text="20" ForeColor="#333333" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="container">
                <div class="panel panel-default" style="background-color: #F9FCFF">
                    <div class="panel-body">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label28" runat="server" Text="Total Product Cost" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label29" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblProductCost" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label52" runat="server" Text="Shipment Expenses" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label53" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblShipmentExpenses" runat="server" Text="0,00" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td style="width:300px">
                                    <asp:Label ID="Label54" runat="server" Text="Marketplace Taxes and Commissions" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label55" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblMarketplaceTaxesandCommissions" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label50" runat="server" Text="Other Expenses" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label51" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblOtherExpenses" runat="server" Text="0,00" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label56" runat="server" Text="Refund" ForeColor="#FFCC00" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label57" runat="server" Text="-$" ForeColor="#FFCC00" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblRefund" runat="server" Text="0,00" ForeColor="#FFCC00" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label32" runat="server" Text="Box Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label33" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblBoxPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label34" runat="server" Text="Bag Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label35" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblBagPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label36" runat="server" Text="Envelope Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label37" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblEnvelopePrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label38" runat="server" Text="Jewelry Card Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label39" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblJewelryCardPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label40" runat="server" Text="FixCard Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label41" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblFixCardPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label42" runat="server" Text="Unicef Card Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label43" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblUnicefCardPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label44" runat="server" Text="Circle Sticker Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label45" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblCircleStickerPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label46" runat="server" Text="Envelope Sticker Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label47" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblEnvelopeStickerPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label48" runat="server" Text="Kargom Kolay Sticker Price" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label49" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblKargomKolayStickerPrice" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label30" runat="server" Text="Total Pack Expenses" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label31" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblPackExpenses" runat="server" Text="20" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                                <td>
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label60" runat="server" Text="Total Expenses" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                                <td style="width:710px">
                                <td style="text-align: right">
                                    <asp:Label ID="Label61" runat="server" Text="-$" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="Label62" runat="server" Text="0,00" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div class="container">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <center>
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="Label58" runat="server" Text="$" ForeColor="#666666" Font-Size="70px" Font-Bold="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblProfit" runat="server" Text="20" ForeColor="#666666" Font-Size="70px" Font-Bold="false"></asp:Label>
                                </td>
                                <td style="width:100px"></td>
                                <td>
                                    <img runat="server" visible="true" src="/Administrator/imagesnew/speedmeter10.png" style="text-align: center; vertical-align: middle; height: 100px;" id="speedmeter10"/>
                                    <img runat="server" visible="false" src="/Administrator/imagesnew/speedmeter20.png" style="text-align: center; vertical-align: middle; height: 100px;" id="speedmeter20"/>
                                    <img runat="server" visible="false" src="/Administrator/imagesnew/speedmeter30.png" style="text-align: center; vertical-align: middle; height: 100px;" id="speedmeter30"/>
                                    <img runat="server" visible="false" src="/Administrator/imagesnew/speedmeter40.png" style="text-align: center; vertical-align: middle; height: 100px;" id="speedmeter40"/>
                                    <img runat="server" visible="false" src="/Administrator/imagesnew/speedmeter50.png" style="text-align: center; vertical-align: middle; height: 100px;" id="speedmeter50"/>
                                </td>
                                <td style="width:100px"></td>
                               <td>
                                    <asp:Label ID="Label59" runat="server" Text="%" ForeColor="#666666" Font-Size="70px" Font-Bold="false"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="lblProfitPercentage" runat="server" Text="20" ForeColor="#666666" Font-Size="70px" Font-Bold="false"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        </center>
                    </div>
                </div>
            </div>
        <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                    <asp:GridView ID="gvReview" runat="server" class="table table-bordered table-condensed table-responsive table-hover" OnRowDataBound="gvReview_RowDataBound" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Review is not available." Font-Size="12px" AllowPaging="True" PageSize="20" BorderStyle="None">
        <Columns>
                        <asp:TemplateField ItemStyle-Width="100px">
            <ItemTemplate>
            <asp:Image ID="ProductPhoto" runat="server" Height="30px" Width="30px" CssClass = "pic" onclick="ShowFull(this)" ondoubleclick="ShowZoomin(this)" style = "cursor:pointer;margin:10px"/>
            </ItemTemplate>
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:TemplateField>
            <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="SKU" Visible="True" ItemStyle-Width="50px">
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="ProductType" Visible="True" ItemStyle-Width="80px">
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="RingSize" HeaderText="Ring Size" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="RingSize" ItemStyle-Width="80px">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Quantity" HeaderText="Quantity" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Quantity" ItemStyle-Width="80px">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Star" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="50px" HeaderStyle-BackColor="White" > 
            <ItemTemplate>
                <table>
                    <tr>
                        <td>
                        <asp:Label ID="lblStar" runat="server" Text='<%# Eval("ReviewStar") %>' Visible="false" ForeColor="#666666" Font-Size="Small" Font-Bold="false"></asp:Label> 
                        <asp:Image ID="rating0" ImageUrl="~/Administrator/imagesnew/rating0.png" runat="server" Height="50px" ToolTip="0" Visible="true" />
                        <asp:Image ID="rating1" ImageUrl="~/Administrator/imagesnew/rating1.png" runat="server" Height="50px" ToolTip="1" Visible="false" /> 
                        <asp:Image ID="rating2" ImageUrl="~/Administrator/imagesnew/rating2.png" runat="server" Height="50px" ToolTip="2" Visible="false" /> 
                        <asp:Image ID="rating3" ImageUrl="~/Administrator/imagesnew/rating3.png" runat="server" Height="50px" ToolTip="3" Visible="false" /> 
                        <asp:Image ID="rating4" ImageUrl="~/Administrator/imagesnew/rating4.png" runat="server" Height="50px" ToolTip="4" Visible="false" /> 
                        <asp:Image ID="rating5" ImageUrl="~/Administrator/imagesnew/rating5.png" runat="server" Height="50px" ToolTip="5" Visible="false" /> 
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="50px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Review" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="400px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td>
                        <asp:Label ID="lblReviewDetails" runat="server" Text='<%# Eval("ReviewDetails") %>' Visible="false" ForeColor="#FF9966" Font-Size="Small" Font-Bold="false"></asp:Label>
                        <asp:Label ID="lblNoReview" runat="server" Text="There is no review" Visible="true" ForeColor="#FF9966" Font-Size="Small" Font-Bold="false"></asp:Label>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="400px" />
            </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
    </asp:GridView>
                </div>
            </div>
            </div>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblStatusID" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblRepeatBuyerCheck" runat="server" Text="Label" Visible="False"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
