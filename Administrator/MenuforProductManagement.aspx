<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MenuforProductManagement.aspx.cs" Inherits="Feniks.Administrator.MenuforProductManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
<script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
<link href="//netdna.bootstrapcdn.com/bootstrap/3.0.0/css/bootstrap-glyphicons.css" rel="stylesheet">
<br />
<div class="container">
  <div class="panel panel-default">
    <div class="panel-body">
    <table>
        <tr>
            <td>
                <asp:Panel ID="Panel1" runat="server" DefaultButton="btnBack">
                    <asp:Button ID="btnBack" runat="server" class="btn btn-default btn-md" OnClick="btnBack_Click" Text="Main Menu" Visible="true" Width="150px" />
                </asp:Panel>
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
                <button id="btntoNewProduct" type="button" href="#" runat="server" onserverclick="toNewProduct_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label4" runat="server" Text="Add a New Product" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label6" runat="server" Text="you can add the product you purchased " Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label7" runat="server" Text="for the first time" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <%--<asp:Label ID="Label8" runat="server" Text="add a photo" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>--%>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #33CC33; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
            <td style="width:20px"></td>
            <td>
                <button id="btnProducts" type="button" href="#" runat="server" onserverclick="toProducts_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label1" runat="server" Text="List of Products" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label2" runat="server" Text="summary information" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label3" runat="server" Text="edit features" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #33CC33; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
            <td style="width:20px"></td>
            <td>
                <button id="btnAddPhotos" type="button" href="#" runat="server" onserverclick="toAddPhotos_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label8" runat="server" Text="Add Photos to Products" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label9" runat="server" Text="you can add photos to the products" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #33CC33; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
            <td style="width:20px"></td>
            <td>
                <button id="btnListing" type="button" href="#" runat="server" onserverclick="toListing_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label5" runat="server" Text="Listings" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label10" runat="server" Text="you can see all the details of the listings" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label22" runat="server" Text="edit listings" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #33CC33; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
        </tr>
        <tr>
            <td style="height:20px"></td>
        </tr>
        <tr>
            <td>
                <button id="btnKeywords" type="button" href="#" runat="server" onserverclick="toKeywords_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label11" runat="server" Text="Keywords" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label12" runat="server" Text="you can add and manage new keywords" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
    
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <%--<asp:Label ID="Label8" runat="server" Text="add a photo" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>--%>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #33CC33; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
            <td style="width:20px"></td>
            <td>
                <button id="btnKeyWordsAssign" type="button" href="#" runat="server" onserverclick="toKeyWordsAssign_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label14" runat="server" Text="Assign Keywords to Products" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label15" runat="server" Text="you can assign keywords to products" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">

                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #33CC33; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
            <td style="width:20px"></td>
            <td>
                <button id="btnListingDescription" type="button" href="#" runat="server" onserverclick="toListingDescription_click" class="btn btn-default" style="margin: 0px; padding: -100px 100px 150px -150px; width: 250px; height: 250px; text-align: left; vertical-align: top;">
                    <table>
                        <tr>
                            <td>
                                <i class="glyphicon glyphicon-th-list" style="color: #3399FF; font-size: 30px;"></i>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:30px"></td>
                        </tr>
                        <tr>
                            <td style="height:20px">
                                <asp:Label ID="Label17" runat="server" Text="Description of Listing" Font-Bold="True" ForeColor="#333333" Font-Size="15px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label18" runat="server" Text="used for descriptions that are usually " Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <asp:Label ID="Label13" runat="server" Text="common in listings" Font-Bold="False" ForeColor="#333333" Font-Size="10px"></asp:Label>                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="height:70px"></td>
                        </tr>
                        <tr>
                            <td style="height:10px">
                                <i class="glyphicon glyphicon-record" style="color: #FF5050; font-size: 5px;"></i>
                            </td>
                        </tr>
                    </table>
                </button>
            </td>
        </tr>
    </table>
</center>
    </div>
  </div>
</div>
</asp:Content>
