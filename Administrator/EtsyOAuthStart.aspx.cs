using Feniks.Services;
using System;
using System.Web.UI;

namespace Feniks.Administrator
{
    public partial class EtsyOAuthStart : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var client = new EtsyApiClient();

            string state = Guid.NewGuid().ToString("N");
            string verifier = EtsyApiClient.GenerateCodeVerifier();
            string challenge = EtsyApiClient.GenerateCodeChallenge(verifier);

            Session["EtsyOAuthState"] = state;
            Session["EtsyCodeVerifier"] = verifier;

            Response.Redirect(client.BuildAuthorizeUrl(state, challenge), false);
        }
    }
}