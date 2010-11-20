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
using System.Windows.Threading;

namespace EIP
{
    public partial class App : System.Windows.Application
    {
        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();

            if (App.Current.IsRunningOutOfBrowser)
            {
                App.Current.CheckAndDownloadUpdateCompleted +=
                    new CheckAndDownloadUpdateCompletedEventHandler(CheckAndDownloadUpdateCompleted);
                App.Current.CheckAndDownloadUpdateAsync();
            }
        }

        private void CheckAndDownloadUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.Error == null && e.UpdateAvailable)
            {
                MessageBox msgBox = new MessageBox("Mise à jour réussi !", "myNETwork a été mis à jour, veuillez relancer l'application", MessageBoxButton.OK);
                msgBox.Show();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Connexion.Start();
           
            this.RootVisual = new MainPage();
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
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(sender, e); });
            }
        }
        private void ReportErrorToDOM(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");


                if (Connexion.serviceEIP != null)
                {
                    long groupID = 0;
                    if (Connexion.accounts.Count > 0)
                    {
                        groupID = Connexion.accounts.First().Value.account.groupID;
                    }

                    Connexion.serviceEIP.LogErrorCompleted += new EventHandler<ServiceEIP.LogErrorCompletedEventArgs>(serviceEIP_LogErrorCompleted);
                    Connexion.serviceEIP.LogErrorAsync(groupID, e.ExceptionObject.StackTrace, e.ExceptionObject.Message);
                }

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight 4 Application, Erreur : " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }

        void serviceEIP_LogErrorCompleted(object sender, ServiceEIP.LogErrorCompletedEventArgs e)
        {
            if (e.Error != null )
            {
                string errorMsg = e.Error.Message + e.Error.StackTrace;
                 errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"LogErrorCompleted, Erreur : "+ errorMsg + "\");");
            }
           if (!e.Result)
           {
               System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"LogErrorCompleted, Erreur : retour false\");");
           }
        }

        
    }
}
