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
    public partial class AlbumView : UserControl
    {
        private album album { get; set; }

        public AlbumView()
        {
            this.Loaded += new RoutedEventHandler(AlbumView_Loaded);
            InitializeComponent();
        }

        void AlbumView_Loaded(object sender, RoutedEventArgs e)
        {
            this.album = this.DataContext as album;
        }
    }
}
