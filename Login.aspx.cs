using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;
using System.Web.Security;

namespace Feniks
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dvMessage.Visible = false;
                lblMessage.Text = "";

                pnlForgotOk.Visible = false;
                pnlForgotErr.Visible = false;
                lblForgotErr.Text = "";
            }
        }

        protected void ValidateUser(object sender, EventArgs e)
        {
            Logger.Log("CLICK ValidateUser");

            dvMessage.Visible = false;
            lblMessage.Text = "";

            string usernameOrEmail = (txtUsername.Text ?? "").Trim();
            string password = (txtPassword.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                dvMessage.Visible = true;
                lblMessage.Text = "Please enter username/email and password.";
                HideLoadingClient();
                return;
            }

            try
            {
                int userId = CallValidateUser(usernameOrEmail, password);

                HideLoadingClient();

                Logger.Log("ValidateUser got userId, continuing...");

                switch (userId)
                {
                    case -1:
                        dvMessage.Visible = true;
                        lblMessage.Text = "Username/Email and/or password is incorrect.";
                        return;

                    case -2:
                        dvMessage.Visible = true;
                        lblMessage.Text = "Account has not been activated.";
                        return;

                    default:
                        bool rememberMe = chkRememberMe.Checked;
                        FormsAuthentication.SetAuthCookie(usernameOrEmail, rememberMe);

                        string returnUrl = Request.QueryString["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            Response.Redirect(returnUrl, true);
                            return;
                        }

                        Response.Redirect("~/MenuRoute.aspx", true);
                        return;
                }
            }
            catch (SqlException ex)
            {
                dvMessage.Visible = true;
                lblMessage.Text = "SQL error: " + ex.Number + " - " + ex.Message;
                HideLoadingClient();
            }
            catch (Exception ex)
            {
                dvMessage.Visible = true;
                lblMessage.Text = "Error: " + ex.Message;
                HideLoadingClient();
            }
        }

        private int CallValidateUser(string usernameOrEmail, string password)
        {
            Logger.Log("CallValidateUser START | u=" + usernameOrEmail);

            string raw = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            var b = new SqlConnectionStringBuilder(raw);
            b.ConnectTimeout = 5;
            b.ConnectRetryCount = 0;
            string constr = b.ToString();

            Logger.Log("Before SqlConnection.Open()");

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand("Validate_User", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 10;

                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = usernameOrEmail;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = password;

                con.Open();
                Logger.Log("After SqlConnection.Open()");

                object result = cmd.ExecuteScalar();
                Logger.Log("After ExecuteScalar() | result=" + (result == null ? "NULL" : result.ToString()));

                if (result == null) return -1;
                return Convert.ToInt32(result);
            }
        }

        protected void btnSendReset_Click(object sender, EventArgs e)
        {
            pnlForgotOk.Visible = false;
            pnlForgotErr.Visible = false;
            lblForgotErr.Text = "";

            string email = (txtForgotEmail.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                pnlForgotErr.Visible = true;
                lblForgotErr.Text = "Please enter your email.";
                HideLoadingClient();
                ReopenModalClient();
                return;
            }

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                int userId = 0;
                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 UserId FROM dbo.Users WHERE Email=@Email", con))
                {
                    cmd.CommandTimeout = 15;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                    con.Open();
                    object v = cmd.ExecuteScalar();
                    con.Close();

                    if (v != null)
                        userId = Convert.ToInt32(v);
                }

                if (userId <= 0)
                {
                    pnlForgotOk.Visible = true;
                    HideLoadingClient();
                    ReopenModalClient();
                    return;
                }

                string token = Guid.NewGuid().ToString("N");
                DateTime expires = DateTime.UtcNow.AddHours(2);

                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.T_PasswordReset(UserId, Email, Token, ExpiresAtUtc, IsUsed, CreatedAtUtc)
VALUES(@UserId, @Email, @Token, @Exp, 0, GETUTCDATE())", con))
                {
                    cmd.CommandTimeout = 15;
                    cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                    cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = token;
                    cmd.Parameters.Add("@Exp", SqlDbType.DateTime).Value = expires;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                string resetUrl = GetBaseUrl() + "/ResetPassword.aspx?token=" + HttpUtility.UrlEncode(token);

                SendMail(
                    to: email,
                    subject: "lamaX - Password Reset",
                    bodyHtml:
                        "<p>Hello,</p>" +
                        "<p>You requested a password reset for lamaX.</p>" +
                        "<p><a href='" + resetUrl + "'>Click here to reset your password</a></p>" +
                        "<p>This link expires in 2 hours.</p>"
                );

                pnlForgotOk.Visible = true;
            }
            catch (Exception ex)
            {
                pnlForgotErr.Visible = true;
                lblForgotErr.Text = ex.Message;
            }
            finally
            {
                HideLoadingClient();
                ReopenModalClient();
            }
        }

        private string GetBaseUrl()
        {
            var req = HttpContext.Current.Request;
            var uri = req.Url;
            string appPath = req.ApplicationPath;
            if (appPath == "/") appPath = "";
            return uri.Scheme + "://" + uri.Authority + appPath;
        }

        private void SendMail(string to, string subject, string bodyHtml)
        {
            var from = ConfigurationManager.AppSettings["MailFrom"];
            if (string.IsNullOrWhiteSpace(from))
                from = "no-reply@hgerman.pl";

            using (var msg = new MailMessage(from, to, subject, bodyHtml))
            {
                msg.IsBodyHtml = true;
                using (var client = new SmtpClient())
                {
                    client.Send(msg);
                }
            }
        }

        private void HideLoadingClient()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "hideLoading", "hideLoading();", true);
        }

        private void ReopenModalClient()
        {
            ClientScript.RegisterStartupScript(this.GetType(), "reopenModal", "$('#forgotModal').modal('show');", true);
        }
    }
}