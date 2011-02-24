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
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;
using System.ComponentModel;
//using EIP.ServiceEIP;

namespace EIP.Views
{
    public partial class StreamFeeds : Page

    {
        private ObservableCollection<Topic> topics = new ObservableCollection<Topic>();
        private string filterFB = string.Empty;
        bool isDead = false;       

        //public Dictionary<string, List<Topic>> allTopics = new Dictionary<string, List<Topic>>();

        public StreamFeeds()
        {
            this.Unloaded += new RoutedEventHandler(StreamFeeds_Unloaded);

            InitializeComponent();

            App.Current.Host.Content.Resized -= new EventHandler(Content_Resized);
            App.Current.Host.Content.Resized += new EventHandler(Content_Resized);
         

            //scroolView.ScrollToVerticalOffset(scroolView.VerticalOffset + 25);
        }

        void StreamFeeds_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(StreamFeeds_Unloaded);

            foreach (KeyValuePair<long, AccountLight> accountLight in Connexion.accounts)
            {
                switch (accountLight.Value.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        ((AccountFacebookLight)accountLight.Value).LoadFeedsCalled -= new AccountFacebookLight.OnLoadFeedsCompleted(StreamFeeds_LoadFeedsCalled);
                        break;
                    case Account.TypeAccount.Twitter:
                        ((AccountTwitterLight)accountLight.Value).LoadHomeStatusesCalled -= new AccountTwitterLight.OnLoadHomeStatusesCompleted(StreamFeeds_LoadHomeStatusesCalled);
                        break;
                }
              
            }
            isDead = true;
        }

        void Content_Resized(object sender, EventArgs e)
        {
            FeedsControl.MaxHeight = App.Current.Host.Content.ActualHeight - 140;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadFeedsPage();
        }

        private void LoadFeedsPage()
        {
            Connexion.Loading(false);
            busyIndicator.IsBusy = true;
            if(Connexion.navigationContext == null)
                Connexion.navigationContext = NavigationContext;
            //Connexion.GetFrob();


            FeedsControl.MaxHeight = App.Current.Host.Content.ActualHeight - 140;


            Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.Normal;

            if (this.NavigationContext.QueryString.ContainsKey("filter"))
                this.filterFB = this.NavigationContext.QueryString["filter"];


            //Connexion.allTopics = new Dictionary<string, List<Topic>>();

            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                dt_Tick(true, null);

                Connexion.dt.Stop();
                Connexion.dt = new DispatcherTimer();
                Connexion.dt.Interval = new TimeSpan(0, 0, 1, 00, 000);
                Connexion.dt.Tick += new EventHandler(dt_Tick);
                Connexion.dt.Start();
            }
            else
            {
                busyIndicator.IsBusy = false;
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
                    if (accountLight.Value.selected)
                    {
                        switch (accountLight.Value.account.typeAccount)
                        {
                            case Account.TypeAccount.Facebook:
                                accountSelected = true;
                                ((AccountFacebookLight)accountLight.Value).LoadFeedsCalled -= new AccountFacebookLight.OnLoadFeedsCompleted(StreamFeeds_LoadFeedsCalled);
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
                    else
                    {
                        Connexion.allTopics.Remove(accountLight.Value.account.userID.ToString());
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

        public static readonly DependencyProperty CustomerListProperty = DependencyProperty.Register("topics", typeof(ObservableCollection<Topic>), typeof(StreamFeeds), new PropertyMetadata(new ObservableCollection<Topic>()));


        public void LoadContext()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (!isDead)
                    {
                        
                        topics = new ObservableCollection<Topic>();
                        //topics..Capacity = topics.Capacity + Connexion.allTopics.Count;
                        foreach (KeyValuePair<string, List<Topic>> item in Connexion.allTopics)
                        {
                            //topics.AddRange(item.Value);
                            foreach (Topic topic in item.Value)
                            {
                                topics.Add(topic);
                            }

                        }

                        //topics.Sort(delegate(Topic t1, Topic t2) { return t2.date.CompareTo(t1.date); });

                        //if (FeedsControl.DataContext == null)
                            FeedsControl.DataContext = topics;
                       //else
                            //SetValue(CustomerListProperty, topics);
                            //OnPropertyChanged("topics");


                        busyIndicator.IsBusy = false;
                        //ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                        FeedsControl.Visibility = System.Windows.Visibility.Visible;
                    }
                }); 
        }

        /*
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } */

       

        

    }


    public class Article
	   {
	       private string _titre;
	       private string _lien;
	       private string _contenu;
	 
	       public string Contenu
	       {
	           get { return _contenu; }
	           set { _contenu = value; }
	       }
	 
	       public Article()
	       {
	       }
	 
	       public string Lien
	       {
	           get { return _lien; }
	           set { _lien = value; }
	       }
	 
	       public string Titre
	       {
	           get { return _titre; }
	           set { _titre = value; }
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
