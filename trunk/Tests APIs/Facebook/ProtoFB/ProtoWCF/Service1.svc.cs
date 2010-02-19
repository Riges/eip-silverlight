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

namespace ProtoWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.

    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }



        /*
        public bool LoginTwitter()
        {
          
            
            TwitterClientInfo clientInfo = new TwitterClientInfo();
            clientInfo.ConsumerKey = "BuHnRBigk7Z9ODANTQxxLg";
            clientInfo.ConsumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

            var twitter = FluentTwitter.CreateRequest(clientInfo);
            twitter.AuthenticateAs("pocketino", "fdsfds");
           
            twitter.Request();
            
            return true;
           
        }
        */

        public bool LoginTwitter(string pseudo, string password)
        {
            Console.WriteLine("Called synchronous sample method with \"{0}\"", pseudo);
           // return "The sychronous service greets you: " + pseudo;

            return false;
        }

        public IEnumerable<TwitterStatus> PublicStatues(string pseudo, string password)
        {
            // Get the public timeline
            var twitter = FluentTwitter.CreateRequest().AuthenticateAs(pseudo, password)
                 .Statuses().OnHomeTimeline().AsXml();  
              
            // Sequential call for data  
            var response = twitter.Request();  
              
            // Convert response to data classes  
            var statuses = response.AsStatuses();
            return statuses;
        }

        private string consumerKey = "BuHnRBigk7Z9ODANTQxxLg";
        private string consumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";


        public string AuthorizeDesktop(string consumerKey, string consumerSecret)
        {
            var requestToken = GetRequestToken(consumerKey, consumerSecret);

            FluentTwitter.CreateRequest()
               .Authentication
               .AuthorizeDesktop(consumerKey,
                                 consumerSecret,
                                 requestToken.Token);

            

            return requestToken.Token;
        }

        // This asynchronously implemented operation is never called because 
        // there is a synchronous version of the same method.
        public IAsyncResult BeginLoginTwitter(string pseudo, string password, AsyncCallback callback, object asyncState)
        {
            Console.WriteLine("BeginSampleMethod called with: " + pseudo);

            /*
            var requestToken = GetRequestToken(consumerKey, consumerSecret);

            // automatically starts the default web browser, sending the 
            // user to the authorization URL.
            FluentTwitter.CreateRequest()
                .Authentication
                .AuthorizeDesktop(consumerKey,
                                  consumerSecret,
                                  requestToken.Token);
            */


            /*TwitterClientInfo clientInfo = new TwitterClientInfo();
            clientInfo.ConsumerKey = "BuHnRBigk7Z9ODANTQxxLg";
            clientInfo.ConsumerSecret = "UkVn1sB1MkUwcHEKcWERsBHTEc0REPn5vdw4jDqk4";

            var twitter = FluentTwitter.CreateRequest(clientInfo);
            twitter.AuthenticateAs(pseudo, password);

            twitter.Authentication.GetRequestToken(clientInfo.ConsumerKey, clientInfo.ConsumerSecret);
           */
            //twitter.Request();

             var twitter = FluentTwitter.CreateRequest()
                 .AuthenticateAs(pseudo, password).Users().GetFollowers();
                 //.Statuses().Update("Mon status à mettre à jour");

    // On vérifie que tout s'est bien passé

            var response = twitter.Request();
            

            if (response.IsTwitterError)
                return new CompletedAsyncResult<bool>(false);
            else
                return new CompletedAsyncResult<bool>(true);


            
        }

        public bool EndLoginTwitter(IAsyncResult r)
        {
            CompletedAsyncResult<bool> result = r as CompletedAsyncResult<bool>;
            Console.WriteLine("EndSampleMethod called with: " + result.Data);
            return result.Data;
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

        public string GetAccessToken(string consumerKey, string consumerSecret, string token, string pin)
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

            return result.TokenSecret;
        }
        
        
        
        
        
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
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
