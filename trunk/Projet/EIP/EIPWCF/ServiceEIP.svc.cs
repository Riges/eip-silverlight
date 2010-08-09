using System;
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


namespace EIPWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ServiceEIP : IServiceEIP
    {
        public bool IsUp()
        {
            return Model.IsDBUp();
        }
        /*
        public bool test(Account newAccount)
        {
            return AddAccount(newAccount);
        }*/

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


/*
        public string AuthorizeDesktop(string consumerKey, string consumerSecret)
        {
            var requestToken = GetRequestToken(consumerKey, consumerSecret);
            
            
            FluentTwitter.CreateRequest()
               .Authentication
               .AuthorizeDesktop(consumerKey,
                                 consumerSecret,
                                 requestToken.Token);
             
            
            return requestToken.Token;
        }*/
        /*
        public IFluentTwitter GetFluent()
        {
            return FluentTwitter.CreateRequest();
        }*/
        /*
        private static void GetResponse(TwitterResult response)
        {
            var identity = response.AsUser();
            if (identity != null)
            {
                Console.WriteLine("{0} authenticated successfully.", identity.ScreenName);
            }
            else
            {
                var error = response.AsError();
                if (error != null)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
        }*/
        

        public string GetRequestToken(string consumerKey, string consumerSecret)
        {
            var requestToken = FluentTwitter.CreateRequest()
                .Authentication.GetRequestToken(consumerKey, consumerSecret);

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
        /*
        public AccountTwitter GetAccessToken(string consumerKey, string consumerSecret, string token, string pin)
        {
           
            var accessToken = FluentTwitter.CreateRequest()
                .Authentication.GetAccessToken(consumerKey, consumerSecret, token, pin);

            var response = accessToken.Request();
            var result = response.AsToken();

            if (result == null)
            {
               
                var error = response.AsError();
                if (error != null)
                {
                    throw new Exception(error.ErrorMessage);
                }
            }
            AccountTwitter accountTwitter = new AccountTwitter();
            
            accountTwitter.token = result.Token;
            accountTwitter.tokenSecret = result.TokenSecret;
            accountTwitter.name = result.ScreenName;
            accountTwitter.userID =  Convert.ToInt64(result.UserId);

            

            return accountTwitter;// result.TokenSecret;
        }
        */
        /*
        public TwitterUser TwitterGetUserInfo(string consumerKey, string consumerSecret, string token, string tokenSecret, long userId)
        {
            var userInfo = FluentTwitter.CreateRequest()
               .AuthenticateWith(consumerKey, consumerSecret, token, tokenSecret)
               .Users().ShowProfileFor(userId);

            var response = userInfo.Request();
            var result = response.AsUser();

            return result;
        }*/
        /*
        public long TwitterCheckUserInfo(string username, string password)
        {
            var userInfo = FluentTwitter.CreateRequest()
               .AuthenticateAs(username, password);
               
            var response = userInfo.Request();

            var result = response.AsToken();

            return Convert.ToInt64(result.UserId);
        }*/
        /*
        public IEnumerable<TwitterStatus> TwitterGetHomeStatuses(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            var userInfo = FluentTwitter.CreateRequest()
               .AuthenticateWith(consumerKey, consumerSecret, token, tokenSecret)
               .Statuses().OnHomeTimeline();

            var response = userInfo.Request();
            var result = response.AsStatuses();

            return result;
        }*/


        

        
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
