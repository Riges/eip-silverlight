﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Facebook.Schema;
using FlickrNet;

namespace EIP.Objects
{
    public class AlbumToImageConverter : IValueConverter
    {

        #region IValueConverter Membres

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage bt = new BitmapImage();


            if (value.GetType() == typeof(album))
            {
                album album = value as album;
                if (album.aid != null && album.aid != "")
                {
                    foreach (System.Collections.Generic.KeyValuePair<long, AccountLight> account in Connexion.accounts)
                    {
                        if (account.Value.account.typeAccount == ServiceEIP.Account.TypeAccount.Facebook)
                        {
                            AccountFacebookLight accFb = ((AccountFacebookLight)account.Value);
                            if (accFb.photos.ContainsKey(album.aid))
                            {
                                if (accFb.photos[album.aid].ContainsKey(album.cover_pid))
                                {
                                    bt = new BitmapImage(new Uri(accFb.photos[album.aid][album.cover_pid].src_big, UriKind.Absolute));
                                }
                            }
                        }

                    }

                }
            }
            else if (value.GetType() == typeof(Photoset))
            {
                Photoset album = value as Photoset;
                bt = new BitmapImage(new Uri(album.PhotosetSquareThumbnailUrl, UriKind.Absolute));
            }
            else if (value.GetType() == typeof(string))
            {
                string image = value as string;
                switch (image)
                {
                    case "Facebook":
                        return "../../Assets/Images/facebook-icon.png";
                    case "Twitter":
                        return "../../Assets/Images/twitter-icon.png";
                    case "Flickr":
                        return "../../Assets/Images/flickr-icon.png";
                    default:
                        return "";
                }
                
            }


            return bt;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
