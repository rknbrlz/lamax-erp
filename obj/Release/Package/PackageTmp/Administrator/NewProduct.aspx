<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewProduct.aspx.cs" Inherits="Feniks.Administrator.NewProduct" %>
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
<hr />
<p style="width:70%;text-align:right;margin-left:0; font-size: x-small; color: #999999;">Product Details</p>
<table id="tblProduct" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Product Type</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddProductType" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" ></asp:DropDownList>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">SKU</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtSKU" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="13" placeholder=" --- please enter ---"></asp:TextBox>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Personalized</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddPersonalized" runat="server" DataSourceID="forYesNo1" DataValueField="YesNo" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White" AppendDataBoundItems="true"><asp:ListItem Text="--- please choose ---" Value="%"/></asp:DropDownList>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Material</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddMaterial" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Band Type</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddBandType" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Color</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddColor" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Stone Status</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStoneStatus" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
    </table>
    <table id="tblStone" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Stone</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStone1" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
<%--    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Stone #2</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStone2" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>--%>
<%--    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Stone #3</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStone3" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>--%>
    <tr>
        <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Weight</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">

            <table>
                <tr>
                    <td>
                <asp:TextBox ID="txtWeight" runat="server" class="form-control" width="70px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="10" OnTextChanged="txtWeight_TextChanged"></asp:TextBox>
                    </td>
                    <td style="width: 30px; height: 22px; color: #808080; font-family: Arial, Helvetica, sans-serif; font-weight: normal; font-size: small;">
                        <p style="text-align: right">gr</p>
                    </td>
                </tr>
            </table>
            </td>
    </tr>
    <tr>
        <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Diameter</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">

            <table>
                <tr>
                    <td>
                <asp:TextBox ID="txtDiameter" runat="server" class="form-control" width="70px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="10" OnTextChanged="txtWeight_TextChanged"></asp:TextBox>
                    </td>
                    <td style="width: 30px; height: 22px; color: #808080; font-family: Arial, Helvetica, sans-serif; font-weight: normal; font-size: small;">
                        <p style="text-align: right">mm</p>
                    </td>
                </tr>
            </table>
            </td>
    </tr>
    <tr>
        <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Lenght</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">

            <table>
                <tr>
                    <td>
                <asp:TextBox ID="txtLenght" runat="server" class="form-control" width="70px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="10" OnTextChanged="txtWeight_TextChanged"></asp:TextBox>
                    </td>
                    <td style="width: 30px; height: 22px; color: #808080; font-family: Arial, Helvetica, sans-serif; font-weight: normal; font-size: small;">
                        <p style="text-align: right">mm</p>
                    </td>
                </tr>
            </table>
            </td>
    </tr>
    <tr>
        <td style="width: 50px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Width</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">

            <table>
                <tr>
                    <td>
                <asp:TextBox ID="txtWidth" runat="server" class="form-control" width="70px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="104" OnTextChanged="txtWeight_TextChanged"></asp:TextBox>
                    </td>
                    <td style="width: 30px; height: 22px; color: #808080; font-family: Arial, Helvetica, sans-serif; font-weight: normal; font-size: small;">
                        <p style="text-align: right">mm</p>
                    </td>
                </tr>
            </table>
            </td>
    </tr>
    </table>
<hr style="width:70%;text-align:left;margin-left:0" />
<p style="width:70%;text-align:right;margin-left:0; font-size: x-small; color: #999999;">Stock Details</p>
<table id="tblStockAddress" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Stock Address</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddStockAddress" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
    </table>
<hr style="width:70%;text-align:left;margin-left:0" />
<p style="width:70%;text-align:right;margin-left:0; font-size: x-small; color: #999999;">Photo Details</p>
<table id="tblPhotoDetails" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 5px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Watermark</td>
            <td style="padding-top: 1px; padding-bottom: 1px; height: 15px">
                <table>
                    <tr>
                        <td style="width: 20px; height: 1px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">
                        </td>
                        <td>
                <asp:RadioButtonList ID="rdbtnWatermark" runat="server" CssClass="radio" Font-Bold="False" Font-Size="Small" ForeColor="#333333" width="0" CellPadding="-1" CellSpacing="-1" Font-Strikeout="False">
                    <asp:ListItem Text="Ok" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Nok" Value="0"></asp:ListItem>
                </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 5px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Video</td>
            <td style="padding-top: -1px; padding-bottom: -1px; height: 1px">
                <table>
                    <tr>
                        <td style="width: 20px; height: 1px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">
                        </td>
                        <td>
                <asp:RadioButtonList ID="rdbtnVideo" runat="server" CssClass="radio" Font-Bold="False" Font-Size="Small" ForeColor="#333333" width="0" CellPadding="-1" CellSpacing="-1" Font-Strikeout="False">
                    <asp:ListItem Text="Ok" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Nok" Value="0"></asp:ListItem>
                </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 5px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Hand Photo</td>
            <td style="padding-top: -1px; padding-bottom: -1px; height: 1px">
                <table>
                    <tr>
                        <td style="width: 20px; height: 1px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">
                        </td>
                        <td>
                <asp:RadioButtonList ID="rdbtnHandPhoto" runat="server" CssClass="radio" Font-Bold="False" Font-Size="Small" ForeColor="#333333" width="0" CellPadding="-1" CellSpacing="-1" Font-Strikeout="False">
                    <asp:ListItem Text="Ok" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Nok" Value="0"></asp:ListItem>
                </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </td>
    </tr>
                </table>
