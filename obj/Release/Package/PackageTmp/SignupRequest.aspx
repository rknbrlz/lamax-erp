<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignupRequest.aspx.cs" Inherits="Feniks.SignupRequest" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign Up Request | lamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js"></script>

    <style>
        body{
            background: radial-gradient(circle at 20% 20%, #eef2ff 0%, #f5f6f8 40%, #eef7ff 100%);
            font-family: Arial, sans-serif;
        }
        .card{
            max-width: 720px;
            margin: 40px auto;
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
        .loading-overlay{
            display:none; position: fixed; inset:0; background: rgba(255,255,255,.65);
            z-index: 9999; align-items:center; justify-content:center;
        }
        .loading-box{
            background:#fff; border-radius:14px; padding:18px 20px;
            box-shadow:0 18px 50px rgba(0,0,0,.12); text-align:center; min-width:240px;
        }
        .spinner{
            width: 34px; height: 34px; border: 4px solid #e9ecf5; border-top: 4px solid #2f5bd4;
            border-radius: 50%; margin: 0 auto 10px auto; animation: spin 1s linear infinite;
        }
        @keyframes spin { from {transform:rotate(0deg);} to {transform:rotate(360deg);} }
    </style>

    <script>
        function showLoading(msg) {
            if (msg) $('#loadingText').text(msg);
            $('.loading-overlay').css('display', 'flex');
        }
        function hideLoading() { $('.loading-overlay').hide(); }

        // Sayfa yeniden render olduğunda overlay açık kalmasın (garanti)
        $(function () { hideLoading(); });

        // Button disable işlemini submitten SONRA yap (kritik fix)
        function onSendRequestClick(btn) {
            showLoading('Sending request...');
            window.setTimeout(function () { btn.disabled = true; }, 0);
            return true; // postback devam
        }
    </script>
</head>

<body>
    <div class="loading-overlay">
        <div class="loading-box">
            <div class="spinner"></div>
            <div id="loadingText" style="font-weight:700;color:#111;">Please wait...</div>
            <div style="font-size:12px;color:#6b7280;margin-top:4px;">Processing</div>
        </div>
    </div>

    <div class="card">
        <div class="title">Request Access</div>
        <div class="sub">
            This system is private. Your request will be sent to <b>Erkan</b> for approval.
        </div>

        <form id="form1" runat="server">
            <div class="form-group">
                <label style="font-size:12px;color:#6b7280;">Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" placeholder="Your name" />
            </div>

            <div class="form-group">
                <label style="font-size:12px;color:#6b7280;">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="you@example.com" />
            </div>

            <div class="form-group">
                <label style="font-size:12px;color:#6b7280;">Message (optional)</label>
                <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Rows="4"
                    CssClass="form-control" style="height:auto;border-radius:10px;"
                    placeholder="Why do you need access?" />
            </div>

            <asp:Button ID="btnRequest" runat="server" Text="SEND REQUEST"
                CssClass="btn btn-primary"
                OnClick="btnRequest_Click"
                OnClientClick="return onSendRequestClick(this);" />

            <a href="Login.aspx" class="btn btn-link" style="margin-left:10px;">Back to Login</a>

            <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="alert alert-success" style="border-radius:10px;margin-top:14px;">
                <strong>Sent!</strong> Your request has been emailed to Erkan for approval.
            </asp:Panel>

            <asp:Panel ID="pnlErr" runat="server" Visible="false" CssClass="alert alert-danger" style="border-radius:10px;margin-top:14px;">
                <strong>Error!</strong> <asp:Label ID="lblErr" runat="server" />
            </asp:Panel>
        </form>
    </div>
</body>
</html>