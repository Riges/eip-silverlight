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
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class Com : UserControl
    {
        public comment com { get; set; }
        public profile profile { get; set; }
        public long accountID { get; set; }


        public Com()
        {
            InitializeComponent();
        }

        public Com(comment unCom, profile unProfile, long unAccountID)
        {
            InitializeComponent();

            this.com = unCom;
            this.profile = unProfile;
            this.accountID = unAccountID;
            
            if (profile.pic_square != null)
            {
                BitmapImage btImgFB = null;
                Uri uriImg = new Uri(profile.pic_square);
                btImgFB = new BitmapImage(uriImg);
                imgUser.Source = btImgFB;
            }


            DateTime dateTime = Utils.DateFromStamp(com.time);
            dateTimeFeed.Text = Utils.Day2Jour(dateTime) + ", à " + dateTime.ToShortTimeString();

            HyperlinkButton userName = new HyperlinkButton();
            userName.Content = profile.name;
            content.Children.Add(userName);

            if (com.text != "")
            {
                foreach (UIElement element in Utils.LoadMessage(com.text))
                    content.Children.Add(element);
            }

            


        }

        private void deleteCom_Click(object sender, RoutedEventArgs e)
        {
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).DeleteCom(this.com);
        }
    }
}
