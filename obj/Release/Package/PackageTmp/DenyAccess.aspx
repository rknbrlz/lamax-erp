<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DenyAccess.aspx.cs" Inherits="Feniks.DenyAccess" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Deny Access | lamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <style>
        body{background:#f5f6f8;font-family:Arial;}
        .card{max-width:720px;margin:40px auto;background:#fff;border-radius:14px;box-shadow:0 12px 30px rgba(0,0,0,.08);padding:22px;}
        .title{font-weight:900;font-size:20px;margin-bottom:8px;}
    </style>
</head>
<body>
    <form id="form1" runat="server" class="card">
        <div class="title">Deny Access</div>

        <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="alert alert-success" style="border-radius:10px;">
            <strong>Denied.</strong> The requester has been notified.
        </asp:Panel>

        <asp:Panel ID="pnlErr" runat="server" Visible="false" CssClass="alert alert-danger" style="border-radius:10px;">
            <strong>Error!</strong> <asp:Label ID="lblErr" runat="server" />
        </asp:Panel>

        <asp:Panel ID="pnlInfo" runat="server" Visible="false" CssClass="alert alert-info" style="border-radius:10px;">
            <strong>Info:</strong> <asp:Label ID="lblInfo" runat="server" />
        </asp:Panel>
    </form>
</body>
</html>