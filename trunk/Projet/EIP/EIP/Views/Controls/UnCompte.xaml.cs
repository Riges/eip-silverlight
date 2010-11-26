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
using Facebook.Schema;
using EIP.ServiceEIP;
using System.Windows.Media.Imaging;
using EIP.Views.Controls;
using System.Windows.Browser;

namespace EIP.Views.Controls
{
    public partial class UnCompte : UserControl
    {
        public Frame contentFrame { get; set; }
        private ListeComptes.ListeCptMode mode;
        private AccountLight account;

        public UnCompte(AccountLight oneAccount, ListeComptes.ListeCptMode leMode)
        {
            this.account = oneAccount;
            InitializeComponent();

            this.mode = leMode;
            string status = string.Empty;
            string imgIcone = string.Empty;
            string imgAcc = string.Empty;

            switch (oneAccount.account.typeAccount)
	        {
		        case Account.TypeAccount.Facebook:
                    if (((AccountFacebookLight)oneAccount).userInfos != null)
                    {
                        if (((AccountFacebookLight)oneAccount).userInfos.status != null && ((AccountFacebookLight)oneAccount).userInfos.status.message != null && ((AccountFacebookLight)oneAccount).userInfos.status.message != "")
                        {
                            status = ((AccountFacebookLight)oneAccount).userInfos.status.message;
                        }
                        if (((AccountFacebookLight)(oneAccount)).userInfos.pic_square != null && ((AccountFacebookLight)(oneAccount)).userInfos.pic_square != "")
                            imgAcc = ((AccountFacebookLight)(oneAccount)).userInfos.pic_square;
                    }               
                    else
                        ((AccountFacebookLight)oneAccount).GetFirstUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(ListeComptes_GetFirstUserInfoCalled);
                    imgIcone = "../../Assets/Images/facebook-icon.png";
                 break;
                case Account.TypeAccount.Twitter:
                 if (((AccountTwitterLight)oneAccount).userInfos != null)
                 {
                     if (((AccountTwitterLight)oneAccount).userInfos.Status != null && ((AccountTwitterLight)oneAccount).userInfos.Status.Text != null && ((AccountTwitterLight)oneAccount).userInfos.Status.Text != "")
                     {
                         // status = ((AccountTwitterLight)oneAccount).userInfos.Status.Text;
                         status = HttpUtility.HtmlDecode(((AccountTwitterLight)oneAccount).userInfos.Status.Text);
                     }
                     if (((AccountTwitterLight)(oneAccount)).userInfos.ProfileImageUrl != null && ((AccountTwitterLight)(oneAccount)).userInfos.ProfileImageUrl != "")
                         imgAcc = ((AccountTwitterLight)(oneAccount)).userInfos.ProfileImageUrl;
                 }
                 else
                 {
                     ((AccountTwitterLight)oneAccount).GetUserInfoCalled += new AccountTwitterLight.OnGetUserInfoCompleted(UnCompte_GetUserInfoCalled);
                     ((AccountTwitterLight)oneAccount).GetUserInfo(oneAccount.account.userID);
                 }
                    imgIcone = "../../Assets/Images/twitter-icon.png";
                    break;
                case Account.TypeAccount.Flickr:
                    if (((AccountFlickrLight)oneAccount).userInfos != null)
                    {

                        if (((AccountFlickrLight)(oneAccount)).userInfos.BuddyIconUrl != null)
                            imgAcc = ((AccountFlickrLight)(oneAccount)).userInfos.BuddyIconUrl;
                    }
                    else
                    {
                        ((AccountFlickrLight)oneAccount).GetUserInfoCalled += new AccountFlickrLight.OnGetUserInfoCompleted(UnCompte_GetUserInfoCalled);
                        ((AccountFlickrLight)oneAccount).GetUserInfo(((AccountFlickr)(oneAccount.account)).userIDstr);
                    }
                    imgIcone = "../../Assets/Images/flickr-icon.png";
                    break;
                default:
                    break;
	        }
            LoadAccount(oneAccount, oneAccount.account.typeAccount, oneAccount.account.name, imgIcone, imgAcc, status);
        }


