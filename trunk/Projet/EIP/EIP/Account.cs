using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace EIP
{
    public class Account
    {
        public long accountID { get; set; }
        public TypeAccount typeAccount { get; set; }
        public long userID { get; set; }
        public string name { get; set; }
        
        

        public enum TypeAccount
        {
            Facebook,
            Twitter,
            Myspace
        }


        public Account()
        {
            
        }

    
    }
}
