using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFacebookLight))]
    public class AccountFacebookLight : AccountLight
    {
        //public long accountID { get; set; }
        //public TypeAccount typeAccount { get; set; }
        //public long userID { get; set; }
        /*
        public bool sessionExpires { get; set; }
        public string sessionKey { get; set; }
        public string sessionSecret { get; set; }

        */

        public AccountFacebookLight()
        {
            this.account = new ServiceEIP.Account();
        }

    
    }
}
