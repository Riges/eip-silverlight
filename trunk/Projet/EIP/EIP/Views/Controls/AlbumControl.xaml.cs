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
using Facebook.Schema;
using System.Windows.Media.Imaging;
using FlickrNet;

namespace EIP.Views.Controls
{
    public partial class AlbumControl : UserControl
    {
        protected album album { get; set; }
        protected Photoset albumFlickr { get; set; }
        public long accountID { get; set; }
        bool loaded;

        public AlbumControl()
        {
            this.Loaded += new RoutedEventHandler(AlbumView_Loaded);
            InitializeComponent();
        }

        void AlbumView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.loaded)
            {
                if (this.DataContext.GetType() == typeof(album))
                    this.album = this.DataContext as album;
                else if (this.DataContext.GetType() == typeof(Photoset))
                    this.albumFlickr = this.DataContext as Photoset;


                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if (this.album != null)
                    {
                        if (accountLight.Value.account.typeAccount == ServiceEIP.Account.TypeAccount.Facebook)
                        {
                            AccountFacebookLight accountFB = ((AccountFacebookLight)accountLight.Value);
                            if (accountFB.photos.ContainsKey(this.album.aid))
                            {
                                if (accountFB.photos[this.album.aid][this.album.cover_pid] != null)
                                {
                                    imgAlbum.Source = new BitmapImage(new Uri(accountFB.photos[this.album.aid][this.album.cover_pid].src_big, UriKind.Absolute));
                                }
                                this.uri.Content = this.album.name;
                                this.uri.NavigateUri = new Uri("/Album/" + this.album.aid + "/uid/" + this.album.owner + "/Account/" + accountFB.account.accountID, UriKind.Relative);
                            }
                        }
                    }

                    if (this.albumFlickr != null)
                    {
                        if (accountLight.Value.account.typeAccount == ServiceEIP.Account.TypeAccount.Flickr)
                        {
                            AccountFlickrLight accountFK = ((AccountFlickrLight)accountLight.Value);
                            if (accountFK.albums.ContainsKey(this.albumFlickr.OwnerId))
                            {
                                if (this.albumFlickr.PhotosetThumbnailUrl != null && this.albumFlickr.PhotosetThumbnailUrl != "")
                                {
                                    imgAlbum.Source = new BitmapImage(new Uri(this.albumFlickr.PhotosetSquareThumbnailUrl, UriKind.Absolute));
                                }
                                this.uri.Content = this.albumFlickr.Title;
                                this.uri.NavigateUri = new Uri("/Album/" + this.albumFlickr.PhotosetId + "/uid/" + this.albumFlickr.OwnerId + "/Account/" + accountFK.account.accountID, UriKind.Relative);
                            }
                        }
                    }
                }
                this.loaded = true;
            }
            
            
        }
    }
}
