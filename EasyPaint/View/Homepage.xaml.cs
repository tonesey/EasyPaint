using EasyPaint.Animations;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

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
            LoadSounds();
            RegisterMessages();
            (Application.Current as App).PlayBackgroundMusic();
        }

        private void LoadSounds()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.click2.wav"))
            {
                _soundEffect1 = SoundEffect.FromStream(stream);
            }
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
            Messenger.Default.Register<ToggleSoundMessage>(this, (action) => ReceiveMessage(action));
            //Messenger.Default.Register<RateAppMessage>(this, (action) => ReceiveMessage(action));
            // Messenger.Default.Register<PollCompletedMessage>(this, (action) => ReceiveMessage(action));
        }

        private object ReceiveMessage(BaseMessage action)
        {
            //FrameworkDispatcher.Update();
            //_soundEffect1.Play();

            if (action is GoToPageMessage)
            {
                GenericHelper.Navigate(NavigationService, Dispatcher, (action as GoToPageMessage).PageName);
                return null;
            }
            else if (action is ToggleSoundMessage)
            { 
            
            
            
            }

            return null;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (JournalEntry item in NavigationService.BackStack.Reverse())
            //{
            //    NavigationService.RemoveBackEntry();
            //}

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
            MessageBoxResult mb = MessageBox.Show(LocalizedResources.MsgBoxExit, LocalizedResources.MsgBoxTitleWarning, MessageBoxButton.OKCancel);
            if (mb != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void ImageSound_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}