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

namespace EIP.Views.Controls
{
    public partial class UploadPhotos : ChildWindow
    {
        public UploadPhotos(FileInfo[] files)
        {
            InitializeComponent();


            List<BitmapImage> Images = new List<BitmapImage>();
            foreach (var fileInfo in files)
            {
                using (var fileStream = fileInfo.OpenRead())
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(fileStream);
                    Images.Add(bitmapImage);

                    fileStream.Close();
                }
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
    }
}

