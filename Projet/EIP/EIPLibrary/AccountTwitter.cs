using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Dimebrain.TweetSharp.Model;


namespace EIPLibrary
{
    public class AccountTwitter : Account
    {
        //public long accountID { get; set; }
        //public TypeAccount typeAccount { get; set; }
        //public long userID { get; set; }

        public string token { get; set; }
        public string tokenSecret { get; set; }
        public string pin { get; set; }
        //public TwitterUser user { get; set; }
        //public IEnumerable<TwitterStatus> homeStatuses { get; set; }

        public AccountTwitter()
        {

        }

        //*******************************\\
        //*Methode de récupération d'infos*\\
        //***********************************\\

        /// <summary>
        /// Met à jour l'attribut "homeStatuses"
        /// </summary>
        public void LoadHomeStatuses()
        {
            //Connexion.serviceEIP.TwitterGetHomeStatusesCompleted += new EventHandler<TwitterGetHomeStatusesCompletedEventArgs>(serviceEIP_TwitterGetHomeStatusesCompleted);
            //Connexion.serviceEIP.TwitterGetHomeStatusesAsync(Connexion.consumerKey, Connexion.consumerSecret, this.token, this.tokenSecret);
        }

       /* private void serviceEIP_TwitterGetHomeStatusesCompleted(object sender, TwitterGetHomeStatusesCompletedEventArgs e)
        {
            if (this.typeAccount == Account.TypeAccount.Twitter && e != null && e.Result != null)
            {
                this.homeStatuses = e.Result;
                Connexion.SaveAccount();
            }
        }*/


    }
}
