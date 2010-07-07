using System;
using System.Windows;
using System.Windows.Controls;

namespace EIP
{
    public partial class MessageBox : ChildWindow
    {

        
        public MessageBox(string title, string details, MessageBoxButton boutons)
        {
            InitializeComponent();
            if (title != null)
                this.Title = title;
            if (details != null)
                MessageTextBox.Text = details;

            switch (boutons)
            {
                case MessageBoxButton.OK:
                    OKButton.Visibility = System.Windows.Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    OKButton.Visibility = System.Windows.Visibility.Visible;
                    CancelButton.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        public MessageBox(string title, string details)
        {
            InitializeComponent();
            if (title != null)
                this.Title = title;
            if (details != null)
                MessageTextBox.Text = details;

            OKButton.Visibility = System.Windows.Visibility.Visible;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}