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
using System.Windows.Controls.Primitives;

namespace MediaPlayer
{
    public class CustomSlider : Slider
    {
        private Thumb _HorizontalThumb;
        private FrameworkElement left;
        private FrameworkElement right;

        public CustomSlider() { }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _HorizontalThumb = GetTemplateChild("HorizontalThumb") as Thumb;

            left = GetTemplateChild("LeftTrack") as FrameworkElement;
            right = GetTemplateChild("RightTrack") as FrameworkElement;

            if (left != null) left.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveThumbToMouse);
            if (right != null) right.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveThumbToMouse);
        }

        public Storyboard ProgressStoryboard { get { return (GetTemplateChild("ProgressStoryboard") as Storyboard); } }

        public Rectangle ProgressBar { get { return (GetTemplateChild("Progress") as Rectangle); } }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size s = base.ArrangeOverride(finalSize);

            if (double.IsNaN(_HorizontalThumb.Width) && (_HorizontalThumb.ActualWidth != 0))
            {
                _HorizontalThumb.Width = _HorizontalThumb.ActualWidth;
            }

            if (double.IsNaN(_HorizontalThumb.Height) && (_HorizontalThumb.ActualHeight != 0))
            {
                _HorizontalThumb.Height = _HorizontalThumb.ActualHeight;
            }

            if (double.IsNaN(_HorizontalThumb.Width)) _HorizontalThumb.Width = _HorizontalThumb.Height;
            if (double.IsNaN(_HorizontalThumb.Height)) _HorizontalThumb.Height = _HorizontalThumb.Width;

            return (s);
        }

        private void OnMoveThumbToMouse(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);

            Value = (p.X - (_HorizontalThumb.ActualWidth / 2)) / (ActualWidth - _HorizontalThumb.ActualWidth) * Maximum;
        }
    }
}
