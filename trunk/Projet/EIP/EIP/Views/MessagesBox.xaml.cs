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
        public String ongletActive;
        private String ongletDefault = "today";
        private DateTime start, end;
        private Dictionary<long, long> nbThreads;

        public MessagesBox()
        {
            InitializeComponent();
            nbThreads = new Dictionary<long, long>();
            
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
                nbThreads.Clear();

                switch (this.boxActive)
                {
                    case "outbox":
                        HeaderText.Text = "Boîte d'envoi";
                        break;
                    case "inbox":
                        HeaderText.Text = "Boîte de réception";
                        break;
                }

                if (this.NavigationContext.QueryString.ContainsKey("onglet"))
                    this.ongletActive = this.NavigationContext.QueryString["onglet"];
                else
                    this.ongletActive = ongletDefault;
                
                ResourceDictionary Resources = App.Current.Resources;
                double offset = 0;
                switch (this.ongletActive)
                {
                    case "yesterday": 
                        start = DateTime.Today.AddDays(-1);
                        end = DateTime.Today;
                        yesterday.Style = Resources["HyperlinkButtonActiveStyle"] as Style;
                        break;

                    case "week":
                        switch (DateTime.Today.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                offset = 0;
                                break;
                            case DayOfWeek.Tuesday:
                                offset = -1;
                                break;
                            case DayOfWeek.Wednesday:
                                offset = -2;
                                break;
                            case DayOfWeek.Thursday:
                                offset = -3;
                                break;
                            case DayOfWeek.Friday:
                                offset = -4;
                                break;
                            case DayOfWeek.Saturday:
                                offset = -5;
                                break;
                            case DayOfWeek.Sunday:
                                offset = -6;
                                break;
                        }
                        start = DateTime.Today.AddDays(offset);
                        end = DateTime.Today.AddDays(1);
                        week.Style = Resources["HyperlinkButtonActiveStyle"] as Style;
                        break;

                    case "lastWeek": 
                        switch (DateTime.Today.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                offset = 0;
                                break;
                            case DayOfWeek.Tuesday:
                                offset = -1;
                                break;
                            case DayOfWeek.Wednesday:
                                offset = -2;
                                break;
                            case DayOfWeek.Thursday:
                                offset = -3;
                                break;
                            case DayOfWeek.Friday:
                                offset = -4;
                                break;
                            case DayOfWeek.Saturday:
                                offset = -5;
                                break;
                            case DayOfWeek.Sunday:
                                offset = -6;
                                break;
                        }
                        start = DateTime.Today.AddDays(-7 + offset);
                        end = DateTime.Today.AddDays(offset);
                        lastWeek.Style = Resources["HyperlinkButtonActiveStyle"] as Style;
                        break;

                    case "month":
                        start = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
                        end = DateTime.Today.AddDays(1);
                        month.Style = Resources["HyperlinkButtonActiveStyle"] as Style;
                        break;

                    case "year":
                        start = DateTime.Today.AddDays(-DateTime.Today.Day + 1).AddMonths(-DateTime.Today.Month + 1);
                        end = DateTime.Today.AddDays(1);
                        year.Style = Resources["HyperlinkButtonActiveStyle"] as Style;
                        break;

                    default:
                    case "today":
                        start = DateTime.Today;
                        end = DateTime.Today.AddDays(1);
                        today.Style = Resources["HyperlinkButtonActiveStyle"] as Style;
                        break;
                }

                foreach (HyperlinkButton link in OngletsNavigation.Children)
                    link.NavigateUri = new Uri("/Messages/" + this.boxActive + "/" + link.Name, UriKind.Relative);
                
                foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
                {
                    if (account.Value.selected)
                    {
                        busyIndicator.IsBusy = true;
                        switch (account.Value.account.typeAccount)
                        {
                            case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                                ((AccountFacebookLight)Connexion.accounts[account.Value.account.accountID]).CountThreadCalled += new AccountFacebookLight.OnCountThreadCompleted(Messages_LoadMessagesFb);
                                
                                switch (this.boxActive)
                                {
                                    case "outbox":
                                        ((AccountFacebookLight)account.Value).CountOutboxMessages(start, end);
                                        break;
                                    case "inbox":
                                        ((AccountFacebookLight)account.Value).CountInboxMessages(start, end);
                                        break;
                                }                                
                            break;
                            case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                                ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled -= new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                                ((AccountTwitterLight)Connexion.accounts[account.Value.account.accountID]).LoadDirectMessagesCalled += new AccountTwitterLight.OnLoadDirectMessagesCompleted(Messages_LoadDirectMessagesCalled);
                                switch (this.boxActive)
                                {
                                    case "outbox":
                                        ((AccountTwitterLight)account.Value).LoadDirectMessagesSent(start, end);
                                        break;
                                    case "inbox":
                                        ((AccountTwitterLight)account.Value).LoadDirectMessagesReceived(start, end);
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


        void Messages_LoadMessagesFb(long count, AccountFacebookLight accountFb)
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                accountFb.CountThreadCalled -= new AccountFacebookLight.OnCountThreadCompleted(Messages_LoadMessagesFb);
                nbThreads.Add(accountFb.account.accountID, count);
                if (count > 0)
                {                                
                    accountFb.GetMessagesCalled -= new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);
                    accountFb.GetMessagesCalled += new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);
                    switch (this.boxActive)
                    {
                        case "outbox":
                            accountFb.LoadOutboxMessages(start, end);
                            break;
                        case "inbox":
                            accountFb.LoadInboxMessages(start, end);
                            break;
                    }

                }
                else
                {
                    Boolean wait = false;
                    foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
                    {
                        if (account.Value.selected && account.Value.account.typeAccount != EIP.ServiceEIP.Account.TypeAccount.Flickr && !nbThreads.ContainsKey(account.Value.account.accountID))
                        {
                            wait = true;
                            break;
                        }
                    }
                    if (!wait)
                        busyIndicator.IsBusy = false;
                }
            });
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
                ResourceDictionary Resources = App.Current.Resources;

                foreach (profile user in th.getRecipients())
                {
                    HyperlinkButton linkBtn = new HyperlinkButton();
                    linkBtn.Content = user.name;
                    linkBtn.Style = Resources["HyperlinkButtonFonceStyle"] as Style;
                    linkBtn.NavigateUri = new Uri("/ProfilInfos/" + user.id + "/Account/" + th.accountID, UriKind.Relative);
                    RecipientsList.Children.Add(linkBtn);

                    TextBlock txt2 = new TextBlock();
                    txt2.Text = compteur++ < th.getRecipients().Count ? (compteur == th.getRecipients().Count ? " et " : ", ") : ".";
                    RecipientsList.Children.Add(txt2);
                }

                HeaderText.Text = th.getSubject();

                busyIndicator.IsBusy = false;
                MessageDefault.Visibility = System.Windows.Visibility.Collapsed;
            });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Connexion.navigationService.CanGoBack)
                Connexion.navigationService.GoBack();
        }

        
    }
}
