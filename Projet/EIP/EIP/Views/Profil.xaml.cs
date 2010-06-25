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
    public partial class Profil : Page
    {
        public Dictionary<String, Profil> profil;

        public Profil()
        {
            InitializeComponent();
            profil = new Dictionary<String, Profil>();
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

        }

    }
}
