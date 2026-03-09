<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PhotoAddforProduct.aspx.cs" Inherits="Feniks.Administrator.PhotoAddforProduct" %>
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
    <div class="container" id="container4" runat="server" visible="true">
        <div class="panel panel-default">
             <div class="panel-body">
                 <table>
                                  <tr>
                                                                                        <td style="width: 100px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">SKU</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtSKU" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="false" MaxLength="50" ></asp:TextBox>
            </td>
                                  </tr>
                         <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Photo Upload</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:FileUpload ID="FileUploadImage" runat="server" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"/>
            </td>
    </tr>
                 </table>
             </div>
        </div>
    </div>
    <div class="container" id="container5" runat="server" visible="true">
          <div class="panel panel-default">
              <div class="panel-body">
        <table id="tblSaveButton" runat="server" visible="true">
        <tr>
            <td style="width: 200px; height:22px; color:#003366; font-family:Arial, Helvetica, sans-serif; font-weight: bold;" ></td>
            <td  style="width:1000px"></td>
            <td>
                <asp:Button ID="NewPhotoSave" runat="server" class="btn btn-success" style="width:100px" Text="Save" OnClick="NewPhotoSave_Click" />
            </td>
            <td style="width:30px"></td>
            <td>
                <asp:Button ID="btnNewPhotoSaveCancel" runat="server" class="btn btn-danger" style="width:100px" Text="Cancel" OnClick="btnNewPhotoSaveCancel_Click" />
            </td>
        </tr>
    </table>
                  </div>
              </div>
          </div>
</asp:Content>
