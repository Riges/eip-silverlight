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
using Facebook.Schema;
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class UserInfos : UserControl
    {
        private List<user> mutualFriends;
        private long accountID;
        private long uid;

        public UserInfos()
        {
            InitializeComponent();
        }

        public UserInfos(long accountID, long uid)
        {
            this.accountID = accountID;
            this.uid = uid;

            InitializeComponent();
            LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
            LoadInfos();
        }

        private void LoadInfos()
        {  
            switch (Connexion.accounts[accountID].account.typeAccount)
            {
                case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                    AccountFacebookLight acc = (AccountFacebookLight)Connexion.accounts[accountID];
                    acc.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                    acc.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);

                    //acc.LoadFriendsOfCalled += new AccountFacebookLight.OnLoadFriendsOfCompleted(acc_LoadFriendsOfCalled);
                    //acc.LoadFriendsOf(uid);

                    acc.LoadMutualFriendsCalled += new AccountFacebookLight.OnLoadMutualFriendsCompleted(acc_LoadMutualFriendsCalled);
                    acc.LoadMutualFriends(uid);

                    break;
                case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                    break;
                default:
                    break;
            }
        }



        void acc_GetUserInfoCalled(user monUser)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (monUser != null)
                    {
                        pseudoUser.Text = monUser.name;
                        
                        if (monUser.sex == null)
                        {
                            this.sexLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.sex.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.sex.Text = Utils.FirstLetterUp(monUser.sex);
                            this.sexLabel.Visibility = System.Windows.Visibility.Visible;
                            this.sex.Visibility = System.Windows.Visibility.Visible;
                        }


                        if (monUser.birthday == null)
                        {
                            this.annivLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.anniv.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.anniv.Text = monUser.birthday;
                            this.annivLabel.Visibility = System.Windows.Visibility.Visible;
                            this.anniv.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.meeting_sex == null || monUser.meeting_sex.sex == null || monUser.meeting_sex.sex.Count == 0)
                        {
                            this.interesseLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.interesse.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.interesse.Children.Clear();
                            foreach (string sex in monUser.meeting_sex.sex)
                            {
                                TextBlock txtBlock = new TextBlock();
                                txtBlock.Text = Utils.FirstLetterUp(sex);
                                this.interesse.Children.Add(txtBlock);
                            }
                            this.interesseLabel.Visibility = System.Windows.Visibility.Visible;
                            this.interesse.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.meeting_for == null || monUser.meeting_for.seeking == null || monUser.meeting_for.seeking.Count == 0)
                        {
                            this.rechercheLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.recherche.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.recherche.Children.Clear();
                            foreach (string seek in monUser.meeting_for.seeking)
                            {
                                TextBlock txtBlock = new TextBlock();
                                txtBlock.Text = Utils.FirstLetterUp(seek);
                                this.recherche.Children.Add(txtBlock);
                            }
                            this.rechercheLabel.Visibility = System.Windows.Visibility.Visible;
                            this.recherche.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.current_location == null)
                        {
                            this.villeActuelleLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.villeActuelle.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.villeActuelle.Text = "";
                            if (monUser.current_location.city != null)
                                this.villeActuelle.Text += monUser.current_location.city + "   ";

                            if (monUser.current_location.state != null)
                                this.villeActuelle.Text += monUser.current_location.state + "   ";

                            if (monUser.current_location.country != null)
                                this.villeActuelle.Text += monUser.current_location.country;

                            this.villeActuelleLabel.Visibility = System.Windows.Visibility.Visible;
                            this.villeActuelle.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.hometown_location == null)
                        {
                            this.originaireDeLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.originaireDe.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.originaireDe.Text = "";
                            if(monUser.hometown_location.city != null)
                                this.originaireDe.Text += monUser.hometown_location.city + " ";

                            if(monUser.hometown_location.state != null)
                                this.originaireDe.Text += monUser.hometown_location.state + " ";

                            if(monUser.hometown_location.country != null)
                                this.originaireDe.Text += monUser.hometown_location.country;
                                    

                            this.originaireDeLabel.Visibility = System.Windows.Visibility.Visible;
                            this.originaireDe.Visibility = System.Windows.Visibility.Visible;
                        }


                        if (monUser.political == null)
                        {
                            this.opinionsLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.opinions.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.opinions.Text = monUser.political;
                            this.opinionsLabel.Visibility = System.Windows.Visibility.Visible;
                            this.opinions.Visibility = System.Windows.Visibility.Visible;
                        }

                        
                        if (monUser.religion == null)
                        {
                            this.religionLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.religion.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.religion.Text = monUser.religion;
                            this.religionLabel.Visibility = System.Windows.Visibility.Visible;
                            this.religion.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.about_me == null)
                        {
                            this.bioLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.bio.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.bio.Text = monUser.about_me;
                            this.bioLabel.Visibility = System.Windows.Visibility.Visible;
                            this.bio.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.quotes == null)
                        {
                            this.quoteLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.quote.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.quote.Text = monUser.quotes;
                            this.quoteLabel.Visibility = System.Windows.Visibility.Visible;
                            this.quote.Visibility = System.Windows.Visibility.Visible;
                        }

                        


                        LayoutRoot.Visibility = System.Windows.Visibility.Visible;


                    }


                });
        }

        void acc_LoadMutualFriendsCalled(long uid, List<user> friendsFB)
        {
            this.mutualFriends = friendsFB;
            LoadFriendsToFlowControl(this.mutualFriends);
        }

        private void LoadFriendsToFlowControl(List<user> users)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (users != null)
                    {
                        if (users.Count > 0)
                        {
                            amisCommun.Text = users.Count + " amis en commun";
                            List<Friend> friends = new List<Friend>();
                            foreach (user friend in users)
                            {
                                friends.Add(new Friend() { accountID = this.accountID, userFB = friend });
                            }

                            flowControl.DataContext = friends;
                            return;
                        }
                    }


                    amisCommun.Text = "Pas d'amis en commun";

                });
  
        }

        private void searchFriend_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<user> users = new List<user>();
            if (searchFriend.Text.Trim() != "" && searchFriend.Text.Trim() != "Chercher un ami...")
            {
                var result = from user friend in this.mutualFriends
                             where friend.first_name.ToLower().StartsWith(searchFriend.Text.Trim().ToLower())
                             || friend.last_name.ToLower().StartsWith(searchFriend.Text.Trim().ToLower())
                             select friend;

                users = result.ToList<user>();
                
            }
            else
                users = mutualFriends;

            if (users.Count > 0)
            {
                List<Friend> friends = new List<Friend>();
                foreach (user friend in users)
                {
                    friends.Add(new Friend() { accountID = this.accountID, userFB = friend });
                }
                flowControl.DataContext = friends;
            }
            else
            {
                flowControl.DataContext = null;
            }
        }

        private void searchFriend_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchFriend.Text.Trim() == "Chercher un ami...")
            {
                searchFriend.Text = "";
            }
        }

        private void searchFriend_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchFriend.Text.Trim() == "")
            {
                searchFriend.Text = "Chercher un ami...";
                /*
                List<Friend> friends = new List<Friend>();
                foreach (user friend in this.mutualFriends)
                {
                    friends.Add(new Friend() { accountID = this.accountID, userFB = friend });
                }

                flowControl.DataContext = friends;*/
            }
        }

      

    }
}
