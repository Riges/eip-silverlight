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

namespace EIP.Views.Controls
{
    public partial class UploadPhotos : ChildWindow
    {
        private long accountID;
        private long uid;
        private string aid;
        

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
                using (var fileStream = fileInfo.OpenRead())
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    Images.Add(new  UpPhoto("", bitmapImage));

                    fileStream.Close();
                }
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




            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;




        }

        private void comboAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            album selectedAlbum = (album)comboAlbums.SelectedItem;

            if (selectedAlbum.aid == null || selectedAlbum.aid == "")
            {
                nameAlbum.IsEnabled = true;
                nameAlbum.Text = "";
            }
            else
            {
                nameAlbum.Text = selectedAlbum.name;
                nameAlbum.IsEnabled = false;
            }
            

        }
    }

    public class UpPhoto
    {
        public BitmapImage img { get; set; }
        public string text { get; set; }

        public UpPhoto(string unText, BitmapImage uneImg)
        {
            this.text = unText;
            this.img = uneImg;
        }
    }
}

