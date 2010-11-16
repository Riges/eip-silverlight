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
                    case EIP.ServiceEIP.Account.TypeAccount.Flickr:
                    default:
                        break;
                }
            }*/
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (this.NavigationContext.QueryString.ContainsKey("accountId") && this.NavigationContext.QueryString.ContainsKey("threadId"))
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
                      }*/
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
            }
        }

        void Messages_LoadMessagesFlickr()
        {
            Boolean wait = false;
            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                if (account.Value.selected && account.Value.account.typeAccount != EIP.ServiceEIP.Account.TypeAccount.Flickr)
                {
                    wait = true;
                    break;
                }
            }
            if (!wait)
                busyIndicator.IsBusy = false;
        }

        void Messages_LoadDirectMessagesCalled(List<ThreadMessage> liste)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                listeMessagesBox.box.AddRange(liste);
                listeMessagesBox.box.Sort(delegate(ThreadMessage t1, ThreadMessage t2) { return t2.date.CompareTo(t1.date); });
                listeMessagesBox.LoadMessages();

                busyIndicator.IsBusy = false;
                if (listeMessagesBox.box.Count > 0)
                    MessageDefault.Visibility = System.Windows.Visibility.Collapsed;
                //listeMessagesBox.Messages_GetThreadCalled(th);
            });
        }

        void Messages_GetMessagesCalled(List<ThreadMessage> liste)
        {
            //this.box = liste;
            Connexion.dispatcher.BeginInvoke(() =>
            {
                listeMessagesBox.box.AddRange(liste);
                listeMessagesBox.box.Sort(delegate(ThreadMessage t1, ThreadMessage t2) { return t2.date.CompareTo(t1.date); });
                listeMessagesBox.LoadMessages();

                busyIndicator.IsBusy = false;
                if (listeMessagesBox.box.Count > 0)
                    MessageDefault.Visibility = System.Windows.Visibility.Collapsed;
            });


        }
        void Messages_GetThreadCalled(ThreadMessage th)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                Back.Visibility = System.Windows.Visibility.Visible;
                listeMessagesBox.Messages_GetThreadCalled(th);

                TextBlock txt = new TextBlock();
                txt.Text = "Entre ";
                RecipientsList.Children.Add(txt);

                int compteur = 1;

                foreach (profile user in th.getRecipients())
                {
                    HyperlinkButton linkBtn = new HyperlinkButton();
                    linkBtn.Content = user.name;
                    linkBtn.NavigateUri = new Uri("/ProfilInfos/" + user.id + "/Account/" + th.accountID, UriKind.Relative);
                    RecipientsList.Children.Add(linkBtn);

                    TextBlock txt2 = new TextBlock();
                    txt2.Text = compteur++ < th.getRecipients().Count ? (compteur == th.getRecipients().Count ? " et " : ", ") : ".";
                    RecipientsList.Children.Add(txt2);
                }

                HeaderText.Text = th.getSubject();

                busyIndicator.IsBusy = false;
            });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Connexion.navigationService.CanGoBack)
                Connexion.navigationService.GoBack();
        }

        
    }
}
