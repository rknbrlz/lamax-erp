<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuRoute.aspx.cs" Inherits="Feniks.MenuRoute" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
                        <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblRole" runat="server" Text="Label" Visible="True"></asp:Label>
    <%--Visible Area--%>
        </div>
    </form>
</body>
</html>
