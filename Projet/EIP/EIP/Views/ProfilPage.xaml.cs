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

namespace EIP.Views
{
    public partial class ProfilPage : Page
    {
        public Dictionary<String, Profil> profil;

        public ProfilPage()
        {
            InitializeComponent();
            profil = new Dictionary<String, Profil>();
            LoadProfil();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        /// <summary>
        /// methode qui merge les profils de l'user ou du friend
        /// </summary>
        private void LoadProfil()
        {
            foreach (KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        //Profil toto = new Profil();
                        //toto.profilFB = ;
                        //profil.Add(toto, toto.profilFB.id);
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                        //Profil toto = new Profil();
                        //toto.profilTW = ;
                        //profil.Add(toto, );
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Myspace:
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
