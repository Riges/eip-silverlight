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
using System.IO;

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

               if (this.uid == Connexion.accounts[this.accountID].account.userID ||
                   (Connexion.accounts[this.accountID].account.typeAccount == ServiceEIP.Account.TypeAccount.Flickr && ((AccountFlickr)Connexion.accounts[this.accountID].account).userIDstr == this.uidFlickr))
               {
                   dragText.Visibility = System.Windows.Visibility.Visible;
                   this.AllowDrop = true;
               }
               else
               {
                   dragText.Visibility = System.Windows.Visibility.Collapsed;
                   this.AllowDrop = false;
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
                        this.Title = "myNETwork - Albums photo de " + monUser.name;
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
                    this.Title = "myNETwork - Albums photo de " + user.UserName;
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
                        if (this.uid == Connexion.accounts[this.accountID].account.userID)
                        {
                            noAlbums.Text = "Vous n'avez pas d'albums.";
                        }
                        else
                            noAlbums.Text = "Cet utilisateur ne possède pas de photos ou ne souhaite pas que vous y accédiez.";

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
                    if (Connexion.accounts[this.accountID].account.typeAccount == ServiceEIP.Account.TypeAccount.Flickr && ((AccountFlickr)Connexion.accounts[this.accountID].account).userIDstr == this.uidFlickr)
                    {
                        noAlbums.Text = "Vous n'avez pas d'albums.";
                    }
                    else
                        noAlbums.Text = "Cet utilisateur ne possède pas de photos ou ne souhaite pas que vous y accédiez.";
                    noAlbums.Visibility = System.Windows.Visibility.Visible;
                }
            });
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;

            var files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

            if (files == null) return;

            if (Connexion.accounts[this.accountID].account.typeAccount == Account.TypeAccount.Flickr)
            {
                UploadPhotos uploadPhotos = new UploadPhotos(this.accountID, this.uidFlickr, "", files);
                uploadPhotos.Show();
            }
            else
            {
                UploadPhotos uploadPhotos = new UploadPhotos(this.accountID, this.uid, "", files);
                uploadPhotos.Show();
            }
        }
    }
}
