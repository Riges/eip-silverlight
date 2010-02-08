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
using Facebook;
using Facebook.Schema;
using Facebook.Rest;
using Facebook.Session;
using Facebook.Utility;
using System.IO.IsolatedStorage;

namespace EIP.Views
{
    public partial class LeftMenu : Page
    {
        private Api facebookAPI;
        private IList<stream_filter> filters;
        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;

        public LeftMenu()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            
            facebookAPI = ((App)System.Windows.Application.Current)._facebookAPI;

            
            this.filters = (storage.Contains("filters-" + facebookAPI.Session.UserId) ? (IList<stream_filter>)storage["filters-" + facebookAPI.Session.UserId] : null);

            string filterStr = (this.NavigationContext.QueryString.ContainsKey("filter")) ? this.NavigationContext.QueryString["filter"] : null;

            if (filterStr == null || filters == null)
                facebookAPI.Stream.GetFiltersAsync(new Stream.GetFiltersCallback(GetFiltersCompleted), null);
            else
                LoadFilters();
            
            
        }

        private void GetFiltersCompleted(IList<stream_filter> filters, object o, FacebookException ex)
        {
            storage["filters-" + facebookAPI.Session.UserId] = filters;
            this.filters = filters;
            LoadFilters();

        }

        private void LoadFilters()
        {
            Dispatcher.BeginInvoke(() =>
            {
                foreach (stream_filter filter in this.filters)
                {
                    //Button btn = new Button() { Content = filter.name, CommandParameter = filter };
                    //btn.Click += new RoutedEventHandler(btn_Click);

                    Controls.FilterButton btn = new Controls.FilterButton();
                    btn.Source = filter;
                    btn.NavigationService = this.NavigationService;
                    LayoutPanel.Children.Add(btn);
                }
            }
           );
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            stream_filter filter = (stream_filter)((Button)sender).CommandParameter;
            this.NavigationService.Navigate(new Uri(string.Format("/Feeds/{0}", filter.filter_key), UriKind.Relative));
        }

    }
}
