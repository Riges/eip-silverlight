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
using System.Windows.Media.Imaging;

namespace EIP.Views.Controls
{
    public partial class Coms : UserControl
    {

        private stream_comments commentaires;

        public long accountID { get; set; }
        public string postId { get; set; }
        public List<profile> profiles { get; set; }
        public stream_likes likes { get; set; }
        public long postUserId { get; set; }

        public Coms()
        {
            InitializeComponent();
            
        }


        public stream_comments Commentaires
        {
            get
            {
                return commentaires;
            }
            set
            {
                this.commentaires = value;
                
                //LoadComsControl();

            }
        }


        public void LoadComsControl()
        {
            //comsPanel.Children.Add(new TextBlock() { Text = "jaime" });
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetComsCalled += new AccountFacebookLight.OnGetComsCompleted(Coms_GetComsCalled);
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetUsersLikesCalled += new AccountFacebookLight.GetUsersLikesCompleted(Coms_GetUsersLikesCalled);

            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetUsersLikes(this.likes, this.postId);

            if (this.Commentaires.count > this.Commentaires.comment_list.comment.Count())
            {
                HyperlinkButton linkBtn = new HyperlinkButton();
                linkBtn.Style = App.Current.Resources["HyperlinkButtonFonceStyle"] as Style;
                linkBtn.Content = "Afficher les " + this.Commentaires.count + " commentaires";
                linkBtn.Click += new RoutedEventHandler(linkBtn_Click);
                displayAllComsPanel.Children.Add(linkBtn);
            }

            imgNewCom.Source = new BitmapImage(new Uri(((AccountFacebookLight)Connexion.accounts[this.accountID]).userInfos.pic_square, UriKind.Absolute));
            //textNewCom.Text = "Rédigez un commentaire...";
            textNewCom_LostFocus(null, null);

            LoadComs();
        }

        

        
        private void LoadComs()
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    comsPanel.Children.Clear();
                    if (this.Commentaires.comment_list.comment != null && this.Commentaires.comment_list.comment.Count > 0)
                    {
                        foreach (comment com in Commentaires.comment_list.comment)
                        {

                            //comsPanel.Children.Add(new TextBlock() { Text = com.text });
                            var theProfile = from profile prof in profiles
                                             where prof.id == Convert.ToInt64(com.fromid)
                                             select prof;

                            Com comControl = new Com(com, (profile)theProfile.First(), this.accountID, postUserId, postId);


                            comsPanel.Children.Add(comControl);
                        }
                    }
                });
        }

        void linkBtn_Click(object sender, RoutedEventArgs e)
        {

            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetComs(this.postId);

        }

        void Coms_GetComsCalled(List<comment> coms, string postId)
        {

            //MessageBox toto = new MessageBox("", "invoked");
            //toto.Show();

            if (postId == this.postId)
            {

                Connexion.dispatcher.BeginInvoke(() =>
                {
                    this.Commentaires = new stream_comments() { comment_list = new stream_commentsComment_list() { comment = coms } };
                    LoadComs();
                    displayAllComsPanel.Visibility = System.Windows.Visibility.Collapsed;
                });
            }
        }

        void Coms_GetUsersLikesCalled(bool ok, string postId)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    if (this.postId == postId)
                    {
                        List<long> uids = new List<long>();
                        uids.AddRange(this.likes.friends.uid);
                        uids.AddRange(this.likes.sample.uid);




                        if (this.likes.user_likes)
                        {
                            TextBlock txtUserLike = new TextBlock() { Text = "Vous " };
                            jaimePanel.Children.Add(txtUserLike);
                        }

                        foreach (long uid in uids)
                        {
                            profile prof = ((AccountFacebookLight)(Connexion.accounts[accountID])).profiles.Where<profile>(delegate(profile profTmp) { if (profTmp != null && profTmp.id == uid)return true; return false; }).First();
                            if (prof != null)
                            {
                                HyperlinkButton link = new HyperlinkButton() { Content = prof.name, Style = App.Current.Resources["HyperlinkButtonFonceStyle"] as Style, NavigateUri = new Uri("/ProfilInfos/" + uid + "/Account/" + this.accountID, UriKind.Relative) };
                                if (jaimePanel.Children.Count > 0)
                                    jaimePanel.Children.Add(new TextBlock() { Text = ", " });
                                jaimePanel.Children.Add(link);
                            }
                        }

                        if (this.likes.user_likes)
                            jaimePanel.Children.Add(new TextBlock() { Text = " aimez ça." });
                        else if (!this.likes.user_likes && uids.Count == 1)
                            jaimePanel.Children.Add(new TextBlock() { Text = " aime ça." });
                        else if (!this.likes.user_likes && uids.Count > 1)
                            jaimePanel.Children.Add(new TextBlock() { Text = " aiment ça." });


                    }
                });
        }

        private void textNewCom_GotFocus(object sender, RoutedEventArgs e)
        {
            imgNewCom.Visibility = System.Windows.Visibility.Visible;
            btnNewCom.Visibility = System.Windows.Visibility.Visible;
            if(textNewCom.Text == "Rédigez un commentaire...")
                textNewCom.Text = "";
        }

        private void textNewCom_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textNewCom.Text == "")
            {
                imgNewCom.Visibility = System.Windows.Visibility.Collapsed;
                btnNewCom.Visibility = System.Windows.Visibility.Collapsed;
                textNewCom.Text = "Rédigez un commentaire...";
            }
        }

        private void btnNewCom_Click(object sender, RoutedEventArgs e)
        {
            string text = textNewCom.Text.Trim();
            if (text != string.Empty)
            {
                ((AccountFacebookLight)(Connexion.accounts[accountID])).AddCom(this.postId, text);
                textNewCom.Text = string.Empty;
            }
        }
    }
}
