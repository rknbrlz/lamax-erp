<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Listing.aspx.cs" Inherits="Feniks.Administrator.Listing" %>
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
    <%--Modal Form Area Title Edit--%>
            <div id="ModalTitleEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Title Edit</h4>
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
                                    <asp:UpdatePanel ID="UpdatePanel8" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtTitle" runat="server" visible="true" CssClass="form-control" Width="570px" Height="150px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
<tr>
    <td>
        <table>
                <tr>
        <td>
        <asp:Label ID="lblTitleCount" runat="server" Text="" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
        </td>
        <td style="width:5px"></td>
        <td>
        <asp:Label ID="Label13" runat="server" Text="Characters" ForeColor="#666666" Font-Size="Small" Font-Bold="False"></asp:Label>
        </td>
    </tr>
        </table>
    </td>
</tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
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
<asp:Button ID="btnTitleSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnTitleSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Title Edit--%>
    <%--Modal Form Area Etsy Description Edit--%>
            <div id="ModalEtsyDescriptionEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Etsy Description Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel9" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label15" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label17" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel10" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtEtsyDescriptionEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="350px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="btnEtsyDescriptionCancel" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnEtsyDescriptionSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnEtsyDescriptionSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Etsy Description Edit--%>
    <%--Modal Form Area Etsy Sale Price Edit--%>
            <div id="ModalEtsySalePriceEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Etsy Sale Price Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel11" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label19" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label20" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel12" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtEtsySalePriceEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button1" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnEtsySalePriceSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnEtsySalePriceSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Etsy Sale Price Edit--%>

    <%--Modal Form Area Amazon Description Edit--%>
            <div id="ModalAmazonDescriptionEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Amazon Description Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel13" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label21" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label22" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel14" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtAmazonDescriptionEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="350px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button2" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnAmazonDescriptionSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnAmazonDescriptionSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Amazon Description Edit--%>
    <%--Modal Form Area Amazon Sale Price Edit--%>
            <div id="ModalAmazonSalePriceEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Amazon Sale Price Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel15" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label23" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label24" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel16" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtAmazonSalePriceEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button4" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnAmazonSalePriceSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnAmazonSalePriceSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Amazon Sale Price Edit--%>
    <%--Modal Form Area Amazon Bullet Point Edit--%>
            <div id="ModalAmazonBulletPointEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Amazon Bullet Points Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel17" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label25" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label26" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel18" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtAmazonBulletPoint1" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtAmazonBulletPoint2" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
        <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtAmazonBulletPoint3" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
        <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtAmazonBulletPoint4" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
        <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtAmazonBulletPoint5" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button5" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnAmazonBulletPointsSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnAmazonBulletPointsSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Amazon Bullet Point Edit--%>
    <%--Modal Form Area Listing Status Edit--%>
            <div id="ModalListingStatusEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Listing Status Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel19" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label27" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label28" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel20" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtEtsyListingEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtAmazonListingEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
        <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtEbayListingEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
        <tr>
        <td style="height:10px"></td>
    </tr>
        <tr>
        <td>
        <asp:TextBox ID="txtWixListingEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button6" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnListingStatusSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnListingStatusSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Listing Status Edit--%>
    <%--Modal Form Area Total Listing Status List--%>
            <div id="ModalTotalListingStatusList" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        Listing Status</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxxxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel21" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label29" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label30" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel22" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
