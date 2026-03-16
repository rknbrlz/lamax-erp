using Feniks.Services;
using System;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class EtsyOAuthCallback : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            try
            {
                string state = Request.QueryString["state"];
                string code = Request.QueryString["code"];
                string sessionState = Convert.ToString(Session["EtsyOAuthState"]);
                string verifier = Convert.ToString(Session["EtsyCodeVerifier"]);

                if (string.IsNullOrWhiteSpace(code))
                    throw new Exception("Authorization code gelmedi.");

                if (string.IsNullOrWhiteSpace(state) || state != sessionState)
                    throw new Exception("OAuth state doğrulaması başarısız.");

                if (string.IsNullOrWhiteSpace(verifier))
                    throw new Exception("Code verifier bulunamadı.");

                var client = new EtsyApiClient();
                var tokenJson = client.ExchangeCodeForToken(code, verifier);
                client.SaveToken(tokenJson);

                litResult.Text = "<div class='alert alert-success'>Etsy bağlantısı başarılı. Token kaydedildi.</div>";
            }
            catch (Exception ex)
            {
                litResult.Text = "<div class='alert alert-danger'>" + Server.HtmlEncode(ex.Message) + "</div>";
            }
        }
    }
}