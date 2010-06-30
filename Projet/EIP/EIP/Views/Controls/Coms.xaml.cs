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


        public List<profile> profiles { get; set; }
        public stream_comments coms 
        {
            get
            {
                return this.coms;
            }
            set
            {
                LoadComs();
            }
        }


        public Coms()
        {
            InitializeComponent();
        }

        private void LoadComs()
        {
            
            if (coms.count > 0)
            {
                foreach (comment com in coms.comment_list.comment)
                {
                    var theProfile = from profile prof in profiles
                                     where prof.id == Convert.ToInt64(com.post_id)
                                     select prof;
                   


                    Com comControl = new Com(com, (profile)theProfile);


                    comsPanel.Children.Add(comControl);
                }
            }
        }
    }
}
