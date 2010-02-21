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
using EIP.ServiceEIP;
using System.Windows.Data;


namespace EIP.Views.Controls
{
    public partial class Feed : UserControl
    {
       //public stream_post post { get; set; }

        public stream_post post { get; set; }
        public TwitterStatus status { get; set; }
      

        private Api facebookAPI;

        public Feed()
        {
            InitializeComponent();
            //
          
            this.Loaded += new RoutedEventHandler(Feed_Loaded);

            picUser.MouseEnter += new MouseEventHandler(picUser_MouseEnter);
            picUser.MouseLeave += new MouseEventHandler(picUser_MouseLeave);

            //SetBinding(DataContextWatcherProperty, new Binding());
        }
        /*
        public static readonly DependencyProperty DataContextWatcherProperty =
            DependencyProperty.Register("DataContextWatcher",
                                        typeof(Object), typeof(Feed),
                                        new PropertyMetadata(DataContextChanged));

        private static void DataContextChanged(object sender,
                                               DependencyPropertyChangedEventArgs e)
        {
            var feed = (Feed)sender;
            // Update the control as needed
        }
        */
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
            if(Connexion.currentAccount != null)
                switch (Connexion.currentAccount.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        facebookAPI = Connexion.facebookAPI;
                        stream_post typeF = new stream_post();
                        if (this.DataContext.GetType() == typeF.GetType())
                        {
                            post = (stream_post)this.DataContext;
                            if (post != null)
                            {
                                facebookAPI.Users.GetInfoAsync(post.source_id, new Users.GetInfoCallback(GetUser_Completed), new object());
                            }
                        }
                        break;
                    case Account.TypeAccount.Twitter:
                        BitmapImage btImg = null;
                        TwitterStatus typeT = new TwitterStatus();
                        if (this.DataContext.GetType() == typeT.GetType())
                        {
                            status = (TwitterStatus)this.DataContext;
                            if (status.User.ProfileImageUrl != null)
                            {
                             Uri uriImg = new Uri(status.User.ProfileImageUrl);
                             btImg = new BitmapImage(uriImg);
                            }
                            picUser.Source = btImg;
                            nameUser.Text = status.User.Name;
                            message.Text = status.Text;
                            dateTimeFeed.Text = Day2Jour(status.CreatedDate) + ", à " + status.CreatedDate.AddHours(1).ToShortTimeString();
                        }

                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }  
            
        }


        private void GetUser_Completed(IList<user> users, object o, FacebookException ex)
        {
            BitmapImage btImg = null;
            if(users != null)
                if (users.Count > 0)
                {
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
            
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday :
                    jour = "Dimanche";
                    break;
                case DayOfWeek.Monday:
                    jour = "Lundi";
                    break;
                case DayOfWeek.Tuesday:
                    jour = "Mardi";
                    break;
                case DayOfWeek.Wednesday:
                    jour = "Mercredi";
                    break;
                case DayOfWeek.Thursday:
                    jour = "Jeudi";
                    break;
                case DayOfWeek.Friday:
                    jour = "Vendredi";
                    break;
                case DayOfWeek.Saturday:
                    jour = "Samedi";
                    break;
            }

            if (date < DateTime.Today.AddDays(-6))
            {
                jour = "Le " + date.Day + " " + GetMonthFr(date.Month);
            }


            return jour;
        }

        private string GetMonthFr(int month)
        {
            string mois = string.Empty;
            switch(month)
            {
                case 1:
                    mois = "Janvier";
                    break;
                case 2:
                    mois = "Février";
                    break;
                case 3:
                    mois = "Mars";
                    break;
                case 4:
                    mois = "Avril";
                    break;
                case 5:
                    mois = "Mai";
                    break;
                case 6:
                    mois = "Juin";
                    break;
                case 7:
                    mois = "Juillet";
                    break;
                case 8:
                    mois = "Août";
                    break;
                case 9:
                    mois = "Septembre";
                    break;
                case 10:
                    mois = "Octobre";
                    break;
                case 11:
                    mois = "Novembre";
                    break;
                case 12:
                    mois = "Décembre";
                    break;
                
            }
            return mois;
        }

    }
}
