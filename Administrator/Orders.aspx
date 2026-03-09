
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Orders.aspx.cs" Inherits="Feniks.Administrator.Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />  
    <!-- jQuery library -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Latest compiled JavaScript -->
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <!-- Bootstrap Modal Dialog -->
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

    <style>
    body {background-color:  #EFEFEF;} 
</style>
            <script type="text/javascript">
        var soundObject = null;
        function PlaySound() {
            if (soundObject != null) {
                document.body.removeChild(soundObject);
                soundObject.removed = true;
                soundObject = null;
            }
            soundObject = document.createElement("embed");
            soundObject.setAttribute("src", "sounds/chaching.wav");
            soundObject.setAttribute("hidden", true);
            soundObject.setAttribute("autostart", true);
            document.body.appendChild(soundObject);
            }
            window.onload = function () {
                PlaySound();
            }
            </script>
    <script>
        $(document).ready(function () {

            $(".order-number-link").click(function () {

                var orderNumber = $(this).data("ordernumber");
                var market = $(this).data("market");
                var buyer = $(this).data("buyer");
                var country = $(this).data("country");
                var ship = $(this).data("ship");
                var total = $(this).data("total");
                var profit = $(this).data("profit");
                var currency = $(this).data("currency");
                var status = $(this).data("status");

                var ShippingCompanyID = $(this).data("ShippingCompanyID");
                var kkid = $(this).data("kkid");
                var TrackingNumber = $(this).data("TrackingNumber");
                var ShipDate = $(this).data("ShipDate");
                var ShippingPrice = $(this).data("ShippingPrice");

                $("#modalOrderNumber").text(orderNumber);
                $("#modalMarketplace").text(market);
                $("#modalBuyer").text(buyer);
                $("#modalCountry").text(country);
                $("#modalShip").text(ship);
                $("#modalTotal").text(total);
                $("#modalProfit").text(profit);
                $("#modalCurrency").text(currency);
                $("#modalStatus").text(status);

                $("#txtShippingCompanyID").val(ShippingCompanyID || "");
                $("#txtKKID").val(kkid || "");
                $("#txtTrackingNumber").val(TrackingNumber || "");
                $("#txtShipDate").val(ShipDate || "");
                $("#txtShippingPrice").val(ShippingPrice || "");

                $("#<%=hfOrderNumberModal.ClientID%>").val(orderNumber);

    $("#orderDetailsModal").modal("show");
});

        });

        function saveShipping() {
            var orderNumber = $("#<%=hfOrderNumberModal.ClientID%>").val(orderNumber);

            // Tarih dönüşümü
            var rawDate = $("#txtShipDate").val();
            var formattedDate = rawDate;
            if (rawDate.indexOf(".") !== -1) {
                var parts = rawDate.split(".");
                formattedDate = parts[2] + "-" + parts[1] + "-" + parts[0];
            }

            var shippingData = {
                orderNumber: orderNumber,
                ShippingCompanyID: $("#txtShippingCompanyID").val(),
                KKID: $("#txtKKID").val(),
                TrackingNumber: $("#txtTrackingNumber").val(),
                ShipDate: formattedDate,
                ShippingPrice: $("#txtShippingPrice").val()
            };

            $.ajax({
                type: "POST",
                url: "Orders.aspx/SaveShippingInfo",
                data: JSON.stringify(shippingData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    alert("Shipping info başarıyla kaydedildi!");
                    $("#orderDetailsModal").modal("hide");
                    __doPostBack('gvOrder', '');
                },
                error: function (xhr, status, error) {
                    console.error("AJAX Hata:", xhr.responseText);
                    alert("Shipping bilgisi kaydedilirken hata oluştu:\n" + error);
                }
            });
        }


    </script>

    <style type="text/css">
        .pagination-ys {
    /*display: inline-block;*/
    padding-left: 0;
    margin: 20px 0;
    border-radius: 4px;
}

        /* modal-body scrollable */
#orderDetailsModal .modal-body {
    max-height: calc(100vh - 200px) !important;
    overflow-y: auto !important;
}


.pagination-ys table > tbody > tr > td {
    display: inline;
}

