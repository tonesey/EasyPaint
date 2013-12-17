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

namespace EasyPaint.View
{
    /// <summary>
    /// Description for Homepage.
    /// </summary>
    public partial class HomePage : PhoneApplicationPage
    {

        SoundEffect _soundEffect1 = null;

        /// <summary>
        /// Initializes a new instance of the Homepage class.
        /// </summary>
        public HomePage()
        {
            InitializeComponent();
            LoadSounds();
            RegisterMessages();
            (Application.Current as App).TryPlayBackgroundMusic();
        }

        private void LoadSounds()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.click2.wav"))
            {
                _soundEffect1 =  SoundEffect.FromStream(stream);
            }
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<GoToPageMessage>(this, (action) => ReceiveMessage(action));
            //Messenger.Default.Register<RateAppMessage>(this, (action) => ReceiveMessage(action));
            // Messenger.Default.Register<PollCompletedMessage>(this, (action) => ReceiveMessage(action));
        }

        private object ReceiveMessage(BaseMessage action)
        {
            FrameworkDispatcher.Update();
            _soundEffect1.Play();

            if (action is GoToPageMessage)
            {
                GenericHelper.Navigate(NavigationService, Dispatcher, (action as GoToPageMessage).PageName);
                return null;
            }

            return null;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (JournalEntry item in NavigationService.BackStack.Reverse())
            //{
            //    NavigationService.RemoveBackEntry();
            //}
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show("$Are you sure you want to exit?", "$Warning", MessageBoxButton.OKCancel);
            if (mb != MessageBoxResult.OK)
            {
                e.Cancel = true;
            }
        }
    }
}