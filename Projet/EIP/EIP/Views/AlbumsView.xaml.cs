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
        public string uidFlickr { get; set; }
        public long accountID { get; set; }
       // protected List<album> albums;

        public AlbumsView()
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


                   switch (Connexion.accounts[this.accountID].account.typeAccount)
                   {
                       case Account.TypeAccount.Facebook:
                            AccountFacebookLight accFB = (AccountFacebookLight)Connexion.accounts[this.accountID];
                            if (accFB.selected)
                           {
                               accFB.GetAlbumsCalled -= new AccountFacebookLight.OnGetAlbumsCompleted(AlbumsView_GetAlbumsCalled);
                               accFB.GetAlbumsCalled += new AccountFacebookLight.OnGetAlbumsCompleted(AlbumsView_GetAlbumsCalled);
                               accFB.GetAlbums(this.uid);

                               accFB.GetUserInfoCalled -= new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                               accFB.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                               accFB.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);

                           }
                           break;
                       case Account.TypeAccount.Twitter:
                           break;
                       case Account.TypeAccount.Flickr:
                           AccountFlickrLight accFK = (AccountFlickrLight)Connexion.accounts[this.accountID];

                           accFK.GetAlbumsCalled -= new AccountFlickrLight.OnGetAlbumsCompleted(accFK_GetAlbumsCalled);
                           accFK.GetAlbumsCalled += new AccountFlickrLight.OnGetAlbumsCompleted(accFK_GetAlbumsCalled);
                           accFK.GetAlbums(this.uidFlickr);

                           accFK.GetUserInfoCalled -= new AccountFlickrLight.OnGetUserInfoCompleted(accFK_GetUserInfoCalled);
                           accFK.GetUserInfoCalled += new AccountFlickrLight.OnGetUserInfoCompleted(accFK_GetUserInfoCalled);
                           accFK.GetUserInfo(this.uidFlickr);
                           break;
                       default:
                           break;
                   }
                  
               }
           }
        }

       

        void acc_GetUserInfoCalled(user monUser)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (monUser != null)
                    {
                        this.Title = "Albums photo de " + monUser.name;
                        PseudoUser.Text = monUser.name;
                    }
                });
        }

        void accFK_GetUserInfoCalled(FlickrNet.Person user)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (user != null)
                {
                    this.Title = "Albums photo de " + user.UserName;
                    PseudoUser.Text = user.UserName;
                }
            });
        }


        private void AlbumsView_GetAlbumsCalled(List<album> albums)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    if (albums.Count > 0)
                    {
                        noAlbums.Visibility = System.Windows.Visibility.Collapsed;
                        flowControl.DataContext = albums;
                    }
                    else
                    {
                        noAlbums.Visibility = System.Windows.Visibility.Visible;                        
                    }
                });
        }

        void accFK_GetAlbumsCalled(FlickrNet.PhotosetCollection albums)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                if (albums.Count > 0)
                {
                    noAlbums.Visibility = System.Windows.Visibility.Collapsed;
                    flowControl.DataContext = albums;
                }
                else
                {
                    noAlbums.Visibility = System.Windows.Visibility.Visible;
                }
            });
        }

        /*protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;
        }*/

    }
}
