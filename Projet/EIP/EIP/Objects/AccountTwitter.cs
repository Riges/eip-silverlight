using System;
//using EIP.ServiceEIP;
using System.Collections.Generic;
using System.Runtime.Serialization;
using EIP.Objects;
//using TweetSharp.Fluent;
//using TweetSharp.Model;
//using TweetSharp.Extensions;
using EIP.ServiceEIP;
using EIP.Views;

namespace EIP
{
    
    [KnownTypeAttribute(typeof(AccountTwitterLight))]
    public class AccountTwitterLight : AccountLight
    {
        //public long accountID { get; set; }
        //public TypeAccount typeAccount { get; set; }
        //public long userID { get; set; }

        //public string token { get; set; }
        //public string tokenSecret { get; set; }
        public string pin { get; set; }
        public TwitterUser userInfos { get; set; }
        public List<Topic> homeStatuses { get; set; }
        public List<TwitterFilter> filters { get; set; }
        public List<TwitterUser> friends { get; set; }
        public List<TwitterUser> followers { get; set; }

        /*
        OnFriendsTimeline
        OnListTimeline
        OnPublicTimeline
        OnUserTimeline
         * Retweet
         * RetweetedByMe
         * RetweetedToMe
        */

        //Controls
        //private StreamFeeds streamFeeds;

        public AccountTwitterLight()
        {
            this.account = new AccountTwitter();
            this.homeStatuses = new List<Topic>();

            this.filters = new List<TwitterFilter>();
            filters.Add(new TwitterFilter("Home", "OnHomeTimeline"));
            filters.Add(new TwitterFilter("OnFriendsTimeline", "OnFriendsTimeline"));
            filters.Add(new TwitterFilter("OnListTimeline", "OnListTimeline"));
            filters.Add(new TwitterFilter("OnPublicTimeline", "OnPublicTimeline"));
            filters.Add(new TwitterFilter("Mes tweets", "OnUserTimeline"));
            filters.Add(new TwitterFilter("Retweet", "Retweet"));
            filters.Add(new TwitterFilter("RetweetedByMe", "RetweetedByMe"));
            filters.Add(new TwitterFilter("RetweetedToMe", "RetweetedToMe"));

            Connexion.serviceEIP.LoadHomeStatusesCompleted += new EventHandler<LoadHomeStatusesCompletedEventArgs>(serviceEIP_LoadHomeStatusesCompleted);
            Connexion.serviceEIP.GetUserInfosCompleted += new EventHandler<GetUserInfosCompletedEventArgs>(serviceEIP_GetUserInfosCompleted);

            Connexion.serviceEIP.GetFiendsCompleted += new EventHandler<GetFiendsCompletedEventArgs>(serviceEIP_GetFiendsCompleted);
        }

      

        public void Start()
        {
            if (this.account != null)
            {
                this.GetUserInfo(this.account.userID);
            }
        }

      


        /// <summary>
        /// Envoyer un nouveau Tweet
        /// <param name="statut">Statut à envoyer</param>
        /// </summary>
        public void SendStatus(string status)
        {
            if (status.Trim() != "")
            {
                Connexion.serviceEIP.SendTweetAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, status);
            }
        }

        public delegate void OnGetUserInfoCompleted(TwitterUser user);
        public event OnGetUserInfoCompleted GetUserInfoCalled;

