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

//using TweetSharp.Fluent;
//using TweetSharp.Extensions;
//using TweetSharp.Model;

//using TweetSharp.Twitter.Fluent;
//using TweetSharp.Twitter.Extensions;
//using TweetSharp.Twitter.Model;

//using TweetSharp;
using EIP.Views.Child;
using EIP.Objects;
//using TweetSharp.Fluent;




namespace EIP
{
    public static class Connexion
    {
        //Objets Facebook
        public static Api facebookAPI { get; set; }
        private static BrowserSession browserSession { get; set; }

        //api key Facebook
        public static string ApplicationKey = "";// = "e0c1f6b95b88d23bfc9727e0ea90602a";

        //api key Twitter
        //public const string ProxyUrl = "http://localhost:4164/proxy";
        //public const string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        //public const string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

        //Accounts
        public static Dictionary<long, AccountLight> accounts { get; set; }

        //storage
        public static IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
       
        //Controls
        public static ListeComptes listeComptes;
        public static Dispatcher dispatcher;
        public static Frame contentFrame;
        public static NavigationContext navigationContext;
        public static NavigationService navigationService;
        public static BusyIndicator mainBusyIndicator;  


        //WCF
        public static ServiceEIP.ServiceEIPClient serviceEIP;

        //data
        public static Dictionary<string, List<Topic>> allTopics = new Dictionary<string, List<Topic>>();

        //Autre
        private static bool addAccount = false;
        private static Loading loadingChild;
        private static bool connexionActive = false;

        public static DispatcherTimer dt = new DispatcherTimer();

        public static Enums.ExtendedPermissions[] perms = new Enums.ExtendedPermissions[8];
       
        /////////
   
        public static void Start()
        {
            //browserSession = new BrowserSession(ApplicationKey);
            //browserSession.LoginCompleted += LoginFacebook_LoginCompleted;
            accounts = new Dictionary<long, AccountLight>();
            serviceEIP = new ServiceEIP.ServiceEIPClient();
            loadingChild = new Loading();

            serviceEIP.IsUpCompleted += new EventHandler<IsUpCompletedEventArgs>(serviceEIP_IsUpCompleted);
            serviceEIP.GetFBAppKeyCompleted += new EventHandler<GetFBAppKeyCompletedEventArgs>(serviceEIP_GetFBAppKeyCompleted);
            Connexion.serviceEIP.DeleteAccountCompleted += new EventHandler<DeleteAccountCompletedEventArgs>(serviceEIP_DeleteAccountCompleted);

            perms[0] = Enums.ExtendedPermissions.offline_access;
            perms[1] = Enums.ExtendedPermissions.publish_stream;
            perms[2] = Enums.ExtendedPermissions.photo_upload;
            perms[3] = Enums.ExtendedPermissions.read_mailbox;
            perms[4] = Enums.ExtendedPermissions.email;
            perms[5] = Enums.ExtendedPermissions.status_update;
            perms[6] = Enums.ExtendedPermissions.read_stream;
            perms[7] = Enums.ExtendedPermissions.manage_mailbox;

            try
            {
                serviceEIP.IsUpAsync();
            }
            catch (Exception ex)
            {

            }

            //SetTwitterClientInfo();

            
            
        }

        static void serviceEIP_GetFBAppKeyCompleted(object sender, GetFBAppKeyCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ApplicationKey = e.Result;
                GetSession();
            }
        }

        

        static void serviceEIP_IsUpCompleted(object sender, IsUpCompletedEventArgs e)
        {
            if (e.Error == null && e.Result == true)
            {
                connexionActive = true;
                serviceEIP.GetFBAppKeyAsync();
            }
        }



        public static void StartDisplay()
        {
            Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
            listeComptes.Reload();
        }

