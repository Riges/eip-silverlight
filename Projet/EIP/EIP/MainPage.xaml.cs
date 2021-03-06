﻿using System;
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
using System.Windows.Browser;

namespace EIP
{
    public partial class MainPage : UserControl
    {

        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;
        private long UnreadMessagesNumber { get; set; }

        public MainPage()
        {
            InitializeComponent();

            HtmlPage.RegisterScriptableObject("slObject", this);
            

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            Connexion.listeComptes = liste;
            Connexion.contentFrame = ContentFrame;
            //Connexion.LoginToAccount();
            Connexion.dispatcher = Dispatcher;
            Connexion.mainBusyIndicator = mainBusyIndicator;
            Connexion.StartDisplay();
            LoadInterface();
        }

        [ScriptableMember]
        public void LoginComplete(string accesstoken, string errorDescription)
        {
            Connexion.LoginFB_Completed(accesstoken, errorDescription);
            /*
            if (string.IsNullOrEmpty(errorDescription) && !string.IsNullOrEmpty(accesstoken))
            {
                // we have access token.
                fb = new FacebookClient(accesstoken);
                loginSucceeded();
            }
            else
            {
                HtmlPage.Window.Alert(errorDescription);
            }*/
        }

        private void LoadInterface()
        {
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                LinkHome.Visibility = System.Windows.Visibility.Visible;
                DividerHome.Visibility = System.Windows.Visibility.Visible;
                LinkFriends.Visibility = System.Windows.Visibility.Visible;
                DividerFriends.Visibility = System.Windows.Visibility.Visible;
                LinkMessages.Visibility = System.Windows.Visibility.Visible;
                DividerMessages.Visibility = System.Windows.Visibility.Visible;
                //LinkProfil.Visibility = System.Windows.Visibility.Visible;
                //DividerProfil.Visibility = System.Windows.Visibility.Visible;
                updateStatusControl.Visibility = System.Windows.Visibility.Visible;
                LinkSeDeco.Visibility = System.Windows.Visibility.Visible;

                LinkSeConnecter.Visibility = System.Windows.Visibility.Collapsed;
                DividerSeCo.Visibility = System.Windows.Visibility.Collapsed;

                // load unread messages number
                UnreadMessagesNumber = 0;
                foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
                {
                    if (account.Value.selected)
                    {
                        switch (account.Value.account.typeAccount)
                        {
                            case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).CountUnreadThreadCalled -= new AccountFacebookLight.OnCountUnreadThreadCompleted(MainPage_CountUnreadThread);
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).CountUnreadThreadCalled += new AccountFacebookLight.OnCountUnreadThreadCompleted(MainPage_CountUnreadThread);
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).CountUnreadThreads();
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                                //((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled -= new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                                break;
                        }
                    }
                }

            }
            else
            {
                LinkHome.Visibility = System.Windows.Visibility.Collapsed;
                DividerHome.Visibility = System.Windows.Visibility.Collapsed;
                LinkFriends.Visibility = System.Windows.Visibility.Collapsed;
                DividerFriends.Visibility = System.Windows.Visibility.Collapsed;
                LinkMessages.Visibility = System.Windows.Visibility.Collapsed;
                DividerMessages.Visibility = System.Windows.Visibility.Collapsed;
                //LinkProfil.Visibility = System.Windows.Visibility.Collapsed;
                //DividerProfil.Visibility = System.Windows.Visibility.Collapsed;
                updateStatusControl.Visibility = System.Windows.Visibility.Collapsed;
                LinkSeDeco.Visibility = System.Windows.Visibility.Collapsed;

                LinkSeConnecter.Visibility = System.Windows.Visibility.Visible;
                DividerSeCo.Visibility = System.Windows.Visibility.Visible;
            }
            //LinkHome.Visibility = System.Windows.Visibility.Visible;
            //DividerHome.Visibility = System.Windows.Visibility.Visible;
            //LinkSeDeco.Visibility = System.Windows.Visibility.Visible;

            LinkHome.NavigateUri = new Uri("/Home?time=" + DateTime.Now.Ticks, UriKind.Relative);
        }

        void MainPage_CountUnreadThread(long count)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                UnreadMessagesNumber += count;
                if (UnreadMessagesNumber > 0)
                    TxtLinkMessages.Text = "Messages (" + UnreadMessagesNumber.ToString() + ")";
            });
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Connexion.Loading(true);

            //bool showLogin = true;
            // if (storage.Contains("groupID"))
            //     if (storage["groupID"].ToString() != "0")
            //     {
            //         showLogin = false;
            //     }
            // if (showLogin)
            // {
            //     Login loginWindow = new Login(false);
            //     loginWindow.Show();
            //     Connexion.Loading(false);
            // }
             //Connexion.Loading(false);
        }

        /* browserSession_LoginCompleted
        private void browserSession_LoginCompleted(object sender, EventArgs e)
        {

        }
        */

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
            LoadInterface();
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
            HtmlPage.Window.Navigate(new Uri("http://"+App.Current.Host.Source.Host + (App.Current.Host.Source.Port != 80?":"+App.Current.Host.Source.Port:null), UriKind.Absolute));
            //ContentFrame.Navigate(new Uri("/Deconnexion", UriKind.Relative));

            /*LinkSeConnecter.Visibility = System.Windows.Visibility.Visible;
            DividerSeCo.Visibility = System.Windows.Visibility.Visible;*/
        }

        private void LinkHome_Click(object sender, RoutedEventArgs e)
        {
           // if (((App)System.Windows.Application.Current)._facebookAPI == null)
            //    LoginFB();
            //LeftFrame.Navigate(new Uri("/Views/MenuFeeds.xaml", UriKind.Relative));
            LinkHome.NavigateUri = new Uri("/Home?time=" + DateTime.Now.Ticks, UriKind.Relative);
        }
        

        private void LinkCreateNewAccount_Click(object sender, RoutedEventArgs e)
        {
            Login loginBox = new Login(true);
            loginBox.Show();
          
        }

        private void LinkSeConnecter_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login(false);
            loginWindow.Show();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Connexion.SilverClick();
        }

    


    }
}