<asp:UpdatePanel ID="UpdatePanel23" runat="server">
            <ContentTemplate>
                    <asp:GridView ID="gvTotalListingStatusList" runat="server" class="table table-bordered table-condensed table-responsive table-hover" DataSourceID="gvTotalListingStatusListSqlDataSource" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" onRowDataBound="gvTotalListingStatusList_RowDataBound" Font-Size="12px" AllowPaging="True" PageSize="10" BorderStyle="None" HeaderStyle-BorderStyle="None">
        <Columns>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="ProductType" ItemStyle-Width="150">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="TotalProduct" HeaderText="Total Product" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="TotalProduct" ItemStyle-Width="200">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="EtsyListingCount" HeaderText="Etsy" ItemStyle-BorderColor="White"  ItemStyle-Font-Size="10px" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="EtsyListingCount" ItemStyle-Width="200">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="AmazonListingCount" HeaderText="Amazon" ItemStyle-BorderColor="White"  ItemStyle-Font-Size="10px" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="AmazonListingCount" ItemStyle-Width="200">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="EbayListingCount" HeaderText="eBay" ItemStyle-BorderColor="White"  ItemStyle-Font-Size="10px" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="EbayListingCount" ItemStyle-Width="200">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="WixListingCount" HeaderText="hgerman.shop" ItemStyle-BorderColor="White"  ItemStyle-Font-Size="10px" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="WixListingCount" ItemStyle-Width="200">
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
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvProduct" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
                 <asp:SqlDataSource ID="gvTotalListingStatusListSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" 
                     SelectCommand="SELECT * FROM [V_TotalListing] order by TotalProduct desc"> 
                                     <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="SKU" ControlID="txtFilterSKU" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
</asp:SqlDataSource>  
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td>
<asp:Button ID="Button7" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Close" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Total Listing Status List--%>
    <%--Modal Form Area Ebay Description Edit--%>
            <div id="ModalEbayDescriptionEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        eBay Description Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxxxxxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel24" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label31" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label32" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel25" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtEbayDescriptionEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="350px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button8" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnEbayDescriptionSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnEbayDescriptionSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Ebay Description Edit--%>
    <%--Modal Form Area Wix Description Edit--%>
            <div id="ModalWixDescriptionEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        hgerman.shop Description Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxxxxxxxxxz" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel26" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label33" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label34" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel27" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtWixDescriptionEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="350px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button9" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnWixDescriptionSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnWixDescriptionSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Wix Description Edit--%>
    <%--Modal Form Area eBay Sale Price Edit--%>
            <div id="ModalEbaySalePriceEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        eBay Sale Price Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxcxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel28" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label35" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label36" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel29" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtEbaySalePriceEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button10" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnEbaySalePriceSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnEbaySalePriceSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Etsy Sale Price Edit--%>
    <%--Modal Form Area Wix Sale Price Edit--%>
            <div id="ModalWixSalePriceEdit" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <!--<button type="button" class="close" data-dismiss="modal">
                        &times;</button>-->
                    <h4 class="modal-title" style="font-weight: bold; color: #C0C0C0; font-family: Arial; font-size: medium;">
                        hgerman.shop Sale Price Edit</h4>
                </div>
                <div class="modal-body">

                    <div class="modal fade" id="myModalxxcxxx" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <asp:UpdatePanel ID="UpdatePanel30" runat="server" ChildrenAsTriggers="false" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"><asp:Label ID="Label37" runat="server" Text=""></asp:Label></h4>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="Label38" runat="server" Text=""></asp:Label>
                    </div>
                    <div class="modal-footer">
                        <%--<button class="btn btn-info" data-dismiss="modal" aria-hidden="true">Close</button>--%>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</div>
                                    <asp:UpdatePanel ID="UpdatePanel31" runat="server">
            <ContentTemplate>
<table>
    <tr>
        <td>
        <asp:TextBox ID="txtWixSalePriceEdit" runat="server" visible="true" CssClass="form-control" Width="570px" Height="50px" TextMode="SingleLine"></asp:TextBox>
        </td>
    </tr>
    </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                    <br />
                </div>
                <div class="modal-footer">
                    <table>
                        <tr>
                            <td style="width:400px"></td>
                            <td>
<asp:Button ID="Button11" style="width: 120px;" runat="server" class="btn btn-default btn-md" Text="Cancel" />
                            </td>
                            <td style="width:20px"></td>
                            <td>
