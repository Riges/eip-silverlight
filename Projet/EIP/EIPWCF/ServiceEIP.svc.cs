using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Dimebrain.TweetSharp;
using Dimebrain.TweetSharp.Fluent;
using Dimebrain.TweetSharp.Extensions;
using Dimebrain.TweetSharp.Model;
using System.Threading;
using EIP;

namespace EIPWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class ServiceEIP : IServiceEIP
    {

        public string AuthorizeDesktop(string consumerKey, string consumerSecret)
        {
            var requestToken = GetRequestToken(consumerKey, consumerSecret);
            
            /*
            FluentTwitter.CreateRequest()
               .Authentication
               .AuthorizeDesktop(consumerKey,
                                 consumerSecret,
                                 requestToken.Token);
             */
            
            return requestToken.Token;
        }

        public IFluentTwitter GetFluent()
        {
            return FluentTwitter.CreateRequest();
        }

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
        }

        private static OAuthToken GetRequestToken(string consumerKey, string consumerSecret)
        {
            var requestToken = FluentTwitter.CreateRequest()
                .Authentication.GetRequestToken(consumerKey, consumerSecret);

            var response = requestToken.Request();
            var result = response.AsToken();

            if (result == null)
            {
                var error = response.AsError();
                if (error != null)
                {
                    throw new Exception(error.ErrorMessage);
                }
            }

            return result;
        }

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

        public TwitterUser TwitterGetUserInfo(string consumerKey, string consumerSecret, string token, string tokenSecret, long userId)
        {
            var userInfo = FluentTwitter.CreateRequest()
               .AuthenticateWith(consumerKey, consumerSecret, token, tokenSecret)
               .Users().ShowProfileFor(userId);

            var response = userInfo.Request();
            var result = response.AsUser();

            return result;
        }

        public long TwitterCheckUserInfo(string username, string password)
        {
            var userInfo = FluentTwitter.CreateRequest()
               .AuthenticateAs(username, password);
               
            var response = userInfo.Request();

            var result = response.AsToken();

            return Convert.ToInt64(result.UserId);
        }

        public IEnumerable<TwitterStatus> TwitterGetHomeStatuses(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            var userInfo = FluentTwitter.CreateRequest()
               .AuthenticateWith(consumerKey, consumerSecret, token, tokenSecret)
               .Statuses().OnHomeTimeline();

            var response = userInfo.Request();
            var result = response.AsStatuses();

            return result;
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
