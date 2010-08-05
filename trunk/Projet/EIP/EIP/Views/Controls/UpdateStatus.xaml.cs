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
using EIP.ServiceEIP;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using Hammock;
using Newtonsoft.Json;
using Hammock.Web;

namespace EIP.Views.Controls
{
    public partial class UpdateStatus : UserControl
    {
        public UpdateStatus()
        {
            InitializeComponent(); 
        }

        private void LoadAccountButtons()
        {
            SolidColorBrush brush  = new SolidColorBrush();
            brush.Color = Colors.White;//gray
           if (Connexion.accounts != null)
            {
                if (Connexion.accounts.Count > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        NetworkStackPanel.Children.Clear();
                        foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
                        {
                            if (oneAccount.Value.selected)
                            {
                                
                                StackPanel panel = new StackPanel();
                                panel.Orientation = Orientation.Horizontal;
                                

                                Image img = new Image();
                                img.Width = 12;
                                switch (oneAccount.Value.account.typeAccount)
                                {
                                    case Account.TypeAccount.Facebook:
                                        img.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.png", UriKind.Relative));
                                        break;
                                    case Account.TypeAccount.Twitter:
                                        img.Source = new BitmapImage(new Uri("../../Assets/Images/twitter-icon.png", UriKind.Relative));
                                        break;
                                    case Account.TypeAccount.Myspace:
                                        break;
                                    default:
                                        break;
                                }
                                panel.Children.Add(img);

                                TextBlock text = new TextBlock();
                                text.Text = oneAccount.Value.account.name + " ";
                                text.FontStyle = FontStyles.Italic;
                                text.FontSize = 10;
                                text.Foreground = brush;
                                text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                panel.Children.Add(text);

                                NetworkStackPanel.Children.Add(panel);
                            }
                        }

                    });
                }
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    NetworkStackPanel.Children.Clear();
                });
            }
        }

        public RoutedEventHandler box_Checked { get; set; }

        public RoutedEventHandler box_Unchecked { get; set; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LoadAccountButtons();
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            LoadAccountButtons();
        }


        private bool cheackIfTwitterActiveAccount()
        {
            foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
            {
                if (oneAccount.Value.selected && oneAccount.Value.account.typeAccount == Account.TypeAccount.Twitter)
                {
                    return true;
                }
            }
            return false;
        }

        public string checkShorter(string url)
        {
            if (url.Length <= 30)
                return url;
            else
                return "******************************";
        }

        private void sendStatu_Click(object sender, RoutedEventArgs e)
        {
            string tweet = statuValue.Text;
            Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection mactches = regx.Matches(tweet);
            foreach (Match match in mactches)
            {
                tweet = tweet.Replace(match.Value, "******************************");
            }

            if (Connexion.accounts != null)
            {
                if (Connexion.accounts.Count > 0)
                {
                    if ((cheackIfTwitterActiveAccount() && tweet.Length <= 140) || !cheackIfTwitterActiveAccount())
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
                            {
                                if (oneAccount.Value.selected)
                                {

                                    switch (oneAccount.Value.account.typeAccount)
                                    {
                                        case Account.TypeAccount.Facebook:
                                            ((AccountFacebookLight)oneAccount.Value).SendStatus(statuValue.Text);
                                            break;
                                        case Account.TypeAccount.Twitter:
                                            ((AccountTwitterLight)oneAccount.Value).SendStatus(statuValue.Text);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            statuValue.Text = "";
                        });
                    }
                    else
                    {
                        MessageBox error = new MessageBox("Message trop long", "Vous utilisez Twitter qui limite la taille du message à 140 charactère et le message que vous voulez envoyez fait plus de 140 charactère.");
                        error.Show();
                    }
                }
            }
        }
    }
}
