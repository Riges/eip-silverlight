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
using System.Collections.Generic;
using Facebook.Schema;
using System.IO;

namespace EIP.Objects
{
    public static class Utils
    {

        public static DateTime DateFromStamp(long time)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(time).AddHours(2);
            return dateTime;
        }

        public static string Day2Jour(DateTime date)
        {
            string jour = string.Empty;

            if (date.Date == DateTime.Today)
                return "Aujourd'hui";
            if (date.Date == DateTime.Today.AddDays(-1))
                return "Hier";
            if (date.Date == DateTime.Today.AddDays(1))
                return "Demain";
            if (date.Date == DateTime.Today.AddDays(-2))
                return "Avant-hier";

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    jour = "Dimanche";
                    break;
                case DayOfWeek.Monday:
                    jour = "Lundi";
                    break;
                case DayOfWeek.Tuesday:
                    jour = "Mardi";
                    break;
                case DayOfWeek.Wednesday:
                    jour = "Mercredi";
                    break;
                case DayOfWeek.Thursday:
                    jour = "Jeudi";
                    break;
                case DayOfWeek.Friday:
                    jour = "Vendredi";
                    break;
                case DayOfWeek.Saturday:
                    jour = "Samedi";
                    break;
            }

            if (date < DateTime.Today.AddDays(-6))
            {
                jour = "Le " + date.Day + " " + GetMonthFr(date.Month);
            }


            return jour;
        }

        public static string GetMonthFr(int month)
        {
            string mois = string.Empty;
            switch (month)
            {
                case 1:
                    mois = "Janvier";
                    break;
                case 2:
                    mois = "Février";
                    break;
                case 3:
                    mois = "Mars";
                    break;
                case 4:
                    mois = "Avril";
                    break;
                case 5:
                    mois = "Mai";
                    break;
                case 6:
                    mois = "Juin";
                    break;
                case 7:
                    mois = "Juillet";
                    break;
                case 8:
                    mois = "Août";
                    break;
                case 9:
                    mois = "Septembre";
                    break;
                case 10:
                    mois = "Octobre";
                    break;
                case 11:
                    mois = "Novembre";
                    break;
                case 12:
                    mois = "Décembre";
                    break;

            }
            return mois;
        }

        public static List<UIElement> LoadMessage(string msg, ResourceDictionary Resources)
        {
            List<UIElement> list = new List<UIElement>();
            msg = msg.Replace("\n", " ");
            char[] charTab = new char[1];
            charTab[0] = ' ';
            string[] mots = msg.Split(charTab, StringSplitOptions.RemoveEmptyEntries);

            foreach (string mot in mots)
            {
                if (mot.StartsWith("http://") || mot.StartsWith("https://") || mot.StartsWith("www."))
                {
                    string theMot = mot;
                    if (mot.StartsWith("www."))
                        theMot = "http://" + mot;
                    theMot = theMot.Replace(".co..", ".com").Replace(".c..", ".com");
                    HyperlinkButton link = new HyperlinkButton();
                    link.Style = Resources["HyperlinkButtonStyle"] as Style;
                    link.Foreground = new SolidColorBrush(Colors.White);
                    link.NavigateUri = new Uri(theMot, UriKind.Absolute);
                    link.Content = theMot + " ";
                    link.TargetName = "_blank";
                    list.Add(link);
                }
                else if (mot.StartsWith("@"))
                {
                    HyperlinkButton link = new HyperlinkButton();
                    link.Style = Resources["HyperlinkButtonStyle"] as Style;
                    link.Foreground = new SolidColorBrush(Colors.White);
                    if (mot.EndsWith("!"))
                        link.NavigateUri = new Uri("http://twitter.com/" + mot.Substring(1, mot.Length - 2), UriKind.Absolute);
                    else
                        link.NavigateUri = new Uri("http://twitter.com/" + mot.Substring(1), UriKind.Absolute);
                    link.Content = mot + " ";
                    link.TargetName = "_blank";
                    list.Add(link);
                }
                else if (mot.StartsWith("#"))
                {
                    HyperlinkButton link = new HyperlinkButton();
                    link.Style = Resources["HyperlinkButtonStyle"] as Style;
                    link.Foreground = new SolidColorBrush(Colors.White);
                    link.NavigateUri = new Uri("http://twitter.com/#search?q=" + mot, UriKind.Absolute);
                    link.Content = mot + " ";
                    link.TargetName = "_blank";
                    list.Add(link);
                }
                else
                {
                    TextBlock txtBlock = new TextBlock();
                    txtBlock.Text = mot + " ";
                    list.Add(txtBlock);
                }
            }
            return list;

        }

        /// <summary>
        /// Put the first letter of the word upper
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string FirstLetterUp(string text)
        {
            if(text != null)
                if (text.Length > 0)
                {
                    char[] letters = text.ToCharArray();
                    letters[0] = char.ToUpper(letters[0]);

                    return new string(letters);                
                }
            return text;
        }

        public static Enums.FileType GetFileType(FileInfo file)
        {
            switch (file.Extension.ToLower())
            {
                case ".jpg": return Enums.FileType.jpg;
                case ".bmp": return Enums.FileType.bmp;
                case ".gif": return Enums.FileType.gif;
                case ".tiff": return Enums.FileType.tiff;
                case ".png": return Enums.FileType.png;
                default: return Enums.FileType.jp2;
            }

        }

    }


}
