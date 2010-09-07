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

namespace EIP.Views.Controls
{
    public partial class AlbumControl : UserControl
    {
        protected album album { get; set; }
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
                this.album = this.DataContext as album;

                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
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
                this.loaded = true;
            }
            
            
        }
    }
}
