<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="Home.aspx.cs" Inherits="Feniks.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>test</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
        </div>
            <%--Visible Area--%>
    <asp:Label ID="lblLoginName" runat="server" Text="Label" Visible="False"></asp:Label>
    <asp:Label ID="lblRole" runat="server" Text="Label" Visible="True"></asp:Label>
    <%--Visible Area--%>
    </form>
</body>
</html>
