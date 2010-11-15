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
    public partial class PhotoControl : UserControl
    {
        protected photo photo { get; set; }
        protected Photo photoFlickr { get; set; }
        public long accountID { get; set; }
        bool loaded;

        public PhotoControl()
        {
            this.Loaded += new RoutedEventHandler(PhotoControl_Loaded);
            InitializeComponent();
        }

        void PhotoControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!this.loaded)
            {
                if (this.DataContext.GetType() == typeof(photo))
                this.photo = this.DataContext as photo;
                else if (this.DataContext.GetType() == typeof(Photo))
                    this.photoFlickr = this.DataContext as Photo;

                if (this.photo != null)
                    imgPhoto.Source = new BitmapImage(new Uri(this.photo.src_big, UriKind.Absolute));
                else if (this.photoFlickr != null)
                    imgPhoto.Source = new BitmapImage(new Uri(this.photoFlickr.LargeUrl, UriKind.Absolute));
 
                this.loaded = true;
            }
        }

        //private void imgPhoto_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    ContextMenu.IsOpen = false;
        //    e.Handled = true;
        //    var p = e.GetPosition(null);
        //    //lastSelectedObject = ((Shape)sender);
        //    //initMenu = true;
        //    //foreach (RadioButton rb in stackPopup.Children)
        //    //    rb.IsChecked = lastSelectedObject.Tag.ToString() == rb.Tag.ToString();
        //    //initMenu = false;
        //    ContextMenu.IsOpen = true;
        //    ContextMenu.SetValue(Canvas.LeftProperty, (double)p.X);
        //    ContextMenu.SetValue(Canvas.TopProperty, (double)p.Y);
            
        //}
    }
}
