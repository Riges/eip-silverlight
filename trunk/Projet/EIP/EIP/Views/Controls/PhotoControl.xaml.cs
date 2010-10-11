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
    public partial class PhotoControl : UserControl
    {
        protected photo photo { get; set; }
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
                this.photo = this.DataContext as photo;

                imgPhoto.Source = new BitmapImage(new Uri(this.photo.src_big, UriKind.Absolute));
                
                
                this.loaded = true;
            }
        }

        private void imgPhoto_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ContextMenu.IsOpen = false;
            //e.Handled = true;
            //var p = e.GetPosition(null);
            //lastSelectedObject = ((Shape)sender);
            //initMenu = true;
            //foreach (RadioButton rb in stackPopup.Children)
            //    rb.IsChecked = lastSelectedObject.Tag.ToString() == rb.Tag.ToString();
            //initMenu = false;
            //ContextMenu.IsOpen = true;
            //ContextMenu.SetValue(Canvas.LeftProperty, (double)p.X);
            //ContextMenu.SetValue(Canvas.TopProperty, (double)p.Y);
            
        }
    }
}