.pagination-ys table > tbody > tr > td > a,
.pagination-ys table > tbody > tr > td > span {
    position: relative;
    float: left;
    padding: 8px 12px;
    line-height: 1.42857143;
    text-decoration: none;
    color: #dd4814;
    background-color: #ffffff;
    border: 1px solid #dddddd;
    margin-left: -1px;
}

.pagination-ys table > tbody > tr > td > span {
    position: relative;
    float: left;
    padding: 8px 12px;
    line-height: 1.42857143;
    text-decoration: none;    
    margin-left: -1px;
    z-index: 2;
    color: #aea79f;
    background-color: #f5f5f5;
    border-color: #dddddd;
    cursor: default;
}

.pagination-ys table > tbody > tr > td:first-child > a,
.pagination-ys table > tbody > tr > td:first-child > span {
    margin-left: 0;
    border-bottom-left-radius: 4px;
    border-top-left-radius: 4px;
}

.pagination-ys table > tbody > tr > td:last-child > a,
.pagination-ys table > tbody > tr > td:last-child > span {
    border-bottom-right-radius: 4px;
    border-top-right-radius: 4px;
}

.pagination-ys table > tbody > tr > td > a:hover,
.pagination-ys table > tbody > tr > td > span:hover,
.pagination-ys table > tbody > tr > td > a:focus,
.pagination-ys table > tbody > tr > td > span:focus {
    color: #97310e;
    background-color: #eeeeee;
    border-color: #dddddd;
}
        .txtbox
        {
            border-top-left-radius: 100px;
            border-top-right-radius: 100px;
            border-bottom-left-radius: 100px;
            border-bottom-right-radius: 100px;

            behavior: url(/css/border-radius.htc);
            border-radius: 100px;
        }
            </style>
    <style type="text/css">
.label_middlex {
  vertical-align:middle;
 }
        .auto-style2 {
            height: 47px;
        }
        .auto-style3 {
            width: 10px;
            height: 47px;
        }
    </style>
    <br />
<%--    <table id="tbl1" runat="server" align="center">
        <tr>
            <td>--%>
<div class="full-width">
  <div class="panel panel-default">
    <div class="panel-body">
            <table>
        <tr>
            <td>
                <asp:Panel ID="Panel1" runat="server" DefaultButton="btnBack">
                    <asp:Button ID="btnBack" runat="server" class="btn btn-default btn-md" OnClick="btnBack_Click" Text="Main Menu" Visible="true" Width="150px" />
                </asp:Panel>
            </td>
            <td style="width:15px"></td>
            <td>
                <asp:Button ID="btnNewOrder" runat="server" class="btn btn-success" OnClick="btnNewOrder_Click" Text="Place New Order" Visible="true" Width="150px" />
            </td>
            <td style="width:15px"></td>
            <td>
                <asp:Panel ID="Panel2" runat="server" DefaultButton="btnBack">
                    <asp:Button ID="btnShipping" runat="server" class="btn btn-info btn-md" OnClick="btnShipping_Click" Text="Shipping Actions" Visible="true" Width="150px" />
                </asp:Panel>
            </td>
            <td style="width:15px"></td>
            <td>
