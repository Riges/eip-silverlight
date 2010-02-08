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
using ImageTools;
using ImageTools.IO.Gif;
using System.IO;
using System.Windows.Resources;
using System.Windows.Navigation;


namespace EIP.Views.Controls
{
    public partial class FilterButton : UserControl
    {
        public FilterButton()
        {
            InitializeComponent();
        }

        public stream_filter  Source
        {
            get { return base.GetValue(SourceProperty) as stream_filter; }
            set 
            { 
                base.SetValue(SourceProperty, value);
                Load();
            }
        }


        private NavigationService ns;
        public NavigationService NavigationService
        {
            get { return ns; }
            set { ns = value; }
        }

        public static readonly DependencyProperty SourceProperty =
                DependencyProperty.Register("SourceProperty", typeof(stream_filter), typeof(FilterButton), null);

        private void Load()
        {
            ImageTools.IO.Decoders.AddDecoder<GifDecoder>();

            //Uri uriImg = new Uri("http://localhost:4164/GifHandler.ashx?link=" + this.Source.icon_url, UriKind.Absolute);

           // iconFilter.Source = new BitmapImage(uriImg);
            nameFilter.Text = this.Source.name;

            
        }

        private void LayoutPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Hand)
                this.Cursor = Cursors.Hand;
            
        }

        private void LayoutPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            LayoutPanel.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void LayoutPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            LayoutPanel.Background = new SolidColorBrush(Colors.White);
        }

        private void LayoutPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            this.NavigationService.Navigate(new Uri(string.Format("/Feeds/{0}", this.Source.filter_key), UriKind.Relative));
        }

 

    }
}
