<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="KeywordsAssigned.aspx.cs" Inherits="Feniks.Administrator.KeywordsAssigned" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
      <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
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
            .auto-style1 {
                width: 123px;
            }
    </style>
    <style type="text/css">
.label_middlex {
  vertical-align:middle;
 }
</style>
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
                    </button></center>
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
        <div class="panel panel-default">
            <div class="panel-body">
                            <table>
        <tr>
            <td>
                <asp:TextBox ID="txtFilterSKU" runat="server" class="form-control" Width="250px" placeholder="SKU" ></asp:TextBox>
            </td>
            <td style="width:5px"></td>
            <td>
            <asp:ImageButton ID="btnFilter" ImageUrl="~/Administrator/Images/search.png" runat="server" Height="15px" style="width: 15px" Visible="True" OnClick="btnFilter_Click" />
            </td>
            <td style="width:5px"></td>
                        <td>
            <asp:ImageButton ID="btnFilterClear" ImageUrl="~/Administrator/Images/352269_all_clear_icon.png" runat="server" OnClick="btnFilterClear_Click" Height="15px" style="width: 15px" Visible="True" />
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
                <table>
                    <tr>
                        <td style="border: 1px solid #CCCCCC">
                                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvProductSelect" runat="server" DataSourceID="gvProductSelectDataSource" HeaderStyle-Font-Size="Small" CssClass="table table-bordered table-condensed table-responsive table-hover" RowStyle-VerticalAlign="Middle" AlternatingRowStyle-VerticalAlign="Middle" EditRowStyle-VerticalAlign="Middle" EmptyDataRowStyle-VerticalAlign="Middle" SelectedRowStyle-VerticalAlign="Middle"  HorizontalAlign="Left" Width="500" GridLines="None" EnableSortingAndPagingCallbacks="false" AutoGenerateColumns="False" EmptyDataText="SKU Not Found!" OnRowDataBound="gvProductSelect_RowDataBound" OnPageIndexChanging ="gvProductSelect_PageIndexChanging" CellPadding="10" Font-Size="12px" AllowPaging="True" PageSize="5">
                <Columns>
        <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25px"> 
            <ItemTemplate>
                <asp:CheckBox ID="chkRow" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
            <%--<asp:BoundField DataField="ProductPhoto" HeaderText="Photo" SortExpression="ProductPhoto" ItemStyle-HorizontalAlign="Left" />--%>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" ItemStyle-Height="20px" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="75px">
            <ItemTemplate>
            <asp:Image ID="ProductPhoto" runat="server" Height="30px" Width="30px" CssClass = "pic" onclick="ShowFull(this)" ondoubleclick="ShowZoomin(this)" style = "cursor:pointer;margin:10px"/>
            </ItemTemplate>
            <ItemStyle VerticalAlign="Middle" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="SKU" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100px"> 
            <ItemTemplate>
            <asp:Label ID="lblSKURow" runat="server" Text='<%# Eval("SKU") %>' Visible="true" ForeColor="Black"></asp:Label>
            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="Product Type" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100px"> 
            <ItemTemplate>
            <asp:Label ID="lblProductType" runat="server" Text='<%# Eval("ProductType") %>' Visible="true" ForeColor="Black"></asp:Label>
            </ItemTemplate>
            <ItemStyle VerticalAlign="Middle" />
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="Title" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px"> 
            <ItemTemplate>
            <asp:Label ID="lblTitle" runat="server" Text='<%# Eval("Title") %>' Visible="true" ForeColor="Black"></asp:Label>
            </ItemTemplate>
            <ItemStyle VerticalAlign="Middle" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" Visible="False"> 
            <ItemTemplate>
                <asp:Label ID="lblSKU" runat="server" Text='<%# Eval("SKU") %>' Visible="false" ForeColor="#336699"></asp:Label>
                <asp:Label ID="lblType" runat="server" Text='<%# Eval("ProductTypeID") %>' Visible="false" ForeColor="#336699"></asp:Label>
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
                    <asp:AsyncPostBackTrigger ControlID="gvProductSelect" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
                                    <asp:SqlDataSource ID="gvProductSelectDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>"
                SelectCommand="SELECT * FROM V_ProductwithListing order by SKU asc" FilterExpression="SKU Like '%{0}%'">
                <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="SKU" ControlID="txtFilterSKU" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
                </asp:SqlDataSource>
                        </td>
                        <td style="width: 10px"></td>
                        <td>
                                            <center>
                    <asp:Button ID="btnAddList1" runat="server" class="btn btn-primary btn-md" OnClick="btnAddList1_Click" Text="Add to List" Visible="true" Width="100px" />
                </center>
                        </td>
                        <td style="width: 10px"></td>
                        <td>
                                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" Visible="true">
            <ContentTemplate>
        <asp:GridView ID="gvProductSelectedList" runat="server" CssClass="table table-hover table-bordered" Width="500" GridLines="None" EmptyDataText="There is no selected product!" OnRowCommand="gvProductSelectedList_RowCommand"
    AutoGenerateColumns="false" Font-Size="12px">
    <Columns>
            <asp:BoundField DataField="SKU" HeaderText="SKU" SortExpression="SKU" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" SortExpression="ProductType" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField DataField="Title" HeaderText="Title" ItemStyle-Width="250" SortExpression="Title" ItemStyle-HorizontalAlign="Left" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkUpdate" Text="Delete" runat="server" OnClientClick="return confirm('Will be deleted. Are you sure?');" CommandName="DeleteRow" CommandArgument='<%# Bind("KeywordsAssignedDynamicID") %>'/>
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
                </div>
            </div>
        </div>
        <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                            <table>
        <tr>
            <td>
                <asp:TextBox ID="txtFilterKeyWord" runat="server" class="form-control" Width="250px" placeholder="Keyword" ></asp:TextBox>
            </td>
            <td style="width:5px"></td>
            <td>
            <asp:ImageButton ID="btnFilterKeyWord" ImageUrl="~/Administrator/Images/search.png" runat="server" Height="15px" style="width: 15px" Visible="True" OnClick="btnFilterKeyWord_Click" />
            </td>
            <td style="width:5px"></td>
                        <td>
            <asp:ImageButton ID="btnFilterKeyWordClear" ImageUrl="~/Administrator/Images/352269_all_clear_icon.png" runat="server" OnClick="btnFilterKeyWordClear_Click" Height="15px" style="width: 15px" Visible="True" />
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
                    <table>
                        <tr>
                            <td style="border: 1px solid #CCCCCC">
                                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvKeywordsSelect" runat="server" DataSourceID="gvKeywordsSelectDataSource" HeaderStyle-Font-Size="Small" CssClass="table table-bordered table-condensed table-responsive table-hover" RowStyle-VerticalAlign="Middle" AlternatingRowStyle-VerticalAlign="Middle" EditRowStyle-VerticalAlign="Middle" EmptyDataRowStyle-VerticalAlign="Middle" SelectedRowStyle-VerticalAlign="Middle"  HorizontalAlign="Left" Width="500" GridLines="None" EnableSortingAndPagingCallbacks="false" AutoGenerateColumns="False" EmptyDataText="SKU Not Found!" OnRowDataBound="gvKeywordsSelect_RowDataBound" OnPageIndexChanging ="gvKeywordsSelect_PageIndexChanging" CellPadding="10" Font-Size="12px" AllowPaging="True" PageSize="15">
                <Columns>
        <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25px"> 
            <ItemTemplate>
                <asp:CheckBox ID="chkRow" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="KeyWord" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="100px"> 
            <ItemTemplate>
            <asp:Label ID="lblKeyWordRow" runat="server" Text='<%# Eval("KeyWord") %>' Visible="true" ForeColor="Black"></asp:Label>
            </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="SuperStar KeyWord" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="150px"> 
            <ItemTemplate>
            <asp:Label ID="lblSuperStarKeyWord" runat="server" Text='<%# Eval("SuperStarKeyWord") %>' Visible="true" ForeColor="Black"></asp:Label>
            </ItemTemplate>
            <ItemStyle VerticalAlign="Middle" />
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle" HeaderText="SpecialDay" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="200px"> 
            <ItemTemplate>
            <asp:Label ID="lblSpecialDay" runat="server" Text='<%# Eval("SpecialDay") %>' Visible="true" ForeColor="Black"></asp:Label>
            </ItemTemplate>
            <ItemStyle VerticalAlign="Middle" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="" Visible="False"> 
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
                    <asp:AsyncPostBackTrigger ControlID="gvKeywordsSelect" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
                                    <asp:SqlDataSource ID="gvKeywordsSelectDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>"
                SelectCommand="SELECT * FROM V_KeyWords order by KeyWord asc" FilterExpression="KeyWord Like '%{0}%'">
                <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="KeyWord" ControlID="txtFilterKeyWord" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
                </asp:SqlDataSource>
                            </td>
                            <td style="width: 10px"></td>
                            <td>
                                            <center>
                    <asp:Button ID="btnAddList2" runat="server" class="btn btn-primary btn-md" OnClick="btnAddList2_Click" Text="Add to List" Visible="true" Width="100px" />
                </center>
                            </td>
                            <td style="width: 10px"></td>
                            <td>
                                                <asp:UpdatePanel ID="UpdatePanel4" runat="server" Visible="true">
            <ContentTemplate>
        <asp:GridView ID="gvKeywordsSelectedList" runat="server" CssClass="table table-hover table-bordered" Width="500" GridLines="None" EmptyDataText="There is no selected keywords!" OnRowCommand="gvKeywordsSelectedList_RowCommand"
    AutoGenerateColumns="false" Font-Size="12px">
    <Columns>
            <asp:BoundField DataField="KeyWord" HeaderText="KeyWord" SortExpression="KeyWord" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Left" />
            <asp:BoundField DataField="SuperStarKeyWord" HeaderText="SuperStar KeyWord" SortExpression="SuperStarKeyWord" ItemStyle-Width="150" ItemStyle-HorizontalAlign="Left"/>
            <asp:BoundField DataField="SpecialDay" HeaderText="Special Day" ItemStyle-Width="250" SortExpression="Title" ItemStyle-HorizontalAlign="Left" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkUpdate" Text="Delete" runat="server" OnClientClick="return confirm('Will be deleted. Are you sure?');" CommandName="DeleteRow" CommandArgument='<%# Bind("KeywordsAssignedDynamicID") %>'/>
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
                </div>
            </div>
        </div>
    <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
    <table>
        <tr>
            <td>
                    <asp:Button ID="btnSave" runat="server" class="btn btn-success btn-md" OnClick="btnSave_Click" Text="Save" Visible="true" Width="150px" />
            </td>
            <td style="width: 10px"></td>
            <td>
                    <asp:Button ID="btnCancel" runat="server" class="btn btn-danger btn-md" OnClick="btnCancel_Click" Text="Cancel" Visible="true" Width="150px" />
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
