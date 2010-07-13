using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using EIP.Objects;
using Facebook.Schema;

namespace EIP.Views
{
    public partial class MessagesBox : Page
    {
        //public Dictionary<String, Friend> friends;
        
        public String boxActive;

        public MessagesBox()
        {
            InitializeComponent();
            
            /*foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        inbox = ((AccountFacebookLight)account.Value).inbox;
                        outbox = ((AccountFacebookLight)account.Value).outbox;
                        break;

                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                    case EIP.ServiceEIP.Account.TypeAccount.Myspace:
                    default:
                        break;
                }
            }*/
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("box"))
                this.boxActive = this.NavigationContext.QueryString["box"];

            

            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        switch (this.boxActive)
                        {
                            case "outbox":
                                HeaderText.Text = "Boîte d'envoi";
                                break;

                            case "inbox":
                            default:
                                HeaderText.Text = "Boîte de réception";
                                switch (this.boxActive)
                                {
                                    case "outbox":
                                        HeaderText.Text = "Boîte d'envoi";
                                        ((AccountFacebookLight)account.Value).LoadOutboxMessages();
                                        break;

                                    case "inbox":
                                    default:
                                        HeaderText.Text = "Boîte de réception";
                                        ((AccountFacebookLight)account.Value).LoadInboxMessages();
                                        // this.box = ((AccountFacebookLight)account.Value).inbox;
                                        break;
                                }
                                
                                break;
                        }
                        break;

                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                    case EIP.ServiceEIP.Account.TypeAccount.Myspace:
                    default:
                        break;
                }
            }


        }

        /// <summary>
        /// methode qui merge les listes de friends
        /// pitetre un jour
        /// </summary>
       /* private void LoadList()
        {
            foreach(KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        List<thread> friendsFB = ((AccountFacebookLight)account.Value).inbox;
                        foreach (user toto in friendsFB)
                        {
                            if (friends.Keys.Contains(toto.proxied_email))
                            {
                                if (friends[toto.proxied_email].userFB == null)
                                    friends[toto.proxied_email].userFB = toto;
                            }
                            else
                            {
                                Friend titi = new Friend();
                                titi.userFB = toto;
                                friends.Add(toto.proxied_email, titi);
                            }
                        }
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:

                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
            }

        }*/
    }
}
