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
using Facebook.Schema;
using System.Windows.Media.Imaging;
using EIP.Objects;

namespace EIP.Views
{
    public partial class MenuProfil : Page
    {
        public long uid { get; set; }
        public string uidFlickr { get; set; }
        public long accountID { get; set; }
        public Dictionary<String, Profil> profil;

        public MenuProfil()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);
            if (this.accountID > 0)
            {
                if (Connexion.accounts[this.accountID].account.typeAccount == ServiceEIP.Account.TypeAccount.Flickr)
                {
                    if (this.NavigationContext.QueryString.ContainsKey("uid"))
                        this.uidFlickr = this.NavigationContext.QueryString["uid"];
                }
                else
                {
                    if (this.NavigationContext.QueryString.ContainsKey("uid"))
                        this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);
                }

                if (Connexion.accounts[accountID].account.typeAccount == ServiceEIP.Account.TypeAccount.Twitter)
                {
                    photosBtn.Visibility = System.Windows.Visibility.Collapsed;
                    videosBtn.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (Connexion.accounts[accountID].account.typeAccount == ServiceEIP.Account.TypeAccount.Flickr)
                {
                    wallBtn.Visibility = System.Windows.Visibility.Collapsed;
                    videosBtn.Visibility = System.Windows.Visibility.Collapsed;
                }

                LoadMenuProfil();
            }
        }

        private void LoadMenuProfil()
        {

            switch (Connexion.accounts[accountID].account.typeAccount)
            {
                case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                    AccountFacebookLight accFB = (AccountFacebookLight)Connexion.accounts[accountID];
                    accFB.GetUserInfoCalled -= new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                    accFB.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                    accFB.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);

                    break;
                case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                    AccountTwitterLight accTW = (AccountTwitterLight)Connexion.accounts[accountID];
                    accTW.GetUserInfoCalled -= new AccountTwitterLight.OnGetUserInfoCompleted(accTW_GetUserInfoCalled);
                    accTW.GetUserInfoCalled += new AccountTwitterLight.OnGetUserInfoCompleted(accTW_GetUserInfoCalled);
                    accTW.GetUserInfo(this.uid);
                    break;
                case ServiceEIP.Account.TypeAccount.Flickr:
                    AccountFlickrLight accFK = (AccountFlickrLight)Connexion.accounts[accountID];
                    accFK.GetUserInfoCalled -= new AccountFlickrLight.OnGetUserInfoCompleted(accFK_GetUserInfoCalled);
                    accFK.GetUserInfoCalled += new AccountFlickrLight.OnGetUserInfoCompleted(accFK_GetUserInfoCalled);
                    accFK.GetUserInfo(this.uidFlickr);
                    break;
                default:
                    break;
            }


        }

        void accFK_GetUserInfoCalled(FlickrNet.Person user)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (user != null)
                {
                    this.Title = "myNETwork - Profil de " + user.UserName;
                    photoUser.UriSource = new Uri(user.BuddyIconUrl, UriKind.Absolute);
                }
            });
        }

        void accTW_GetUserInfoCalled(ServiceEIP.TwitterUser user, long accountID, bool isUserAccount)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (user != null)
                {
                    this.Title = "myNETwork - Profil de " + user.ScreenName;
                    photoUser.UriSource = new Uri(user.ProfileImageUrl, UriKind.Absolute);
                }
            });
        }




        void acc_GetUserInfoCalled(user monUser)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (monUser != null)
                {
                    this.Title = "myNETwork - Profil de " + monUser.name;
                    photoUser.UriSource = new Uri(monUser.pic_big, UriKind.Absolute);
                }
            });
        }

        private void infosBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilInfos/" + (this.uid.ToString() != "0" ? this.uid.ToString() : this.uidFlickr) + "/Account/" + this.accountID, UriKind.Relative)); ;
        }

        private void photosBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilPhotos/" + (this.uid.ToString() != "0" ? this.uid.ToString() : this.uidFlickr) + "/Account/" + this.accountID, UriKind.Relative));
        }

        private void videosBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilVideos/" + this.uid + "/Account/" + this.accountID, UriKind.Relative));
        }

        private void wallBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilWall/" + this.uid + "/Account/" + this.accountID, UriKind.Relative));
        }






    }
}
