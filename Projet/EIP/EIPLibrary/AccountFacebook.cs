using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Facebook;

namespace EIPLibrary
{
    public class AccountFacebook : Account
    {
        public bool sessionExpires { get; set; }
        public string sessionKey { get; set; }
        public string sessionSecret { get; set; }



        public AccountFacebook()
        {

        }


    }
}