<asp:Button ID="btnWixSalePriceSave" style="width: 120px;" runat="server" class="btn btn-info btn-md" OnClick="btnWixSalePriceSave_Click" Text="Save" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <%--Modal Form Area Etsy Sale Price Edit--%>
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
            </td>
            <td style="width: 10px"></td>
            <td>
                    <asp:Button ID="btnMainMenu" runat="server" class="btn btn-default btn-md" OnClick="btnMainMenu_Click" Text="Main Menu" Visible="true" Width="150px" />
            </td>
            <td style="width: 10px"></td>
            <td>
                <asp:Button ID="btnTotalListingStatus" runat="server" class="btn btn-warning btn-md" OnClick="btnTotalListingStatus_Click" Text="Listing Status" Visible="true" Width="150px" />
            </td>
        </tr>
    </table>
    </div>
  </div>
</div>
        <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
            <ContentTemplate>
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
                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                </div>
            </div>
                    </div>
        <div class="container">
        <div class="panel panel-default">
            <div class="panel-body">
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                    <asp:GridView ID="gvProduct" runat="server" class="table table-bordered table-condensed table-responsive table-hover" DataSourceID="gvProductSqlDataSource" onPageIndexChanging="gvProduct_PageIndexChanging" onSelectedIndexChanged="gvProduct_SelectedIndexChanged" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" RowStyle-BorderColor="White" HeaderStyle-HorizontalAlign="Center" GridLines="None" AutoGenerateColumns="False" EmptyDataText="Producy is not available." onRowDataBound="gvProduct_RowDataBound" Font-Size="12px" AllowPaging="True" PageSize="10" BorderStyle="None" HeaderStyle-BorderStyle="None">
        <Columns>
            <asp:ButtonField buttontype="Link" 
                 commandname="Select" 
                 text="Select" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BorderStyle="None" ItemStyle-BorderColor="White" ItemStyle-BorderStyle="None" ItemStyle-Width="50"/>
            <asp:BoundField DataField="SKU" HeaderText="SKU" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="SKU" ItemStyle-Width="150">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="ProductType" HeaderText="Product Type" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="ProductType" ItemStyle-Width="100">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:BoundField DataField="Title" HeaderText="Title" ItemStyle-BorderColor="White"  ItemStyle-Font-Size="10px" HeaderStyle-BorderColor="White" HeaderStyle-BackColor="White" SortExpression="Title_Etsy" ItemStyle-Width="600">
            <HeaderStyle BackColor="White" BorderColor="White" />
            <ItemStyle BorderColor="White" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Etsy" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center"> 
            <ItemTemplate>
            <asp:Label ID="lblEtsy" runat="server" Text='<%# Eval("Etsy") %>' Visible="false" ForeColor="#336699" ></asp:Label>
             <asp:Label ID="lblEtsyOk" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
             <asp:Label ID="lblEtsyNok" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center"/>
                <ItemStyle BorderColor="White" HorizontalAlign="Center"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Amazon" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center"> 
            <ItemTemplate>
            <asp:Label ID="lblAmazon" runat="server" Text='<%# Eval("Amazon") %>' Visible="false" ForeColor="#336699" ></asp:Label>
             <asp:Label ID="lblAmazonOk" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
             <asp:Label ID="lblAmazonNok" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center"/>
                <ItemStyle BorderColor="White" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Ebay" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center"> 
            <ItemTemplate>
            <asp:Label ID="lblEbay" runat="server" Text='<%# Eval("Ebay") %>' Visible="false" ForeColor="#336699" ></asp:Label>
             <asp:Label ID="lblEbayOk" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
             <asp:Label ID="lblEbayNok" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center"/>
                <ItemStyle BorderColor="White" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Wix" HeaderStyle-BackColor="White" ItemStyle-BorderColor="White" HeaderStyle-BorderColor="White" ItemStyle-HorizontalAlign="Center"> 
            <ItemTemplate>
            <asp:Label ID="lblWix" runat="server" Text='<%# Eval("Wix") %>' Visible="false" ForeColor="#336699" ></asp:Label>
             <asp:Label ID="lblWixOk" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
             <asp:Label ID="lblWixNok" runat="server" Visible="false" ForeColor="#336699" >
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
            </ItemTemplate>
                <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center"/>
                <ItemStyle BorderColor="White" HorizontalAlign="Center" />
            </asp:TemplateField>
        </Columns>
                <EmptyDataRowStyle ForeColor="Red" BorderStyle="None" />
        <HeaderStyle BackColor="White" />
                                                 <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" FirstPageText="First" LastPageText="Last"/>
                                        <PagerStyle CssClass="pagination-ys" BorderStyle="None" BackColor="White" BorderColor="White" />
        <RowStyle BorderColor="White" />
        <SelectedRowStyle BackColor="White" BorderStyle="None" BorderColor="White" Font-Bold="False" />
    </asp:GridView>
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="gvProduct" EventName="PageIndexChanging" />
            </Triggers>
            </asp:UpdatePanel>
                 <asp:SqlDataSource ID="gvProductSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" 
                     SelectCommand="SELECT * FROM [V_Listing] order by Title desc" FilterExpression="SKU Like '%{0}%'"> 
                                     <FilterParameters>        
