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
    public class AccountRSS : Account
    {
        [DataMember]
        public string url { get; set; }

        public AccountRSS()
        {

        }
    }
}
