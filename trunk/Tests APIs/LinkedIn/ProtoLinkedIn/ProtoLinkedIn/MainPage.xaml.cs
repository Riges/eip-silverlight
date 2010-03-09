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
using ProtoLinkedIn.ServiceLinkedin;

namespace ProtoLinkedIn
{
    public partial class MainPage : UserControl
    {
        private Service1Client wcf = new Service1Client();

        public MainPage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
           wcf.linkedInConnectCompleted +=new EventHandler<linkedInConnectCompletedEventArgs>(wcf_linkedInConnectCompleted);
           wcf.linkedInConnectAsync();
        }
        static void wcf_linkedInConnectCompleted(object sender, linkedInConnectCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                
            }
        }
    }
}
