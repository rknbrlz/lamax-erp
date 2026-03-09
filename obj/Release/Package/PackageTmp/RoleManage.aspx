<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RoleManage.aspx.cs" Inherits="Feniks.RoleManage" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Role Management | lamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <style>
        body{background:#f5f6f8;font-family:Arial;}
        .card{max-width:980px;margin:30px auto;background:#fff;border-radius:14px;box-shadow:0 12px 30px rgba(0,0,0,.08);padding:22px;}
        .title{font-weight:900;font-size:20px;margin-bottom:10px;}
        .mt10{margin-top:10px;}
    </style>
</head>
<body>
<form id="form1" runat="server" class="card">
    <div class="title">Role Management</div>

    <asp:Panel runat="server" ID="pnlMsg" Visible="false" CssClass="alert" />

    <div class="row">
        <div class="col-sm-4">
            <label>Role Name</label>
            <asp:TextBox ID="txtRoleName" runat="server" CssClass="form-control" MaxLength="100" />
        </div>
        <div class="col-sm-6">
            <label>Description</label>
            <asp:TextBox ID="txtDesc" runat="server" CssClass="form-control" MaxLength="250" />
        </div>
        <div class="col-sm-2">
            <label>&nbsp;</label>
            <asp:Button ID="btnAdd" runat="server" CssClass="btn btn-primary btn-block" Text="Add" OnClick="btnAdd_Click" />
        </div>
    </div>

    <hr class="mt10"/>

    <asp:GridView ID="gvRoles" runat="server" CssClass="table table-striped table-bordered"
        AutoGenerateColumns="false" DataKeyNames="RoleId" OnRowCommand="gvRoles_RowCommand">
        <Columns>
            <asp:BoundField DataField="RoleId" HeaderText="RoleId" />
            <asp:BoundField DataField="RoleName" HeaderText="RoleName" />
            <asp:BoundField DataField="Description" HeaderText="Description" />
            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:LinkButton runat="server" CssClass="btn btn-xs btn-danger"
                        CommandName="del" CommandArgument='<%# Eval("RoleId") %>'
                        OnClientClick="return confirm('Delete role?');">Delete</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</form>
</body>
</html>