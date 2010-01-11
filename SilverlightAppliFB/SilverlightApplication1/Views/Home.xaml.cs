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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Facebook;
using Facebook.Session;
using Facebook.Rest;
using Facebook.Session;
using Facebook.Schema;
using Facebook.Utility;


namespace SilverlightApplication1
{
    public partial class Home : Page
    {

        internal Api _facebookAPI;
        internal BrowserSession _browserSession;
        private const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a"; //temp
        private user _aUser;

        public Home()
        {
            InitializeComponent();
            
            _browserSession = new BrowserSession(ApplicationKey);
            _browserSession.LoginCompleted += browserSession_LoginCompleted;
            
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _browserSession.Login();
        }

        private void GetStatusCompleted(IList<user_status> status, Object state, FacebookException e)
        {
            if (e == null)
            {
                Dispatcher.BeginInvoke(() => textBox1.Text = status[0].message);

            } 
        }

        private void browserSession_LoginCompleted(object sender, EventArgs e)
        {
            
            _facebookAPI = new Api(_browserSession);
            if (_browserSession.UserId != 0)
            {
                Dispatcher.BeginInvoke(() => textBox1.Text = "Connected");
                Dispatcher.BeginInvoke(() => button2.IsEnabled = true);
            }
            else
                Dispatcher.BeginInvoke(() => textBox1.Text = "Error");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _facebookAPI.Status.GetAsync(new Status.GetCallback(GetStatusCompleted), null);

            _facebookAPI.Notifications.GetListAsync(new DateTime(2009, 11, 1, 00,00,00), true, new Notifications.GetListCallback(GetNotificationsCompleted), null);
        }

        private void GetNotificationsCompleted(notification_data notifs, Object state, FacebookException e)
        {
          
            if (e == null)
            {
                foreach (notification notif in notifs.notifications.notification)
    			{
                    _facebookAPI.Users.GetInfoAsync(notif.sender_id, new Users.GetInfoCallback(GetUserInfoCompleted), notif);
    			}
            }
        }

        private void GetUserInfoCompleted(IList<user> users, Object state, FacebookException e)
        {
             if (e == null)
             {
                 notification notif = (notification)state;
                 _aUser = new user();
                 _aUser = users[0];
                 string title = notif.title_text;
                 string msg = notif.body_text;
                 
                 
                 Dispatcher.BeginInvoke(() => NotifListe.Items.Add(notif));

                 //Dispatcher.BeginInvoke(() => textBox1.Text += _aUser.name + " - " + title + " - " + msg + "\n");
             }
        }

        private void NotifListe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = (ListBox)sender;
            notification notif = (notification)list.SelectedItem;
            List<long> notifs = new List<long>();
            notifs.Add(notif.notification_id);
            _facebookAPI.Notifications.MarkReadAsync(notifs, new Notifications.MarkReadCallback(MarkedAsRead), null);
        }

        private void MarkedAsRead(bool unread, Object state, FacebookException e)
        {
            //Dispatcher.BeginInvoke(() => NotifListe.Items.Clear());
            button2_Click(new object(), null);
        }

    }
}