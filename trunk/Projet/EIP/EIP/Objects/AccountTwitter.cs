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
//using EIP.ServiceEIP;
using System.Collections.Generic;
using TweetSharp.Fluent;
using TweetSharp.Model;
using TweetSharp.Extensions;
using EIP.ServiceEIP;
using EIP.Objects;
using EIP.Views;
using System.Collections.ObjectModel;


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
        public TwitterUser user { get; set; }
        public List<Topic> homeStatuses { get; set; }

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
        }

          //********************************\\
         //*Methodes de récupération d'infos*\\
        //************************************\\

        /// <summary>
        /// Met à jour l'attribut "homeStatuses" (les tweets de la homepage)
        /// </summary>
        public void LoadHomeStatuses(StreamFeeds aStreamFeeds)
        {
            if (aStreamFeeds != null)
            {
                this.streamFeeds = aStreamFeeds;
                LoadStreamFeedsContext();
            }
            var homeTimeline = FluentTwitter.CreateRequest()
               .Configuration.UseTransparentProxy(Connexion.ProxyUrl)
               .AuthenticateWith(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret)
               .Statuses().OnHomeTimeline()
               .CallbackTo(HomeTimelineReceived);

            homeTimeline.RequestAsync();
        }

        /// <summary>
        /// Met à jour la liste des topics si streamFeeds  à été précédemment passé en parametre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void HomeTimelineReceived(object sender, TwitterResult result)
        {
            var statuses = result.AsStatuses();

            if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (result.AsError() == null) && (statuses != null))
            {
                homeStatuses = new List<Topic>();
                foreach(TwitterStatus status in statuses)
                {
                    homeStatuses.Add(new Topic(status.CreatedDate.AddHours(1), Account.TypeAccount.Twitter, this.account.userID, status));
                }

                LoadStreamFeedsContext();
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
                }*/

                //Connexion.SaveAccount(this);
            }
        }

        private void LoadStreamFeedsContext()
        {
            if (streamFeeds != null)
            {
                List<Topic> t_topics = null;

                if (streamFeeds.allTopics.ContainsKey(this.account.userID.ToString()))
                    t_topics = (List<Topic>)streamFeeds.allTopics[this.account.userID.ToString()];
                if (t_topics != null && t_topics.Count > 0)
                {
                    if (this.homeStatuses.Count > 0)
                    {
                        TwitterStatus last = t_topics[0].t_post;
                        if (last.Id != this.homeStatuses[0].t_post.Id)
                        {
                            streamFeeds.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                            streamFeeds.LoadContext();
                        }
                    }
                }
                else
                {
                    if (this.homeStatuses.Count > 0)
                    {
                        streamFeeds.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                        streamFeeds.LoadContext();
                    }
                }
            }
        }

    
    }
}
