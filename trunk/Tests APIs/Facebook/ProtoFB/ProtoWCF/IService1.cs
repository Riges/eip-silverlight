using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using Dimebrain.TweetSharp.Model;

namespace ProtoWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace = "")]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        /*[OperationContract]
        bool LoginTwitter();
        */

        [OperationContract]
        IEnumerable<TwitterStatus> PublicStatues(string pseudo, string password);

        [OperationContract]
        string AuthorizeDesktop(string consumerKey, string consumerSecret);

        [OperationContract]
        string GetAccessToken(string consumerKey, string consumerSecret, string token, string pin);

        [OperationContractAttribute(AsyncPattern = true)]
        IAsyncResult BeginLoginTwitter(string pseudo, string passord, AsyncCallback callback, object asyncState);

        //Note: There is no OperationContractAttribute for the end method.
        bool EndLoginTwitter(IAsyncResult result);


        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
