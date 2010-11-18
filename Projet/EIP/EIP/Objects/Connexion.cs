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

using FlickrNet;

//using TweetSharp.Fluent;
//using TweetSharp.Extensions;
//using TweetSharp.Model;

//using TweetSharp.Twitter.Fluent;
//using TweetSharp.Twitter.Extensions;
//using TweetSharp.Twitter.Model;

//using TweetSharp;
using EIP.Views.Child;
using EIP.Objects;
using System.Windows.Browser;
using System.Reflection;
//using TweetSharp.Fluent;




namespace EIP
{
    public static class Connexion
    {
        //Objets Facebook
        public static Api facebookAPI { get; set; }
        public static BrowserSession browserSession { get; set; }
        public static Flickr flickr { get; set; }
        public static string keyFlickr { get; set; }
        public static string secretFlickr { get; set; }




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

        public delegate void OnClick();
        public static event OnClick OnClickCalled;

        /////////
   
        public static void Start()
        {
           
            string host = App.Current.Host.Source.Host;
            if(host.Contains("localhost"))
            {
                host += ":6080/ServiceEIP.svc";
            }
            else
                host += "/WCF/ServiceEIP.svc";
            string urlWCF = "http://" + host;
            Uri addressWCF = new Uri(urlWCF, UriKind.Absolute);

            serviceEIP = new ServiceEIP.ServiceEIPClient("BasicHttpBinding_Service", addressWCF.AbsoluteUri);

            #if (DEBUG)
            keyFlickr = "7121e516217daa3202fbc6f269cc3874";
            secretFlickr = "4a2d74587af51a4d";
            #else
                keyFlickr = "de649ab5a4af089ff78ce07576f0477f";
                secretFlickr = "3d2ad5bfaf480eae";
            #endif

            //browserSession = new BrowserSession(ApplicationKey);
            //browserSession.LoginCompleted += LoginFacebook_LoginCompleted;
            accounts = new Dictionary<long, AccountLight>();
            //serviceEIP = new ServiceEIP.ServiceEIPClient();
            loadingChild = new Loading();

            serviceEIP.IsUpCompleted += new EventHandler<IsUpCompletedEventArgs>(serviceEIP_IsUpCompleted);
            serviceEIP.GetFBAppKeyCompleted += new EventHandler<GetFBAppKeyCompletedEventArgs>(serviceEIP_GetFBAppKeyCompleted);
            Connexion.serviceEIP.DeleteAccountCompleted += new EventHandler<DeleteAccountCompletedEventArgs>(serviceEIP_DeleteAccountCompleted);

            serviceEIP.GetRequestTokenCompleted += new EventHandler<GetRequestTokenCompletedEventArgs>(serviceEIP_GetRequestTokenCompleted);

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

        public static void SilverClick()
        {
            if (OnClickCalled != null)
                OnClickCalled.Invoke();
        }

     
        static void serviceEIP_GetFBAppKeyCompleted(object sender, GetFBAppKeyCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ApplicationKey = e.Result;

                GetFrob();              
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

        public static void GetFrob()
        {
            if (App.Current.IsRunningOutOfBrowser)
            {
                GetSession(0);
            }
            else
            {
                string frob = string.Empty;
                if (HtmlPage.Document.QueryString.ContainsKey("frob"))
                    frob = HtmlPage.Document.QueryString["frob"];

                string oauth_token = string.Empty;
                if (HtmlPage.Document.QueryString.ContainsKey("oauth_token"))
                    oauth_token = HtmlPage.Document.QueryString["oauth_token"];

                string oauth_verifier = string.Empty;
                if (HtmlPage.Document.QueryString.ContainsKey("oauth_verifier"))
                    oauth_verifier = HtmlPage.Document.QueryString["oauth_verifier"];

                if (frob != string.Empty)
                {
                    flickr = new Flickr(Connexion.keyFlickr, Connexion.secretFlickr);
                    FlickrGetFrob_Completed(frob);
                }
                else if (oauth_token != string.Empty && oauth_verifier != string.Empty)
                {
                    AccountTwitterLight accountTwitter = new AccountTwitterLight();
                    ((AccountTwitter)accountTwitter.account).token = oauth_token;
                    ((AccountTwitter)accountTwitter.account).pin = oauth_verifier;

                    AddTwitterAccount(accountTwitter);
                }
                else
                {
                    GetSession(0);
                }
            }
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

        private static void GetSession(long groupID)
        {
            bool showLogin = true;

            if (connexionActive)
            {
                

                if (groupID != 0)
                {
                    showLogin = false;
                    serviceEIP.GetAccountsByGroupIDCompleted += new EventHandler<GetAccountsByGroupIDCompletedEventArgs>(serviceEIP_GetAccountsByGroupIDCompleted);
                    serviceEIP.GetAccountsByGroupIDAsync(groupID);
                }
                else if (storage.Contains("groupID-" + ApplicationKey))
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
        }

        private static void DestroySession()
        {
            accounts = null;
            storage.Remove("groupID-" + ApplicationKey);
            storage.Save();
        }

        static void serviceEIP_GetAccountsByGroupIDCompleted(object sender, GetAccountsByGroupIDCompletedEventArgs e)
        {
            if (e.Result != null && e.Error == null)
            {
                LoadAccountsFromDB(e.Result);
                //long groupid = 0;
                //accounts.Clear();
                //foreach (Account oneAccount in e.Result)
                //{
                //    groupid = oneAccount.groupID;
                //    switch (oneAccount.typeAccount)
                //    {
                //        case Account.TypeAccount.Facebook:
                //            AccountFacebookLight newAccountFacebook = new AccountFacebookLight();
                //            newAccountFacebook.account = oneAccount;
                //            accounts[newAccountFacebook.account.accountID] = newAccountFacebook;
                //            newAccountFacebook.Login();
                //            break;
                //        case Account.TypeAccount.Twitter:
                //            AccountTwitterLight newAccountTwitter = new AccountTwitterLight();
                //            newAccountTwitter.account = oneAccount;
                //            newAccountTwitter.Start();
                //            //accounts.Add(newAccountTwitter);
                //            accounts[newAccountTwitter.account.accountID] = newAccountTwitter;
                //            break;
                //        case Account.TypeAccount.Flickr:
                //            AccountFlickrLight newAccountFlickr = new AccountFlickrLight();
                //            newAccountFlickr.account = oneAccount;
                //            accounts[newAccountFlickr.account.accountID] = newAccountFlickr;
                            
                //            break;
                //        default:
                //            break;

                //    }
                //}
                //SetSession(groupid);
                //Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
                //listeComptes.Reload();


                //Connexion.contentFrame.Navigate(new Uri("/Home", UriKind.Relative));
            }
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
                    /*
                    serviceEIP.GetAccountsByTwitterCompleted += new EventHandler<GetAccountsByTwitterCompletedEventArgs>(serviceEIP_GetAccountsByTwitterCompleted);
                    serviceEIP.GetAccountsByTwitterAsync(pseudo, password);
                    */
                    string host = App.Current.Host.Source.Host;
                    string callback = string.Empty;
                    if (host.Contains("localhost"))
                        callback = "http://localhost:4164/";
                    else
                        callback = "http://mynetwork.selfip.net/";

                    serviceEIP.GetRequestTokenAsync(callback);
                    
                    break;
                case Account.TypeAccount.Flickr:

                    Flickr flickr = new Flickr(Connexion.keyFlickr, Connexion.secretFlickr);

                    string urlAuth = flickr.AuthCalcWebUrl(AuthLevel.Delete);

                    HtmlPage.Window.Navigate(new Uri(urlAuth, UriKind.Absolute));
                    break;
             
                default:
                    break;
            }
        }

        public static void AddAccount(Account.TypeAccount type)
        {
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

                    string host = App.Current.Host.Source.Host;
                    string callback = string.Empty;
                    if (host.Contains("localhost"))
                        callback = "http://localhost:4164/";
                    else
                        callback = "http://mynetwork.selfip.net/";

                    serviceEIP.GetRequestTokenAsync(callback);

                    break;
                case Account.TypeAccount.Flickr:

                    flickr = new Flickr(Connexion.keyFlickr, Connexion.secretFlickr);

                    string urlAuth = flickr.AuthCalcWebUrl(AuthLevel.Delete);

                    HtmlPage.Window.Navigate(new Uri(urlAuth, UriKind.Absolute));

                    break;     
                default:
                    break;
            }
        }

        static void FlickrGetFrob_Completed(string frob)
        {
            flickr.AuthGetTokenAsync(frob, new Action<FlickrResult<FlickrNet.Auth>>(AuthGetToken_Completed));
        }

        static void AuthGetToken_Completed(FlickrResult<FlickrNet.Auth> auth)
        {
            flickr.AuthToken = auth.Result.Token;

            AccountFlickrLight newAccount = new AccountFlickrLight();

            Random rand = new Random();
            if (storage.Contains("groupID-" + ApplicationKey) && storage["groupID-" + ApplicationKey].ToString() != "0")
            {
                newAccount.account.groupID = Convert.ToInt64(storage["groupID-" + ApplicationKey]);
            }
            else if (accounts.Count > 0)
            {
                newAccount.account.groupID = accounts.First().Value.account.groupID;
            }
            else
            {
                int number = rand.Next(999999999);
                newAccount.account.groupID = number;// Convert.ToInt64(token.UserId);// (long)tmp.userID;
            }

            newAccount.account.typeAccount = Account.TypeAccount.Flickr;
            newAccount.account.userID = rand.Next(999999999);;
            newAccount.account.name = auth.Result.User.UserName;
            newAccount.selected = true;
            ((AccountFlickr)newAccount.account).token = auth.Result.Token;
            ((AccountFlickr)newAccount.account).userIDstr = auth.Result.User.UserId;

            SetSession(newAccount.account.groupID);

            serviceEIP.AddAccountCompleted += new EventHandler<AddAccountCompletedEventArgs>(serviceEIP_AddAccountCompleted);
            serviceEIP.AddAccountAsync(newAccount.account, null, null);
        }

        static void serviceEIP_GetRequestTokenCompleted(object sender, GetRequestTokenCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                string token = e.Result;
                Uri uri = new Uri("http://api.twitter.com/oauth/authorize?oauth_token=" + token, UriKind.Absolute);//oauth_callback="+ HttpUtility.UrlEncode("http://localhost:4164/" + "&

                HtmlPage.Window.Navigate(uri);
                /*AccountTwitterLight accountTwitter = new AccountTwitterLight();
                ((AccountTwitter)accountTwitter.account).token = token;
                TwitterPin twitterPin = new TwitterPin(accountTwitter, uri);
                twitterPin.Show();*/
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
                if (storage.Contains("groupID-" + ApplicationKey))
                {
                    if (storage["groupID-" + ApplicationKey] != null && (storage["groupID-" + ApplicationKey].ToString() != "0"))
                    {
                        accountTwitter.account.groupID = Convert.ToInt64(storage["groupID-" + ApplicationKey].ToString());
                    }
                }
                else
                {
                    Random rand = new Random();
                    int number = rand.Next(999999999);
                    accountTwitter.account.groupID = number;// Convert.ToInt64(token.UserId);// (long)tmp.userID;
                }
            }
            accountTwitter.selected = true;

            SetSession(accountTwitter.account.groupID);
            serviceEIP.AddAccountCompleted += new EventHandler<AddAccountCompletedEventArgs>(serviceEIP_AddAccountCompleted);
            serviceEIP.AddAccountAsync(accountTwitter.account, ((AccountTwitter)accountTwitter.account).token, ((AccountTwitter)accountTwitter.account).pin);
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
                    {
                        Random rand = new Random();
                        int number = rand.Next(999999999);
                        newAccount.account.groupID = number;
                    }
                        //newAccount.account.groupID = (long)users[0].uid;

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
            {
                LoadAccountsFromDB(e.Result);

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
                long groupid = 0;
                accounts.Clear();
                foreach (Account oneAccount in result)
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
                            accounts[newAccountTwitter.account.accountID] = newAccountTwitter;
                            break;
                        case Account.TypeAccount.Flickr:
                            AccountFlickrLight newAccountFlickr = new AccountFlickrLight();
                            newAccountFlickr.account = oneAccount;
                            accounts[newAccountFlickr.account.accountID] = newAccountFlickr;
                            break;
                        default:
                            break;
                    }
                }
                SetSession(groupid);
                Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
                listeComptes.Reload();
                addAccount = false;

                Connexion.contentFrame.Navigate(new Uri("/Home", UriKind.Relative));
            }
            else
                mainBusyIndicator.IsBusy = false;
            //if (result != null && result.Count > 0)
            //{
            //    accounts = new Dictionary<long, AccountLight>();
            //    foreach (Account oneAccount in result)
            //    {
            //        switch (oneAccount.typeAccount)
            //        {
            //            case Account.TypeAccount.Facebook:
            //                AccountFacebookLight newAccountFacebook = new AccountFacebookLight();
            //                newAccountFacebook.account = oneAccount;
            //                //accounts.Add(newAccountFacebook);

