﻿using System;
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
using EIP.ServiceEIP;
using System.Collections.Generic;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountTwitter))]
    public class AccountTwitter : Account
    {
        //public long accountID { get; set; }
        //public TypeAccount typeAccount { get; set; }
        //public long userID { get; set; }

        public string token { get; set; }
        public string tokenSecret { get; set; }
        public string pin { get; set; }
        public TwitterUser user { get; set; }
        public IEnumerable<TwitterStatus> homeStatuses { get; set; }

        public AccountTwitter()
        {
            
        }

    
    }
}
