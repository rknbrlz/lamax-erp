using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Feniks
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private string Token => (Request.QueryString["token"] ?? "").Trim();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlErr.Visible = false;
                pnlOk.Visible = false;
                lblErr.Text = "";

                if (!IsTokenValid(Token))
                {
                    pnlInvalid.Visible = true;
                    pnlForm.Visible = false;
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            pnlErr.Visible = false;
            pnlOk.Visible = false;
            lblErr.Text = "";

            if (!IsTokenValid(Token))
            {
                pnlInvalid.Visible = true;
                pnlForm.Visible = false;
                return;
            }

            string p1 = (txtNewPass.Text ?? "").Trim();
            string p2 = (txtNewPass2.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(p1) || p1.Length < 6)
            {
                pnlErr.Visible = true;
                lblErr.Text = "Password must be at least 6 characters.";
                return;
            }
            if (p1 != p2)
            {
                pnlErr.Visible = true;
                lblErr.Text = "Passwords do not match.";
                return;
            }

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                int userId = 0;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 UserId
FROM dbo.T_PasswordReset
WHERE Token=@Token AND IsUsed=0 AND ExpiresAtUtc > GETUTCDATE()", con))
                    {
                        cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = Token;
                        object v = cmd.ExecuteScalar();
                        if (v == null)
                        {
                            pnlInvalid.Visible = true;
                            pnlForm.Visible = false;
                            return;
                        }
                        userId = Convert.ToInt32(v);
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Users SET [Password]=@P WHERE UserId=@UserId", con))
                    {
                        cmd.Parameters.Add("@P", SqlDbType.NVarChar, 255).Value = p1;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = new SqlCommand("UPDATE dbo.T_PasswordReset SET IsUsed=1, UsedAtUtc=GETUTCDATE() WHERE Token=@Token", con))
                    {
                        cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = Token;
                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }

                pnlOk.Visible = true;
            }
            catch (Exception ex)
            {
                pnlErr.Visible = true;
                lblErr.Text = ex.Message;
            }
        }

        private bool IsTokenValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.T_PasswordReset
WHERE Token=@Token AND IsUsed=0 AND ExpiresAtUtc > GETUTCDATE()", con))
            {
                cmd.Parameters.Add("@Token", SqlDbType.NVarChar, 64).Value = token;
                con.Open();
                int c = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();
                return c > 0;
            }
        }
    }
}