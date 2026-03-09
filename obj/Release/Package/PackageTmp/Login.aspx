<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Feniks.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login | lamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="~/lamaX_logo_trans.ico" rel="shortcut icon" type="image/x-icon" />

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js"></script>

    <style>
        html, body { height: 100%; }
        body {
            margin: 0;
            background: radial-gradient(circle at 20% 20%, #eef2ff 0%, #f5f6f8 40%, #eef7ff 100%);
            font-family: Arial, sans-serif;
        }

        .auth-wrap{
            min-height: 100%;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
        }

        .auth-card{
            width: 980px;
            max-width: 100%;
            background: #fff;
            border-radius: 18px;
            box-shadow: 0 18px 50px rgba(0,0,0,.10);
            overflow: hidden;
        }

        /* ✅ FORM'u flex container yaptık */
        .auth-form{
            width: 100%;
            display: flex;
        }

        .auth-left{
            flex: 1;
            padding: 44px 46px;
            background: #fff;
            min-width: 0;
        }

        .auth-right{
            width: 380px;
            flex-shrink: 0;
            background: linear-gradient(135deg, #2f5bd4 0%, #243a92 100%);
            color: #fff;
            padding: 44px 38px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            text-align: center;
        }

        .brand-row{
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 18px;
        }
        .brand-row img{ width: 52px; height: auto; }
        .brand-title{
            font-weight: 800;
            font-size: 18px;
            letter-spacing: .2px;
            color: #111;
        }

        .auth-title{
            margin: 8px 0 6px 0;
            font-size: 28px;
            font-weight: 800;
            color: #111;
        }

        .auth-subtitle{
            color: #7b7f8a;
            margin-bottom: 18px;
            font-size: 13px;
        }

        .or-line{
            display: flex;
            align-items: center;
            gap: 12px;
            color: #9aa0ad;
            font-size: 12px;
            margin: 12px 0 18px 0;
        }
        .or-line:before,
        .or-line:after{
            content: "";
            flex: 1;
            height: 1px;
            background: #eceef4;
        }

        .form-control{
            height: 44px;
            border-radius: 10px !important;
            border: 1px solid #e6e8ee;
            box-shadow: none !important;
        }
        .form-control:focus{ border-color: #3d67db; }

        .btn-primary{
            height: 44px;
            border-radius: 999px;
            font-weight: 700;
            border: 0;
            background: linear-gradient(135deg, #2f5bd4 0%, #243a92 100%);
            box-shadow: 0 10px 22px rgba(47,91,212,.28);
        }

        .forgot{
            display: inline-block;
            margin-top: 10px;
            font-size: 12px;
            color: #6b7280;
            text-decoration: none !important;
            cursor: pointer;
        }
        .forgot:hover{ color: #2f5bd4; }

        .right-title{ font-size: 26px; font-weight: 900; margin-bottom: 10px; }
        .right-text{ opacity: .9; font-size: 13px; line-height: 1.5; margin-bottom: 18px; }

        .btn-outline{
            display: inline-block;
            padding: 10px 18px;
            border-radius: 999px;
            border: 2px solid rgba(255,255,255,.7);
            color: #fff !important;
            text-decoration: none !important;
            font-weight: 800;
            letter-spacing: .3px;
        }

        .alert{ border-radius: 10px; margin-top: 14px; }

        /* Loading overlay */
        .loading-overlay{
            display:none;
            position: fixed;
            inset: 0;
            background: rgba(255,255,255,.65);
            z-index: 9999;
            align-items: center;
            justify-content: center;
        }
        .loading-box{
            background:#fff;
            border-radius:14px;
            padding:18px 20px;
            box-shadow:0 18px 50px rgba(0,0,0,.12);
            text-align:center;
            min-width: 240px;
        }
        .spinner{
            width: 34px;
            height: 34px;
            border: 4px solid #e9ecf5;
            border-top: 4px solid #2f5bd4;
            border-radius: 50%;
            margin: 0 auto 10px auto;
            animation: spin 1s linear infinite;
        }
        @keyframes spin { from {transform:rotate(0deg);} to {transform:rotate(360deg);} }

        /* ✅ Responsive: mobilde alt alta */
        @media (max-width: 900px){
            .auth-form{ flex-direction: column; }
            .auth-right{ width: 100%; }
        }
    </style>

    <script type="text/javascript">
        function showLoading(msg) {
            if (msg) $('#loadingText').text(msg);
            $('.loading-overlay').css('display', 'flex');
        }
        function hideLoading() { $('.loading-overlay').hide(); }
    </script>
</head>

<body>
    <div class="loading-overlay" id="loadingOverlay">
        <div class="loading-box">
            <div class="spinner"></div>
            <div id="loadingText" style="font-weight:700;color:#111;">Please wait...</div>
            <div style="font-size:12px;color:#6b7280;margin-top:4px;">Processing</div>
        </div>
    </div>

    <div class="auth-wrap">
        <div class="auth-card">

            <!-- ✅ Tüm asp kontrolleri tek form içinde -->
            <form id="form1" runat="server" class="auth-form">

                <!-- LEFT -->
                <div class="auth-left">
                    <div class="brand-row">
                        <img src="Images/lamaX_logo_trans.png" alt="lamaX" />
                        <div class="brand-title">lamaX</div>
                    </div>

                    <div class="auth-title">Sign In</div>
                    <div class="auth-subtitle">Use your LamaX Account</div>

                    <div class="or-line">use your account</div>

                    <div class="form-group">
                        <asp:TextBox ID="txtUsername" runat="server"
                            CssClass="form-control"
                            placeholder="Email Address / Username"
                            required="Yes" />
                    </div>

                    <div class="form-group">
                        <asp:TextBox ID="txtPassword" runat="server"
                            TextMode="Password"
                            CssClass="form-control"
                            placeholder="Password"
                            required="Yes" />
                    </div>

                    <asp:CheckBox ID="chkRememberMe" Visible="false" Text="Remember Me" runat="server" style="font-size: 13px" />

                    <asp:Button ID="btnLogin" runat="server"
                        Text="SIGN IN"
                        CssClass="btn btn-primary btn-block"
                        OnClick="ValidateUser"
                        UseSubmitBehavior="true"
                        OnClientClick="showLoading('Signing in...');" />

                    <a class="forgot" data-toggle="modal" data-target="#forgotModal">Forgot your password?</a>

                    <div id="dvMessage" runat="server" visible="false" class="alert alert-danger">
                        <strong style="font-size: small">Error!</strong>
                        <asp:Label ID="lblMessage" runat="server" Font-Size="Small" />
                    </div>

                    <asp:Label ID="lblLoginName" runat="server" Visible="False" />
                    <asp:Label ID="lblLocation" runat="server" Visible="False" />
                    <asp:Label ID="lblRole" runat="server" Visible="False" />
                </div>

                <!-- RIGHT -->
                <div class="auth-right">
                    <div class="right-title">Hey There!</div>
                    <div class="right-text">
                        Begin your journey by requesting access. Your request will be sent to Erkan for approval.
                    </div>
                    <a class="btn-outline" href="SignupRequest.aspx">SIGN UP</a>
                </div>

                <!-- Forgot Password Modal (form içinde) -->
                <div class="modal fade" id="forgotModal" tabindex="-1" role="dialog" aria-labelledby="forgotModalLabel">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content" style="border-radius:14px; overflow:hidden;">
                            <div class="modal-header" style="background:#f6f7fb;">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span>&times;</span></button>
                                <h4 class="modal-title" id="forgotModalLabel" style="font-weight:800;">Reset Password</h4>
                            </div>

                            <div class="modal-body">
                                <p style="color:#6b7280;font-size:13px;margin-top:0;">
                                    Enter your email address. We'll send you a secure password reset link.
                                </p>

                                <div class="form-group">
                                    <label style="font-size:12px;color:#6b7280;">Email</label>
                                    <asp:TextBox ID="txtForgotEmail" runat="server" CssClass="form-control" placeholder="you@example.com" />
                                </div>

                                <asp:Panel ID="pnlForgotOk" runat="server" Visible="false" CssClass="alert alert-success" style="border-radius:10px;">
                                    <strong>Done!</strong> If the email exists, we sent a reset link.
                                </asp:Panel>

                                <asp:Panel ID="pnlForgotErr" runat="server" Visible="false" CssClass="alert alert-danger" style="border-radius:10px;">
                                    <strong>Error!</strong> <asp:Label ID="lblForgotErr" runat="server" />
                                </asp:Panel>
                            </div>

                            <div class="modal-footer" style="background:#fff;">
                                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>

                                <asp:Button ID="btnSendReset" runat="server" Text="Send reset link"
                                    CssClass="btn btn-primary"
                                    OnClick="btnSendReset_Click"
                                    OnClientClick="showLoading('Sending reset link...'); this.disabled=true; return true;" />
                            </div>
                        </div>
                    </div>
                </div>

            </form>
        </div>
    </div>
</body>
</html>