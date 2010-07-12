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
using EIP.Objects;
using Facebook.Schema;
using EIP.ServiceEIP;
using System.Windows.Media.Imaging;

namespace EIP.Views
{
    public partial class MenuMessagesBox : Page
    {
        //public Dictionary<String, Friend> friends;
        //public List<thread> inbox;
        //public List<thread> outbox;
        public String boxActive;

        public MenuMessagesBox()
        {
            InitializeComponent();
            
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("box"))
                this.boxActive = this.NavigationContext.QueryString["box"];

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
                            //((AccountFacebookLight)account.Value).LoadFilters(this);
                            break;
                        case Account.TypeAccount.Twitter:
                            break;
                        case Account.TypeAccount.Myspace:
                            break;
                        default:
                            break;
                    }
                }
            }
        }


/*
        public void LoadFilters(AccountLight account)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AccordionItem item = (AccordionItem)FindName(account.account.accountID.ToString());
                StackPanel panelContent = new StackPanel();
                switch (account.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        /*foreach (stream_filter filter in ((AccountFacebookLight)account).filters)
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
                        }*//*
                        foreach (AccountFacebookLight.MsgFolder folder in 
                        { }
                        break;

                    case Account.TypeAccount.Twitter:
                    case Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
                item.Content = panelContent;
            }
           );
        }*/
    }
}
