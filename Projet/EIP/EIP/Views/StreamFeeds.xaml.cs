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
using System.Windows.Navigation;
using Facebook.Rest;
using Facebook.Schema;
using Facebook.Utility;
using System.Collections;
using System.Windows.Browser;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using TweetSharp.Model;
using EIP.ServiceEIP;
using EIP.Objects;
//using EIP.ServiceEIP;

namespace EIP.Views
{
    public partial class StreamFeeds : Page
    {
        private List<Topic> topics = new List<Topic>();

        public Dictionary<string, List<Topic>> allTopics = new Dictionary<string, List<Topic>>();

        public StreamFeeds()
        {
            InitializeComponent();

            //scroolView.ScrollToVerticalOffset(scroolView.VerticalOffset + 25);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Connexion.navigationContext = NavigationContext;
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if (accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                /*Uri urlSource = System.Windows.Application.Current.Host.Source;
                                string filter = string.Empty;

                                if (this.NavigationContext.QueryString.ContainsKey("filter"))
                                    filter = this.NavigationContext.QueryString["filter"];
                                else
                                    filter = null;
                                ((AccountFacebookLight)accountLight.Value).facebookAPI.Stream.GetAsync(accountLight.Value.account.userID, new List<long>(), DateTime.Now.AddDays(-2), DateTime.Now, 30, filter, new Stream.GetCallback(GetStreamCompleted), accountLight.Value);
                                */
                                //Connexion.facebookAPI.Stream.GetAsync(accountLight.Value.account.userID, new List<long>(), DateTime.Now.AddDays(-2), DateTime.Now, 30, filter, new Stream.GetCallback(GetStreamCompleted), accountLight.Value);
                                dt_Tick(null, null);
                                break;
                            case Account.TypeAccount.Twitter:
                                dt_Tick(null, null);


                                /*if (((AccountTwitterLight)accountLight).homeStatuses != null)
                                {
                                    FeedsControl.DataContext = ((AccountTwitterLight)Connexion.currentAccount).homeStatuses;
                                    ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                                    ContentPanel.Visibility = System.Windows.Visibility.Visible;
                                }*/
                                break;
                            case Account.TypeAccount.Myspace:
                                break;
                            default:
                                break;
                        }
                }

                DispatcherTimer dt = new DispatcherTimer();
                dt.Interval = new TimeSpan(0, 0, 0, 30, 000);
                dt.Tick += new EventHandler(dt_Tick);
                dt.Start();


            }
            else
            {
                ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
            }
           
           
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if(accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                string filter = string.Empty;

                                if (this.NavigationContext.QueryString.ContainsKey("filter"))
                                    filter = this.NavigationContext.QueryString["filter"];
                                ((AccountFacebookLight)accountLight.Value).LoadFeeds(filter, this);
                                break;
                            case Account.TypeAccount.Twitter:
                                ((AccountTwitterLight)accountLight.Value).LoadHomeStatuses(this);
                                break;
                            case Account.TypeAccount.Myspace:
                                break;
                            default:
                                break;
                        }
                }
            }
            
        }

        private void GetStreamCompleted(stream_data data, object o, FacebookException ex)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    AccountLight accountLight = (AccountLight)o;

                    List<Topic> fb_topics = new List<Topic>();
                    foreach (stream_post post in data.posts.stream_post)
                    {
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        dateTime = dateTime.AddSeconds(post.updated_time).AddHours(1);
                        //fb_topics.Add(new Topic(dateTime, Account.TypeAccount.Facebook, accountLight.account.userID, post));
                    }

                    allTopics[accountLight.account.userID.ToString()] = fb_topics;

                    LoadContext();
                    

                    //FeedsControl.DataContext = data.posts.stream_post;
                    //ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                    //ContentPanel.Visibility = System.Windows.Visibility.Visible;
                }
                );
        }

        public void LoadContext()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    topics = new List<Topic>();
                    topics.Capacity = topics.Capacity + allTopics.Count;
                    foreach (KeyValuePair<string, List<Topic>> item in allTopics)
                    {
                        
                        topics.AddRange(item.Value);
                    }

                    topics.Sort(delegate(Topic t1, Topic t2) { return t2.date.CompareTo(t1.date); });
                    FeedsControl.DataContext = topics;
                    ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                    ContentPanel.Visibility = System.Windows.Visibility.Visible;
                });
        }

    }



    


    /*
    public class DataSource
    {
        private ObservableCollection<stream_post> items = new ObservableCollection<stream_post>();

        public DataSource()
        {
            for (int i = 0; i < 10; ++i)
                this.items.Add(i);
        }

        public ObservableCollection<stream_post> Items
        {
            get { return this.items; }
            set { this.items = value; }
        }
    }*/
}
