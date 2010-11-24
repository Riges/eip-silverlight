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
using EIP.Objects;
using Facebook.Schema;
using EIP.ServiceEIP;
using EIP.Views.Controls;
using System.Windows.Media.Imaging;
using FlickrNet;

namespace EIP.Views
{
    public partial class FriendList : Page
    {
        public Dictionary<String, Friend> friends;

        public FriendList()
        {
            InitializeComponent();

        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;

            busyIndicator.BusyContent = "Chargement des amis en cours...";
            busyIndicator.IsBusy = true;
            busyIndicator.DisplayAfter = new TimeSpan(200);

            this.Title = "myNETwork - Amis";
            this.friends = new Dictionary<String, Friend>();
            LoadList();

            //LoadDisplay();
            
           
        }


        /// <summary>
        /// methode qui merge les listes de friends
        /// </summary>
        private void LoadList()
        {
            foreach(KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                if(account.Value.selected)
                    switch (account.Value.account.typeAccount)
                    {
                        case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                            ((AccountFacebookLight)account.Value).GetFriendsCalled -= new AccountFacebookLight.OnGetFriendsCompleted(FriendList_GetFriendsCalled);
                            ((AccountFacebookLight)account.Value).GetFriendsCalled += new AccountFacebookLight.OnGetFriendsCompleted(FriendList_GetFriendsCalled);
                            ((AccountFacebookLight)account.Value).LoadFriends();

                            break;
                        case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                            AccountTwitterLight accTW = (AccountTwitterLight)account.Value;
                            accTW.GetFriendsCalled -= new AccountTwitterLight.OnGetFriendsCompleted(accTW_GetFriendsCalled);
                            accTW.GetFriendsCalled += new AccountTwitterLight.OnGetFriendsCompleted(accTW_GetFriendsCalled);
                            accTW.LoadFriends();

                            break;
                        case Account.TypeAccount.Flickr:
                            AccountFlickrLight accFK = (AccountFlickrLight)account.Value;
                            accFK.GetFriendsCalled -= new AccountFlickrLight.OnGetFriendsCompleted(accFK_GetFriendsCalled);
                            accFK.GetFriendsCalled += new AccountFlickrLight.OnGetFriendsCompleted(accFK_GetFriendsCalled);
                            accFK.GetFriends();

                            break;
                        default:
                            break;
                    }
            }
            
        }

        void accTW_GetFriendsCalled(List<TwitterUser> friends, long accountID)
        {
            foreach (TwitterUser toto in friends)
            {
                if (this.friends.Keys.Contains(toto.ScreenName))
                {
                    if (this.friends[toto.ScreenName].userTW == null)
                        this.friends[toto.ScreenName].userTW = toto;
                }
                else
                {
                    Friend titi = new Friend();
                    titi.accountID = accountID;
                    titi.userTW = toto;
                    this.friends.Add(toto.ScreenName, titi);
                }
            }

            this.friends = this.friends.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Dispatcher.BeginInvoke(() =>
            {
                LoadDisplay();
            });
        }

        

        private void FriendList_GetFriendsCalled(List<user> friendsFB, long accountID)
        {
            //((AccountFacebookLight)Connexion.accounts[accountID]).GetFriendsCalled -= FriendList_GetFriendsCalled;

            foreach (user toto in friendsFB)
            {
                string key = toto.name; // (toto.proxied_email != null) ? toto.proxied_email : 

                if (friends.Keys.Contains(key))
                {
                    if (friends[key].userFB == null)
                        friends[key].userFB = toto;
                }
                else
                {
                    Friend titi = new Friend();
                    titi.accountID = accountID;
                    titi.userFB = toto;
                    friends.Add(key, titi);
                }
            }

            this.friends = this.friends.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Dispatcher.BeginInvoke(() =>
                {
                    LoadDisplay();
                });
        }

        void accFK_GetFriendsCalled(FlickrNet.ContactCollection friends, long accountID)
        {
            foreach (Contact toto in friends)
            {
                string key = toto.UserName; // (toto.proxied_email != null) ? toto.proxied_email : 

                if (this.friends.Keys.Contains(key))
                {
                    if (this.friends[key].userFK == null)
                        this.friends[key].userFK = toto;
                }
                else
                {
                    Friend titi = new Friend();
                    titi.accountID = accountID;
                    titi.userFK = toto;
                    this.friends.Add(key, titi);
                }
            }

            this.friends = this.friends.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Dispatcher.BeginInvoke(() =>
            {
                LoadDisplay();
            });
        }

        private void LoadDisplay()
        {
            if (friends.Count == 1)
                nbAmis.Text = "Vous avez un seul ami, il est temps de se sociabiliser !";
            else if (friends.Count > 0)
                nbAmis.Text = "Vous avez " + friends.Count + " Amis";
            else
                nbAmis.Text = "Vous n'avez pas d'amis. Poor little thing...";

            flowControl.DataContext = friends.Values;
            
            //ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
            busyIndicator.IsBusy = false;
            searchBox.Visibility = System.Windows.Visibility.Visible;
            flowControl.Visibility = System.Windows.Visibility.Visible;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dictionary<String, Friend> users = new Dictionary<String, Friend>();
            if (searchBox.Text.Trim() != "" && searchBox.Text.Trim() != "Chercher un ami...")
            {
                string searchTxt = searchBox.Text.Trim().ToLower();
                foreach (KeyValuePair<String, Friend> friend in this.friends)
                {
                    if (friend.Value.userFB != null)
                    {
                        if (friend.Value.userFB.first_name.ToLower().StartsWith(searchTxt) || friend.Value.userFB.last_name.ToLower().StartsWith(searchTxt))
                            users.Add(friend.Key, friend.Value);
                    }
                    else if (friend.Value.userTW != null)
                    {
                        if (friend.Value.userTW.Name.ToLower().StartsWith(searchTxt) || friend.Value.userTW.ScreenName.ToLower().StartsWith(searchTxt))
                            users.Add(friend.Key, friend.Value);
                    }

                }
                nbAmis.Text = users.Count + " Amis";

            }
            else
            {
                users = this.friends;
                nbAmis.Text = "Vous avez " + users.Count + " Amis";
            }

            flowControl.DataContext = users.Values;
            
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Trim() == "Chercher un ami...")
            {
                searchBox.Text = "";
            }
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text.Trim() == "")
            {
                searchBox.Text = "Chercher un ami...";
            }
        }

    }
}
