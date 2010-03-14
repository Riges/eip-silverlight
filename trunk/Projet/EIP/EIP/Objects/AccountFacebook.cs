using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using Facebook.Rest;
using Facebook.Session;
using EIP.ServiceEIP;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFacebookLight))]
    public class AccountFacebookLight : AccountLight
    {
        public Api facebookAPI { get; set; }
        private BrowserSession browserSession { get; set; }
        /*
        public bool sessionExpires { get; set; }
        public string sessionKey { get; set; }
        public string sessionSecret { get; set; }

        */

        public AccountFacebookLight()
        {
            this.account = new AccountFacebook();
   
        }

        public void Login()
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    browserSession = new BrowserSession(Connexion.ApplicationKey);
                    browserSession.LoginCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(browserSession_LoginCompleted);
                    browserSession.LoggedIn(((AccountFacebook)this.account).sessionKey,
                                                                ((AccountFacebook)this.account).sessionSecret,
                                                                Convert.ToInt32(((AccountFacebook)this.account).sessionExpires),
                                                                this.account.userID);
                });
        }

        void browserSession_LoginCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            facebookAPI = new Api(browserSession);
            Connexion.accounts[this.account.userID] = this;
        }

    
    }
}
