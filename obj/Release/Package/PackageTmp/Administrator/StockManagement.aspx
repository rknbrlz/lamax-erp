<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="StockManagement.aspx.cs" Inherits="Feniks.Administrator.StockManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />  
    <!-- jQuery library -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <!-- Latest compiled JavaScript -->
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <!-- Bootstrap Modal Dialog -->
    <link rel="stylesheet" href="//ajax.googleapis.com/ajax/libs/jqueryui/1.11.2/themes/smoothness/jquery-ui.css" />
    <link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">
    <br />
    <div class="container">
  <div class="panel panel-default">
    <div class="panel-body">
    <table>
        <tr>
            <td>
                    <asp:Button ID="btnMainMenu" runat="server" class="btn btn-default btn-md" OnClick="btnMainMenu_Click" Text="Main Menu" Visible="true" Width="150px" />
            </td>
                        <td style="width:15px"></td>
            <td>
                    <asp:Button ID="btnStockEntry" runat="server" class="btn btn-info btn-md" OnClick="btnStockEntry_Click" Text="Stock Entry" Visible="true" Width="150px" />
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
<div class="container">
  <h2 style="font-size: medium">Product Search for Ring</h2>
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
  <h2>Stocks | Ring</h2>
        <br />
        <div style="text-align: right">
                        <table style="text-align: right;">
                    <tr>
                        <td>
                <span class="badge" style="border: thin solid #CCCCCC; background-color: #FFFFFF; font-size: small; color: #666666; font-family: Arial; font-weight: bold">Ring: <asp:Label ID="lblRingStockQty" runat="server" ForeColor="Silver" Font-Size="Small" Font-Bold="True"></asp:Label></span>
                        </td>
                    </tr>
                </table>
        </div>
        <br />
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
    <asp:GridView ID="gvStock" runat="server" DataSourceID="V_StockRingDataSource" class="table table-bordered table-condensed table-responsive table-hover" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Stock is not available." OnPageIndexChanging="gvStock_PageIndexChanging" OnRowDataBound="gvStock_RowDataBound" Font-Size="12px" AllowPaging="False" PageSize="20" BorderStyle="None">
        <Columns>
            <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="SKU" >
            <HeaderStyle BackColor="#CCDDEE" BorderColor="White" />
            <ItemStyle BorderColor="White" BackColor="#FBFBFB"/>
            </asp:BoundField>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="ProductType" >
            <HeaderStyle BackColor="#CCDDEE" BorderColor="White" />
            <ItemStyle BorderColor="White" BackColor="#FBFBFB"/>
            </asp:BoundField>
            <asp:BoundField DataField="StockAddress" HeaderText="Stock Address" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="OrderNumber" >
            <HeaderStyle BackColor="#CCDDEE" BorderColor="White" />
            <ItemStyle BorderColor="White" BackColor="#FBFBFB"/>
            </asp:BoundField>
            <asp:TemplateField HeaderText="Total Quantity" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="75px"> 
            <ItemTemplate>
            <span class="badge" style="background-color: #FFFFFF; border: thin solid #C0C0C0; color: #666666;"><asp:Label ID="lblTotalStockQty" runat="server" Text='<%# Eval("TotalStockQty") %>' Visible="true" ForeColor="#666666" Font-Size="XX-Small" Font-Bold="True"></asp:Label></span>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="75px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Adj." ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="divadj1" visible="false">
                            <span runat="server" id="spanAdj1" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lblAdjustable1" runat="server" Text='<%# Eval("Adjustable") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="divadj2" visible="false">
                            <span runat="server" id="spanAdj2" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lblAdjustable2" runat="server" Text='<%# Eval("Adjustable") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="divadj3" visible="false">
                            <span runat="server" id="spanAdj3" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lblAdjustable3" runat="server" Text='<%# Eval("Adjustable") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" HorizontalAlign="Center" VerticalAlign="Middle" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB" VerticalAlign="Middle" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="5" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div5USCA1" visible="false">
                            <span runat="server" id="x1" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl5USCA1" runat="server" Text='<%# Eval("5 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div5USCA2" visible="false">
                            <span runat="server" id="x2" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl5USCA2" runat="server" Text='<%# Eval("5 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div5USCA3" visible="false">
                            <span runat="server" id="x3" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl5USCA3" runat="server" Text='<%# Eval("5 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="5 1/2" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div512USCA1" visible="false">
                            <span runat="server" id="x4" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl512USCA1" runat="server" Text='<%# Eval("5 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div512USCA2" visible="false">
                            <span runat="server" id="x5" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl512USCA2" runat="server" Text='<%# Eval("5 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div512USCA3" visible="false">
                            <span runat="server" id="x6" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl512USCA3" runat="server" Text='<%# Eval("5 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="6" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div6USCA1" visible="false">
                            <span runat="server" id="x7" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl6USCA1" runat="server" Text='<%# Eval("6 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div6USCA2" visible="false">
                            <span runat="server" id="x8" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl6USCA2" runat="server" Text='<%# Eval("6 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div6USCA3" visible="false">
                            <span runat="server" id="x9" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl6USCA3" runat="server" Text='<%# Eval("6 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="6 1/2" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div612USCA1" visible="false">
                            <span runat="server" id="x10" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl612USCA1" runat="server" Text='<%# Eval("6 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div612USCA2" visible="false">
                            <span runat="server" id="x11" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl612USCA2" runat="server" Text='<%# Eval("6 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div612USCA3" visible="false">
                            <span runat="server" id="x12" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl612USCA3" runat="server" Text='<%# Eval("6 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="7" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div7USCA1" visible="false">
                            <span runat="server" id="x13" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl7USCA1" runat="server" Text='<%# Eval("7 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div7USCA2" visible="false">
                            <span runat="server" id="x14" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl7USCA2" runat="server" Text='<%# Eval("7 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div7USCA3" visible="false">
                            <span runat="server" id="x15" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl7USCA3" runat="server" Text='<%# Eval("7 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="7 1/2" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div712USCA1" visible="false">
                            <span runat="server" id="x16" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl712USCA1" runat="server" Text='<%# Eval("7 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div712USCA2" visible="false">
                            <span runat="server" id="x17" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl712USCA2" runat="server" Text='<%# Eval("7 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div712USCA3" visible="false">
                            <span runat="server" id="x18" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl712USCA3" runat="server" Text='<%# Eval("7 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="8" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div8USCA1" visible="false">
                            <span runat="server" id="x19" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl8USCA1" runat="server" Text='<%# Eval("8 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div8USCA2" visible="false">
                            <span runat="server" id="x20" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl8USCA2" runat="server" Text='<%# Eval("8 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div8USCA3" visible="false">
                            <span runat="server" id="x21" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl8USCA3" runat="server" Text='<%# Eval("8 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="8 1/2" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div812USCA1" visible="false">
                            <span runat="server" id="x22" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl812USCA1" runat="server" Text='<%# Eval("8 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div812USCA2" visible="false">
                            <span runat="server" id="x23" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl812USCA2" runat="server" Text='<%# Eval("8 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div812USCA3" visible="false">
                            <span runat="server" id="x24" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl812USCA3" runat="server" Text='<%# Eval("8 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="9" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div9USCA1" visible="false">
                            <span runat="server" id="x25" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl9USCA1" runat="server" Text='<%# Eval("9 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div9USCA2" visible="false">
                            <span runat="server" id="x26" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl9USCA2" runat="server" Text='<%# Eval("9 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div9USCA3" visible="false">
                            <span runat="server" id="x27" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl9USCA3" runat="server" Text='<%# Eval("9 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="9 1/2" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div912USCA1" visible="false">
                            <span runat="server" id="x28" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl912USCA1" runat="server" Text='<%# Eval("9 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div912USCA2" visible="false">
                            <span runat="server" id="x29" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl912USCA2" runat="server" Text='<%# Eval("9 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div912USCA3" visible="false">
                            <span runat="server" id="x30" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl912USCA3" runat="server" Text='<%# Eval("9 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="10" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="40px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:40px">
                            <div runat="server" id="div10USCA1" visible="false">
                            <span runat="server" id="x31" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl10USCA1" runat="server" Text='<%# Eval("10 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div10USCA2" visible="false">
                            <span runat="server" id="x32" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl10USCA2" runat="server" Text='<%# Eval("10 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div10USCA3" visible="false">
                            <span runat="server" id="x33" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl10USCA3" runat="server" Text='<%# Eval("10 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="40px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="10 1/2" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="45px" HeaderStyle-BackColor="White"> 
            <ItemTemplate>
                <table>
                    <tr>
                        <td style="width:45px">
                            <div runat="server" id="div1012USCA1" visible="false">
                            <span runat="server" id="x34" class="badge" style="background-color: #DFF4EA; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl1012USCA1" runat="server" Text='<%# Eval("10 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                          
                            </span>
                            </div>
                            <div runat="server" id="div1012USCA2" visible="false">
                            <span runat="server" id="x35" class="badge" style="background-color: #FFFFC1; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl1012USCA2" runat="server" Text='<%# Eval("10 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                            <div runat="server" id="div1012USCA3" visible="false">
                            <span runat="server" id="x36" class="badge" style="background-color: #FFDFD7; border: thin solid #C0C0C0; color: #666666;">
                                <asp:Label ID="lbl1012USCA3" runat="server" Text='<%# Eval("10 1/2 US/CA") %>' Visible="true" ForeColor="#333333" Font-Size="Smaller" Font-Bold="True"></asp:Label>                           
                            </span>
                            </div>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="45px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
    </asp:GridView>
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvStock" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
    </div>
  </div>