        private void LoadAccount(AccountLight oneAccount, Account.TypeAccount typeAccount, string userName, string imgIcone, string imgAcc, string status)
        {
            if (imgIcone != string.Empty)
                imgReseau.Source = new BitmapImage(new Uri(imgIcone, UriKind.Relative));
            if (imgAcc != string.Empty)
                imgAccount.Source = new BitmapImage(new Uri(imgAcc, UriKind.Absolute));


            Account acc = oneAccount.account;
            string key = acc.groupID + "-" + acc.userID + "-selected";

            box.CommandParameter = oneAccount;
            if (oneAccount.selected || Convert.ToBoolean(Connexion.GetStorageValue(key)) || !Connexion.storage.Contains(key))
                box.IsChecked = true;

            accountName.Content = userName;

            if(typeAccount == Account.TypeAccount.Flickr)
                accountName.NavigateUri = new Uri("/ProfilInfos/" + ((AccountFlickr)oneAccount.account).userIDstr + "/Account/" + oneAccount.account.accountID, UriKind.Relative);
            else
                accountName.NavigateUri = new Uri("/ProfilInfos/" + oneAccount.account.userID + "/Account/" + oneAccount.account.accountID, UriKind.Relative);

            if (oneAccount.account.typeAccount == Account.TypeAccount.Flickr)
                imgAccount.Name = "img" + ((AccountFlickr)oneAccount.account).userIDstr;
            else
                imgAccount.Name = "img" + oneAccount.account.userID;

            imgDel.Source = new BitmapImage(new Uri("../../Assets/Images/croix-rouge.png", UriKind.Relative));
            imgDel.DataContext = oneAccount.account.accountID;
            imgDel.Name = "imgDel" + oneAccount.account.accountID;

            accountStatus.Text = status;
            accountStatusTooltip.Text = status;
        }

        void ListeComptes_GetFirstUserInfoCalled(Facebook.Schema.user monUser)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                ((Image)LayoutRoot.FindName("img" + monUser.uid)).Source = new BitmapImage(new Uri(monUser.pic_square, UriKind.Absolute));
                if (monUser.status != null && monUser.status.message != null && monUser.status.message != "")
                {
                    accountStatus.Text = monUser.status.message;
                    accountStatusTooltip.Text = monUser.status.message;
                }

            });
        }

        void UnCompte_GetUserInfoCalled(TwitterUser user, long accountID, bool isUserAccount)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.account.account.userID == user.Id)
                {
                    ((Image)LayoutRoot.FindName("img" + user.Id)).Source = new BitmapImage(new Uri(user.ProfileImageUrl, UriKind.Absolute));
                    if (user.Status != null && user.Status.Text != null && user.Status.Text != "")
                    {
                        accountStatus.Text = user.Status.Text;
                        accountStatusTooltip.Text = user.Status.Text;
                    }
                }
            });
        }

        void UnCompte_GetUserInfoCalled(FlickrNet.Person user)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (((AccountFlickr)(this.account.account)).userIDstr == user.UserId)
                {
                    if (user.BuddyIconUrl != null && user.BuddyIconUrl != "")
                        ((Image)LayoutRoot.FindName("img" + user.UserId)).Source = new BitmapImage(new Uri(user.BuddyIconUrl, UriKind.Absolute));
                }
           });
        }

        private void imgDel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
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


        void box_Unchecked(object sender, RoutedEventArgs e)
        {
            Account acc = ((AccountLight)box.CommandParameter).account;
            string key = acc.groupID + "-" + acc.userID + "-selected";
            Connexion.SaveStorageValue(key, false);

            Connexion.accounts[((AccountLight)((CheckBox)sender).CommandParameter).account.accountID].selected = false;
            if (this.mode == ListeComptes.ListeCptMode.Normal)
                ReloadPage();
        }

        void box_Checked(object sender, RoutedEventArgs e)
        {
            Account acc = ((AccountLight)box.CommandParameter).account;
            string key = acc.groupID + "-" + acc.userID + "-selected";
            Connexion.SaveStorageValue(key, true);

            Connexion.accounts[((AccountLight)((CheckBox)sender).CommandParameter).account.accountID].selected = true;
            if (this.mode == ListeComptes.ListeCptMode.Normal)
                ReloadPage();
        }

        void img_MouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        //    string obj = "imgDel" + (long)((StackPanel)sender).DataContext;
        //    Image imgDel = (Image)FindName(obj);
        //    imgDel.Visibility = System.Windows.Visibility.Collapsed;
        }

        void img_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            //string obj = "imgDel" + (long)((StackPanel)sender).DataContext;
            //Image imgDel = (Image)FindName(obj);
            //imgDel.Visibility = System.Windows.Visibility.Visible;
        }

        private void ReloadPage()
        {
            string sourceStr = Application.Current.Host.NavigationState;

            if (!sourceStr.StartsWith("/WaitLoad"))
            {
                string query = "?time=" + DateTime.Now.Ticks;

                foreach (KeyValuePair<string, string> param in Connexion.navigationContext.QueryString)
                {
                    if (param.Key != "time")
                    {
                        //query += (query == string.Empty ? "?" : "&");
                        query += string.Format("{0}{1}={2}", "&", param.Key, param.Value);
                    }
                }

                Uri source = new Uri((sourceStr.Contains('?') ? sourceStr.Substring(0, sourceStr.IndexOf('?')) : sourceStr) + query, UriKind.Relative);
                if (Connexion.navigationService != null)
                    Connexion.navigationService.Navigate(source);
                else
                    Connexion.contentFrame.Navigate(source);
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            imgDel.Visibility = System.Windows.Visibility.Visible;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            imgDel.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
