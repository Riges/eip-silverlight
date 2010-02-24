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

namespace EIP.Views.Child
{
    public partial class Login : ChildWindow
    {
        public Login()
        {
            InitializeComponent();
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
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

