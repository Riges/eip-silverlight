﻿using System;
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
using System.Windows.Threading;
using System.Xml;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFacebookLight))]
    public class AccountFacebookLight : AccountLight
    {

        public static Dispatcher dispatcher;
        
        public Api facebookAPI { get; set; }
        private BrowserSession browserSession { get; set; }
        public user userInfos { get; set; }
        public Dictionary<string, List<Topic>> feeds { get; set; }
        public List<user> friends { get; set; }
        public List<stream_filter> filters { get; set; }
        public List<profile> profiles { get; set; }
        public List<thread> box { get; set; }
        public Dictionary<long, List<album>> albums { get; set; }
        public Dictionary<string, Dictionary<string, photo>> photos { get; set; }

        public enum MsgFolder
        {
            Inbox = 0,
            Outbox = 1,
            Updates = 4
        }


        private bool busy = false;
        //private int nbFeeds = 0;

        //Controls
        private StreamFeeds streamFeeds;
        private LeftMenu menuFeeds;

        public AccountFacebookLight()
        {
            this.account = new AccountFacebook();
            this.feeds = new Dictionary<string, List<Topic>>();
            this.friends = new List<user>();
            this.albums = new Dictionary<long, List<album>>();
            this.photos = new Dictionary<string, Dictionary<string, photo>>();

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
            Connexion.accounts[this.account.accountID] = this;

            this.GetUserInfo(this.facebookAPI.Session.UserId, GetUserInfoFrom.Login);
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
            this.facebookAPI.Users.GetInfoAsync(uid, new Users.GetInfoCallback(GetUserInfo_Completed), from);
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


         ///  Messages

        public delegate void OnGetMessagesCompleted(List<ThreadMessage> liste);
        public event OnGetMessagesCompleted GetMessagesCalled;

        public void LoadInboxMessages()
        {

            //this.facebookAPI.Message.GetThreadsInFolderAsynch(0, (int)this.account.userID, 42, 0, new Message.GetThreadsInFolderCallback(LoadMessagesCompleted), null);

            this.facebookAPI.Fql.QueryAsync<message_getThreadsInFolder_response>("SELECT thread_id,folder_id,subject,recipients,updated_time,parent_message_id,parent_thread_id,message_count,snippet,snippet_author,object_id,unread,viewer_id from thread where folder_id=0", new Fql.QueryCallback<message_getThreadsInFolder_response>(GetThreadsFQL_Completed), null);

        }
        public void LoadOutboxMessages()
        {
            //this.facebookAPI.Message.GetThreadsInFolderAsynch(Int32.Parse(EIP.AccountFacebookLight.MsgFolder.Outbox.ToString()), (int)this.account.userID, 42, 0, new Message.GetThreadsInFolderCallback(GetThreadsFQL_Completed), null);
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
                    this.facebookAPI.Fql.QueryAsync("SELECT id, name, url, pic_square, type from profile where id IN (" + String.Join(",", userIds) + ")", new Fql.QueryCallback(GetAuthor_Completed), liste2);
                }
            }


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
            
            this.GetMessagesCalled.Invoke(liste2);
            
        }






        public void LoadFriends()
        {
            if (this.friends == null || this.friends.Count == 0)
                this.facebookAPI.Friends.GetAsync(new Friends.GetFriendsCallback(GetFriendsIDs_Completed), null);
            else
            {
                if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                         this.GetFriendsCalled.Invoke(this.friends);
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

        public delegate void OnGetFriendsCompleted(List<user> friendsFB);
        public event OnGetFriendsCompleted GetFriendsCalled;

        private void GetFriends_Completed(IList<user> users, Object obj, FacebookException ex)
        {
            if (users != null)
                if (users.Count > 0)
                 {
                     this.friends = users as List<user>;

                     if (this.GetFriendsCalled != null)//evite que ca plante si pas dabo
                         this.GetFriendsCalled.Invoke(this.friends);
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

                profiles = data.profiles.profile;

                foreach (stream_post post in data.posts.stream_post)
                {
                    //this.facebookAPI.Users.GetInfoAsync(post.source_id, new Users.GetInfoCallback(GetUser_Completed), post);

                    if (post.actor_id > 0 && post.actor_id != post.source_id)
                        if (!userIds.Contains(post.actor_id))
                            userIds.Add((long)post.actor_id);

                    if(!userIds.Contains(post.source_id))
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
                        this.feeds[post.filter_key].Add(new Topic(dateTime, Account.TypeAccount.Facebook, this.account.accountID, topicFB));
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
            //this.facebookAPI.Comments.GetAsync(xid.Split('_')[1], new Comments.GetCallback(GetComs_Completed), type);
            this.facebookAPI.Fql.QueryAsync<comments_get_response>("SELECT xid, fromid, post_id, time, text, id, username FROM comment WHERE object_id=" + postId.Split('_')[1], new Fql.QueryCallback<comments_get_response>(GetComsFQL_Completed), postId);
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

       /* public void GetComs_Completed(IList<comment> coms, object obj, FacebookException ex)
        {
           
        }*/

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
            
            //MessageBox msgBox = new MessageBox(com.id, com.post_id, MessageBoxButton.OK);
            //msgBox.Show();
            //string[] tmp = com.id.Split('_');
            this.facebookAPI.Stream.RemoveCommentAsync(com.id, new Stream.RemoveCommentCallback(DeleteCom_Completed), postId);
        }

        private void DeleteCom_Completed(bool result, object obj, FacebookException ex)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    //MessageBox msgBoxs = new MessageBox(result.ToString(), ex.Message, MessageBoxButton.OK);
                    //msgBoxs.Show();
                    if (ex == null)
                    {
                        if (result)
                        {
                            //MessageBox msgBox = new MessageBox("Succès", "Le commentaire à bien été supprimé.", MessageBoxButton.OK);
                            //msgBox.Show();
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


                //this.GetAlbumsCalled.Invoke(true, (long)uid);
            }
            /*else if (ex == null)
            {
                this.facebookAPI.Fql.QueryAsync<photos_getAlbums_response>("SELECT aid, owner, cover_pid, name, created, modified, description, location, size, link, visible, modified_major, edit_link, type, object_id, can_upload FROM album WHERE owner=" + uid, new Fql.QueryCallback<photos_getAlbums_response>(GetAlbumsFQL_Completed), uid);
            }*/
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
                            this.photos.Add(tof.aid, tofs);
                            //this.photos[tof.aid].Add(tof);
                        }
                    }

                    if (this.GetAlbumsCalled != null)//evite que ca plante si pas dabo
                        this.GetAlbumsCalled.Invoke(this.albums[(long)uid]);
                }

            }
        }

        public delegate void OnGetPhotosCompleted(bool ok);
        public event OnGetPhotosCompleted GetPhotosCalled;

        /// <summary>
        /// Récupérer les photos d'un album
        /// </summary>
        /// <param name="aid">album id</param>
        public void GetPhotos(string aid)
        {
            //if (!this.photos.ContainsKey(aid))
                this.facebookAPI.Photos.GetAsync(null, aid, null, new Photos.GetCallback(GetPhotos_Completed), aid);
        }

        private void GetPhotos_Completed(IList<photo> photos, object aid, FacebookException ex)
        {
            if (ex == null)
            {
                //this.photos[aid.ToString()] = (List<photo>)photos;
                foreach (photo tof in photos)
                {
                    this.photos[aid.ToString()][tof.pid] = tof;
                }

                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(true);

            }
            else
            {
                if (this.GetPhotosCalled != null)//evite que ca plante si pas dabo
                    this.GetPhotosCalled.Invoke(false);
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

        

        
    
    }
}
