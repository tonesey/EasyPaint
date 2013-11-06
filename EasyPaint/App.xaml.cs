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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using EasyPaint.ViewModel;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace EasyPaint
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        public static new App Current
        {
            get { return Application.Current as App; }
        }

        //private static MainViewModel _viewModel;
        //public static MainViewModel ViewModel
        //{
        //    get
        //    {
        //        // Delay creation of the view model until necessary
        //        return _viewModel ?? (_viewModel = new MainViewModel());
        //    }
        //}

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {


           // var _lineArtWb = BitmapFactory.New(400, 400).FromResource("/Assets/groups/3/lres/coccodrillo lineart.png");

            // StreamResourceInfo streamInfo = App.GetResourceStream(new Uri("/Image;component/Images/123.png", UriKind.Relative));


           // For content in external assemblies set the Build Action as "Resource".  
            //You should then be able to use a Uri format like: "[assemblyname];component/[filename]" to access the resource stream.
            //StreamResourceInfo streamInfo = App.GetResourceStream(new Uri("EasyPaint;component/Assets/groups/3/lres/diavolo_colore.png", 
            //    UriKind.RelativeOrAbsolute));

            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}

        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        #region audio

        public static MediaElement GlobalMediaElement
        {
            get { return Current.Resources["GlobalMedia"] as MediaElement; }
        }

        public static bool BackgroundMusicAllowed()
        {
            //disabilitata temporaneamente musica

            bool allowed = false;

            //you can check a stored property here and return false if you want to disable all bgm
            //if (!MediaPlayer.GameHasControl)
            //{
            //    //ask user about background music
            //    MessageBoxResult mbr = MessageBox.Show("press ok if you’d like to use this app’s background music (this will stop your current music playback)", "use app background music?", MessageBoxButton.OKCancel);
            //    if (mbr != MessageBoxResult.OK)
            //    {
            //        allowed = false;
            //    }
            //}

            return allowed;
        }

        public void TryPlayBackgroundMusic()
        {
            if (BackgroundMusicAllowed())
            {
                MediaPlayer.Stop(); //stop to clear any existing bg music

                GlobalMediaElement.Source = new Uri("Audio/mp3/song.mp3", UriKind.Relative);
                GlobalMediaElement.MediaOpened += MediaElement_MediaOpened; //wait until Media is ready before calling .Play()
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            App.GlobalMediaElement.Play();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (GlobalMediaElement.CurrentState != System.Windows.Media.MediaElementState.Playing)
            {
                //loop bg music
                GlobalMediaElement.Play();
            }
        }
        #endregion
    }
}