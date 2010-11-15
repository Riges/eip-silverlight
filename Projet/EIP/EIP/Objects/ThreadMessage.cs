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
using System.Collections.Generic;

namespace EIP.Objects
{
    public class ThreadMessage
    {
        public DateTime date { get; set; }
        public Account.TypeAccount typeAccount { get; set; }
        public long accountID { get; set; }

        // Facebook
        private thread MessageFb { get; set; }
        private profile authorFb { get; set; }
        private List<profile> recipientsFb { get; set; }
        private List<MessageFacebook> messagesFb { get; set; }

        // Twitter
        private TwitterDirectMessage MessageTwitter { get; set; }

        public ThreadMessage() { }

        // Facebook
        public ThreadMessage(thread th, long accountID)
        {
            this.MessageFb = th;
            this.accountID = accountID;
            
            this.typeAccount = Account.TypeAccount.Facebook;
            this.date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.date = this.date.AddSeconds(th.updated_time).AddHours(2); // TODO : GTM parametrable
        }

        public ThreadMessage(TwitterDirectMessage dm, long accountID)
        {
            this.MessageTwitter = dm;
            this.accountID = accountID;

            this.typeAccount = Account.TypeAccount.Twitter;
            this.date = dm.CreatedDate;
        }


        public String getSubject()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb.subject != "" ? this.MessageFb.subject : "(Sans objet)";

                case Account.TypeAccount.Twitter:
                    return "";
            }
            return null;
        }

        public String getContent()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return "";

                case Account.TypeAccount.Twitter:
                    return this.MessageTwitter.Text;
            }
            return null;
        }

        public String getSummary()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb.snippet;

                case Account.TypeAccount.Twitter:
                    return "";
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

        // TODO : si user courant, prendre un des destinataires
        public void setAuthor(object author)
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    this.authorFb = (profile)author;
                    break;
            }
        }

        public String getAuthorName()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    if (this.authorFb != null)
                        return this.authorFb.name;
                    else
                        return "Undefined";


                case Account.TypeAccount.Twitter:
                    return this.MessageTwitter.Sender.ScreenName;
            }
            return null;
        }



        public String getPic()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    if (this.authorFb != null && this.authorFb.pic_square != null && this.authorFb.pic_square != "")
                        return this.authorFb.pic_square;
                    break;

                case Account.TypeAccount.Twitter:
                    return this.MessageTwitter.Sender.ProfileImageUrl;
                    break;
            }
            return null;
        }

        public Boolean hasDetails()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return true;
                    break;
            }
            return false;
        }


        public thread getThread()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb;
                    break;
            }
            return null;
        }

        public long getThreadId()
        {
            switch (this.typeAccount)
            {
                case Account.TypeAccount.Facebook:
                    return this.MessageFb.thread_id;
                    break;
            }
            return 0;
        }

        // Facebook, a adapter pour twitter et autres
        public void setMessages(List<MessageFacebook> liste)
        {
            this.messagesFb = liste;
        }
        public List<MessageFacebook> getMessages()
        {
            return this.messagesFb;
        }
        public void setRecipients(List<profile> users)
        {
            recipientsFb = users;
        }
        public List<profile> getRecipients()
        {
            return recipientsFb;
        }
    }
}
