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
using System.Windows.Controls.Primitives;

namespace EIP.Views.Controls
{
    public partial class UpdateStatus : UserControl
    {
        Popup p = new Popup();

        public UpdateStatus()
        {
            InitializeComponent();
            LayoutRoot.Children.Add(p);
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

                        //if (NetworkStackPanel.Children == null || NetworkStackPanel.Children.Count == 0)
                        //{
                        //    NetworkStackPanel.Children.Clear();
                        //    TextBlock text = new TextBlock();
                        //    text.Text = "Accounts selected";
                        //    text.FontStyle = FontStyles.Italic;
                        //    text.FontSize = 10;
                        //    text.Foreground = brush;
                        //    text.VerticalAlignment = System.Windows.VerticalAlignment.Center; ;
                        //    NetworkStackPanel.Children.Add(text);
                        //}

                    });
                }
                //else
                //{
                //    NetworkStackPanel.Children.Clear();
                //    TextBlock text = new TextBlock();
                //    text.Text = "Accounts selected";
                //    text.FontStyle = FontStyles.Italic;
                //    text.FontSize = 10;
                //    text.Foreground = brush;
                //    text.VerticalAlignment = System.Windows.VerticalAlignment.Center; ;
                //    NetworkStackPanel.Children.Add(text);
                //}
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    NetworkStackPanel.Children.Clear();

                    //TextBlock text = new TextBlock();
                    //text.Text = "Accounts selected";
                    //text.FontStyle = FontStyles.Italic;
                    //text.FontSize = 10;
                    //text.Foreground = brush;
                    //text.VerticalAlignment = System.Windows.VerticalAlignment.Center;;
                    //NetworkStackPanel.Children.Add(text);
                });
            }
        }

        void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            // Close the popup.
            p.IsOpen = false;

        }


        public RoutedEventHandler box_Checked { get; set; }

        public RoutedEventHandler box_Unchecked { get; set; }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LoadAccountButtons();
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            DisplayPopup();
            LoadAccountButtons();

        }


        private void DisplayPopup()
        {
            // Create some content to show in the popup. Typically you would 
            // create a user control.
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(3.0);
            
            //border.CornerRadius = new CornerRadius(5);

            StackPanel panel1 = new StackPanel();
            
            
            panel1.Background = new SolidColorBrush(Colors.LightGray);

            Button button1 = new Button();
            button1.Content = "Close";
            button1.Margin = new Thickness(5.0);
            button1.Click += new RoutedEventHandler(buttonClose_Click);
            TextBlock textblock1 = new TextBlock();
            textblock1.Text = "The popup controlsssssssssssssss";
            textblock1.Margin = new Thickness(5.0);
            panel1.Children.Add(textblock1);
            panel1.Children.Add(button1);
            border.Child = panel1;

            // Set the Child property of Popup to the border 
            // which contains a stackpanel, textblock and button.
            p.Child = border;

            // Set where the popup will show up on the screen.
            p.VerticalOffset = 25;
            p.HorizontalOffset = 0;
          

            p.IsOpen = true;


           
            
            // Open the popup.
           
        }


        private bool checkIfTwitterActiveAccount()
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
                    if ((checkIfTwitterActiveAccount() && tweet.Length <= 140) || !checkIfTwitterActiveAccount())
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
