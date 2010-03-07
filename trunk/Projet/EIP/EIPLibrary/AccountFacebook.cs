using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;
//using Facebook;

namespace EIPLibrary
{

    [Serializable]
    [DataContract]
    [ServiceContract]
    public class AccountFacebook : Account
    {
        [DataMember]
        public bool sessionExpires { get; set; }

        [DataMember]
        public string sessionKey { get; set; }

        [DataMember]
        public string sessionSecret { get; set; }



        public AccountFacebook()
        {

        }


    }
}
