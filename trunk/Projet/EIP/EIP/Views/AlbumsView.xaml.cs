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
using Facebook.Schema;
using EIP.Views.Controls;

namespace EIP.Views
{
    public partial class AlbumsView : Page
    {
        public long uid { get; set; }
        public long accountID { get; set; }
       // protected List<album> albums;

        public static readonly DependencyProperty MyAccountIDProperty = DependencyProperty.Register("MyAccountID", typeof(string), typeof(AlbumsView), new PropertyMetadata(string.Empty));

        public string MyAccountID
        {
            get { return (string)GetValue(MyAccountIDProperty); }
            set { SetValue(MyAccountIDProperty, value); }
        }


        public AlbumsView()
        {
            
            InitializeComponent();
            
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);

            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);

            this.MyAccountID = this.accountID.ToString();

            //App.Current.Resources.Add("accountID", this.accountID);

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
                account.GetAlbumsCalled += new AccountFacebookLight.OnGetAlbumsCompleted(AlbumsView_GetAlbumsCalled);
                account.GetAlbums(this.uid);

            }
        }


        private void AlbumsView_GetAlbumsCalled(List<album> albums)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    flowControl.DataContext = albums;
                });
        }

        /*protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
        }*/

    }
}