<asp:Button ID="btnRefresh" runat="server"
    CssClass="btn btn-primary"
    Text="&#x21bb; Refresh List"
    Width="150px"
    OnClick="btnRefresh_Click" />
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
<%--            </td>
        </tr>
    </table>--%>
    <center>
    <table>
        <tr>
            <td style="text-align: center">
    <div class="full-width" style="border: thin none #FFFFFF; background-color: #EFEFEF; text-align: center;">
  <div class="panel panel-default" style="border: thin none #FFFFFF; background-color: #EFEFEF; text-align: center;">
    <div class="panel-body" style="border: thin none #FFFFFF; background-color: #EFEFEF; text-align: center;">
    <table id="tbl2" style="text-align: center">
        <tr>
            <td>
                <div class="well" style="font-size: x-small; color: #333333; font-weight: bold; background-color: #FFFFFF; text-align: left; height:200px; width:270px">Order Revenue
            <center>
                <table>
                    <tr>
                        <td style="height:15px"></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle" class="auto-style2">
                        <asp:Label ID="lblTotalOrder" runat="server" Text='<%# Eval("TotalPrice") %>' Visible="true" Font-Bold="True" Font-Size="25pt" ForeColor="#336699"></asp:Label> 
                        </td>
                        <td class="auto-style3"></td>
                        <td style="font-size: xx-large; color: #666666; font-family: Arial;" class="auto-style2">
                            <asp:Label id="lblUSD" runat="server" visible="true">$</asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height:15px"></td>
                    </tr>
                </table>
                <table>
                </table>
           <table>
               <tr>
                   <td style="text-align: center">
                       <asp:Label ID="Label2" runat="server" Text="Etsy" Font-Size="8pt" ForeColor="#999999"></asp:Label>                        
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label3" runat="server" Text="Amazon" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label4" runat="server" Text="eBay" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label5" runat="server" Text="hgerman" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
               </tr>
               <tr>
                   <td style="text-align: center">
                        <asp:Label ID="lblEtsyTotalOrder" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#0099CC"></asp:Label> $ 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblAmazonTotalOrder" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#0099CC"></asp:Label> $ 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lbleBayTotalOrder" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#0099CC"></asp:Label> $ 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblhgermanTotalOrder" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#0099CC"></asp:Label> $ 
                   </td>
               </tr>
           </table>    
            </center>
        </div>
            </td>
            <td style="width: 10px;"></td>
            <td>
                <div class="well" style="font-size: x-small; color: #333333; font-weight: bold; background-color: #FFFFFF; text-align: left; height:200px; width:270px">Profit
            <center>
                <table>
                    <tr>
                        <td style="height:15px"></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle">
                        <asp:Label ID="lblTotalProfit" runat="server" Text='<%# Eval("TotalProfit") %>' Visible="true" Font-Bold="True" Font-Size="25pt" ForeColor="#339966"></asp:Label> 
                        </td>
                                                <td style="width:10px"></td>
                        <td style="font-size: xx-large; color: #666666; font-family: Arial;">
                            <asp:Label id="Label1" runat="server" visible="true">$</asp:Label>
                        </td>
                    </tr>
                                        <tr>
                        <td style="height:15px"></td>
                    </tr>
                </table>
           <table>
               <tr>
                   <td style="text-align: center">
                       <asp:Label ID="Label6" runat="server" Text="Etsy" Font-Size="8pt" ForeColor="#999999"></asp:Label>                        
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label7" runat="server" Text="Amazon" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label8" runat="server" Text="eBay" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label9" runat="server" Text="hgerman" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
               </tr>
               <tr>
                   <td style="text-align: center">
                        <asp:Label ID="lblEtsyTotalProfit" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#00CC66"></asp:Label> $ 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblAmazonTotalProfit" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#00CC66"></asp:Label> $ 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lbleBayTotalProfit" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#00CC66"></asp:Label> $ 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblhgermanTotalProfit" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#00CC66"></asp:Label> $ 
                   </td>
               </tr>
           </table>    
            </center>
        </div>
            </td>
            <td style="width: 10px;"></td>
            <td>
                <div class="well" style="font-size: x-small; color: #333333; font-weight: bold; background-color: #FFFFFF; text-align: left; height:200px; width:270px">Order Quantity
            <center>
                <table>
                    <tr>
                        <td style="height:15px"></td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle">
                        <asp:Label ID="lblTotalQuantity" runat="server" Text='<%# Eval("TotalProfit") %>' Visible="true" Font-Bold="True" Font-Size="25pt" ForeColor="#333333"></asp:Label> 
                        </td>
                    </tr>
                                        <tr>
                        <td style="height:15px"></td>
                    </tr>
                </table>
           <table>
               <tr>
                   <td style="text-align: center">
                       <asp:Label ID="Label12" runat="server" Text="Etsy" Font-Size="8pt" ForeColor="#999999"></asp:Label>                        
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label13" runat="server" Text="Amazon" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label14" runat="server" Text="eBay" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="Label15" runat="server" Text="hgerman" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>
               </tr>
               <tr>
                   <td style="text-align: center">
                        <asp:Label ID="lblEtsyTotalQty" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#666666"></asp:Label>
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblAmazonTotalQty" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#666666"></asp:Label>
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lbleBayTotalQty" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#666666"></asp:Label> 
                   </td>
                   <td style="width: 15px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblhgermanTotalQty" runat="server" Text='<%# Eval("TotalProfit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#666666"></asp:Label> 
                   </td>
               </tr>
           </table>    
            </center>
        </div>
            </td>
            <td style="width: 10px;"></td>
            <td>
                <center>
                <div class="well" style="font-size: x-small; color: #333333; font-weight: bold; background-color: #F9F9F9; text-align: left; height:200px; width:270px">Target
                    <table>
                        <tr>
                            <td>
                                                            <center>
