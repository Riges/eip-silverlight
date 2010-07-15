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
    public partial class AlbumsView : Page
    {
        //public string aid { get; set; }

        public AlbumsView()
        {
            //InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            /*if(this.NavigationContext.QueryString.ContainsKey("aid"))
                this.aid = this.NavigationContext.QueryString["aid"];
             * */

            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if (accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                //((AccountFacebookLight)accountLight.Value).GetAlbums();

                                break;
                            case Account.TypeAccount.Twitter:
                                break;
                            default:
                                break;
                        }
                }
            }

        }

    }
}
