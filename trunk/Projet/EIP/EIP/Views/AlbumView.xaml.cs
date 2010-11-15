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
using Facebook.Schema;
using System.Windows.Media.Imaging;
using System.IO;
using EIP.Views.Controls;
using FlickrNet;
using EIP.ServiceEIP;

namespace EIP.Views
{
    public partial class AlbumView : Page
    {
        public string aid { get; set; }
        public long uid { get; set; }
        public string uidFlickr { get; set; }
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
                    (Connexion.accounts[this.accountID].account.typeAccount == ServiceEIP.Account.TypeAccount.Flickr && ((AccountFlickr)Connexion.accounts[this.accountID].account).userIDstr == this.uidFlickr  ))
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
                    if (this.accountID != 0 && this.accountID > 0)
                    {
                        switch (Connexion.accounts[this.accountID].account.typeAccount)
                        {
                            case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                                 AccountFacebookLight accFB = (AccountFacebookLight)Connexion.accounts[this.accountID];
                                 if (accFB.selected)
                                 {
                                     accFB.GetPhotosCalled += new AccountFacebookLight.OnGetPhotosCompleted(account_GetPhotosCalled);
                                     accFB.GetPhotos(this.aid);

                                     accFB.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                                     accFB.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);
                                 }
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                                AccountFlickrLight accFK = (AccountFlickrLight)Connexion.accounts[this.accountID];

                                accFK.GetPhotosCalled += new AccountFlickrLight.OnGetPhotosCompleted(accFK_GetPhotosCalled);
                                accFK.GetPhotos(this.aid);

                                accFK.GetUserInfoCalled += new AccountFlickrLight.OnGetUserInfoCompleted(accFK_GetUserInfoCalled);
                                accFK.GetUserInfo(this.uidFlickr);
                     
                                break;
                            default:
                                break;
                        }
                       
                    }

                }
            }
        }

      

      

        void acc_GetUserInfoCalled(user monUser)
        {
            
        }

        void accFK_GetUserInfoCalled(FlickrNet.Person user)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (user != null)
                {
                    this.Title = "Album de " + user.UserName;
                    PseudoUser.Text = user.UserName;
                }
            });
        }

        void account_GetPhotosCalled(bool ok, string aid, Dictionary<string, photo> photos)
        {
            if (ok && aid == this.aid)
                this.Dispatcher.BeginInvoke(() =>
                {

                    //if (((AccountFacebookLight)Connexion.accounts[this.accountID]).photos.ContainsKey(this.aid))
                    if (photos.Count > 0) //((AccountFacebookLight)Connexion.accounts[this.accountID]).photos[this.aid].Values
                    {
                        noPhotos.Visibility = System.Windows.Visibility.Collapsed;
                        flowControl.DataContext = photos.Values;



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

        void accFK_GetPhotosCalled(string aid, FlickrNet.PhotosetPhotoCollection photos)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (aid == this.aid && photos.Count > 0)
                {
                    noPhotos.Visibility = System.Windows.Visibility.Collapsed;
                    flowControl.DataContext = photos;

                    PhotosetCollection albums = ((AccountFlickrLight)Connexion.accounts[this.accountID]).albums[this.uidFlickr];
                    var al = from a in albums
                                where a.PhotosetId == this.aid
                                select a;

                    albumName.Text = al.First().Title;
                }
                else
                    noPhotos.Visibility = System.Windows.Visibility.Visible;

            });
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;

            var files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];

            if (files == null) return;

            if (Connexion.accounts[this.accountID].account.typeAccount == Account.TypeAccount.Flickr)
            {
                UploadPhotos uploadPhotos = new UploadPhotos(this.accountID, this.uidFlickr, this.aid, files);
                uploadPhotos.Show();
            }
            else
            {
                UploadPhotos uploadPhotos = new UploadPhotos(this.accountID, this.uid, this.aid, files);
                uploadPhotos.Show();
            }
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
