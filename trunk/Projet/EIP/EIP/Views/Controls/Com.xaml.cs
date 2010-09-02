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
        public string postId { get; set; }
        public profile profile { get; set; }
        public long accountID { get; set; }
        public long postUserId { get; set; }
        


        public Com()
        {
            InitializeComponent();
        }

        public Com(comment unCom, profile unProfile, long unAccountID, long unPostUserId, string unPostId)
        {
            InitializeComponent();

            this.com = unCom;
            this.profile = unProfile;
            this.accountID = unAccountID;
            this.postUserId = unPostUserId;
            this.postId = unPostId;
            
            
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
            userName.Style = App.Current.Resources["HyperlinkButtonStyle"] as Style;
            userName.Content = profile.name;
            userName.Margin = new Thickness(0, 0, 5, 0);
            content.Children.Add(userName);

            if (com.text != "")
            {
                foreach (UIElement element in Utils.LoadMessage(com.text, Resources))
                    content.Children.Add(element);
            }

            /*
            if (Connexion.accounts[this.accountID].account.userID == com.fromid || Connexion.accounts[this.accountID].account.userID == this.postUserId)
                deleteCom.Visibility = System.Windows.Visibility.Visible;
            else
                deleteCom.Visibility = System.Windows.Visibility.Collapsed;

            */
            


        }

        private void deleteCom_Click(object sender, RoutedEventArgs e)
        {
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).DeleteCom(this.com, postId);
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (Connexion.accounts[this.accountID].account.userID == com.fromid || Connexion.accounts[this.accountID].account.userID == this.postUserId)
                deleteCom.Visibility = System.Windows.Visibility.Visible;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            deleteCom.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
