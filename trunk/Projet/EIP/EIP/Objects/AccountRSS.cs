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
using System.ServiceModel.Syndication;
using System.Linq;
using System.Xml.Linq;
using EIP.ServiceEIP;

namespace EIP.Objects
{
    [KnownTypeAttribute(typeof(AccountFlickrLight))]
    public class AccountRSSLight : AccountLight
    {
        SyndicationFeed feeds { get; set; }

        public AccountRSSLight()
        {
            this.feeds = new SyndicationFeed();
            Connexion.serviceEIP.GetRSSCompleted += new EventHandler<ServiceEIP.GetRSSCompletedEventArgs>(serviceEIP_GetRSSCompleted);
        }

        public void LoadFeeds()
        {
            Connexion.serviceEIP.GetRSSAsync(((AccountRSS)this.account).url, this.account.accountID);
        }

        void serviceEIP_GetRSSCompleted(object sender, ServiceEIP.GetRSSCompletedEventArgs e)
        {
            if (e.Error == null && this.account.accountID == (long)e.UserState)
            {
                feeds = SyndicationFeed.Load(e.Result);
                
                //articles = (from article in feeds.Items
                //            select new Article
                //            {
                //                Titre = article.Title.Text,
                //                Lien = article.Links.First().Uri.ToString()
                //            }).ToList();
            }
        }

    
    }
}