        public void GetUserInfo(long userId)
        {
            if (userInfos != null && userInfos.Id == userId)
            {
                if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                    this.GetUserInfoCalled.Invoke(userInfos);
            }
            else
                Connexion.serviceEIP.GetUserInfosAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, userId);
        }

        void serviceEIP_GetUserInfosCompleted(object sender, GetUserInfosCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result != null)
                {
                    TwitterUser user = e.Result;
                    
                    if (user.Id == account.userID)
                        userInfos = user;

                    if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                        this.GetUserInfoCalled.Invoke(user);
                }
            }
        }


          //********************************\\
         //*Methodes de récupération d'infos*\\
        //************************************\\

        public delegate void OnLoadDirectMessagesCompleted();
        public event OnLoadDirectMessagesCompleted LoadDirectMessagesCalled;

        public bool LoadDirectMessages()
        {
            //if (aStreamFeeds != null)
            //{
            // this.streamFeeds = aStreamFeeds;

            //bool ret = false;
            //ret = LoadStreamFeedsContext(first);
            // }

            Connexion.serviceEIP.LoadDirectMessagesAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret);

            //return ret;
        }

        void serviceEIP_LoadDirectMessagesCompleted(object sender, LoadDirectMessagesCompletedEventArgs e)
        {
            List<ServiceEIP.TwitterDirectMessage> dms = e.Result;

            if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null) && (dms != null))
            {
                homeStatuses = new List<Topic>();
                foreach (ServiceEIP.TwitterDirectMessage dm in dms)
                {
                    //homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.accountID, status));
                }

                LoadStreamFeedsContext(false);
            }
        }


        public delegate void OnLoadHomeStatusesCompleted();
        public event OnLoadHomeStatusesCompleted LoadHomeStatusesCalled;

        /// <summary>
        /// Met à jour l'attribut "homeStatuses" (les tweets de la homepage)
        /// </summary>
        public bool LoadHomeStatuses(bool first)
        {
            //if (aStreamFeeds != null)
            //{
               // this.streamFeeds = aStreamFeeds;

            bool ret = false;
            ret = LoadStreamFeedsContext(first);
           // }

            Connexion.serviceEIP.LoadHomeStatusesAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret);

            return ret;
        }

        void serviceEIP_LoadHomeStatusesCompleted(object sender, LoadHomeStatusesCompletedEventArgs e)
        {
            List<ServiceEIP.TwitterStatus> statuses = e.Result;

            if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null) && (statuses != null))
            {
                homeStatuses = new List<Topic>();
                foreach (ServiceEIP.TwitterStatus status in statuses)
                {
                    homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.accountID, status));
                }

                LoadStreamFeedsContext(false);
            }
        }

       
        /// <summary>
        /// methode pour charger les amis (gens que l'on suit)
        /// </summary>
        public delegate void OnGetFriendsCompleted(List<TwitterUser> friends);
        public event OnGetFriendsCompleted GetFriendsCalled;

        public void LoadFriends()
        {
            if (this.friends == null || this.friends.Count == 0)
                Connexion.serviceEIP.GetFiendsAsync(((AccountTwitter)this.account).token, ((AccountTwitter)this.account).tokenSecret);
            else
            {
                if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                    this.GetFriendsCalled.Invoke(this.friends);
            }
        }



        void serviceEIP_GetFiendsCompleted(object sender, GetFiendsCompletedEventArgs e)
        {
            if(e.Error == null)
            {
            if (e.Result != null)
                if (e.Result.Count > 0)
                {
                    this.friends = e.Result;

                    if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                        this.GetFriendsCalled.Invoke(this.friends);
                }
            }
        }

        /// <summary>
        /// callback de la methode loadfriends
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        /*
        private void FriendsReceived(object sender, TwitterResult result)
        {
            if (!result.IsTwitterError)
            {
                var friendsTmp = result.AsUsers();
                this.friends = friendsTmp as List<TwitterUser>;
            }
        }
        */
        /// <summary>
        /// methode pour charger les followers (gens qui nous suivent)
        /// </summary>
        public void LoadFollowers()
        {
            /*
            var homeTimeline = FluentTwitter.CreateRequest()
               .Configuration.UseTransparentProxy(Connexion.ProxyUrl)
               .AuthenticateWith(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret)
               .Users().GetFollowers()
               .CallbackTo(FollowersReceived);
             * */
        }

        /// <summary>
        /// callback de la methode loadfollowers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
       
        /*
        private void FollowersReceived(object sender, TwitterResult result)
        {
            if (!result.IsTwitterError)
            {
                var followersTmp = result.AsUsers();
                this.followers = followersTmp as List<TwitterUser>;
            }
        }
        */
        /*
        private void StatusSended(object sender, TwitterResult result)
        {
            if (!result.IsTwitterError)
            {
               
            }
        }
        */

        private bool LoadStreamFeedsContext(bool first)
        {
            //if (streamFeeds != null)
            //{
            List<Topic> t_topics = null;

            if (Connexion.allTopics.ContainsKey(this.account.userID.ToString()))
                t_topics = (List<Topic>)Connexion.allTopics[this.account.userID.ToString()];
            if (this.homeStatuses.Count > 0)
            {
                if (t_topics != null && t_topics.Count > 0 && !first)
                {
                    TwitterStatus last = t_topics[0].t_post;
                    if (last.Id != this.homeStatuses[0].t_post.Id)
                    {
                        Connexion.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                        if (this.LoadHomeStatusesCalled != null)//evite que ca plante si pas dabo
                            this.LoadHomeStatusesCalled.Invoke();
                    }
                }
                else
                {
                    Connexion.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                    if (this.LoadHomeStatusesCalled != null)//evite que ca plante si pas dabo
                        this.LoadHomeStatusesCalled.Invoke();
                }
                return true;
            }

            return false;
           //}
        }


         public delegate void OnLoadFiltersCompleted(long accountID, List<TwitterFilter> filters);
        public event OnLoadFiltersCompleted LoadFiltersCalled;

        public void LoadFilters()
        {
            if (this.LoadFiltersCalled != null)//evite que ca plante si pas dabo
                this.LoadFiltersCalled.Invoke(this.account.accountID, this.filters);
        }

    
    }

    public class TwitterFilter
    {
        public string name { get; set; }
        public string value { get; set; }

        public TwitterFilter()
        {
        }

        public TwitterFilter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
