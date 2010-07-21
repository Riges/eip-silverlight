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
using Facebook.Schema;
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class listeMessages : UserControl
    {
        public List<ThreadMessage> box { get; set; }
        public long accountID { get; set; }

        public listeMessages()
        {
            InitializeComponent();
        }


        private void LoadMessagesControl()
        {
            //((AccountFacebookLight)Connexion.accounts[this.accountID]).GetMessagesCalled += new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);

            /*
            if (this.box.Count > this.box.comment_list.comment.Count())
            {
                HyperlinkButton linkBtn = new HyperlinkButton();
                linkBtn.Content = "Afficher les " + this.Commentaires.count + " commentaires";
                linkBtn.Click += new RoutedEventHandler(linkBtn_Click);
                displayAllComsPanel.Children.Add(linkBtn);
            }
             * */

            //LoadMessages();
        }


        public void LoadMessages()
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                /*string tmp = this.box.Count.ToString();
                MessageBox toto = new MessageBox("", "LoadMessages " + tmp);
                toto.Show();*/
                
                if (this.box.Count > 0)
                {
                    messagesPanel.Children.Clear();
                    foreach (ThreadMessage thread in this.box)
                    {
                        Message monMessage = new Message(thread.getSubject(), thread.getSummary());
                        messagesPanel.Children.Add(monMessage);
                    }
                    //FeedsControl.DataContext = this.box;

                }
            });
        }
    }
}
