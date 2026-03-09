<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Feniks.ResetPassword" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password | lamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />

    <style>
        body{
            background: radial-gradient(circle at 20% 20%, #eef2ff 0%, #f5f6f8 40%, #eef7ff 100%);
            font-family: Arial, sans-serif;
        }
        .card{
            max-width: 520px;
            margin: 60px auto;
            background:#fff;
            border-radius: 18px;
            box-shadow: 0 18px 50px rgba(0,0,0,.10);
            padding: 26px;
        }
        .title{ font-weight:900; font-size: 22px; margin-bottom: 8px; }
        .sub{ color:#6b7280; font-size: 13px; margin-bottom: 18px; }
        .form-control{ height:44px; border-radius: 10px; }
        .btn-primary{
            height: 44px; border-radius: 999px; font-weight:800; border:0;
            background: linear-gradient(135deg, #2f5bd4 0%, #243a92 100%);
        }
    </style>
</head>

<body>
    <div class="card">
        <div class="title">Set a new password</div>
        <div class="sub">Enter your new password below.</div>

        <form id="form1" runat="server">
            <asp:Panel ID="pnlInvalid" runat="server" Visible="false" CssClass="alert alert-danger" style="border-radius:10px;">
                <strong>Invalid link.</strong> This reset link is expired or already used.
            </asp:Panel>

            <asp:Panel ID="pnlForm" runat="server" Visible="true">
                <div class="form-group">
                    <label style="font-size:12px;color:#6b7280;">New Password</label>
                    <asp:TextBox ID="txtNewPass" runat="server" CssClass="form-control" TextMode="Password" />
                </div>
                <div class="form-group">
                    <label style="font-size:12px;color:#6b7280;">Confirm Password</label>
                    <asp:TextBox ID="txtNewPass2" runat="server" CssClass="form-control" TextMode="Password" />
                </div>

                <asp:Button ID="btnSave" runat="server" Text="SAVE PASSWORD" CssClass="btn btn-primary"
                    OnClick="btnSave_Click" />

                <a href="Login.aspx" class="btn btn-link" style="margin-left:10px;">Back to Login</a>

                <asp:Panel ID="pnlErr" runat="server" Visible="false" CssClass="alert alert-danger" style="border-radius:10px;margin-top:14px;">
                    <strong>Error!</strong> <asp:Label ID="lblErr" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="alert alert-success" style="border-radius:10px;margin-top:14px;">
                    <strong>Done!</strong> Password updated. You can sign in now.
                </asp:Panel>
            </asp:Panel>
        </form>
    </div>
</body>
</html>