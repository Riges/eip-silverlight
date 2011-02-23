using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facebook;
using System.Configuration;

namespace EIP.Web
{
    public partial class LoginFB : System.Web.UI.Page
    {
        protected string AccessToken { get; set; }
        protected string ErrorDescription { get; set; }

        private void Page_Load(object sender, EventArgs e)
        {
            var url = HttpContext.Current.Request.Url;
            FacebookOAuthResult authResult;

            if (FacebookOAuthResult.TryParse(url, out authResult))
            {
                if (authResult.IsSuccess)
                {
                    var oauth = new FacebookOAuthClient
                    {
                        ClientId = ConfigurationManager.AppSettings["FBAppID"],
                        ClientSecret = ConfigurationManager.AppSettings["FBSecretKey"],
                        RedirectUri = new Uri("http://localhost:4164/LoginFB.aspx")
                    };
                    
                    var result = (IDictionary<string, object>)oauth.ExchangeCodeForAccessToken(authResult.Code, null);
                    this.AccessToken = result["access_token"].ToString();
                }
                else
                {
                    this.ErrorDescription = authResult.ErrorDescription;
                }
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
            else
            {
                Response.Redirect("~/");
            }
        }
    }
}