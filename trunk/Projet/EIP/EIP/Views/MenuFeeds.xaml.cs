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
using EIP.ServiceEIP;
using System.Windows.Media.Imaging;
using System.Windows.Data;


namespace EIP.Views
{
    public partial class LeftMenu : Page
    {
        //private Api facebookAPI;
        private IList<stream_filter> filters;
        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;

        public LeftMenu()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                if (account.Value.selected)
                {
                    AccordionItem item = new AccordionItem();

                    Image img = new Image();
                    img.Width = 16;
                    switch (account.Value.account.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            img.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.jpg", UriKind.Relative));
                            break;
                        case Account.TypeAccount.Twitter:
                            img.Source = new BitmapImage(new Uri("../../Assets/Images/twitter-icon.png", UriKind.Relative));
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;
                    }
                    StackPanel panelHeader = new StackPanel();
                    panelHeader.Orientation = Orientation.Horizontal;

                    TextBlock textHeader = new TextBlock();
                    textHeader.Text = account.Value.account.name;
                    textHeader.Margin = new Thickness(5, 0, 0, 0);

                    panelHeader.Children.Add(img);
                    panelHeader.Children.Add(textHeader);

                    item.Name = account.Value.account.accountID.ToString();
                    item.Header = panelHeader;

                    menufiltre.Items.Add(item);
                }
            }

            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                if (account.Value.selected)
                {
                    switch (account.Value.account.typeAccount)
                    {
                        case Account.TypeAccount.Facebook:
                            ((AccountFacebookLight)account.Value).LoadFilters(this);
                            break;
                        case Account.TypeAccount.Twitter:
                            LoadFilters(account.Value);
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;
                    }
                }
            }

            /*
            ((AccordionItem)FindName("Pocket Ino")).Content = "toto";

            ((AccordionItem)FindName("Pocketino")).Content = "titi";

            */

            /*
            if (Connexion.currentAccount != null)
                switch (Connexion.currentAccount.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        facebookAPI = Connexion.facebookAPI;

                        this.filters = (storage.Contains("filters-" + facebookAPI.Session.UserId) ? (IList<stream_filter>)storage["filters-" + facebookAPI.Session.UserId] : null);

                        string filterStr = (this.NavigationContext.QueryString.ContainsKey("filter")) ? this.NavigationContext.QueryString["filter"] : null;

                        if (filterStr == null || filters == null)
                            facebookAPI.Stream.GetFiltersAsync(new Stream.GetFiltersCallback(GetFiltersCompleted), null);
                        else
                            LoadFilters();

                        break;
                    case Account.TypeAccount.Twitter:
                        


                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
            */
            
        }

        private void GetFiltersCompleted(IList<stream_filter> filters, object o, FacebookException ex)
        {
            /*
            storage["filters-" + facebookAPI.Session.UserId] = filters;
            this.filters = filters;
            LoadFilters();*/

        }

        public void LoadFilters(AccountLight account)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AccordionItem item = (AccordionItem)FindName(account.account.accountID.ToString());
                StackPanel panelContent = new StackPanel();
                switch (account.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        foreach (stream_filter filter in ((AccountFacebookLight)account).filters)
                        {
                            StackPanel panelFilter = new StackPanel();
                            panelFilter.Orientation = Orientation.Horizontal;
                            Image imgFilter = new Image();
                            Uri uriImg = new Uri("http://localhost:4164/GifHandler.ashx?link=" + filter.icon_url, UriKind.Absolute);
                            imgFilter.Source = new BitmapImage(uriImg);
                            imgFilter.Width = 16;
                            imgFilter.Stretch = Stretch.None;
                            imgFilter.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            imgFilter.FlowDirection = System.Windows.FlowDirection.RightToLeft;
                            imgFilter.MouseLeftButtonUp += new MouseButtonEventHandler(linkFilter_MouseLeftButtonUp);
                            imgFilter.Cursor = Cursors.Hand;
                            imgFilter.DataContext = filter;

                            TextBlock linkFilter = new TextBlock();
                            linkFilter.MouseLeftButtonUp += new MouseButtonEventHandler(linkFilter_MouseLeftButtonUp);
                            linkFilter.DataContext = filter.filter_key;
                            linkFilter.Margin = new Thickness(5, 0, 0, 0);
                            linkFilter.Cursor = Cursors.Hand;

                            Binding binding = new Binding();
                            binding.Source = filter;
                            binding.Path = new PropertyPath("name");
                            linkFilter.SetBinding(TextBlock.TextProperty, binding);

                            panelFilter.Children.Add(imgFilter);
                            panelFilter.Children.Add(linkFilter);
                            panelContent.Children.Add(panelFilter);
                        }
                        break;
                    case Account.TypeAccount.Twitter:
                        foreach (TwitterFilter filter in ((AccountTwitterLight)account).filters)
                        {
                            StackPanel panelFilter = new StackPanel();
                            panelFilter.Orientation = Orientation.Horizontal;
                            

                            TextBlock linkFilter = new TextBlock();
                            linkFilter.MouseLeftButtonUp += new MouseButtonEventHandler(linkFilter_MouseLeftButtonUp);
                            
                            linkFilter.Margin = new Thickness(5, 0, 0, 0);
                            linkFilter.Cursor = Cursors.Hand;

                            linkFilter.DataContext = filter.value;

                            Binding binding = new Binding();
                            binding.Source = filter;
                            binding.Path = new PropertyPath("name");
                            linkFilter.SetBinding(TextBlock.TextProperty, binding);

                           

                            panelFilter.Children.Add(linkFilter);
                            panelContent.Children.Add(panelFilter);
                        }
                        break;
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
                item.Content = panelContent;
            }
           );

            /*
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
             * */
        }

        void linkFilter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*
            stream_filter filter = new stream_filter();
            filter.name = "";

            if(sender.GetType() == typeof(TextBlock))
                filter = (((TextBlock)sender).DataContext) as stream_filter;
            else if(sender.GetType() == typeof(Image))
                filter = (((Image)sender).DataContext) as stream_filter;*/
            string filter = string.Empty;
            if (sender.GetType() == typeof(TextBlock))
                filter = ((TextBlock)sender).DataContext.ToString();
            else if (sender.GetType() == typeof(Image))
                filter = ((Image)sender).DataContext.ToString();
            Connexion.contentFrame.Navigate(new Uri(string.Format("/Feeds/{0}", filter), UriKind.Relative));
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            stream_filter filter = (stream_filter)((Button)sender).CommandParameter;
            this.NavigationService.Navigate(new Uri(string.Format("/Feeds/{0}", filter.filter_key), UriKind.Relative));
        }

    }
}
