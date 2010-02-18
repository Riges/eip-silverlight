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
using Facebook;
using Facebook.Session;
using Facebook.Rest;
using Facebook.Schema;
using Facebook.Utility;
using System.IO.IsolatedStorage;
using System.IO;
using ProtoFB;

namespace ProtoFB
{
    public partial class MainPage : UserControl
    {
        internal Api _facebookAPI;
        internal BrowserSession _browserSession;

        private const string ApplicationKey = "e0c1f6b95b88d23bfc9727e0ea90602a"; //temp
        bool sessionExpires;
        string sessionKey;
        string sessionSecret;
        long userID;

        private string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        private string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

        private IsolatedStorageSettings storage = IsolatedStorageSettings.ApplicationSettings;

        //private user _aUser;

        IList<user> myFriends;

        public MainPage()
        {
            InitializeComponent();

            _browserSession = new BrowserSession(ApplicationKey);
            _browserSession.LoginCompleted += browserSession_LoginCompleted;
            _browserSession.LogoutCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(_browserSession_LogoutCompleted);
        }

        void _browserSession_LogoutCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            BtnSeConnecter.Content = "Se connecter";
            BtnSeConnecter.Click += new RoutedEventHandler(SeConnecterButton_Click);
        }

        private void SeConnecterButton_Click(object sender, RoutedEventArgs e)
        {
            List<Enums.ExtendedPermissions> perms = new List<Enums.ExtendedPermissions>();
            perms.Add(Enums.ExtendedPermissions.offline_access);
            perms.Add(Enums.ExtendedPermissions.publish_stream);
            perms.Add(Enums.ExtendedPermissions.read_stream);
            _browserSession.RequiredPermissions = perms;

            _browserSession.ApplicationKey = ApplicationKey;

            GetSession();
            if (userID != 0)
            {
                _browserSession.LoggedIn(sessionKey, sessionSecret, Convert.ToInt32(sessionExpires), userID);
            }
            else
                _browserSession.Login();    


            ///// Twitter 
            ProtoFB.toto.Service1Client test = new toto.Service1Client();

           // test.LoginTwitterCompleted += new EventHandler<toto.LoginTwitterCompletedEventArgs>(test_LoginTwitterCompleted);
            //test.LoginTwitterAsync("pocketino", "fdsfds");
            test.AuthorizeDesktopCompleted += new EventHandler<toto.AuthorizeDesktopCompletedEventArgs>(test_AuthorizeDesktopCompleted);
            test.AuthorizeDesktopAsync(consumerKey, consumerSecret);
        }

        void test_AuthorizeDesktopCompleted(object sender, toto.AuthorizeDesktopCompletedEventArgs e)
        {
            string token = e.Result;
            TwitterPin twitterPin = new TwitterPin();
            twitterPin.Show();
        }

        void test_LoginTwitterCompleted(object sender, toto.LoginTwitterCompletedEventArgs e)
        {
           
        }



        private void SeDeConnecterButton_Click(object sender, RoutedEventArgs e)
        {
            _browserSession.Logout();
        }

        private void GetSession()
        {
            sessionExpires = (storage.Contains("SessionExpires") ? (bool)storage["SessionExpires"] : true);
            sessionKey = (storage.Contains("SessionKey") ? (string)storage["SessionKey"] : null);
            sessionSecret = (storage.Contains("SessionSecret") ? (string)storage["SessionSecret"] : null);
            userID = (storage.Contains("UserId") ? (long)storage["UserId"] : 0);
        }

        private void SetSession()
        {
            //storage["ApplicationSecret"] = _facebookAPI.Session.ApplicationSecret;
            sessionExpires = _facebookAPI.Session.SessionExpires;
            storage["SessionExpires"] = sessionExpires;

            sessionKey = _facebookAPI.Session.SessionKey;
            storage["SessionKey"] = sessionKey; 

            sessionSecret = _facebookAPI.Session.SessionSecret;
            storage["SessionSecret"] = sessionSecret;

            userID = _facebookAPI.Session.UserId;
            storage["UserId"] = userID;
        }

        private void browserSession_LoginCompleted(object sender, EventArgs e)
        {
            _facebookAPI = new Api(_browserSession);
            SetSession();

            Load();
        }

