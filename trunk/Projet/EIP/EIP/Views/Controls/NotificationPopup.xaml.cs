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
using System.Threading;
using System.Windows.Threading;
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class NotificationPopup : UserControl
    {
        public DispatcherTimer dt = new DispatcherTimer();

        public NotificationPopup()
        {
            InitializeComponent();
        }

        public NotificationPopup(string content)
        {
            InitializeComponent();

            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }
        }

        public NotificationPopup(string content, int time)
        {
            InitializeComponent();

            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }

            dt.Interval = new TimeSpan(0, 0, 0, time, 000);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
            
        }

        public NotificationPopup(string header, string content)
        {
            InitializeComponent();

            Header.Text = header;
            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }
        }

        public NotificationPopup(AccountLight account, string header, string content, int time)
        {
            InitializeComponent();


            if (account != null)
            {
                switch (account.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        AccountFacebookLight accFB = (AccountFacebookLight)account;

                        imgAccount.UriSource = new Uri(accFB.userInfos.pic_square, UriKind.Absolute);
                        userAccountName.Content = accFB.userInfos.name;
                        userAccountName.NavigateUri = new Uri("/ProfilInfos/" + accFB.account.userID + "/Account/" + accFB.account.accountID, UriKind.Relative);
                        barAccount.Background = App.Current.Resources["BgFB"] as SolidColorBrush;
                        barAccount.BorderBrush = App.Current.Resources["BorderFB"] as SolidColorBrush;
                        borderImgAccount.BorderBrush = App.Current.Resources["BorderFB"] as SolidColorBrush;
                        borderNotif.BorderBrush = App.Current.Resources["BorderFB"] as SolidColorBrush;

                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                        AccountTwitterLight accTW = (AccountTwitterLight)account;
                        
                        if (accTW.userInfos != null)
                        {
                            imgAccount.UriSource = new Uri(accTW.userInfos.ProfileImageUrl, UriKind.Absolute);
                            userAccountName.Content = accTW.userInfos.Name;
                        }
                        userAccountName.NavigateUri = new Uri("/ProfilInfos/" + accTW.userInfos.Id + "/Account/" + accTW.account.accountID, UriKind.Relative);
                        barAccount.Background = App.Current.Resources["BgTW"] as SolidColorBrush;
                        barAccount.BorderBrush = App.Current.Resources["BorderTW"] as SolidColorBrush;
                        borderImgAccount.BorderBrush = App.Current.Resources["BorderTW"] as SolidColorBrush;
                        borderNotif.BorderBrush = App.Current.Resources["BorderTW"] as SolidColorBrush;


                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                        break;
                    default:

                        break;
                }
            }


            Header.Text = header;

            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }

            //Content.Text = content;


            dt.Interval = new TimeSpan(0, 0, 0, time, 000);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Remove the control from the layout.
            DislayOff();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            dt.Stop();
            DislayOff();
        }

        private void DislayOff()
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            dt.Stop();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            dt.Start();
        }
    }
}
