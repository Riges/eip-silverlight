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
using EIP.ServiceEIP;

namespace EIP.Views
{
    public partial class Wall : Page
    {
        public long uid { get; set; }
        public long accountID { get; set; }

        public Wall()
        {
            InitializeComponent();
        }

        // S'exécute lorsque l'utilisateur navigue vers cette page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            

            if (this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);

            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);

            this.LoadFeeds();
        }



        private void LoadFeeds()
        {
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                busyIndicator.IsBusy = true;

                AccountLight accountLight = Connexion.accounts[this.accountID];

                switch (accountLight.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:

                        ((AccountFacebookLight)accountLight).LoadWallCalled -= new AccountFacebookLight.OnLoadWallCompleted(Wall_LoadWallCalled);
                        ((AccountFacebookLight)accountLight).LoadWallCalled += new AccountFacebookLight.OnLoadWallCompleted(Wall_LoadWallCalled);

                        ((AccountFacebookLight)accountLight).LoadWall(this.uid);
                        break;
                    case Account.TypeAccount.Twitter:

                        break;
                    default:
                        break;
                }
            }
        }

        private void Wall_LoadWallCalled(long uid, List<Objects.Topic> feeds)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    busyIndicator.IsBusy = false;
                    if (uid == this.uid && feeds != null && feeds.Count > 0)
                    {
                        FeedsControl.DataContext = feeds;
                        noFeeds.Visibility = System.Windows.Visibility.Collapsed;
                        FeedsControl.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        noFeeds.Visibility = System.Windows.Visibility.Visible;
                    }
                });
        }

    }
}
