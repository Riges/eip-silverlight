using System;
using System.Windows;
using System.Windows.Controls;

namespace EIP
{
    public partial class MessageBox : ChildWindow
    {
        public MessageBox(string title, string details)
        {
            InitializeComponent();
            if (title != null)
                this.Title = title;
            MessageTextBox.Text = details;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}