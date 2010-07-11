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

namespace EIP.Views.Controls
{
    public partial class Coms : UserControl
    {

        private stream_comments commentaires;

        public long accountID { get; set; }
        public string postId { get; set; }
        public List<profile> profiles { get; set; }
        public stream_likes likes { get; set; }

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
                
                LoadComsControl();

            }
        }


        private void LoadComsControl()
        {
            //comsPanel.Children.Add(new TextBlock() { Text = "jaime" });
            ((AccountFacebookLight)Connexion.accounts[this.accountID]).GetComsCalled += new AccountFacebookLight.OnGetComsCompleted(Coms_GetComsCalled); 
            
            if (this.Commentaires.count > this.Commentaires.comment_list.comment.Count())
            {
                HyperlinkButton linkBtn = new HyperlinkButton();
                linkBtn.Content = "Afficher les " + this.Commentaires.count + " commentaires";
                linkBtn.Click += new RoutedEventHandler(linkBtn_Click);
                displayAllComsPanel.Children.Add(linkBtn);
            }

            LoadComs();
        }

        
        private void LoadComs()
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    if (this.Commentaires.comment_list.comment.Count > 0)
                    {
                        comsPanel.Children.Clear();
                        foreach (comment com in Commentaires.comment_list.comment)
                        {

                            //comsPanel.Children.Add(new TextBlock() { Text = com.text });
                            var theProfile = from profile prof in profiles
                                             where prof.id == Convert.ToInt64(com.fromid)
                                             select prof;

                            Com comControl = new Com(com, (profile)theProfile.First(), this.accountID);


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
                this.Commentaires = new stream_comments() { comment_list = new stream_commentsComment_list() { comment = coms } };
                LoadComs();
                Connexion.dispatcher.BeginInvoke(() =>
                    {
                        displayAllComsPanel.Visibility = System.Windows.Visibility.Collapsed;
                    });
            }
        }
    }
}
