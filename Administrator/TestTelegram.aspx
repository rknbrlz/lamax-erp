<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestTelegram.aspx.cs" Inherits="Feniks.Administrator.TestTelegram" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Telegram</title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding:30px;font-family:Arial;">
            <asp:Button ID="btnSend" runat="server" Text="Send Telegram Test" OnClick="btnSend_Click" />
            <br /><br />
            <asp:Label ID="lblResult" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>