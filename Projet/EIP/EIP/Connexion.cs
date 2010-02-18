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

namespace EIP
{
    public static class Connexion
    {
        public static Api facebookAPI { get; set; }
        private static BrowserSession browserSession { get; set; }

        private const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a";

        private const string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        private const string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

        public static List<Account> currentAccounts { get; set; }
        public static List<Account> storageAccounts { get; set; }
        public static Account currentAccount { get; set; }

        private static IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;

        private static ListeComptes listeComptes;

        public static void Start()
        {
            browserSession = new BrowserSession(ApplicationKey);
            browserSession.LoginCompleted += LoginFacebook_LoginCompleted;
            LoadFromStorage();
        }

        private static void LoadFromStorage()
        {
            storageAccounts = (storage.Contains("Accounts") ? (List<Account>)storage["Accounts"] : new List<Account>());
            storageAccounts = (storageAccounts == null) ? new List<Account>() : storageAccounts;
            GetSession();

            if (currentAccount != null)
            {
                if (currentAccount.userID != 0)
                {
                    var theAccountID = from Account account in storageAccounts
                                       where account.userID == currentAccount.userID
                                       select account.accountID;
                    if (theAccountID.Count() > 0)
                    {
                        var theAccounts = from Account account in storageAccounts
                                          where account.accountID == theAccountID.First()
                                          select account;

                        currentAccounts = new List<Account>();
                        foreach (Account acc in theAccounts)
                        {
                            currentAccounts.Add(acc);
                        }
                    }
                }
            }
        }


        private static void GetSession()
        {
            currentAccount = (storage.Contains("CurrentAccount") ? (Account)storage["CurrentAccount"] : null);
           /* currentSessionExpires = (storage.Contains("SessionExpires") ? (bool)storage["SessionExpires"] : true);
            currentSessionKey = (storage.Contains("SessionKey") ? (string)storage["SessionKey"] : null);
            currentSessionSecret = (storage.Contains("SessionSecret") ? (string)storage["SessionSecret"] : null);
            currentUserID = (storage.Contains("UserId") ? (long)storage["UserId"] : 0);*/
        }

        public static void SetSession()
        {
            //storage["ApplicationSecret"] = _facebookAPI.Session.ApplicationSecret;
            /*currentSessionExpires = facebookAPI.Session.SessionExpires;
            storage["SessionExpires"] = currentSessionExpires;

            currentSessionKey = facebookAPI.Session.SessionKey;
            storage["SessionKey"] = currentSessionKey;

            currentSessionSecret = facebookAPI.Session.SessionSecret;
            storage["SessionSecret"] = currentSessionSecret;

            currentUserID = facebookAPI.Session.UserId;
            storage["UserId"] = currentUserID;*/
            storage["CurrentAccount"] = currentAccount;
        }

        public static void DestroySession()
        {
            /*currentSessionExpires = true;
            storage["SessionExpires"] = null;

            currentSessionKey = null;
            storage["SessionKey"] = null;

            currentSessionSecret = null;
            storage["SessionSecret"] = null;

            currentUserID = 0;
            storage["UserId"] = null;*/
            storage["CurrentAccount"] = null;
        }

        public static void AddAccount(Account.TypeAccount type, ListeComptes listes)
        {
            listeComptes = listes;
            switch (type)
            {
                case Account.TypeAccount.Facebook:
                    if (facebookAPI != null)
                    {
                        browserSession = (BrowserSession)facebookAPI.Session;
                        facebookAPI = null;
                        //facebookAPI.Session.LogoutCompleted += BrowserSession_LogoutCompleted;
                        //facebookAPI.Session.Logout();
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
                    ServiceEIP.ServiceEIPClient serviceEIP = new ServiceEIP.ServiceEIPClient();

                    serviceEIP.AuthorizeDesktopCompleted += new EventHandler<ServiceEIP.AuthorizeDesktopCompletedEventArgs>(test_AuthorizeDesktopCompleted);
                    serviceEIP.AuthorizeDesktopAsync(consumerKey, consumerSecret);
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        private static void test_AuthorizeDesktopCompleted(object sender, ServiceEIP.AuthorizeDesktopCompletedEventArgs e)
        {
            string token = e.Result;
            TwitterPin twitterPin = new TwitterPin();
            twitterPin.Show();
        }

        private static void BrowserSession_LogoutCompleted(object sender, EventArgs e)
        {
            browserSession = new BrowserSession(ApplicationKey);
            browserSession.LoginCompleted += NewAccountFacebook_LoginCompleted;
            browserSession.Login();
        }

        public static void LoadAccount(Account account)
        {
            /*var theAccount = from Account oneAccount in this.currentAccounts
                             where oneAccount.userID == account.userID
                               select account;*/
            switch (account.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    currentAccount = (AccountFacebook)account;//theAccount.First();
                    LoginToAccount();
                    break;
                case Account.TypeAccount.Twitter:
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        private static void LoginToAccount()
        {
            if (currentAccount != null)
            {
                switch (currentAccount.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        {
                            AccountFacebook myCurrentAccount = (AccountFacebook)currentAccount;
                            browserSession.LoggedIn(myCurrentAccount.sessionKey,
                                                            myCurrentAccount.sessionSecret,
                                                            Convert.ToInt32(myCurrentAccount.sessionExpires),
                                                            myCurrentAccount.userID);
                            //browserSession.Login();
                        }
                        break;
                    case Account.TypeAccount.Twitter:
                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
            }
        }

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
                long accountId;
                bool currentSessionExpires = true;
                string currentSessionKey = string.Empty;
                string currentSessionSecret = string.Empty;
                if (currentAccounts != null)
                {
                    accountId = currentAccounts[0].accountID;
                    currentSessionExpires = ((AccountFacebook)currentAccount).sessionExpires;
                    currentSessionKey = ((AccountFacebook)currentAccount).sessionKey;
                    currentSessionSecret = ((AccountFacebook)currentAccount).sessionSecret;
                }
                else
                {
                    accountId = (long)users[0].uid;
                    currentSessionExpires = facebookAPI.Session.SessionExpires;
                    currentSessionKey = facebookAPI.Session.SessionKey;
                    currentSessionSecret = facebookAPI.Session.SessionSecret;
                }

                AccountFacebook newAccount = new AccountFacebook()
                {
                    accountID = accountId,
                    typeAccount = Account.TypeAccount.Facebook,
                    userID = facebookAPI.Session.UserId,
                    name = users[0].name,
                    sessionExpires = currentSessionExpires,
                    sessionKey = currentSessionKey,
                    sessionSecret = currentSessionSecret
                };
                currentAccount = newAccount;
                SetSession();

                var theAccountID = from Account account in storageAccounts
                                   where account.userID == currentAccount.userID
                                   select account.accountID;
                if (theAccountID.Count() == 0)
                {
                    storageAccounts.Add(newAccount);
                    storage["Accounts"] = storageAccounts;
                }

                LoadFromStorage();
                listeComptes.Reload();
            }
        }

        private static void LoginFacebook_LoginCompleted(object sender, EventArgs e)
        {
            facebookAPI = new Api(browserSession);
            SetSession();
        }
    }
}
