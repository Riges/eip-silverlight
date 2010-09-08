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

namespace EIP.Objects
{
    public class VideoLight
    {
        public long vid { get; set; }
        public int uid { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string thumbnail_link { get; set; }
        public string updated_time { get; set; }
        public string created_time { get; set; }
        public string src { get; set; }
        public string src_hq { get; set; }
    }
}
