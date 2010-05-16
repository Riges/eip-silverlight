using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using Facebook.Rest;
using Facebook.Session;
using EIP.ServiceEIP;
using System.Collections.Generic;
using Facebook.Schema;
using Facebook.Utility;
using EIP.Views;
using EIP.Objects;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFacebookLight))]
    public class AccountFacebookLight : AccountLight
    {
        public Api facebookAPI { get; set; }
        private BrowserSession browserSession { get; set; }
        public Dictionary<string, List<Topic>> feeds { get; set; }
        public List<user> friends { get; set; }

        //Controls
        private StreamFeeds streamFeeds;

        public AccountFacebookLight()
        {
            this.account = new AccountFacebook();
            this.feeds = new Dictionary<string, List<Topic>>();

            Connexion.dispatcher.BeginInvoke(() =>
                {
                    browserSession = new BrowserSession(Connexion.ApplicationKey);
                    browserSession.LoginCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(browserSession_LoginCompleted);
                });
        }

        public void Login()
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    
                    browserSession.LoggedIn(((AccountFacebook)this.account).sessionKey,
                                                                ((AccountFacebook)this.account).sessionSecret,
                                                                Convert.ToInt32(((AccountFacebook)this.account).sessionExpires),
                                                                this.account.userID);
                });
        }

        void browserSession_LoginCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            facebookAPI = new Api(browserSession);
            Connexion.accounts[this.account.userID] = this;
        }


        public void LoadFriends()
        {
            
            this.facebookAPI.Friends.GetAsync(new Friends.GetFriendsCallback(GetFriendsIDs_Completed), null);
        }

        private void GetFriendsIDs_Completed(IList<long> usersIDs, Object obj, FacebookException ex)
        {
            if (usersIDs != null)
                if (usersIDs.Count > 0)
                 {
                     this.facebookAPI.Users.GetInfoAsync((List<long>)usersIDs, new Users.GetInfoCallback(GetFriends_Completed), null);
                 }
        }

        private void GetFriends_Completed(IList<user> users, Object obj, FacebookException ex)
        {
            if (users != null)
                if (users.Count > 0)
                 {
                     this.friends = users as List<user>;
                 }
        }


        public void LoadFeeds(string filtre, StreamFeeds aStreamFeeds)
        {
            if(aStreamFeeds != null)
            {
                this.streamFeeds = aStreamFeeds;
                LoadStreamFeedsContext(filtre);
            }
        
            this.facebookAPI.Stream.GetAsync(this.account.userID, new List<long>(), DateTime.Now.AddDays(-2), DateTime.Now, 30, filtre, new Stream.GetCallback(GetStreamCompleted), filtre);
        }

        private void GetStreamCompleted(stream_data data, object o, FacebookException ex)
        {
            //string filtre = string.IsNullOrEmpty(o.ToString())?"all":o.ToString();
            //List<Topic> fb_topics = new List<Topic>();
            this.feeds[o.ToString()] = new List<Topic>();
            foreach (stream_post post in data.posts.stream_post)
            {
                this.facebookAPI.Users.GetInfoAsync(post.source_id, new Users.GetInfoCallback(GetStreamUser_Completed), post);
                //fb_topics.Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.userID, post));
            }

        }

        private void GetStreamUser_Completed(IList<user> users, object o, FacebookException ex)
        {
            //stream_post post = (stream_post)o;
            if (users != null)
                if (users.Count > 0)
                {
                    stream_post post = (stream_post)o;
                    TopicFB topicFB = new TopicFB(post, users[0]);
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    dateTime = dateTime.AddSeconds(post.updated_time).AddHours(1);
                    this.feeds[post.filter_key].Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.userID, topicFB));

                    LoadStreamFeedsContext(post.filter_key);
/*
                    if (streamFeeds != null && this.feeds[post.filter_key].Count >= 30)
                    {
                        streamFeeds.allTopics[this.account.userID.ToString()] = this.feeds[post.filter_key];
                        streamFeeds.LoadContext();
                    }*/
                }
        }

        private void LoadStreamFeedsContext(string filtre)
        {
            if (this.feeds.ContainsKey(filtre))
                if (streamFeeds != null && this.feeds[filtre].Count >= 1)
                {
                    streamFeeds.allTopics[this.account.userID.ToString()] = this.feeds[filtre];
                    streamFeeds.LoadContext();
                }
        }

    
    }
}
