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
using System.Windows.Markup;

namespace MediaPlayer
{
    [ContentProperty("Child")]
    [TemplatePart(Name = AnimateElement, Type = typeof(Storyboard))]
    public class Spinner: Control
    {
        internal const string AnimateElement = "Animate";
        private Storyboard _animate = new Storyboard();
        public Storyboard Animate
        {
            get { return _animate; }
            set { _animate = value; }
        }
        
        public Spinner(): base()
        {
            base.DefaultStyleKey = typeof(Spinner);
        }
        

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.Animate = base.GetTemplateChild(AnimateElement) as Storyboard;
        }
    }
}
