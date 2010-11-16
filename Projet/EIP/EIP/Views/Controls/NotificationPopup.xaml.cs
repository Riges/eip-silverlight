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
using System.Threading;

namespace EIP.Views.Controls
{
    public partial class NotificationPopup : UserControl
    {
        public NotificationPopup()
        {
            InitializeComponent();
        }

        public NotificationPopup(string content)
        {
            InitializeComponent();
            Content.Text = content;
        }

        public NotificationPopup(string header, string content)
        {
            InitializeComponent();
            Header.Text = header;
            Content.Text = content;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Remove the control from the layout.
            DislayOff();
        }

        private void DislayOff()
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
