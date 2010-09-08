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
using System.Windows.Media.Imaging;
using Facebook.Schema;

namespace EIP.Views.Controls
{
    public partial class VideoControl : UserControl
    {
        protected video video { get; set; }
        public long accountID { get; set; }
        bool loaded;

        public VideoControl()
        {
            this.Loaded += new RoutedEventHandler(VideoControl_Loaded);
            InitializeComponent();
        }

        void VideoControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.loaded)
            {
                this.video = this.DataContext as video;

                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if (accountLight.Value.account.typeAccount == ServiceEIP.Account.TypeAccount.Facebook)
                    {
                        AccountFacebookLight accountFB = ((AccountFacebookLight)accountLight.Value);
                        if (accountFB.videos.ContainsKey(this.video.vid))
                        {
                            if (accountFB.thumbVideos[this.video.vid] != null)
                            {
                                imgVideo.Source = new BitmapImage(new Uri(accountFB.thumbVideos[this.video.vid], UriKind.Absolute));
                            }
                            this.uri.Content = this.video.title;
                            this.uri.NavigateUri = new Uri("/Video/" + this.video.vid + "/Account/" + accountFB.account.accountID, UriKind.Relative);
                        }
                    }
                }
                this.loaded = true;
            }
        }
    }
}
