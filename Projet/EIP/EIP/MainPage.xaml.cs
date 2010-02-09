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
using System.Windows.Navigation;
using Facebook;
using Facebook.Schema;
using Facebook.Rest;
using Facebook.Session;
using System.IO.IsolatedStorage;

namespace EIP
{
    public partial class MainPage : UserControl
    {

        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
        App app = (App)System.Windows.Application.Current;


        public MainPage()
        {
            InitializeComponent();
            //((App)System.Windows.Application.Current)._browserSession.LoginCompleted += browserSession_LoginCompleted;

            //LoginFB();
        }

        private void LoginFB()
        {
            if (((App)System.Windows.Application.Current).currentUserID != 0)
            {
                ((App)System.Windows.Application.Current)._browserSession.LoggedIn(((App)System.Windows.Application.Current).currentSessionKey,
                                                                                    ((App)System.Windows.Application.Current).currentSessionSecret,
                                                                                    Convert.ToInt32(((App)System.Windows.Application.Current).currentSessionExpires),
                                                                                    ((App)System.Windows.Application.Current).currentUserID);
            }
            else
                ((App)System.Windows.Application.Current)._browserSession.Login();   
        }

        private void browserSession_LoginCompleted(object sender, EventArgs e)
        {
            ((App)System.Windows.Application.Current)._facebookAPI = new Api(((App)System.Windows.Application.Current)._browserSession);
            ((App)System.Windows.Application.Current).SetSession();

            //ContentFrame.Source = new Uri("Home");
        }

        

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            /*foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }*/
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }

        private void LinkSeDeco_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (((App)System.Windows.Application.Current)._facebookAPI != null)
            {
                ((App)System.Windows.Application.Current)._facebookAPI.Session.Logout();
                ((App)System.Windows.Application.Current)._facebookAPI = null;
            }
            ((App)System.Windows.Application.Current).DestroySession();
             * */
        }

        private void LinkHome_Click(object sender, RoutedEventArgs e)
        {
           // if (((App)System.Windows.Application.Current)._facebookAPI == null)
            //    LoginFB();
            //LeftFrame.Navigate(new Uri("/Views/MenuFeeds.xaml", UriKind.Relative));
        }

        private void LinkCreateNewFbAccount_Click(object sender, RoutedEventArgs e)
        {
            app.AddAccount(Account.TypeAccount.Facebook);
        }
    }
}
