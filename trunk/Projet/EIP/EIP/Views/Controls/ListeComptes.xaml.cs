﻿using System;
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

namespace EIP.Views.Controls
{
    public partial class ListeComptes : UserControl
    {

        public Frame contentFrame { get; set; }

        public ListeComptes()
        {
            InitializeComponent();
            //app = (App)System.Windows.Application.Current;
            LoadAccountButtons();
        }

        public void Reload()
        {
            LoadAccountButtons();
        }

        private void LoadAccountButtons()
        {
            if (Connexion.currentAccounts != null)
            {
                if (Connexion.currentAccounts.Count > 0)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        LayoutPanel.Children.Clear();
                    
                   
                    foreach (Account oneAccount in Connexion.currentAccounts)
                    {
                        //Dispatcher.BeginInvoke(() =>
                            //{
                        Button btnAccount = new Button();
                        btnAccount.Content = oneAccount.name;
                        btnAccount.Click += btnAccount_Click;
                        btnAccount.CommandParameter = oneAccount;
                        if (Connexion.currentAccount.typeAccount == oneAccount.typeAccount && Connexion.currentAccount.userID == oneAccount.userID)
                            btnAccount.IsEnabled = false;
                        LayoutPanel.Children.Add(btnAccount);
                            //});
                    }

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

       void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            //app.LoadAccount((Account)((Button)sender).CommandParameter);
            Connexion.LoadAccount((Account)((Button)sender).CommandParameter);
        }
    }
}
