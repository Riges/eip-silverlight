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
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Facebook.Session;
using Facebook.Rest;
using Facebook.Schema;
using Facebook.Utility;
using System.Linq;
using EIP.Views.Controls;
using System.Runtime.Serialization;
using System.Windows.Threading;
using EIP.ServiceEIP;
 


using System.Windows.Navigation;
using EIP.Views;
using System.ComponentModel;
using TweetSharp.Fluent;
using TweetSharp.Extensions;
using TweetSharp.Model;
using TweetSharp;

namespace EIP
{
    public static class Connexion
    {
        //Objets Facebook
        public static Api facebookAPI { get; set; }
        private static BrowserSession browserSession { get; set; }

        //api key Facebook
        private const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a";

        //api key Twitter
        public const string ProxyUrl = "http://localhost:4164/proxy";
        public const string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        public const string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

        //Accounts
        public static List<AccountLight> currentAccounts { get; set; }
        public static List<AccountLight> storageAccounts { get; set; }
        public static AccountLight currentAccount { get; set; }

        private static IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
       
        //Controls
        public static ListeComptes listeComptes;
        public static Dispatcher dispatcher;
        public static Frame contentFrame;

        //WCF
        public static ServiceEIP.ServiceEIPClient serviceEIP = new ServiceEIP.ServiceEIPClient();

        //Autre
        private static bool addAccount = false;
       
        /////////
   
        public static void Start()
        {
            browserSession = new BrowserSession(ApplicationKey);
            browserSession.LoginCompleted += LoginFacebook_LoginCompleted;
            LoadFromStorage();
            SetTwitterClientInfo();
        }

        private static void LoadFromStorage()
        {
            storageAccounts = new List<AccountLight>();
            foreach (string key in storage.Keys)
            {
                if (key.StartsWith("Account-"))
                {
                    storageAccounts.Add((AccountLight)storage[key]);
                }
            }

            GetSession();

            if (currentAccount != null)
            {
                if (currentAccount.account.userID != 0)
                {
                    var theAccountID = from AccountLight account in storageAccounts
                                       where account.account.userID == currentAccount.account.userID
                                       select account.account.accountID;
                    if (theAccountID.Count() > 0)
                    {
                        var theAccounts = from AccountLight account in storageAccounts
                                          where account.account.accountID == theAccountID.First()
                                          select account;

                        currentAccounts = new List<AccountLight>();
                        foreach (AccountLight acc in theAccounts)
                        {
                            currentAccounts.Add(acc);
                        }
                    }
                }
            }
        }

        private static void GetSession()
        {
            if (storage.Contains("CurrentAccount"))
            {
                if (storage["CurrentAccount"] != null)
                {
                    string curAccountKey = storage["CurrentAccount"].ToString();
                    if (storage.Contains(curAccountKey))
                        currentAccount = (AccountLight)storage[curAccountKey];
                }
            }
        }

        private static void SetSession()
        {
            //storage["CurrentAccount"] = currentAccount;
            storage["CurrentAccount"] = "Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID;
        }

