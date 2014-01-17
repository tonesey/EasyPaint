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

            Storyboard storyBoard = new Storyboard();
            storyBoard.Children.Add(timeline1);
            storyBoard.Children.Add(timeline2);
            storyBoard.Children.Add(timeline3);
            storyBoard.Children.Add(timeline4);
            //storyBoard.AutoReverse = true;
            storyBoard.RepeatBehavior = RepeatBehavior.Forever;
            storyBoard.Begin();

        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult mb = MessageBox.Show(LocalizedResources.MsgBoxExit, LocalizedResources.MsgBoxTitleWarning, MessageBoxButton.OKCancel);
            //if (mb != MessageBoxResult.OK)
            //{
            //    e.Cancel = true;
            //}

            MyMsgbox.Show(this, MsgboxMode.YesNo, LocalizedResources.WarningExit, response =>
            {
                if (response == MsgboxResponse.No) {
                    e.Cancel = true;
                }
            });

        }
    }
}