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
//using TweetSharp.Twitter.Model;
using EIP.ServiceEIP;
using EIP.Objects;
//using EIP.ServiceEIP;

namespace EIP.Views
{
    public partial class StreamFeeds : Page
    {
        private List<Topic> topics = new List<Topic>();
        private string filterFB = string.Empty;
       

        //public Dictionary<string, List<Topic>> allTopics = new Dictionary<string, List<Topic>>();

        public StreamFeeds()
        {
            InitializeComponent();

            //scroolView.ScrollToVerticalOffset(scroolView.VerticalOffset + 25);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("filter"))
                this.filterFB = this.NavigationContext.QueryString["filter"];


            Connexion.allTopics = new Dictionary<string, List<Topic>>();
            Connexion.navigationContext = NavigationContext;
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {

                dt_Tick(true, null);
                /*
                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if (accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                dt_Tick(null, null);
                                break;
                            case Account.TypeAccount.Twitter:
                                dt_Tick(null, null);
                                break;
                            case Account.TypeAccount.Myspace:
                                break;
                            default:
                                break;
                        }
                }*/
                Connexion.dt.Stop();
                Connexion.dt = new DispatcherTimer();
                Connexion.dt.Interval = new TimeSpan(0, 0, 0, 30, 000);
                Connexion.dt.Tick += new EventHandler(dt_Tick);
                Connexion.dt.Start();
            }
            else
            {
                ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if(accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                //string filter = string.Empty;

                                /*if (this.NavigationContext.QueryString.ContainsKey("filter"))
                                    filter = this.NavigationContext.QueryString["filter"];*/
                                if (sender.GetType() == typeof(Boolean) && Convert.ToBoolean(sender) == true)
                                    ((AccountFacebookLight)accountLight.Value).LoadFeeds(this.filterFB, this, true);
                                else
                                    ((AccountFacebookLight)accountLight.Value).LoadFeeds(this.filterFB, this, false);
                                break;
                            case Account.TypeAccount.Twitter:
                               
                                if (sender.GetType() == typeof(Boolean) && Convert.ToBoolean(sender) == true)
                                    ((AccountTwitterLight)accountLight.Value).LoadHomeStatuses(this, true);
                                else
                                    ((AccountTwitterLight)accountLight.Value).LoadHomeStatuses(this, false);
                                break;
                            case Account.TypeAccount.Myspace:
                                break;
                            default:
                                break;
                        }
                }
            }
            
        }

        public void LoadContext()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    topics = new List<Topic>();
                    topics.Capacity = topics.Capacity + Connexion.allTopics.Count;
                    foreach (KeyValuePair<string, List<Topic>> item in Connexion.allTopics)
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
