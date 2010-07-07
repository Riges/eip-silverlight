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

namespace EIP.Views.Controls
{
    public partial class jaime : UserControl
    {
        public long accountID { get; set; }
        public bool userLikes { get; set; }
        public string postId { get; set; }

        public jaime()
        {
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(jaime_Loaded);

           
        }
        /*
        void jaime_Loaded(object sender, RoutedEventArgs e)
        {

            
            MessageBox msg = new MessageBox("", "jaime_Loaded");
            msg.Show();
        }
         * */


        void Feed_RemoveLikeCalled(bool ok, string postId)
        {
            if (ok && this.postId == postId)
                LoadJaimeButton(false, false);
        }

        void Feed_AddLikeCalled(bool ok, string postId)
        {
            if (ok && this.postId == postId)
                LoadJaimeButton(true, false);
        }

        private void jaimebutton_Click(object sender, RoutedEventArgs e)
        {
            bool jaime = (bool)((HyperlinkButton)sender).DataContext;
            if (jaime)
                ((AccountFacebookLight)Connexion.accounts[this.accountID]).RemoveLike(this.postId);
            else
                ((AccountFacebookLight)Connexion.accounts[this.accountID]).AddLike(this.postId);
        }

        public void LoadJaimeButton(bool jaime)
        {
            LoadJaimeButton(jaime, true);
        }

        private void LoadJaimeButton(bool jaime, bool abo)
        {
            if (abo)
            {
                ((AccountFacebookLight)Connexion.accounts[this.accountID]).AddLikeCalled += new AccountFacebookLight.OnAddLikeCompleted(Feed_AddLikeCalled);
                ((AccountFacebookLight)Connexion.accounts[this.accountID]).RemoveLikeCalled += new AccountFacebookLight.OnRemoveLikeCompleted(Feed_RemoveLikeCalled);
            }

            Connexion.dispatcher.BeginInvoke(() =>
            {
                if (jaime)
                    jaimebutton.Content = "Je n'aime plus";
                else
                    jaimebutton.Content = "J'aime";
                jaimebutton.DataContext = jaime;
            });
        }
    }
}
