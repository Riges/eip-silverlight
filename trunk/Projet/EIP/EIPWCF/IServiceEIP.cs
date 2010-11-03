using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using EIPLibrary;
using TweetSharp.Twitter.Model;
using System.IO;
//using Dimebrain.TweetSharp.Model;
//using Dimebrain.TweetSharp.Fluent;

namespace EIPWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceEIP
    {
        [OperationContract]
        bool IsUp();

        [OperationContract]
        string GetFBAppKey();

        /*
        [OperationContract]
        bool test(Account newAccount);
         * */

        [OperationContract]
        Account GetAccountByUserID(long userID);

        [OperationContract]
        List<Account> GetAccountsByUserID(long userID);

        [OperationContract]
        List<Account> GetAccountsByGroupID(long groupID);

        [OperationContract]
        List<Account> GetAccountsByTwitter(string pseudo, string password);

        [OperationContract]
        bool AddAccount(Account newAccount, string token, string pin);

        [OperationContract]
        bool SaveAccount(Account accountToSave);

        [OperationContract]
        long DeleteAccount(long accountID);



        [OperationContract]
        AccountFacebook testfb();

        [OperationContract]
        AccountTwitter testT();




        [OperationContract]
        string GetRequestToken();

        [OperationContract]
        IEnumerable<TwitterStatus> LoadHomeStatuses(string token, string tokenSecret);

        [OperationContract]
        bool SendTweet(string token, string tokenSecret, string tweet);

        [OperationContract]
        TwitterUser GetUserInfos(string token, string tokenSecret, long userId);


        [OperationContract]
        List<TwitterUser> GetFiends(string token, string tokenSecret);

        [OperationContract]
        string UploadPhoto(string name, byte[] img);

        [OperationContract]
        IEnumerable<TwitterDirectMessage> LoadDirectMessages(string token, string tokenSecret);

        /*[OperationContract]
        string AuthorizeDesktop(string consumerKey, string consumerSecret);
        */
       /* [OperationContract]
        AccountTwitter GetAccessToken(string consumerKey, string consumerSecret, string token, string pin);
        */
       /* [OperationContract]
        TwitterUser TwitterGetUserInfo(string consumerKey, string consumerSecret, string token, string tokenSecret, long userId);
        */
        /*[OperationContract]
        long TwitterCheckUserInfo(string username, string password);
        */
       /* [OperationContract]
        IEnumerable<TwitterStatus> TwitterGetHomeStatuses(string consumerKey, string consumerSecret, string token, string tokenSecret);


        [OperationContract]
        IFluentTwitter GetFluent();
        */
        
    
    }



}
