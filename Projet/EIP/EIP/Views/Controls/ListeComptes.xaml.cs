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
    public partial class ListeComptes : UserControl
    {

        public Frame contentFrame { get; set; }

        public ListeComptes()
        {
            InitializeComponent();
            //app = (App)System.Windows.Application.Current;
            LoadAccountButtons();


            Button test = new Button();
            test.Content = "MSN";
            test.Name = "MSN";
            LayoutPanel.Children.Add(test);

            
            
        }

        public void Reload()
        {
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
                        LayoutPanel.Children.Clear();
                    
                   
                    foreach (AccountLight oneAccount in Connexion.accounts)
                    {
                        //Dispatcher.BeginInvoke(() =>
                            //{
                        StackPanel panel = new StackPanel();
                        panel.Orientation = Orientation.Horizontal;

                        CheckBox box = new CheckBox();
                        box.Name = oneAccount.account.accountID.ToString();
                        panel.Children.Add(box);

                        /*
                        Button btnAccount = new Button();
                        btnAccount.Content = oneAccount.account.name;
                        btnAccount.Click += btnAccount_Click;
                        btnAccount.CommandParameter = oneAccount;
                        if (Connexion.currentAccount.account.typeAccount == oneAccount.account.typeAccount && Connexion.currentAccount.account.userID == oneAccount.account.userID)
                            btnAccount.IsEnabled = false;
                         * */

                        Image img = new Image();
                        img.Width = 16;
                        switch (oneAccount.account.typeAccount)
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
                        text.Text = oneAccount.account.name;
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

       void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            //app.LoadAccount((Account)((Button)sender).CommandParameter);
            Connexion.LoadAccount((AccountLight)((Button)sender).CommandParameter);
        }
    }
}
