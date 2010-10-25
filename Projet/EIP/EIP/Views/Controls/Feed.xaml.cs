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
//using EIP.ServiceEIP;
using System.Windows.Data;
//using TweetSharp.Model;
using EIP.ServiceEIP;
using EIP.Objects;
using EIP.Views.Controls.Feeds;


namespace EIP.Views.Controls
{
    public partial class Feed : UserControl
    {
       //public stream_post post { get; set; }

        //public stream_post post { get; set; }
        public TopicFB post { get; set; }
        public TwitterStatus status { get; set; }
        private Topic topic;
      
        public Feed()
        {
            InitializeComponent();
            //
          
            this.Loaded += new RoutedEventHandler(Feed_Loaded);

           // picUser.MouseEnter += new MouseEventHandler(picUser_MouseEnter);
            //picUser.MouseLeave += new MouseEventHandler(picUser_MouseLeave);


           


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
            if (Connexion.accounts.Count > 0)
            {
                this.topic = (Topic)this.DataContext;

                

                switch (topic.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        if (topic.fb_post != null && topic.fb_post.userSource != null)
                        {
                            post = topic.fb_post;

                           

                            BitmapImage btImgFB = null;

                            if (post.userSource.pic_square != null) // verif si gif
                            {
                                Uri uriImg = new Uri(post.userSource.pic_square);
                                //btImgFB = new BitmapImage(uriImg);

                                picUser.UriSource = uriImg;
                            }
                            userSource.Content = post.userSource.name;
                            userSource.NavigateUri = new Uri("/ProfilInfos/" + post.userSource.id + "/Account/" + topic.accountID, UriKind.Relative);
                            if (post.userTarget != null)
                            {
                                chevronUserTarget.Visibility = System.Windows.Visibility.Visible;
                                userTarget.Content = post.userTarget.name;
                                userTarget.NavigateUri = new Uri("/ProfilInfos/" + post.userTarget.id + "/Account/" + topic.accountID, UriKind.Relative);
                            }
                            else
                            {
                                chevronUserTarget.Visibility = System.Windows.Visibility.Collapsed;
                            }
                            
                           

                            imgCpt.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.png", UriKind.Relative));
                            //borderFeed.Background = new SolidColorBrush(new Color() {A = 255, R = Convert.ToByte("3B", 16), G = Convert.ToByte("59", 16), B = Convert.ToByte("98", 16) });
                            picUserBorder.BorderBrush = new SolidColorBrush(new Color() {A = 255, R = Convert.ToByte("3B", 16), G = Convert.ToByte("59", 16), B = Convert.ToByte("98", 16) });
                                //<!--#3B5998 -->

                            //LoadMessage(post.post.message);
                            //message.Text = post.post.message;

                            DateTime dateTime = topic.date;// new DateTime(1970, 1, 1, 0, 0, 0, 0);
                            // dateTime = dateTime.AddSeconds(post.post.created_time);//.AddHours(1);
                            dateTimeFeed.Text = Utils.Day2Jour(dateTime) + ", à " + dateTime.ToShortTimeString();

                            if (post.post.attachment.icon != "" && post.post.attachment.icon != null)
                            {
                                imgVia.Source = new BitmapImage(new Uri("http://localhost:4164/GifHandler.ashx?link=" + post.post.attachment.icon, UriKind.Absolute));
                                imgVia.Visibility = System.Windows.Visibility.Visible;
                            }

                            if (post.post.attribution != "" && post.post.attribution != null)
                            {
                                if (!post.post.attribution.StartsWith("via"))
                                {
                                    viaAppliText.Visibility = System.Windows.Visibility.Visible;
                                    viaAppli.Content = post.post.attribution;
                                    viaAppli.TargetName = "_blank";
                                    viaAppli.NavigateUri = new Uri("http://www.facebook.com/apps/application.php?id=" + post.post.app_id, UriKind.Absolute);
                                    viaAppli.Visibility = System.Windows.Visibility.Visible;
                                }
                            }

                            switch (post.post.type)
                            {
                                case "46":
                                    StatutFeed statut = new StatutFeed(post.post.message);
                                    content.Children.Add(statut);
                                    break;
                                case "80":
                                    /*LienFeed lienFeed = new LienFeed(post);
                                    content.Children.Add(lienFeed);
                                    
                                    break;*/
                                case "237":
                                    LienFeed appliFeed = new LienFeed(post);
                                    content.Children.Add(appliFeed);
                                    break;
                                case "128":
                                    VideoFeed videoFeed = new VideoFeed();
                                    content.Children.Add(videoFeed);
                                    break;
                                case "247":
                                    if (post.post.attachment.icon != "")
                                    {
                                        imgVia.Source = new BitmapImage(new Uri("http://localhost:4164/GifHandler.ashx?link=" + post.post.attachment.icon, UriKind.Absolute));
                                        imgVia.Visibility = System.Windows.Visibility.Visible;
                                    }
                                    PhotosFeed photosFeed = new PhotosFeed(post);
                                    content.Children.Add(photosFeed);
                                    //this.Height = 220;
                                    break;
                                default:
                                    TextBlock block = new TextBlock();
                                    block.Text = post.post.type + " - " + post.post.message;
                                    content.Children.Add(block);
                                    break;
                            }

                            jaimeButton.accountID = topic.accountID;
                            jaimeButton.postId = post.post.post_id;
                            jaimeButton.LoadJaimeButton(post.post.likes.user_likes);
                            stream_likes test = post.post.likes;

                            //commentaires
                            // stream_comments stream_coms = topic.fb_post.post.comments;
                            comsControl.profiles = ((AccountFacebookLight)Connexion.accounts[topic.accountID]).profiles;
                            comsControl.postId = post.post.post_id;
                            comsControl.postUserId = (long)post.userSource.id;
                            comsControl.accountID = topic.accountID;
                            comsControl.likes = post.post.likes;
                            comsControl.Commentaires = post.post.comments;
                            comsControl.Width = this.ActualWidth * 0.7;

                            comsControl.LoadComsControl();


                            /*
                            List<profile> profiles = ((AccountFacebookLight)Connexion.accounts[topic.accountID]).profiles;

                            var theProfile = from profile prof in profiles
                                               where prof.id == Convert.ToInt64(coms[0].post_id)
                                               select prof;
                            */
                        }
                        else
                        {
                            this.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        break;
                    case Account.TypeAccount.Twitter:
                        BitmapImage btImg = null;
                        //if (this.DataContext.GetType() == typeof(TwitterStatus))
                        //{
                        if (topic.t_post != null)
                        {
                            status = topic.t_post;// (TwitterStatus)this.DataContext;
                            if (status.User.ProfileImageUrl != null)
                            {
                                Uri uriImg = new Uri(status.User.ProfileImageUrl);
                                //btImg = new BitmapImage(uriImg);

                                picUser.UriSource = uriImg;
                            }
                            userSource.Content = status.User.Name;
                            imgCpt.Source = new BitmapImage(new Uri("../../Assets/Images/twitter-icon.png", UriKind.Relative));
                            //borderFeed.Background = new SolidColorBrush(new Color() {A = 255, R = Convert.ToByte("5E", 16), G = Convert.ToByte("C7", 16), B = Convert.ToByte("E5", 16) });
                            picUserBorder.BorderBrush = new SolidColorBrush(new Color() {A = 255, R = Convert.ToByte("5E", 16), G = Convert.ToByte("C7", 16), B = Convert.ToByte("E5", 16) });
                            //<!--#5ec7e5--> 

                            //LoadMessage(status.Text);
                            StatutFeed statut = new StatutFeed(status.Text);
                            content.Children.Add(statut);
                            //message.Text = status.Text;
                            
                            //dateTimeFeed.Text = Day2Jour(status.CreatedDate.AddHours(2)) + ", à " + status.CreatedDate.AddHours(2).ToShortTimeString();
                            dateTimeFeed.Text = Utils.Day2Jour(topic.date) + ", à " + topic.date.ToShortTimeString();

                            string source = status.Source;
                            if (source != "")
                            {
                                string[] tab = new string[2];
                                if (source != "web")
                                {
                                    source = source.Remove(0, 9);
                                    source = source.Remove(source.Length - 4, 4);
                                    source = source.Replace("\" rel=\"nofollow\">", "|");
                                    tab = source.Split('|');
                                }
                                else
                                {
                                    tab[0] = "http://www.facebook.com";
                                    tab[1] = source;
                                }

                                 

                                viaAppliText.Visibility = System.Windows.Visibility.Visible;
                                viaAppli.Content = tab[1];
                                viaAppli.TargetName = "_blank";
                                viaAppli.NavigateUri = new Uri(tab[0], UriKind.Absolute);
                                viaAppli.Visibility = System.Windows.Visibility.Visible;
                            }

                        }
                        //}

                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
            }
            
        }

        private void picUserBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           
            Connexion.navigationService.Navigate(new Uri("/ProfilInfos/" + post.userSource.id + "/Account/" + topic.accountID, UriKind.Relative));
        }




        /*
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
                            dateTime = dateTime.AddSeconds(post.updated_time).AddHours(1);
                            dateTimeFeed.Text = Day2Jour(dateTime) + ", à " + dateTime.ToShortTimeString();

                        }
                    );
                }
        }*/

        /*
        private void LoadMessage(string msg)
        {
            msg = msg.Replace("\n", " ");
            char[] charTab = new char[1];
            charTab[0] = ' ';
            string[] mots = msg.Split(charTab, StringSplitOptions.RemoveEmptyEntries);

            foreach (string mot in mots)
            {
                if (mot.StartsWith("http://") || mot.StartsWith("https://") || mot.StartsWith("www."))
                {
                    string theMot = mot;
                    if (mot.StartsWith("www."))
                        theMot = "http://" + mot;
                    HyperlinkButton link = new HyperlinkButton();
                    link.NavigateUri = new Uri(theMot, UriKind.Absolute);
                    link.Content = theMot + " ";
                    link.TargetName = "_blank";
                    message.Children.Add(link);

                }
                else if (mot.StartsWith("@"))
                {
                    HyperlinkButton link = new HyperlinkButton();
                    if (mot.EndsWith("!"))
                        link.NavigateUri = new Uri("http://twitter.com/" + mot.Substring(1, mot.Length - 2), UriKind.Absolute);
                    else
                        link.NavigateUri = new Uri("http://twitter.com/" + mot.Substring(1), UriKind.Absolute);
                    link.Content = mot + " ";
                    link.TargetName = "_blank";
                    message.Children.Add(link);

                    
                }
                else if (mot.StartsWith("#"))
                {
                    HyperlinkButton link = new HyperlinkButton();
                    link.NavigateUri = new Uri("http://twitter.com/search?q=" + mot, UriKind.Absolute);
                    link.Content = mot + " ";
                    link.TargetName = "_blank";
                    message.Children.Add(link);
                }
                else
                {
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Text = mot + " ";
                    message.Children.Add(txtBlock);
                }
            }
        }*/

        /*
                    System.Windows.Browser.HtmlElement myFrame = System.Windows.Browser.HtmlPage.Document.GetElementById("ifHtmlContent");
                    if (myFrame != null)
                    {
                        myFrame.SetStyleAttribute("width", "1024");
                        myFrame.SetStyleAttribute("height", "768");
                        myFrame.SetAttribute("src", link.NavigateUri.ToString());
                        myFrame.SetStyleAttribute("left", "0");
                        myFrame.SetStyleAttribute("top", "50");
                        myFrame.SetStyleAttribute("visibility", "visible");
                    }
 * */

       

    }
}
