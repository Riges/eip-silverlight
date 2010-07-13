﻿using System;
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

namespace EIP.Views.Controls
{
    public partial class listeMessages : UserControl
    {
        private List<thread> box;
        public long accountID { get; set; }

        public listeMessages()
        {
            InitializeComponent();
        }


        private void LoadMessagesControl()
        {
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetMessagesCalled += new AccountFacebookLight.OnGetMessagesCompleted(Messages_GetMessagesCalled);

            /*if (this.box.Count > this.box.comment_list.comment.Count())
            {
                HyperlinkButton linkBtn = new HyperlinkButton();
                linkBtn.Content = "Afficher les " + this.Commentaires.count + " commentaires";
                linkBtn.Click += new RoutedEventHandler(linkBtn_Click);
                displayAllComsPanel.Children.Add(linkBtn);
            }*/

            LoadMessages();
        }


        private void LoadMessages()
        {
            Connexion.dispatcher.BeginInvoke(() =>
            {
                if (this.box.Count > 0)
                {
                    messagesPanel.Children.Clear();
                    foreach (thread thread in this.box)
                    {
                        Message monMessage = new Message(thread.subject);
                        messagesPanel.Children.Add(monMessage);
                    }

                }
            });
        }

        void Messages_GetMessagesCalled(List<thread> liste)
        {
            this.box = liste;
            //MessageBox toto = new MessageBox("", "invoked");
            //toto.Show();

            /*if (postId == this.postId)
            {
                this.Commentaires = new stream_comments() { comment_list = new stream_commentsComment_list() { comment = coms } };
                LoadComs();
                Connexion.dispatcher.BeginInvoke(() =>
                {
                    displayAllComsPanel.Visibility = System.Windows.Visibility.Collapsed;
                });
            }*/
        }
    }
}