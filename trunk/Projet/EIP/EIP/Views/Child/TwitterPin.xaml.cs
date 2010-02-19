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

namespace EIP
{
    public partial class TwitterPin : ChildWindow
    {
        private AccountTwitter _accountTwitter;
        public TwitterPin(AccountTwitter accountTwitter)
        {
            InitializeComponent();
            _accountTwitter = accountTwitter;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            
            _accountTwitter.pin = pinBox.Text.Trim();
            Connexion.AddTwitterAccount(_accountTwitter, Dispatcher);
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

