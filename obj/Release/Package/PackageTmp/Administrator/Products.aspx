<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="Feniks.Administrator.Products" %>
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
<%--                                                <td style="width:15px"></td>
                                                <td>
                            <asp:Button ID="btnNewProduct" runat="server" class="btn btn-success btn-md" OnClick="btnNewProduct_Click" Text="New Product" Visible="true" Width="150px"/>
                        </td>--%>
                                                                        <td style="width:15px"></td>
            <td>
                    <asp:Button ID="btnPhotoAdd" runat="server" class="btn btn-info btn-md" OnClick="btnPhotoAdd_Click" Text="Photo Add" Visible="true" Width="150px" />
            </td>
                    </tr>
                </table>
    <br />
    <table id="tblProduct" runat="server" visible ="true">
    <tr>
        <td style="width: 100px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Product Type</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddProductTypeFilter" runat="server" class="form-control" OnSelectedIndexChanged="ddProductTypeFilter_Changed" AutoPostBack="true" width="100px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" ></asp:DropDownList>
            </td>
    </tr>
    </table>
    <hr />
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
<%--                <asp:Timer ID="Timer1" runat="server" OnTick="RefreshGridView" Interval="60000"/>--%>
    <asp:GridView ID="gvProduct" runat="server" CssClass="table table-hover table-bordered" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Product is not available." OnPageIndexChanging="gvProduct_PageIndexChanging" OnRowDataBound="gvProduct_RowDataBound" Font-Size="12px" AllowPaging="True" PageSize="5">
        <Columns>
            <asp:TemplateField>
            <ItemTemplate>
            <asp:Image ID="ProductPhoto" runat="server" Height="30px" Width="30px" CssClass = "pic" onclick="ShowFull(this)" ondoubleclick="ShowZoomin(this)" style = "cursor:pointer;margin:10px"/>
            </ItemTemplate>
            <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center" />
            </asp:TemplateField>
            <%--<asp:BoundField DataField="ProductId" HeaderText="Product ID" SortExpression="ProductId" />--%>
            <asp:BoundField DataField="SKU" HeaderText="SKU" SortExpression="SKU" />
            <asp:BoundField DataField="ProductType" HeaderText="Type" SortExpression="ProductType" />
            <%--<asp:BoundField DataField="Personalized" HeaderText="Personalized" SortExpression="Personalized" />--%>
            <asp:BoundField DataField="StockAddress" HeaderText="Stock Address" SortExpression="StockAddress" />
            <asp:BoundField DataField="Material" HeaderText="Material" SortExpression="Material" />
            <asp:BoundField DataField="BandType" HeaderText="Band Type" SortExpression="BandType" />
            <asp:BoundField DataField="Color" HeaderText="Band Color" SortExpression="Color" />
            <%--<asp:BoundField DataField="StoneStatus" HeaderText="Stone" SortExpression="StoneStatus" />--%>
            <asp:BoundField DataField="Weight" HeaderText="Weight" SortExpression="Weight" />
            <%--<asp:BoundField DataField="Diameter" HeaderText="Diameter" SortExpression="Diameter" />
            <asp:BoundField DataField="Length" HeaderText="Length" SortExpression="Length" />
            <asp:BoundField DataField="Width" HeaderText="Width" SortExpression="Width" />--%>
            <asp:BoundField DataField="Supplier" HeaderText="Supplier" SortExpression="Supplier" />
                                <%--<asp:TemplateField HeaderText="?"> 
            <ItemTemplate>
               <asp:Label ID="lblStatusID" runat="server" Text='<%# Eval("StatusID") %>' Visible="false" ForeColor="#336699"></asp:Label>
<asp:Button ID="btnStatus1" runat="server" class="btn btn-default btn-xs" Text="Talep Oluşturuldu" Visible="False" Width="140" ToolTip="Satınalma Talebi Oluşturuldu" CommandName ="btnStatus1" CommandArgument='<%# Eval("Satid") %>' />
<asp:Button ID="btnStatus2" runat="server" class="btn btn-default btn-xs" Text="Onaylandı" Visible="False" Width="140" ToolTip="Satınalma Talebi Onaylandı" CommandName ="btnStatus2" CommandArgument='<%# Eval("Satid") %>' />
<asp:Button ID="btnStatus3" runat="server" class="btn btn-primary btn-xs" Text="Sipariş Verildi" Visible="False" Width="140" ToolTip="Satınalma Siparişi Verildi" CommandName ="btnStatus3" CommandArgument='<%# Eval("Satid") %>' />  
<asp:Button ID="btnStatus6" runat="server" class="btn btn-danger btn-xs" Text="Reddedildi" Visible="False" Width="140" ToolTip="Satınalma Talebi Reddedildi" CommandName ="btnStatus6" CommandArgument='<%# Eval("Satid") %>' />   
<asp:Button ID="btnStatus7" runat="server" class="btn btn-warning btn-xs" Text="Kısmi Sipariş Verildi" Visible="False" Width="140" ToolTip="Malzemelerinizin Bir Kısmı Sipariş Verildi" CommandName ="btnStatus7" CommandArgument='<%# Eval("Satid") %>' />   
<asp:Button ID="btnStatus4" runat="server" class="btn btn-success btn-xs" Text="Stok Girişi Yapıldı" Visible="False" Width="140" ToolTip="Tüm MalzemelerinStok Girişi Yapıldı" CommandName ="btnStatus4" CommandArgument='<%# Eval("Satid") %>' />       
            </ItemTemplate>
        </asp:TemplateField>--%>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                                    <PagerSettings Mode="NumericFirstLast" />
                                        <PagerStyle CssClass="pagination-ys" />
    </asp:GridView>
                </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvProduct" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
<%--                <script type = "text/javascript">
        $(".pic").on("mousemove", function (e) {
            var dv = $("#popup");
            if (dv.length == 0) {
                var src = $(this)[0].src;
                var dv = $("<div />").css({ height: 350, width: 300, position: "absolute" });
                var img = $("<img />").css({ height: 350, width: 300 }).attr("src", src);
                dv.append(img);
                dv.attr("id", "popup");
                $("body").append(dv);
            }
            dv.css({ left: e.pageX, top: e.pageY });
        });
        $(".pic").on("mouseout", function () {
            $("#popup").remove();
        });
                </script> --%>
</asp:Content>