        private void Load()
        {
            if (_facebookAPI.Session.UserId != 0)
            {
                //_facebookAPI.Auth.RevokeAuthorizationAsync(null, null);

               // BtnSeConnecter.Content = "Se déconnecter";
                //BtnSeConnecter.Click += new RoutedEventHandler(SeDeConnecterButton_Click);
                BtnGetAmis.IsEnabled = true;
                BtnGetMyWall.IsEnabled = true;
            }
            else
            {
               // BtnSeConnecter.Content = "Se connecter";
               // BtnSeConnecter.Click += new RoutedEventHandler(SeConnecterButton_Click);
            }
            //TxtLogged.Visibility = System.Windows.Visibility.Visible;
        }       

        private void BtnGetAmis_Click(object sender, RoutedEventArgs e)
        {
            //_facebookAPI.Friends.GetListsAsync(new Friends.GetListsCallback(GetFriendsListCompleted), null);
            _facebookAPI.Friends.GetAsync(new Friends.GetFriendsCallback(GetFriendsCompleted), null);
        }

        private void GetFriendsListCompleted(IList<friendlist> list, object o, FacebookException ex)
        {
            Dispatcher.BeginInvoke(() => ListBoxFriends.DataContext = list);
            
        }

        private void GetFriendsCompleted(IList<long> friends, object o, FacebookException ex)
        {

            if (ex == null)
            {
                _facebookAPI.Users.GetInfoAsync((List<long>)friends, new Users.GetInfoCallback(GetUserInfoCompleted_FriendList), null);
            }

            //Dispatcher.BeginInvoke(() => ListBoxFriends.DataContext = friends);

        }

        private void GetUserInfoCompleted_FriendList(IList<user> users, object o, FacebookException ex)
        {
            var enumType = typeof(SortUsersBy);
            var fields =  from field in enumType.GetFields()
                          where field.IsLiteral
                          select field.GetValue(null).ToString();

            Dispatcher.BeginInvoke(() => CbBoxSortUsersBy.ItemsSource = fields);


            myFriends = users;
            SortUsers(users, SortUsersBy.FirstName);
            Dispatcher.BeginInvoke(() => ListBoxFriends.ItemsSource = users);
           
        }

        enum SortUsersBy
        {
            FirstName,
            LastName,
            Birthday
        };

        private void SortUsers(IList<user> users, SortUsersBy sortBy)
        {
            user q;
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = i + 1; j < users.Count; j++)
                {
                    switch (sortBy)
                    {
                        case SortUsersBy.FirstName:
                            if (users[i].first_name.CompareTo(users[j].first_name) == 1)
                            {
                                q = users[i];
                                users[i] = users[j];
                                users[j] = q;
                            }
                            break;
                        case SortUsersBy.LastName:
                            if (users[i].last_name.CompareTo(users[j].last_name) == 1)
                            {
                                q = users[i];
                                users[i] = users[j];
                                users[j] = q;
                            }
                            break;
                        case SortUsersBy.Birthday:
                            if (users[i].birthday.CompareTo(users[j].birthday) == 1)
                            {
                                q = users[i];
                                users[i] = users[j];
                                users[j] = q;
                            }
                            break;
                        default:
                            break;
                    }                    
                }
            } 
        }

        private void BtnGetMyWall_Click(object sender, RoutedEventArgs e)
        {

            

            //_facebookAPI.Stream.GetFiltersAsync(new Stream.GetFiltersCallback(GetStreamFiltersCompleted), null);
            _facebookAPI.Stream.GetAsync(_facebookAPI.Session.UserId, new List<long>(), DateTime.Now.AddDays(-2), DateTime.Now, 30, null, new Facebook.Rest.Stream.GetCallback(GetStreamCompleted), null);

        }

        private void GetStreamFiltersCompleted(IList<stream_filter> filters, object o, FacebookException ex)
        {
            
        }

        private void GetStreamCompleted(stream_data data, object o, FacebookException ex)
        {
            Dispatcher.BeginInvoke(() => ListBoxWall.ItemsSource = data.posts.stream_post);
            
        }

        private void CbBoxSortUsersBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //IList<user> users = (IList<user>)ListBoxFriends.DataContext;
            IList<user> users = myFriends;
            SortUsers(users, (SortUsersBy)Enum.Parse(typeof(SortUsersBy), CbBoxSortUsersBy.SelectedItem.ToString(), true));
            
            ListBoxFriends.ItemsSource = users;
           
        }
        
    }
}
