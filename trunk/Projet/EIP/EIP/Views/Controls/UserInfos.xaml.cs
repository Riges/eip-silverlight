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
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class UserInfos : UserControl
    {
        public UserInfos()
        {
            InitializeComponent();
        }


        public UserInfos(long accountID, long uid)
        {
            InitializeComponent();
            LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
            LoadInfos(accountID, uid);
        }

        private void LoadInfos(long accountID, long uid)
        {
            
            switch (Connexion.accounts[accountID].account.typeAccount)
            {
                case EIP.ServiceEIP.Account.TypeAccount.Facebook:
                    AccountFacebookLight acc = (AccountFacebookLight)Connexion.accounts[accountID];
                    acc.GetUserInfoCalled += new AccountFacebookLight.OnGetUserInfoCompleted(acc_GetUserInfoCalled);
                    acc.GetUserInfo(uid, AccountFacebookLight.GetUserInfoFrom.Profil);

                    break;
                case EIP.ServiceEIP.Account.TypeAccount.Twitter:
                    break;
                default:
                    break;
            }
        }

        void acc_GetUserInfoCalled(user monUser)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    if (monUser != null)
                    {
                        if (monUser.sex == null)
                        {
                            this.sexLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.sex.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.sex.Text = Utils.FirstLetterUp(monUser.sex);
                            this.sexLabel.Visibility = System.Windows.Visibility.Visible;
                            this.sex.Visibility = System.Windows.Visibility.Visible;
                        }


                        if (monUser.birthday == null)
                        {
                            this.annivLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.anniv.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            this.anniv.Text = monUser.birthday;
                            this.annivLabel.Visibility = System.Windows.Visibility.Visible;
                            this.anniv.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.meeting_sex.sex == null || monUser.meeting_sex.sex.Count == 0)
                        {
                            this.interesseLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.interesse.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            foreach (string sex in monUser.meeting_sex.sex)
                            {
                                TextBlock txtBlock = new TextBlock();
                                txtBlock.Text = Utils.FirstLetterUp(sex);
                                this.interesse.Children.Add(txtBlock);
                            }
                            this.interesseLabel.Visibility = System.Windows.Visibility.Visible;
                            this.interesse.Visibility = System.Windows.Visibility.Visible;
                        }

                        if (monUser.meeting_for.seeking == null || monUser.meeting_for.seeking.Count == 0)
                        {
                            this.rechercheLabel.Visibility = System.Windows.Visibility.Collapsed;
                            this.recherche.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            foreach (string seek in monUser.meeting_for.seeking)
                            {
                                TextBlock txtBlock = new TextBlock();
                                txtBlock.Text = Utils.FirstLetterUp(seek);
                                this.recherche.Children.Add(txtBlock);
                            }
                            this.rechercheLabel.Visibility = System.Windows.Visibility.Visible;
                            this.recherche.Visibility = System.Windows.Visibility.Visible;
                        }


                        LayoutRoot.Visibility = System.Windows.Visibility.Visible;


                    }


                });
        }
    }
}
