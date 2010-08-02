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
        public Message(String mysubject, String mysummary, String myauthorName)
        {
            InitializeComponent();
            TextBlock authorName = new TextBlock();
            authorName.Text = myauthorName;
            messagesPanel.Children.Add(authorName);
            TextBlock subject = new TextBlock();
            subject.Text = mysubject;
            messagesPanel.Children.Add(subject);
            if (mysummary != "")
            {
                TextBlock summary = new TextBlock();
                summary.Text = mysummary;
                messagesPanel.Children.Add(summary);
            }

        }
    }
}
