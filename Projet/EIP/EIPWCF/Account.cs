using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EIP
{
    public class Account
    {
        public long accountID { get; set; }
        public TypeAccount typeAccount { get; set; }
        public long userID { get; set; }
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
