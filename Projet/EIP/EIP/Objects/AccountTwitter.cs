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
        private StreamFeeds streamFeeds;

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


          //********************************\\
         //*Methodes de récupération d'infos*\\
        //************************************\\


        public delegate void OnLoadHomeStatusesCompleted();
        public event OnLoadHomeStatusesCompleted LoadHomeStatusesCalled;

        /// <summary>
        /// Met à jour l'attribut "homeStatuses" (les tweets de la homepage)
        /// </summary>
        public void LoadHomeStatuses(StreamFeeds aStreamFeeds, bool first)
        {
            if (aStreamFeeds != null)
            {
                this.streamFeeds = aStreamFeeds;
                LoadStreamFeedsContext(first);
            }

            Connexion.serviceEIP.LoadHomeStatusesAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret);
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

        /*
        /// <summary>
        /// Met à jour la liste des topics si streamFeeds à été précédemment passé en parametre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void HomeTimelineReceived(object sender, TwitterResult result)
        {
            var statuses = result.AsStatuses();

            if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (result.AsError() == null) && (statuses != null))
            {
                homeStatuses = new List<Topic>();
                foreach (TwitterStatus status in statuses)
                {
                    homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.userID, status));
                }

                LoadStreamFeedsContext(false);
                /*
                if (streamFeeds != null)
                {
                    List<Topic> t_topics = null;

                    if (streamFeeds.allTopics.ContainsKey(this.account.userID.ToString()))
                        t_topics = (List<Topic>)streamFeeds.allTopics[this.account.userID.ToString()];
                    if (t_topics != null)
                    {
                        TwitterStatus last = t_topics[0].t_post;
                        if (last.Id != this.homeStatuses[0].t_post.Id)
                        {
                            streamFeeds.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                            streamFeeds.LoadContext();
                        }
                    }
                    else
                    {
                        streamFeeds.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                        streamFeeds.LoadContext();
                    }
                }

                //Connexion.SaveAccount(this);
            }
        }
*/
        /// <summary>
        /// methode pour charger les amis (gens que l'on suit)
        /// </summary>
        public void LoadFriends()
        {

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

        private void LoadStreamFeedsContext(bool first)
        {
            if (streamFeeds != null)
            {
                List<Topic> t_topics = null;

                if (Connexion.allTopics.ContainsKey(this.account.userID.ToString()))
                    t_topics = (List<Topic>)Connexion.allTopics[this.account.userID.ToString()];
                if (this.homeStatuses.Count > 0)
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
            }
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
