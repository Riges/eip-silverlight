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
using Facebook;

namespace EIP.Objects
{
    public class UserFB
    {

        public long accountID { get; set; } // Pour savoir à quel compte appartient cet user

        public string userID { get; set; }
        public string name { get; set; }

        private string userImage;
        public string description { get; set; }
        public string location { get; set; }
        public string link { get; set; }

        public string UserImage
        {
            get
            {
                if (!string.IsNullOrEmpty(userImage))
                    return userImage;
                else
                    return "http://graph.facebook.com/" + this.userID + "/picture";//?type=normal";
            }
            set
            {
                userImage = value;
            }

        }

        public void PopulateFB(JsonObject element)
        {
            this.userID = element["id"].ToString();

            if (element.ContainsKey("link"))
                this.link = element["link"].ToString();

            if (element.ContainsKey("name"))
                this.name = element["name"].ToString();
 
            if (element.ContainsKey("bio"))
                this.description = element["bio"].ToString();
        }

    }
}
