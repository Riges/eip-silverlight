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
using Facebook.Schema;

namespace EIP.Views.Controls
{
    public partial class Coms : UserControl
    {

        private stream_comments commentaires;

        public List<profile> profiles { get; set; }

        public Coms()
        {
            InitializeComponent();
        }

        public stream_comments Commentaires
        {
            get
            {
                return commentaires;
            }
            set
            {
                this.commentaires = value;
                LoadComs();
            }
        }


       

        private void LoadComs()
        {

            if (this.Commentaires.count > 0)
            {
                foreach (comment com in Commentaires.comment_list.comment)
                { 
                    var theProfile = from profile prof in profiles
                                     where prof.id == Convert.ToInt64(com.fromid)
                                     select prof;

                    Com comControl = new Com(com, (profile)theProfile.First());


                    comsPanel.Children.Add(comControl);
                }
            }
        }
    }
}
