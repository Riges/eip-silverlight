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
using EIP.ServiceEIP;

namespace EIP.Views
{
    public partial class StreamFeeds : Page
    {
        private Api facebookAPI;
        public StreamFeeds()
        {
            InitializeComponent();

            //scroolView.ScrollToVerticalOffset(scroolView.VerticalOffset + 25);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Connexion.currentAccount != null)
            {
                switch (Connexion.currentAccount.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        facebookAPI = Connexion.facebookAPI;

                        Uri urlSource = System.Windows.Application.Current.Host.Source;
                        string filter = string.Empty;

                        if (this.NavigationContext.QueryString.ContainsKey("filter"))
                            filter = this.NavigationContext.QueryString["filter"];
                        else
                            filter = null;
                        /*FeedsControl.ClearValue(DataContextProperty);
                        FeedsControl.Items.Clear();
                        List<stream_post> test = new List<stream_post>();
                        test.Add(new stream_post() { message = "test" });
                        FeedsControl.DataContext = test;*/
                        facebookAPI.Stream.GetAsync(facebookAPI.Session.UserId, new List<long>(), DateTime.Now.AddDays(-2), DateTime.Now, 30, filter, new Stream.GetCallback(GetStreamCompleted), null);

                        break;
                    case Account.TypeAccount.Twitter:
                        dt_Tick(null, null);
                        if (((AccountTwitter)Connexion.currentAccount).homeStatuses != null)
                        {
                            FeedsControl.DataContext = ((AccountTwitter)Connexion.currentAccount).homeStatuses;
                            ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                            ContentPanel.Visibility = System.Windows.Visibility.Visible;
                        }
                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
                
                DispatcherTimer dt = new DispatcherTimer();
                dt.Interval = new TimeSpan(0, 0, 10, 20, 000);
                dt.Tick += new EventHandler(dt_Tick);
                dt.Start();


            }
           
        }

        void dt_Tick(object sender, EventArgs e)
        {
            switch (Connexion.currentAccount.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    break;
                case Account.TypeAccount.Twitter:
                    //Connexion.TwitterReloadHomeStatuses();
                    ((AccountTwitter)Connexion.currentAccount).LoadHomeStatuses();
                    
                    if (((AccountTwitter)Connexion.currentAccount).homeStatuses != null)
                    {
                        if (FeedsControl.DataContext != null)
                        {
                            TwitterStatus last = ((IEnumerable<TwitterStatus>)FeedsControl.DataContext).First();
                            if (last.Id != ((AccountTwitter)Connexion.currentAccount).homeStatuses.First().Id)
                                FeedsControl.DataContext = ((AccountTwitter)Connexion.currentAccount).homeStatuses;
                        }
                        else
                            FeedsControl.DataContext = ((AccountTwitter)Connexion.currentAccount).homeStatuses;
                        ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                        ContentPanel.Visibility = System.Windows.Visibility.Visible;
                    }
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }
            
        }

        private void GetStreamCompleted(stream_data data, object o, FacebookException ex)
        {
            
            /*DataSource dt = new DataSource();
            foreach (stream_post post in data.posts.stream_post)
                dt.Items.Add(post);*/
            
            Dispatcher.BeginInvoke(() =>
                {
                    FeedsControl.DataContext = data.posts.stream_post;
                    ImgLoad.Visibility = System.Windows.Visibility.Collapsed;
                    ContentPanel.Visibility = System.Windows.Visibility.Visible;
                }
                );
             

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
