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
        public const string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        public const string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

        //Accounts
        public static List<Account> currentAccounts { get; set; }
        public static List<Account> storageAccounts { get; set; }
        public static Account currentAccount { get; set; }

        private static IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
       
        //Controls
        public static ListeComptes listeComptes;
        private static Dispatcher dispatcher;
        public static Frame contentFrame;

        //WCF
        public static ServiceEIP.ServiceEIPClient serviceEIP = new ServiceEIP.ServiceEIPClient();

        public static void Start()
        {
            browserSession = new BrowserSession(ApplicationKey);
            browserSession.LoginCompleted += LoginFacebook_LoginCompleted;
            LoadFromStorage();
        }

        private static void LoadFromStorage()
        {
            storageAccounts = new List<Account>();
            foreach (string key in storage.Keys)
            {
                if (key.StartsWith("Account-"))
                {
                    storageAccounts.Add((Account)storage[key]);
                }
            }

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
            if (storage.Contains("CurrentAccount"))
            {
                if (storage["CurrentAccount"] != null)
                {
                    string curAccountKey = storage["CurrentAccount"].ToString();
                    if (storage.Contains(curAccountKey))
                        currentAccount = (Account)storage[curAccountKey];
                }
            }
        }

        private static void SetSession()
        {
            //storage["CurrentAccount"] = currentAccount;
            storage["CurrentAccount"] = "Account-" + currentAccount.typeAccount.ToString() + "-" + currentAccount.userID;
        }

        public static void SaveAccount()
        {
            switch (currentAccount.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    storage["Account-" + currentAccount.typeAccount.ToString() + "-" + currentAccount.userID] = (AccountFacebook)currentAccount;
                    break;
                case Account.TypeAccount.Twitter:
                    storage["Account-" + currentAccount.typeAccount.ToString() + "-" + currentAccount.userID] = (AccountTwitter)currentAccount;
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
            AccountTwitter accountTwitter = new AccountTwitter();
            accountTwitter.token = token;
            TwitterPin twitterPin = new TwitterPin(accountTwitter);
            twitterPin.Show();
        }

        public static void AddTwitterAccount(AccountTwitter accountTwitter, Dispatcher dispatch)
        {


            dispatcher = dispatch;
            serviceEIP.GetAccessTokenCompleted += new EventHandler<ServiceEIP.GetAccessTokenCompletedEventArgs>(serviceEIP_GetAccessTokenCompleted);
            serviceEIP.GetAccessTokenAsync(consumerKey, consumerSecret, accountTwitter.token, accountTwitter.pin);
        }

        static void serviceEIP_GetAccessTokenCompleted(object sender, ServiceEIP.GetAccessTokenCompletedEventArgs e)
        {
            /*
            MessageBox msgBox = new MessageBox("", "token : " + e.Result.token 
                + " - secret : " + e.Result.tokenSecret
                + " - name : " + e.Result.name 
                + " - userId : " + e.Result.userID);
            msgBox.Show();*/
            
            
            EIP.ServiceEIP.AccountTwitter tmp = e.Result;
            currentAccount = new AccountTwitter();
            currentAccount.name = tmp.name;
            currentAccount.typeAccount = Account.TypeAccount.Twitter;
            currentAccount.userID = tmp.userID;
            ((AccountTwitter)currentAccount).pin = tmp.pin;
            ((AccountTwitter)currentAccount).token = tmp.token;
            ((AccountTwitter)currentAccount).tokenSecret = tmp.tokenSecret;

            
            if (currentAccounts != null)
            {
                currentAccount.accountID = currentAccounts[0].accountID;
            }
            else
            {
                currentAccount.accountID = (long)tmp.userID;
            }

            SetSession();
            
            //Ajouter la verif sur cpt de type Twitter
            var theAccountID = from Account account in storageAccounts
                               where account.userID == currentAccount.userID
                               select account.accountID;

            
            if (theAccountID.Count() == 0)
            {
                storage["Account-" + currentAccount.typeAccount.ToString() + "-" + currentAccount.userID] = (AccountTwitter)currentAccount;
            }
           
            LoadFromStorage();
            listeComptes.Reload();
            LoginToAccount();
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
            //dispatcher = dispatch;
            //contentFrame = frame;
            switch (account.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    currentAccount = (AccountFacebook)account;//theAccount.First();
                    LoginToAccount();
                    break;
                case Account.TypeAccount.Twitter:
                    currentAccount = (AccountTwitter)account;//theAccount.First();
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

                        serviceEIP.TwitterGetUserInfoCompleted += new EventHandler<TwitterGetUserInfoCompletedEventArgs>(serviceEIP_TwitterGetUserInfoCompleted);
                        serviceEIP.TwitterGetUserInfoAsync(consumerKey, consumerSecret, ((AccountTwitter)currentAccount).token, ((AccountTwitter)currentAccount).tokenSecret, ((AccountTwitter)currentAccount).userID);

                        ((AccountTwitter)currentAccount).LoadHomeStatuses();
                        //serviceEIP.TwitterGetHomeStatusesCompleted += new EventHandler<TwitterGetHomeStatusesCompletedEventArgs>(serviceEIP_TwitterGetHomeStatusesCompleted);
                        //serviceEIP.TwitterGetHomeStatusesAsync(consumerKey, consumerSecret, ((AccountTwitter)currentAccount).token, ((AccountTwitter)currentAccount).tokenSecret);

                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }


                MessageBox msgBox = new MessageBox(null, "Vous êtes connecté sur le compte " + currentAccount.typeAccount.ToString() + " : " + currentAccount.name);
                msgBox.Show();

                if (contentFrame != null)
                    contentFrame.Navigate(new Uri("/Home?time=" + DateTime.Now.Ticks, UriKind.Relative));
                if (listeComptes != null)
                    listeComptes.Reload();

            }

        }      

        private static void serviceEIP_TwitterGetUserInfoCompleted(object sender, ServiceEIP.TwitterGetUserInfoCompletedEventArgs e)
        {
            if (currentAccount.typeAccount == Account.TypeAccount.Twitter)
                ((AccountTwitter)currentAccount).user = e.Result;
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

                //Ajouter la verif sur cpt de type Facebook
                var theAccountID = from Account account in storageAccounts
                                   where account.userID == currentAccount.userID
                                   select account.accountID;
                if (theAccountID.Count() == 0)
                {
                    storage["Account-" + currentAccount.typeAccount.ToString() + "-" + currentAccount.userID] = (AccountFacebook)currentAccount;
                }

                LoadFromStorage();
                listeComptes.Reload();
                LoginToAccount();
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
