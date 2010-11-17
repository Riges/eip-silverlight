using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
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
using OAuth;
using System.Globalization;
using System.Web;


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

        public long AddAccount(Account newAccount, string token, string pin)
        {
            SetClientInfo();

            switch (newAccount.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    if (Model.AddAccount(newAccount))
                        return newAccount.groupID;
                    break;
                case Account.TypeAccount.Twitter:
                    var accessToken = FluentTwitter.CreateRequest()
                    .Authentication.GetAccessToken(ConfigurationManager.AppSettings["ConsumerKey"], ConfigurationManager.AppSettings["ConsumerSecret"], token, pin);

                    TwitterResult result = accessToken.Request();
                    var tokenResult = result.AsToken();

                    if (!result.IsTwitterError && tokenResult != null)
                    {
                        Account acc = Model.GetAccount(Convert.ToInt64(tokenResult.UserId));

                        if (acc == null)
                        {
                            newAccount.groupID = newAccount.groupID;
                            newAccount.name = tokenResult.ScreenName;
                            newAccount.userID = Convert.ToInt64(tokenResult.UserId);
                            newAccount.typeAccount = Account.TypeAccount.Twitter;
                            ((AccountTwitter)newAccount).token = tokenResult.Token;
                            ((AccountTwitter)newAccount).tokenSecret = tokenResult.TokenSecret;

                            if (Model.AddAccount(newAccount))
                                return newAccount.groupID;
                        }
                        else
                        {
                            acc.name = tokenResult.ScreenName;
                            ((AccountTwitter)acc).token = tokenResult.Token;
                            ((AccountTwitter)acc).tokenSecret = tokenResult.TokenSecret;

                            if (Model.SaveAccount(acc))
                                return acc.groupID;
                            else
                                return -2;
                        }
                    }
                    else 
                        return -1;
                    break;
               case Account.TypeAccount.Flickr:
                    Account accFK = Model.GetAccountFlickr(((AccountFlickr)newAccount).userIDstr);

                    if (accFK == null)
                    {
                        if (Model.AddAccount(newAccount))
                            return newAccount.groupID;
                    }
                    else
                    {
                        accFK.name = newAccount.name;
                        ((AccountFlickr)accFK).token = ((AccountFlickr)newAccount).token;

                        if (Model.SaveAccount(accFK))
                            return accFK.groupID;
                        else
                            return -2;
                    }
                    break;
                default:
                    break;
            }

            return 0;
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

        public List<TwitterStatus> LoadUserStatuses(string token, string tokenSecret, int userID)
        {
            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .Statuses().OnUserTimeline().For(userID);

            var response = query.Request();
            if (!response.IsTwitterError)
            {
                var statuses = response.AsStatuses();

                return statuses.ToList();
            }

            return null;
        }

        public IEnumerable<TwitterDirectMessage> LoadDirectMessagesSent(string token, string tokenSecret)
        {
            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .DirectMessages().Sent();

            var response = query.Request();
            if (!response.IsTwitterError)
            {
                var dms = response.AsDirectMessages();
                //dms.ElementAt(0).
                return dms;
            }

            return null;
        }

        public IEnumerable<TwitterDirectMessage> LoadDirectMessagesReceived(string token, string tokenSecret)
        {
            SetClientInfo();
            var query = FluentTwitter.CreateRequest()
                   .AuthenticateWith(token, tokenSecret)
                   .DirectMessages().Received();

            var response = query.Request();
            if (!response.IsTwitterError)
            {
                var dms = response.AsDirectMessages();
                //dms.ElementAt(0).
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

        public string SendTwitPic(string token, string tokenSecret, byte[] fileByte, string fileContentType, string fileName, string tweet)
        {
            var twitpicApiKey = "ff46639ea6e738e51222d49c5b7289e8";
            var oauthSignaturePattern = "OAuth realm=\"{0}\", oauth_consumer_key=\"{1}\", oauth_signature_method=\"HMAC-SHA1\", oauth_token=\"{2}\", oauth_timestamp=\"{3}\", oauth_nonce=\"{4}\", oauth_version=\"1.0\", oauth_signature=\"{5}\"";
            var authenticationRealm = "http://api.twitter.com/";
            var twitpicUploadApiUrl = "http://api.twitpic.com/2/upload.xml";
            var twitterVerifyCredentialsApiUrl = "https://api.twitter.com/1/account/verify_credentials.json";
            var contentEncoding = "iso-8859-1";
            var ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            var ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];
            
            Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection mactches = regx.Matches(tweet);
            foreach (Match match in mactches)
            {
                tweet = tweet.Replace(match.Value, MakeTinyUrl(match.Value));
            }

            var oauth = new OAuthBase();
            string normalizedString, normalizedParameters;
            var timestamp = oauth.GenerateTimeStamp();
            var nounce = oauth.GenerateNonce();
            var signature = oauth.GenerateSignature(
                                new Uri(twitterVerifyCredentialsApiUrl),
                                ConsumerKey,
                                ConsumerSecret,
                                token,
                                tokenSecret,
                                "GET",
                                timestamp,
                                nounce,
                                out normalizedString,
                                out normalizedParameters);

            signature = HttpUtility.UrlEncode(signature);

            var boundary = Guid.NewGuid().ToString();
            var request = (HttpWebRequest)WebRequest.Create(twitpicUploadApiUrl);

            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

            request.Headers.Add("X-Auth-Service-Provider", twitterVerifyCredentialsApiUrl);

            var authorizationHeader = string.Format(
                                        CultureInfo.InvariantCulture,
                                        oauthSignaturePattern,
                                        authenticationRealm,
                                        ConsumerSecret,
                                        token,
                                        timestamp,
                                        nounce,
                                        signature);
            request.Headers.Add("X-Verify-Credentials-Authorization", authorizationHeader);

            request.Method = "POST";

            var header = string.Format("--{0}", boundary);
            var footer = string.Format("--{0}--", boundary);

            var contents = new StringBuilder();
            contents.AppendLine(header);

            string fileHeader = string.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", "media", fileName);
            string fileData = Encoding.GetEncoding(contentEncoding).GetString(fileByte);

            contents.AppendLine(fileHeader);
            contents.AppendLine(string.Format("Content-Type: {0}", fileContentType));
            contents.AppendLine();
            contents.AppendLine(fileData);

            contents.AppendLine(header);
            contents.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", "key"));
            contents.AppendLine();
            contents.AppendLine(twitpicApiKey);

            contents.AppendLine(header);
            contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "message"));
            contents.AppendLine();
            contents.AppendLine(tweet + " " + Path.GetTempFileName()); // GetTempFileName is to avoid duplicate prevention.

            contents.AppendLine(footer);

            byte[] bytes = Encoding.GetEncoding(contentEncoding).GetBytes(contents.ToString());
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);

                using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                    {
                        string data = reader.ReadToEnd();
                        XmlDocument ret = new XmlDocument();
                        ret.LoadXml(data);
                        XmlElement retElem = ret.DocumentElement["url"];
                        return retElem.InnerText;

                    }
                }
            }
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

        public AccountFlickr testFl()
        {
            return new AccountFlickr() { typeAccount = Account.TypeAccount.Flickr };
        }

        public bool TestAddAccount()
        {
            return Model.AddAccount(new AccountFlickr());
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


        public string GetRequestToken(string callback)//string consumerKey, string consumerSecret
        {
            //SetClientInfo();

            var requestToken = FluentTwitter.CreateRequest()
                .Authentication.GetRequestToken(ConfigurationManager.AppSettings["ConsumerKey"], ConfigurationManager.AppSettings["ConsumerSecret"], callback);

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
