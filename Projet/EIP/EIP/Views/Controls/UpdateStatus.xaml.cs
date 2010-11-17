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
//using Hammock;
using Newtonsoft.Json;
//using Hammock.Web;
using System.Windows.Controls.Primitives;
using System.IO;
using EIP.Objects;
using Facebook.Schema;

namespace EIP.Views.Controls
{
    public partial class UpdateStatus : UserControl
    {
        Popup p = new Popup();
        FileInfo file = null;
        string caption = string.Empty;

        public UpdateStatus()
        {
            InitializeComponent();
            LayoutRoot.Children.Add(p);
        }
        
        private void LoadAccountButtons()
        {
            //SolidColorBrush brush  = new SolidColorBrush();
            //brush.Color = Colors.White;//gray
            if (Connexion.accounts != null)
            {
                if (Connexion.accounts.Count > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        NetworkStackPanel.Children.Clear();
                        foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
                        {
                            if (oneAccount.Value.selected && oneAccount.Value.account.typeAccount != Account.TypeAccount.Flickr)
                            {
                                StackPanel panel = new StackPanel();
                                panel.Orientation = Orientation.Horizontal;

                                Image img = new Image();
                                img.Width = 14;
                                switch (oneAccount.Value.account.typeAccount)
                                {
                                    case Account.TypeAccount.Facebook:
                                        img.Source = new BitmapImage(new Uri("../../Assets/Images/facebook-icon.png", UriKind.Relative));
                                        break;
                                    case Account.TypeAccount.Twitter:
                                        img.Source = new BitmapImage(new Uri("../../Assets/Images/twitter-icon.png", UriKind.Relative));
                                        break;
                                    case Account.TypeAccount.Flickr:
                                        break;
                                    default:
                                        break;
                                }
                                panel.Children.Add(img);

                                TextBlock text = new TextBlock();
                                text.Text = oneAccount.Value.account.name;
                                //text.FontStyle = FontStyles.Italic;
                                text.FontSize = 12;
                                text.FontWeight = FontWeights.Bold;
                                text.Foreground = App.Current.Resources["LinkColorFonceBrush"] as SolidColorBrush;
                                text.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                                text.Margin = new Thickness(5, 0, 0, 0);
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

        private void sendStatut_Click(object sender, RoutedEventArgs e)
        {

            string tweet = statutBox.Text;
            Regex regx = new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase);
            MatchCollection mactches = regx.Matches(tweet);
            foreach (Match match in mactches)
            {
                tweet = tweet.Replace(match.Value, "******************************");
            }

            Dispatcher.BeginInvoke(() =>
            {

                if (Connexion.accounts != null)
                {
                    if (Connexion.accounts.Count > 0)
                    {
                        bool textEmpty = false;
                        if (shareLinkTextBox.Visibility == System.Windows.Visibility.Collapsed && this.file == null)
                        {
                            if (statutBox.Text.Trim().Length == 0)
                                textEmpty = true;
                        }

                        if (textEmpty)
                        {
                            MessageBox msgNox = new MessageBox("Action impossible", "Vous ne pouvez pas partager un statut vide !", MessageBoxButton.OK);
                            msgNox.Show();
                        }
                        else
                        {

                            if ((checkIfTwitterActiveAccount() && tweet.Trim().Length <= 140) || !checkIfTwitterActiveAccount())
                            {
                                bool selected = false;
                                foreach (KeyValuePair<long, AccountLight> oneAccount in Connexion.accounts)
                                {
                                    if (oneAccount.Value.selected)
                                    {
                                        selected = true;
                                        switch (oneAccount.Value.account.typeAccount)
                                        {
                                            case Account.TypeAccount.Facebook:
                                                if (shareLinkTextBox.Visibility == System.Windows.Visibility.Visible)
                                                {
                                                    if (linkText.Text.Trim() != "" && linkText.Text.Trim() != "http://")
                                                        ((AccountFacebookLight)oneAccount.Value).SendStreamLink(statutBox.Text, linkText.Text.Trim());
                                                    else
                                                        ((AccountFacebookLight)oneAccount.Value).SendStatus(statutBox.Text);
                                                }
                                                else if (this.file != null)
                                                {
                                                    this.caption = statutBox.Text.Trim();
                                                    ((AccountFacebookLight)oneAccount.Value).CreateMyNetWorkAlbumCalled += UpdateStatus_CreateMyNetWorkAlbumCalled;
                                                    ((AccountFacebookLight)oneAccount.Value).CreateMyNetWorkAlbum();
                                                }
                                                else
                                                {
                                                    ((AccountFacebookLight)oneAccount.Value).SendStatus(statutBox.Text.Trim());
                                                }
                                                break;
                                            case Account.TypeAccount.Twitter:
                                                string status = statutBox.Text.Trim();

                                                if (shareLinkTextBox.Visibility == System.Windows.Visibility.Visible)
                                                {
                                                    if (linkText.Text.Trim() != "" && linkText.Text.Trim() != "http://")
                                                        status += " " + linkText.Text.Trim();
                                                }
                                                if (this.file != null)
                                                {
                                                    using (System.IO.Stream str = file.OpenRead())
                                                    {
                                                        Byte[] bytes = new Byte[str.Length];
                                                        str.Read(bytes, 0, bytes.Length);
                                                        
                                                        str.Close();
                                                        string fileType = this.file.Extension;
                                                        fileType = "image/" + fileType.Substring(fileType.IndexOf(".") + 1, fileType.Length - 1);
                                                        ((AccountTwitterLight)oneAccount.Value).SendStatus(status, bytes, fileType, this.file.Name);
                                                    }
                                                }
                                                else
                                                {
                                                    ((AccountTwitterLight)oneAccount.Value).SendStatus(status);
                                                }

                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                if (selected)
                                {
                                    statutBox.Text = "";
                                    linkText.Text = "";
                                    CleanDisplay();
                                }
                                else
                                {
                                    MessageBox msgNox = new MessageBox("Action impossible", "Vous devez sélectionner au moins un compte pour mettre à jour votre statut !", MessageBoxButton.OK);
                                    msgNox.Show();
                                }

                            }
                            else
                            {
                                MessageBox error = new MessageBox("Message trop long", "Vous utilisez Twitter qui limite la taille du message à 140 charactère et le message que vous voulez envoyez fait plus de 140 charactère.");
                                error.Show();
                            }
                        }
                    }

                }
            });
        }

        void UpdateStatus_CreateMyNetWorkAlbumCalled(AccountFacebookLight oneAccount, album album)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (album != null && this.file != null)
                    {
                        oneAccount.UploadPhotoCalled += new AccountFacebookLight.UploadPhotoCompleted(UpdateStatus_UploadPhotoCalled);
                        using (System.IO.Stream str = file.OpenRead())
                        {
                            Byte[] bytes = new Byte[str.Length];
                            str.Read(bytes, 0, bytes.Length);

                            str.Close();
                            oneAccount.UploadPhoto(album.aid, this.caption, bytes, Utils.GetFileType(this.file));
                        }

                        Connexion.navigationService.Navigate(new Uri("/Home", UriKind.Relative));
                    }
                });
        }

        void UpdateStatus_UploadPhotoCalled(photo photo)
        {
            this.file = null;
            this.caption = string.Empty;
            //Connexion.navigationService.Navigate(new Uri("/Home", UriKind.Relative));
        }



        private void myPopup_Drop(object sender, DragEventArgs e)
        {
            if (e.Data == null) return;
            var files = e.Data.GetData(DataFormats.FileDrop) as FileInfo[];
            if (files == null) return;

            foreach (var fileInfo in files)
            {

                Enums.FileType fileType = Utils.GetFileType(fileInfo);
                if (fileType != Enums.FileType.gif && fileType != Enums.FileType.jp2)
                {
                    this.file = fileInfo;
                    using (var fileStream = fileInfo.OpenRead())
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(fileStream);
                        imgPhoto.Source = bitmapImage;
                        shareLink.Visibility = System.Windows.Visibility.Collapsed;
                        dropPhotoText.Visibility = System.Windows.Visibility.Collapsed;
                        removePhotoLink.Visibility = System.Windows.Visibility.Visible;

                        fileStream.Close();
                    }
                    break;
                }
            }

           
        }

        void serviceEIP_UploadPhotoCompleted(object sender, UploadPhotoCompletedEventArgs e)
        {
            string message = e.Result;
            /*
            using (System.IO.Stream str = fileInfo.OpenRead())
            {
                Byte[] bytes = new Byte[str.Length];
                str.Read(bytes, 0, bytes.Length);

                Connexion.serviceEIP.UploadPhotoAsync(fileInfo.Name, bytes);
                Connexion.serviceEIP.UploadPhotoCompleted += new EventHandler<UploadPhotoCompletedEventArgs>(serviceEIP_UploadPhotoCompleted);
            }  
             * */
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenPopup();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (linkText.Text.Trim() == "http://")
            {
                linkText.Text = "";
            }
        }

        private void linkText_LostFocus(object sender, RoutedEventArgs e)
        {
            if (linkText.Text.Trim() == "")
            {
                linkText.Text = "http://";
            }
        }

        private void borderPopup_LostFocus(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        private void borderPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            OpenPopup();
        }

        private void borderPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            ClosePopup();
        }

        private void OpenPopup()
        {
            exprimeToiTxt.Visibility = System.Windows.Visibility.Collapsed;
            statutBox.Visibility = System.Windows.Visibility.Visible;

            LoadAccountButtons();
            borderPopup.MaxHeight = 500;
            borderPopup.MaxWidth = 485;
            borderPopup.BorderBrush = new SolidColorBrush(){  Color = Colors.Black};
        } 

        private void ClosePopup()
        {
            exprimeToiTxt.Visibility = System.Windows.Visibility.Visible;
            statutBox.Visibility = System.Windows.Visibility.Collapsed;

            borderPopup.MaxHeight = 41;
            borderPopup.MaxWidth = 385;
            borderPopup.BorderBrush = new SolidColorBrush();
        }

        private void removePhotoLink_Click(object sender, RoutedEventArgs e)
        {
            /*
            shareLink.Visibility = System.Windows.Visibility.Visible;
            dropPhotoText.Visibility = System.Windows.Visibility.Visible;
            removePhotoLink.Visibility = System.Windows.Visibility.Collapsed;
            imgPhoto.Source = null;
            this.file = null;*/

            CleanDisplay();
            this.file = null;
            this.caption = string.Empty;
        }

        private void shareLink_Click(object sender, RoutedEventArgs e)
        {
            shareLink.Visibility = System.Windows.Visibility.Collapsed;
            dropPhotoText.Visibility = System.Windows.Visibility.Collapsed;

            removeLink.Visibility = System.Windows.Visibility.Visible;
            shareLinkTextBox.Visibility = System.Windows.Visibility.Visible;
        }

        private void removeLink_Click(object sender, RoutedEventArgs e)
        {
            /*
            shareLink.Visibility = System.Windows.Visibility.Visible;
            dropPhotoText.Visibility = System.Windows.Visibility.Visible;
            removeLink.Visibility = System.Windows.Visibility.Collapsed;

            shareLinkTextBox.Visibility = System.Windows.Visibility.Collapsed;
            */
            CleanDisplay();
        }

        private void CleanDisplay()
        {
            shareLink.Visibility = System.Windows.Visibility.Visible;
            dropPhotoText.Visibility = System.Windows.Visibility.Visible;

            removeLink.Visibility = System.Windows.Visibility.Collapsed;
            shareLinkTextBox.Visibility = System.Windows.Visibility.Collapsed;
            removePhotoLink.Visibility = System.Windows.Visibility.Collapsed;
            imgPhoto.Source = null;
           
        }

        private void statutBox_GotFocus(object sender, RoutedEventArgs e)
        {

            if (statutBox.Text.Trim() == "Exprime toi !")
            {
                statutBox.Text = "";
            }

        }

        private void statutBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (statutBox.Text.Trim() == "")
            {
                statutBox.Text = "Exprime toi !";
            }
        }


    }
}
