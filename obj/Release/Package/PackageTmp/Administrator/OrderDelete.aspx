<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderDelete.aspx.cs" Inherits="Feniks.Administrator.OrderDelete" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order Delete</title>

    <link href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" rel="stylesheet" />

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False"
            DataKeyNames="OrderNumber">
            <Columns>
                <asp:BoundField DataField="Marketplace" HeaderText="Marketplace" />
                <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" />
                <asp:BoundField DataField="BuyerFullName" HeaderText="Buyer" />
                <asp:BoundField DataField="Country" HeaderText="Country" />

                <asp:TemplateField HeaderText="Action">
                    <ItemTemplate>
                        <button type="button" class="btn btn-danger btn-sm btnShowDeleteModal"
                            data-order='<%# Eval("OrderNumber") %>'
                            data-market='<%# Eval("Marketplace") %>'
                            data-buyer='<%# Eval("BuyerFullName") %>'
                            data-country='<%# Eval("Country") %>'>
                            Sil
                        </button>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>
    </form>

    <!-- Bootstrap Delete Confirmation Modal -->
    <div id="deleteModal" class="modal fade" tabindex="-1" role="dialog">
      <div class="modal-dialog">
        <div class="modal-content">

          <div class="modal-header">
            <h4 class="modal-title">Siparişi Sil</h4>
          </div>

          <div class="modal-body">
            <p id="deleteInfo"></p>
          </div>

          <div class="modal-footer">
            <asp:HiddenField ID="hfOrderNumber" runat="server" />
            <button type="button" class="btn btn-default" data-dismiss="modal">Vazgeç</button>
            <button type="button" class="btn btn-danger" id="btnConfirmDelete">Sil</button>
          </div>

        </div>
      </div>
    </div>

    <script>
        $(document).ready(function () {
            $(".btnShowDeleteModal").click(function () {
                var order = $(this).data("order");
                var market = $(this).data("market");
                var buyer = $(this).data("buyer");
                var country = $(this).data("country");

                $("#deleteInfo").text("Sipariş: " + order + " | Alıcı: " + buyer + " | Ülke: " + country);
                $("#<%= hfOrderNumber.ClientID %>").val(order);

                $("#deleteModal").modal("show");
            });

            $("#btnConfirmDelete").click(function () {
                __doPostBack('<%= btnHiddenDelete.UniqueID %>', '');
            });
        });
    </script>
</body>
</html>
