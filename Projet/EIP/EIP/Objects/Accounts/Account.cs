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

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountFacebookLight))]
    [KnownTypeAttribute(typeof(AccountTwitterLight))]
    [KnownTypeAttribute(typeof(AccountFlickrLight))]
    public class AccountLight
    {
        public Account account { get; set; }
        public bool selected { get; set; }

 
        public AccountLight()
        {
            
        }

    }

    
}
