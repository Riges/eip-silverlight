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
using System.Windows.Navigation;
using EIP.Views.Controls;
using EIP.Objects;
using System.Windows.Threading;
using System.Windows.Browser;

namespace EIP.Views
{
    public partial class VideoView : Page
    {
        public long uid { get; set; }
        public long vid { get; set; }
        public long accountID { get; set; }

        public VideoView()
        {
            InitializeComponent();

            _Timer.Interval = new TimeSpan(0, 0, 5);
            _Timer.Tick += new EventHandler(_Timer_Tick);
            App.Current.Host.Content.FullScreenChanged += new System.EventHandler(Content_FullScreenChanged);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (this.NavigationContext.QueryString.ContainsKey("uid"))
                this.uid = Convert.ToInt64(this.NavigationContext.QueryString["uid"]);

            if (this.NavigationContext.QueryString.ContainsKey("vid"))
                this.vid = Convert.ToInt64(this.NavigationContext.QueryString["vid"]);

            if (this.NavigationContext.QueryString.ContainsKey("accid"))
                this.accountID = Convert.ToInt64(this.NavigationContext.QueryString["accid"]);

            LayoutRoot2.MouseEnter += new MouseEventHandler(LayoutRoot2_MouseEnter);
            LayoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp);
            controlsContainer.MouseLeave += new MouseEventHandler(controlsContainer_MouseLeave);

            // TODO: check if there isn't one and show an image
            //mediaPlayer.Source = App.Current.Resources["m"] as Uri;


           VideoLight video = ((AccountFacebookLight)Connexion.accounts[accountID]).videos[uid][vid];



           mediaPlayer.Source = new Uri((video.src != "" ? video.src : video.src_hq), UriKind.Absolute);

            #region Additional Startup Params
            if (App.Current.Resources.Contains("autostart"))
            {
                mediaPlayer.AutoPlay = Convert.ToBoolean(App.Current.Resources["autostart"]);
            }
            if (App.Current.Resources.Contains("thumbnail"))
            {
                ThumbnailImage.SetValue(Image.SourceProperty, App.Current.Resources["thumbnail"].ToString());
                Thumbnail.Visibility = Visibility.Visible;
            }
            if (App.Current.Resources.Contains("markers"))
            {
                mediaControls.EnableMarkers = Convert.ToBoolean(App.Current.Resources["markers"]);
            }
            if (App.Current.Resources.Contains("cc"))
            {
                mediaControls.EnableCC = Convert.ToBoolean(App.Current.Resources["cc"]);
            }
            if (App.Current.Resources.Contains("markerpath"))
            {
                mediaControls.MarkerPath = App.Current.Resources["markerpath"] as Uri;
            }
            if (App.Current.Resources.Contains("muted"))
            {
                mediaPlayer.IsMuted = Convert.ToBoolean(App.Current.Resources["muted"]);
            }
            if (App.Current.Resources.Contains("autohide"))
            {
                _autoHideControls = Convert.ToBoolean(App.Current.Resources["autohide"]);
            }
            if (App.Current.Resources.Contains("canscrub"))
            {
                mediaControls.sliderTimeline.IsHitTestVisible = Convert.ToBoolean(App.Current.Resources["canscrub"]);
            }
            #endregion

            mediaControls.Media = mediaPlayer;

            _defaultPlayerWidth = this.Width;
            _defaultPlayerHeight = this.Height;


            mediaPlayer.Width = _defaultPlayerWidth;
            mediaPlayer.Height = _defaultPlayerHeight;

            RepositionCenterPlay();

            mediaControls.FullScreenClicked += new MediaPlayer.EventHandler(mediaControls_FullScreenClicked);
            mediaControls.PlayClicked += new MediaPlayer.EventHandler(mediaControls_PlayClicked);
            mediaControls.PauseClicked += new MediaPlayer.EventHandler(mediaControls_PauseClicked);
            mediaControls.StopClicked += new MediaPlayer.EventHandler(mediaControls_StopClicked);
            mediaControls.BufferingStart += new MediaPlayer.EventHandler(mediaControls_BufferingStart);
            mediaControls.BufferingEnd += new MediaPlayer.EventHandler(mediaControls_BufferingEnd);
            mediaControls.MediaCompleted += new MediaPlayer.EventHandler(mediaControls_MediaCompleted);
            mediaControls.SoundChanged += new MediaPlayer.SoundEventHandler(mediaControls_SoundChanged);
            mediaControls.MarkerReached += new MediaPlayer.MarkerEventHandler(mediaControls_MarkerReached);

            controlsContainer.Visibility = Visibility.Visible;
            controlsIn.Begin();
            BigBuffer.Visibility = Visibility.Visible;
            BigBuffer.Animate.Begin();
        }

        private DispatcherTimer _Timer = new DispatcherTimer();
        private double _defaultPlayerWidth = 0.0;
        private double _defaultPlayerHeight = 0.0;
        private DateTime _lastClick = DateTime.Now;