<%--  <asp:Chart ID="Chart1" runat="server" Height="100px" Width="200px"  >
            <Titles>
                <asp:Title ShadowOffset="3" Name="Items" />
            </Titles>
            <Legends>
                <asp:Legend Alignment="Center" Docking="Bottom" IsTextAutoFit="False" Name="Default" 
                    LegendStyle="Row"  Enabled="False" />
            </Legends>
            <Series>
                <asp:Series Name="Data" ChartType="Doughnut" Color="Chocolate" BackGradientStyle="HorizontalCenter" ToolTip="#PERCENT{P} #AXISLABEL"/>
            </Series>
            <ChartAreas >
                <asp:ChartArea Name="ChartArea1"  BorderWidth="0" BackColor="White"   />
            </ChartAreas>
        </asp:Chart> --%>

                                                                <table>
                                                                    <tr>
                                                                        <td style="text-align: center; width: 10px;"></td>
                                                                        <td>
                                <asp:Chart ID="Chart8" runat=server BackColor="Transparent" DataSourceID="SqlDataSource9" OnLoad="Chart8_Load" CustomProperties="DoughnutRadius=30" OnDataBound="ChartExample_DataBound" Width="200px" IsSoftShadows="False" TextAntiAliasingQuality="Normal" Palette="None" PaletteCustomColors="0, 153, 255; Red" Height="100px">
    <Legends>
        <asp:Legend Alignment="Center" Docking="Bottom" LegendStyle="Row" Name="Legend1" TableStyle="Wide" BackColor="Transparent" Enabled="False" ForeColor="Gray" Font="Arial Narrow, 3.75pt" TitleFont="Arial Narrow, 3.75pt" TitleForeColor="DarkGray">
        </asp:Legend>
    </Legends>
    <BorderSkin BackColor="Transparent" BorderColor="Transparent" />
    <Series>
    <asp:Series Name="TargetYear" XValueMember="TargetYear" YValueMembers="5" ChartType="Doughnut" Legend="Legend1" Color="#FF5050" CustomProperties="DoughnutRadius=20" Font="Arial Narrow, 3.75pt" LabelForeColor="Transparent" IsVisibleInLegend="False" LabelBorderDashStyle="NotSet">
        <SmartLabelStyle Enabled="False" />
        </asp:Series>
<%--    <asp:Series Name="3" XValueMember="3" YValueMembers="MonthlyProfit" IsValueShownAsLabel="True" ChartType="Doughnut" Legend="Legend1" Color="#00CC66" Palette="Bright">
        <Points>
            <asp:DataPoint YValues="0" />
        </Points>
        </asp:Series>--%>
    </Series>
            <ChartAreas>
            <asp:ChartArea Name="ChartArea1" BackColor="Transparent" BorderColor="Transparent" Area3DStyle-Rotation="70">
                <AxisY IsLabelAutoFit="False" IsMarginVisible="False" LineColor="Transparent" LineDashStyle="NotSet" LineWidth="0" TitleForeColor="Transparent">
                    <MajorGrid LineColor="Transparent" Enabled="False" />
                    <MajorTickMark Enabled="False" />
                    <MinorTickMark LineColor="Transparent" />
                    <LabelStyle Enabled="False" />
                </AxisY>
                <AxisX Interval=1 >       
                    <MajorGrid LineWidth= 0 Enabled="False" />
                    <MajorGrid LineColor="Transparent" Enabled="False" />
                    <MinorGrid LineColor="Transparent" />
                    <MajorTickMark Enabled="False" />
                </AxisX>
                <AxisX2 LineColor="Transparent" IsMarksNextToAxis="False">
                </AxisX2>
                <AxisY2 LineColor="Transparent">
                </AxisY2>

