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
using EIP.Views.Controls;
using Facebook.Schema;
using EIP.Objects;

namespace EIP.Views
{
    public partial class VideosView : Page
    {
        public long uid { get; set; }
        public long accountID { get; set; }

        public VideosView()
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


            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> acc in Connexion.accounts)
                {
                    if (acc.Key == this.accountID)
                        acc.Value.selected = true;
                    else
                        acc.Value.selected = false;
                }
                Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.ReadOnly;
                Connexion.listeComptes.Reload();



                AccountFacebookLight account = (AccountFacebookLight)Connexion.accounts[this.accountID];
                if (account.selected)
                {
                    account.GetVideosCalled += new AccountFacebookLight.GetVideosCompleted(account_GetVideosCalled);
                    account.GetVideos(this.uid);

                    account.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                    account.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);

                }
            }
        }

        void account_GetVideosCalled(List<VideoLight> videos, long uid)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                flowControl.DataContext = videos;
            });
        }

        void acc_GetUserInfoCalled(user monUser)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (monUser != null)
                {
                    PseudoUser.Text = monUser.name;
                }
            });
        }




    }
}
