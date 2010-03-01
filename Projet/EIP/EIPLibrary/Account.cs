using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace EIPLibrary
{
    [ServiceContract]
    public class Account
    {
        [DataMember]
        public long accountID { get; set; }

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
            Myspace
        }


        public Account()
        {

        }


    }
}