<Area3DStyle Rotation="70"></Area3DStyle>
            </asp:ChartArea>
        </ChartAreas>
            </asp:Chart>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                        <asp:SqlDataSource ID="SqlDataSource9" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" SelectCommand="SELECT [TargetYear], [5] FROM [V_ActualandRemaining]">
                            </asp:SqlDataSource>
                            </center>
                            </td>
                        </tr>
                    </table>
                               <table>
               <tr>
                   <center>
                   <td style="text-align: center">
                       <asp:Label ID="Label10" runat="server" Text="Actual" Font-Size="8pt" ForeColor="#999999"></asp:Label>                        
                   </td>

                   <td style="width: 130px;"></td>
                   <td style="text-align: center; width: 70px;">
                       <asp:Label ID="Label11" runat="server" Text="Remaining" Font-Size="8pt" ForeColor="#999999"></asp:Label>     
                   </td>

                   </center>
               </tr>
               <tr>
                   <center>
                   <td style="text-align: center; width: 70px;">
                        <asp:Label ID="lblActual" runat="server" Text='<%# Eval("TotalPrMonthlyProfitofit") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#666666"></asp:Label>$
                   </td>
                   <td style="width: 130px;"></td>
                   <td style="text-align: center">
                       <asp:Label ID="lblRemaining" runat="server" Text='<%# Eval("TargetRemaining") %>' Font-Bold="True" Font-Size="8pt" ForeColor="#666666"></asp:Label>$
                   </td>
                   </center>
               </tr>
           </table>    
                </div>
                </center>
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
            </td>
        </tr>
    </table>
        </center>
    <div class="full-width">
  <div class="panel panel-default">
    <div class="panel-body">
            <table id="tblMarketplaceFilter" runat="server" visible ="true">
    <tr>
        <td style="width: 100px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Marketplace</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddMarketplaceFilter" runat="server" class="form-control" OnSelectedIndexChanged="ddMarketplaceFilter_Changed" AutoPostBack="true" width="100px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" ></asp:DropDownList>
            </td>
       <td style="width: 25px;"></td>
                <td style="width: 60px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Status</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStatusFilter" runat="server" class="form-control" OnSelectedIndexChanged="ddStatusFilter_Changed" AutoPostBack="true" width="100px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" ></asp:DropDownList>
            </td>
    </tr>
    </table>
        </div>
      </div>
        </div>
    <style>
    .full-width {
        width: 100% !important;
        max-width: none !important;
        padding-left: 0;
        padding-right: 0;
    }
</style>
    <div class="full-width">
  <div class="panel panel-default">
    <div class="panel-body">
  <h2>Orders</h2>
        <br />
        <div style="text-align: right">
                                        <span class="badge" style="background-color: #3366CC; font-size: large; color: #FFFFFF; font-family: Arial; font-weight: bold">Open: <asp:Label ID="lblOpenQty" runat="server" ForeColor="White" Font-Size="Large" Font-Bold="True"></asp:Label></span>
        </div>
        <br />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" OnTick="RefreshGridView" Interval="999999999"/>
    <asp:GridView ID="gvOrder" runat="server" class="table table-bordered table-condensed table-responsive table-hover" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Order is not available." OnPageIndexChanging="gvOrder_PageIndexChanging" OnRowDataBound="gvOrder_RowDataBound" onRowCommand="gvOrder_RowCommand" Font-Size="12px" AllowPaging="True" PageSize="20" BorderStyle="None">
        <Columns>
            <asp:TemplateField HeaderText="" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center"> 
            <ItemTemplate>
            <asp:Label ID="lblMarketplace" runat="server" Text='<%# Eval("Marketplace") %>' Visible="false" ForeColor="#336699" ></asp:Label>
            <asp:Image ID="AmazonIcon" ImageUrl="~/Administrator/Images/Amazon_icon.png" runat="server" Height="20px" ToolTip="Amazon" Visible="false" /> 
            <asp:Image ID="EtsyIcon" ImageUrl="~/Administrator/Images/Etsy_icon.png" runat="server" Height="20px" ToolTip="Amazon" Visible="false" />
            <asp:Image ID="EbayIcon" ImageUrl="~/Administrator/Images/Ebay_icon.png" runat="server" Height="25px" ToolTip="Amazon" Visible="false" /> 
            <asp:Image ID="HgermanIcon" ImageUrl="~/Administrator/Images/Hgerman_icon.png" runat="server" Height="25px" ToolTip="Amazon" Visible="false" /> 
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="Marketplace" Visible="false" >
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:BoundField DataField="OrderDate" HeaderText="Order Date" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="OrderDate" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Order Number"> 
                <ItemTemplate>
                   <%--<asp:LinkButton ID="btnOrderDetails" runat="server" CommandName ="btnOrderDetails" CommandArgument='<%# Eval("OrderID") %>'><asp:Label ID="lblOrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' Visible="true" ForeColor="#0066CC" Font-Size="Small"></asp:Label></asp:LinkButton>--%>
