﻿#pragma checksum "C:\eip-silverlight\Projet\EIP\EIP\Views\Controls\Feed.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5DA7E04A5D0D41CB6EFF43FFD31799DE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.21006.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace EIP.Views.Controls {
    
    
    public partial class Feed : System.Windows.Controls.UserControl {
        
        internal System.Windows.Media.Animation.Storyboard OnImg;
        
        internal System.Windows.Media.Animation.Storyboard QuitImg;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image picUser;
        
        internal System.Windows.Controls.TextBlock nameUser;
        
        internal System.Windows.Controls.TextBlock message;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/EIP;component/Views/Controls/Feed.xaml", System.UriKind.Relative));
            this.OnImg = ((System.Windows.Media.Animation.Storyboard)(this.FindName("OnImg")));
            this.QuitImg = ((System.Windows.Media.Animation.Storyboard)(this.FindName("QuitImg")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.picUser = ((System.Windows.Controls.Image)(this.FindName("picUser")));
            this.nameUser = ((System.Windows.Controls.TextBlock)(this.FindName("nameUser")));
            this.message = ((System.Windows.Controls.TextBlock)(this.FindName("message")));
        }
    }
}

