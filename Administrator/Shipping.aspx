<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="Shipping.aspx.cs" Inherits="Feniks.Administrator.Shipping" %>
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
    <style type="text/css">
        .pagination-ys {
    /*display: inline-block;*/
    padding-left: 0;
    margin: 20px 0;
    border-radius: 4px;
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
        .Textbox
{
min-width:100%;
max-width:100%;
}
            </style>
        <script>
            function ShowFull(ctrl) {
                ctrl.style.height = '200px';
                ctrl.style.width = '200px';
            }
        </script>
    <script>
        function ShowZoomin(ctrl) {
            ctrl.style.height = '50px';
            ctrl.style.width = '50px';
        }
    </script>
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
    <br />
        <div class="container">
  <div class="panel panel-default">
    <div class="panel-body">
  <h2>Open Orders</h2>
<%--                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
    <asp:GridView ID="gvOpenOrder" runat="server" class="table table-bordered table-condensed table-responsive table-hover" DataSourceID="SqlDataSource1" OnPageIndexChanging="gvOpenOrder_PageIndexChanging" OnSelectedIndexChanged="gvOpenOrder_SelectedIndexChanged" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Open order is not available." RowDataBound="gvOpenOrder_RowDataBound" Font-Size="12px" AllowPaging="False" PageSize="20" BorderStyle="None" HeaderStyle-BorderStyle="None">
        <Columns>
            <asp:ButtonField buttontype="Link" 
                 commandname="Select" 
                 text="Select" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BorderStyle="None" ItemStyle-BorderColor="White" ItemStyle-BorderStyle="None" />
<%--            <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="Marketplace" Visible="true" >
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>--%>
            <asp:BoundField DataField="OrderDate" HeaderText="Order Date" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="OrderDate" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="OrderNumber" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="BuyerFullName" HeaderText="Buyer" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="BuyerFullName" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Country" HeaderText="Country" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Country" >       
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="ShippingStatus" HeaderText="Shipping Status" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Country" >       
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="White" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
        <SelectedRowStyle BackColor="White" BorderStyle="None" BorderColor="White" Font-Bold="False" />
    </asp:GridView>
<%--            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvOpenOrder" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>--%>
        </div>
      </div>
            </div>
 <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" SelectCommand="SELECT * FROM [V_ShippingShort]">  
</asp:SqlDataSource>  
            <div class="container" id="container2" runat="server" visible="false">
  <div class="panel panel-default">
    <div class="panel-body">
        <table>
        <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Order Number</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtOrderNumber" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
            <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Order Date</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtOrderDate" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
        </tr>
        <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Buyer</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtBuyerName" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
                        <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Country</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtCountry" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
        </tr>
        <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Phone Number</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtPhone" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
                        <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">E-mail</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtEmail" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
        </tr>
    </table>
        <table>
        <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Ship To</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtShipTo" runat="server" class="form-control" width="348px" height="70px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="500" TextMode="MultiLine"></asp:TextBox>
            </td>
                                    <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Shipping Type</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtShippingType" runat="server" class="form-control" width="350px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="50" ></asp:TextBox>
            </td>
        </tr>
        </table>
        <table>
            <tr>
                                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Seller Note</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px; text-align: left;">
                <asp:TextBox ID="txtSellerNote" runat="server" class="form-control" width="928px" height="70px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="500" TextMode="MultiLine" ForeColor="#CC0000"></asp:TextBox>
            </td>
            </tr>
        </table>
                <table>
        <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Buyer Note</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtBuyerNote" runat="server" class="form-control" width="350px" height="70px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="500" TextMode="MultiLine" ForeColor="#CC0000"></asp:TextBox>
            </td>
                                    <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Gift Message</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtGiftMessage" runat="server" class="form-control" width="350px" height="70px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="True" MaxLength="500" TextMode="MultiLine" ForeColor="#CC0000"></asp:TextBox>
            </td>
        </tr>
        </table>
        </div>
      </div>
                </div>
                <div class="container" id="container1" runat="server" visible="false">
  <div class="panel panel-default">
    <div class="panel-body">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
 <%--               <asp:Timer ID="Timer1" runat="server" OnTick="RefreshGridView" Interval="1200" />--%>
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
            <asp:BoundField DataField="StockQty" HeaderText="Stock" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="StockQty" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="StockAddress" HeaderText="Address" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="StockAddress" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
    </asp:GridView>
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvOrderDetail" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
        </div>
      </div>
                    </div>
      <div class="container" id="container4" runat="server" visible="false">
          <div class="panel panel-default">
              <div class="panel-body">
                  <table>
                              <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Forwarder</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddForwarder" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" Visible="false"></asp:DropDownList>
                                <asp:TextBox ID="txtForwarder" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" Visible="true" ></asp:TextBox>
            </td>
                                                                    <td style="width: 5px;"></td>
                                  <td style="width: 10px;">
                                      <asp:LinkButton runat="server" id="btntxtForwarderEdit" OnClick="btntxtForwarderEdit_Click">Edit</asp:LinkButton>
                                  </td>
                                                                      <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Ship Date</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtShipDate" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" TextMode="Date" ></asp:TextBox>
            </td>
                                  <td style="width: 5px;"></td>
                                  <td style="width: 10px;">
                                      <asp:LinkButton runat="server" id="btntxtShipDateEdit" OnClick="btntxtShipDateEdit_Click">Edit</asp:LinkButton>
                                  </td>
    </tr>
                              <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">KK ID</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtKKid" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
            </td>
                                                                                                      <td style="width: 5px;"></td>
                                  <td style="width: 10px;">
                                      <asp:LinkButton runat="server" id="btntxtKKIDEdit" OnClick="btntxtKKIDEdit_Click">Edit</asp:LinkButton>
                                  </td>
                        <td style="width: 20px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Shipping Price</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtShippingPrice" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
            </td>
                                                                    <td style="width: 5px;"></td>
                                  <td style="width: 10px;">
                                      <asp:LinkButton runat="server" id="btntxtShippingPriceEdit" OnClick="btntxtShippingPriceEdit_Click">Edit</asp:LinkButton>
                                  </td>
        </tr>
                  </table>
                  <table>
                                                                          <tr>
                    <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Tracking Number</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtTrackingNumber" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
            </td>
                                                                                                                                                  <td style="width: 5px;"></td>
                                  <td style="width: 10px;">
                                      <asp:LinkButton runat="server" id="btntxtTrackingNumberEdit" OnClick="btntxtTrackingNumberEdit_Click">Edit</asp:LinkButton>
                                  </td>
                                                                                                                                                    <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;"></td>
                                                                                                  <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Status</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStatus" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" Visible="false"></asp:DropDownList>
                                <asp:TextBox ID="txtStatus" runat="server" class="form-control" width="335px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" Visible="true" ></asp:TextBox>
            </td>
                                                                                                                                                  <td style="width: 5px;"></td>
                                  <td style="width: 10px;">
                                      <asp:LinkButton runat="server" id="btnStatusEdit" OnClick="btnStatusEdit_Click">Edit</asp:LinkButton>
                                  </td>
        </tr>
                  </table>
                          <div class="alert alert-warning" id="AlertStatus" runat="server" visible ="false">
  <strong><i class="glyphicon glyphicon-alert" style="color: #FFCC00"></i></strong><asp:Label ID="lblStatus" runat="server" Text="Label"></asp:Label>
</div>
                                            <div class="alert alert-danger" id="AlertStatusSelect" runat="server" visible ="false">
  <strong><i class="glyphicon glyphicon-alert" style="color: #FF3300"></i></strong><asp:Label ID="Label1" runat="server" Text="Please select the status of the order."></asp:Label>
</div>
              </div>
          </div>
      </div>      
    <div class="container" id="container5" runat="server" visible="false">
          <div class="panel panel-default">
              <div class="panel-body">
        <table id="tblSaveButton" runat="server" visible="true">
        <tr>
            <td style="width: 200px; height:22px; color:#003366; font-family:Arial, Helvetica, sans-serif; font-weight: bold;" ></td>
            <td  style="width:1000px"></td>
            <td>
                <asp:Button ID="NewOrderSave" runat="server" class="btn btn-success" style="width:100px" Text="Save" OnClick="btnNewOrderSave_Click" />
            </td>
            <td style="width:30px"></td>
            <td>
                <asp:Button ID="btnSatCreationCancel" runat="server" class="btn btn-danger" style="width:100px" Text="Cancel" OnClick="btnNewOrderCancel_Click" />
            </td>
        </tr>
    </table>
                  </div>
              </div>
          </div>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="false"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
