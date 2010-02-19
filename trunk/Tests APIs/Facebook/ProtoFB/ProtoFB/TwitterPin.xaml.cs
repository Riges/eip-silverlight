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

namespace ProtoFB
{
    public partial class TwitterPin : ChildWindow
    {
        public TwitterPin()
        {
            InitializeComponent();
        }

        public TwitterPin(string pin)
            :this()
        {
            TBoxPin.Text = pin;
        }

        public string tPin
        {
            get;
            set;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.tPin = TBoxPin.Text;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

