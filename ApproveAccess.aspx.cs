using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Web;

namespace Feniks
{
    public partial class ApproveAccess : System.Web.UI.Page
    {
        private string Token => (Request.QueryString["token"] ?? "").Trim();
        private string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

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
                ShowError("Missing token.");
                return;
            }

            try
            {
                var req = GetRequest(ConnStr, Token);
                if (req == null)
                {
                    ShowError("Request not found / invalid token.");
                    return;
                }

                // ✅ daha önce işlenmiş mi?
                if (!string.Equals(req.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    ShowInfo("This request is already processed: " + req.Status);
                    return;
                }

                // ✅ Role geçerli mi? (T_Role içinde var mı ve aktif mi?)
                if (!RoleExists(ConnStr, req.RoleId))
                {
                    ShowError("Invalid RoleId: " + req.RoleId + " (Role not found or inactive).");
                    return;
                }

                // Username olarak email kullanıyoruz
                string username = req.Email;

                // Geçici şifre oluştur
                string tempPass = GenerateTempPassword(10);

                // User create/update
                int userId = EnsureUserExists(ConnStr, username, req.Email, req.FullName, req.RoleId, tempPass);

                // Aktivasyon: UserActivation varsa sil -> kullanıcı aktif olur
                EnsureActivated(ConnStr, userId);

                // Request status -> Approved
                MarkRequest(ConnStr, Token, "Approved");

                // Kullanıcıya mail
                string loginUrl = GetBaseUrl() + "/Login.aspx";
                string subject = "lamaX - Access Approved";
                string body =
                    "<p>Hello,</p>" +
                    "<p>Your access request has been <b>approved</b>.</p>" +
                    "<p><b>Login:</b> " + HttpUtility.HtmlEncode(username) + "<br/>" +
                    "<b>Temporary Password:</b> " + HttpUtility.HtmlEncode(tempPass) + "</p>" +
                    "<p>Login here: <a href='" + loginUrl + "'>" + loginUrl + "</a></p>" +
                    "<p>Please change your password after login.</p>";

                SendMail(req.Email, subject, body);

                pnlOk.Visible = true;
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        // ==========================================================
        // DB: Read Request
        // ==========================================================
        private AccessRequest GetRequest(string constr, string token)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 RequestId, FullName, Email, Message, RoleId, Status
FROM dbo.T_AccessRequest
WHERE Token=@Token", con))
            {
                cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = token;
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new AccessRequest
                    {
                        RequestId = Convert.ToInt32(r["RequestId"]),
                        FullName = r["FullName"] as string,
                        Email = r["Email"] as string,
                        Message = r["Message"] as string,
                        RoleId = Convert.ToInt32(r["RoleId"]),
                        Status = r["Status"] as string
                    };
                }
            }
        }

        // ✅ NEW: Role valid mi?
        private bool RoleExists(string constr, int roleId)
        {
            using (var con = new SqlConnection(constr))
            using (var cmd = new SqlCommand(@"
SELECT COUNT(*)
FROM dbo.T_Role
WHERE RoleId=@r AND IsActive=1", con))
            {
                cmd.Parameters.Add("@r", SqlDbType.Int).Value = roleId;
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // ==========================================================
        // DB: Create/Update User
        // ==========================================================
        private int EnsureUserExists(string constr, string username, string email, string fullName, int roleId, string tempPass)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                int userId = 0;

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 UserId FROM dbo.Users WHERE Username=@U OR Email=@E", con))
                {
                    cmd.Parameters.Add("@U", SqlDbType.NVarChar, 255).Value = username;
                    cmd.Parameters.Add("@E", SqlDbType.NVarChar, 255).Value = email;
                    object v = cmd.ExecuteScalar();
                    if (v != null) userId = Convert.ToInt32(v);
                }

                if (userId > 0)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.Users
SET Username=@U,
    Email=@Email,
    FullName=@FullName,
    RoleId=@RoleId,
    [Password]=@Pass
WHERE UserId=@UserId", con))
                    {
                        cmd.Parameters.Add("@U", SqlDbType.NVarChar, 255).Value = username;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = email;
                        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 255).Value = (object)fullName ?? DBNull.Value;
                        cmd.Parameters.Add("@RoleId", SqlDbType.Int).Value = roleId;
                        cmd.Parameters.Add("@Pass", SqlDbType.NVarChar, 255).Value = tempPass;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

                        cmd.ExecuteNonQuery();
                    }
                    return userId;
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.Users(Username, [Password], Email, CreatedDate, LastLoginDate, RoleId, FullName)
VALUES(@U, @P, @E, GETDATE(), NULL, @R, @F);
SELECT SCOPE_IDENTITY();", con))
                    {
                        cmd.Parameters.Add("@U", SqlDbType.NVarChar, 255).Value = username;
                        cmd.Parameters.Add("@P", SqlDbType.NVarChar, 255).Value = tempPass;
                        cmd.Parameters.Add("@E", SqlDbType.NVarChar, 255).Value = email;
                        cmd.Parameters.Add("@R", SqlDbType.Int).Value = roleId;
                        cmd.Parameters.Add("@F", SqlDbType.NVarChar, 255).Value = (object)fullName ?? DBNull.Value;

                        object v = cmd.ExecuteScalar();
                        return Convert.ToInt32(v);
                    }
                }
            }
        }

        private void EnsureActivated(string constr, int userId)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.UserActivation WHERE UserId=@UserId", con))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void MarkRequest(string constr, string token, string status)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.T_AccessRequest
SET Status=@S, ProcessedAtUtc=GETUTCDATE()
WHERE Token=@T", con))
            {
                cmd.Parameters.Add("@S", SqlDbType.NVarChar, 20).Value = status;
                cmd.Parameters.Add("@T", SqlDbType.NVarChar, 64).Value = token;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ==========================================================
        // Helpers: URL + Mail + Password
        // ==========================================================
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

        private string GenerateTempPassword(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            var rnd = new Random();
            char[] buffer = new char[len];
            for (int i = 0; i < len; i++)
                buffer[i] = chars[rnd.Next(chars.Length)];
            return new string(buffer);
        }

        // ==========================================================
        // UI helpers
        // ==========================================================
        private void ShowError(string msg)
        {
            pnlErr.Visible = true;
            lblErr.Text = HttpUtility.HtmlEncode(msg);
        }

        private void ShowInfo(string msg)
        {
            pnlInfo.Visible = true;
            lblInfo.Text = HttpUtility.HtmlEncode(msg);
        }

        private class AccessRequest
        {
            public int RequestId;
            public string FullName;
            public string Email;
            public string Message;
            public int RoleId;
            public string Status;
        }
    }
}