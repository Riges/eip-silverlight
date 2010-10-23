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
        public long accountID { get; set; }

        public MessageFacebook(message mess, profile unUser, long accountID)
        {
            this.message = mess;
            this.author = unUser;
            this.accountID = accountID;
        }

        public MessageFacebook(){}

        public String getPic()
        {
            return this.author.pic_square;
        }

        public String getContent()
        {
            return this.message.body;
        }

        public String getAuthorName()
        {
            return this.author.name;
        }

        public long getAuthorAccountID()
        {
            return this.author.id;
        }

        public long getDate()
        {
            return this.message.created_time;
        }
    }
}
