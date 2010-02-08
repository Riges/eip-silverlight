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

      

        private Api facebookAPI;

        public Feed()
        {
            InitializeComponent();
            //


          
            this.Loaded += new RoutedEventHandler(Feed_Loaded);

            picUser.MouseEnter += new MouseEventHandler(picUser_MouseEnter);
            picUser.MouseLeave += new MouseEventHandler(picUser_MouseLeave);
        }

        void picUser_MouseLeave(object sender, MouseEventArgs e)
        {
            //picUser.Projection = new PlaneProjection() { RotationY=-35, CenterOfRotationY=-1};
            QuitImg.Begin();            
        }

        void picUser_MouseEnter(object sender, MouseEventArgs e)
        {
            OnImg.Begin();
        }

        void Feed_Loaded(object sender, RoutedEventArgs e)
        {
            facebookAPI = ((App)System.Windows.Application.Current)._facebookAPI;
            post = (stream_post)this.DataContext;
            if (post != null)
             {
                 facebookAPI.Users.GetInfoAsync(post.source_id, new Users.GetInfoCallback(GetUser_Completed), new object());
             }
            /* else
             {
                 Uri uriImg = new Uri("http://t3.gstatic.com/images?q=tbn:YEeflcdkzQD0tM:http://photodezign.free.fr/wp-content/icone-vector.jpg");
                 picUser.Source = new BitmapImage(uriImg);
                 nameUser.Text = "Pocket";
                 message.Text = "Kikoooo !!!";
             }*/
        }

        private void GetUser_Completed(IList<user> users, object o, FacebookException ex)
        {
            BitmapImage btImg = null;
            
            Dispatcher.BeginInvoke(() =>
                {
                    if (users[0].pic != null)
                    {
                        Uri uriImg = new Uri(users[0].pic);
                        btImg = new BitmapImage(uriImg);
                    }
                    picUser.Source = btImg;
                    nameUser.Text = users[0].name;
                    message.Text = post.message;

                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    dateTime = dateTime.AddSeconds(post.updated_time);
                    dateTimeFeed.Text = Day2Jour(dateTime) + ", à " + dateTime.AddHours(1).ToShortTimeString();
                   
                }
            );
        }

        private string Day2Jour(DateTime date)
        {
            string jour = string.Empty;

            if (date.Date == DateTime.Today)
                return "Aujourd'hui";
            if (date.Date == DateTime.Today.AddDays(-1))
                return "Hier";
            if (date.Date == DateTime.Today.AddDays(1))
                return "Demain";
            if (date.Date == DateTime.Today.AddDays(-2))
                return "Avant-hier";

            switch (date.Day)
            {
                case 1 :
                    return "Dimanche";
                    break;
                case 2:
                    return "Lundi";
                    break;
                case 3:
                    return "Mardi";
                    break;
                case 4:
                    return "Mercredi";
                    break;
                case 5:
                    return "Jeudi";
                    break;
                case 6:
                    return "Vendredi";
                    break;
                case 7:
                    return "Samedi";
                    break;
            }


            return jour;
        }

    }
}