<%--<a href="javascript:void(0);" class="order-number-link"
   data-ordernumber='<%# Eval("OrderNumber") %>'
   data-market='<%# Eval("Marketplace") %>'
   data-buyer='<%# Eval("BuyerFullName") %>'
   data-country='<%# Eval("Country") %>'
   data-ship='<%# Eval("ShippingType") %>'
   data-total='<%# Eval("TotalPrice") %>'
   data-profit='<%# Eval("Profit") %>'
   data-currency='<%# Eval("Currency") %>'
   data-status='<%# Eval("ShippingStatus") %>'
   data-shippingcompanyid='<%# Eval("ShippingCompanyID") %>'
   data-kkid='<%# Eval("KKID") %>'
   data-trackingnumber='<%# Eval("TrackingNumber") %>'
   data-shipdate='<%# Eval("ShipDate", "{0:yyyy-MM-dd}") %>'
   data-shippingprice='<%# Eval("ShippingPrice") %>'>
   <%# Eval("OrderNumber") %>
</a>--%>
<a href='OrderDetailsUpdate.aspx?OrderNumber=<%# Eval("OrderNumber") %>' target="_blank" style="color:#0066CC;cursor:pointer;">
   <%# Eval("OrderNumber") %>
</a>


                </ItemTemplate>
                            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:TemplateField>
<%--            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="OrderNumber" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>--%>
            <asp:BoundField DataField="BuyerFullName" HeaderText="Buyer" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="BuyerFullName" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Country" HeaderText="Country" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Country" >       
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ship Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="75px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
            <asp:Label ID="lblShippingType" runat="server" Text='<%# Eval("ShippingType") %>' Visible="false" ForeColor="#0066CC" Font-Size="Smaller"></asp:Label>
            <asp:Label id="lblStandard" runat="server" class="label label-info" visible="false">Standard</asp:Label>
            <asp:Label id="lblExpedite" runat="server" class="label label-danger" visible="false">Expedited</asp:Label>
