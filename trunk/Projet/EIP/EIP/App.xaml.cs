using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Facebook;
using Facebook.Schema;
using Facebook.Rest;
using Facebook.Session;
using System.IO.IsolatedStorage;
using Facebook.Utility;

namespace EIP
{
    public partial class App : System.Windows.Application
    {
        internal Api _facebookAPI { get; set; }
        internal BrowserSession _browserSession { get; set; }

        public const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a";
        public bool currentSessionExpires { get; set; }
        public string currentSessionKey { get; set; }
        public string currentSessionSecret { get; set; }
        public long currentUserID { get; set; }
        public List<Account> currentAccounts { get; set; }
        public List<Account> storageAccounts { get; set; }
        public Account currentAccount { get; set; }

        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            this._browserSession = new BrowserSession(ApplicationKey);
            this._browserSession.LoginCompleted += LoginFacebook_LoginCompleted;

            /*AccountFacebook acc = new AccountFacebook() { accountID = 0, typeAccount = Account.TypeAccount.Facebook, userID = 0 };
            List<Account> accs = new List<Account>();
            accs.Add(acc);
            storage["Accounts"] = accs;*/


            this.storageAccounts = (storage.Contains("Accounts") ? (List<Account>)storage["Accounts"] : null);
            GetSession();

            if (currentUserID != 0)
            {
                var theAccountID = from Account account in storageAccounts
                                 where account.userID == currentUserID
                                 select account.accountID;
                if (theAccountID.Count() > 0)
                {
                    var theAccounts = from Account account in storageAccounts
                                      where account.accountID == theAccountID.First()
                                      select account;

                    this.currentAccounts = new List<Account>();
                    foreach (Account acc in theAccounts)
                    {
                        this.currentAccounts.Add(acc);
                    }
                }
            }
           
            this.RootVisual = new MainPage();
        }

        private void GetSession()
        {
            currentSessionExpires = (storage.Contains("SessionExpires") ? (bool)storage["SessionExpires"] : true);
            currentSessionKey = (storage.Contains("SessionKey") ? (string)storage["SessionKey"] : null);
            currentSessionSecret = (storage.Contains("SessionSecret") ? (string)storage["SessionSecret"] : null);
            currentUserID = (storage.Contains("UserId") ? (long)storage["UserId"] : 0);
        }

        public void SetSession()
        {
            //storage["ApplicationSecret"] = _facebookAPI.Session.ApplicationSecret;
            currentSessionExpires = this._facebookAPI.Session.SessionExpires;
            storage["SessionExpires"] = currentSessionExpires;

            currentSessionKey = this._facebookAPI.Session.SessionKey;
            storage["SessionKey"] = currentSessionKey;

            currentSessionSecret = this._facebookAPI.Session.SessionSecret;
            storage["SessionSecret"] = currentSessionSecret;

            currentUserID = this._facebookAPI.Session.UserId;
            storage["UserId"] = currentUserID;
        }

        public void DestroySession()
        {
            currentSessionExpires = true;
            storage["SessionExpires"] = null;

            currentSessionKey = null;
            storage["SessionKey"] = null;

            currentSessionSecret = null;
            storage["SessionSecret"] = null;

            currentUserID = 0;
            storage["UserId"] = null;
        }

        public void AddAccount(Account.TypeAccount type)
        { 
            switch (type)
            {
                case Account.TypeAccount.Facebook:
                    if (this._facebookAPI != null)
                        this._facebookAPI.Session.Logout();
                    this._browserSession.LoginCompleted += NewAccountFacebook_LoginCompleted;
                    this._browserSession.Login();
                    break;
                case Account.TypeAccount.Twitter:
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
        }

        public void LoadAccount(Account account)
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

        private void LoginToAccount()
        {
            if (this.currentAccount != null)
            {
                switch (currentAccount.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        {
                            AccountFacebook myCurrentAccount = (AccountFacebook)currentAccount;
                            this._browserSession.LoggedIn(myCurrentAccount.sessionKey,
                                                            myCurrentAccount.sessionSecret,
                                                            Convert.ToInt32(myCurrentAccount.sessionExpires),
                                                            myCurrentAccount.userID);
                                //this._browserSession.Login();
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

        private void NewAccountFacebook_LoginCompleted(object sender, EventArgs e)
        {
            //LoginFacebook_LoginCompleted(sender, e);
            this._facebookAPI = new Api(this._browserSession);
            this._facebookAPI.Users.GetInfoAsync(new Users.GetInfoCallback(GetUserFacebook_Completed), new object()); 
        }

        private void GetUserFacebook_Completed(IList<user> users, object o, FacebookException ex)
        {
            if (users.Count > 0)
            {
                long accountId;
                if (currentAccounts != null)
                    accountId = currentAccounts[0].accountID;
                else
                    accountId = (long)users[0].uid;
                AccountFacebook newAccount = new AccountFacebook()
                {
                    accountID = accountId,
                    typeAccount = Account.TypeAccount.Facebook,
                    userID = this._facebookAPI.Session.UserId,
                    name = users[0].name,
                    sessionExpires = currentSessionExpires,
                    sessionKey = currentSessionKey,
                    sessionSecret = currentSessionSecret
                };
                this.currentAccount = newAccount;
                this.storageAccounts.Add(newAccount);
                storage["Accounts"] = this.storageAccounts;
            }
        }

        private void LoginFacebook_LoginCompleted(object sender, EventArgs e)
        {
            this._facebookAPI = new Api(this._browserSession);
            SetSession();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {

                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }
        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 2 Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        
    }
}
