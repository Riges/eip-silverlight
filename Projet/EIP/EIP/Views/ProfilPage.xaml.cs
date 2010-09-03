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
using System.Windows.Navigation;
using EIP.Objects;
using Facebook.Schema;
using EIP.Views.Controls;

namespace EIP.Views
{
    public partial class ProfilPage : Page
    {

        public long uid { get; set; }
        public long accountID { get; set; }
        public Dictionary<String, Profil> profil;

        public ProfilPage()
        {
            InitializeComponent();
            profil = new Dictionary<String, Profil>();
            //LoadProfil();
        }


        public enum Tab
        {
            Mur,
            Infos,
            Photos,
            Videos
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);

            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);

            Tab tab = Tab.Mur;
            if (this.NavigationContext.QueryString.ContainsKey("tab"))
                Enum.TryParse<Tab>(this.NavigationContext.QueryString["tab"].ToString(), out tab);

           
            LoadProfilPage(tab);

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl != null)
            {
                LoadProfilPage((Tab)Enum.Parse(typeof(Tab), ((TabItem)tabControl.SelectedItem).DataContext.ToString(), true));
            }
        }

        private void LoadProfilPage(Tab tab)
        {
            switch (tab)
            {
                case Tab.Mur:
                    murTab.IsSelected = true;
                    break;
                case Tab.Infos:
                    infosTab.IsSelected = true;
                    UserInfos infosControl = new UserInfos(this.accountID, this.uid);
                    infosTab.Content = infosControl;

                    break;
                case Tab.Photos:
                    string aid = string.Empty;
                    if (this.NavigationContext.QueryString.ContainsKey("aid"))
                    {
                        aid = this.NavigationContext.QueryString["aid"].ToString();
                    }
                    else
                    {
                        AlbumsControl albumsControl = new AlbumsControl(this.accountID, this.uid);
                        photosTab.Content = albumsControl;
                        photosTab.IsSelected = true;
                    }
                    break;
                case Tab.Videos:
                    videosTab.IsSelected = true;
                    break;
                default:
                    break;
            }
        }

    }
}
