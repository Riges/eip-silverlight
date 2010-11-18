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
//using TweetSharp.Model;
//using TweetSharp.Twitter.Model;
using EIP.ServiceEIP;
using EIP.Objects;
using EIP.Views.Controls;
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
            App.Current.Host.Content.Resized += new EventHandler(Content_Resized);

            //scroolView.ScrollToVerticalOffset(scroolView.VerticalOffset + 25);
        }

        void Content_Resized(object sender, EventArgs e)
        {
            FeedsControl.MaxHeight = App.Current.Host.Content.ActualHeight - 140;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Connexion.Loading(false);
            busyIndicator.IsBusy = true;
            Connexion.navigationContext = NavigationContext;
            //Connexion.GetFrob();


            FeedsControl.MaxHeight = App.Current.Host.Content.ActualHeight - 140;


            Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;

            if (this.NavigationContext.QueryString.ContainsKey("filter"))
                this.filterFB = this.NavigationContext.QueryString["filter"];


            //Connexion.allTopics = new Dictionary<string, List<Topic>>();
            
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
               // ImgLoad.Visibility = System.Windows.Visibility.Visible;
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
                            case Account.TypeAccount.Flickr:
                                break;
                            default:
                                break;
                        }
                }*/
                Connexion.dt.Stop();
                Connexion.dt = new DispatcherTimer();
                Connexion.dt.Interval = new TimeSpan(0, 0, 1, 00, 000);
                Connexion.dt.Tick += new EventHandler(dt_Tick);
                Connexion.dt.Start();
            }
            else
            {
                busyIndicator.IsBusy = false;
                //ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                bool wait = false;
                bool accountSelected = false;

                foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
                {
                    if(accountLight.Value.selected)
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                accountSelected = true;
                                ((AccountFacebookLight)accountLight.Value).LoadFeedsCalled += new AccountFacebookLight.OnLoadFeedsCompleted(StreamFeeds_LoadFeedsCalled);

                                bool waitFB = false;

                                if (sender.GetType() == typeof(Boolean) && Convert.ToBoolean(sender) == true)
                                    waitFB = !((AccountFacebookLight)accountLight.Value).LoadFeeds(this.filterFB, true);
                                else
                                    waitFB = !((AccountFacebookLight)accountLight.Value).LoadFeeds(this.filterFB, false);
                                if (waitFB)
                                    wait = waitFB;
                                ((AccountFacebookLight)accountLight.Value).GetNotification();
                                break;
                            case Account.TypeAccount.Twitter:
                                accountSelected = true;

                                ((AccountTwitterLight)accountLight.Value).LoadHomeStatusesCalled -= new AccountTwitterLight.OnLoadHomeStatusesCompleted(StreamFeeds_LoadHomeStatusesCalled);
                                ((AccountTwitterLight)accountLight.Value).LoadHomeStatusesCalled += new AccountTwitterLight.OnLoadHomeStatusesCompleted(StreamFeeds_LoadHomeStatusesCalled);
                                bool waitT = false;
                                if (sender.GetType() == typeof(Boolean) && Convert.ToBoolean(sender) == true)
                                    waitT = !((AccountTwitterLight)accountLight.Value).LoadHomeStatuses(true);
                                else
                                    waitT = !((AccountTwitterLight)accountLight.Value).LoadHomeStatuses(false);
                                if (waitT)
                                    wait = waitT;
                                break;
                            case Account.TypeAccount.Flickr:
                                break;
                            default:
                                break;
                        }
                }

                if (!accountSelected)
                {
                    busyIndicator.IsBusy = false;                        
                }

                //if (wait)
                //    busyIndicator.IsBusy = false;
                    //ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
               // else if (sender.GetType() == typeof(Boolean) && Convert.ToBoolean(sender) == true)
                //    busyIndicator.IsBusy = true;
                    //ImgLoad.Visibility = System.Windows.Visibility.Visible;
            }
            
        }

        void StreamFeeds_LoadHomeStatusesCalled()
        {
            LoadContext();
        }

        void StreamFeeds_LoadFeedsCalled()
        {
            LoadContext();
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
                    busyIndicator.IsBusy = false;
                    //ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                    FeedsControl.Visibility = System.Windows.Visibility.Visible;
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
