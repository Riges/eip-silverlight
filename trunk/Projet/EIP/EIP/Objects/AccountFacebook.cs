using System;
using System.Net;
using System.Linq;
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
using System.Windows.Threading;
using System.Xml;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFacebookLight))]
    public class AccountFacebookLight : AccountLight
    {

        public static Dispatcher dispatcher;

        private Api facebookAPI;
        private BrowserSession browserSession;
        public string appID { get; set; }
        public user userInfos { get; set; }
        public Dictionary<string, List<Topic>> feeds { get; set; }
        public List<user> friends { get; set; }
        public Dictionary<long, List<user>> allFriends { get; set; }
        public Dictionary<long, List<user>> mutualFriends { get; set; }
        public List<stream_filter> filters { get; set; }
        public List<profile> profiles { get; set; }
        public List<thread> box { get; set; }

        public Dictionary<long, List<Topic>> walls { get; set; }

        public Dictionary<long, List<album>> albums { get; set; }
        public Dictionary<string, Dictionary<string, photo>> photos { get; set; }

        public Dictionary<long, Dictionary<long, VideoLight>> videos { get; set; }
        //public Dictionary<long, string> thumbVideos { get; set; }

      

        public enum MsgFolder
        {
            Inbox = 0,
            Outbox = 1,
            Updates = 4
        }


        private bool busy = false;
        //private int nbFeeds = 0;

        //Controls
        //private StreamFeeds streamFeeds;

        public AccountFacebookLight()
        {
            this.account = new AccountFacebook();
            this.feeds = new Dictionary<string, List<Topic>>();
            this.friends = new List<user>();
            this.allFriends = new Dictionary<long, List<user>>();
            this.mutualFriends = new Dictionary<long, List<user>>();
            this.albums = new Dictionary<long, List<album>>();
            this.photos = new Dictionary<string, Dictionary<string, photo>>();
            this.videos = new Dictionary<long, Dictionary<long, VideoLight>>();
            this.profiles = new List<profile>();
            this.walls = new Dictionary<long, List<Topic>>();

            #if (DEBUG)
                this.appID = "131664040210585";
            #else
                this.appID = "185484705355";
            #endif

            //this.thumbVideos = new Dictionary<long, string>();

            Connexion.dispatcher.BeginInvoke(() =>
                {
                    if (Connexion.ApplicationKey != "")
                    {
                        browserSession = new BrowserSession(Connexion.ApplicationKey, Connexion.perms);
                        browserSession.LoginCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(browserSession_LoginCompleted);
                    }
                    else
                    {

                    }
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
                    /*NotificationWindow notify = new NotificationWindow();
                    notify.Height = 75; notify.Width = 200;
                    TextBlock note = new TextBlock();
                    note.Text = "Ceci est une notification Silverlight 4 !";
                    notify.Content = note;
                    notify.Show(4000);*/
                });
        }

        void browserSession_LoginCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            facebookAPI = new Api(browserSession);
            Connexion.accounts[this.account.accountID] = this;

            this.GetUserInfo(this.facebookAPI.Session.UserId, GetUserInfoFrom.Login);
            
            
            //pré load
            if (this.facebookAPI != null)
            {
                this.LoadFriends();
                this.LoadFilters();
                this.LoadFeeds("", false);
            }
            
        }

        public void SendStatus(string status)
        {
            this.facebookAPI.Status.SetAsync(status, SetStatusCompleted, null);
        }

        void SetStatusCompleted(bool result, Object state, FacebookException e)
        {
            if (e == null)
            {
                if (result == false)
                {
                    dispatcher.BeginInvoke(() =>
                    {
                        MessageBox msg = new MessageBox("Erreur", "Impossible de poster ce message");
                        msg.Show();
                    });
                }
                else
                {
                    Connexion.navigationService.Navigate(new Uri("/Home", UriKind.Relative)); // TODO : reload si deja sur home...
                }
            }
            else
            {
                dispatcher.BeginInvoke(() =>
                {
                    MessageBox msg = new MessageBox("Erreur", "T'es Null !");
                    msg.Show();
                });
            }
        }

        public enum GetUserInfoFrom
        {
            Login, 
            Profil
        }

        public delegate void OnGetUserInfoCompleted(user monUser);
        public event OnGetUserInfoCompleted GetUserInfoCalled;

        public event OnGetUserInfoCompleted GetFirstUserInfoCalled;

        public void GetUserInfo(long uid, GetUserInfoFrom from)
        {
            switch (from)
            {
                case GetUserInfoFrom.Login:
                    this.facebookAPI.Users.GetInfoAsync(uid, new Users.GetInfoCallback(GetUserInfo_Completed), from);
                    break;
                case GetUserInfoFrom.Profil:

                    var result =  from user unUser in friends
                                  where unUser.uid == uid
                                  select unUser;
                    user toto = result as user;

                    if (this.userInfos != null && this.userInfos.uid == uid)
                    {
                        if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                            this.GetUserInfoCalled.Invoke(this.userInfos);
                    }
                    else if (toto != null)
                    {
                        if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                            this.GetUserInfoCalled.Invoke((user)result);
                        
                    }
                    else
                    {
                        this.facebookAPI.Users.GetInfoAsync(uid, new Users.GetInfoCallback(GetUserInfo_Completed), from);
                    }
                    break;
                default:
                    break;
            }


           
        }

        private void GetUserInfo_Completed(IList<user> users, Object from, FacebookException ex)
        {
            if (ex == null && users.Count > 0)
            {
                user toto = users[0];

                switch ((GetUserInfoFrom)from)
                {
                    case GetUserInfoFrom.Login:
                        if (this.facebookAPI.Session.UserId == toto.uid)
                        {
                            this.userInfos = toto;
                            if (this.GetFirstUserInfoCalled != null)//evite que ca plante si pas dabo
                                this.GetFirstUserInfoCalled.Invoke(this.userInfos);
                        }
                        break;
                    case GetUserInfoFrom.Profil:
                        if (this.GetUserInfoCalled != null)//evite que ca plante si pas dabo
                            this.GetUserInfoCalled.Invoke(toto);
                        break;
                    default:
                        break;

                }
              


                

            }
        }


        ////////// Messages \\\\\\\\\\\
        #region Messages

        public delegate void OnGetMessagesCompleted(List<ThreadMessage> liste);
        public event OnGetMessagesCompleted GetMessagesCalled;


        public delegate void OnGetThreadCompleted(ThreadMessage th);
        public event OnGetThreadCompleted GetThreadCalled;

        public void LoadInboxMessages()
        {

            //this.facebookAPI.Message.GetThreadsInFolderAsynch(0, (int)this.account.userID, 42, 0, new Message.GetThreadsInFolderCallback(LoadMessagesCompleted), null);

            this.facebookAPI.Fql.QueryAsync<message_getThreadsInFolder_response>("SELECT thread_id,folder_id,subject,recipients,updated_time,parent_message_id,parent_thread_id,message_count,snippet,snippet_author,object_id,unread,viewer_id from thread where folder_id=0", new Fql.QueryCallback<message_getThreadsInFolder_response>(GetThreadsFQL_Completed), null);

        }
        public void LoadOutboxMessages()
        {
            //this.facebookAPI.Message.GetThreadsInFolderAsynch(Int32.Parse(EIP.AccountFacebookLight.MsgFolder.Outbox.ToString()), (int)this.account.userID, 42, 0, new Message.GetThreadsInFolderCallback(GetThreadsFQL_Completed), null);

            this.facebookAPI.Fql.QueryAsync<message_getThreadsInFolder_response>("SELECT thread_id,folder_id,subject,recipients,updated_time,parent_message_id,parent_thread_id,message_count,snippet,snippet_author,object_id,unread,viewer_id from thread where folder_id=1", new Fql.QueryCallback<message_getThreadsInFolder_response>(GetThreadsFQL_Completed), null);

        }

        public void GetThreadsFQL_Completed(message_getThreadsInFolder_response liste, object obj, FacebookException ex)
        {
            /*Connexion.dispatcher.BeginInvoke(() =>
             {
                 string tmp = (ex == null ? liste.thread.Count.ToString() : ex.Message);
                 MessageBox toto = new MessageBox("", "LoadMessagesCompleted " + tmp);
                 toto.Show();
             });*/


            //this.box = liste as List<thread>;

            if (ex == null && liste.thread.Count > 0)
            {
                
                if (this.GetMessagesCalled != null)//OBLIGATOIRE pr etre sur qu'il y a bien des abonnements sur l'event et éviter un plantage
                {
                    List<ThreadMessage> liste2 = new List<ThreadMessage>();
                    List<long> userIds = new List<long>();
                    foreach (thread th in liste.thread)
                    {
                        liste2.Add(new ThreadMessage(th, this.account.accountID));
                        // TODO : si user courant, prendre un des destinataires
                        if (!userIds.Contains(th.snippet_author))
                            userIds.Add(th.snippet_author);

                    }
                    //this.facebookAPI.Fql.QueryAsync("SELECT id, name, url, pic_square, type from profile where id IN (" + String.Join(",", userIds) + ")", new Fql.QueryCallback(GetAuthor_Completed), liste2);
                    GetAuthors(userIds, liste2, new Fql.QueryCallback(GetAuthor_Completed));
                }
            }


        }

        public void GetAuthors(List<long> userIds, object obj, Fql.QueryCallback callback)
        {
            this.facebookAPI.Fql.QueryAsync("SELECT id, name, url, pic_square, type from profile where id IN (" + String.Join(",", userIds) + ")", callback, obj);
        }

        public void GetAuthor_Completed(String usersXml, object data, FacebookException ex)
        {
            List<ThreadMessage> posts = data as List<ThreadMessage>;
            List<ThreadMessage> liste2 = new List<ThreadMessage>();
            List<profile> users = new List<profile>();
            // A la mano parce que pas de type de retour de l'api pour les profile
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(usersXml)))
            {
                do {
                    try
                    {
                        profile myUser = new profile();

                        reader.ReadToFollowing("id");
                        myUser.id = reader.ReadElementContentAsLong();
                        reader.ReadToFollowing("name");
                        myUser.name = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("url");
                        myUser.url = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("pic_square");
                        myUser.pic_square = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("type");
                        myUser.type = reader.ReadElementContentAsString();

                        users.Add(myUser);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                } while (!reader.EOF);

            }


            if (users != null)
                if (users.Count > 0)
                {
                    foreach (ThreadMessage post in posts)
                    {
                        ThreadMessage mypost = post;
                        foreach (profile unUser in users)
                        {
                            if (post.getAuthorAccountID() > 0)
                            {
                                if (post.getAuthorAccountID() == unUser.id)
                                {
                                    mypost.setAuthor(unUser);
                                }
                            }
                        }
                        liste2.Add(mypost);
                    }
                }
            if(this.GetMessagesCalled != null)
                this.GetMessagesCalled.Invoke(liste2);
        }


        public void LoadThreadMessages(thread th)
        {
            this.facebookAPI.Fql.QueryAsync("SELECT message_id, thread_id, author_id, body,created_time,attachment,viewer_id from message where thread_id=" + th.thread_id + " ORDER BY created_time ASC", new Fql.QueryCallback(GetThreadMessagesFQL_Completed), th);

        }

        public void GetThreadMessagesFQL_Completed(String messagesXml, object obj, FacebookException ex)
        {
            List<message> liste = new List<message>();
            List<long> userIds = new List<long>();
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(messagesXml)))
            {
                do
                {
                    try
                    {
                        message myMessage = new message();

                        reader.ReadToFollowing("message_id");
                        myMessage.message_id = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("author_id");
                        myMessage.author_id = reader.ReadElementContentAsLong();
                        userIds.Add(myMessage.author_id);
                        reader.ReadToFollowing("body");
                        myMessage.body = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("created_time");
                        myMessage.created_time = reader.ReadElementContentAsLong();
                        //reader.ReadToFollowing("attachment");
                        //myMessage.attachment = reader.ReadElementContentAs(stream_attachment, null);


                        liste.Add(myMessage);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                } while (!reader.EOF);
                ((thread)obj).messages.message = liste;
                /*if (this.GetMessagesCalled != null)
                    this.GetThreadCalled.Invoke((thread)obj);*/
                GetAuthors(userIds, obj, new Fql.QueryCallback(GetAuthorThread_Completed));
            }
        }


        public void GetAuthorThread_Completed(String usersXml, object obj, FacebookException ex)
        {
            thread th = obj as thread;
            List<MessageFacebook> liste = new List<MessageFacebook>();
            List<profile> users = new List<profile>();
            // A la mano parce que pas de type de retour de l'api pour les profile
            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(usersXml)))
            {
                do
                {
                    try
                    {
                        profile myUser = new profile();

                        reader.ReadToFollowing("id");
                        myUser.id = reader.ReadElementContentAsLong();
                        reader.ReadToFollowing("name");
                        myUser.name = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("url");
                        myUser.url = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("pic_square");
                        myUser.pic_square = reader.ReadElementContentAsString();
                        reader.ReadToFollowing("type");
                        myUser.type = reader.ReadElementContentAsString();

                        users.Add(myUser);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                } while (!reader.EOF);

            }


            if (users != null)
                if (users.Count > 0)
                {

                    MessageFacebook tmp = new MessageFacebook();
                    foreach (message mess in th.messages.message)
                    {
                        foreach (profile unUser in users)
                        {
                            
                            if (mess.author_id > 0)
                            {
                                if (mess.author_id == unUser.id)
                                {
                                    tmp = new MessageFacebook(mess, unUser, this.account.accountID);
                                    liste.Add(tmp);
                                }
                            }
                        }
                    }
                }
            ThreadMessage thread = new ThreadMessage((thread)obj, this.account.accountID);
            thread.setMessages(liste);
             if (this.GetMessagesCalled != null)
                 this.GetThreadCalled.Invoke(thread);

        }

        #endregion



        #region Friends

        public delegate void OnGetFriendsCompleted(List<user> friendsFB, long accountID);
        public event OnGetFriendsCompleted GetFriendsCalled;

        public void LoadFriends()
        {
            if (this.friends == null || this.friends.Count == 0)
                this.facebookAPI.Friends.GetAsync(new Friends.GetFriendsCallback(GetFriendsIDs_Completed), null);
            else
            {
                if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                         this.GetFriendsCalled.Invoke(this.friends, this.account.accountID);
            }
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

                     if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                         this.GetFriendsCalled.Invoke(this.friends, this.account.accountID);
                 }
        }


        //public delegate void OnLoadFriendsOfCompleted(long uid, List<user> friendsFB);
        //public event OnLoadFriendsOfCompleted LoadFriendsOfCalled;


        //public void LoadFriendsOf(long uid)
        //{
        //    if (this.allFriends.ContainsKey(uid) && this.allFriends[uid].Count > 0)
        //    {
        //        if (this.LoadFriendsOfCalled != null)//evite que ca plante si pas dabo
        //            this.LoadFriendsOfCalled.Invoke(uid, this.allFriends[uid]);
        //    }
        //    else
        //    {
        //         this.facebookAPI.Friends.GetAsync(uid, new Friends.GetFriendsCallback(LoadFriendsOfIDs_Completed), uid);
        //    }

            
        //}

        //private void LoadFriendsOfIDs_Completed(IList<long> usersIDs, Object uid, FacebookException ex)
        //{
        //    if (usersIDs != null)
        //        if (usersIDs.Count > 0)
        //        {
        //            this.facebookAPI.Users.GetInfoAsync((List<long>)usersIDs, new Users.GetInfoCallback(LoadFriendsOf_Completed), uid);
        //        }
        //}

        //private void LoadFriendsOf_Completed(IList<user> users, Object uidObj, FacebookException ex)
        //{
        //    long uid = Convert.ToInt64(uidObj);
        //    if (users != null)
        //        if (users.Count > 0)
        //        {
        //            this.allFriends[uid] = users as List<user>;

        //            this.LoadMutualFriends(uid);

        //            if (this.LoadFriendsOfCalled != null)//evite que ca plante si pas dabo
        //                this.LoadFriendsOfCalled.Invoke(uid, this.allFriends[uid]);
        //        }
        //}

        public delegate void OnLoadMutualFriendsCompleted(long uid, List<user> friendsFB);
        public event OnLoadMutualFriendsCompleted LoadMutualFriendsCalled;


        public void LoadMutualFriends(long uid)
        {
            if (this.mutualFriends.ContainsKey(uid) && this.mutualFriends[uid].Count > 0)
            {
                if (this.LoadMutualFriendsCalled != null)//evite que ca plante si pas dabo
                    this.LoadMutualFriendsCalled.Invoke(uid, this.mutualFriends[uid]);
            }
            else
            {
                this.facebookAPI.Friends.GetMutualFriendsAsync(uid, LoadMutualFriendIDs_Completed, uid);
            }


        }

        private void LoadMutualFriendIDs_Completed(IList<long> usersIDs, Object uid, FacebookException ex)
        {
            if (usersIDs != null)
                if (usersIDs.Count > 0)
                {
                    this.facebookAPI.Users.GetInfoAsync((List<long>)usersIDs, new Users.GetInfoCallback(LoadMutualFriends_Completed), uid);
                }
        }

        private void LoadMutualFriends_Completed(IList<user> users, Object uidObj, FacebookException ex)
        {
            long uid = Convert.ToInt64(uidObj);
            if (users != null)
                if (users.Count > 0)
                {
                    if(this.mutualFriends.ContainsKey(uid))
                        this.mutualFriends[uid].Clear();


                    this.mutualFriends[uid] = users as List<user>;

                    if (this.LoadMutualFriendsCalled != null)//evite que ca plante si pas dabo
                        this.LoadMutualFriendsCalled.Invoke(uid, this.mutualFriends[uid]);
                }
        }

        /*private void LoadMutualFriends_Completed(IList<long> usersIDs, Object uidObj, FacebookException ex)
        {
            long uid = Convert.ToInt64(uidObj);

            if (usersIDs != null)
                if (usersIDs.Count > 0)
                {
                    if (this.allFriends.ContainsKey(uid))
                    {
                        this.mutualFriends[uid].Clear();
                        foreach (long id in usersIDs)
                        {
                            var result = from user friend in this.allFriends[uid]
                                         where friend.uid == id
                                         select friend;
                            if (result != null && result.Count() > 0)
                            {
                                this.mutualFriends[uid].Add(result.First());
                            }
                        }
                    }

                    if (this.LoadMutualFriendsCalled != null)//evite que ca plante si pas dabo
                        this.LoadMutualFriendsCalled.Invoke(uid, this.mutualFriends[uid]);
                    return;
                }

            if (this.LoadMutualFriendsCalled != null)//evite que ca plante si pas dabo
                this.LoadMutualFriendsCalled.Invoke(uid, new List<user>());
        }*/

        #endregion

        /////////// Feeds \\\\\\\\\\\\\\\\
        #region Feeds

        public delegate void OnLoadFeedsCompleted();
        public event OnLoadFeedsCompleted LoadFeedsCalled;

        /// <summary>
        /// Méthode pour charger la liste des feeds correspondant au filtre passé en paramètre.
        /// </summary>
        /// <param name="filtre">Filtre de la liste des feeds.</param>
        /// <param name="aStreamFeeds">object StreamFeeds permettant de vérifier s'il y a de nouveau feeds pour mettre à jour ou non la liste.</param>
        public bool LoadFeeds(string filtre, bool first)
        {
            //if(aStreamFeeds != null)
            //{
                //this.streamFeeds = aStreamFeeds;
            bool ret = false;
            if (this.facebookAPI != null)
            {
                if (first)
                {
                    ret = LoadStreamFeedsContext(filtre);
                }
                //}
                if (!busy)
                    this.facebookAPI.Stream.GetAsync(this.account.userID, new List<long>(), null, null, 30, filtre, new Stream.GetCallback(GetStreamCompleted), filtre);
            }

            return ret;
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
            }

            if(needUpdate && data != null && ex == null)
            {
                this.feeds[filtre.ToString()] = new List<Topic>();


                //List<long> userIds = new List<long>();

                profiles.AddRange(data.profiles.profile);


                foreach (stream_post post in data.posts.stream_post)
                {
                    profile userSource = null;
                    profile userTarget = null;
                    foreach (profile unUser in profiles)
                    {                     
                        if (post.actor_id > 0 && post.actor_id != post.source_id)
                        {
                            if (post.actor_id == unUser.id)
                                userSource = unUser;
                            if (post.source_id == unUser.id)
                                userTarget = unUser;
                        }
                        else
                        {
                            if (post.source_id == unUser.id)
                                userSource = unUser;
                        }
                    }
                    TopicFB topicFB = new TopicFB(post, userSource, userTarget);
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    dateTime = dateTime.AddSeconds(post.created_time).AddHours(2);
                    this.feeds[post.filter_key].Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.accountID, topicFB));
                }

                if (this.feeds.ContainsKey(filtre.ToString()) && this.feeds[filtre.ToString()].Count > 0)
                {
                    Connexion.allTopics[this.account.userID.ToString()] = this.feeds[filtre.ToString()];
                    if (this.LoadFeedsCalled != null)//evite que ca plante si pas dabo
                        this.LoadFeedsCalled.Invoke();
                }

                this.busy = false;

              
            }
            else
                this.busy = false;
        }

        /// <summary>
        /// Met à jour l'affichage avec les feeds récupérés
        /// </summary>
        /// <param name="filtre"></param>
        private bool LoadStreamFeedsContext(string filtre)
        {
            if (this.feeds.ContainsKey(filtre) && this.feeds[filtre].Count > 0)
            {
                Connexion.allTopics[this.account.userID.ToString()] = this.feeds[filtre];
                if (this.LoadFeedsCalled != null)//evite que ca plante si pas dabo
                    this.LoadFeedsCalled.Invoke();
                return true;
            }
            return false;
        }

        public delegate void OnLoadWallCompleted(long uid, List<Topic> feeds);
        public event OnLoadWallCompleted LoadWallCalled;

        public void LoadWall(long uid)
        {
            if (this.facebookAPI != null)
            { 
                if(this.walls.ContainsKey(uid))
                    if(this.walls[uid].Count > 0)
                    {
                        if (this.LoadWallCalled != null)//evite que ca plante si pas dabo
                            this.LoadWallCalled.Invoke(uid, this.walls[uid]);
                    }

                List<long> uids = new List<long>();
                uids.Add(uid);
                this.facebookAPI.Stream.GetAsync(this.account.userID, uids, null, null, 30, "", new Stream.GetCallback(LoadWallCompleted), uid);
            }
        }

        private void LoadWallCompleted(stream_data data, object uid, FacebookException ex)
        {
            List<Topic> listTemp = new List<Topic>();

            profiles.AddRange(data.profiles.profile);

            foreach (stream_post post in data.posts.stream_post)
            {
                profile userSource = null;
                profile userTarget = null;
                foreach (profile unUser in profiles)
                {
                    if (post.actor_id > 0 && post.actor_id != post.source_id)
                    {
                        if (post.actor_id == unUser.id)
                            userSource = unUser;
                        if (post.source_id == unUser.id)
                            userTarget = unUser;
                    }
                    else
                    {
                        if (post.source_id == unUser.id)
                            userSource = unUser;
                    }
                }
                TopicFB topicFB = new TopicFB(post, userSource, userTarget);
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                dateTime = dateTime.AddSeconds(post.created_time).AddHours(2);
                //this.walls[(long)uid].Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.accountID, topicFB));
                listTemp.Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.accountID, topicFB));
            }

            bool send = false;

            if (this.walls.ContainsKey((long)uid))
            {
                if (this.walls[(long)uid] != listTemp)
                {
                    send = true;
                }
            }
            else
                send = true;

            if (send)
            {
                this.walls[(long)uid] = listTemp;
                if (this.LoadWallCalled != null)
                    this.LoadWallCalled.Invoke((long)uid, this.walls[(long)uid]);
            }
        }


        #endregion


        public delegate void OnLoadFiltersCompleted(long accounID, List<stream_filter> filters);
        public event OnLoadFiltersCompleted LoadFiltersCalled;

        public void LoadFilters()
        {
            if (this.facebookAPI != null)
            {
                if (this.filters == null)
                    this.facebookAPI.Stream.GetFiltersAsync(new Stream.GetFiltersCallback(GetFiltersCompleted), null);
                else
                    if (this.LoadFiltersCalled != null)//evite que ca plante si pas dabo
                        this.LoadFiltersCalled.Invoke(this.account.accountID, this.filters);
            }
        }

        private void GetFiltersCompleted(IList<stream_filter> filtres, object o, FacebookException ex)
        {
            if (filtres != null)
            {
                this.filters = filtres as List<stream_filter>;
                if (this.LoadFiltersCalled != null)//evite que ca plante si pas dabo
                    this.LoadFiltersCalled.Invoke(this.account.accountID, this.filters);
            }
        }

        public enum FBobjectType
        {
            Feed,
            Photo,
            Video
        }

       

        public delegate void OnGetComsCompleted(List<comment> coms, string postId);
        public event OnGetComsCompleted GetComsCalled;

        public void GetComs(string postId)
        {
            this.facebookAPI.Stream.GetCommentsAsync(postId, new Stream.GetCommentsCallback(GetComs_Completed), postId);
            //this.facebookAPI.Fql.QueryAsync<comments_get_response>("SELECT xid, fromid, post_id, time, text, id, username FROM comment WHERE object_id=" + postId.Split('_')[1], new Fql.QueryCallback<comments_get_response>(GetComsFQL_Completed), postId);
        }

        public void GetComs_Completed(IList<comment> coms, object obj, FacebookException ex)
        {
            if (ex == null && coms.Count > 0)
            {
                List<long> userIds = new List<long>();
                foreach (comment com in coms)
                {

                    bool exist = false;
                    foreach (profile prof in this.profiles)
                    {
                        if (com.fromid == prof.id)
                            exist = true;
                    }
                    if (!exist)
                        userIds.Add(com.fromid);
                }

                if (userIds.Count == 0)
                {
                    if (this.GetComsCalled != null)//evite que ca plante si pas dabo
                        this.GetComsCalled.Invoke(coms.ToList(), obj.ToString());
                }
                else
                {
                    List<object> list = new List<object>();
                    list.Add(coms);//[0]
                    list.Add(obj);//[1]

                    this.facebookAPI.Users.GetInfoAsync(userIds, new Users.GetInfoCallback(GetUserComs_Completed), list);
                }
            }
            else if (ex == null && coms.Count == 0)
            {
                if (this.GetComsCalled != null)//evite que ca plante si pas dabo
                    this.GetComsCalled.Invoke(null, obj.ToString());
            }
        }

        public void GetComsFQL_Completed(comments_get_response coms, object obj, FacebookException ex)
        {

            if (ex == null && coms.comment.Count > 0)
            {
                List<long> userIds = new List<long>();
                foreach (comment com in coms.comment)
                {

                    bool exist = false;
                    foreach (profile prof in this.profiles)
                    {
                        if (com.fromid == prof.id)
                            exist = true;
                    }
                    if (!exist)
                        userIds.Add(com.fromid);
                }

                if (userIds.Count == 0)
                {
                    if (this.GetComsCalled != null)//evite que ca plante si pas dabo
                        this.GetComsCalled.Invoke(coms.comment, obj.ToString());
                }
                else
                {
                    List<object> list = new List<object>();
                    list.Add(coms.comment);//[0]
                    list.Add(obj);//[1]

                    this.facebookAPI.Users.GetInfoAsync(userIds, new Users.GetInfoCallback(GetUserComs_Completed), list);
                }
            }
            else if (ex == null && coms.comment.Count == 0)
            {
                if (this.GetComsCalled != null)//evite que ca plante si pas dabo
                    this.GetComsCalled.Invoke(null, obj.ToString());
            }
        }

        private void GetUserComs_Completed(IList<user> users, object obj, FacebookException ex)
        {
            if (ex == null)
            {
                foreach (user user in users)
                {
                    profile prof = new profile();
                    prof.id = (long)user.uid;
                    prof.name = user.name;
                    prof.pic_square = user.pic_square;
                    prof.url = user.profile_url;
                    this.profiles.Add(prof);
                }
                List<object> list = (List<object>)obj;
                if (this.GetComsCalled != null)//evite que ca plante si pas d'abo
                    this.GetComsCalled.Invoke((List<comment>)list[0], list[1].ToString());
            }
        }

        public void AddCom(string postId, string comment)
        {
            this.facebookAPI.Stream.AddCommentAsync(postId, comment, new Stream.AddCommentCallback(AddCom_Completed), postId);
        }

        private void AddCom_Completed(string result, object postId, FacebookException ex)
        {
            if (ex == null)
            {
                this.GetComs(postId.ToString());
            }
        }

        public void DeleteCom(comment com, string postId)
        {
            this.facebookAPI.Stream.RemoveCommentAsync(com.id, new Stream.RemoveCommentCallback(DeleteCom_Completed), postId);
        }

        private void DeleteCom_Completed(bool result, object obj, FacebookException ex)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    if (ex == null)
                    {
                        if (result)
                        {
                            if (obj != null)
                                this.GetComs(obj.ToString());
                        }
                        else
                        {
                            MessageBox msgBox = new MessageBox("Erreur", "Le commentaire n'a pas pu être supprimé.", MessageBoxButton.OK);
                            msgBox.Show();
                        }
                    }
                    
                });
        }

        public delegate void OnAddLikeCompleted(bool ok, string postId);
        public event OnAddLikeCompleted AddLikeCalled;

        /// <summary>
        /// Ajouter le "j'aime" sur un post
        /// </summary>
        /// <param name="postId">post id</param>
        public void AddLike(string postId)
        {
            this.facebookAPI.Stream.AddLikeAsync(postId, new Stream.AddLikeCallback(AddLike_Completed), postId);
        }

        private void AddLike_Completed(bool result, object o, FacebookException ex)
        {
            if (this.AddLikeCalled != null)//evite que ca plante si pas dabo
                this.AddLikeCalled.Invoke(result, o.ToString());
        }

        public delegate void OnRemoveLikeCompleted(bool ok, string postId);
        public event OnRemoveLikeCompleted RemoveLikeCalled;

        /// <summary>
        /// Supprimer le "j'aime" sur un post
        /// </summary>
        /// <param name="postId">post id</param>
        public void RemoveLike(string postId)
        {
            this.facebookAPI.Stream.RemoveLikeAsync(postId, new Stream.RemoveLikeCallback(RemoveLike_Completed), postId);
        }

        private void RemoveLike_Completed(bool result, object o, FacebookException ex)
        {
            if (this.RemoveLikeCalled != null)//evite que ca plante si pas dabo
                this.RemoveLikeCalled.Invoke(result, o.ToString());
        }

        public delegate void OnGetAlbumsCompleted(List<album> albums);
        public event OnGetAlbumsCompleted GetAlbumsCalled;

        /// <summary>
        /// Récupérer les albums d'un user
        /// </summary>
        /// <param name="uid">user id</param>
        public void GetAlbums(long uid)
        {
            if(!this.albums.ContainsKey(uid))
                this.facebookAPI.Photos.GetAlbumsAsync(uid, new Photos.GetAlbumsCallback(GetAlbums_Completed), uid);
            else
                if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                    this.GetAlbumsCalled.Invoke(this.albums[(long)uid]);
        }

        private void GetAlbums_Completed(IList<album> albums, object uid, FacebookException ex)
        {
            if (ex == null && albums.Count > 0)
            {
                this.albums[(long)uid] = (List<album>)albums;

                List<string> covers = new List<string>();
                foreach (album al in albums)
                {
                    covers.Add(al.cover_pid);
                }

                this.facebookAPI.Photos.GetAsync(null, null, covers, new Photos.GetCallback(GetAlbumsCover_Completed), uid);

            }
            else
            {
                if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                    this.GetAlbumsCalled.Invoke(new List<album>());
            }
        }

        /*private void GetAlbumsFQL_Completed(photos_getAlbums_response albums, object uid, FacebookException ex)
        {
            if (ex == null && albums.album.Count > 0)
            {
                this.albums[(long)uid] = (List<album>)albums.album;

                List<string> covers = new List<string>();
                foreach (album al in albums.album)
                {
                    covers.Add(al.cover_pid);
                }

                this.facebookAPI.Photos.GetAsync(null, null, covers, new Photos.GetCallback(GetAlbumsCover_Completed), uid);



                //this.GetAlbumsCalled.Invoke(true, (long)uid);
            }
        }*/

        private void GetAlbumsCover_Completed(IList<photo> photos, object uid, FacebookException ex)
        {
            if (ex == null)
            {
                if (photos.Count > 0)
                {
                    foreach (photo tof in photos)
                    {
                        if (this.photos.ContainsKey(tof.aid))
                        {

                        }
                        else
                        {
                            Dictionary<string, photo> tofs = new Dictionary<string, photo>();
                            tofs.Add(tof.pid, tof);
                            //this.photos.Add(tof.aid, tofs);
                            //this.photos[tof.aid] = new Dictionary<string, photo>();
                            this.photos[tof.aid] = tofs;
                            //this.photos[tof.aid].Add(tof);
                        }
                    }

                    if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                        this.GetAlbumsCalled.Invoke(this.albums[(long)uid]);
                }

            }
        }

        public delegate void OnGetPhotosCompleted(bool ok, string aid, Dictionary<string, photo> photos);
        public event OnGetPhotosCompleted GetPhotosCalled;

        /// <summary>
        /// Récupérer les photos d'un album
        /// </summary>
        /// <param name="aid">album id</param>
        public void GetPhotos(string aid)
        {
            if (this.photos.ContainsKey(aid))
            {
                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(true, aid, this.photos[aid]);
            }
            this.facebookAPI.Photos.GetAsync(null, aid, null, new Photos.GetCallback(GetPhotos_Completed), aid);
        }

        private void GetPhotos_Completed(IList<photo> photos, object aid, FacebookException ex)
        {
            if (ex == null && photos.Count > 0)
            {
                if (this.photos[aid.ToString()].Count != photos.Count)
                {
                    this.photos[aid.ToString()] = new Dictionary<string, photo>();
                    foreach (photo tof in photos)
                    {
                        this.photos[aid.ToString()][tof.pid] = tof;
                    }

                    if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                        this.GetPhotosCalled.Invoke(true, aid.ToString(), this.photos[aid.ToString()]);
                }

            }
            else
            {
                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(false, aid.ToString(), this.photos[aid.ToString()]);
            }
        }

        public delegate void GetUsersLikesCompleted(bool ok, string postId);
        public event GetUsersLikesCompleted GetUsersLikesCalled;

        public void GetUsersLikes(stream_likes likes, string postId)
        {
            List<long> userIdsLike = new List<long>();
            userIdsLike.AddRange(likes.sample.uid);
            userIdsLike.AddRange(likes.friends.uid);

            List<long> userIds = new List<long>();
            foreach (long uid in userIdsLike)
            {
                bool exist = false;
                foreach (profile prof in this.profiles)
                {
                    if (uid == prof.id)
                        exist = true;
                }
                if (!exist)
                    userIds.Add(uid);
            }

            if (userIds.Count > 0)
                this.facebookAPI.Users.GetInfoAsync(userIds, new Users.GetInfoCallback(GetUsersLikes_Completed), postId);
            else
            {
                if (this.GetUsersLikesCalled != null)
                    this.GetUsersLikesCalled.Invoke(true, postId);
            }
        }

        private void GetUsersLikes_Completed(IList<user> users, object obj, FacebookException ex)
        {
            if (ex == null)
            {
                foreach (user user in users)
                {
                    profile prof = new profile();
                    prof.id = (long)user.uid;
                    prof.name = user.name;
                    prof.pic_square = user.pic_square;
                    prof.url = user.profile_url;
                    this.profiles.Add(prof);
                }


                if (this.GetUsersLikesCalled != null)
                    this.GetUsersLikesCalled.Invoke(true, obj.ToString());
            }
        }


        //////////////////////////////////////////////////////
        /////////               Photos              //////////
        //////////////////////////////////////////////////////

        public delegate void CreateMyNetWorkAlbumCompleted(AccountFacebookLight acount, album album);
        public event CreateMyNetWorkAlbumCompleted CreateMyNetWorkAlbumCalled;

        public void CreateMyNetWorkAlbum()
        {
            if (this.albums.Count > 0)
            {
                CheckIfAlbumExist("myNETwork", this.account.userID);
            }
            else
            {
                this.GetAlbumsCalled += new OnGetAlbumsCompleted(AccountFacebookLight_GetAlbumsCalled);
                this.GetAlbums(this.account.userID);
            }
        }

        private void AccountFacebookLight_GetAlbumsCalled(List<album> albums)
        {
            CheckIfAlbumExist("myNETwork", this.account.userID);
        }

        private void CheckIfAlbumExist(string albumText, long uid)
        {
            bool exist = false;

            var result = from album al in this.albums[uid]
                         where al.name == albumText
                         select al;

            if (result.Count() > 0)
                exist = true;

            if (!exist)
            {
                this.CreateAlbumCalled += new CreateAlbumCompleted(AccountFacebookLight_CreateAlbumCalled);
                this.CreateAlbum(albumText, "", "Album de l'application myNETwork");
            }
            else
            {
                if (this.CreateMyNetWorkAlbumCalled != null)
                    this.CreateMyNetWorkAlbumCalled.Invoke(this, result.First());
            }
        }

        void AccountFacebookLight_CreateAlbumCalled(album album)
        {
            if (this.CreateMyNetWorkAlbumCalled != null)
                this.CreateMyNetWorkAlbumCalled.Invoke(this, album);
        }


        public delegate void CreateAlbumCompleted(album album);
        public event CreateAlbumCompleted CreateAlbumCalled;

        public void CreateAlbum(string name, string location, string description)
        {
            this.facebookAPI.Photos.CreateAlbumAsync(name, location, description, new Photos.CreateAlbumCallback(CreateAlbum_Completed), null);
        }

        private void CreateAlbum_Completed(album album, object o, FacebookException ex)
        {
            if (this.CreateAlbumCalled != null)
                this.CreateAlbumCalled.Invoke(album);
        }

        public delegate void UploadPhotoCompleted(photo photo);
        public event UploadPhotoCompleted UploadPhotoCalled;

        public void UploadPhoto(string aid, string caption, byte[] photos, Enums.FileType type)
        {

            this.facebookAPI.Photos.UploadAsync(aid, caption, photos, type, new Photos.UploadCallback(UploadPhoto_Completed), null);


            /*
            attachment test = new attachment();
            test.caption = "captionnn";
            //test.latitude = string.Empty;
            //test.longitude = string.Empty;
            //test.comments_xid = string.Empty;

            test.name = "namee";
            test.description = "descriptioneuhhh";
          
            
            attachment_media media = new attachment_media();
            media.type = attachment_media_type.image;
            //test.media = new List<attachment_media>();
            //test.properties = new List<attachment_property>();
            //test.media.Add(media);

             test.media = new List<attachment_media>(){ new attachment_media_image()
                                {
                                    type = attachment_media_type.image,
                                    src = "http://sphotos.ak.fbcdn.net/hphotos-ak-ash2/hs011.ash2/33900_1592267456173_1520509439_31468853_3802398_n.jpg"
                              

                                }};

            // this.facebookAPI.Stream.PublishAsync("kikoolol", test, null, "609934043", this.account.userID, new Stream.PublishCallback(PublishStream_completed), null);
             
             */
        }

        private void UploadPhoto_Completed(photo photo, object o, FacebookException ex)
        {
            if (this.UploadPhotoCalled != null)//evite que ca plante si pas dabo
                this.UploadPhotoCalled.Invoke(photo);
        }


        //////////////////////////////////////////////////////
        /////////               Liens              //////////
        //////////////////////////////////////////////////////

        public void SendStreamLink(string msg, string link)
        {
            attachment attachment = new attachment();
            attachment.href = link;
            attachment.name = link;

            this.facebookAPI.Stream.PublishAsync(msg, attachment, null, this.account.userID.ToString(), this.account.userID, new Stream.PublishCallback(SendStreamLink_Completed), null);
        }

        private void SendStreamLink_Completed(string result, object o, FacebookException ex)
        {
            if (ex == null)
            {
                Connexion.navigationService.Navigate(new Uri("/Home", UriKind.Relative));
            }
        }

    

        //////////////////////////////////////////////////////
        /////////               Vidéos              //////////
        //////////////////////////////////////////////////////

        public delegate void GetVideosCompleted(Dictionary<long, VideoLight> videos, long uid);
        public event GetVideosCompleted GetVideosCalled;


        public void GetVideos(long uid)
        {
            if (this.videos.ContainsKey(uid))
            {
                if (this.GetVideosCalled != null)
                    this.GetVideosCalled.Invoke(this.videos[uid], uid);
            }
            else
                this.facebookAPI.Fql.QueryAsync("SELECT vid, owner, title, description, thumbnail_link, updated_time, created_time, src, src_hq FROM video WHERE owner=" + uid, new Fql.QueryCallback(GetVideos_completed), uid);
        }

        private void GetVideos_completed(string result, object uid, FacebookException ex)
        {
            List<VideoLight> vids = new List<VideoLight>();

            using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(result)))
            {
                do
                {
                    try
                    {
                        VideoLight vid = new VideoLight();
                        
                        reader.ReadToFollowing("vid");
                        vid.vid = reader.ReadElementContentAsLong();

                        vid.uid = Convert.ToInt32(uid);

                        reader.ReadToFollowing("title");
                        vid.title = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("description");
                        vid.description = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("thumbnail_link");
                        vid.thumbnail_link = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("updated_time");
                        vid.updated_time = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("created_time");
                        vid.created_time = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("src");
                        vid.src = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("src_hq");
                        vid.src_hq = reader.ReadElementContentAsString();

                        

                        vids.Add(vid);
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                } while (!reader.EOF);
            }


            if (vids.Count > 0)
            {
                this.videos[(long)uid] = new Dictionary<long, VideoLight>();
                foreach (VideoLight vid in vids)
                {
                    this.videos[(long)uid][vid.vid] = vid;
                }

                if (this.GetVideosCalled != null)
                    this.GetVideosCalled.Invoke(this.videos[(long)uid], (long)uid);
            }
            else
            {
                if (this.GetVideosCalled != null)
                    this.GetVideosCalled.Invoke(new Dictionary<long, VideoLight>(), (long)uid);
            }

        }
        

        
    
    }
}
