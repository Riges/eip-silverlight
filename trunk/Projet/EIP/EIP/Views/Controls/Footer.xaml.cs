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
using EIP.Objects;
using System.Windows.Browser;

namespace EIP.Views.Controls
{
    public partial class Footer : UserControl
    {
        public Footer()
        {
            InitializeComponent();
            if (Application.Current.InstallState != InstallState.Installed && Application.Current.InstallState != InstallState.Installing)
            {
                installApp.Visibility = System.Windows.Visibility.Visible;
                installAppTiret.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                installApp.Visibility = System.Windows.Visibility.Collapsed;
                installAppTiret.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void webSite_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void contact_Click(object sender, RoutedEventArgs e)
        {

        }

        private void installApp_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.InstallState != InstallState.Installed && Application.Current.InstallState != InstallState.Installing)
            {
                Application.Current.Install();
            }

        }
        
    }
}
