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

namespace EIP.Views.Child
{
    public partial class Login : ChildWindow
    {
        private bool addAccount;

        public Login(bool addAnAccount)
        {
            this.addAccount = addAnAccount;
            InitializeComponent();
            LoadDropDownType();

            if (!this.addAccount)
            {
                this.Title = "Veuillez choisir un réseau social pour vous connecter";
                linkAddAccount.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Title = "Veuillez choisir le réseau social à ajouter";
                linkAddAccount.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void LoadDropDownType()
        {
            var enumType = typeof(Account.TypeAccount);
            var fields =  from field in enumType.GetFields()
                          where field.IsLiteral
                          select field.GetValue(null).ToString();
            DropDownTypes.ItemsSource = fields;



        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            if (!this.addAccount)
            {
                Connexion.Login((Account.TypeAccount)Enum.Parse(typeof(Account.TypeAccount), DropDownTypes.SelectedValue.ToString(), true), string.Empty, string.Empty);
            }
            else
            {
                Connexion.AddAccount((Account.TypeAccount)Enum.Parse(typeof(Account.TypeAccount), DropDownTypes.SelectedValue.ToString(), true));
            }

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void DropDownTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((Account.TypeAccount)Enum.Parse(typeof(Account.TypeAccount), DropDownTypes.SelectedItem.ToString(),true) )
            {
                case Account.TypeAccount.Facebook:
                    pseudoText.Visibility = System.Windows.Visibility.Collapsed;
                    pseudoBox.Visibility = System.Windows.Visibility.Collapsed;
                    mdpText.Visibility = System.Windows.Visibility.Collapsed;
                    mdpBox.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case Account.TypeAccount.Twitter:

                    if (!this.addAccount)
                    {
                        pseudoText.Visibility = System.Windows.Visibility.Visible;
                        pseudoBox.Visibility = System.Windows.Visibility.Visible;
                        mdpText.Visibility = System.Windows.Visibility.Visible;
                        mdpBox.Visibility = System.Windows.Visibility.Visible;
                    }
                    break;
                case Account.TypeAccount.Myspace:
                    break;
                default:
                    break;
            }

        }

        private void linkAddAccount_Click(object sender, RoutedEventArgs e)
        {
            Login loginBox = new Login(true);
            loginBox.Show();

            
            this.DialogResult = false;
        }
    }
}

