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
using System.IO;
using System.Windows.Media.Imaging;
using Facebook.Schema;

namespace EIP.Views.Controls
{
    public partial class UploadPhotos : ChildWindow
    {
        private long accountID;
        private long uid;
        private string aid;
        private List<UpPhoto> photos;
        

        public UploadPhotos(long laccountID, long luserID, string laid, FileInfo[] files)
        {
            InitializeComponent();

            this.accountID = laccountID;
            this.uid = luserID;
            this.aid = laid;

            LoadAlbums();


            //List<BitmapImage> Images = new List<BitmapImage>();

            List<UpPhoto> Images = new List<UpPhoto>();

            foreach (var fileInfo in files)
            {
               /* using (var fileStream = fileInfo.OpenRead())
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    Images.Add(new  UpPhoto("", bitmapImage));

                    fileStream.Close();
                }*/
                Images.Add(new UpPhoto("", fileInfo));
            }

            PhotosControl.DataContext = Images;
        }

        private void LoadAlbums()
        {
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetAlbumsCalled += new AccountFacebookLight.OnGetAlbumsCompleted(UploadPhotos_GetAlbumsCalled); 
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetAlbums(this.uid);
        }

        void UploadPhotos_GetAlbumsCalled(List<album> albumsResult)
        {
            if (albumsResult != null)
            {
                List<album> albums = new List<album>();


                albums.AddRange(albumsResult);
                albums.Add(new album() { name = "Nouvel Album" });


                comboAlbums.DataContext = albums;
                var al = from a in albums
                            where a.aid == this.aid
                            select a;
                
                comboAlbums.SelectedItem = (album)al.First();

              
                // imgAlbum.Source = new BitmapImage(new Uri(accountFB.photos[this.album.aid][this.album.cover_pid].src_big, UriKind.Absolute));
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            album album = comboAlbums.SelectedItem as album;
            this.photos = PhotosControl.DataContext as List<UpPhoto>;

            if (album.aid == null || album.aid == "")
            {
                ((AccountFacebookLight)Connexion.accounts[this.accountID]).CreateAlbumCalled += new AccountFacebookLight.CreateAlbumCompleted(UploadPhotos_CreateAlbumCalled);
                ((AccountFacebookLight)Connexion.accounts[this.accountID]).CreateAlbum(nameAlbum.Text.Trim(), lieuAlbum.Text.Trim(), descriptionAlbum.Text.Trim());
            }
            else
            {
                LetsUploadPhotos(album);
            }

            //this.DialogResult = true;
        }

        void UploadPhotos_CreateAlbumCalled(album album)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    LetsUploadPhotos(album);
                });
        }

        private void LetsUploadPhotos(album album)
        {
            ProgressUploadPhotos progressUploadPhotos = new ProgressUploadPhotos(this.accountID, this.uid, this.aid, this.photos);

            progressUploadPhotos.Show();
            


            this.DialogResult = true;

            //foreach( UpPhoto photo in this.photos)
            //{
            //    FileInfo file = photo.img;
            //    using (System.IO.Stream str = file.OpenRead())
            //    {
            //        Byte[] bytes = new Byte[str.Length];
            //        str.Read(bytes, 0, bytes.Length);

            //        if (GetFileType(file) != Enums.FileType.jp2)
            //        {
            //            ((AccountFacebookLight)Connexion.accounts[this.accountID]).UploadPhotoCalled += new AccountFacebookLight.UploadPhotoCompleted(UploadPhotos_UploadPhotoCalled);
            //            ((AccountFacebookLight)Connexion.accounts[this.accountID]).UploadPhoto(album.aid, photo.text, bytes, GetFileType(file));
            //        }
            //    }
            //}
        }

       

        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void comboAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            album selectedAlbum = (album)comboAlbums.SelectedItem;
            if (selectedAlbum != null)
            {
                if (selectedAlbum.aid == null || selectedAlbum.aid == "")
                {
                    nameAlbum.IsEnabled = true;
                    lieuAlbum.IsEnabled = true;
                    descriptionAlbum.IsEnabled = true;
                    nameAlbum.Text = "";
                    lieuAlbum.Text = "";
                    descriptionAlbum.Text = "";
                }
                else
                {
                    nameAlbum.Text = selectedAlbum.name;
                    nameAlbum.IsEnabled = false;
                    lieuAlbum.IsEnabled = false;
                    descriptionAlbum.IsEnabled = false;
                }
            }
        }
    }

    public class UpPhoto
    {
        public FileInfo img { get; set; }
        public string text { get; set; }

        public UpPhoto(string unText, FileInfo uneImg)
        {
            this.text = unText;
            this.img = uneImg;
        }
    }
}
