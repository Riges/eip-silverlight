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
using Facebook.Schema;
using EIP.Objects;
using EIP.ServiceEIP;
using System.Windows.Media.Imaging;
using FlickrNet;

namespace EIP.Views.Controls
{
    public partial class FriendView : UserControl
    {
        private long accountID;
        private user friendFB;
        private TwitterUser friendTW;
        private Contact friendFK;

        public bool FriendsPage
        {
            get
            {
                if (imgUser.MaxWidth == 250)
                    return true;

                return false;
            }
            set
            {
                if (value)
                {
                    //imgUser.MaxWidth = 250;
                    //imgUser.MaxHeight = 150;
                    this.Width = 150;
                    this.MaxHeight = 150;
                }
            }
        }



        public FriendView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(FriendView_Loaded);
        }

        public FriendView(Friend friend)
        {
            InitializeComponent();
            LoadFriend(friend);  
        }

   
        void FriendView_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Width = imgUser.MaxWidth;

            Friend friend = (Friend)this.DataContext;
            LoadFriend(friend);
        }

        private void LoadFriend(Friend friend)
        {
            if (friend != null)
            {
                this.accountID = friend.accountID;
                if (friend.userFB != null)
                {
                    this.friendFB = friend.userFB;
                    LoadFriendFB();
                }

                if (friend.userTW != null)
                {
                    this.friendTW = friend.userTW;
                    LoadFriendTW();
                }

                if (friend.userFK != null)
                {
                    this.friendFK = friend.userFK;
                    LoadFriendFK();
                }
            }
        }

        private void LoadFriendFB()
        {
            if (this.friendFB.pic_big != null)
            {
                //imgUser.UriSource = new Uri(this.friendFB.pic_big);
                imgUser.Source = new BitmapImage(new Uri(this.friendFB.pic_big));
            }

            nomFriend.Content = this.friendFB.name;
            nomFriend.NavigateUri = new Uri("/ProfilInfos/" + this.friendFB.uid + "/Account/" + accountID, UriKind.Relative);
        }

        private void LoadFriendTW()
        {
            if (this.friendTW.ProfileImageUrl != null)
            {
                //imgUser.UriSource = new Uri(this.friendTW.ProfileImageUrl);
                imgUser.Source = new BitmapImage(new Uri(this.friendTW.ProfileImageUrl));
            }

            nomFriend.Content = this.friendTW.ScreenName;
            nomFriend.NavigateUri = new Uri("/ProfilInfos/" + this.friendTW.Id + "/Account/" + accountID, UriKind.Relative);


        }

        private void LoadFriendFK()
        {
            if (this.friendFK.BuddyIconUrl != null)
            {
                imgUser.Source = new BitmapImage(new Uri(this.friendFK.BuddyIconUrl));
            }

            nomFriend.Content = this.friendFK.UserName;
            nomFriend.NavigateUri = new Uri("/ProfilInfos/" + this.friendFK.UserId + "/Account/" + accountID, UriKind.Relative);
        }

    }
}
