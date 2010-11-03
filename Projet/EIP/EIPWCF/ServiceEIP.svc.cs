﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
/*
using Dimebrain.TweetSharp;
using Dimebrain.TweetSharp.Fluent;
using Dimebrain.TweetSharp.Extensions;
using Dimebrain.TweetSharp.Model;*/
using System.Threading;
using EIPLibrary;

using TweetSharp.Twitter.Model;
using TweetSharp.Twitter.Fluent;
using TweetSharp.Twitter.Extensions;
using System.Configuration;
using TweetSharp;
using TweetSharp.Fluent;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;


namespace EIPWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ServiceEIP : IServiceEIP
    {
        public bool IsUp()
        {
            return Model.IsDBUp();
        }

        public string GetFBAppKey()
        {
            return ConfigurationManager.AppSettings["FBAppKey"];
        }



        public Account GetAccountByUserID(long userID)
        {
            return Model.GetAccount(userID);
        }

        public List<Account> GetAccountsByUserID(long userID)
        {
            return Model.GetAccountsByUserID(userID);
        }

        public List<Account> GetAccountsByGroupID(long groupID)
        {
            return Model.GetAccountsByGroupID(groupID);
        }

        public List<Account> GetAccountsByTwitter(string pseudo, string password)
        {
            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                 .AuthenticateAs(pseudo, password)
                 .Account()
                 .VerifyCredentials()
                 .AsXml();

            var response = query.Request();
            var identity = response.AsUser();

            return Model.GetAccountsByUserID(identity.Id);
        }

        public bool AddAccount(Account newAccount, string token, string pin)
        {
            SetClientInfo();
            switch (newAccount.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    break;
                case Account.TypeAccount.Twitter:
                    var accessToken = FluentTwitter.CreateRequest()
                    .Authentication.GetAccessToken(token, pin);

                    TwitterResult result = accessToken.Request();
                    var tokenResult = result.AsToken();

                    newAccount.name = tokenResult.ScreenName;
                    newAccount.userID = Convert.ToInt64(tokenResult.UserId);
                    ((AccountTwitter)newAccount).token = tokenResult.Token;
                    ((AccountTwitter)newAccount).tokenSecret = tokenResult.TokenSecret;

                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }

            return Model.AddAccount(newAccount);
        }

        public IEnumerable<TwitterStatus> LoadHomeStatuses(string token, string tokenSecret)
        {
            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .Statuses().OnHomeTimeline();

            var response = query.Request();
            if (!response.IsTwitterError)
            {
                var statuses = response.AsStatuses();

                return statuses;
            }

            return null;
        }

        public IEnumerable<TwitterDirectMessage> LoadDirectMessages(string token, string tokenSecret)
        {
            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .DirectMessages().Received();

            var response = query.Request();
            if (!response.IsTwitterError)
            {
                var dms = response.AsDirectMessages();

                return dms;
            }

            return null;
        }

        private static string MakeTinyUrl(string Url)
        {
            try
            {
                if (Url.Length <= 30)
                {
                    return Url;
                }
                if (!Url.ToLower().StartsWith("http") && !Url.ToLower().StartsWith("ftp"))
                {
                    Url = "http://" + Url;
                }
                var request = WebRequest.Create("http://tinyurl.com/api-create.php?url=" + Url);
                var res = request.GetResponse();
                string text;
                using (var reader = new StreamReader(res.GetResponseStream()))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }
            catch (Exception)
            {
                return Url;
            }
        }

        public bool SendTweet(string token, string tokenSecret, string tweet)
        {
            Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection mactches = regx.Matches(tweet);
            foreach (Match match in mactches)
            {
                tweet = tweet.Replace(match.Value, MakeTinyUrl(match.Value));
            }

            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .Statuses().Update(tweet);

            var response = query.Request();
            if (response.IsTwitterError)
                return false;
            else
                return true;

        }

        public TwitterUser GetUserInfos(string token, string tokenSecret, long userId)
        {
            List<int> list = new List<int>();
            list.Add(Convert.ToInt32(userId));

            SetClientInfo();

            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .Users().Lookup(list);

            var response = query.Request();

            if (!response.IsTwitterError)
            {
                var users = response.AsUsers();
                if (users.Count() > 0)
                {
                    return users.First();
                }
            }

            return null;
        }

        public List<TwitterUser> GetFiends(string token, string tokenSecret)
        {
            List<TwitterUser> allFriends = new List<TwitterUser>();

            SetClientInfo();

            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .Users().GetFriends();

            var response = query.Request();

            if (!response.IsTwitterError)
            {
                var users = response.AsUsers();
                allFriends.AddRange(users);
            }

            var query2 = FluentTwitter.CreateRequest()
                  .AuthenticateWith(token, tokenSecret)
                  .Users().GetFollowers();

            var response2 = query2.Request();

            if (!response2.IsTwitterError)
            {
                var users = response.AsUsers();
                foreach (TwitterUser user in users)
                {
                    if (!allFriends.Contains(user))
                    {
                        allFriends.Add(user);

                    }
                }
            }

            return allFriends;
        }


        public string UploadPhoto(string name, byte[] img)
        {
            try
            {
                string save = @"c:\www\photos\" + name;


                Image monImage = Image.FromStream(new MemoryStream(img));
                FileStream stream = new FileStream(save, FileMode.Create);
                monImage.Save(stream, ImageFormat.Jpeg);

                stream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "http://mynetwork.selfip.net/photos/" + name;
        }


        public bool SaveAccount(Account accountToSave)
        {
            return Model.SaveAccount(accountToSave);
        }

        public long DeleteAccount(long accountID)
        {
            return Model.DeleteAccount(accountID);
        }


        public AccountFacebook testfb()
        {
            return  new AccountFacebook();
        }

        public AccountTwitter testT()
        {
            return new AccountTwitter();
        }

        private void SetClientInfo()
        {
            var clientInfo = new TwitterClientInfo
            {
                ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"]
            };

            FluentBase<TwitterResult>.SetClientInfo(clientInfo);
        }


        public string GetRequestToken()//string consumerKey, string consumerSecret
        {
            var requestToken = FluentTwitter.CreateRequest()
                .Authentication.GetRequestToken(ConfigurationManager.AppSettings["ConsumerKey"], ConfigurationManager.AppSettings["ConsumerSecret"]);

            var response = requestToken.Request();
            var result = response.AsToken();

            if (!response.IsTwitterError)
            {
                if (result != null)
                {
                    return result.Token;
                }
            }

            return null;
        }

        
    }


    class CompletedAsyncResult<T> : IAsyncResult
    {
        T data;

        public CompletedAsyncResult(T data)
        { this.data = data; }

        public T Data
        { get { return data; } }

        #region IAsyncResult Members
        public object AsyncState
        { get { return (object)data; } }

        public WaitHandle AsyncWaitHandle
        { get { throw new Exception("The method or operation is not implemented."); } }

        public bool CompletedSynchronously
        { get { return true; } }

        public bool IsCompleted
        { get { return true; } }
        #endregion
    }

}
