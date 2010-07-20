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
            brush.Color = Colors.Gray;;
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
                                //Dispatcher.BeginInvoke(() =>
                                //{
                                StackPanel panel = new StackPanel();
                                panel.Orientation = Orientation.Horizontal;
                                

                                //CheckBox box = new CheckBox();

                                //box.DataContext = oneAccount.Value.account;
                                /*
                                Binding binding = new Binding();
                                binding.Source = oneAccount.Value.account;
                                binding.Path = new PropertyPath("name");
                                box.SetBinding(CheckBox.ContentProperty, binding);
                                */

                                /*box.Name = oneAccount.Value.account.accountID.ToString();
                                box.CommandParameter = oneAccount.Value;
                                if (oneAccount.Value.selected)
                                    box.IsChecked = true;
                                panel.Children.Add(box);*/

                                Image img = new Image();
                                img.Width = 12;
                                switch (oneAccount.Value.account.typeAccount)
                                {
                                    case Account.TypeAccount.Facebook:
                                        img.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.jpg", UriKind.Relative));
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

        public string getShorter(string url)
        {
            string returnUrl = url;
            var client = new RestClient
            {
                Authority = "http://tinyurl.com",
                UserAgent = "Hammock",
                SilverlightAuthorizationHeader = "X-Twitter-Auth",
                SilverlightMethodHeader = "X-Twitter-Method",
                SilverlightUserAgentHeader = "X-Twitter-Agent",
                SilverlightAcceptEncodingHeader = "X-Twitter-Accept"
            };

            var request = new RestRequest
            {
                Path = "Home/SleepReport",
                Method = WebMethod.Get
            };
            request.AddParameter("url", url);

            // The shortened URL is contained in response.Content
            //RestResponse response = client.Request(request);
            IAsyncResult result = client.BeginRequest(request);
            returnUrl = result.ToString();
            return returnUrl + " TOTO ";
        }

        private void sendStatu_Click(object sender, RoutedEventArgs e)
        {
            Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            if (Connexion.accounts != null)
            {
                if (Connexion.accounts.Count > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
                        {
                            if(oneAccount.Value.selected)
                            {
                               //statuValue.Text += " " + oneAccount.Value.account.name;
                            
                                if ((cheackIfTwitterActiveAccount() && statuValue.Text.Count() < 140) || !cheackIfTwitterActiveAccount())
                                {
                                    MatchCollection mactches = regx.Matches(statuValue.Text); 
    
                                    foreach (Match match in mactches) {
                                        statuValue.Text = statuValue.Text.Replace(match.Value, getShorter(match.Value));
                                    }
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
                        }
                        statuValue.Text = "";
                    });
                }
            }
        }
    }
}
