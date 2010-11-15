using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace EIPLibrary
{
    [Serializable]
    [DataContract]
    [ServiceContract]
    public class AccountFlickr : Account
    {
        [DataMember]
        public string token { get; set; }

        [DataMember]
        public string userIDstr { get; set; }

        public AccountFlickr()
        {

        }


    }
}
