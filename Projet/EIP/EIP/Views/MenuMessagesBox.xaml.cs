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
using System.Windows.Data;

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
                            img.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.png", UriKind.Relative));
                            break;

                        case Account.TypeAccount.Twitter:
                            break;
                        case Account.TypeAccount.Flickr:
                            break;
                        default:
                            continue;
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
                            this.LoadMessages(account.Value);
                            break;

                        case Account.TypeAccount.Twitter:
                            break;
                        case Account.TypeAccount.Flickr:
                            break;
                        default:
                            break;
                    }
                }
            }
        }



        public void LoadMessages(AccountLight account)
        {
            Dispatcher.BeginInvoke(() =>
            {
                AccordionItem item = (AccordionItem)FindName(account.account.accountID.ToString());
                StackPanel panelContent = new StackPanel();
                switch (account.account.typeAccount)
                {
                    case Account.TypeAccount.Facebook:
                        StackPanel panelFilter = new StackPanel();

                        TextBlock linkFilter = new TextBlock();
                        linkFilter.Margin = new Thickness(5, 0, 0, 0);
                        linkFilter.Cursor = Cursors.Hand;
                        linkFilter.Text = "Boîte de réception";
                        linkFilter.Tag = "inbox";
                        linkFilter.MouseLeftButtonUp += new MouseButtonEventHandler(linkMessage_MouseLeftButtonUp);

                        TextBlock linkFilter2 = new TextBlock();
                        linkFilter2.Margin = new Thickness(5, 0, 0, 0);
                        linkFilter2.Cursor = Cursors.Hand;
                        linkFilter2.Text = "Boîte d'envoi";
                        linkFilter2.Tag = "outbox";
                        
                        linkFilter2.MouseLeftButtonUp += new MouseButtonEventHandler(linkMessage_MouseLeftButtonUp);
                        
                        panelFilter.Children.Add(linkFilter);
                        panelFilter.Children.Add(linkFilter2);
                        panelContent.Children.Add(panelFilter);
                        break;

                    case Account.TypeAccount.Twitter:
                        break;
                    case Account.TypeAccount.Flickr:
                        break;
                    default:
                        break;
                }
                item.Content = panelContent;
            }
           );
        }

        void linkMessage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Connexion.contentFrame.Navigate(new Uri(string.Format("/Messages/{0}", ((TextBlock)sender).Tag), UriKind.Relative));
        }
    }
}
