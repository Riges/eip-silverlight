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
using System.ComponentModel;
//using TweetSharp.Model;

namespace EIP.Objects
{
    public class Topic
    {
        //private DateTime dateTime;
        //private Account.TypeAccount typeAccount_2;
        //private TweetSharp.Model.TwitterStatus status;

        public DateTime date { get; set; }
        public Account.TypeAccount typeAccount { get; set; }
        public long accountID { get; set; }

        //public stream_post fb_post  {get; set; }
        public TopicFB fb_post { get; set; }
        public ServiceEIP.TwitterStatus t_post { get; set; }
        


        public Topic()
        {

        }

        /// <summary>
        /// Topic de type Facebook
        /// </summary>
        /// <param name="aDate">Date du post</param>
        /// <param name="aFb_post">value du post Facebook</param>
        public Topic(DateTime aDate, Account.TypeAccount aTypeAccount, long aAccountID, TopicFB aFb_post)
        {
            this.date = aDate;
            this.typeAccount = aTypeAccount;
            this.accountID = aAccountID;

            this.fb_post = aFb_post;
        }

        /// <summary>
        /// Topic de type Twitter
        /// </summary>
        /// <param name="aDate">Date du post</param>
        /// <param name="aFb_post">value du post Facebook</param>
        public Topic(DateTime aDate, Account.TypeAccount aTypeAccount, long aAccountID, TwitterStatus aT_post)
        {
            this.date = aDate;
            this.typeAccount = aTypeAccount;
            this.accountID = aAccountID;

            this.t_post = aT_post;
        }

       
    }
}
