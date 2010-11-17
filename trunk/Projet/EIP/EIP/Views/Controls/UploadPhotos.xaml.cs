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
using System.IO;
using System.Windows.Media.Imaging;
using Facebook.Schema;
using FlickrNet;

namespace EIP.Views.Controls
{
    public partial class UploadPhotos : ChildWindow
    {
        private long accountID;
        private long uid;
        private string uidFlickr;
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

        public UploadPhotos(long laccountID, string luserID, string laid, FileInfo[] files)
        {
            InitializeComponent();

            this.accountID = laccountID;
            this.uidFlickr = luserID;
            this.aid = laid;

            LoadAlbumsFlickr();

            List<UpPhoto> Images = new List<UpPhoto>();

            foreach (var fileInfo in files)
            {
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
                album newAlbum = new album() { name = "Nouvel Album" };
                albums.Add(newAlbum);


                comboAlbums.DataContext = albums;

                if (this.aid == "")
                {
                    comboAlbums.SelectedItem = newAlbum;
                }
                else
                {
                    var al = from a in albums
                             where a.aid == this.aid
                             select a;

                    comboAlbums.SelectedItem = (album)al.First();
                }
                comboAlbums.Visibility = System.Windows.Visibility.Visible;
              
                // imgAlbum.Source = new BitmapImage(new Uri(accountFB.photos[this.album.aid][this.album.cover_pid].src_big, UriKind.Absolute));
            }
            EnableUploadBtn();
        }

        private void LoadAlbumsFlickr()
        {
            ((AccountFlickrLight)Connexion.accounts[this.accountID]).GetAlbumsCalled += new AccountFlickrLight.OnGetAlbumsCompleted(UploadPhotos_GetAlbumsCalled);
            ((AccountFlickrLight)Connexion.accounts[this.accountID]).GetAlbums(this.uidFlickr);
        }

        void UploadPhotos_GetAlbumsCalled(FlickrNet.PhotosetCollection albumsResult)
        {
            if (albumsResult != null && albumsResult.Count > 0)
            {
                List<Photoset> albums = new List<Photoset>();


                albums.AddRange(albumsResult);
                Photoset newAlbum = new Photoset() { Title = "Nouvel Album" };
                albums.Add(newAlbum);

                comboPhotosSets.DataContext = albums;

                if (this.aid == "")
                {
                    comboAlbums.SelectedItem = newAlbum;
                }
                else
                {
                    var al = from a in albums
                             where a.PhotosetId == this.aid
                             select a;

                    comboPhotosSets.SelectedItem = (Photoset)al.First();
                }
                comboPhotosSets.Visibility = System.Windows.Visibility.Visible;
            }
            EnableUploadBtn();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.uid > 0)
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
            }
            else if (this.uidFlickr != null && this.uidFlickr != "")
            {
                Photoset album = comboPhotosSets.SelectedItem as Photoset;
                this.photos = PhotosControl.DataContext as List<UpPhoto>;

                if (album.PhotosetId == null || album.PhotosetId == "")
                {
                    album.Title = nameAlbum.Text.Trim();
                    album.Description = descriptionAlbum.Text.Trim();
                //    ((AccountFlickrLight)Connexion.accounts[this.accountID]).CreateAlbumCalled += new AccountFlickrLight.CreateAlbumCompleted(UploadPhotos_CreateAlbumCalled);
                //    ((AccountFlickrLight)Connexion.accounts[this.accountID]).CreateAlbum(nameAlbum.Text.Trim(), lieuAlbum.Text.Trim(), descriptionAlbum.Text.Trim());
                }
                //else
                //{
                //    LetsUploadPhotos(album);
                //}
                LetsUploadPhotos(album);
            }
        }

        void UploadPhotos_CreateAlbumCalled(Photoset album)
        {
            Dispatcher.BeginInvoke(() =>
            {
                LetsUploadPhotos(album);
            });
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
            this.aid = album.aid;
            ProgressUploadPhotos progressUploadPhotos = new ProgressUploadPhotos(this.accountID, this.uid, this.aid, this.photos);

            progressUploadPhotos.Show();

            this.DialogResult = true;
        }

        private void LetsUploadPhotos(Photoset photoset)
        {

            this.aid = photoset.PhotosetId;
            ProgressUploadPhotos progressUploadPhotos = new ProgressUploadPhotos(this.accountID, this.uidFlickr, this.aid, this.photos, photoset);

            progressUploadPhotos.Show();

            this.DialogResult = true;

        }
        

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void comboAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (comboAlbums.SelectedItem != null && comboAlbums.SelectedItem.GetType() == typeof(album))
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
                else if (comboPhotosSets.SelectedItem != null && comboPhotosSets.SelectedItem.GetType() == typeof(Photoset))
                {
                    Photoset selectedAlbum = (Photoset)comboPhotosSets.SelectedItem;
                    if (selectedAlbum != null)
                    {
                        if (selectedAlbum.PhotosetId == null || selectedAlbum.PhotosetId == "")
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
                            nameAlbum.Text = selectedAlbum.Title;
                            descriptionAlbum.Text = selectedAlbum.Description;
                            nameAlbum.IsEnabled = false;
                            lieuAlbum.IsEnabled = false;
                            descriptionAlbum.IsEnabled = false;
                        }
                    }
                }

            
            EnableUploadBtn();
        }

        private void nameAlbum_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableUploadBtn();
        }

        private void EnableUploadBtn()
        {
            if (comboAlbums.Visibility == System.Windows.Visibility.Visible)
            {
                if (comboAlbums.SelectedItem == null)
                {
                    OKButton.IsEnabled = false;
                }
                else if (comboAlbums.SelectedItem.GetType() == typeof(album))
                {
                    album selectedAlbum = (album)comboAlbums.SelectedItem;
                    if (selectedAlbum.aid == null || selectedAlbum.aid == "")
                    {
                        if (nameAlbum.Text.Trim() != "")
                            OKButton.IsEnabled = true;
                        else
                            OKButton.IsEnabled = false;
                    }
                    else
                    {
                        OKButton.IsEnabled = true;
                    }
                }
            }

            if (comboPhotosSets.Visibility == System.Windows.Visibility.Visible)
            {
                if (comboPhotosSets.SelectedItem == null)
                {
                    OKButton.IsEnabled = false;
                }
                else if (comboPhotosSets.SelectedItem.GetType() == typeof(Photoset))
                {
                    Photoset selectedAlbum = (Photoset)comboPhotosSets.SelectedItem;
                    if (selectedAlbum.PhotosetId == null || selectedAlbum.PhotosetId == "")
                    {
                        if (nameAlbum.Text.Trim() != "")
                            OKButton.IsEnabled = true;
                        else
                            OKButton.IsEnabled = false;
                    }
                    else
                    {
                        OKButton.IsEnabled = true;
                    }
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

