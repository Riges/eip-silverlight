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
using System.Windows.Browser;
using System.Windows.Navigation;
using System.Windows.Data;


namespace EIP.Views.Controls
{
    public partial class ListeComptes : UserControl
    {

        public Frame contentFrame { get; set; }

        public ListeComptes()
        {
            InitializeComponent();

            LoadAccountButtons();             
        }

        public void Reload()
        {
            LoadAccountButtons();
            Connexion.Loading(false);
        }

        private void LoadAccountButtons()
        {
            if (Connexion.accounts != null)
            {
                if (Connexion.accounts.Count > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        LayoutPanel.Children.Clear();

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
                                    ((AccountTwitterLight)oneAccount.Value).LoadFriends();
                                    break;
                                case Account.TypeAccount.Myspace:
                                    break;
                                default:
                                    break;
                            }
                            panel.Children.Add(img);

                            TextBlock text = new TextBlock();
                            text.Text = oneAccount.Value.account.name;
                            text.Margin = new Thickness(5, 0, 0, 0);
                            panel.Children.Add(text);
                            

                            LayoutPanel.Children.Add(panel);
                                //});
                        }

                    });
                }
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        LayoutPanel.Children.Clear();
                    });
            }
        }

        void box_Unchecked(object sender, RoutedEventArgs e)
        {
            Connexion.accounts[((AccountLight)((CheckBox)sender).CommandParameter).account.userID].selected = false;
            //Connexion.SaveAccount(((AccountLight)((CheckBox)sender).CommandParameter));
            ReloadPage();
        }

        void box_Checked(object sender, RoutedEventArgs e)
        {
            Connexion.accounts[((AccountLight)((CheckBox)sender).CommandParameter).account.userID].selected = true;
            //Connexion.SaveAccount(((AccountLight)((CheckBox)sender).CommandParameter));
            ReloadPage();
        }

        private void ReloadPage()
        {
            string sourceStr = Application.Current.Host.NavigationState;
            string query = "?time=" + DateTime.Now.Ticks;

            foreach (KeyValuePair<string, string> param in Connexion.navigationContext.QueryString)
            {
                if (param.Key != "time")
                {
                    //query += (query == string.Empty ? "?" : "&");
                    query += string.Format("{0}{1}={2}", "&", param.Key, param.Value);
                }
            }
            
            Uri source = new Uri((sourceStr.Contains('?')?sourceStr.Substring(0, sourceStr.IndexOf('?')):sourceStr) + query, UriKind.Relative);
            if (Connexion.navigationService != null)
                Connexion.navigationService.Navigate(source);
            else
                Connexion.contentFrame.Navigate(source);
        }

        /*
       void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            //app.LoadAccount((Account)((Button)sender).CommandParameter);
            Connexion.LoadAccount((AccountLight)((Button)sender).CommandParameter);
        }*/
    }
}
