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
using System.Threading;
using System.Windows.Threading;
using EIP.Objects;

namespace EIP.Views.Controls
{
    public partial class NotificationPopup : UserControl
    {
        public DispatcherTimer dt = new DispatcherTimer();

        public NotificationPopup()
        {
            InitializeComponent();
        }

        public NotificationPopup(string content)
        {
            InitializeComponent();

            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }
        }

        public NotificationPopup(string content, int time)
        {
            InitializeComponent();

            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }

            dt.Interval = new TimeSpan(0, 0, 0, time, 000);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        public NotificationPopup(string header, string content)
        {
            InitializeComponent();

            Header.Text = header;
            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }
        }

        public NotificationPopup(string header, string content, int time)
        {
            InitializeComponent();

            Header.Text = header;

            List<WrapPanel> panels = Utils.LoadMessageLn(content);

            foreach (WrapPanel wrap in panels)
            {
                panelContent.Children.Add(wrap);
            }

            //Content.Text = content;


            dt.Interval = new TimeSpan(0, 0, 0, time, 000);
            dt.Tick += new EventHandler(dt_Tick);
            dt.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Remove the control from the layout.
            DislayOff();
        }

        void dt_Tick(object sender, EventArgs e)
        {
            dt.Stop();
            DislayOff();
        }

        private void DislayOff()
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