</div>
                    <asp:SqlDataSource ID="V_StockRingDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>"
                SelectCommand="SELECT * FROM V_StockRing order by SKU asc" FilterExpression="SKU Like '%{0}%'">
                <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="SKU" ControlID="txtFilterSKU" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
                </asp:SqlDataSource>
<div class="container">
  <h2 style="font-size: medium">Product Search for No Ring</h2>
  <div class="panel panel-default">
    <div class="panel-heading">Please enter SKU</div>
    <div class="panel-body">
            <table>
        <tr>
            <td>
                <asp:Panel ID="Panel5" runat="server" DefaultButton="ImageButton8">
                <asp:TextBox ID="TextBox1" runat="server" class="form-control" Width="250px" placeholder="SKU" ></asp:TextBox>
                    </asp:Panel>
            </td>
            <td style="width:5px"></td>
            <td>
                <asp:Panel ID="Panel6" runat="server" DefaultButton="ImageButton8">
            <asp:ImageButton ID="ImageButton1" ImageUrl="~/Images/search.png" runat="server" Height="15px" style="width: 15px" Visible="True" OnClick="ImageButton1_Click" />
                    </asp:Panel>
            </td>
            <td style="width:5px"></td>
                        <td>
                <asp:Panel ID="Panel7" runat="server" DefaultButton="ImageButton8">
            <asp:ImageButton ID="ImageButton2" ImageUrl="~/Images/352269_all_clear_icon.png" runat="server" OnClick="btnFilter2Clear_Click" Height="15px" style="width: 15px" Visible="True" />
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
  <h2>Stocks | Necklace/Earrings/Bracelet/Pendant/Set</h2>
        <br />
        <div style="text-align: right">
                <table>
                <tr style="text-align: right;">
            <td style="text-align: right;">
                <table style="text-align: right;">
                    <tr>
                        <td>
                <span class="badge" style="border: thin solid #CCCCCC; background-color: #FFFFFF; font-size: small; color: #666666; font-family: Arial; font-weight: bold">Necklace: <asp:Label ID="lblNecklaceQty" runat="server" ForeColor="Silver" Font-Size="Small" Font-Bold="True"></asp:Label></span>
                        </td>
                        <td style="height:20px; width: 10px;"></td>
                                                <td>
                <span class="badge" style="border: thin solid #CCCCCC; background-color: #FFFFFF; font-size: small; color: #666666; font-family: Arial; font-weight: bold">Earrings: <asp:Label ID="lblEarringsceQty" runat="server" ForeColor="Silver" Font-Size="Small" Font-Bold="True"></asp:Label></span>
                        </td>
                        <td style="height:20px; width: 10px;"></td>
                                                <td>
                <span class="badge" style="border: thin solid #CCCCCC; background-color: #FFFFFF; font-size: small; color: #666666; font-family: Arial; font-weight: bold">Bracelet: <asp:Label ID="lblBraceletQty" runat="server" ForeColor="Silver" Font-Size="Small" Font-Bold="True"></asp:Label></span>
                        </td>
                        <td style="height:20px; width: 10px;"></td>
                                                <td>
                <span class="badge" style="border: thin solid #CCCCCC; background-color: #FFFFFF; font-size: small; color: #666666; font-family: Arial; font-weight: bold">Pendant: <asp:Label ID="lblPendantQty" runat="server" ForeColor="Silver" Font-Size="Small" Font-Bold="True"></asp:Label></span>
                        </td>
                                                <td style="height:20px; width: 10px;"></td>
                                                <td>
                <span class="badge" style="border: thin solid #CCCCCC; background-color: #FFFFFF; font-size: small; color: #666666; font-family: Arial; font-weight: bold">Set: <asp:Label ID="lblSetQty" runat="server" ForeColor="Silver" Font-Size="Small" Font-Bold="True"></asp:Label></span>
                        </td>
                    </tr>
                </table>
            </td>             
        </tr>
    </table>
        </div>
        <br />
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
    <asp:GridView ID="gvStockNoRing" runat="server" DataSourceID="V_StockNoRingDataSource" class="table table-bordered table-condensed table-responsive table-hover" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Stock is not available." OnPageIndexChanging="gvStockNoRing_PageIndexChanging" OnRowDataBound="gvStockNoRing_RowDataBound" Font-Size="12px" AllowPaging="False" PageSize="20" BorderStyle="None">
        <Columns>
            <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="SKU" >
            <HeaderStyle BackColor="#CCDDEE" BorderColor="White" />
            <ItemStyle BorderColor="White" BackColor="#FBFBFB"/>
            </asp:BoundField>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="ProductType" >
            <HeaderStyle BackColor="#CCDDEE" BorderColor="White" />
            <ItemStyle BorderColor="White" BackColor="#FBFBFB"/>
            </asp:BoundField>
            <asp:BoundField DataField="StockAddress" HeaderText="Stock Address" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="OrderNumber" >
            <HeaderStyle BackColor="#CCDDEE" BorderColor="White" />
            <ItemStyle BorderColor="White" BackColor="#FBFBFB"/>
            </asp:BoundField>
            <asp:TemplateField HeaderText="Total Quantity" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-Width="75px"> 
            <ItemTemplate>
            <span class="badge" style="background-color: #FFFFFF; border: thin solid #C0C0C0; color: #666666;"><asp:Label ID="lblTotalStockQty" runat="server" Text='<%# Eval("TotalStockQty") %>' Visible="true" ForeColor="#666666" Font-Size="XX-Small" Font-Bold="True"></asp:Label></span>
            </ItemTemplate>
                <HeaderStyle BackColor="#D8E4F1" BorderColor="White" />
                <ItemStyle BorderColor="White" Width="75px" BackColor="#FBFBFB"/>
            </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="#F4F4F4" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
    </asp:GridView>
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvStockNoRing" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
    </div>
  </div>
</div>
                        <asp:SqlDataSource ID="V_StockNoRingDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>"
                SelectCommand="SELECT * FROM V_StockNoRing order by SKU asc" FilterExpression="SKU Like '%{0}%'">
                <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="SKU" ControlID="TextBox1" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
                </asp:SqlDataSource>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="false"></asp:Label>
    <%--Visible Area--%>
</asp:Content>