        private ClosedCaptionOk _cc = null;

        private bool _autoHideControls = true;

       
        #region Handlers

        void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // The the last click was within 500ms...
            if ((DateTime.Now - _lastClick).TotalMilliseconds < 500)
            {
                // Toggle fullscreen
                Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;

                _lastClick = DateTime.Now.AddSeconds(-1);
            }
            else
            {

                // Store the last click time
                _lastClick = DateTime.Now;
            }
        }
        void mediaControls_MarkerReached(object sender, TimelineMarker m)
        {
            if (_cc != null)
            {
                if (string.Compare(m.Type, "cc", StringComparison.InvariantCultureIgnoreCase) <= 0)
                {
                    _cc.Update(m.Text);
                }
            }
        }
        void mediaControls_SoundChanged(object sender, bool isMuted)
        {
            if (mediaControls.EnableCC)
            {
                if (_cc == null)
                {
                    _cc = new ClosedCaptionOk();
                    _cc.VerticalAlignment = VerticalAlignment.Top;
                    _cc.Margin = new Thickness(100, 100, 0, 0);
                    LayoutRoot.Children.Add(_cc);
                }
                if (isMuted)
                {
                    _cc.Visibility = Visibility.Visible;
                }
                else
                    _cc.Visibility = Visibility.Collapsed;
            }

        }
        void mediaControls_MediaCompleted(object sender, EventArgs e)
        {
            Thumbnail.Visibility = Visibility.Visible;
        }
        void mediaControls_BufferingEnd(object sender, EventArgs e)
        {
            BigBuffer.Animate.Stop();
            LargeSpinnerArea.Visibility = Visibility.Collapsed;
        }
        void mediaControls_BufferingStart(object sender, EventArgs e)
        {
            BigBuffer.Animate.Begin();
            LargeSpinnerArea.Visibility = Visibility.Visible;
        }
        void mediaControls_StopClicked(object sender, EventArgs e)
        {
            PlayIcon.Visibility = Visibility.Visible;
        }
        void mediaControls_PauseClicked(object sender, EventArgs e)
        {
            PlayIcon.Visibility = Visibility.Visible;
        }
        void mediaControls_PlayClicked(object sender, EventArgs e)
        {
            Thumbnail.Visibility = Visibility.Collapsed;
            PlayIcon.Visibility = Visibility.Collapsed;
        }
        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            if (App.Current.Host.Content.IsFullScreen == true)
            {
                double targetWidth = (double)App.Current.Host.Content.ActualWidth;
                double targetHeight = (double)App.Current.Host.Content.ActualHeight;

                this.Width = targetWidth;
                this.Height = targetHeight;

                mediaPlayer.Width = targetWidth;
                mediaPlayer.Height = targetHeight;
            }

            else
            {
                this.Width = _defaultPlayerWidth;
                this.Height = _defaultPlayerHeight;

                mediaPlayer.Width = _defaultPlayerWidth * 0.8;
                mediaPlayer.Height = _defaultPlayerHeight * 0.8;
            }

            RepositionCenterPlay();
        }
        void mediaControls_FullScreenClicked(object sender, EventArgs e)
        {
            if (App.Current.Host.Content.IsFullScreen)
            {
                App.Current.Host.Content.IsFullScreen = false;
            }
            else
            {
                App.Current.Host.Content.IsFullScreen = true;
            }
        }
        void controlsContainer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_autoHideControls)
            {
                _Timer.Start();
            }
        }
        void _Timer_Tick(object sender, EventArgs e)
        {
            controlsOut.Completed += new EventHandler(controlsOut_Completed);
            controlsOut.Begin();
        }
        void controlsOut_Completed(object sender, EventArgs e)
        {
            controlsContainer.Visibility = Visibility.Collapsed;
        }
        void LayoutRoot2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_autoHideControls)
            {
                if (controlsContainer.Visibility == Visibility.Collapsed)
                {
                    controlsContainer.Visibility = Visibility.Visible;
                    controlsIn.Begin();
                }
                _Timer.Stop();
            }
        }
        private void PlayIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mediaControls.CenterPlay();
        }
        #endregion
        private void RepositionCenterPlay()
        {
            double newLeft = (App.Current.Host.Content.ActualWidth / 2) - 50;
            double newTop = (App.Current.Host.Content.ActualHeight / 2) - 50;

            PlayIcon.SetValue(Canvas.LeftProperty, newLeft);
            PlayIcon.SetValue(Canvas.TopProperty, newTop);

            LargeSpinnerArea.SetValue(Canvas.LeftProperty, newLeft);
            LargeSpinnerArea.SetValue(Canvas.TopProperty, newTop);
        }

        [ScriptableMember]
        public void SeekPlayback(string time)
        {
            TimeSpan tsTime = TimeSpan.Parse(time);
            mediaControls.Seek(tsTime);
        }

    }
}
