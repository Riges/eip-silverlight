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
        private AccountTwitterLight _accountTwitter;
        public TwitterPin(AccountTwitterLight accountTwitter, Uri url)
        {
            InitializeComponent();
            _accountTwitter = accountTwitter;
            link.NavigateUri = url;//new Uri("http://api.twitter.com/oauth/authorize?oauth_token=" + _accountTwitter.token, UriKind.Absolute);
            link.TargetName = "_blank";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            _accountTwitter.pin = pinBox.Text.Trim();
            Connexion.AddTwitterAccount(_accountTwitter);//, Dispatcher
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