<%--            <asp:Button ID="btnShippingStatus1" runat="server" class="btn btn-default btn-xs" Text="Preparing" Visible="False" Width="140" ToolTip="Preparing" CommandName ="btnShippingStatus1" CommandArgument='<%# Eval("ShippingStatus") %>' />
            <asp:Button ID="btnShippingStatus2" runat="server" class="btn btn-warning btn-xs" Text="Packaged" Visible="False" Width="140" ToolTip="Packaged" CommandName ="btnShippingStatus2" CommandArgument='<%# Eval("ShippingStatus") %>' />
            <asp:Button ID="btnShippingStatus3" runat="server" class="btn btn-success btn-xs" Text="Dispatch" Visible="False" Width="140" ToolTip="Dispatch" CommandName ="btnShippingStatus3" CommandArgument='<%# Eval("ShippingStatus") %>' />   --%>         
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="75px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="20px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:20px">
                            <asp:Label ID="lblMultipleItemCheck" runat="server" Text='<%# Eval("MultipleItemCheck") %>' Visible="false" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label> 
                            <asp:Label id="lblSingleItem" runat="server" class="label label-info" visible="false">S</asp:Label>
                            <asp:Label id="lblMultipleItem" runat="server" class="label label-primary" visible="false">M</asp:Label>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="20px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Order Total" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="75px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:70px">
                            <asp:Label ID="lblTotalPrice" runat="server" Text='<%# Eval("TotalPrice") %>' Visible="true" ForeColor="#333333" Font-Size="Small" Font-Bold="True"></asp:Label> 
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="75px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Profit" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="60px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:60px">
                            <asp:Label ID="lblProfit" runat="server" Text='<%# Eval("Profit") %>' Visible="true" ForeColor="#009900" Font-Size="Small" Font-Bold="True"></asp:Label> 
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="60px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="60px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:60px">
                            <asp:Label ID="Label16" runat="server" ForeColor="#009900" Text="%"></asp:Label>
                            <asp:Label ID="lblProfitPercentage" runat="server" Text='<%# Eval("ProfitPercentage") %>' Visible="true" ForeColor="#009900" Font-Size="Small" Font-Bold="False"></asp:Label> 
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="60px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Currency" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="50px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:20px">
                            <span class="badge"><asp:Label ID="lblCurrency" runat="server" Text='<%# Eval("Currency") %>' Visible="true" ForeColor="White" Font-Size="XX-Small" Font-Bold="True"></asp:Label></span>
                             
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="50px" />
            </asp:TemplateField>   
            <asp:TemplateField HeaderText="" ItemStyle-HorizontalAlign="Left" HeaderStyle-BorderColor="White" ItemStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ShowHeader="False"> 
            <ItemTemplate>
            <asp:Label ID="lblShippingStatusID" runat="server" Text='<%# Eval("ShippingStatusID") %>' Visible="false" ForeColor="#336699"></asp:Label>
                <asp:Label id="Status1" runat="server" class="label label-warning" visible="false">Preparing</asp:Label>
                <asp:Label id="Status2" runat="server" class="label label-default" visible="false">Waiting for Decision</asp:Label>
                <asp:Label id="Status3" runat="server" class="label label-default" visible="false">Size Revision</asp:Label>
                <asp:Label id="Status4" runat="server" class="label label-default" visible="false">Awaiting from Supplier</asp:Label>
                <asp:Label id="Status5" runat="server" class="label label-info" visible="false">Packaged</asp:Label>
                <asp:Label id="Status6" runat="server" class="label label-info" visible="false">Ready</asp:Label>
                <asp:Label id="Status7" runat="server" class="label label-primary" visible="false">Pre-Shipping</asp:Label>
                <asp:Label id="Status8" runat="server" class="label label-success" visible="false">Final Shipping</asp:Label>
                <asp:Label id="Status9" runat="server" class="label label-danger" visible="false">Cancel</asp:Label>
<%--            <asp:Button ID="btnShippingStatus1" runat="server" class="btn btn-default btn-xs" Text="Preparing" Visible="False" Width="140" ToolTip="Preparing" CommandName ="btnShippingStatus1" CommandArgument='<%# Eval("ShippingStatus") %>' />
            <asp:Button ID="btnShippingStatus2" runat="server" class="btn btn-warning btn-xs" Text="Packaged" Visible="False" Width="140" ToolTip="Packaged" CommandName ="btnShippingStatus2" CommandArgument='<%# Eval("ShippingStatus") %>' />
            <asp:Button ID="btnShippingStatus3" runat="server" class="btn btn-success btn-xs" Text="Dispatch" Visible="False" Width="140" ToolTip="Dispatch" CommandName ="btnShippingStatus3" CommandArgument='<%# Eval("ShippingStatus") %>' />   --%>         
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
    </asp:GridView>
                                        <!-- Order Details Modal -->
<div id="orderDetailsModal" class="modal fade" tabindex="-1">
  <div class="modal-dialog modal-lg modal-dialog-scrollable">
    <div class="modal-content">

      <div class="modal-header">
        <h5 class="modal-title">Order Details</h5>
        <button type="button" class="close" data-dismiss="modal">&times;</button>
      </div>

      <div class="modal-body">
        <table class="table table-bordered">
          <tr><th>Order Number</th><td id="modalOrderNumber"></td></tr>
          <tr><th>Marketplace</th><td id="modalMarketplace"></td></tr>
          <tr><th>Buyer</th><td id="modalBuyer"></td></tr>
          <tr><th>Country</th><td id="modalCountry"></td></tr>
          <tr><th>Shipping Type</th><td id="modalShip"></td></tr>
          <tr><th>Total Price</th><td id="modalTotal"></td></tr>
          <tr><th>Profit</th><td id="modalProfit"></td></tr>
          <tr><th>Currency</th><td id="modalCurrency"></td></tr>
          <tr><th>Status</th><td id="modalStatus"></td></tr>
        </table>

                      <!-- Shipping Info Section -->
