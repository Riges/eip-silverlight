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
    public static class Utils
    {



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

    }


}
