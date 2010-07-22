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
using EIP.ServiceEIP;

namespace EIP.Objects
{
    public class ThreadMessage
    {
        public DateTime date { get; set; }
        public Account.TypeAccount typeAccount { get; set; }

        // Facebook
        private thread MessageFb { get; set; }

        public ThreadMessage() { }

        // Facebook
        public ThreadMessage(thread th)
        {
            this.MessageFb = th;
            
            this.typeAccount = Account.TypeAccount.Facebook;
            this.date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.date = this.date.AddSeconds(th.updated_time).AddHours(2); // TODO : GTM parametrable
        }

        public String getSubject()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb.subject;
            }
            return null;
        }

        public String getSummary()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb.snippet;
            }
            return null;
        }

        public long getAuthorAccountID()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb.snippet_author;
            }
            return 0;
        }


    }
}