<hr />
<h5>Shipping Information</h5>


  <div class="form-group">
    <label class="col-sm-3 control-label">Shipping Company ID</label>
    <div class="col-sm-9">
      <input type="text" id="txtShippingCompanyID" class="form-control" placeholder="Shipping Company ID" />
    </div>
  </div>

  <div class="form-group">
    <label class="col-sm-3 control-label">Pre Shipment ID</label>
    <div class="col-sm-9">
      <input type="text" id="txtKKID" class="form-control" placeholder="Pre Shipment ID (KKID)" />
    </div>
  </div>

  <div class="form-group">
    <label class="col-sm-3 control-label">Tracking Number</label>
    <div class="col-sm-9">
      <input type="text" id="txtTrackingNumber" class="form-control" placeholder="Tracking Number" />
    </div>
  </div>

  <div class="form-group">
    <label class="col-sm-3 control-label">Ship Date</label>
    <div class="col-sm-9">
      <input type="date" id="txtShipDate" class="form-control" />
    </div>
  </div>

  <div class="form-group">
    <label class="col-sm-3 control-label">Shipping Price</label>
    <div class="col-sm-9">
      <input type="text" id="txtShippingPrice" class="form-control" placeholder="Shipping Price" />
    </div>
  </div>



      </div>
</div>
        <asp:HiddenField ID="hfOrderNumberModal" runat="server" />

                      <div class="modal-footer">
        <asp:HiddenField ID="hfOrderToDelete" runat="server" />

        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
          <button type="button" class="btn btn-success" onclick="saveShipping()">Save Shipping</button>
<asp:Button ID="btnDeleteFromModal" runat="server"
                    CssClass="btn btn-danger" Text="Delete Order"
                    OnClick="btnDeleteFromModal_Click" />
      </div>
    </div>
  </div>
</div>
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvOrder" EventName="PageIndexChanging" />
                <asp:AsyncPostBackTrigger ControlID="btnDeleteFromModal" EventName="Click" />
<asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />

            </Triggers>
            </asp:UpdatePanel>

    </div>
  </div>
</div>
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderNumber">
    <Columns>
        <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:yyyy-MM-dd}" />

        <asp:TemplateField HeaderText="Order Number">
            <ItemTemplate>
                <a href="javascript:void(0);" class="open-order-modal"
                   data-OrderNumber='<%# Eval("OrderNumber") %>'
                   data-market='<%# Eval("Marketplace") %>'
                   data-buyer='<%# Eval("BuyerFullName") %>'
                   data-country='<%# Eval("Country") %>'
                   data-ship='<%# Eval("ShippingType") %>'
                   data-total='<%# Eval("TotalPrice") %>'
                   data-profit='<%# Eval("Profit") %>'
                   data-currency='<%# Eval("Currency") %>'
                   data-status='<%# Eval("ShippingStatus") %>'>
                    <%# Eval("OrderNumber") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField DataField="BuyerFullName" HeaderText="Buyer" />
        <asp:BoundField DataField="Country" HeaderText="Country" />
        <asp:BoundField DataField="ShippingType" HeaderText="Ship Type" />
        <asp:BoundField DataField="TotalPrice" HeaderText="Order Total" DataFormatString="{0:C}" />
        <asp:BoundField DataField="Profit" HeaderText="Profit" DataFormatString="{0:C}" />

    </Columns>
</asp:GridView>


    <%--Visible Area--%>
    <asp:TextBox ID="lblLastOrderID" runat="server" Visible="False" BackColor="#009933"></asp:TextBox>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="false"></asp:Label>
    <asp:Label ID="lblOrderNumberforOrderDetails" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblActualMonth" runat="server" Text="Label" Visible="False"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
