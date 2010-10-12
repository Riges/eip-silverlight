﻿using System;
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
using System.IO;
using EIP.Views.Controls;

namespace EIP.Views
{
    public partial class AlbumView : Page
    {
        public string aid { get; set; }
        public long uid { get; set; }
        public long accountID { get; set; }

        public AlbumView()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("aid"))
                this.aid = this.NavigationContext.QueryString["aid"];

            if (this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);

            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);
            /*
            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);
            */

            if (this.uid == Connexion.accounts[this.accountID].account.userID)
            {
                dragText.Visibility = System.Windows.Visibility.Visible;
                this.AllowDrop = true;
            }
            else
            {
                dragText.Visibility = System.Windows.Visibility.Collapsed;
                this.AllowDrop = false;
            }

            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                /*foreach (KeyValuePair<long, AccountLight> acc in Connexion.accounts)
                {
                    if (acc.Value.account.typeAccount == ServiceEIP.Account.TypeAccount.Facebook && acc.Value.selected)
                        this.accountID = acc.Value.account.accountID;
                }*/
                if (this.accountID != 0 && this.accountID > 0)
                {
                    AccountFacebookLight account = (AccountFacebookLight)Connexion.accounts[this.accountID];
                    if (account.selected)
                    {
                        account.GetPhotosCalled += new AccountFacebookLight.OnGetPhotosCompleted(account_GetPhotosCalled);
                        account.GetPhotos(this.aid);

                        account.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                        account.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);
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
                    PseudoUser.Text = monUser.name;
                }
            });
        }

        void account_GetPhotosCalled(bool ok)
        {
            if (ok)
                this.Dispatcher.BeginInvoke(() =>
                {

                    if (((AccountFacebookLight)Connexion.accounts[this.accountID]).photos.ContainsKey(this.aid))
                        if (((AccountFacebookLight)Connexion.accounts[this.accountID]).photos[this.aid].Values.Count > 0)
                        {
                            noPhotos.Visibility = System.Windows.Visibility.Collapsed;
                            flowControl.DataContext = ((AccountFacebookLight)Connexion.accounts[this.accountID]).photos[this.aid].Values;



                            List<album> albums = ((AccountFacebookLight)Connexion.accounts[this.accountID]).albums[this.uid];
                            var al = from a in albums
                                     where a.aid == this.aid
                                     select a;

                            albumName.Text = al.First().name;
                        }
                        else
                        {
                            noPhotos.Visibility = System.Windows.Visibility.Visible;
                        }
                });

            //photo tof = new photo();
           
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;

            var files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

            if (files == null) return;

            UploadPhotos uploadPhotos = new UploadPhotos(this.accountID, this.uid, this.aid, files);
            uploadPhotos.Show();

            //List<BitmapImage> Images = new List<BitmapImage>();
            //foreach (var fileInfo in files)
            //{
            //    using (var fileStream = fileInfo.OpenRead())
            //    {
            //        var bitmapImage = new BitmapImage();
            //        bitmapImage.SetSource(fileStream);
            //        Images.Add(bitmapImage);
             
            //        fileStream.Close();
            //    }
            //}


        }

        

    }
}
