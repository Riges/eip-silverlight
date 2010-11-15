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
using Facebook.Schema;
using System.Windows.Media.Imaging;
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class ProgressUploadPhotos : ChildWindow
    {
        private long accountID;
        private long uid;
        private string uidFlickr;
        private string aid;
        private List<UpPhoto> photos;

        int i = 0;

        public ProgressUploadPhotos(long laccountID, long luserID, string laid, List<UpPhoto> lesPhotos)
        {
            InitializeComponent();

            this.accountID = laccountID;
            this.uid = luserID;
            this.aid = laid;
            this.photos = lesPhotos;

            LetsUploadPhotos();
        }

        public ProgressUploadPhotos(long laccountID, string luserID, string laid, List<UpPhoto> lesPhotos)
        {
            InitializeComponent();

            this.accountID = laccountID;
            this.uidFlickr = luserID;
            this.aid = laid;
            this.photos = lesPhotos;

            LetsUploadPhotos();
        }

        private void LetsUploadPhotos()
        {
            SendPhoto();
        }

        private void SendPhoto()
        {
            if (i < this.photos.Count)
            {
                UpPhoto photo = this.photos[i];
                FileInfo file = photo.img;
                BitmapImage btImg = new BitmapImage();
                using (System.IO.Stream str = file.OpenRead())
                {
                     btImg.SetSource(str);
                     str.Close();
                }

                if (this.uid > 0)
                {

                    using (System.IO.Stream str = file.OpenRead())
                    {

                        // btImg.SetSource(str);
                        Byte[] bytes = new Byte[str.Length];
                        str.Read(bytes, 0, bytes.Length);

                        str.Close();
                        if (Utils.GetFileType(file) != Enums.FileType.jp2)
                        {
                            sendPhotoImg.Source = btImg;
                            sendPhotoText.Text = "Upload de la photo " + ++i + " / " + this.photos.Count;
                            ((AccountFacebookLight)Connexion.accounts[this.accountID]).UploadPhotoCalled += new AccountFacebookLight.UploadPhotoCompleted(UploadPhotos_UploadPhotoCalled);
                            ((AccountFacebookLight)Connexion.accounts[this.accountID]).UploadPhoto(this.aid, photo.text, bytes, Utils.GetFileType(file));
                        }
                    }
                }
                else if (this.uidFlickr != null && this.uidFlickr != "")
                {
                    using (System.IO.Stream str = file.OpenRead())
                    {
                        if (Utils.GetFileType(file) != Enums.FileType.jp2)
                        {
                            sendPhotoImg.Source = btImg;
                            sendPhotoText.Text = "Upload de la photo " + ++i + " / " + this.photos.Count;
                            ((AccountFlickrLight)Connexion.accounts[this.accountID]).UploadPhotoCalled += new AccountFlickrLight.UploadPhotoCompleted(ProgressUploadPhotos_UploadPhotoCalled);
                            ((AccountFlickrLight)Connexion.accounts[this.accountID]).UploadPhoto(this.aid, photo.text, str, file.Name);
                        }
                        str.Close();
                    }
                }
                //i++;
            }
        }

        
        void UploadPhotos_UploadPhotoCalled(photo photo)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (i < this.photos.Count)
                {
                    SendPhoto();
                }
                else
                {
                    ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetPhotos(aid);
                    this.DialogResult = true;
                }
            });

        }

        void ProgressUploadPhotos_UploadPhotoCalled()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (i < this.photos.Count)
                {
                    SendPhoto();
                }
                else
                {
                    ((AccountFlickrLight)Connexion.accounts[this.accountID]).GetPhotos(aid);
                    this.DialogResult = true;
                }
            });
        }
       

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

