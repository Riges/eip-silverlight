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

namespace EIP.Views.Controls
{
    public partial class AlbumsControl : UserControl
    {
        public AlbumsControl()
        {
            InitializeComponent();
        }

        public AlbumsControl(long accountID, long uid)
        {
            InitializeComponent();
            LoadAlbums(accountID, uid);

        }

        public void LoadAlbums(long accountID, long uid)
        {
            if (Connexion.accounts != null && Connexion.accounts.Count > 0)
            {
                foreach (KeyValuePair<long, AccountLight> acc in Connexion.accounts)
                {
                    if (acc.Key == accountID)
                        acc.Value.selected = true;
                    else
                        acc.Value.selected = false;
                }
                Connexion.listeComptes.ListeCompteMode = ListeComptes.ListeCptMode.ReadOnly;
                Connexion.listeComptes.Reload();
                


                AccountFacebookLight account = (AccountFacebookLight)Connexion.accounts[accountID];
                if (account.selected)
                account.GetAlbumsCalled += new AccountFacebookLight.OnGetAlbumsCompleted(AlbumsView_GetAlbumsCalled);
                account.GetAlbums(uid);
            }

        }

          private void AlbumsView_GetAlbumsCalled(List<album> albums)
        {
            Connexion.dispatcher.BeginInvoke(() =>
                {
                    flowControl.DataContext = albums;
                });
        }


    }
}
