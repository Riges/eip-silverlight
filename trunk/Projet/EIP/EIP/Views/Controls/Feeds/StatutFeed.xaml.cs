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

namespace EIP.Views.Controls.Feeds
{
    public partial class StatutFeed : UserControl
    {
        public StatutFeed()
        {
            InitializeComponent();
        }

        public StatutFeed(string message)
        {
            InitializeComponent();
            LoadMessage(message);
        }

        private void LoadMessage(string msg)
        {
            msg = msg.Replace("\n", " ");
            char[] charTab = new char[1];
            charTab[0] = ' ';
            string[] mots = msg.Split(charTab, StringSplitOptions.RemoveEmptyEntries);

            foreach (string mot in mots)
            {
                if (mot.StartsWith("http://") || mot.StartsWith("https://") || mot.StartsWith("www."))
                {
                    string theMot = mot;
                    if (mot.StartsWith("www."))
                        theMot = "http://" + mot;
                    HyperlinkButton link = new HyperlinkButton();
                    link.NavigateUri = new Uri(theMot, UriKind.Absolute);
                    link.Content = theMot + " ";
                    link.TargetName = "_blank";
                    messagePanel.Children.Add(link);
                }
                else if (mot.StartsWith("@"))
                {
                    HyperlinkButton link = new HyperlinkButton();
                    if (mot.EndsWith("!"))
                        link.NavigateUri = new Uri("http://twitter.com/" + mot.Substring(1, mot.Length - 2), UriKind.Absolute);
                    else
                        link.NavigateUri = new Uri("http://twitter.com/" + mot.Substring(1), UriKind.Absolute);
                    link.Content = mot + " ";
                    link.TargetName = "_blank";
                    messagePanel.Children.Add(link);
                }
                else if (mot.StartsWith("#"))
                {
                    HyperlinkButton link = new HyperlinkButton();
                    link.NavigateUri = new Uri("http://twitter.com/#search?q=" + mot, UriKind.Absolute);
                    link.Content = mot + " ";
                    link.TargetName = "_blank";
                    messagePanel.Children.Add(link);
                }
                else
                {
                    TextBlock txtBlock = new TextBlock();
                    
                    txtBlock.Text = mot + " ";
                    messagePanel.Children.Add(txtBlock);
                }
            }
        }
    }
}
