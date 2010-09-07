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

            /*Image avatar = new Image();
            avatar.Width = 50;
            avatar.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            if (th.getPic() != null) // verif si gif
            {
                Uri uriImg = new Uri(th.getPic());
                avatar.Source = new BitmapImage(uriImg);
            }
            messagesPanel.Children.Add(avatar);
            TextBlock authorName = new TextBlock();
            authorName.Text = th.getAuthorName();
            messagesPanel.Children.Add(authorName);
            TextBlock subject = new TextBlock();
            subject.Text = th.getSubject();
            messagesPanel.Children.Add(subject);
            if (th.getSummary() != "")
            {
                TextBlock summary = new TextBlock();
                summary.Text = th.getSummary();
                messagesPanel.Children.Add(summary);
            }*/

            if (th.getPic() != null) // verif si gif
            {
                Uri uriImg = new Uri(th.getPic());
                picUser.Source = new BitmapImage(uriImg);
            }
            //subject.Text = th.getSubject();
            subjectText.Text = th.getSubject();
            subject.NavigateUri = new Uri("/Messages/", UriKind.Relative); //TODO
            if (th.getSummary() != "")
                summary.Text = th.getSummary().Replace("\n", " ");
            else
                summary.Visibility = System.Windows.Visibility.Collapsed;
            personText.Text = th.getAuthorName();
            person.NavigateUri = new Uri("/ProfilInfos/" + th.getAuthorAccountID() + "/Account/" + th.accountID, UriKind.Relative); // TODO : url profil
            date.Text = th.date.ToString();
        }

    }
}
