using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Feniks
{
    public partial class DenyAccess : System.Web.UI.Page
    {
        private string Token => (Request.QueryString["token"] ?? "").Trim();

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlOk.Visible = false;
            pnlErr.Visible = false;
            pnlInfo.Visible = false;
            lblErr.Text = "";
            lblInfo.Text = "";

            if (IsPostBack) return;

            if (string.IsNullOrWhiteSpace(Token))
            {
                pnlErr.Visible = true;
                lblErr.Text = "Missing token.";
                return;
            }

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                string email = null;
                string status = null;

                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 Email, Status
FROM dbo.T_AccessRequest
WHERE Token=@Token", con))
                {
                    cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = Token;
                    con.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            pnlErr.Visible = true;
                            lblErr.Text = "Request not found / invalid token.";
                            return;
                        }
                        email = r["Email"] as string;
                        status = r["Status"] as string;
                    }
                }

                if (!string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    pnlInfo.Visible = true;
                    lblInfo.Text = "This request is already processed: " + status;
                    return;
                }

                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.T_AccessRequest
SET Status='Denied', ProcessedAtUtc=GETUTCDATE()
WHERE Token=@Token", con))
                {
                    cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = Token;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                SendMail(email, "lamaX - Access Request Denied",
                    "<p>Hello,</p><p>Your access request has been <b>denied</b>.</p><p>If you think this is a mistake, contact the administrator.</p>");

                pnlOk.Visible = true;
            }
            catch (Exception ex)
            {
                pnlErr.Visible = true;
                lblErr.Text = ex.Message;
            }
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
    }
}