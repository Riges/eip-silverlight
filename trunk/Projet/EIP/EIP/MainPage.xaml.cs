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
using EIP.ServiceEIP;
using EIP.Views.Child;

namespace EIP
{
    public partial class MainPage : UserControl
    {

        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;

        public MainPage()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            Connexion.listeComptes = liste;
            Connexion.contentFrame = ContentFrame;
            Connexion.LoginToAccount();
            Connexion.dispatcher = Dispatcher;
            Connexion.StartDisplay();
            LoadInterface();
        }

        private void LoadInterface()
        {
            if (Connexion.currentAccount == null)
            {
                LinkHome.Visibility = System.Windows.Visibility.Collapsed;
                DividerHome.Visibility = System.Windows.Visibility.Collapsed;
                LinkFriends.Visibility = System.Windows.Visibility.Collapsed;
                DividerFriends.Visibility = System.Windows.Visibility.Collapsed;
                LinkSeDeco.Visibility = System.Windows.Visibility.Collapsed;

            }
            else
            {
                LinkHome.Visibility = System.Windows.Visibility.Collapsed;
                DividerHome.Visibility = System.Windows.Visibility.Collapsed;
                LinkFriends.Visibility = System.Windows.Visibility.Collapsed;
                DividerFriends.Visibility = System.Windows.Visibility.Collapsed;
                LinkSeDeco.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool showLogin = true;
             if (storage.Contains("groupID"))
                 if (storage["groupID"] != null)
                 {
                     showLogin = false;
                 }
             if (showLogin)
             {
                 Views.Child.Login loginWindow = new Views.Child.Login(false);
                 loginWindow.Show();
             }

        }

        private void browserSession_LoginCompleted(object sender, EventArgs e)
        {

        }

        

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
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
            }
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
            Connexion.Deconnexion();
            liste.Reload();
            ContentFrame.Navigate(new Uri("/Deconnexion", UriKind.Relative));
            
        }

        private void LinkHome_Click(object sender, RoutedEventArgs e)
        {
           // if (((App)System.Windows.Application.Current)._facebookAPI == null)
            //    LoginFB();
            //LeftFrame.Navigate(new Uri("/Views/MenuFeeds.xaml", UriKind.Relative));
        }



        private void LinkCreateNewAccount_Click(object sender, RoutedEventArgs e)
        {
            Login loginBox = new Login(true);
            loginBox.Show();
        }


    }
}
