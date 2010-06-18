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
        public List<stream_filter> filters { get; set; }

        private bool busy = false;
        //private int nbFeeds = 0;

        //Controls
        private StreamFeeds streamFeeds;
        private LeftMenu menuFeeds;

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

        public void sendStatu(string statu)
        {
            this.facebookAPI.Status.SetAsync(statu, SetStatusCompleted, null);
        }

        void SetStatusCompleted(bool result, Object state, FacebookException e)
        {
            if (e == null)
            {
                if (result == false)
                {
                    
                }
            }
            else
            {
                
            }
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

        /// <summary>
        /// Méthode pour charger la liste des feeds correspondant au filtre passé en paramètre.
        /// </summary>
        /// <param name="filtre">Filtre de la liste des feeds.</param>
        /// <param name="aStreamFeeds">object StreamFeeds permettant de vérifier s'il y a de nouveau feeds pour mettre à jour ou non la liste.</param>
        public void LoadFeeds(string filtre, StreamFeeds aStreamFeeds, bool first)
        {
            if(aStreamFeeds != null)
            {
                this.streamFeeds = aStreamFeeds;
                if (first)
                    LoadStreamFeedsContext(filtre);
            }
            if(!busy)
                this.facebookAPI.Stream.GetAsync(this.account.userID, new List<long>(), null, null, 30, filtre, new Stream.GetCallback(GetStreamCompleted), filtre);

            
            //this.facebookAPI.Application.GetPublicInfoAsync(6628568379, "3e7c78e35a76a9299309885393b02d97", null, new Facebook.Rest.Application.GetPublicInfoCallback(test), null);
        }

        /*
        private void test(app_info info, object obj, FacebookException e)
        {

        }*/

        /// <summary>
        /// Callback de la méthode LoadFeeds
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filtre"></param>
        /// <param name="ex"></param>
        private void GetStreamCompleted(stream_data data, object filtre, FacebookException ex)
        {
            this.busy = true;
            bool needUpdate = true;

           
            if(this.feeds.ContainsKey(filtre.ToString()))
            {
                if(this.feeds[filtre.ToString()].Count > 0)
                    if (data.posts.stream_post[0].post_id == this.feeds[filtre.ToString()][0].fb_post.post.post_id)
                        needUpdate = false;
                //fb_topics.Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.userID, post));
            }



            if(needUpdate)
            {
                this.feeds[filtre.ToString()] = new List<Topic>();


                List<long> userIds = new List<long>();

                foreach (stream_post post in data.posts.stream_post)
                {
                    //this.facebookAPI.Users.GetInfoAsync(post.source_id, new Users.GetInfoCallback(GetUser_Completed), post);

                    if (post.actor_id > 0 && post.actor_id != post.source_id)
                        userIds.Add((long)post.actor_id);
                    
                    userIds.Add(post.source_id);
                }

                this.facebookAPI.Users.GetInfoAsync(userIds, new Users.GetInfoCallback(GetUsers_Completed), data.posts.stream_post);
            }
            else
                this.busy = false;
        }

        

        private void GetUsers_Completed(IList<user> users, object data, FacebookException ex)
        {
            if (users != null)
                if (users.Count > 0)
                {
                    List<stream_post> posts = data as List<stream_post>;

                    foreach (stream_post post in posts)
                    {
                        user userSource = null;
                        user userTarget = null;
                        foreach (user unUser in users)
                        {
                            if (post.actor_id > 0 && post.actor_id != post.source_id)
                            {
                                if (post.actor_id == unUser.uid)
                                    userSource = unUser;
                                if (post.source_id == unUser.uid)
                                    userTarget = unUser;
                            }
                            else
                            {
                                if (post.source_id == unUser.uid)
                                    userSource = unUser;
                            }
                        }
                        TopicFB topicFB = new TopicFB(post, userSource, userTarget);
                        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        dateTime = dateTime.AddSeconds(post.created_time).AddHours(2);
                        this.feeds[post.filter_key].Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.userID, topicFB));
                    }
                    LoadStreamFeedsContext(posts[0].filter_key);
                }
            this.busy = false;
        }
        /*
        private void GetUser_Completed(IList<user> users, object o, FacebookException ex)
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
                    //
                    //if (streamFeeds != null && this.feeds[post.filter_key].Count >= 30)
                    //{
                    //    streamFeeds.allTopics[this.account.userID.ToString()] = this.feeds[post.filter_key];
                    //    streamFeeds.LoadContext();
                    //}
                }
        }*/


        /// <summary>
        /// Met à jour l'affichage avec les feeds récupérés
        /// </summary>
        /// <param name="filtre"></param>
        private void LoadStreamFeedsContext(string filtre)
        {
           /* if (this.feeds.ContainsKey(filtre))
                if (streamFeeds != null && this.feeds[filtre].Count >= 1)
                {
                    Connexion.allTopics[this.account.userID.ToString()] = this.feeds[filtre];
                    streamFeeds.LoadContext();
                }
            */
            if (streamFeeds != null && this.feeds.ContainsKey(filtre) && this.feeds[filtre].Count > 0)
            {
                /*List<Topic> f_topics = null;
                if (checkLast)
                {
                    if (Connexion.allTopics.ContainsKey(this.account.userID.ToString()))
                        f_topics = (List<Topic>)Connexion.allTopics[this.account.userID.ToString()];
                    if (f_topics != null && f_topics.Count > 0)
                    {

                        stream_post last = f_topics[0].fb_post.post;
                        if ((last.post_id != this.feeds[filtre][0].fb_post.post.post_id) || (last.filter_key != this.feeds[filtre][0].fb_post.post.filter_key) || first)
                        {
                            Connexion.allTopics[this.account.userID.ToString()] = this.feeds[filtre];
                            streamFeeds.LoadContext();
                        }
                        
                    }
                    else
                    {
                        Connexion.allTopics[this.account.userID.ToString()] = this.feeds[filtre];
                        streamFeeds.LoadContext();
                       
                    }
                }
                else
                {*/
                    Connexion.allTopics[this.account.userID.ToString()] = this.feeds[filtre];
                    streamFeeds.LoadContext();
                //}
            }
        }

        public void LoadFilters(LeftMenu menuFeeds)
        {
            this.menuFeeds = menuFeeds;
            if (this.filters == null)
                this.facebookAPI.Stream.GetFiltersAsync(new Stream.GetFiltersCallback(GetFiltersCompleted), menuFeeds);
            else
                menuFeeds.LoadFilters(this);
        }

        private void GetFiltersCompleted(IList<stream_filter> filtres, object o, FacebookException ex)
        {
            if (filtres != null)
            {
                this.filters = filtres as List<stream_filter>;
                menuFeeds.LoadFilters(this);
            }
        }
        
    
    }
}
