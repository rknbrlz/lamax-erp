<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Feniks.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login | LamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="~/lamaX_logo_trans.ico" rel="shortcut icon" type="image/x-icon" />

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <script src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.3.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.0.3/js/bootstrap.min.js"></script>

    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap" rel="stylesheet" />

    <style>
        * { box-sizing: border-box; }

        html, body { height: 100%; }

        body {
            margin: 0;
            font-family: 'Inter', 'Segoe UI', Arial, sans-serif;
            color: #2f3349;
            background:
                radial-gradient(circle at top left, rgba(140,112,255,.16) 0%, rgba(140,112,255,0) 28%),
                radial-gradient(circle at bottom right, rgba(108,76,255,.10) 0%, rgba(108,76,255,0) 25%),
                linear-gradient(180deg, #f5f6fb 0%, #eff1f8 100%);
        }

        .page-shell {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 24px;
        }

        .login-shell {
            width: 1320px;
            max-width: 100%;
            min-height: 760px;
            border-radius: 26px;
            overflow: hidden;
            background: rgba(255,255,255,.72);
            box-shadow:
                0 24px 80px rgba(26, 33, 61, .12),
                0 8px 22px rgba(26, 33, 61, .05);
            border: 1px solid rgba(255,255,255,.65);
            backdrop-filter: blur(14px);
            -webkit-backdrop-filter: blur(14px);
        }

        .login-form {
            display: flex;
            width: 100%;
            min-height: 760px;
        }

        /* LEFT */
        .visual-side {
            position: relative;
            flex: 1;
            min-width: 0;
            overflow: hidden;
            padding: 30px 34px;
            background:
                linear-gradient(180deg, rgba(255,255,255,.38) 0%, rgba(255,255,255,.18) 100%),
                linear-gradient(180deg, #f5f5fb 0%, #eff1f7 100%);
        }

        .visual-side:before {
            content: "";
            position: absolute;
            width: 520px;
            height: 520px;
            border-radius: 50%;
            left: -120px;
            top: -120px;
            background: radial-gradient(circle, rgba(140,112,255,.18) 0%, rgba(140,112,255,0) 70%);
            pointer-events: none;
        }

        .visual-side:after {
            content: "";
            position: absolute;
            width: 500px;
            height: 500px;
            border-radius: 50%;
            right: -140px;
            bottom: -140px;
            background: radial-gradient(circle, rgba(108,76,255,.12) 0%, rgba(108,76,255,0) 70%);
            pointer-events: none;
        }

        .brand-top {
            display: flex;
            align-items: center;
            gap: 10px;
            position: relative;
            z-index: 5;
        }

        .brand-top img {
            width: 36px;
            height: 36px;
            object-fit: contain;
            filter: drop-shadow(0 8px 18px rgba(108,76,255,.18));
        }

        .brand-top .brand-name {
            font-family: 'Plus Jakarta Sans', 'Inter', sans-serif;
            font-size: 21px;
            font-weight: 500;
            color: #6c4cff;
            letter-spacing: .9px;
        }

        .visual-stage {
            position: absolute;
            inset: 0;
            padding: 92px 46px 74px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .stage-inner {
            position: relative;
            width: 100%;
            max-width: 820px;
            height: 100%;
            max-height: 560px;
        }

        .soft-wave {
            position: absolute;
            left: 4%;
            right: 3%;
            bottom: 4%;
            height: 160px;
            border-radius: 45% 55% 0 0 / 100% 100% 0 0;
            background: linear-gradient(180deg, rgba(255,255,255,0) 0%, rgba(213,217,233,.58) 100%);
            filter: blur(.2px);
        }

        .glow-ring {
            position: absolute;
            left: 50%;
            top: 50%;
            width: 430px;
            height: 430px;
            transform: translate(-50%, -50%);
            border-radius: 50%;
            background: radial-gradient(circle, rgba(140,112,255,.20) 0%, rgba(140,112,255,.06) 42%, rgba(140,112,255,0) 72%);
            z-index: 1;
            filter: blur(2px);
        }

        .hero-wrap {
            position: absolute;
            left: 50%;
            top: 51%;
            transform: translate(-50%, -50%);
            z-index: 3;
            text-align: center;
        }

        .hero-figure {
            position: relative;
            width: 360px;
            max-width: 44vw;
            z-index: 3;
            opacity: .98;
            filter: drop-shadow(0 24px 38px rgba(73, 67, 112, .18));
        }

        .hero-fallback {
            position: relative;
            z-index: 3;
            width: 160px;
            height: 160px;
            border-radius: 26px;
            background: rgba(255,255,255,.65);
            border: 1px solid rgba(229,222,253,.9);
            display: none;
            align-items: center;
            justify-content: center;
            flex-direction: column;
            box-shadow: 0 22px 44px rgba(108,76,255,.10);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
        }

        .hero-fallback img {
            width: 60px;
            margin-bottom: 10px;
            object-fit: contain;
        }

        .hero-fallback .hero-text {
            font-family: 'Plus Jakarta Sans', 'Inter', sans-serif;
            font-size: 13px;
            font-weight: 400;
            color: #7c7f93;
            letter-spacing: .2px;
        }

        .float-card {
            position: absolute;
            background: rgba(255,255,255,.80);
            border: 1px solid rgba(236,236,246,.95);
            border-radius: 18px;
            box-shadow:
                0 18px 34px rgba(31,41,55,.08),
                0 3px 10px rgba(31,41,55,.03);
            padding: 14px 16px;
            z-index: 4;
            backdrop-filter: blur(8px);
            -webkit-backdrop-filter: blur(8px);
            animation: floatY 5.2s ease-in-out infinite;
        }

        .float-card.small { min-width: 124px; }

        .float-card .fc-label {
            font-size: 11px;
            color: #969bb0;
            margin-bottom: 5px;
            font-weight: 500;
        }

        .float-card .fc-value {
            font-size: 22px;
            line-height: 1.1;
            font-weight: 800;
            color: #2f3349;
        }

        .float-card .fc-sub {
            font-size: 11px;
            margin-top: 6px;
            color: #8c92a8;
        }

        .float-card .fc-green {
            color: #34b37a;
            font-weight: 700;
            font-size: 11px;
        }

        .float-card .fc-red {
            color: #ea5455;
            font-weight: 700;
            font-size: 11px;
        }

        .card-project {
            top: 64px;
            left: 12%;
            animation-delay: .1s;
        }

        .card-sales {
            bottom: 138px;
            left: 10%;
            animation-delay: .8s;
        }

        .card-profit {
            right: 13%;
            bottom: 116px;
            min-width: 180px;
            animation-delay: .4s;
        }

        .card-insight {
            top: 118px;
            right: 13%;
            min-width: 152px;
            animation-delay: 1.1s;
        }

        .mini-chart {
            height: 38px;
            margin-top: 8px;
            display: flex;
            align-items: flex-end;
            gap: 6px;
        }

        .mini-chart span {
            display: inline-block;
            width: 10px;
            border-radius: 999px 999px 4px 4px;
            background: linear-gradient(180deg, #8c70ff 0%, #6c4cff 100%);
        }

        .line-chart {
            margin-top: 10px;
            height: 46px;
            position: relative;
        }

        .line-chart svg {
            width: 100%;
            height: 46px;
            display: block;
        }

        .leaf-mark {
            position: absolute;
            left: 46px;
            bottom: 40px;
            width: 44px;
            height: 70px;
            z-index: 3;
            opacity: .9;
        }

        .leaf-mark:before,
        .leaf-mark:after {
            content: "";
            position: absolute;
            background: linear-gradient(180deg, #8c70ff 0%, #6c4cff 100%);
            border-radius: 100% 0 100% 0;
        }

        .leaf-mark:before {
            width: 24px;
            height: 42px;
            left: 0;
            top: 10px;
            transform: rotate(-24deg);
        }

        .leaf-mark:after {
            width: 24px;
            height: 42px;
            right: 0;
            top: 0;
            transform: rotate(22deg);
        }

        .leaf-stem {
            position: absolute;
            left: 20px;
            bottom: 0;
            width: 4px;
            height: 26px;
            background: #bfb6f4;
            border-radius: 999px;
        }

        @keyframes floatY {
            0%   { transform: translateY(0); }
            50%  { transform: translateY(-8px); }
            100% { transform: translateY(0); }
        }

        /* RIGHT */
        .form-side {
            width: 400px;
            flex-shrink: 0;
            position: relative;
            background: rgba(255,255,255,.82);
            border-left: 1px solid rgba(233,235,244,.95);
            padding: 74px 38px 34px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
        }

        .welcome-title {
            margin: 0 0 8px 0;
            font-family: 'Plus Jakarta Sans', 'Inter', sans-serif;
            font-size: 28px;
            font-weight: 500;
            color: #2f3349;
            letter-spacing: -.3px;
        }

        .welcome-sub {
            color: #8f95a8;
            font-size: 13px;
            line-height: 1.65;
            margin-bottom: 24px;
        }

        .field-label {
            display: block;
            font-size: 12px;
            font-weight: 700;
            color: #8b91a5;
            margin: 0 0 7px 2px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-control {
            height: 48px;
            border-radius: 12px !important;
            border: 1px solid #e5e7f0;
            box-shadow: none !important;
            font-size: 14px;
            color: #2f3349;
            padding-left: 14px;
            padding-right: 14px;
            background: rgba(255,255,255,.96);
            transition: all .2s ease;
        }

        .form-control:focus {
            border-color: #7c5cff;
            box-shadow: 0 0 0 4px rgba(124,92,255,.10) !important;
            background: #fff;
        }

        .password-wrap {
            position: relative;
        }

        .password-wrap .form-control {
            padding-right: 44px;
        }

        .pass-eye {
            position: absolute;
            right: 14px;
            top: 50%;
            transform: translateY(-50%);
            color: #a1a6ba;
            font-size: 15px;
            cursor: pointer;
            user-select: none;
        }

        .below-row {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            margin: 6px 0 18px;
            flex-wrap: wrap;
        }

        .remember-wrap {
            display: flex;
            align-items: center;
            gap: 7px;
            font-size: 12px;
            color: #8b91a5;
            font-weight: 500;
        }

        .forgot-link {
            font-size: 12px;
            color: #7c5cff;
            text-decoration: none !important;
            font-weight: 700;
            cursor: pointer;
        }

        .forgot-link:hover {
            color: #6848f4;
        }

        .btn-login {
            width: 100%;
            height: 48px;
            border: 0;
            border-radius: 12px;
            font-weight: 800;
            font-size: 13px;
            letter-spacing: .5px;
            color: #fff;
            background: linear-gradient(90deg, #8c70ff 0%, #6c4cff 100%);
            box-shadow: 0 16px 26px rgba(108,76,255,.24);
            transition: all .18s ease;
        }

        .btn-login:hover,
        .btn-login:focus {
            color: #fff;
            transform: translateY(-1px);
            box-shadow: 0 18px 30px rgba(108,76,255,.28);
        }

        .signup-row {
            margin-top: 16px;
            text-align: center;
            color: #9aa0b3;
            font-size: 12px;
        }

        .signup-row a {
            color: #7c5cff;
            text-decoration: none !important;
            font-weight: 700;
            margin-left: 3px;
        }

        .divider-or {
            margin: 18px 0 16px;
            display: flex;
            align-items: center;
            gap: 10px;
            color: #b0b5c7;
            font-size: 12px;
        }

        .divider-or:before,
        .divider-or:after {
            content: "";
            flex: 1;
            height: 1px;
            background: #ececf5;
        }

        .social-row {
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 14px;
        }

        .social-btn {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            border: 1px solid #ebeaf4;
            background: rgba(255,255,255,.95);
            color: #7f8599;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-weight: 800;
            text-decoration: none !important;
            transition: all .18s ease;
        }

        .social-btn:hover {
            transform: translateY(-2px);
            border-color: #dcd8f7;
            color: #6c4cff;
            box-shadow: 0 8px 16px rgba(108,76,255,.10);
        }

        .alert {
            border-radius: 12px;
            margin-top: 14px;
            font-size: 13px;
        }

        /* LOADING */
        .loading-overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(255,255,255,.62);
            z-index: 9999;
            align-items: center;
            justify-content: center;
            backdrop-filter: blur(4px);
            -webkit-backdrop-filter: blur(4px);
        }

        .loading-box {
            background: #fff;
            border-radius: 16px;
            padding: 18px 20px;
            box-shadow: 0 18px 50px rgba(0,0,0,.12);
            text-align: center;
            min-width: 240px;
        }

        .spinner {
            width: 36px;
            height: 36px;
            border: 4px solid #ececf6;
            border-top: 4px solid #6c4cff;
            border-radius: 50%;
            margin: 0 auto 10px auto;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            from { transform: rotate(0deg); }
            to { transform: rotate(360deg); }
        }

        @media (max-width: 1120px) {
            .login-form {
                flex-direction: column;
            }

            .form-side {
                width: 100%;
                border-left: 0;
                border-top: 1px solid rgba(233,235,244,.95);
                padding-top: 40px;
            }

            .visual-side {
                min-height: 500px;
            }

            .card-project { top: 72px; left: 8%; }
            .card-sales { bottom: 110px; left: 7%; }
            .card-profit { right: 9%; bottom: 100px; }
            .card-insight { right: 10%; top: 104px; }
        }

        @media (max-width: 768px) {
            .page-shell {
                padding: 14px;
            }

            .login-shell {
                min-height: auto;
                border-radius: 20px;
            }

            .visual-side {
                min-height: 390px;
                padding: 20px;
            }

            .visual-stage {
                padding: 76px 18px 60px;
            }

            .form-side {
                padding: 28px 20px 24px;
            }

            .welcome-title {
                font-size: 24px;
            }

            .hero-figure {
                width: 220px;
                max-width: 58vw;
            }

            .hero-fallback {
                width: 134px;
                height: 134px;
            }

            .float-card {
                transform: scale(.92);
            }

            .card-project { top: 52px; left: 2%; }
            .card-sales { bottom: 72px; left: 1%; }
            .card-profit { right: 1%; bottom: 74px; }
            .card-insight { top: 72px; right: 2%; }
        }

        @media (max-width: 520px) {
            .float-card,
            .leaf-mark,
            .card-insight {
                display: none;
            }

            .visual-side {
                min-height: 300px;
            }

            .hero-figure {
                width: 190px;
            }
        }
    </style>

    <script type="text/javascript">
        function showLoading(msg) {
            if (msg) $('#loadingText').text(msg);
            $('.loading-overlay').css('display', 'flex');
        }

        function hideLoading() {
            $('.loading-overlay').hide();
        }

        function togglePassword() {
            var $txt = $('#<%= txtPassword.ClientID %>');
            var current = $txt.attr('type');
            if (current === 'password') {
                $txt.attr('type', 'text');
            } else {
                $txt.attr('type', 'password');
            }
        }
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

    <div class="page-shell">
        <div class="login-shell">
            <form id="form1" runat="server" class="login-form">

                <!-- LEFT VISUAL -->
                <div class="visual-side">
                    <div class="brand-top">
                        <img src="Images/lamaX_logo_trans.png" alt="LamaX" />
                        <div class="brand-name">lamaX</div>
                    </div>

                    <div class="visual-stage">
                        <div class="stage-inner">
                            <div class="soft-wave"></div>
                            <div class="glow-ring"></div>

                            <div class="float-card small card-project">
                                <div class="fc-label">Sales</div>
                                <div class="fc-value">862 <span class="fc-red" style="font-size:12px;">-18%</span></div>
                                <div class="fc-sub">Yearly Sales</div>
                            </div>

                            <div class="float-card small card-sales">
                                <div class="fc-label">Total Profit</div>
                                <div class="fc-value" style="font-size:20px;">2,856 <span class="fc-green">+18%</span></div>
                            </div>

                            <div class="float-card card-profit">
                                <div class="fc-label">$86.4k</div>
                                <div class="mini-chart">
                                    <span style="height:9px;"></span>
                                    <span style="height:17px;"></span>
                                    <span style="height:13px;"></span>
                                    <span style="height:24px;"></span>
                                    <span style="height:18px;"></span>
                                    <span style="height:31px;"></span>
                                </div>
                                <div class="fc-sub">Total Profit</div>
                            </div>

                            <div class="float-card card-insight">
                                <div class="fc-label">Conversion</div>
                                <div class="fc-value" style="font-size:18px;">12.8%</div>
                                <div class="line-chart">
                                    <svg viewBox="0 0 120 46" preserveAspectRatio="none">
                                        <path d="M4,35 C18,30 24,24 36,26 C48,28 58,14 70,16 C82,18 90,9 116,6"
                                              fill="none"
                                              stroke="#7c5cff"
                                              stroke-width="3"
                                              stroke-linecap="round" />
                                    </svg>
                                </div>
                                <div class="fc-sub">This month</div>
                            </div>

                            <div class="hero-wrap">
                                <img src="Images/login-hero.png"
                                     alt="lamaX Visual"
                                     class="hero-figure"
                                     onerror="this.style.display='none'; document.getElementById('heroFallback').style.display='flex';" />

                                <div id="heroFallback" class="hero-fallback">
                                    <img src="Images/lamaX_logo_trans.png" alt="LamaX" />
                                    <div class="hero-text">LamaX Platform</div>
                                </div>
                            </div>

                            <div class="leaf-mark">
                                <div class="leaf-stem"></div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- RIGHT FORM -->
                <div class="form-side">
                    <h1 class="welcome-title">Welcome back</h1>
                    <div class="welcome-sub">
                        Sign in to access your lamaX workspace and continue managing your operations.
                    </div>

                    <div class="form-group">
                        <label class="field-label">Email or Username</label>
                        <asp:TextBox ID="txtUsername" runat="server"
                            CssClass="form-control"
                            placeholder="Enter your email or username" />
                    </div>

                    <div class="form-group">
                        <label class="field-label">Password</label>
                        <div class="password-wrap">
                            <asp:TextBox ID="txtPassword" runat="server"
                                TextMode="Password"
                                CssClass="form-control"
                                placeholder="••••••••" />
                            <span class="pass-eye" onclick="togglePassword()">👁</span>
                        </div>
                    </div>

                    <div class="below-row">
                        <label class="remember-wrap">
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            <span>Remember me</span>
                        </label>

                        <a class="forgot-link" data-toggle="modal" data-target="#forgotModal">Forgot Password?</a>
                    </div>

                    <asp:Button ID="btnLogin" runat="server"
                        Text="LOGIN"
                        CssClass="btn btn-login"
                        OnClick="ValidateUser"
                        UseSubmitBehavior="true"
                        OnClientClick="showLoading('Signing in...');" />

                    <div id="dvMessage" runat="server" visible="false" class="alert alert-danger">
                        <strong>Error!</strong>
                        <asp:Label ID="lblMessage" runat="server" />
                    </div>

                    <div class="signup-row">
                        New on our platform?
                        <a href="SignupRequest.aspx">Create an account</a>
                    </div>

                    <div class="divider-or">or</div>

                    <div class="social-row">
                        <a href="javascript:void(0)" class="social-btn">f</a>
                        <a href="javascript:void(0)" class="social-btn">t</a>
                        <a href="javascript:void(0)" class="social-btn">in</a>
                        <a href="javascript:void(0)" class="social-btn">G</a>
                    </div>

                    <asp:Label ID="lblLoginName" runat="server" Visible="False" />
                    <asp:Label ID="lblLocation" runat="server" Visible="False" />
                    <asp:Label ID="lblRole" runat="server" Visible="False" />
                </div>

                <!-- MODAL -->
                <div class="modal fade" id="forgotModal" tabindex="-1" role="dialog" aria-labelledby="forgotModalLabel">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content" style="border-radius:16px; overflow:hidden;">
                            <div class="modal-header" style="background:#f7f7fc;">
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span>&times;</span></button>
                                <h4 class="modal-title" id="forgotModalLabel" style="font-weight:800;">Reset Password</h4>
                            </div>

                            <div class="modal-body">
                                <p style="color:#6b7280;font-size:13px;margin-top:0;">
                                    Enter your email address. We’ll send you a secure password reset link.
                                </p>

                                <div class="form-group">
                                    <label class="field-label">Email</label>
                                    <asp:TextBox ID="txtForgotEmail" runat="server" CssClass="form-control" placeholder="you@example.com" />
                                </div>

                                <asp:Panel ID="pnlForgotOk" runat="server" Visible="false" CssClass="alert alert-success" style="border-radius:12px;">
                                    <strong>Done!</strong> If the email exists, we sent a reset link.
                                </asp:Panel>

                                <asp:Panel ID="pnlForgotErr" runat="server" Visible="false" CssClass="alert alert-danger" style="border-radius:12px;">
                                    <strong>Error!</strong> <asp:Label ID="lblForgotErr" runat="server" />
                                </asp:Panel>
                            </div>

                            <div class="modal-footer" style="background:#fff;">
                                <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>

                                <asp:Button ID="btnSendReset" runat="server" Text="Send reset link"
                                    CssClass="btn btn-primary"
                                    Style="background:#6c4cff;border-color:#6c4cff;border-radius:10px;"
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