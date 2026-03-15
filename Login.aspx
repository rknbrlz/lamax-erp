<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Feniks.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login | lamaX</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="~/lamaX_logo_trans.ico" rel="shortcut icon" type="image/x-icon" />

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>

    <style>
        * {
            box-sizing: border-box;
        }

        html, body, form {
            height: 100%;
            margin: 0;
            padding: 0;
            font-family: "Segoe UI", Arial, Helvetica, sans-serif;
            background: #f3f5f8;
            color: #1f2937;
        }

        body {
            overflow-x: hidden;
        }

        .page-wrap {
            min-height: 100vh;
            display: flex;
            align-items: stretch;
        }

        .left-side {
            width: 42%;
            background: #1f4aa8;
            color: #ffffff;
            padding: 42px 40px;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }

        .brand {
            font-size: 28px;
            font-weight: 700;
            letter-spacing: .4px;
        }

        .brand-sub {
            margin-top: 10px;
            font-size: 13px;
            line-height: 1.7;
            color: rgba(255,255,255,.86);
            max-width: 320px;
        }

        .left-content {
            max-width: 360px;
        }

        .left-title {
            font-size: 34px;
            font-weight: 700;
            line-height: 1.25;
            margin: 0 0 14px 0;
        }

        .left-text {
            font-size: 14px;
            line-height: 1.8;
            color: rgba(255,255,255,.88);
            margin-bottom: 24px;
        }

        .left-button {
            display: inline-block;
            min-width: 150px;
            padding: 12px 20px;
            border: 1px solid rgba(255,255,255,.45);
            color: #ffffff;
            text-decoration: none !important;
            text-align: center;
            font-size: 13px;
            font-weight: 600;
            letter-spacing: .3px;
            background: transparent;
            transition: background .2s ease;
        }

        .left-button:hover {
            background: rgba(255,255,255,.08);
            color: #ffffff;
        }

        .left-footer {
            font-size: 12px;
            color: rgba(255,255,255,.75);
            line-height: 1.8;
        }

        .right-side {
            width: 58%;
            background: #ffffff;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 40px;
        }

        .login-panel {
            width: 100%;
            max-width: 430px;
        }

        .panel-top {
            margin-bottom: 28px;
        }

        .panel-title {
            font-size: 30px;
            font-weight: 700;
            color: #111827;
            margin: 0 0 8px 0;
        }

        .panel-subtitle {
            font-size: 14px;
            color: #6b7280;
            line-height: 1.7;
        }

        .field-group {
            margin-bottom: 18px;
        }

        .field-label {
            display: block;
            margin-bottom: 7px;
            font-size: 12px;
            font-weight: 600;
            color: #374151;
        }

        .input-box {
            width: 100%;
            height: 46px;
            border: 1px solid #d1d5db;
            background: #ffffff;
            padding: 0 14px;
            font-size: 14px;
            color: #111827;
            outline: none;
            transition: border-color .2s ease, box-shadow .2s ease;
        }

        .input-box:focus {
            border-color: #1f4aa8;
            box-shadow: 0 0 0 3px rgba(31, 74, 168, .08);
        }

        .password-wrap {
            position: relative;
        }

        .toggle-password {
            position: absolute;
            top: 50%;
            right: 10px;
            transform: translateY(-50%);
            border: none;
            background: transparent;
            color: #6b7280;
            font-size: 12px;
            font-weight: 600;
            cursor: pointer;
            padding: 4px 6px;
        }

        .meta-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            margin: 12px 0 20px 0;
            flex-wrap: wrap;
        }

        .remember-row {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-size: 13px;
            color: #4b5563;
        }

        .remember-row input[type=checkbox] {
            margin: 0;
            width: 14px;
            height: 14px;
        }

        .simple-link {
            font-size: 13px;
            font-weight: 600;
            color: #1f4aa8;
            text-decoration: none !important;
        }

        .simple-link:hover {
            color: #173a84;
        }

        .btn-submit {
            width: 100%;
            height: 48px;
            border: none;
            background: #1f4aa8;
            color: #ffffff;
            font-size: 14px;
            font-weight: 700;
            letter-spacing: .3px;
            transition: background .2s ease;
        }

        .btn-submit:hover {
            background: #173f92;
        }

        .btn-submit:disabled {
            opacity: .75;
            cursor: not-allowed;
        }

        .error-box {
            margin-bottom: 16px;
            padding: 12px 14px;
            background: #fef2f2;
            border: 1px solid #fecaca;
            color: #b91c1c;
            font-size: 13px;
        }

        .signup-row {
            margin-top: 18px;
            font-size: 13px;
            color: #6b7280;
            text-align: center;
        }

        .signup-row a {
            color: #1f4aa8;
            font-weight: 700;
            text-decoration: none;
        }

        .loading-overlay {
            position: fixed;
            inset: 0;
            background: rgba(17, 24, 39, .28);
            display: none;
            align-items: center;
            justify-content: center;
            z-index: 99999;
        }

        .loading-box {
            width: 220px;
            background: #ffffff;
            padding: 28px 22px;
            text-align: center;
            box-shadow: 0 20px 50px rgba(0,0,0,.18);
        }

        .spinner-modern {
            width: 40px;
            height: 40px;
            margin: 0 auto 14px auto;
            border: 4px solid #dbe4f3;
            border-top-color: #1f4aa8;
            border-radius: 50%;
            animation: spin .8s linear infinite;
        }

        .loading-title {
            font-size: 15px;
            font-weight: 700;
            color: #111827;
            margin-bottom: 4px;
        }

        .loading-text {
            font-size: 13px;
            color: #6b7280;
        }

        @keyframes spin {
            to { transform: rotate(360deg); }
        }

        .modal-content {
            border: none;
            box-shadow: 0 20px 50px rgba(0,0,0,.18);
        }

        .modal-header {
            border-bottom: 1px solid #e5e7eb;
        }

        .modal-title {
            font-weight: 700;
            color: #111827;
        }

        .modal-footer {
            border-top: 1px solid #e5e7eb;
        }

        .soft-alert-ok {
            background: #ecfdf3;
            border: 1px solid #bbf7d0;
            color: #166534;
            padding: 12px 14px;
            margin-bottom: 14px;
            font-size: 13px;
        }

        .soft-alert-err {
            background: #fef2f2;
            border: 1px solid #fecaca;
            color: #b91c1c;
            padding: 12px 14px;
            margin-bottom: 14px;
            font-size: 13px;
        }

        .btn-primary-modern {
            background: #1f4aa8;
            border: none;
            color: #fff;
            padding: 10px 18px;
            font-weight: 600;
        }

        .btn-primary-modern:hover {
            background: #173f92;
            color: #fff;
        }

        .btn-light-modern {
            background: #f9fafb;
            border: 1px solid #d1d5db;
            color: #374151;
            padding: 10px 18px;
            font-weight: 600;
        }

        @media (max-width: 991px) {
            .page-wrap {
                flex-direction: column;
            }

            .left-side,
            .right-side {
                width: 100%;
            }

            .left-side {
                padding: 32px 24px;
            }

            .right-side {
                padding: 32px 20px;
            }

            .left-title {
                font-size: 28px;
            }
        }

        @media (max-width: 575px) {
            .panel-title {
                font-size: 26px;
            }

            .meta-row {
                flex-direction: column;
                align-items: flex-start;
            }
        }
    </style>

    <script type="text/javascript">
        function showLoading() {
            $("#loadingOverlay").css("display", "flex");
        }

        function hideLoading() {
            $("#loadingOverlay").hide();
        }

        function submitLogin() {
            showLoading();
            return true;
        }

        function togglePassword() {
            var txt = document.getElementById('<%= txtPassword.ClientID %>');
            var btn = document.getElementById('btnTogglePassword');

            if (!txt) return false;

            if (txt.type === "password") {
                txt.type = "text";
                if (btn) btn.innerHTML = "Hide";
            } else {
                txt.type = "password";
                if (btn) btn.innerHTML = "Show";
            }

            return false;
        }

        $(document).ready(function () {
            hideLoading();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page-wrap">
            <div class="left-side">
                <div>
                    <div class="brand">lamaX</div>
                    <div class="brand-sub">
                        Operational platform for order, stock, product and marketplace management.
                    </div>
                </div>

                <div class="left-content">
                    <h1 class="left-title">Welcome back</h1>
                    <div class="left-text">
                        Sign in to continue managing your business operations securely and efficiently.
                    </div>
                    <a href="javascript:void(0);" class="left-button" onclick="document.getElementById('<%= txtUsername.ClientID %>').focus();">
                        Reguest Account
                    </a>
                </div>

                <div class="left-footer">
                    lamaX ERP System<br />
                    Secure access panel
                </div>
            </div>

            <div class="right-side">
                <div class="login-panel">
                    <div class="panel-top">
                        <h2 class="panel-title">Sign in</h2>
                        <div class="panel-subtitle">
                            Please enter your credentials to access your account.
                        </div>
                    </div>

                    <div id="dvMessage" runat="server" class="error-box" visible="false">
                        <asp:Label ID="lblMessage" runat="server" />
                    </div>

                    <div class="field-group">
                        <label class="field-label" for="<%= txtUsername.ClientID %>">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="input-box" placeholder="Enter your email or username" />
                    </div>

                    <div class="field-group">
                        <label class="field-label" for="<%= txtPassword.ClientID %>">Password</label>
                        <div class="password-wrap">
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="input-box" TextMode="Password" placeholder="Enter your password" />
                            <button id="btnTogglePassword" type="button" class="toggle-password" onclick="return togglePassword();">Show</button>
                        </div>
                    </div>

                    <div class="meta-row">
                        <label class="remember-row">
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            <span>Remember me</span>
                        </label>

                        <a href="javascript:void(0);" class="simple-link" data-toggle="modal" data-target="#forgotModal">
                            Forgot Password?
                        </a>
                    </div>

                    <asp:Button
                        ID="btnLogin"
                        runat="server"
                        Text="SIGN IN"
                        CssClass="btn-submit"
                        OnClick="ValidateUser"
                        OnClientClick="return submitLogin();" />


                    <asp:Label ID="lblLoginName" runat="server" Visible="false" />
                    <asp:Label ID="lblLocation" runat="server" Visible="false" />
                    <asp:Label ID="lblRole" runat="server" Visible="false" />
                </div>
            </div>
        </div>

        <div id="loadingOverlay" class="loading-overlay">
            <div class="loading-box">
                <div class="spinner-modern"></div>
                <div class="loading-title">Please wait</div>
                <div class="loading-text">Signing in...</div>
            </div>
        </div>

        <div id="forgotModal" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
            <div class="modal-dialog modal-sm" style="width:100%; max-width:520px; margin-top:80px;">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                        <h4 class="modal-title">Reset Password</h4>
                    </div>

                    <div class="modal-body">
                        <p style="font-size:13px; color:#6b7280; line-height:1.7; margin-bottom:16px;">
                            Enter your email address to receive a password reset link.
                        </p>

                        <asp:Panel ID="pnlForgotOk" runat="server" Visible="false" CssClass="soft-alert-ok">
                            Done. If the email exists, a reset link has been sent.
                        </asp:Panel>

                        <asp:Panel ID="pnlForgotErr" runat="server" Visible="false" CssClass="soft-alert-err">
                            <asp:Label ID="lblForgotErr" runat="server" />
                        </asp:Panel>

                        <div class="field-group" style="margin-bottom:0;">
                            <label class="field-label" for="<%= txtForgotEmail.ClientID %>">Email</label>
                            <asp:TextBox ID="txtForgotEmail" runat="server" CssClass="input-box" placeholder="Enter your email address" />
                        </div>
                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-light-modern" data-dismiss="modal">Close</button>
                        <asp:Button
                            ID="btnSendReset"
                            runat="server"
                            Text="Send Reset Link"
                            CssClass="btn btn-primary-modern"
                            OnClick="btnSendReset_Click"
                            OnClientClick="showLoading();" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>