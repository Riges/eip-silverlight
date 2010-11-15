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

namespace EIP.Views
{
    public partial class ProfilInfos : Page
    {
        public long uid { get; set; }
        public string uidFlickr { get; set; }
        public long accountID { get; set; }

        public ProfilInfos()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);
            if(this.accountID >0)
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


                UserInfos infosControl = new UserInfos(this.accountID, this.uid, this.uidFlickr);
                LayoutRoot.Children.Add(infosControl);
            }
        }

    }
}
