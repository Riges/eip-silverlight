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
using Facebook.Rest;
using Facebook.Utility;
using System.Windows.Media.Imaging;
using Facebook.Session;


namespace EIP.Views.Controls
{
    public partial class Feed : UserControl
    {
        public stream_post post { get; set; }

        public object test { get; set; }

        private Api facebookAPI;

        public Feed()
        {
            InitializeComponent();
            facebookAPI = ((App)System.Windows.Application.Current)._facebookAPI;

            test = this.DataContext;

            if (post != null)
            {
                facebookAPI.Users.GetInfoAsync(post.source_id, new Users.GetInfoCallback(GetUser_Completed), null);
            }
            else
            {
                Uri uriImg = new Uri("http://t3.gstatic.com/images?q=tbn:YEeflcdkzQD0tM:http://photodezign.free.fr/wp-content/icone-vector.jpg");
                picUser.Source = new BitmapImage(uriImg);
                nameUser.Text = "Pocket";
                message.Text = "Kikoooo !!!";
            }

        }

        private void GetUser_Completed(IList<user> users, object o, FacebookException ex)
        {
            Uri uriImg = new Uri(users[0].pic_small);
            Dispatcher.BeginInvoke(() =>
                {
                    picUser.Source = new BitmapImage(uriImg);
                    nameUser.Text = users[0].name;
                    message.Text = post.message;
                }
            );
        }
    }
}
