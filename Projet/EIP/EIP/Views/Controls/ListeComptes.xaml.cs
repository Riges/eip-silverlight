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
        private ListeCptMode mode;

        public enum ListeCptMode
        {
            Normal,
            ReadOnly,
            Unvisible
        }

        public ListeCptMode ListeCompteMode
        {
            get { return mode; }
            set
            {
                mode = value;
                
            }
        }

        private void SetMode()
        {
            if (this.mode == ListeCptMode.Unvisible)
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Visible;

                foreach (UIElement element in LayoutPanel.Children)
                {
                    if (element.GetType() == typeof(CheckBox))
                    {
                        switch (this.mode)
                        {
                            case ListeCptMode.Normal:
                                ((CheckBox)element).IsEnabled = true;
                                break;
                            case ListeCptMode.ReadOnly:
                                ((CheckBox)element).IsEnabled = false;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

  

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
                            UnCompte compte = new UnCompte(oneAccount.Value);
                            compte.Margin = new Thickness(0, 0, 0, 5);

                            LayoutPanel.Children.Add(compte);
                           /*
                            StackPanel panel = new StackPanel();
                            panel.Orientation = Orientation.Horizontal;
                            
                            Image imgReseau = new Image();
                            imgReseau.Width = 20;
                            imgReseau.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            imgReseau.Margin = new Thickness(0, 0, 5, 0);
                            switch (oneAccount.Value.account.typeAccount)
                            {
                                case Account.TypeAccount.Facebook:
                                    imgReseau.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.png", UriKind.Relative));
                                    break;
                                case Account.TypeAccount.Twitter:
                                    imgReseau.Source = new BitmapImage(new Uri("../../Assets/Images/twitter-icon.png", UriKind.Relative));
                                    //((AccountTwitterLight)oneAccount.Value).LoadFriends();
                                    break;
                                case Account.TypeAccount.Myspace:
                                    break;
                                default:
                                    break;
                            }
                            panel.Children.Add(imgReseau);

                            CheckBox box = new CheckBox();
                            box.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            
                            box.Name = oneAccount.Value.account.accountID.ToString();
                            box.Checked += new RoutedEventHandler(box_Checked);
                            box.Unchecked += new RoutedEventHandler(box_Unchecked);
                            box.CommandParameter = oneAccount.Value;
                            box.DataContext = oneAccount.Value;
                            if (oneAccount.Value.selected)
                                box.IsChecked = true;
                            panel.Children.Add(box);

                            Image imgAccount = new Image();
                            imgAccount.Width = 20;
                            imgAccount.Name = "img"+oneAccount.Value.account.userID;
                            imgAccount.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                            switch (oneAccount.Value.account.typeAccount)
                            {
                                case Account.TypeAccount.Facebook:
                                    if(((AccountFacebookLight)(oneAccount.Value)).userInfos != null)
                                        imgAccount.Source = new BitmapImage(new Uri(((AccountFacebookLight)(oneAccount.Value)).userInfos.pic_square, UriKind.Absolute));
                                    else
                                        ((AccountFacebookLight)oneAccount.Value).GetFirstUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(ListeComptes_GetFirstUserInfoCalled);
                                    break;
                                case Account.TypeAccount.Twitter:
                                    if (((AccountTwitterLight)(oneAccount.Value)).userInfos != null)
                                        imgAccount.Source = new BitmapImage(new Uri(((AccountTwitterLight)(oneAccount.Value)).userInfos.ProfileImageUrl, UriKind.Absolute));
                                    //((AccountTwitterLight)oneAccount.Value).LoadFriends();
                                    break;
                                default:
                                    break;
                            }
                            panel.Children.Add(imgAccount);

                            TextBlock text = new TextBlock();
                            text.Text = oneAccount.Value.account.name;
                            text.Padding = new Thickness(5, 0, 5, 0);
                            text.FontSize = 18;

                            panel.Children.Add(text);

                            Image imgDel = new Image();
                            imgDel.Source = new BitmapImage(new Uri("../../Assets/Images/bullet_delete.png", UriKind.Relative));
                            imgDel.Width = 20;
                            imgDel.Name = "imgDel" + oneAccount.Value.account.accountID;

                            imgDel.Visibility = System.Windows.Visibility.Collapsed;

                            imgDel.MouseLeftButtonUp += new MouseButtonEventHandler(imgDel_Click);
                            imgDel.DataContext = oneAccount.Value.account.accountID;
                            panel.Children.Add(imgDel);
                            
                            panel.MouseMove += new MouseEventHandler(img_MouseMove);
                            panel.MouseLeave += new MouseEventHandler(img_MouseLeave);
                            panel.DataContext = oneAccount.Value.account.accountID;

                            panel.Margin = new Thickness(0, 0, 0, 5);
                            

                            LayoutPanel.Children.Add(panel);
                            * 
                            * */                              
                        }

                    });
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        LayoutPanel.Children.Clear();
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
        /*
        void ListeComptes_GetFirstUserInfoCalled(Facebook.Schema.user monUser)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    ((Image)LayoutPanel.FindName("img" + monUser.uid)).Source = new BitmapImage(new Uri(monUser.pic_square, UriKind.Absolute));
                });
        }

        void imgDel_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox msgBox = new MessageBox("Demande de confirmation", "Etes vous sur de vouloir supprimer définitivement ce compte ?", MessageBoxButton.OKCancel);
            msgBox.Closed += new EventHandler(msgBox_Closed);
            msgBox.DataContext = (long)((Image)sender).DataContext;
            msgBox.Show();
            
            
        }

        void msgBox_Closed(object sender, EventArgs e)
        {
            MessageBox msgBox = (MessageBox)sender;

            bool result = (bool)msgBox.DialogResult;
            if (result)
            {
                Connexion.serviceEIP.DeleteAccountAsync((long)msgBox.DataContext);
            }
        }

        void img_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            string obj = "imgDel" + (long)((StackPanel)sender).DataContext;
            Image imgDel = (Image)FindName(obj);
            imgDel.Visibility = System.Windows.Visibility.Collapsed;
        }

        void img_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            string obj = "imgDel" + (long)((StackPanel)sender).DataContext;
            Image imgDel = (Image)FindName(obj);
            imgDel.Visibility = System.Windows.Visibility.Visible;
        }

        void box_Unchecked(object sender, RoutedEventArgs e)
        {
            Connexion.accounts[((AccountLight)((CheckBox)sender).CommandParameter).account.accountID].selected = false;
            //Connexion.SaveAccount(((AccountLight)((CheckBox)sender).CommandParameter));
            if(this.mode == ListeCptMode.Normal)
                ReloadPage();
        }

        void box_Checked(object sender, RoutedEventArgs e)
        {
            Connexion.accounts[((AccountLight)((CheckBox)sender).CommandParameter).account.accountID].selected = true;
            //Connexion.SaveAccount(((AccountLight)((CheckBox)sender).CommandParameter));
            if (this.mode == ListeCptMode.Normal)
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
         */

        /*
       void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            //app.LoadAccount((Account)((Button)sender).CommandParameter);
            Connexion.LoadAccount((AccountLight)((Button)sender).CommandParameter);
        }*/
    }
}
