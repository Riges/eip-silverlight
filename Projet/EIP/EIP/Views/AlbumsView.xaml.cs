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

namespace EIP.Views
{
    public partial class AlbumsView : Page
    {
        public long uid { get; set; }
        public long accountID { get; set; }
       // protected List<album> albums;

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
             

            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                //foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                //{
                AccountFacebookLight account = (AccountFacebookLight)Connexion.accounts[this.accountID];
                if (account.selected)
                    //switch (account.account.typeAccount)
                        {
                                
                           // case Account.TypeAccount.Facebook:
                            account.GetAlbumsCalled += new AccountFacebookLight.OnGetAlbumsCompleted(AlbumsView_GetAlbumsCalled);
                            account.GetAlbums(this.uid);
                                //break;
                           // case Account.TypeAccount.Twitter:
                              //  break;
                           // default:
                            //    break;
                        }
                //}
            }

        }

        void AlbumsView_GetAlbumsCalled(List<album> albums)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    flowControl.DataContext = albums;
                    
                });
           

        }

    }
}
