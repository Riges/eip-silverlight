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
using System.Linq;

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
        public Dictionary<int, List<Topic>> userStatuses { get; set; }

        public List<TwitterFilter> filters { get; set; }
        public List<TwitterUser> friends { get; set; }
        public List<TwitterUser> followers { get; set; }
        public List<TwitterUser> profiles { get; set; }
        public List<ThreadMessage> messagesReceived { get; set; }
        public List<ThreadMessage> messagesSent { get; set; }
        public Int32 messagesWait { get; set; }

        private DateTime messageReceivedStart, messageReceivedEnd, messageSentStart, messageSentEnd;

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
            this.userStatuses = new Dictionary<int, List<Topic>>();
            this.friends = new List<TwitterUser>();
            this.followers = new List<TwitterUser>();
            this.profiles = new List<TwitterUser>();
            this.messagesReceived = new List<ThreadMessage>();
            this.messagesSent = new List<ThreadMessage>();


            this.filters = new List<TwitterFilter>();
            filters.Add(new TwitterFilter("Home", "OnHomeTimeline"));
            filters.Add(new TwitterFilter("OnFriendsTimeline", "OnFriendsTimeline"));
            filters.Add(new TwitterFilter("OnListTimeline", "OnListTimeline"));
            filters.Add(new TwitterFilter("OnPublicTimeline", "OnPublicTimeline"));
            filters.Add(new TwitterFilter("Mes tweets", "OnUserTimeline"));
            filters.Add(new TwitterFilter("Retweet", "Retweet"));
            filters.Add(new TwitterFilter("RetweetedByMe", "RetweetedByMe"));
            filters.Add(new TwitterFilter("RetweetedToMe", "RetweetedToMe"));

            Connexion.serviceEIP.LoadUserStatusesCompleted += new EventHandler<LoadUserStatusesCompletedEventArgs>(serviceEIP_LoadUserStatusesCompleted);

            Connexion.serviceEIP.LoadDirectMessagesReceivedCompleted += new EventHandler<LoadDirectMessagesReceivedCompletedEventArgs>(serviceEIP_LoadDirectMessagesReceivedCompleted);
            Connexion.serviceEIP.LoadDirectMessagesSentCompleted += new EventHandler<LoadDirectMessagesSentCompletedEventArgs>(serviceEIP_LoadDirectMessagesSentCompleted);
            Connexion.serviceEIP.GetUserInfosCompleted += new EventHandler<GetUserInfosCompletedEventArgs>(serviceEIP_GetUserInfosCompleted);

            Connexion.serviceEIP.GetFiendsCompleted += new EventHandler<GetFiendsCompletedEventArgs>(serviceEIP_GetFiendsCompleted);
            Connexion.serviceEIP.GetFollowersCompleted += new EventHandler<GetFollowersCompletedEventArgs>(serviceEIP_GetFollowersCompleted);

            Connexion.serviceEIP.SendTwitPicCompleted += new EventHandler<SendTwitPicCompletedEventArgs>(serviceEIP_SendTwitPicCompleted);
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

        public void SendStatus(string status, byte[] fileByte, string fileContentType, string fileName)
        {
            if (status.Trim() != "")
            {
                Connexion.serviceEIP.SendTwitPicAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, fileByte, fileContentType, fileName, status);
            }
        }

        void serviceEIP_SendTwitPicCompleted(object sender, SendTwitPicCompletedEventArgs e)
        {
            
        }

        public delegate void OnGetUserInfoCompleted(TwitterUser user, long accountID, bool isUserAccount);
        public event OnGetUserInfoCompleted GetUserInfoCalled;

        public void GetUserInfo(long userId)
        {
            if (userInfos != null && userInfos.Id == userId)
            {
                if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                    this.GetUserInfoCalled.Invoke(userInfos, this.account.accountID, true);
            }
            else
            {
                TwitterUser toto = null;
                var resultFriends = from TwitterUser unUser in this.friends
                             where unUser.Id == userId
                             select unUser;
                if (resultFriends.Count() > 0)
                    toto = resultFriends.First() as TwitterUser;

                if (toto == null)
                {
                    var resultProfiles = from TwitterUser unUser in this.profiles
                                 where unUser.Id == userId
                                 select unUser;
                    if (resultProfiles.Count() > 0)
                        toto = resultProfiles.First() as TwitterUser;
                }

                if (toto != null)
                {
                    if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                        this.GetUserInfoCalled.Invoke(toto, this.account.accountID, false);

                }
                else
                    Connexion.serviceEIP.GetUserInfosAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, userId, this.account.userID);
            }
        }

        void serviceEIP_GetUserInfosCompleted(object sender, GetUserInfosCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.UserState) == this.account.userID)
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                    {
                        TwitterUser user = e.Result;

                        if (user.Id == account.userID)
                            userInfos = user;
                        else
                        {
                            if (!this.profiles.Contains(user))
                                this.profiles.Add(user);
                        }

                        if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                        {
                            if (user.Id == account.userID)
                                this.GetUserInfoCalled.Invoke(user, this.account.accountID, true);
                            else
                                this.GetUserInfoCalled.Invoke(user, this.account.accountID, false);
                        }
                    }
                }
            }
        }


          //********************************\\
         //*Methodes de récupération d'infos*\\
        //************************************\\
        #region direct messages
        public delegate void OnLoadDirectMessagesCompleted(List<ThreadMessage> liste, AccountTwitterLight accountTw);
        public event OnLoadDirectMessagesCompleted LoadDirectMessagesCalled;

        public void LoadDirectMessagesReceived(DateTime start, DateTime end)
        {
            LoadDirectMessagesReceived(start, end, false);
        }

        public void LoadDirectMessagesSent(DateTime start, DateTime end)
        {
            LoadDirectMessagesSent(start, end, false);
        }

        // stop sert a savoir si on peut faire des nouvelles requetes ou pas !
        public void LoadDirectMessagesReceived(DateTime start, DateTime end, Boolean stop)
        {
            // PSEUDO CODE
            // on requete 20 messages
            // on regarde si premier message et dernier messages sont dans le champ
            // on requete si necessaire pour compléter la liste (si on veut les messages de l'année, il faudra pl requetes !)
            // (-detailler l'algo me souvient plus-)
            // a un moment on aura un resultat nul (plus de messages a recup !) ou les dates depasseront le champ d'action (end - start)
            // alors on pourra generer la liste a renvoyer
            // TODO : optimiser un peu et corriger des bugs dans l'algo

            messageReceivedStart = start;
            messageReceivedEnd = end;
            if (this.messagesReceived.Count > 0)
            {
                // regarde dans la liste 
                // si on a ce qu'il faut, on renvoie les bon messages
                // sinon : (champ = partie entre start et end)
                // si dernier dans le champ ou date supérieure, requete de 20 inf
                // si premier dans le champ ou inférieur, requete de 20 sup
                ThreadMessage prems = this.messagesReceived.ElementAt(0);
                ThreadMessage last = this.messagesReceived.ElementAt(this.messagesReceived.Count - 1);

                // A t on ce qu'il faut ?
                if (prems.date.CompareTo(start) >= 0 && last.date.CompareTo(end) <= 0)
                {
                    // prems et last sont dans le champ
                    List<ThreadMessage> returnList = new List<ThreadMessage>();
                    foreach (ThreadMessage message in this.messagesReceived)
                    {
                        // si message entre start et end on l'ajoute a la liste
                        if (message.date.CompareTo(start) >= 0 && message.date.CompareTo(end) <= 0)
                            returnList.Add(message);
                    }
                    if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(returnList, this);
                }
                else if (!stop)
                {
                    // Sans doute besoin de nouvelles requetes !!!
                    if (
                            (prems.date.CompareTo(start) <= 0 && prems.date.CompareTo(end) >= 0)
                        || (prems.date.CompareTo(end) > 0)
                    )
                    {
                        // prems dans le champ ou date inférieure

                        Connexion.serviceEIP.LoadDirectMessagesReceivedAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, 0, prems.getDm().Id + 1);
                        messagesWait += 1;
                    }

                    if (
                            (last.date.CompareTo(start) >= 0 && last.date.CompareTo(end) <= 0)
                        || (last.date.CompareTo(start) < 0)
                    )
                    {
                        // last dans le champ ou date supérieure
                        Connexion.serviceEIP.LoadDirectMessagesReceivedAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, last.getDm().Id - 1, 0);
                        messagesWait += 1;
                    }
                }
                else if ( messagesWait == 0) {
                    if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(new List<ThreadMessage>(), this);
                }
            }
            else
            {
                // requete 20 premiers
                Connexion.serviceEIP.LoadDirectMessagesReceivedAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, 0, 0);
                messagesWait += 1;
            }
        }

        /*public void LoadDirectMessagesReceived()
        {

            Connexion.serviceEIP.LoadDirectMessagesReceivedAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, 0 ,0);

        }*/
        /*public void LoadDirectMessagesSent()
        {
            Connexion.serviceEIP.LoadDirectMessagesSentAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID);
        }*/

        // stop sert a savoir si on peut faire des nouvelles requetes ou pas !
        public void LoadDirectMessagesSent(DateTime start, DateTime end, Boolean stop)
        {
            messageSentStart = start;
            messageSentEnd = end;
            if (this.messagesSent.Count > 0)
            {
                // regarde dans la liste 
                // si on a ce qu'il faut, on renvoie les bon messages
                // sinon : 
                // si dernier dans le champ ou date supérieure, requete de 20 inf
                // si premier dans le champ ou inférieur, requete de 20 sup
                ThreadMessage prems = this.messagesSent.ElementAt(0);
                ThreadMessage last = this.messagesSent.ElementAt(this.messagesSent.Count - 1);

                // A t on ce qu'il faut ?
                if (prems.date.CompareTo(start) >= 0 && last.date.CompareTo(end) <= 0)
                {
                    List<ThreadMessage> returnList = new List<ThreadMessage>();
                    foreach (ThreadMessage message in this.messagesSent)
                    {
                        if (message.date.CompareTo(start) >= 0 && message.date.CompareTo(end) <= 0)
                            returnList.Add(message);
                    }
                    if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(returnList, this);
                }
                else if (!stop)
                {
                    // Sans doute besoin de nouvelles requetes !!!
                    if (
                            (prems.date.CompareTo(start) <= 0 && prems.date.CompareTo(end) >= 0)
                        || (prems.date.CompareTo(end) > 0)
                    )
                    {
                        // prems dans le champ ou date inférieure
                        // Y passe jamais la je sais pas pk, cette partie est elle vraiment utile ? le test suivant suffit peut etre !!!
                        Connexion.serviceEIP.LoadDirectMessagesSentAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, 0, prems.getDm().Id + 1);
                        messagesWait += 1;
                    }

                    if (
                            (last.date.CompareTo(start) >= 0 && last.date.CompareTo(end) <= 0)
                        || (last.date.CompareTo(start) < 0)
                    )
                    {
                        // last dans le champ ou date supérieure
                        Connexion.serviceEIP.LoadDirectMessagesSentAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, last.getDm().Id - 1, 0);
                        messagesWait += 1;
                    }
                }
                else if (messagesWait == 0)
                {
                    if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(new List<ThreadMessage>(), this);
                }
            }
            else
            {
                // requete 20 premiers
                Connexion.serviceEIP.LoadDirectMessagesSentAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID, 0, 0);
                messagesWait += 1;
            }
        }

        void serviceEIP_LoadDirectMessagesReceivedCompleted(object sender, LoadDirectMessagesReceivedCompletedEventArgs e)
        {
           /* if (Convert.ToInt64(e.UserState) == this.account.userID)
            {*/
                messagesWait -= 1;
                List<ServiceEIP.TwitterDirectMessage> dms = e.Result;
                List<ThreadMessage> directMessages = new List<ThreadMessage>();


                if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null))
                {

                    if (dms == null || dms.Count == 0)
                    {
                        LoadDirectMessagesReceived(messageReceivedStart, messageReceivedEnd, true);
                    }
                    else
                    {
                        foreach (ServiceEIP.TwitterDirectMessage dm in dms)
                        {
                            ThreadMessage directMessage = new ThreadMessage(dm, this.account.accountID);
                            directMessages.Add(directMessage);
                            //homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.accountID, status));
                        }
                        this.messagesReceived.AddRange(directMessages);
                        this.messagesReceived.Sort(delegate(ThreadMessage t1, ThreadMessage t2) { return t2.date.CompareTo(t1.date); }); // on sait jms

                        // on retourne vers la première méthode pour vérifier !!!
                        LoadDirectMessagesReceived(messageReceivedStart, messageReceivedEnd);

                        /*if (this.LoadDirectMessagesCalled != null)
                            this.LoadDirectMessagesCalled.Invoke(directMessages);*/
                    }
                }
            //}
        }

        /*void serviceEIP_LoadDirectMessagesReceivedCompleted(object sender, LoadDirectMessagesReceivedCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.UserState) == this.account.userID)
            {
                List<ServiceEIP.TwitterDirectMessage> dms = e.Result;
                List<ThreadMessage> directMessages = new List<ThreadMessage>();

                if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null) && (dms != null))
                {
                    foreach (ServiceEIP.TwitterDirectMessage dm in dms)
                    {
                        ThreadMessage directMessage = new ThreadMessage(dm, this.account.accountID);
                        directMessages.Add(directMessage);
                        //homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.accountID, status));
                    }

                    //LoadStreamFeedsContext(false);
                    if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(directMessages);
                }
            }
        }*/

        void serviceEIP_LoadDirectMessagesSentCompleted(object sender, LoadDirectMessagesSentCompletedEventArgs e)
        {
            /* if (Convert.ToInt64(e.UserState) == this.account.userID)
             {*/
            messagesWait -= 1;
            List<ServiceEIP.TwitterDirectMessage> dms = e.Result;
            List<ThreadMessage> directMessages = new List<ThreadMessage>();


            if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null))
            {

                if (dms == null || dms.Count == 0)
                {
                    LoadDirectMessagesSent(messageSentStart, messageSentEnd, true);
                }
                else
                {
                    foreach (ServiceEIP.TwitterDirectMessage dm in dms)
                    {
                        ThreadMessage directMessage = new ThreadMessage(dm, this.account.accountID);
                        directMessages.Add(directMessage);
                        //homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.accountID, status));
                    }
                    this.messagesSent.AddRange(directMessages);
                    this.messagesSent.Sort(delegate(ThreadMessage t1, ThreadMessage t2) { return t2.date.CompareTo(t1.date); }); // on sait jms

                    // on retourne vers la première méthode pour vérifier !!!
                    LoadDirectMessagesSent(messageSentStart, messageSentEnd);

                    /*if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(directMessages);*/
                }
            }
            //}
        }

        /*void serviceEIP_LoadDirectMessagesSentCompleted(object sender, LoadDirectMessagesSentCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.UserState) == this.account.userID)
            {
                List<ServiceEIP.TwitterDirectMessage> dms = e.Result;
                List<ThreadMessage> directMessages = new List<ThreadMessage>();

                if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null) && (dms != null))
                {
                    foreach (ServiceEIP.TwitterDirectMessage dm in dms)
                    {
                        ThreadMessage directMessage = new ThreadMessage(dm, this.account.accountID);
                        directMessages.Add(directMessage);
                        //homeStatuses.Add(new Topic(status.CreatedDate.AddHours(2), Account.TypeAccount.Twitter, this.account.accountID, status));
                    }

                    //LoadStreamFeedsContext(false);
                    if (this.LoadDirectMessagesCalled != null)
                        this.LoadDirectMessagesCalled.Invoke(directMessages);
                }
            }
        }*/


        #endregion

        #region status
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

            Connexion.serviceEIP.LoadHomeStatusesCompleted -= serviceEIP_LoadHomeStatusesCompleted;
            Connexion.serviceEIP.LoadHomeStatusesCompleted += serviceEIP_LoadHomeStatusesCompleted;

            Connexion.serviceEIP.LoadHomeStatusesAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, this.account.userID);

            return ret;
        }

        public delegate void OnLoadUserStatusesCompleted(int userID);
        public event OnLoadUserStatusesCompleted LoadUserStatusesCalled;

        void serviceEIP_LoadHomeStatusesCompleted(object sender, LoadHomeStatusesCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.UserState) == this.account.userID)
            {
                List<ServiceEIP.TwitterStatus> statuses = e.Result;

                if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (e.Error == null) && (statuses != null))
                {
                    homeStatuses = new List<Topic>();
                    foreach (ServiceEIP.TwitterStatus status in statuses)
                    {
                        homeStatuses.Add(new Topic(status.CreatedDate.ToUniversalTime(), Account.TypeAccount.Twitter, this.account.accountID, status));
                    }

                    LoadStreamFeedsContext(false);
                }
            }
        }


        /// <summary>
        /// Méthode pour charger les tweets d'un utilisateur
        /// </summary>
        /// <param name="userID">ID de l'utilisateur</param>
        public void LoadUserStatuses(int userID)
        {
            if (this.userStatuses.ContainsKey(userID) && this.userStatuses[userID] != null && this.userStatuses[userID].Count > 0)
            {
                if (this.LoadUserStatusesCalled != null)
                    this.LoadUserStatusesCalled.Invoke(userID);
            }
            else
               Connexion.serviceEIP.LoadUserStatusesAsync(((AccountTwitter)account).token, ((AccountTwitter)account).tokenSecret, userID, userID);
        }

        void serviceEIP_LoadUserStatusesCompleted(object sender, LoadUserStatusesCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                this.userStatuses[(int)e.UserState] = new List<Topic>();
                foreach (ServiceEIP.TwitterStatus status in e.Result)
                {
                    this.userStatuses[(int)e.UserState].Add(new Topic(status.CreatedDate.ToUniversalTime(), Account.TypeAccount.Twitter, this.account.accountID, status));
                }

                if (this.LoadUserStatusesCalled != null)
                    this.LoadUserStatusesCalled.Invoke((int)e.UserState);
            }
        }
        #endregion


        /// <summary>
        /// methode pour charger les amis (gens que l'on suit)
        /// </summary>
        public delegate void OnGetFriendsCompleted(List<TwitterUser> friends, long accountID);
        public event OnGetFriendsCompleted GetFriendsCalled;

        public void LoadFriends()
        {
            if (this.friends == null || this.friends.Count == 0)
                Connexion.serviceEIP.GetFiendsAsync(((AccountTwitter)this.account).token, ((AccountTwitter)this.account).tokenSecret, this.account.userID);
            else
            {
                if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                    this.GetFriendsCalled.Invoke(this.friends, this.account.accountID);
            }
        }



        void serviceEIP_GetFiendsCompleted(object sender, GetFiendsCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.UserState) == this.account.userID)
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                        if (e.Result.Count > 0)
                        {
                            this.friends = e.Result;

                            if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                                this.GetFriendsCalled.Invoke(this.friends, this.account.accountID);
                        }
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
        /// 
        public delegate void OnGetFollowersCompleted(List<TwitterUser> followers, long accountID);
        public event OnGetFollowersCompleted GetFollowersCalled;
        public void LoadFollowers()
        {
            if (this.followers == null || this.followers.Count == 0)
                Connexion.serviceEIP.GetFollowersAsync(((AccountTwitter)this.account).token, ((AccountTwitter)this.account).tokenSecret, this.account.userID);
            else
            {
                if (this.GetFollowersCalled != null)//evite que ca plante si pas dabo
                    this.GetFollowersCalled.Invoke(this.followers, this.account.accountID);
            }
        }

        /// <summary>
        /// callback de la methode loadfollowers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>


        private void serviceEIP_GetFollowersCompleted(object sender, GetFollowersCompletedEventArgs e)
        {
            if (Convert.ToInt64(e.UserState) == this.account.userID)
            {
                if (e.Error == null)
                {
                    if (e.Result != null)
                        if (e.Result.Count > 0)
                        {
                            this.followers = e.Result;

                            if (this.GetFollowersCalled != null)//evite que ca plante si pas dabo
                                this.GetFollowersCalled.Invoke(this.followers, this.account.accountID);
                        }
                }
            }
        }
        
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

                        Utils.NotificationMessage(this, "Vous avez de nouveaux tweet !", "");
                    }
                }
                else
                {
                    Connexion.allTopics[this.account.userID.ToString()] = this.homeStatuses;
                    if (this.LoadHomeStatusesCalled != null)//evite que ca plante si pas dabo
                        this.LoadHomeStatusesCalled.Invoke();

                    //Utils.NotificationMessage(this, "Vous avez de nouveaux tweet !", "");
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
