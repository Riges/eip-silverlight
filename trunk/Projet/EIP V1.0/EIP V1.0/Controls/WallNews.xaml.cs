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
using Facebook;
using Facebook.Schema;

namespace EIP.Controls
{
    public partial class WallNews : UserControl
    {

        public stream_data stream { get; set; }

        public WallNews()
        {
            InitializeComponent();
        }
    }
}
