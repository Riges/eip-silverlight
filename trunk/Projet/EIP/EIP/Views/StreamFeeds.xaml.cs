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
            facebookAPI = ((App)System.Windows.Application.Current)._facebookAPI;

            Uri urlSource = System.Windows.Application.Current.Host.Source;

            string filter = string.Empty;
            //Dictionary<string, string> urlparams = HtmlPage.Document.QueryString as Dictionary<string, string>;
            //urlparams.TryGetValue("filter", out filter);

            if (this.NavigationContext.QueryString.ContainsKey("filter"))
                filter = this.NavigationContext.QueryString["filter"];
            else
                filter = null;

            facebookAPI.Stream.GetAsync(facebookAPI.Session.UserId, new List<long>(), DateTime.Now.AddDays(-2), DateTime.Now, 30, filter, new Stream.GetCallback(GetStreamCompleted), null);
        }

        private void GetStreamCompleted(stream_data data, object o, FacebookException ex)
        {
            
            /*DataSource dt = new DataSource();
            foreach (stream_post post in data.posts.stream_post)
                dt.Items.Add(post);*/
            Dispatcher.BeginInvoke(() => FeedsControl.DataContext = data.posts.stream_post);//data.posts.stream_post);

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
