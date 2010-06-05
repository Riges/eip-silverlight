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

namespace EIP.Views.Controls
{
    public partial class UpdateStatus : UserControl
    {
        public UpdateStatus()
        {
            InitializeComponent();


            
        }

        private void totoBtn_Click(object sender, RoutedEventArgs e)
        {
            KeyValuePair<long, AccountLight> toto = Connexion.accounts.First();

            if (toto.Value.account.typeAccount == ServiceEIP.Account.TypeAccount.Facebook)
            {
                AccountFacebookLight totoLight = (AccountFacebookLight)toto.Value;


                checktoto.DataContext = totoLight;
            }
            LoadAccountButtons();
        }

        private void LoadAccountButtons()
        {
            if (Connexion.accounts != null)
            {
                if (Connexion.accounts.Count > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        TwitterStackPanel.Children.Clear();
                        FaceBookStackPanel.Children.Clear();
                        foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
                        {
                            //Dispatcher.BeginInvoke(() =>
                            //{
                            StackPanel panel = new StackPanel();
                            panel.Orientation = Orientation.Horizontal;

                            CheckBox box = new CheckBox();

                            //box.DataContext = oneAccount.Value.account;
                            /*
                            Binding binding = new Binding();
                            binding.Source = oneAccount.Value.account;
                            binding.Path = new PropertyPath("name");
                            box.SetBinding(CheckBox.ContentProperty, binding);
                            */

                            box.Name = oneAccount.Value.account.accountID.ToString();
                            box.Checked += new RoutedEventHandler(box_Checked);
                            box.Unchecked += new RoutedEventHandler(box_Unchecked);
                            box.CommandParameter = oneAccount.Value;
                            if (oneAccount.Value.selected)
                                box.IsChecked = true;
                            panel.Children.Add(box);

                            Image img = new Image();
                            img.Width = 16;
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
                            text.Text = oneAccount.Value.account.name;
                            panel.Children.Add(text);

                            switch (oneAccount.Value.account.typeAccount)
                            {
                                case Account.TypeAccount.Facebook:
                                    FaceBookStackPanel.Children.Add(panel);
                                    break;
                                case Account.TypeAccount.Twitter:
                                    TwitterStackPanel.Children.Add(panel);
                                    break;
                                case Account.TypeAccount.Myspace:
                                    break;
                                default:
                                    break;
                            }
                        }

                    });
                }
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    TwitterStackPanel.Children.Clear();
                    FaceBookStackPanel.Children.Clear();
                });
            }
        }

        public RoutedEventHandler box_Checked { get; set; }

        public RoutedEventHandler box_Unchecked { get; set; }
    }
}
