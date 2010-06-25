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
    public partial class FriendList : Page
    {
        public Dictionary<String, Friend> friends;

        public FriendList()
        {
            InitializeComponent();
            friends = new Dictionary<String, Friend>();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {



        }

        /// <summary>
        /// methode qui merge les listes de friends
        /// </summary>
        private void LoadList()
        {
            foreach(KeyValuePair<long, AccountLight> account in Connexion.accounts)
            {
                switch (account.Value.account.typeAccount)
                {
                    case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                        List<user> friendsFB = ((AccountFacebookLight)account.Value).friends;
                        foreach (user toto in friendsFB)
                        {
                            if (friends.Keys.Contains(toto.proxied_email))
                            {
                                if (friends[toto.proxied_email].userFB == null)
                                    friends[toto.proxied_email].userFB = toto;
                            }
                            else
                            {
                                Friend titi = new Friend();
                                titi.userFB = toto;
                                friends.Add(toto.proxied_email, titi);
                            }
                        }
                        break;
                    case EIP.ServiceEIP.Account.TypeAccount.Twitter:

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
