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
using EIP.Views.Child;



namespace EIP
{
    public static class Connexion
    {
        //Objets Facebook
        public static Api facebookAPI { get; set; }
        private static BrowserSession browserSession { get; set; }

        //api key Facebook
        public const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a";

        //api key Twitter
        public const string ProxyUrl = "http://localhost:4164/proxy";
        public const string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        public const string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

        //Accounts
        //public static List<AccountLight> accounts { get; set; }
        public static Dictionary<long, AccountLight> accounts { get; set; }
        //public static List<AccountLight> currentAccounts { get; set; }
        public static List<AccountLight> storageAccounts { get; set; }
        //public static AccountLight currentAccount { get; set; }

        public static IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
       
        //Controls
        public static ListeComptes listeComptes;
        public static Dispatcher dispatcher;
        public static Frame contentFrame;
        public static NavigationContext navigationContext;
        public static NavigationService navigationService;  

        //WCF
        public static ServiceEIP.ServiceEIPClient serviceEIP;

        //Autre
        private static bool addAccount = false;
        private static Loading loadingChild;
       
        /////////
   
        public static void Start()
        {
            //browserSession = new BrowserSession(ApplicationKey);
            //browserSession.LoginCompleted += LoginFacebook_LoginCompleted;
            accounts = new Dictionary<long, AccountLight>();
            serviceEIP = new ServiceEIP.ServiceEIPClient();
            loadingChild = new Loading();
            //LoadFromStorage();
            SetTwitterClientInfo();
            GetSession();
        }

        public static void StartDisplay()
        {
            listeComptes.Reload();
        }

        public static void Loading(bool isLoading)
        {
            dispatcher.BeginInvoke(() =>
                {
                    if (isLoading)
                        loadingChild.Show();
                    else
                        loadingChild.Close();
                });

        }

        /* LoadFromStorage
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

                        accounts = new Dictionary<long, AccountLight>();
                        foreach (AccountLight acc in theAccounts)
                        {
                            //accounts.Add(acc);
                        }
                    }
                }
            }

        }
         * */

        private static void GetSession()
        {
            /*
            if (storage.Contains("CurrentAccount"))
            {
                if (storage["CurrentAccount"] != null)
                {
                    string curAccountKey = storage["CurrentAccount"].ToString();
                    if (storage.Contains(curAccountKey))
                        currentAccount = (AccountLight)storage[curAccountKey];
                }
            }*/
            if (storage.Contains("groupID"))
            {
                if (storage["groupID"] != null && (storage["groupID"].ToString() != "0"))
                {
                    /*
                    string curAccountKey = storage["groupID"].ToString();
                    if (storage.Contains(curAccountKey))
                        currentAccount = (AccountLight)storage[curAccountKey];
                     */
                    serviceEIP.GetAccountsByGroupIDCompleted += new EventHandler<GetAccountsByGroupIDCompletedEventArgs>(serviceEIP_GetAccountsByGroupIDCompleted);
                    serviceEIP.GetAccountsByGroupIDAsync(Convert.ToInt64(storage["groupID"].ToString()));
                    //serviceEIP.GetAccountsAsync(storage["groupID"]
                    if (listeComptes != null)
                        listeComptes.Reload();
                }
            }
            
        }

        private static void SetSession(long groupID)
        {
            storage["groupID"] = groupID.ToString();
            //storage["CurrentAccount"] = currentAccount;
            //storage["CurrentAccount"] = "Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID;
        }

