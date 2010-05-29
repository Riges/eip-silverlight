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

namespace EIP.Views.Controls.Feeds
{
    public partial class VideoFeed : UserControl
    {
        public VideoFeed()
        {
            InitializeComponent();

            mediaElement.Source = new Uri("http://v12.lscache3.c.youtube.com/videoplayback?ip=0.0.0.0&sparams=id%2Cexpire%2Cip%2Cipbits%2Citag%2Calgorithm%2Cburst%2Cfactor%2Coc%3AU0dWSlFMUF9FSkNNNl9JRlRJ&fexp=906208&algorithm=throttle-factor&itag=35&ipbits=0&burst=40&sver=3&expire=1275112800&key=yt1&signature=6B4CAC1AFF4EB7A1E1274762F24884DC4A133D4F.4F514E37AC475CE16D354D7704BFD8E34209771D&factor=1.25&id=4b2a00e0b5d0728e", UriKind.Absolute);
            //mediaElement.Source = new Uri("http://video.ak.facebook.com/cfs-ak-snc4/33057/396/1444411319862_28669.mp4", UriKind.Absolute);
        }
    }
}
