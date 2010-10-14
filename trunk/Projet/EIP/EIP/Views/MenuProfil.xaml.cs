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
        public long accountID { get; set; }
        public Dictionary<String, Profil> profil;

        public MenuProfil()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);

            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);

            /*EIP.Views.ProfilPage.Tab tab = EIP.Views.ProfilPage.Tab.Mur;
            if (this.NavigationContext.QueryString.ContainsKey("tab"))
                Enum.TryParse<EIP.
             * Views.ProfilPage.Tab>(this.NavigationContext.QueryString["tab"].ToString(), out tab);
            */

            

            LoadMenuProfil();
        }

        private void LoadMenuProfil()
        {

            switch (Connexion.accounts[accountID].account.typeAccount)
            {
                case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                    AccountFacebookLight acc = (AccountFacebookLight)Connexion.accounts[accountID];
                    acc.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                    acc.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);

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
                    this.Title = "Profil de " + monUser.name;
                    photoUser.UriSource = new Uri(monUser.pic_big, UriKind.Absolute);
                }
            });
        }

        private void infosBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilInfos/" + this.uid + "/Account/" + this.accountID, UriKind.Relative));          
        }

        private void photosBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilPhotos/" + this.uid + "/Account/" + this.accountID, UriKind.Relative));
        }

        private void videosBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ProfilVideos/" + this.uid + "/Account/" + this.accountID, UriKind.Relative));
        }






    }
}