        public static void SaveAccount(AccountLight accountToSave)
        {
            switch (accountToSave.account.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    storage["Account-" + accountToSave.account.typeAccount.ToString() + "-" + accountToSave.account.userID] = accountToSave;
                    break;
                case Account.TypeAccount.Twitter:
                    storage["Account-" + accountToSave.account.typeAccount.ToString() + "-" + accountToSave.account.userID] = accountToSave;
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
            if (addAccount)
                serviceEIP.AddAccountAsync(accountToSave.account);
            else
                serviceEIP.SaveAccountAsync(accountToSave.account);
            
        }

        static void serviceEIP_GetAccountsByGroupIDCompleted(object sender, GetAccountsByGroupIDCompletedEventArgs e)
        {
            if (e.Result != null && e.Error == null)
            {
                foreach (Account oneAccount in e.Result)
                {
                    switch (oneAccount.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            AccountFacebookLight newAccountFacebook = new AccountFacebookLight();
                            newAccountFacebook.account = oneAccount;
                            accounts[newAccountFacebook.account.userID] = newAccountFacebook;
                            newAccountFacebook.Login();
                            break;
                        case Account.TypeAccount.Twitter:
                            AccountTwitterLight newAccountTwitter = new AccountTwitterLight();
                            newAccountTwitter.account = oneAccount;
                            //accounts.Add(newAccountTwitter);
                            accounts[newAccountTwitter.account.userID] = newAccountTwitter;
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;
                    }
                }
                listeComptes.Reload();
                Connexion.contentFrame.Navigate(new Uri("/Intro", UriKind.Relative));
            }
        }



        private static void DestroySession()
        {
            //storage["CurrentAccount"] = null;
            //currentAccounts = null;
            accounts = null;
            storage["groupID"] = 0;
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
                        //browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        //browserSession.Logout();

                        BrowserSession_LogoutCompleted(null, null);
                    }
                   

                    break;
                case Account.TypeAccount.Twitter:
                     /*var requestToken = FluentTwitter.CreateRequest()
                        .Configuration.UseTransparentProxy(ProxyUrl)
                        .AuthenticateAs(pseudo, password)
                        .Account()
                         .VerifyCredentials()
                         .AsXml()
                        .CallbackTo(TwitterAuthenticateAsCompleted);
                    
                    requestToken.RequestAsync();
                    */
                    serviceEIP.GetAccountsByTwitterCompleted += new EventHandler<GetAccountsByTwitterCompletedEventArgs>(serviceEIP_GetAccountsByTwitterCompleted);
                    serviceEIP.GetAccountsByTwitterAsync(pseudo, password);
                    
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
            addAccount = true;
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
                        /*
                        browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        browserSession.Logout();
                        */
                        BrowserSession_LogoutCompleted(null, null);
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
                ((AccountTwitter)accountTwitter.account).token = token;
                TwitterPin twitterPin = new TwitterPin(accountTwitter, uri);
                twitterPin.Show();
            });

        }

        private static void TwitterAuthenticateAsCompleted(object sender, TwitterResult result)
        {
            var user = result.AsUser();
            var statuses = result.AsStatuses();

             foreach (var status in statuses)
                {
                    string str = status.User.ScreenName;
                }

            serviceEIP.GetAccountsByUserIDCompleted +=new EventHandler<GetAccountsByUserIDCompletedEventArgs>(serviceEIP_GetAccountsByUserIDCompleted);
            serviceEIP.GetAccountsByUserIDAsync((long)user.Id);

        }

        //private static void test_AuthorizeDesktopCompleted(object sender, ServiceEIP.AuthorizeDesktopCompletedEventArgs e)
        private static void Twitter_AuthorizeDesktop(object sender, TwitterResult result)
        {
            var tokenRes = result.AsToken();
            string token = tokenRes.Token;// e.Result;
            AccountTwitterLight accountTwitter = new AccountTwitterLight();
            ((AccountTwitter)accountTwitter.account).token = token;
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
                .Authentication.GetAccessToken(((AccountTwitter)accountTwitter.account).token, accountTwitter.pin)
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
                accountTwitter.account.name = token.ScreenName;
                accountTwitter.account.userID = Convert.ToInt64(token.UserId);
                ((AccountTwitter)accountTwitter.account).token = token.Token;
                ((AccountTwitter)accountTwitter.account).tokenSecret = token.TokenSecret;


                if (accounts.Count > 0)
                {
                    accountTwitter.account.groupID = accounts[0].account.groupID;
                }
                else
                {
                    accountTwitter.account.groupID = Convert.ToInt64(token.UserId);// (long)tmp.userID;
                }

                SetSession(accountTwitter.account.groupID);
                serviceEIP.AddAccountCompleted += new EventHandler<AddAccountCompletedEventArgs>(serviceEIP_AddAccountCompleted);
                serviceEIP.AddAccountAsync(accountTwitter.account);

                
                
                /*SaveAccount(accountTwitter);
                LoadFromStorage();
                listeComptes.Reload();
                */
                //currentAccount = accountTwitter;
                //SetSession();

                //Ajouter la verif sur cpt de type Twitter
                /*var theAccountID = from Account account in storageAccounts
                                   where account.userID == currentAccount.account.userID
                                   && account.typeAccount == Account.TypeAccount.Twitter
                                   select account.accountID;
                */
                
                //serviceEIP.GetAccountByUserIDCompleted +=new EventHandler<GetAccountByUserIDCompletedEventArgs>(serviceEIP_GetAccountByUserIDCompleted);
                //serviceEIP.GetAccountByUserIDAsync(accountTwitter.account.userID);

                /*if (theAccountID.Count() == 0)
                {
                    storage["Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID] = (AccountTwitterLight)currentAccount;
                }*/
                /*
                LoadFromStorage();
                listeComptes.Reload();*/
                //LoginToAccount();
            }
        }

        private static void BrowserSession_LogoutCompleted(object sender, EventArgs e)
        {
            browserSession = new BrowserSession(ApplicationKey);
            browserSession.LoginCompleted += NewAccountFacebook_LoginCompleted;
            browserSession.Login();
        }

        /* LoadAccount
        public static void LoadAccount(AccountLight account)
        {
            //var theAccount = from Account oneAccount in this.currentAccounts
             //                where oneAccount.userID == account.userID
               //                select account;
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
    */

        /* LoginToAccount
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

                        ((AccountTwitterLight)currentAccount).LoadHomeStatuses(null);
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
        */

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

               /* List<long> list = new List<long>();
                list.Add(browserSession.UserId);
                */
               // facebookAPI.Users.GetInfoAsync(list, new Users.GetInfoCallback(GetUserFacebook_Completed), null);
                facebookAPI.Users.GetInfoAsync(browserSession.UserId, new Users.GetInfoCallback(GetUserFacebook_Completed), null);
            }
        }

        private static void GetUserFacebook_Completed(IList<user> users, object o, FacebookException ex)
        {
            if (users != null && users.Count > 0)
            {
                if (addAccount)
                {
                    AccountFacebookLight newAccount = new AccountFacebookLight();
                    if (accounts.Count > 0)
                        newAccount.account.groupID = accounts.First().Value.account.groupID;
                    else
                        newAccount.account.groupID = (long)users[0].uid;

                    newAccount.account.typeAccount = Account.TypeAccount.Facebook;
                    newAccount.account.userID = facebookAPI.Session.UserId;
                    newAccount.account.name = users[0].name;
                    ((AccountFacebook)newAccount.account).sessionExpires = facebookAPI.Session.SessionExpires;
                    ((AccountFacebook)newAccount.account).sessionKey = facebookAPI.Session.SessionKey;
                    ((AccountFacebook)newAccount.account).sessionSecret = facebookAPI.Session.SessionSecret;
                    

                    //currentAccount = newAccount;
                    SetSession(newAccount.account.groupID);

                    //Ajouter la verif sur cpt de type Facebook
                    /*var theAccountID = from Account account in storageAccounts
                                       where account.userID == currentAccount.account.userID
                                       select account.accountID;
                    */
                    serviceEIP.AddAccountCompleted += new EventHandler<AddAccountCompletedEventArgs>(serviceEIP_AddAccountCompleted);
                    serviceEIP.AddAccountAsync(newAccount.account);

       

                }
                else
                {
                    serviceEIP.GetAccountsByUserIDCompleted += new EventHandler<GetAccountsByUserIDCompletedEventArgs>(serviceEIP_GetAccountsByUserIDCompleted);
                    serviceEIP.GetAccountsByUserIDAsync((long)users[0].uid);
                    //serviceEIP.GetAccountByUserIDCompleted += new EventHandler<GetAccountByUserIDCompletedEventArgs>(serviceEIP_GetAccountByUserIDCompleted);
                    //serviceEIP.GetAccountByUserIDAsync((long)users[0].uid);
                }
            }
        }

        static void serviceEIP_GetAccountsByTwitterCompleted(object sender, GetAccountsByTwitterCompletedEventArgs e)
        {
            LoadAccountsFromDB(e.Result);
        }

        static void serviceEIP_GetAccountsByUserIDCompleted(object sender, GetAccountsByUserIDCompletedEventArgs e)
        {
            if (e.Error != null)
                LoadAccountsFromDB(e.Result);
        }

        private static void LoadAccountsFromDB(List<Account> result)
        {
            if (result != null)
            {
                accounts = new Dictionary<long, AccountLight>();
                foreach (Account oneAccount in result)
                {
                    switch (oneAccount.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            AccountFacebookLight newAccountFacebook = new AccountFacebookLight();
                            newAccountFacebook.account = oneAccount;
                            //accounts.Add(newAccountFacebook);

                            newAccountFacebook.Login();

                            accounts[newAccountFacebook.account.userID] = newAccountFacebook;
                            break;
                        case Account.TypeAccount.Twitter:
                            AccountTwitterLight newAccountTwitter = new AccountTwitterLight();
                            newAccountTwitter.account = oneAccount;
                            //accounts.Add(newAccountTwitter);
                            accounts[newAccountTwitter.account.userID] = newAccountTwitter;
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;
                    }
                    SetSession(oneAccount.groupID);
                }
                
                //currentAccount.account = e.Result;
                //LoginToAccount();

                //LoadFromStorage();
                listeComptes.Reload();
                addAccount = false;

                Connexion.contentFrame.Navigate(new Uri("/Intro", UriKind.Relative));
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

        static void serviceEIP_AddAccountCompleted(object sender, AddAccountCompletedEventArgs e)
        {
            if (e.Result)
            {
                GetSession();
                Connexion.contentFrame.Navigate(new Uri("/Intro", UriKind.Relative));
            }
            else
            {
                dispatcher.BeginInvoke(() =>
                {
                    MessageBox msg = new MessageBox("Erreur", "Ce compte existe déjà !");
                    msg.Show();
                });
            }

        }



        private static void LoginFacebook_LoginCompleted(object sender, EventArgs e)
        {
            facebookAPI = new Api(browserSession);
            //SetSession();
        }

        public static void Deconnexion()
        {
            //currentAccount = null;
            DestroySession();
        }

    }
}
