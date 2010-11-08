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
            e = e;
            if (this.NavigationContext.QueryString.ContainsKey("accountId") && this.NavigationContext.QueryString.ContainsKey("threadId"))
            {
                if (this.NavigationContext.QueryString.ContainsKey("box"))
                    this.boxActive = this.NavigationContext.QueryString["box"];

                long accountId = Convert.ToInt64(this.NavigationContext.QueryString["accountId"]);
                switch (Connexion.accounts[accountId].account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        //((AccountFacebookLight)Connexion.accounts[accountId]).GetMessagesCalled += new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);
                        ((AccountFacebookLight)Connexion.accounts[accountId]).GetThreadCalled += new AccountFacebookLight.OnGetThreadCompleted(Messages_GetThreadCalled);
                        ((AccountFacebookLight)Connexion.accounts[accountId]).LoadThread(Convert.ToInt64(this.NavigationContext.QueryString["threadId"]));
                        /*switch (this.boxActive)
                      {
                          case "outbox":
                              HeaderText.Text = "Boîte d'envoi";
                              ((AccountFacebookLight)account.Value).LoadOutboxMessages();
                              break;
                          case "inbox":
                              HeaderText.Text = "Boîte de réception";
                              ((AccountFacebookLight)account.Value).LoadInboxMessages();
                              //this.box = ((AccountFacebookLight)account.Value).inbox;
                              break;
                      }*/
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                        //((AccountTwitterLight)Connexion.accounts[accountId]).LoadDirectMessagesCalled += new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Myspace:
                        break;
                }
            }
            else if (this.NavigationContext.QueryString.ContainsKey("box"))
            {
                this.boxActive = this.NavigationContext.QueryString["box"];
                listeMessagesBox.box.Clear();
                foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
                {
                    if (account.Value.selected)
                    {
                        switch (account.Value.account.typeAccount)
                        {
                            case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).GetMessagesCalled += new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);
                                switch (this.boxActive)
                                {
                                    case "outbox":
                                        HeaderText.Text = "Boîte d'envoi";
                                        ((AccountFacebookLight)account.Value).LoadOutboxMessages();
                                        break;
                                    case "inbox":
                                        HeaderText.Text = "Boîte de réception";
                                        ((AccountFacebookLight)account.Value).LoadInboxMessages();
                                        //this.box = ((AccountFacebookLight)account.Value).inbox;
                                        break;
                                }
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                                ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled += new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                                ((AccountTwitterLight)account.Value).LoadDirectMessages();
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Myspace:
                                break;
                        }
                    }
                }
            }
            e = e;
        }

        void Messages_LoadDirectMessagesCalled(List<ThreadMessage> liste)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                //liste = liste;

                if (listeMessagesBox.box.Count > 0)
                    listeMessagesBox.box.AddRange(liste);
                else
                    listeMessagesBox.box = liste;

                //listeMessagesBox.box.Sort(delegate(ThreadMessage t1, ThreadMessage t2) { return t2.date.CompareTo(t1.date); });
                listeMessagesBox.LoadMessages();
                //listeMessagesBox.Messages_GetThreadCalled(th);
            });
        }

        void Messages_GetMessagesCalled(List<ThreadMessage> liste)
        {
            //this.box = liste;
            Connexion.dispatcher.BeginInvoke(() =>
            {
                if (listeMessagesBox.box.Count > 0)
                    listeMessagesBox.box.AddRange(liste);
                else
                    listeMessagesBox.box = liste;
                //listeMessagesBox.box.Sort(delegate(ThreadMessage t1, ThreadMessage t2) { return t2.date.CompareTo(t1.date); });
                //listeMessagesBox.box = liste;
                listeMessagesBox.LoadMessages();
                //MessageBox toto = new MessageBox("", "invoked count=" + liste.Count);
                //toto.Show();
            });


        }
        void Messages_GetThreadCalled(ThreadMessage th)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                listeMessagesBox.Messages_GetThreadCalled(th);
            });
        }

        
    }
}
