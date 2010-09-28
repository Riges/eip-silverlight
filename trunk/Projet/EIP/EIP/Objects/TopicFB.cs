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
using Facebook.Schema;
using Facebook.Rest;
using System.Collections.Generic;
using Facebook.Utility;

namespace EIP.Objects
{
    public class TopicFB
    {
        public stream_post post { get; set; }
        public profile userSource { get; set; }
        public profile userTarget { get; set; }

        public TopicFB()
        {

        }

        public TopicFB(stream_post aPost, profile aUserSource, profile aUserTarget)
        {
            this.post = aPost;
            this.userSource = aUserSource;
            this.userTarget = aUserTarget;
        }

        

    }
}
