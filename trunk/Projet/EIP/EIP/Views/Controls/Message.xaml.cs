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
using EIP.Objects;
using System.Windows.Media.Imaging;

namespace EIP.Views.Controls
{
    public partial class Message : UserControl
    {
        ThreadMessage thread;

        public Message()
        {
            InitializeComponent();
        }
        /*public Message(String mysubject)
        {
            InitializeComponent();
            //subject.Text = mysubject;
            //summary.Text = "";
        }*/
        public Message(ThreadMessage th)
        {
            InitializeComponent();

            thread = th;
            if (th.getPic() != null) // verif si gif
            {
                Uri uriImg = new Uri(th.getPic());
                picUser.Source = new BitmapImage(uriImg);
            }
            //subject.Text = th.getSubject();
            subjectText.Text = th.getSubject();
            subject.NavigateUri = new Uri("/Messages/", UriKind.Relative); //TODO
            //if (th.hasDetails() && th.getThread().thread_id == 1438170797750)
                //((AccountFacebookLight)Connexion.accounts[th.accountID]).LoadThreadMessages(th.getThread());
            subject.Click += subject_Click;
            if (th.getSummary() != "")
                summary.Text = th.getSummary().Replace("\n", " ");
            else
                summary.Visibility = System.Windows.Visibility.Collapsed;
            personText.Text = th.getAuthorName();
            person.NavigateUri = new Uri("/ProfilInfos/" + th.getAuthorAccountID() + "/Account/" + th.accountID, UriKind.Relative); // TODO : url profil
            date.Text = th.date.ToString();
        }

        private void subject_Click(object sender, RoutedEventArgs e)
        {
            ((AccountFacebookLight)Connexion.accounts[thread.accountID]).LoadThreadMessages(thread.getThread());
        }

    }
}
