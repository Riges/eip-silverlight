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

        public AlbumsView()
        {
            //InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);
             

            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if (accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                                
                            case Account.TypeAccount.Facebook:
                                ((AccountFacebookLight)accountLight.Value).GetAlbumsCalled += new AccountFacebookLight.OnGetAlbumsCompleted(AlbumsView_GetAlbumsCalled);
                                ((AccountFacebookLight)accountLight.Value).GetAlbums(accountLight.Value.account.userID);

                                break;
                            case Account.TypeAccount.Twitter:
                                break;
                            default:
                                break;
                        }
                }
            }

        }

        void AlbumsView_GetAlbumsCalled(List<album> albums)
        {

            FlowControl.DataContext = albums;

        }

    }
}
