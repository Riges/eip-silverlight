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

namespace EIP.Views
{
    public partial class LeftMenu : Page
    {
        private Api facebookAPI;
        public LeftMenu()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

     
            facebookAPI = ((App)System.Windows.Application.Current)._facebookAPI;
            facebookAPI.Stream.GetFiltersAsync(new Stream.GetFiltersCallback(GetFiltersCompleted), null);

            
            
        }

        private void GetFiltersCompleted(IList<stream_filter> filters, object o, FacebookException ex)
        {
            Dispatcher.BeginInvoke(() =>{
                foreach(stream_filter filter in filters)
                {
                    Button btn = new Button() { Content = filter.name, CommandParameter = filter };
                    btn.Click += new RoutedEventHandler(btn_Click);
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
