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
    public class CustomNotification : ContentControl
    {
        public CustomNotification()
        {
            this.DefaultStyleKey = typeof(CustomNotification);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(string),
            typeof(CustomNotification),
            new PropertyMetadata(OnHeaderPropertyChanged));

        /// <summary>
        ///   Gets or sets a value that indicates whether 
        ///   the <see cref="P:System.Windows.Controls.Label.Target" /> field is required. 
        /// </summary>
        public string Header
        {
            get
            {
                return (string)GetValue(CustomNotification.HeaderProperty);
            }

            set
            {
                SetValue(CustomNotification.HeaderProperty, value);
            }
        }

        private static void OnHeaderPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CustomNotification),
            new PropertyMetadata(OnTextPropertyChanged));

        /// <summary>
        ///   Gets or sets a value that indicates whether 
        ///   the <see cref="P:System.Windows.Controls.Label.Target" /> field is required. 
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(CustomNotification.TextProperty);
            }

            set
            {
                SetValue(CustomNotification.TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button closeButton = GetTemplateChild("closeButton") as Button;
            if (closeButton != null)
            {
                closeButton.Click += new RoutedEventHandler(closeButton_Click);
            }
        }


        public event EventHandler<EventArgs> Closed;

        void closeButton_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<EventArgs> handler = this.Closed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