<hr style="width:70%;text-align:left;margin-left:0" />
<p style="width:70%;text-align:right;margin-left:0; font-size: x-small; color: #999999;">Marketplace Details</p>
<table id="tblMarketplaces" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Marketplaces</td>
            <td style="padding-top: -1px; padding-bottom: -1px; height: 22px">
                <table>
                    <tr>
                        <td style="width: 100px;">
                            <asp:CheckBoxList ID="cbxEtsy" runat="server" height="40px" Font-Bold="False" Font-Size="Small" ForeColor="#666666">
                                <asp:ListItem Text="_Etsy" Value="1"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td style="width: 100px;">
                            <asp:CheckBoxList ID="cbxAmazon" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" RepeatDirection="Vertical" RepeatLayout="Table" CellPadding="1" CellSpacing="1">
                                <asp:ListItem Text="_Amazon" Value="1"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td style="width: 100px;">
                            <asp:CheckBoxList ID="cbxEbay" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" RepeatDirection="Vertical" RepeatLayout="Table" CellPadding="1" CellSpacing="1">
                                <asp:ListItem Text="_Ebay" Value="1"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td style="width: 100px;">
                            <asp:CheckBoxList ID="cbxWix" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" RepeatDirection="Vertical" RepeatLayout="Table" CellPadding="1" CellSpacing="1">
                                <asp:ListItem Text="_Wix" Value="1"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td style="width: 100px;">
                            <asp:CheckBoxList ID="cbxCatawiki" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" RepeatDirection="Vertical" RepeatLayout="Table" CellPadding="1" CellSpacing="1">
                                <asp:ListItem Text="_Catawiki" Value="1"></asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </td>
    </tr>
    </table>
<hr style="width:70%;text-align:left;margin-left:0" />
<p style="width:70%;text-align:right;margin-left:0; font-size: x-small; color: #999999;">Purchase Details</p>
    <table id="tblPurchaseDetails" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Purchase Date (first)</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:TextBox ID="txtFirstPurchaseDate" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" MaxLength="15" placeholder=" --- please enter ---" TextMode="Date"></asp:TextBox>
            </td>
    </tr>
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Supplier</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:DropDownList ID="ddSupplier" runat="server" class="form-control" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"></asp:DropDownList>
            </td>
    </tr>
    </table>
<hr style="width:70%;text-align:left;margin-left:0" />
<p style="width:70%;text-align:right;margin-left:0; font-size: x-small; color: #999999;">Photo Upload</p>
    <table id="tblPhotoUpload" runat="server" visible ="true">
    <tr>
        <td style="width: 200px; height: 22px; color: #003366; font-family: Arial, Helvetica, sans-serif; font-weight: bold;">Photo Upload</td>
            <td style="padding-top: 5px; padding-bottom: 5px; height: 22px">
                <asp:FileUpload ID="FileUploadImage" runat="server" width="500px" height="35px" BorderColor="Silver" BorderStyle="Solid" BorderWidth="1px" ReadOnly="False" BackColor="White"/>
            </td>
    </tr>
    </table>
<hr />
    <table id="tblSaveButton" runat="server" visible="true">
        <tr>
            <td style="width: 200px; height:22px; color:#003366; font-family:Arial, Helvetica, sans-serif; font-weight: bold;" ></td>
            <td  style="width:1000px"></td>
            <td>
                <asp:Button ID="btnNewProductSave" runat="server" class="btn btn-success" style="width:100px" Text="Save" OnClick="btnNewProductSave_Click" />
            </td>
            <td style="width:30px"></td>
            <td>
                <asp:Button ID="btnNewProductCancel" runat="server" class="btn btn-danger" style="width:100px" Text="Cancel" OnClick="btnNewProductCancel_Click" />
            </td>
        </tr>
    </table>
<asp:SqlDataSource ID="forYesNo1" runat="server" ConnectionString="<%$ ConnectionStrings:ConStr %>"
    SelectCommand="SELECT * FROM T_forYesNo order by YesNo desc">
    <FilterParameters>        
        <asp:ControlParameter Name="YesNo" ControlID="ddPersonalized" PropertyName="SelectedValue"/>
    </FilterParameters>
</asp:SqlDataSource>
</asp:Content>