            //                newAccountFacebook.Login();

            //                accounts[newAccountFacebook.account.accountID] = newAccountFacebook;
            //                break;
            //            case Account.TypeAccount.Twitter:
            //                AccountTwitterLight newAccountTwitter = new AccountTwitterLight();
            //                newAccountTwitter.account = oneAccount;
            //                newAccountTwitter.Start();
            //                //accounts.Add(newAccountTwitter);
            //                accounts[newAccountTwitter.account.accountID] = newAccountTwitter;
            //                break;
                        
            //            default:
            //                break;
            //        }
            //        SetSession(oneAccount.groupID);
            //    }
                
            //    //currentAccount.account = e.Result;
            //    //LoginToAccount();

            //    //LoadFromStorage();
            //    Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
            //    listeComptes.Reload();
            //    addAccount = false;

            //    Connexion.contentFrame.Navigate(new Uri("/Home", UriKind.Relative));
            //}
            //else
            //{
            //    dispatcher.BeginInvoke(() =>
            //    {
            //        MessageBox msg = new MessageBox("Erreur de connexion", "Vous devez ajouter un compte avant de pouvoir vous connecter dessus !");
            //        msg.Show();
            //    });
            //}
        }

        static void serviceEIP_AddAccountCompleted(object sender, AddAccountCompletedEventArgs e)
        {
            dispatcher.BeginInvoke(() =>
                {
                    if (e.Error == null)
                    {
                        if (e.Result > 0)
                        {
                            GetSession(e.Result);
                            Connexion.contentFrame.Navigate(new Uri("/Home?time=00000", UriKind.Relative));
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
                });

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
            string host = App.Current.Host.Source.Host;
            if (host.Contains("localhost"))
                navigationService.Navigate(new Uri("http://locahost:4164", UriKind.Absolute));
            else
                navigationService.Navigate(new Uri("http://mynetwork.selfip.net", UriKind.Absolute));
            /*
            accounts.Remove(accountID);
            listeComptes.Reload();
            */
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
