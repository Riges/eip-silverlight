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

namespace EIP.Views.Controls
{
    public partial class ClosedCaptionOk : UserControl
    {
        private Dictionary<UIElement, DoubleAnimationUsingKeyFrames> _doubleAnimations = new Dictionary<UIElement, DoubleAnimationUsingKeyFrames>();
        private Stack<TextBlock> _lines = new Stack<TextBlock>();

        private double _Speed = .75;
        public double Speed
        {
            get { return _Speed; }
            set { _Speed = value; }
        }
        private Storyboard _sb = new Storyboard();
        public Storyboard Animation
        {
            get { return _sb; }
        }

        public ClosedCaptionOk()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ClosedCaption_Loaded);
        }

        public void Update(string text)
        {
            if (_sb.GetCurrentState() == ClockState.Stopped)
                _sb.Begin();
            if (_lines.Count > 0)
            {
                TextBlock line = _lines.Pop();
                line.Text = text;
            }
        }

        public void Pause()
        {
            _sb.Pause();
        }

        void ClosedCaption_Loaded(object sender, RoutedEventArgs e)
        {
            tbLine0.LineHeight = 20;
            tbLine1.LineHeight = 20;
            tbLine2.LineHeight = 20;
            //use this list to keep track of what line can be filled
            _lines.Push(tbLine2);
            _lines.Push(tbLine1);
            _lines.Push(tbLine0);

            //tbLine0.SizeChanged += new SizeChangedEventHandler(tbLine_SizeChanged);
            //tbLine1.SizeChanged += new SizeChangedEventHandler(tbLine_SizeChanged);

            AddRenderTransform(tbLine0, 40);
            AddRenderTransform(tbLine1, 60);
            AddRenderTransform(tbLine2, 80);

            _sb.Completed += new EventHandler(_sb_Completed);

        }

        //void tbLine_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    TextBlock line = sender as TextBlock;
        //    double mod = (line.DesiredSize.Height % 20);
        //    line.Height = (line.DesiredSize.Height + mod) + 40;
        //}

        void _sb_Completed(object sender, EventArgs e)
        {
            foreach (KeyValuePair<UIElement, DoubleAnimationUsingKeyFrames> kvp in _doubleAnimations)
            {
                TextBlock line = kvp.Key as TextBlock;
                DoubleAnimationUsingKeyFrames da = kvp.Value;

                if (da.KeyFrames[0].Value == 0)
                {
                    line.Visibility = Visibility.Collapsed;
                    da.KeyFrames[0].Value = 40;
                    da.KeyFrames[1].Value = 40;

                    line.Text = string.Empty;
                    _lines.Push(line);
                }
                else
                {
                    line.Visibility = Visibility.Visible;
                    da.KeyFrames[0].Value -= 20;
                    da.KeyFrames[1].Value = da.KeyFrames[0].Value;
                }
            }
            _sb.Begin();
        }

        private void AddRenderTransform(UIElement line, double startPosition)
        {
            TransformGroup tg = new TransformGroup();
            TranslateTransform translate = new TranslateTransform();
            translate.Y = startPosition;
            tg.Children.Add(translate);
            line.RenderTransform = tg;

            DoubleAnimationUsingKeyFrames da = new DoubleAnimationUsingKeyFrames();
            SplineDoubleKeyFrame sdkf = new SplineDoubleKeyFrame();

            sdkf.KeyTime = TimeSpan.FromSeconds(.5);
            sdkf.KeySpline = new KeySpline();
            sdkf.KeySpline.ControlPoint1 = new Point(0.80, 0.004);
            sdkf.KeySpline.ControlPoint1 = new Point(0.080, 1);

            sdkf.Value = startPosition - 20;
            da.KeyFrames.Add(sdkf);
            Storyboard.SetTarget(da, translate);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Y)"));

            DiscreteDoubleKeyFrame ddkf = new DiscreteDoubleKeyFrame();
            ddkf.KeyTime = TimeSpan.FromSeconds(_Speed);
            ddkf.Value = startPosition - 20;
            da.KeyFrames.Add(ddkf);

            _sb.Children.Add(da);

            _doubleAnimations.Add(line, da);
        }
    }
}