<%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                <asp:ControlParameter Name="SKU" ControlID="txtFilterSKU" PropertyName="Text"/>
<%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                </FilterParameters>
</asp:SqlDataSource>  
                </div>
            </div>
            </div>
        <div class="container" id="containerSKU">
        <div class="panel panel-default">
            <div class="panel-body">
                                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td>
                             <asp:Label ID="Label1" runat="server" Text="SKU" ForeColor="#666666" Font-Size="Large" Font-Bold="True"></asp:Label>
                        </td>
                        <td style="width: 20px"></td>
                        <td>
                             <asp:Label ID="lblSKU" runat="server" Text="Title" ForeColor="#666666" Font-Size="Large" Font-Bold="False"></asp:Label>
                        </td>
                                    <td style="width: 20px"></td>
                                    <td>
                <center>
                <button id="btnListingStatus" type="button" href="#" runat="server" onserverclick="btnListingStatus_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-ok" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
            </td>
                    </tr>
                </table>
                                                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                </div>
            </div>
                </div>
        <div class="container" id="containerDesc">
        <div class="panel panel-default">
            <div class="panel-body">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                <table>
                    <tr>
                        <td style="width: 100px">
                            <asp:Image ID="ProductPhoto" runat="server" Height="100px" Width="100px" CssClass = "pic" onclick="ShowFull(this)" ondoubleclick="ShowZoomin(this)" style = "cursor:pointer;margin:10px"/>
                            <asp:Image ID="Noimage" runat="server" src="/Administrator/imagesnew/Noimage.png" Height="100px" Width="100px" Visible="false"/>                       
                            </td>
                        <td style="width: 50px"></td>
                        <td style="width: 750px; text-align: left;">                                       
                <asp:Label ID="lblTitle" runat="server" Text="There is no title!" width="750px" height="100px" ForeColor="#666666" Font-Size="25px" Font-Bold="False"></asp:Label>    
                        </td>
                        <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnTitleEdit" type="button" href="#" runat="server" onserverclick="toTitleEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                    </tr>
                </table>
                                        <br />
                                        <table>
                                            <tr>
            <div class="container">
                <caption>
                    <ul class="nav nav-tabs">
                        <li class="active"><a data-toggle="tab" href="#home">Etsy</a></li>
                        <li><a data-toggle="tab" href="#menu1">Amazon</a></li>
                        <li><a data-toggle="tab" href="#menu2">eBay</a></li>
                        <li><a data-toggle="tab" href="#menu3">hgerman.shop</a></li>
                    </ul>
                    <div class="tab-content">
                        <div id="home" class="tab-pane fade in active">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtEtsyDescription" runat="server" BorderColor="Silver" BorderStyle="None" BorderWidth="1px" class="form-control" height="500px" ReadOnly="False" TextMode="MultiLine" width="900px"></asp:TextBox>
                                        <%--                <asp:Label ID="lblEtsyDescription" runat="server" Text="label" width="900px" height="500px" ForeColor="#666666" Font-Size="15px" Font-Bold="False"></asp:Label>    --%></td>
                                                            <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnEtsyDescriptionEdit" type="button" href="#" runat="server" onserverclick="btnEtsyDescriptionEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                </tr>
                                <tr>
                                    <td style="height: 20px"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <hr />
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="#666666" Text="Sale Price"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblSalePriceEtsy" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                                <td style="width: 5px"></td>
                                                <td>
                                                    <asp:Label ID="Label3" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="$"></asp:Label>
                                                </td>
                                                                        <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnEtsySalePriceEdit" type="button" href="#" runat="server" onserverclick="toEtsySalePriceEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                            </tr>
                                        </table>
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                            <ContentTemplate>
                                                <asp:GridView ID="gvEtsyKeyword" runat="server" AllowPaging="False" AutoGenerateColumns="False" BorderStyle="None" class="table table-bordered table-condensed table-responsive table-hover" EmptyDataText="Keyword is not available." Font-Size="12px" GridLines="None" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BorderStyle="None" HeaderStyle-HorizontalAlign="Center" onPageIndexChanging="gvEtsyKeyword_PageIndexChanging" onRowDataBound="gvEtsyKeyword_RowDataBound" PageSize="13" RowStyle-BorderColor="White">
                                                    <Columns>
                                                        <asp:BoundField DataField="Keyword" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="Keyword" ItemStyle-BorderColor="White" ItemStyle-Width="100" SortExpression="Keyword">
                                                        <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                                                        <ItemStyle BorderColor="White" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="SuperStar" ItemStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSuperStarKeyWord" runat="server" ForeColor="#336699" Text='<%# Eval("SuperStarKeyWord") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="lblSuperStarKeyWordOk" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
                                                                <asp:Label ID="lblSuperStarKeyWordNok" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                                                            <ItemStyle BorderColor="White" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="Style Keyword" ItemStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblStyleKeywords" runat="server" ForeColor="#336699" Text='<%# Eval("StyleKeywords") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="lblStyleKeywordsOk" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
                                                                <asp:Label ID="lblStyleKeywordsNok" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                                                            <ItemStyle BorderColor="White" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="SpecialDay" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="SpecialDay" ItemStyle-BorderColor="White" ItemStyle-Width="100" SortExpression="SpecialDay">
                                                        <HeaderStyle BackColor="White" BorderColor="White" />
                                                        <ItemStyle BorderColor="White" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <EmptyDataRowStyle BorderStyle="None" ForeColor="Red" />
                                                    <HeaderStyle BackColor="White" />
                                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" PageButtonCount="10" />
                                                    <PagerStyle BackColor="White" BorderColor="White" BorderStyle="None" CssClass="pagination-ys" />
                                                    <RowStyle BorderColor="White" />
                                                    <SelectedRowStyle BackColor="White" BorderColor="White" BorderStyle="None" Font-Bold="False" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="gvEtsyKeyword" EventName="PageIndexChanging" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                        <asp:SqlDataSource ID="gvEtsyKeywordSqlDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" SelectCommand="SELECT * FROM [V_ListingwithKeywords]">
                                            <FilterParameters>
                                                <%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                                                <asp:ControlParameter ControlID="txtFilterSKU" Name="SKU" PropertyName="Text" />
                                                <%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                                            </FilterParameters>
                                        </asp:SqlDataSource>
                                        <hr />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="menu1" class="tab-pane fade">
                            <table>
                                <tr>
                                    <td><%--           <asp:TextBox ID="txtAmazonDescription" runat="server" class="form-control" width="900px" height="250px" BorderColor="Silver" BorderStyle="None" BorderWidth="1px" ReadOnly="False" TextMode="MultiLine" ></asp:TextBox>--%>
                                        <asp:Label ID="lblAmazonDescription" runat="server" Font-Bold="False" Font-Size="15px" ForeColor="#666666" height="250px" Text="label" width="900px"></asp:Label>
                                    </td>
                                                                                                <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="Button3" type="button" href="#" runat="server" onserverclick="btnAmazonDescriptionEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                </tr>
                                <%--        <tr>
            <td>
                <asp:Literal runat="server" ID="htmlLiteral" />
            </td>
        </tr>--%>
                                <tr>
                                    <td style="height: 20px"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <hr />
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label4" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="#666666" Text="Sale Price"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblSalePriceAmazon" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                                <td style="width: 5px"></td>
                                                <td>
                                                    <asp:Label ID="Label6" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="$"></asp:Label>
                                                </td>
                                                                                                                                                <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnAmazonSalePriceEdit" type="button" href="#" runat="server" onserverclick="btnAmazonSalePriceEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                            </tr>
                                        </table>
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                            <ContentTemplate>
                                                <asp:GridView ID="gvAmazonKeyword" runat="server" AllowPaging="False" AutoGenerateColumns="False" BorderStyle="None" class="table table-bordered table-condensed table-responsive table-hover" EmptyDataText="Keyword is not available." Font-Size="12px" GridLines="None" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderStyle-BorderStyle="None" HeaderStyle-HorizontalAlign="Center" onPageIndexChanging="gvAmazonKeyword_PageIndexChanging" onRowDataBound="gvAmazonKeyword_RowDataBound" PageSize="13" RowStyle-BorderColor="White">
                                                    <Columns>
                                                        <asp:BoundField DataField="Keyword" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="Keyword" ItemStyle-BorderColor="White" ItemStyle-Width="100" SortExpression="Keyword">
                                                        <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                                                        <ItemStyle BorderColor="White" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="SuperStar" ItemStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblSuperStarKeyWord" runat="server" ForeColor="#336699" Text='<%# Eval("SuperStarKeyWord") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="lblSuperStarKeyWordOk" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
                                                                <asp:Label ID="lblSuperStarKeyWordNok" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                                                            <ItemStyle BorderColor="White" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="Style Keyword" ItemStyle-BorderColor="White" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="50">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblStyleKeywords" runat="server" ForeColor="#336699" Text='<%# Eval("StyleKeywords") %>' Visible="false"></asp:Label>
                                                                <asp:Label ID="lblStyleKeywordsOk" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-ok" style="color: #66FF99; font-size: 10px;"></i>
            </asp:Label>
                                                                <asp:Label ID="lblStyleKeywordsNok" runat="server" ForeColor="#336699" Visible="false">
                <i class="glyphicon glyphicon-remove" style="color: #FF0066; font-size: 10px;"></i>
            </asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle BackColor="White" BorderColor="White" HorizontalAlign="Center" />
                                                            <ItemStyle BorderColor="White" HorizontalAlign="Center" />
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="SpecialDay" HeaderStyle-BackColor="White" HeaderStyle-BorderColor="White" HeaderText="SpecialDay" ItemStyle-BorderColor="White" ItemStyle-Width="100" SortExpression="SpecialDay">
                                                        <HeaderStyle BackColor="White" BorderColor="White" />
                                                        <ItemStyle BorderColor="White" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                    <EmptyDataRowStyle BorderStyle="None" ForeColor="Red" />
                                                    <HeaderStyle BackColor="White" />
                                                    <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" PageButtonCount="10" />
                                                    <PagerStyle BackColor="White" BorderColor="White" BorderStyle="None" CssClass="pagination-ys" />
                                                    <RowStyle BorderColor="White" />
                                                    <SelectedRowStyle BackColor="White" BorderColor="White" BorderStyle="None" Font-Bold="False" />
                                                </asp:GridView>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="gvEtsyKeyword" EventName="PageIndexChanging" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:constr %>" SelectCommand="SELECT * FROM [V_ListingwithKeywords]">
                                            <FilterParameters>
                                                <%--                <asp:ControlParameter Name="Category" ControlID="ddFilterCategory" PropertyName="SelectedValue"/>--%>
                                                <asp:ControlParameter ControlID="txtFilterSKU" Name="SKU" PropertyName="Text" />
                                                <%--                <asp:ControlParameter Name="MaterialName" ControlID="txtFilterMaterialName" PropertyName="Text" ConvertEmptyStringToNull="false"/>--%>
                                            </FilterParameters>
                                        </asp:SqlDataSource>
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label9" runat="server" Font-Bold="True" Font-Size="Medium" ForeColor="#666666" Text="Keywords"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblKeywordsAmazonFormat" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                        <hr />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                                                                <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                                            <ContentTemplate>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label11" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="#666666" Text="Bullet Point #1"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblBulletPoints1" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="#666666" Text="Bullet Point #2"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblBulletPoints2" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label14" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="#666666" Text="Bullet Point #3"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblBulletPoints3" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label16" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="#666666" Text="Bullet Point #4"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblBulletPoints4" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label18" runat="server" Font-Bold="True" Font-Size="Small" ForeColor="#666666" Text="Bullet Point #5"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblBulletPoints5" runat="server" Font-Bold="False" Font-Size="Small" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                                                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </td>
                           <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnAmazonBulletPointEdit" type="button" href="#" runat="server" onserverclick="btnAmazonBulletPointEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                </tr>
                            </table>
                        </div>
                        <div id="menu2" class="tab-pane fade">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtEbayDescription" runat="server" BorderColor="Silver" BorderStyle="None" BorderWidth="1px" class="form-control" height="500px" ReadOnly="False" TextMode="MultiLine" width="900px"></asp:TextBox>
                                    </td>
                                                                                                <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnEbayDescriptionEdit" type="button" href="#" runat="server" onserverclick="btnEbayDescriptionEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                </tr>
                                <tr>
                                    <td style="height: 20px"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label5" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="#666666" Text="Sale Price"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblSalePriceEbay" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                                <td style="width: 5px"></td>
                                                <td>
                                                    <asp:Label ID="Label8" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="$"></asp:Label>
                                                </td>
                                                                                                                        <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="toEbaySalePriceEdit" type="button" href="#" runat="server" onserverclick="toEbaySalePriceEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="menu3" class="tab-pane fade">
                            <table>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtWixDescription" runat="server" BorderColor="Silver" BorderStyle="None" BorderWidth="1px" class="form-control" height="500px" ReadOnly="False" TextMode="MultiLine" width="900px"></asp:TextBox>
                                    </td>
                                                                                                <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="btnWixDescriptionEdit" type="button" href="#" runat="server" onserverclick="btnWixDescriptionEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                </tr>
                                <tr>
                                    <td style="height: 20px"></td>
                                </tr>
                                <tr>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="Label7" runat="server" Font-Bold="True" Font-Size="Large" ForeColor="#666666" Text="Sale Price"></asp:Label>
                                                </td>
                                                <td style="width: 20px"></td>
                                                <td>
                                                    <asp:Label ID="lblSalePriceWix" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="Title"></asp:Label>
                                                </td>
                                                <td style="width: 5px"></td>
                                                <td>
                                                    <asp:Label ID="Label10" runat="server" Font-Bold="False" Font-Size="Large" ForeColor="#666666" Text="$"></asp:Label>
                                                </td>
                                                                                                                                                                        <td style="width: 20px"></td>
                        <td>
                <center>
                <button id="toWixSalePriceEdit" type="button" href="#" runat="server" onserverclick="toWixSalePriceEdit_click" class="btn btn-default" style="width: 50px; height: 33px; text-align: center;">
                    <center>
                    <i class="glyphicon glyphicon-pencil" style="color: #666666; font-size: 15px;"></i></center>
                    </button>
                        </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </caption>
        </tr>
                                        </table>
                                    </ContentTemplate>
                                            </asp:UpdatePanel>
                </div>
            </div>
            </div>
    <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblSKUforWhere" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblPhotoCheck" runat="server" Text="Label" Visible="False"></asp:Label>
    <%--Visible Area--%>
      </div>
</asp:Content>
