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
using EIP.Objects;
using Facebook.Schema;
using EIP.ServiceEIP;
using EIP.Views.Controls;
using System.Windows.Media.Imaging;

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
                            ((AccountFacebookLight)account.Value).GetFriendsCalled += new AccountFacebookLight.OnGetFriendsCompleted(FriendList_GetFriendsCalled);
                            ((AccountFacebookLight)account.Value).LoadFriends();

                            break;
                        case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                            List<TwitterUser> friendsTW = ((AccountTwitterLight)account.Value).friends;
                            foreach (TwitterUser toto in friendsTW)
                            {
                                if (friends.Keys.Contains(toto.Name))
                                {
                                    if (friends[toto.Name].userTW == null)
                                        friends[toto.Name].userTW = toto;
                                }
                                else
                                {
                                    Friend titi = new Friend();
                                    titi.userTW = toto;
                                    friends.Add(toto.Name, titi);
                                }
                            }
                            break;
                        default:
                            break;
                    }
            }
            
        }

        private void FriendList_GetFriendsCalled(List<user> friendsFB, long accountID)
        {
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

            friends = friends.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            Dispatcher.BeginInvoke(() =>
                {
                    LoadDisplay();
                });
        }

        private void LoadDisplay()
        {
            flowControl.DataContext = friends.Values;
            
            ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
            flowControl.Visibility = System.Windows.Visibility.Visible;

            //foreach (KeyValuePair<String, Friend> poto in friends)
            //{
            //    FriendView friend = new FriendView(poto.Value);

            //    this.Liste.Children.Add(friend);


            //}

        }
    }
}
