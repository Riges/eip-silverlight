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

namespace EIP.Views.Controls
{
    public partial class HeaderLinkAlbum : UserControl
    {
        protected album album { get; set; }


        public HeaderLinkAlbum()
        {
            this.Loaded += new RoutedEventHandler(HeaderLinkAlbum_Loaded);
            InitializeComponent();
        }

        void HeaderLinkAlbum_Loaded(object sender, RoutedEventArgs e)
        {
            this.album = this.DataContext as album;

            this.uri.NavigateUri = new Uri("/Album/" + this.album.aid, UriKind.Relative);
        }
    }
}
