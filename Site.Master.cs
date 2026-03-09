using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Feniks
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTopbarUser();
            }
        }

        public bool IsActive(string pathPrefix)
        {
            var url = (Request?.Url?.AbsolutePath ?? "").ToLowerInvariant();
            pathPrefix = (pathPrefix ?? "").ToLowerInvariant();
            return url.StartsWith(pathPrefix);
        }

        private void BindTopbarUser()
        {
            int? userId = null;

            // ✅ En sağlam: UserId
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int uid))
                userId = uid;

            // Alternatif: username
            string username =
                (Session["Username"] as string)
                ?? (Context?.User?.Identity?.Name);

            var u = GetUserTopbar(userId, username);

            // 🟢 FullName (RoleName)
            var fn = string.IsNullOrWhiteSpace(u.FullName) ? "User" : u.FullName;
            var rn = string.IsNullOrWhiteSpace(u.RoleName) ? "Guest" : u.RoleName;
            litFullName.Text = fn;
            litRoleName.Text = "(" + rn + ")";

            // Avatar fallback
            litAvatarFallback.Text = GetInitials(fn);

            // 🔵 AvatarUrl from T_UserProfile
            if (!string.IsNullOrWhiteSpace(u.AvatarUrl))
            {
                imgAvatar.Visible = true;
                imgAvatar.ImageUrl = ResolveAppUrl(u.AvatarUrl);
                litAvatarFallback.Visible = false;
            }
            else
            {
                imgAvatar.Visible = false;
                litAvatarFallback.Visible = true;
            }

            // Bildirim placeholder
            litNotifCount.Text = "1";
            phNotifBadge.Visible = true;

        }

        private UserTopbarDto GetUserTopbar(int? userId, string username)
        {
            var dto = new UserTopbarDto
            {
                FullName = "User",
                RoleName = "Guest",
                AvatarUrl = null
            };

            try
            {
                using (var con = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(@"
SELECT TOP 1
    u.UserId,
    u.Username,
    u.FullName,
    r.RoleName,
    up.AvatarUrl
FROM dbo.[Users] u
LEFT JOIN dbo.[Roles] r ON r.RoleId = u.RoleId
LEFT JOIN dbo.[T_UserProfile] up ON up.UserId = u.UserId
WHERE
    (@uid IS NOT NULL AND u.UserId = @uid)
 OR (@uid IS NULL AND @un IS NOT NULL AND u.Username = @un)
", con))
                {
                    cmd.Parameters.AddWithValue("@uid", (object)userId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@un", (object)username ?? DBNull.Value);

                    con.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            dto.FullName = r["FullName"] == DBNull.Value ? "User" : r["FullName"].ToString();
                            dto.RoleName = r["RoleName"] == DBNull.Value ? "Guest" : r["RoleName"].ToString();
                            dto.AvatarUrl = r["AvatarUrl"] == DBNull.Value ? null : r["AvatarUrl"].ToString();
                        }
                    }
                }
            }
            catch
            {
                // sessiz fallback
            }

            return dto;
        }

        private string ResolveAppUrl(string dbValue)
        {
            var v = (dbValue ?? "").Trim();
            if (v == "") return "";

            // "~/Uploads/..." => ResolveUrl
            if (v.StartsWith("~/"))
                return ResolveUrl(v);

            // "/Uploads/..." => "~" + v
            if (v.StartsWith("/"))
                return ResolveUrl("~" + v);

            // "Uploads/..." => "~/" + v
            return ResolveUrl("~/" + v);
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpperInvariant();
        }

        private class UserTopbarDto
        {
            public string FullName { get; set; }
            public string RoleName { get; set; }
            public string AvatarUrl { get; set; }
        }
    }
}