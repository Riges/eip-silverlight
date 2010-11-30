using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace EIPLibrary
{
    //[SerializableAttribute()]
    [KnownType(typeof(AccountFacebook))]
    [KnownType(typeof(AccountTwitter))]
    [KnownType(typeof(AccountFlickr))]
    [KnownType(typeof(AccountRSS))]
    [ServiceContract]
    [Serializable]
    [DataContract]
    public class Account : object
    {
        [DataMember]
        public long accountID;

        [DataMember]
        public long groupID { get; set; }

        [DataMember]
        public TypeAccount typeAccount { get; set; }

        [DataMember]
        public long userID { get; set; }

        [DataMember]
        public string name { get; set; }


        public enum TypeAccount
        {
            Facebook,
            Twitter,
            Flickr,
            RSS
        }

       

        public Account()
        {

        }


    }
}
