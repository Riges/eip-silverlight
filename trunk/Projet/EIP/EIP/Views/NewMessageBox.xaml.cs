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
    public partial class NewMessageBox : Page
    {
        //public Dictionary<String, Friend> friends;
        
        public String boxActive;

        public NewMessageBox()
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
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                    default:
                        break;
                }
            }*/
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            busyIndicator.IsBusy = true;
            /*if (this.NavigationContext.QueryString.ContainsKey("accountId") && this.NavigationContext.QueryString.ContainsKey("threadId"))
            {
                busyIndicator.IsBusy = true;
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
                      }
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                        //((AccountTwitterLight)Connexion.accounts[accountId]).LoadDirectMessagesCalled += new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                        break;
                }
            }
            else if (this.NavigationContext.QueryString.ContainsKey("box"))
            {
                this.boxActive = this.NavigationContext.QueryString["box"];
                listeMessagesBox.box.Clear();

                switch (this.boxActive)
                {
                    case "outbox":
                        HeaderText.Text = "Boîte d'envoi";
                        break;
                    case "inbox":
                        HeaderText.Text = "Boîte de réception";
                        break;
                }

                foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
                {
                    if (account.Value.selected)
                    {
                        busyIndicator.IsBusy = true;
                        switch (account.Value.account.typeAccount)
                        {
                            case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).GetMessagesCalled -= new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).GetMessagesCalled += new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);
                                switch (this.boxActive)
                                {
                                    case "outbox":
                                        ((AccountFacebookLight)account.Value).LoadOutboxMessages();
                                        break;
                                    case "inbox":
                                        ((AccountFacebookLight)account.Value).LoadInboxMessages();
                                        //this.box = ((AccountFacebookLight)account.Value).inbox;
                                        break;
                                }
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                                ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled -= new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                                ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled += new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                                switch (this.boxActive)
                                {
                                    case "outbox":
                                        ((AccountTwitterLight)account.Value).LoadDirectMessagesSent();
                                        break;
                                    case "inbox":
                                        ((AccountTwitterLight)account.Value).LoadDirectMessagesReceived();
                                        break;
                                }                                
                                break;
                            case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                                Messages_LoadMessagesFlickr();
                                break;
                        }
                    }
                }
            }*/
            long compteurTwitter = 0;
            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                        /*((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled -= new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                        ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled += new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                        switch (this.boxActive)
                        {
                            case "outbox":
                                ((AccountTwitterLight)account.Value).LoadDirectMessagesSent();
                                break;
                            case "inbox":
                                ((AccountTwitterLight)account.Value).LoadDirectMessagesReceived();
                                break;
                        }*/
                        compteurTwitter++;
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                        break;
                }

                if (compteurTwitter == 0)
                {
                    // Affichage message d'erreur et cachage formulaire
                    MessageDefault.Visibility = System.Windows.Visibility.Visible;
                    //cachage formulaire TODO
                    busyIndicator.IsBusy = false;
                }

            }
        }

        

        
    }
}
