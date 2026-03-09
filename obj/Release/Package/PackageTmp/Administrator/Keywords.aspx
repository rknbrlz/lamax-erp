<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Keywords.aspx.cs" Inherits="Feniks.Administrator.Keywords" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
  <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
  <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
  <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">
    <%--Modal Form Area New Keyword--%>
            <div id="ModalNewKeyWord" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        New KeyWord</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="upModal" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="lblModalTitle" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="lblModalBody" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtNewKeyWord" runat="server" visible="true" CssClass="form-control" placeholder="enter Keyword..." Width="570px" Height="35px" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td style="height:15px"></td>
    </tr>
    <tr>
        <td>
        <asp:DropDownList ID="ddSuperStar" runat="server" class="form-control" Width="570px" AutoPostBack="true" onselectedindexchanged="ddSuperStarChange" ForeColor="#999999" ></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td style="height:15px"></td>
    </tr>
    <tr>
        <td>
        <asp:DropDownList ID="ddSpecialDay" runat="server" class="form-control" Width="570px" AutoPostBack="true" onselectedindexchanged="ddSpecialDayChange" ForeColor="#999999" ></asp:DropDownList>
        </td>
    </tr>
</table>
                    <br />
<div class="alert alert-danger" id="AlertKeyword" runat="server" visible ="false">
  <strong></strong> "Please enter Keyword!
</div>
<div class="alert alert-danger" id="AlertSuperstar" runat="server" visible ="false">
  <strong></strong> "Please select superstar!
</div>
<div class="alert alert-danger" id="AlertSpecialDay" runat="server" visible ="false">
  <strong></strong> "Please select superstar!
</div>
<div class="alert alert-danger" id="AlertKeywordCheck" runat="server" visible ="false">
  <strong></strong> "Keyword already in database!
</div>
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="btnCancel" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnNewKeywordSuccessfull" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnNewKeywordSuccessfull_Click" Text="Kaydet" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
<%--Modal Form Area Keyword--%>
    <br />
    <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
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
                <asp:Panel ID="Panel1" runat="server" DefaultButton="btnBack">
                    <asp:Button ID="btnBack" runat="server" class="btn btn-default btn-md" OnClick="btnBack_Click" Text="Main Menu" Visible="true" Width="150px" />
                </asp:Panel>
            </td>
            <td style="width: 10px"></td>
            <td>
                <asp:Panel ID="Panel2" runat="server" DefaultButton="btnBack">
                    <asp:Button ID="btnNewKeyWord" runat="server" class="btn btn-info btn-md" OnClick="btnNewKeyWord_Click" Text="New KeyWord" Visible="true" Width="150px" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
        <br />
        <div style="text-align: right">
                                        <span class="badge" style="background-color: #3366CC; font-size: large; color: #FFFFFF; font-family: Arial; font-weight: bold">KeyWords: <asp:Label ID="lblKeyWordsQty" runat="server" ForeColor="White" Font-Size="Large" Font-Bold="True"></asp:Label></span>
        </div>
        <br />
    <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                    <asp:GridView ID="gvKeywords" runat="server" class="table table-bordered table-condensed table-responsive table-hover" DataSourceID="SqlDataSourceforgvKeywords" OnPageIndexChanging="gvKeywords_PageIndexChanging" OnSelectedIndexChanged="gvKeywords_SelectedIndexChanged" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="There is no keyword." RowDataBound="gvKeywords_RowDataBound" Font-Size="12px" AllowPaging="False" PageSize="20" BorderStyle="None" HeaderStyle-BorderStyle="None">
        <Columns>
            <asp:ButtonField buttontype="Link" 
                 commandname="Select" 
                 text="Select" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BorderStyle="None" ItemStyle-BorderColor="White" ItemStyle-BorderStyle="None" />
<%--            <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" HeaderStyle-BackColor="White" ItemStyle-HorizontalAlign="Left" SortExpression="Marketplace" Visible="true" >
            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
            <ItemStyle BorderColor="White" HorizontalAlign="Left" />
            </asp:BoundField>--%>
            <asp:BoundField DataField="KeyWordID" HeaderText="ID" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="KeyWordID" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="KeyWord" HeaderText="KeyWord" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="KeyWord" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="SuperStarKeyWord" HeaderText="SuperStar" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="SuperStarKeyWord" >
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="SpecialDay" HeaderText="Special Day" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="SpecialDay" >
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
                 <asp:SqlDataSource ID="SqlDataSourceforgvKeywords" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" SelectCommand="SELECT * FROM [V_KeyWords]">  
</asp:SqlDataSource>  
                <br />
                <table id="tblKeyWord" runat="server" visible="False">
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="ID" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                        </td>
                        <td style="width: 10px"></td>
                        <td>
                            <asp:Label ID="lblKeyWordID" runat="server" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="KeyWord" ForeColor="#666666" Font-Size="Small" Font-Bold="True"></asp:Label>
                        </td>
                        <td style="width: 10px"></td>
                        <td>
                            <asp:Label ID="lblKeyWord" runat="server" Text="" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
     </div>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblKeyWordCheck" runat="server" Text="Label" Visible="False"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
