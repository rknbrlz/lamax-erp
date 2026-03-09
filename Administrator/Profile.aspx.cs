using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class Profile : Page
    {
        private const int MaxBytes = 2 * 1024 * 1024; // 2MB
        private const string RelFolder = "~/Uploads/avatars/";

        private string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var username = GetUsername();
                litUser.Text = username;

                // ✅ Kalıcı kaynak: DB
                int userId = GetUserIdByUsername(username);
                string avatarUrl = (userId > 0) ? GetAvatarUrlByUserId(userId) : "";

                // DB boşsa (geçiş dönemi için) Session fallback
                if (string.IsNullOrWhiteSpace(avatarUrl))
                    avatarUrl = Convert.ToString(Session["AvatarUrl"] ?? "");

                if (!string.IsNullOrWhiteSpace(avatarUrl))
                {
                    // cache bust (tarayıcı eski resmi göstermesin)
                    imgPreview.ImageUrl = ResolveUrl(avatarUrl) + "?v=" + DateTime.UtcNow.Ticks;
                }
                else
                {
                    imgPreview.ImageUrl = ResolveUrl("~/Images/avatar-default.png");
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuAvatar.HasFile)
            {
                ShowMsg("Please choose a file.", isOk: false);
                return;
            }

            if (fuAvatar.PostedFile == null || fuAvatar.PostedFile.ContentLength <= 0)
            {
                ShowMsg("File is empty.", isOk: false);
                return;
            }

            if (fuAvatar.PostedFile.ContentLength > MaxBytes)
            {
                ShowMsg("Max file size is 2MB.", isOk: false);
                return;
            }

            var ext = Path.GetExtension(fuAvatar.FileName).ToLowerInvariant();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
            {
                ShowMsg("Only JPG / PNG / WEBP allowed.", isOk: false);
                return;
            }

            var username = GetUsername();
            int userId = GetUserIdByUsername(username);
            if (userId <= 0)
            {
                ShowMsg("User not found in dbo.Users.", isOk: false);
                return;
            }

            // klasörü garanti et
            var absFolder = Server.MapPath(RelFolder);
            if (!Directory.Exists(absFolder))
                Directory.CreateDirectory(absFolder);

            // ✅ Dosya adı: UserId bazlı (en sağlam)
            // İstersen tek dosyada overwrite: $"{userId}{ext}"
            // Ben timestamp ile bırakıyorum (istersen eskiyi de silebiliriz)
            var fileName = $"{userId}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
            var absPath = Path.Combine(absFolder, fileName);

            try
            {
                fuAvatar.SaveAs(absPath);

                var relPath = RelFolder + fileName; // ör: ~/Uploads/avatars/2_20260226101500.png

                // ✅ DB’ye yaz (kalıcı)
                UpsertAvatar(userId, relPath);

                // (opsiyonel) Session'a da yaz: anlık UI için
                Session["AvatarUrl"] = relPath;

                // preview (cache bust)
                imgPreview.ImageUrl = ResolveUrl(relPath) + "?v=" + DateTime.UtcNow.Ticks;

                ShowMsg("Profile photo updated.", isOk: true);
            }
            catch (Exception ex)
            {
                ShowMsg("Upload failed: " + ex.Message, isOk: false);
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                var username = GetUsername();
                int userId = GetUserIdByUsername(username);

                // DB’de kayıtlı avatar’ı al (dosyayı da silmek için)
                string avatarUrl = (userId > 0) ? GetAvatarUrlByUserId(userId) : "";
                if (string.IsNullOrWhiteSpace(avatarUrl))
                    avatarUrl = Convert.ToString(Session["AvatarUrl"] ?? "");

                // ✅ DB’den temizle
                if (userId > 0)
                    UpsertAvatar(userId, null);

                // Session'dan da sil
                Session["AvatarUrl"] = null;

                // dosyayı da silmek istersen:
                if (!string.IsNullOrWhiteSpace(avatarUrl))
                {
                    var abs = Server.MapPath(avatarUrl);
                    if (File.Exists(abs))
                        File.Delete(abs);
                }

                imgPreview.ImageUrl = ResolveUrl("~/Images/avatar-default.png");
                ShowMsg("Profile photo removed.", isOk: true);
            }
            catch (Exception ex)
            {
                ShowMsg("Remove failed: " + ex.Message, isOk: false);
            }
        }

        private string GetUsername()
        {
            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
                return User.Identity.Name ?? "user";
            return "user";
        }

        // =========================
        // ✅ DB HELPERS (Users + T_UserProfile)
        // =========================

        private int GetUserIdByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return 0;

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT UserId FROM dbo.Users WHERE Username=@u", con))
            {
                cmd.Parameters.AddWithValue("@u", username);
                con.Open();
                var v = cmd.ExecuteScalar();
                return (v == null) ? 0 : Convert.ToInt32(v);
            }
        }

        private string GetAvatarUrlByUserId(int userId)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("SELECT AvatarUrl FROM dbo.T_UserProfile WHERE UserId=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                con.Open();
                var v = cmd.ExecuteScalar();
                return (v == null) ? "" : Convert.ToString(v);
            }
        }

        private void UpsertAvatar(int userId, string avatarUrlOrNull)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
IF EXISTS (SELECT 1 FROM dbo.T_UserProfile WHERE UserId=@id)
BEGIN
    UPDATE dbo.T_UserProfile
        SET AvatarUrl=@a, UpdatedAt=sysdatetime()
        WHERE UserId=@id;
END
ELSE
BEGIN
    INSERT INTO dbo.T_UserProfile(UserId, AvatarUrl)
    VALUES (@id, @a);
END
", con))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                cmd.Parameters.AddWithValue("@a", (object)avatarUrlOrNull ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ShowMsg(string text, bool isOk)
        {
            lblMsg.Text = text;
            lblMsg.ForeColor = isOk ? Color.Green : Color.Red;
        }
    }
}