        public static void SaveAccount()
        {
            switch (currentAccount.account.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    storage["Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID] = (AccountFacebookLight)currentAccount;
                    break;
                case Account.TypeAccount.Twitter:
                    storage["Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID] = (AccountTwitterLight)currentAccount;
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        private static void DestroySession()
        {
            storage["CurrentAccount"] = null;
            currentAccounts = null;
        }

        public static void Login(Account.TypeAccount type, string pseudo, string password)
        {
            addAccount = false;
            switch (type)
            {
                case Account.TypeAccount.Facebook:

                    if (facebookAPI != null)
                    {
                        browserSession = (BrowserSession)facebookAPI.Session;
                        facebookAPI = null;
                        browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        browserSession.Logout();
                    }
                    else
                    {
                        browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        browserSession.Logout();
                    }
                   

                    break;
                case Account.TypeAccount.Twitter:
                     var requestToken = FluentTwitter.CreateRequest()
                        .Configuration.UseTransparentProxy(ProxyUrl)
                        .AuthenticateAs(pseudo, password)
                        .CallbackTo(TwitterAuthenticateAsCompleted);

                    requestToken.RequestAsync();

                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        public static void AddAccount(Account.TypeAccount type)
        {
            /*listeComptes = listes;
            contentFrame = frame;*/
            switch (type)
            {
                case Account.TypeAccount.Facebook:
                    if (facebookAPI != null)
                    {
                        browserSession = (BrowserSession)facebookAPI.Session;
                        facebookAPI = null;
                        browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        browserSession.Logout();
                    }
                    else
                    {
                        browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        browserSession.Logout();
                    }

                    break;
                case Account.TypeAccount.Twitter:

                    //serviceEIP.AuthorizeDesktopCompleted += new EventHandler<ServiceEIP.AuthorizeDesktopCompletedEventArgs>(test_AuthorizeDesktopCompleted);
                    //serviceEIP.AuthorizeDesktopAsync(consumerKey, consumerSecret);


                    //SetTwitterClientInfo();

                   // OAuthBus.RequestTokenRetrieved += OAuthBus_RequestTokenRetrieved;
                    //OAuthBus.AccessTokenRetrieved += OAuthBus_AccessTokenRetrieved;

                     var requestToken = FluentTwitter.CreateRequest()
                        .Configuration.UseTransparentProxy(ProxyUrl)
                        .Authentication.GetRequestToken()
                        .CallbackTo(TwitterRequestTokenReceived);

                    requestToken.RequestAsync();



                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        private static void TwitterRequestTokenReceived(object sender, TwitterResult result)
        {
            var tokenRes = result.AsToken();

            var authorizeUrl = FluentTwitter.CreateRequest()
                .Authentication.GetAuthorizationUrl(tokenRes.Token);
            var uri = new Uri(authorizeUrl);

            dispatcher.BeginInvoke(() =>
            {
                string token = tokenRes.Token;
                AccountTwitterLight accountTwitter = new AccountTwitterLight();
                accountTwitter.token = token;
                TwitterPin twitterPin = new TwitterPin(accountTwitter, uri);
                twitterPin.Show();
            });

        }

        private static void TwitterAuthenticateAsCompleted(object sender, TwitterResult result)
        {
            var user = result.AsUser();

            serviceEIP.GetAccountByUserIDCompleted += new EventHandler<GetAccountByUserIDCompletedEventArgs>(serviceEIP_GetAccountByUserIDCompleted);
            serviceEIP.GetAccountByUserIDAsync((long)user.Id);

        }

        //private static void test_AuthorizeDesktopCompleted(object sender, ServiceEIP.AuthorizeDesktopCompletedEventArgs e)
        private static void Twitter_AuthorizeDesktop(object sender, TwitterResult result)
        {
            var tokenRes = result.AsToken();
            string token = tokenRes.Token;// e.Result;
            AccountTwitterLight accountTwitter = new AccountTwitterLight();
            accountTwitter.token = token;
            TwitterPin twitterPin = new TwitterPin(accountTwitter, new Uri(""));
            twitterPin.Show();
        }

        private static void SetTwitterClientInfo()
        {
            var clientInfo = new TwitterClientInfo
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };

            FluentBase<TwitterResult>.SetClientInfo(clientInfo);
        }

        public static void AddTwitterAccount(AccountTwitterLight accountTwitter)//, Dispatcher dispatch
        {
            var accessToken = FluentTwitter.CreateRequest()
                .Configuration.UseTransparentProxy(ProxyUrl)
                .Authentication.GetAccessToken(accountTwitter.token, accountTwitter.pin)
                .CallbackTo(TwitterAccessTokenReceived);

             accessToken.RequestAsync();
             

            //dispatcher = dispatch;
            //serviceEIP.GetAccessTokenCompleted += new EventHandler<ServiceEIP.GetAccessTokenCompletedEventArgs>(serviceEIP_GetAccessTokenCompleted);
            //serviceEIP.GetAccessTokenAsync(consumerKey, consumerSecret, accountTwitter.token, accountTwitter.pin);
        }

        //static void serviceEIP_GetAccessTokenCompleted(object sender, ServiceEIP.GetAccessTokenCompletedEventArgs e)
        private static void TwitterAccessTokenReceived(object sender, TwitterResult result)
        {
            var token = result.AsToken();

            if (token == null)
            {
                var error = result.AsError();
                if (error != null)
                {
                    throw new Exception(error.ErrorMessage);
                }
            }
            else
            {
                AccountTwitterLight accountTwitter = new AccountTwitterLight();

                accountTwitter.account.typeAccount = Account.TypeAccount.Twitter;
                accountTwitter.token = token.Token;
                accountTwitter.tokenSecret = token.TokenSecret;
                accountTwitter.account.name = token.ScreenName;
                accountTwitter.account.userID = Convert.ToInt64(token.UserId);

                if (currentAccounts != null)
                {
                    accountTwitter.account.accountID = currentAccounts[0].account.accountID;
                }
                else
                {
                    accountTwitter.account.accountID = Convert.ToInt64(token.UserId);// (long)tmp.userID;
                }
                currentAccount = accountTwitter;
                SetSession();

                //Ajouter la verif sur cpt de type Twitter
                var theAccountID = from Account account in storageAccounts
                                   where account.userID == currentAccount.account.userID
                                   && account.typeAccount == Account.TypeAccount.Twitter
                                   select account.accountID;


                if (theAccountID.Count() == 0)
                {
                    storage["Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID] = (AccountTwitterLight)currentAccount;
                }

                LoadFromStorage();
                listeComptes.Reload();
                LoginToAccount();
            }
        }

        private static void BrowserSession_LogoutCompleted(object sender, EventArgs e)
        {
            browserSession = new BrowserSession(ApplicationKey);
            browserSession.LoginCompleted += NewAccountFacebook_LoginCompleted;
            browserSession.Login();
        }

        public static void LoadAccount(AccountLight account)
        {
            /*var theAccount = from Account oneAccount in this.currentAccounts
                             where oneAccount.userID == account.userID
                               select account;*/
            //dispatcher = dispatch;
            //contentFrame = frame;
            switch (account.account.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    currentAccount = (AccountFacebookLight)account;//theAccount.First();
                    LoginToAccount();
                    break;
                case Account.TypeAccount.Twitter:
                    currentAccount = (AccountTwitterLight)account;//theAccount.First();
                    LoginToAccount();
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        public static void LoginToAccount()
        {
            if (currentAccount != null)
            {
                switch (currentAccount.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        {
                            AccountFacebookLight myCurrentAccount = (AccountFacebookLight)currentAccount;
                            browserSession.LoggedIn(((AccountFacebook)myCurrentAccount.account).sessionKey,
                                                            ((AccountFacebook)myCurrentAccount.account).sessionSecret,
                                                            Convert.ToInt32(((AccountFacebook)myCurrentAccount.account).sessionExpires),
                                                            myCurrentAccount.account.userID);
                            //browserSession.Login();
                        }
                        break;
                    case Account.TypeAccount.Twitter:

                        //serviceEIP.TwitterGetUserInfoCompleted += new EventHandler<TwitterGetUserInfoCompletedEventArgs>(serviceEIP_TwitterGetUserInfoCompleted);
                        //serviceEIP.TwitterGetUserInfoAsync(consumerKey, consumerSecret, ((AccountTwitter)currentAccount).token, ((AccountTwitter)currentAccount).tokenSecret, ((AccountTwitter)currentAccount).userID);

                        ((AccountTwitterLight)currentAccount).LoadHomeStatuses();
                        //serviceEIP.TwitterGetHomeStatusesCompleted += new EventHandler<TwitterGetHomeStatusesCompletedEventArgs>(serviceEIP_TwitterGetHomeStatusesCompleted);
                        //serviceEIP.TwitterGetHomeStatusesAsync(consumerKey, consumerSecret, ((AccountTwitter)currentAccount).token, ((AccountTwitter)currentAccount).tokenSecret);

                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }

                dispatcher.BeginInvoke(() =>
                    {
                        MessageBox msgBox = new MessageBox(null, "Vous êtes connecté sur le compte " + currentAccount.account.typeAccount.ToString() + " : " + currentAccount.account.name);
                        msgBox.Show();
                    });

                if (contentFrame != null)
                    contentFrame.Navigate(new Uri("/Home?time=" + DateTime.Now.Ticks, UriKind.Relative));
                if (listeComptes != null)
                    listeComptes.Reload();

            }

        }      

        /*private static void serviceEIP_TwitterGetUserInfoCompleted(object sender, ServiceEIP.TwitterGetUserInfoCompletedEventArgs e)
        {
            if (currentAccount.typeAccount == Account.TypeAccount.Twitter)
                ((AccountTwitter)currentAccount).user = e.Result;
        }*/

        private static void NewAccountFacebook_LoginCompleted(object sender, EventArgs e)
        {
            //LoginFacebook_LoginCompleted(sender, e);
            if (facebookAPI == null)
            {
                facebookAPI = new Api(browserSession);
                facebookAPI.Users.GetInfoAsync(browserSession.UserId, new Users.GetInfoCallback(GetUserFacebook_Completed), new object());
            }
        }

        private static void GetUserFacebook_Completed(IList<user> users, object o, FacebookException ex)
        {
            if (users.Count > 0)
            {
                if (addAccount)
                {
                    long accountId;
                    bool currentSessionExpires = true;
                    string currentSessionKey = string.Empty;
                    string currentSessionSecret = string.Empty;
                    if (currentAccounts != null)
                    {
                        accountId = currentAccounts[0].account.accountID;
                        //currentSessionExpires = ((AccountFacebook)currentAccount).sessionExpires;
                        //currentSessionKey = ((AccountFacebook)currentAccount).sessionKey;
                        //currentSessionSecret = ((AccountFacebook)currentAccount).sessionSecret;
                    }
                    else
                    {
                        accountId = (long)users[0].uid;

                    }
                    currentSessionExpires = facebookAPI.Session.SessionExpires;
                    currentSessionKey = facebookAPI.Session.SessionKey;
                    currentSessionSecret = facebookAPI.Session.SessionSecret;

                    AccountFacebookLight newAccount = new AccountFacebookLight();                                      
                    newAccount.account.accountID = accountId;
                    newAccount.account.typeAccount = Account.TypeAccount.Facebook;
                    newAccount.account.userID = facebookAPI.Session.UserId;
                    newAccount.account.name = users[0].name;
                    ((AccountFacebook)newAccount.account).sessionExpires = currentSessionExpires;
                    ((AccountFacebook)newAccount.account).sessionKey = currentSessionKey;
                    ((AccountFacebook)newAccount.account).sessionSecret = currentSessionSecret;
                    
                    currentAccount = newAccount;
                    SetSession();

                    //Ajouter la verif sur cpt de type Facebook
                    var theAccountID = from Account account in storageAccounts
                                       where account.userID == currentAccount.account.userID
                                       select account.accountID;
                    if (theAccountID.Count() == 0)
                    {
                        storage["Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID] = (AccountFacebookLight)currentAccount;
                    }

                    LoadFromStorage();
                    listeComptes.Reload();
                    LoginToAccount();
                }
                else
                {
                    serviceEIP.GetAccountByUserIDCompleted += new EventHandler<GetAccountByUserIDCompletedEventArgs>(serviceEIP_GetAccountByUserIDCompleted);
                    serviceEIP.GetAccountByUserIDAsync((long)users[0].uid);
                }
            }
        }

        static void serviceEIP_GetAccountByUserIDCompleted(object sender, GetAccountByUserIDCompletedEventArgs e)
        {
            switch (e.Result.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    currentAccount = new AccountFacebookLight();
                    break;
                case Account.TypeAccount.Twitter:
                    currentAccount = new AccountTwitterLight();
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
            
            if (e.Result != null)
            {
                currentAccount.account = e.Result;
                LoginToAccount();
                SaveAccount();
                LoadFromStorage();
            }
            else
            {
                dispatcher.BeginInvoke(() =>
                {
                    MessageBox msg = new MessageBox("Erreur de connexion", "Vous devez ajouter un compte avant de pouvoir vous connecter dessus !");
                    msg.Show();
                });
            }
        }

        private static void LoginFacebook_LoginCompleted(object sender, EventArgs e)
        {
            facebookAPI = new Api(browserSession);
            SetSession();
        }

        public static void Deconnexion()
        {
            currentAccount = null;
            DestroySession();
        }

    }
}
