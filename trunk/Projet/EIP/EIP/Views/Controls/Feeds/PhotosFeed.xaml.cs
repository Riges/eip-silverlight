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
    public partial class PhotosFeed : UserControl
    {
        public PhotosFeed()
        {
            InitializeComponent();
        }

        public PhotosFeed(TopicFB topic, long accountID)
        {
            InitializeComponent();

            /*
            src
                type
                alt
             * */
            if (topic.post.attachment.media.stream_media.Count >= 1)
            {
                string urlImg = string.Empty;
                if (topic.post.attachment.media.stream_media[0].src.ToLower().EndsWith(".gif"))
                {
                    urlImg = "http://localhost:4164/GifHandler.ashx?link=" + topic.post.attachment.media.stream_media[0].src.Replace("&", "||");
                }
                else
                    urlImg = topic.post.attachment.media.stream_media[0].src;

                img1.Source = new BitmapImage(new Uri(urlImg, UriKind.Absolute));
                
                img1Border.Visibility = System.Windows.Visibility.Visible;

                img1Link.NavigateUri = new Uri("/Album/" + topic.post.attachment.media.stream_media[0].photo.aid + "/uid/" + topic.post.attachment.media.stream_media[0].photo.owner + "/Account/" + accountID, UriKind.Relative);


                /*MessageBox msgBox = new MessageBox("", topic.post.attachment.media.stream_media[0].photo.aid);
                msgBox.Show();*/

            }
            if (topic.post.attachment.media.stream_media.Count >= 2)
            {
                string urlImg = string.Empty;
                if (topic.post.attachment.media.stream_media[1].src.ToLower().EndsWith(".gif"))
                {

                    urlImg = "http://localhost:4164/GifHandler.ashx?link=" + topic.post.attachment.media.stream_media[1].src.Replace("&", "||");
                }
                else
                    urlImg = topic.post.attachment.media.stream_media[1].src;

                img2.Source = new BitmapImage(new Uri(urlImg, UriKind.Absolute));
                img2Border.Visibility = System.Windows.Visibility.Visible;                
            }
            if (topic.post.attachment.media.stream_media.Count >= 3)
            {
                string urlImg = string.Empty;
                if (topic.post.attachment.media.stream_media[2].src.ToLower().EndsWith(".gif"))
                {

                    urlImg = "http://localhost:4164/GifHandler.ashx?link=" + topic.post.attachment.media.stream_media[2].src.Replace("&", "||");
                }
                else
                    urlImg = topic.post.attachment.media.stream_media[2].src;

                img3.Source = new BitmapImage(new Uri(urlImg, UriKind.Absolute));
                img3Border.Visibility = System.Windows.Visibility.Visible;
            }

            if (topic.post.attachment.name != "" && topic.post.attachment.name != null)
            {
                //"http://www.facebook.com/album.php?aid=10885&id=100001736018246"
                string href = topic.post.attachment.href;
                if (href != null && href != "")
                {
                    if (href.Contains("aid="))
                    {
                        int start_aid = href.IndexOf("aid=");
                        int end_aid = href.IndexOf("&", start_aid);

                        string aid = href.Substring(start_aid + 4, end_aid - (start_aid + 4));

                        int start_id = href.IndexOf("&id=");
                        int end_id = href.IndexOf("&", start_id + 4);

                        string pid = string.Empty;
                        if(end_id == -1)
                            pid = href.Substring(start_id + 4);
                        else
                            pid = href.Substring(start_id + 4, end_id - (start_id + 4));
                    }

                }

                albumName.Content = topic.post.attachment.name;
                albumName.Visibility = System.Windows.Visibility.Visible;
                if (topic.post.message != "" && topic.post.message != null)
                    albumName.Content += " :";
            }
            if (topic.post.message != "" && topic.post.message != null)
            {
                albumDescription.Text = topic.post.message;
                albumDescription.Visibility = System.Windows.Visibility.Visible;
            }
        }



    }
}
