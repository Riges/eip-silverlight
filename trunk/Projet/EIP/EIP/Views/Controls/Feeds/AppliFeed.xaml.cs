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
using EIP.Objects;
using System.Windows.Media.Imaging;

namespace EIP.Views.Controls.Feeds
{
    public partial class AppliFeed : UserControl
    {
        public AppliFeed()
        {
            InitializeComponent();
        }


        public AppliFeed(TopicFB topic)
        {
            InitializeComponent();


            if (topic.post.attachment.media.stream_media.Count >= 1)
            {
                if (topic.post.attachment.media.stream_media[0].src != "" && topic.post.attachment.media.stream_media[0].src != null)
                {
                    img.Source = new BitmapImage(new Uri(topic.post.attachment.media.stream_media[0].src, UriKind.Absolute));
                    imgBorder.Visibility = System.Windows.Visibility.Visible;
                    linkImg.Visibility = System.Windows.Visibility.Visible;
                }

                if (topic.post.attachment.media.stream_media[0].href != "" && topic.post.attachment.media.stream_media[0].href != null)
                    linkImg.NavigateUri = new Uri(topic.post.attachment.media.stream_media[0].href, UriKind.Absolute);



                switch (topic.post.attachment.media.stream_media[0].type)
                {
                    case "link":
                        break;
                    case "video":
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Grid.SetColumn(contentPanel, 0);
                Grid.SetColumnSpan(contentPanel, 2);
            }

            titre.Content = topic.post.attachment.name;
            titre.NavigateUri = new Uri(topic.post.attachment.href, UriKind.Absolute);

            if (topic.post.attachment.caption != "" && topic.post.attachment.caption != null)
            {
                caption.Text = topic.post.attachment.caption;
                caption.Visibility = System.Windows.Visibility.Visible;
            }

            if (topic.post.attachment.description != "" && topic.post.attachment.description != null)
            {
                foreach (UIElement element in Utils.LoadMessage(topic.post.attachment.description))
                    description.Children.Add(element);
                description.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