        public static void Loading(bool isLoading)
        {
            dispatcher.BeginInvoke(() =>
                {
                    if (isLoading)
                        Connexion.mainBusyIndicator.IsBusy = true;
                        //loadingChild.Show();
                    else
                        Connexion.mainBusyIndicator.IsBusy = false;
                        //loadingChild.Close();
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
            bool showLogin = true;

            if (connexionActive)
            {
                if (storage.Contains("groupID-" + ApplicationKey))
                {
                    if (storage["groupID-" + ApplicationKey] != null && (storage["groupID-" + ApplicationKey].ToString() != "0"))
                    {
                        showLogin = false;
                        serviceEIP.GetAccountsByGroupIDCompleted += new EventHandler<GetAccountsByGroupIDCompletedEventArgs>(serviceEIP_GetAccountsByGroupIDCompleted);
                        serviceEIP.GetAccountsByGroupIDAsync(Convert.ToInt64(storage["groupID-" + ApplicationKey].ToString()));
                        
                    }
                }
            }
            else
            {
                foreach (string key in storage.Keys)
                {
                    if (key.StartsWith("Account-" + ApplicationKey + "-"))
                    {
                        showLogin = false;
                        AccountLight tmp = new AccountLight();
                        tmp.account = (Account)storage[key];
                        accounts[tmp.account.accountID] = tmp;// (AccountLight)storage[key];
                    }
                }
            }
    
            if (showLogin)
            {
                Login loginWindow = new Login(false);
                loginWindow.Show();
                Connexion.Loading(false);
            }

            if (listeComptes != null)
            {
                Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
                listeComptes.Reload();
            } 
        }

        private static void SetSession(long groupID)
        {
            storage["groupID-" + ApplicationKey] = groupID.ToString();
            storage.Save();

            foreach (KeyValuePair<long, AccountLight> acc in accounts)
            {
                storage["Account-" + ApplicationKey +"-" + acc.Value.account.accountID] = acc.Value.account;
            }
            storage.Save();
            

            //storage["CurrentAccount"] = currentAccount;
            //storage["CurrentAccount"] = "Account-" + currentAccount.account.typeAccount.ToString() + "-" + currentAccount.account.userID;
        }

        /*
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
         * */

        static void serviceEIP_GetAccountsByGroupIDCompleted(object sender, GetAccountsByGroupIDCompletedEventArgs e)
        {
            if (e.Result != null && e.Error == null)
            {
                long groupid = 0;
                accounts.Clear();
                foreach (Account oneAccount in e.Result)
                {
                    groupid = oneAccount.groupID;
                    switch (oneAccount.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            AccountFacebookLight newAccountFacebook = new AccountFacebookLight();
                            newAccountFacebook.account = oneAccount;
                            accounts[newAccountFacebook.account.accountID] = newAccountFacebook;
                            newAccountFacebook.Login();
                            break;
                        case Account.TypeAccount.Twitter:
                            AccountTwitterLight newAccountTwitter = new AccountTwitterLight();
                            newAccountTwitter.account = oneAccount;
                            newAccountTwitter.Start();
                            //accounts.Add(newAccountTwitter);
                            accounts[newAccountTwitter.account.accountID] = newAccountTwitter;
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;

                    }
                }
                SetSession(groupid);
                Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
                listeComptes.Reload();
                Connexion.contentFrame.Navigate(new Uri("/Home", UriKind.Relative));
            }
        }



        private static void DestroySession()
        {
            //storage["CurrentAccount"] = null;
            //currentAccounts = null;
            accounts = null;
            //storage["groupID"] = "0";
            storage.Remove("groupID");
            storage.Save();
            /*foreach (string key in storage.Keys)
            {
                if (key.StartsWith("Account-"))
                {
                    storage.Remove(key);
                }
                   
            }*/
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
                        browserSession.LogoutCompleted += BrowserSession_LogoutCompleted;
                        facebookAPI = null;
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

                    /*
                    var requestToken = FluentTwitter.CreateRequest()
                       .Configuration.UseTransparentProxy(ProxyUrl)
                       .Authentication.GetRequestToken()
                       .CallbackTo(TwitterRequestTokenReceived);

                    //requestToken.BeginRequest();
                    requestToken.BeginRequest();
                     * 
                     */

                    serviceEIP.GetRequestTokenCompleted += new EventHandler<GetRequestTokenCompletedEventArgs>(serviceEIP_GetRequestTokenCompleted);
                    serviceEIP.GetRequestTokenAsync();



                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        static void serviceEIP_GetRequestTokenCompleted(object sender, GetRequestTokenCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                string token = e.Result;
                Uri uri = new Uri("http://api.twitter.com/oauth/authorize?oauth_token=" + token);
                AccountTwitterLight accountTwitter = new AccountTwitterLight();
                ((AccountTwitter)accountTwitter.account).token = token;
                TwitterPin twitterPin = new TwitterPin(accountTwitter, uri);
                twitterPin.Show();
            }
            else
            {
                MessageBox msgBox = new MessageBox("Erreur", "Erreur lors de la réception du token de connexion !", MessageBoxButton.OK);
            }

        }

        /*
        private static void TwitterRequestTokenReceived(object sender, TwitterResult result)
        {
            System.Diagnostics.Debugger.Break();
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
         * */

        /*
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
         * */

        //private static void test_AuthorizeDesktopCompleted(object sender, ServiceEIP.AuthorizeDesktopCompletedEventArgs e)
        /*private static void Twitter_AuthorizeDesktop(object sender, TwitterResult result)
        {
            var tokenRes = result.AsToken();
            string token = tokenRes.Token;// e.Result;
            AccountTwitterLight accountTwitter = new AccountTwitterLight();
            ((AccountTwitter)accountTwitter.account).token = token;
            TwitterPin twitterPin = new TwitterPin(accountTwitter, new Uri(""));
            twitterPin.Show();
        }*/

        /*
        public static void SetTwitterClientInfo()
        {
            var clientInfo = new TwitterClientInfo
            {
                ConsumerKey = consumerKey,
                ConsumerSecret = consumerSecret
            };

            FluentBase<TwitterResult>.SetClientInfo(clientInfo);
        }
         * */

        public static void AddTwitterAccount(AccountTwitterLight accountTwitter)//, Dispatcher dispatch
        {
            accountTwitter.account.typeAccount = Account.TypeAccount.Twitter;

            if (accounts.Count > 0)
            {
                accountTwitter.account.groupID = accounts.First().Value.account.groupID;
            }
            else
            {
                Random rand = new Random();
                int number = rand.Next(999999999);
                accountTwitter.account.groupID = number;// Convert.ToInt64(token.UserId);// (long)tmp.userID;
            }
            accountTwitter.selected = true;

            SetSession(accountTwitter.account.groupID);
            serviceEIP.AddAccountCompleted += new EventHandler<AddAccountCompletedEventArgs>(serviceEIP_AddAccountCompleted);
            serviceEIP.AddAccountAsync(accountTwitter.account, ((AccountTwitter)accountTwitter.account).token, accountTwitter.pin);

        }

        private static void BrowserSession_LogoutCompleted(object sender, EventArgs e)
        {
            if (ApplicationKey != "")
            {
                browserSession = new BrowserSession(ApplicationKey, perms);
                browserSession.LoginCompleted += NewAccountFacebook_LoginCompleted;
                browserSession.Login();
            }
            else
                serviceEIP.GetFBAppKeyAsync();
        }

        private static void NewAccountFacebook_LoginCompleted(object sender, EventArgs e)
        {
            if (facebookAPI == null)
            {
                facebookAPI = new Api(browserSession);
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
                    newAccount.selected = true;
                    ((AccountFacebook)newAccount.account).sessionExpires = facebookAPI.Session.SessionExpires;
                    ((AccountFacebook)newAccount.account).sessionKey = facebookAPI.Session.SessionKey;
                    ((AccountFacebook)newAccount.account).sessionSecret = facebookAPI.Session.SessionSecret;
                    
                    SetSession(newAccount.account.groupID);

                    serviceEIP.AddAccountCompleted += new EventHandler<AddAccountCompletedEventArgs>(serviceEIP_AddAccountCompleted);
                    serviceEIP.AddAccountAsync(newAccount.account, null, null);

       

                }
                else
                {
                    serviceEIP.GetAccountsByUserIDCompleted += new EventHandler<GetAccountsByUserIDCompletedEventArgs>(serviceEIP_GetAccountsByUserIDCompleted);
                    serviceEIP.GetAccountsByUserIDAsync((long)users[0].uid);
                }
            }
        }

        static void serviceEIP_GetAccountsByTwitterCompleted(object sender, GetAccountsByTwitterCompletedEventArgs e)
        {
            LoadAccountsFromDB(e.Result);
        }

        static void serviceEIP_GetAccountsByUserIDCompleted(object sender, GetAccountsByUserIDCompletedEventArgs e)
        {
            if (e.Error == null)
            {LoadAccountsFromDB(e.Result);

                //if (e.Result != null)
                //{
                //    if (e.Result.Count > 0)
                //        LoadAccountsFromDB(e.Result);
                //    else
                //    {
                //        dispatcher.BeginInvoke(() =>
                //            {
                //                MessageBox msgBox = new MessageBox(
                //            });
                //    }
                //}

            }
        }

        private static void LoadAccountsFromDB(List<Account> result)
        {
            if (result != null && result.Count > 0)
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

                            accounts[newAccountFacebook.account.accountID] = newAccountFacebook;
                            break;
                        case Account.TypeAccount.Twitter:
                            AccountTwitterLight newAccountTwitter = new AccountTwitterLight();
                            newAccountTwitter.account = oneAccount;
                            newAccountTwitter.Start();
                            //accounts.Add(newAccountTwitter);
                            accounts[newAccountTwitter.account.accountID] = newAccountTwitter;
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
                Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
                listeComptes.Reload();
                addAccount = false;

                Connexion.contentFrame.Navigate(new Uri("/Home", UriKind.Relative));
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
            if (e.Error == null)
            {
                if (e.Result)
                {
                    GetSession();
                    Connexion.contentFrame.Navigate(new Uri("/Home", UriKind.Relative));
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
            else
            {
                MessageBox msg = new MessageBox("Erreur", "Erreur lors de l'ajout du compte");
                msg.Show();
            }

        }


        /*
        private static void LoginFacebook_LoginCompleted(object sender, EventArgs e)
        {
            facebookAPI = new Api(browserSession);
            //SetSession();
        }*/

        public static void Deconnexion()
        {
            DestroySession();
        }

        static void serviceEIP_DeleteAccountCompleted(object sender, DeleteAccountCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != 0)
            {
                MessageBox msgBox = new MessageBox("Confirmation", "Le compte à été supprimé avec succès", MessageBoxButton.OK);
                msgBox.Show();

                DeleteAccount(e.Result);
            }
        }

        private static void DeleteAccount(long accountID)
        {
            accounts.Remove(accountID);
            listeComptes.Reload();
        }


        public static bool SaveStorageValue(string key, object data)
        {
            try
            {
                storage[key] = data;
                storage.Save();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object GetStorageValue(string key)
        {
            try
            {
                if(storage.Contains(key))
                    return storage[key];

                return null;
            }
            catch
            {
                return null;
            }
        }


    }
}
