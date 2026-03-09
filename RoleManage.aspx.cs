using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Feniks
{
    public partial class RoleManage : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindRoles();
        }

        private void BindRoles()
        {
            using (var con = new SqlConnection(ConnStr))
            using (var da = new SqlDataAdapter("SELECT RoleId,RoleName,Description,IsActive FROM dbo.T_Role ORDER BY RoleId", con))
            {
                var dt = new DataTable();
                da.Fill(dt);
                gvRoles.DataSource = dt;
                gvRoles.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            var name = (txtRoleName.Text ?? "").Trim();
            var desc = (txtDesc.Text ?? "").Trim();

            if (name.Length < 2)
            {
                ShowMsg("Role name required.", "alert-danger");
                return;
            }

            try
            {
                using (var con = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.T_Role(RoleName, Description, IsActive)
VALUES(@n, @d, 1);", con))
                {
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@d", (object)desc ?? DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                txtRoleName.Text = "";
                txtDesc.Text = "";
                ShowMsg("Role added.", "alert-success");
                BindRoles();
            }
            catch (SqlException ex)
            {
                ShowMsg(ex.Message, "alert-danger");
            }
        }

        protected void gvRoles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "del") return;

            int roleId = Convert.ToInt32(e.CommandArgument);

            try
            {
                // Rol kullanılıyor mu?
                using (var con = new SqlConnection(ConnStr))
                {
                    con.Open();

                    using (var chk = new SqlCommand("SELECT COUNT(*) FROM dbo.Users WHERE RoleId=@r", con))
                    {
                        chk.Parameters.AddWithValue("@r", roleId);
                        int used = Convert.ToInt32(chk.ExecuteScalar());
                        if (used > 0)
                        {
                            ShowMsg("Role is used by users. You can deactivate instead of delete.", "alert-warning");
                            return;
                        }
                    }

                    using (var cmd = new SqlCommand("DELETE FROM dbo.T_Role WHERE RoleId=@r", con))
                    {
                        cmd.Parameters.AddWithValue("@r", roleId);
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowMsg("Role deleted.", "alert-success");
                BindRoles();
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, "alert-danger");
            }
        }

        private void ShowMsg(string text, string css)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "alert " + css;
            pnlMsg.Controls.Clear();
            pnlMsg.Controls.Add(new Literal { Text = "<strong>" + Server.HtmlEncode(text) + "</strong>" });
        }
    }
}