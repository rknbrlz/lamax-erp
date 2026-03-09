using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;

namespace Feniks
{
    public partial class SignupRequest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlOk.Visible = false;
                pnlErr.Visible = false;
                lblErr.Text = "";
            }
        }

        protected void btnRequest_Click(object sender, EventArgs e)
        {
            pnlOk.Visible = false;
            pnlErr.Visible = false;
            lblErr.Text = "";

            string fullName = (txtFullName.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string message = (txtMessage.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                pnlErr.Visible = true;
                lblErr.Text = "Email is required.";
                ClientScript.RegisterStartupScript(this.GetType(), "hideLoading", "hideLoading();", true);
                return;
            }

            try
            {
                string token = Guid.NewGuid().ToString("N");
                int roleId = 2; // default User

                // 1) DB Insert (timeout ekledim)
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.T_AccessRequest(FullName, Email, Message, RoleId, Token, Status)
VALUES(@FullName, @Email, @Message, @RoleId, @Token, 'Pending')", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 20;

                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 200).Value =
                        string.IsNullOrWhiteSpace(fullName) ? (object)DBNull.Value : fullName;

                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;

                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 1000).Value =
                        string.IsNullOrWhiteSpace(message) ? (object)DBNull.Value : message;

                    cmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId;
                    cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = token;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // 2) Email
                string approver = ConfigurationManager.AppSettings["ApproverEmail"] ?? "erkan.barlaz@hgerman.pl";

                string baseUrl = GetBaseUrl();
                string approveUrl = baseUrl + "/ApproveAccess.aspx?token=" + HttpUtility.UrlEncode(token);
                string denyUrl = baseUrl + "/DenyAccess.aspx?token=" + HttpUtility.UrlEncode(token);

                string subject = "lamaX - Access Request (Approval needed)";
                string body =
                    "<p><b>New access request</b></p>" +
                    "<p><b>Name:</b> " + HttpUtility.HtmlEncode(fullName) + "</p>" +
                    "<p><b>Email:</b> " + HttpUtility.HtmlEncode(email) + "</p>" +
                    "<p><b>Role:</b> User</p>" +
                    "<p><b>Message:</b><br/>" + HttpUtility.HtmlEncode(message).Replace("\n", "<br/>") + "</p>" +
                    "<hr/>" +
                    "<p><a href='" + approveUrl + "' style='display:inline-block;padding:10px 16px;background:#1f7a1f;color:#fff;text-decoration:none;border-radius:8px;font-weight:700;'>APPROVE</a> " +
                    "&nbsp;&nbsp;" +
                    "<a href='" + denyUrl + "' style='display:inline-block;padding:10px 16px;background:#b91c1c;color:#fff;text-decoration:none;border-radius:8px;font-weight:700;'>DENY</a></p>" +
                    "<p style='color:#6b7280;font-size:12px;'>Approve will create/activate the user and send credentials by email.</p>";

                SendMail(approver, subject, body);

                pnlOk.Visible = true;

                // İstersen formu boşalt
                txtFullName.Text = "";
                txtEmail.Text = "";
                txtMessage.Text = "";
            }
            catch (Exception ex)
            {
                pnlErr.Visible = true;
                lblErr.Text = ex.Message;
            }
            finally
            {
                // Sayfa geri dönünce overlay kapanacak
                ClientScript.RegisterStartupScript(this.GetType(), "hideLoading", "hideLoading();", true);
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
                from = "info@hgerman.pl";

            using (var msg = new MailMessage(from, to, subject, bodyHtml))
            {
                msg.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    // Bazı ortamlarda SMTP takılırsa sayfa sonsuza kadar beklemesin:
                    client.Timeout = 15000; // 15s

                    client.Send(msg);
                }
            }
        }
    }
}