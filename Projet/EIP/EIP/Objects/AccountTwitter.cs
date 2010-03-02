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
//using EIP.ServiceEIP;
using System.Collections.Generic;
using TweetSharp.Fluent;
using TweetSharp.Model;
using TweetSharp.Extensions;
using EIP.ServiceEIP;

namespace EIP
{
    [KnownTypeAttribute(typeof(AccountTwitterLight))]
    public class AccountTwitterLight : AccountLight
    {
        //public long accountID { get; set; }
        //public TypeAccount typeAccount { get; set; }
        //public long userID { get; set; }

        public string token { get; set; }
        public string tokenSecret { get; set; }
        public string pin { get; set; }
        public TwitterUser user { get; set; }
        public IEnumerable<TwitterStatus> homeStatuses { get; set; }

        private ItemsControl itemsControl;

        public AccountTwitterLight()
        {
            this.account = new Account();
        }

          //*******************************\\
         //*Methode de récupération d'infos*\\
        //***********************************\\

        /// <summary>
        /// Met à jour l'attribut "homeStatuses" (les tweets de la homepage)
        /// </summary>
        public void LoadHomeStatuses()
        {
            //Connexion.serviceEIP.TwitterGetHomeStatusesCompleted += new EventHandler<TwitterGetHomeStatusesCompletedEventArgs>(serviceEIP_TwitterGetHomeStatusesCompleted);
            //Connexion.serviceEIP.TwitterGetHomeStatusesAsync(Connexion.consumerKey, Connexion.consumerSecret, this.token, this.tokenSecret);

            var homeTimeline = FluentTwitter.CreateRequest()
               .Configuration.UseTransparentProxy(Connexion.ProxyUrl)
               .AuthenticateWith(token, tokenSecret)
               .Statuses().OnHomeTimeline()
               .CallbackTo(HomeTimelineReceived);

            homeTimeline.RequestAsync();
        }

        //private  void serviceEIP_TwitterGetHomeStatusesCompleted(object sender, TwitterGetHomeStatusesCompletedEventArgs e)
        private void HomeTimelineReceived(object sender, TwitterResult result)
        {
            var statuses = result.AsStatuses();

            if ((this.account.typeAccount == Account.TypeAccount.Twitter) && (result.AsError() == null) && (statuses != null))
            {
                this.homeStatuses = statuses;
                Connexion.SaveAccount();
            }
        }

    
    }
}
