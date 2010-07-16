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
using Facebook.Rest;
using Facebook.Schema;
using EIP.ServiceEIP;

namespace EIP.Objects
{
    public class Profil
    {
        public profile profilFB { get; set; }
        public TwitterStatus profilTW { get; set; }
    }
}
