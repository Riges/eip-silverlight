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

namespace EIP
{
    public partial class App : System.Windows.Application
    {
        internal Api _facebookAPI { get; set; }
        internal BrowserSession _browserSession { get; set; }

        public const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a";
        public bool sessionExpires { get; set; }
        public string sessionKey { get; set; }
        public string sessionSecret { get; set; }
        public long userID { get; set; }
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

            _browserSession = new BrowserSession(ApplicationKey);
            GetSession();
            this.RootVisual = new MainPage();
        }

        private void GetSession()
        {
            sessionExpires = (storage.Contains("SessionExpires") ? (bool)storage["SessionExpires"] : true);
            sessionKey = (storage.Contains("SessionKey") ? (string)storage["SessionKey"] : null);
            sessionSecret = (storage.Contains("SessionSecret") ? (string)storage["SessionSecret"] : null);
            userID = (storage.Contains("UserId") ? (long)storage["UserId"] : 0);
        }

        public void SetSession()
        {
            //storage["ApplicationSecret"] = _facebookAPI.Session.ApplicationSecret;
            sessionExpires = _facebookAPI.Session.SessionExpires;
            storage["SessionExpires"] = sessionExpires;

            sessionKey = _facebookAPI.Session.SessionKey;
            storage["SessionKey"] = sessionKey;

            sessionSecret = _facebookAPI.Session.SessionSecret;
            storage["SessionSecret"] = sessionSecret;

            userID = _facebookAPI.Session.UserId;
            storage["UserId"] = userID;
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
