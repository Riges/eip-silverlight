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
using Facebook.Schema;

namespace EIP.Objects
{
    public class MessageFacebook
    {
        private profile author { get; set; }
        private message message { get; set; }

        public MessageFacebook(message mess, profile unUser)
        {
            this.message = mess;
            this.author = unUser;
        }

        public MessageFacebook(){}

    }
}
