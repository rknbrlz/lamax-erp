<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StockEntry.aspx.cs" Inherits="Feniks.Administrator.StockEntry" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />  
    <!-- jQuery library -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Latest compiled JavaScript -->
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <!-- Bootstrap Modal Dialog -->
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
        <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">
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
            <div class="container">
  <h2 style="font-size: medium">Product Search</h2>
  <div class="panel panel-default">
    <div class="panel-heading">Please enter SKU</div>
    <div class="panel-body">
            <table>
        <tr>
            <td>
                <asp:Panel ID="Panel3" runat="server" DefaultButton="ImageButton8">
                <asp:TextBox ID="txtFilterSKU" runat="server" class="form-control" Width="250px" placeholder="SKU" ></asp:TextBox>
                    </asp:Panel>
            </td>
            <td style="width:5px"></td>
            <td>
                <asp:Panel ID="Panel2" runat="server" DefaultButton="ImageButton8">
            <asp:ImageButton ID="ImageButton8" ImageUrl="~/Administrator/Images/search.png" runat="server" Height="15px" style="width: 15px" Visible="True" OnClick="ImageButton8_Click" />
                    </asp:Panel>
            </td>
            <td style="width:5px"></td>
                        <td>
                <asp:Panel ID="Panel4" runat="server" DefaultButton="ImageButton8">
            <asp:ImageButton ID="btnFilterClear" ImageUrl="~/Administrator/Images/352269_all_clear_icon.png" runat="server" OnClick="btnFilterClear_Click" Height="15px" style="width: 15px" Visible="True" />
                    </asp:Panel>
            </td>
                                <td style="width:50px"></td>
            <td style="width:10px"></td>
        </tr>
    </table>
        </div>
  </div>
                </div>
                            <div class="container">
  <div class="panel panel-default">
    <div class="panel-body">
  <h2>Products</h2>
<%--                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>
    <asp:GridView ID="gvProducts" runat="server" class="table table-bordered table-condensed table-responsive table-hover" DataSourceID="SqlDataSource1" OnPageIndexChanging="gvProducts_PageIndexChanging" OnSelectedIndexChanged="gvProducts_SelectedIndexChanged" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Product is not available." RowDataBound="gvProducts_RowDataBound" Font-Size="12px" AllowPaging="true" PageSize="10" BorderStyle="None" HeaderStyle-BorderStyle="None">
        <Columns>
            <asp:ButtonField buttontype="Link" 
                 commandname="Select" 
                 text="Select" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BorderStyle="None" ItemStyle-BorderColor="White" ItemStyle-BorderStyle="None" />
<%--            <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="Marketplace" Visible="true" >
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>--%>
            <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="SKU" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="ProductType" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="StockQty" HeaderText="Stock Quantity" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="StockAddress" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="StockAddress" HeaderText="Stock Address" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="StockQty" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Supplier" HeaderText="Supplier" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Supplier" >       
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
<%-- <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" SelectCommand="SELECT * FROM [V_Product]">  
</asp:SqlDataSource>--%>  
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>"
                SelectCommand="SELECT * FROM V_ProductforStockEntry order by StockQty desc" FilterExpression="SKU Like '%{0}%'">
                <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="SKU" ControlID="txtFilterSKU" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
                </asp:SqlDataSource>
              <div class="container" id="container4" runat="server" visible="false">
          <div class="panel panel-default">
              <div class="panel-body">
                  <table>
                      <tr>
                          <td>
                           <asp:Image ID="ProductPhoto" runat="server" Height="300px" Width="300px" CssClass = "pic" onclick="ShowFull(this)" ondoubleclick="ShowZoomin(this)" style = "cursor:pointer;margin:10px"/>
                          </td>
                          <td style="width: 25px;"></td>
                          <td>
                              <table>
                                  <tr>
                                                                                        <td style="width: 100px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">SKU</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtSKU" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="true" MaxLength="50" ></asp:TextBox>
            </td>
                                  </tr>
            <tr>
                <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Ring Size</td>
                   <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddRingSize" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" Visible="true"></asp:DropDownList>
                   </td>
            </tr>
                                                                    <tr>
                                                                                        <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Stock Entry Date</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtStockEntryDate" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" TextMode="Date"></asp:TextBox>
            </td>
                                  </tr>
            <tr>
                <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Quantity</td>
                   <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                      <asp:TextBox ID="txtQuantity" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
                   </td>
            </tr>
            <tr>
                <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Item Price</td>
                   <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                      <asp:TextBox ID="txtItemPrice" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
                   </td>
            </tr>
            <tr>
                <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Weight Price</td>
                   <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                      <asp:TextBox ID="txtWeightPrice" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
                   </td>
            </tr>
            <tr>
                <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Document No</td>
                   <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                      <asp:TextBox ID="txtDocumentNumber" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="50" ></asp:TextBox>
                   </td>
            </tr>
            <tr>
                <td style="width: 150px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Supplier</td>
                   <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddSupplier" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" Visible="true"></asp:DropDownList>
                   </td>
            </tr>
                              </table>
                          </td>
                      </tr>
                  </table>
                  </div>
              </div>
                  </div>
                                            <div class="alert alert-danger" id="Alert1" runat="server" visible ="false">
  <strong><i class="glyphicon glyphicon-alert" style="color: #FF3300"></i></strong><asp:Label ID="Label1" runat="server" Text="please fill in the blank fields."></asp:Label>
</div>
    <div class="container" id="container5" runat="server" visible="false">
          <div class="panel panel-default">
              <div class="panel-body">
        <table id="tblSaveButton" runat="server" visible="true">
        <tr>
            <td style="width: 200px; height:22px; color:#003366; font-family:Arial, Helvetica, sans-serif; font-weight: bold;" ></td>
            <td  style="width:1000px"></td>
            <td>
                <asp:Button ID="btnStockEntrySave" runat="server" class="btn btn-success" style="width:100px" Text="Save" OnClick="btnStockEntrySave_Click" />
            </td>
            <td style="width:30px"></td>
            <td>
                <asp:Button ID="btnStockEntrySaveCancel" runat="server" class="btn btn-danger" style="width:100px" Text="Cancel" OnClick="btnStockEntrySaveCancel_Click" />
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
