using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using EIP;
using Dimebrain.TweetSharp.Model;

namespace EIPWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IServiceEIP
    {

        [OperationContract]
        string AuthorizeDesktop(string consumerKey, string consumerSecret);

        [OperationContract]
        AccountTwitter GetAccessToken(string consumerKey, string consumerSecret, string token, string pin);

        [OperationContract]
        TwitterUser TwitterGetUserInfo(string consumerKey, string consumerSecret, string token, string tokenSecret, long userId);

        [OperationContract]
        IEnumerable<TwitterStatus> TwitterGetHomeStatuses(string consumerKey, string consumerSecret, string token, string tokenSecret);

        
    }



}
