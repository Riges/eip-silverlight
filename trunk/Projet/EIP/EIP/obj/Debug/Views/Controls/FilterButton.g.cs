﻿#pragma checksum "K:\-- Etna\ETNA 2\eip-silverlight\trunk\Projet\EIP\EIP\Views\Controls\FilterButton.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "93C0A5BDEEE759F7CE067C815530B919"
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
    
    
    public partial class FilterButton : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel LayoutPanel;
        
        internal System.Windows.Controls.TextBlock nameFilter;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/EIP;component/Views/Controls/FilterButton.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.LayoutPanel = ((System.Windows.Controls.StackPanel)(this.FindName("LayoutPanel")));
            this.nameFilter = ((System.Windows.Controls.TextBlock)(this.FindName("nameFilter")));
        }
    }
}

