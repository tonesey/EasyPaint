using EasyPaint.Animations;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Wp8Shared.UserControls;

namespace EasyPaint.View
{
    /// <summary>
    /// Description for Homepage.
    /// </summary>
    public partial class HomePage : PhoneApplicationPage
    {

        SoundEffect _soundEffect1 = null;
        private bool _backgroundAnimationStarted = false;
        Storyboard _bkgAnimationStoryboard = new Storyboard();

        /// <summary>
        /// Initializes a new instance of the Homepage class.
        /// </summary>
        public HomePage()
        {
            InitializeComponent();
            Loaded += HomePage_Loaded;
            Unloaded += HomePage_Unloaded;
        }

        void HomePage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (JournalEntry item in NavigationService.BackStack.Reverse())
            {
                NavigationService.RemoveBackEntry();
            }

            ViewModelLocator.HomepageViewModelStatic.CurrentPage = this;

            (Application.Current as App).PlayBackgroundMusic(App.TrackType.StandardBackground);

            //MessagingHelper.GetInstance().CurrentDispatcher = Dispatcher;
            //MessagingHelper.GetInstance().CurrentNavigationService = NavigationService;
            MessagingHelper.GetInstance().RegisterMessages();

            if (!_backgroundAnimationStarted)
            {
                _backgroundAnimationStarted = true;
                AnimateBackground();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PauseAnimation();
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ResumeAnimation();
            base.OnNavigatedTo(e);
        }

        private void PauseAnimation()
        {
            if (_backgroundAnimationStarted)
            {
                _bkgAnimationStoryboard.Pause();
            }
        }

        private void ResumeAnimation()
        {
            if (_backgroundAnimationStarted)
            {
                _bkgAnimationStoryboard.Resume();
            }
        }

        private void AnimateBackground()
        {

            int duration = 20;
            int beginTime = 0;
            var timeline1 = ThicknessAnimation.Create(ImageBkg, Image.MarginProperty, new Duration(TimeSpan.FromSeconds(duration)),
                           new Thickness(0, 0, 0, 0), new Thickness(-400, 0, 0, 0));
            timeline1.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            var timeline2 = ThicknessAnimation.Create(ImageBkg, Image.MarginProperty, new Duration(TimeSpan.FromSeconds(duration)),
                    new Thickness(-400, 0, 0, 0), new Thickness(-400, -240, 0, 0));
            timeline2.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            var timeline3 = ThicknessAnimation.Create(ImageBkg, Image.MarginProperty, new Duration(TimeSpan.FromSeconds(duration)),
                 new Thickness(-400, -240, 0, 0), new Thickness(0, -240, 0, 0));
            timeline3.BeginTime = TimeSpan.FromSeconds(beginTime);
            beginTime += duration;
            var timeline4 = ThicknessAnimation.Create(ImageBkg, Image.MarginProperty, new Duration(TimeSpan.FromSeconds(duration)),
                         new Thickness(0, -240, 0, 0), new Thickness(0, 0, 0, 0));
            timeline4.BeginTime = TimeSpan.FromSeconds(beginTime);

            _bkgAnimationStoryboard.Children.Add(timeline1);
            _bkgAnimationStoryboard.Children.Add(timeline2);
            _bkgAnimationStoryboard.Children.Add(timeline3);
            _bkgAnimationStoryboard.Children.Add(timeline4);
            //storyBoard.AutoReverse = true;
            _bkgAnimationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
            _bkgAnimationStoryboard.Begin();

        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyMsgbox.Show(this, MsgboxMode.YesNo, LocalizedResources.WarningExit, response =>
            {
                if (response == MsgboxResponse.No)
                {
                    e.Cancel = true;
                }
            });
        }
    }
}