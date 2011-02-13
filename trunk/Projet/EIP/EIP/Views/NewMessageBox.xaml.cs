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

namespace EIP.Views
{
    public partial class NewMessageBox : Page
    {
        //public Dictionary<String, Friend> friends;
        
        public String boxActive;
        protected List<TwitterUser> followers = new List<TwitterUser>();
        private long waitFollowers;

        public NewMessageBox()
        {
            InitializeComponent();
            
            /*foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        inbox = ((AccountFacebookLight)account.Value).inbox;
                        outbox = ((AccountFacebookLight)account.Value).outbox;
                        break;

                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                    default:
                        break;
                }
            }*/
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            busyIndicator.IsBusy = true;

            long compteurTwitter = 0;
            waitFollowers = 0; 
            cbFollowers.ItemsSource = null;
                
            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                        if (account.Value.selected) {
                            compteurTwitter++;
                            waitFollowers++;
                            ((AccountTwitterLight)account.Value).LoadFollowers();
                            ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).GetFollowersCalled += new AccountTwitterLight.OnGetFollowersCompleted(Messages_GetFollowersCalled);
                        }
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                        break;
                }

                if (compteurTwitter == 0)
                {
                    // Affichage message d'erreur et cachage formulaire
                    MessageDefault.Visibility = System.Windows.Visibility.Visible;
                    tbMessage.Visibility = System.Windows.Visibility.Collapsed;
                    cbFollowers.Visibility = System.Windows.Visibility.Collapsed;
                    busyIndicator.IsBusy = false;
                }
                else
                {
                    MessageDefault.Visibility = System.Windows.Visibility.Collapsed;
                    //busyIndicator.IsBusy = false;
                }

            }
        }

        // l'idéal est de la mettre dans le fichier ou est definie twitterUser mais introuvable, ou est elle ??????
        private class TwitterUserComparer : IEqualityComparer<TwitterUser>
        {
            public bool Equals(TwitterUser s1, TwitterUser s2) { return s1.ScreenName == s2.ScreenName; }
            public int GetHashCode(TwitterUser s) { return 0; }
        }

        private void Messages_GetFollowersCalled(List<TwitterUser> followers, long accountID)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                ((AccountTwitterLight)Connexion.accounts[accountID]).GetFollowersCalled -= new AccountTwitterLight.OnGetFollowersCompleted(Messages_GetFollowersCalled);
                this.followers.AddRange(followers);
                
                if (--waitFollowers == 0)
                {
                    // deboullonage
                    List<TwitterUser> tmp = new List<TwitterUser>();
                    foreach (TwitterUser t in this.followers)
                        if (!tmp.Contains(t, new TwitterUserComparer()))
                            tmp.Add(t);
                    this.followers = tmp;
                    this.followers.Sort(delegate(TwitterUser t1, TwitterUser t2) { return t1.ScreenName.CompareTo(t2.ScreenName); });
                    cbFollowers.ItemsSource = this.followers;
                    busyIndicator.IsBusy = false;
                }
            });
        }

        

        
    }
}
