﻿using System;
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
using System.Reflection;
using System.IO.IsolatedStorage;
using System.IO;
using EasyPaint.Settings;
using Microsoft.Xna.Framework.Audio;
using Wp8Shared.UserControls;

namespace EasyPaint
{
    public delegate void MediaStateChangedHandler(bool isMuted);


    public enum GameMode
    {
        Arcade,
        Gallery
    }

    public partial class App : Application
    {

        #region audio
        public event MediaStateChangedHandler MediaStateChanged;
        private Dictionary<TrackType, string> _tracks = new Dictionary<TrackType, string>();

        public enum TrackType
        {
            Paint,
            StandardBackground
        }

        private string _currentTrack;

        private Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        public Dictionary<string, SoundEffect> Sounds
        {
            get { return _sounds; }
            private set { _sounds = value; }
        }

        #endregion

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        private bool _wasApplicationTerminated = true;

        public static new App Current
        {
            get { return Application.Current as App; }
        }

        public bool IsSoundEnabled { get; set; }

        public GameMode GameMode { get; set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {

            // var test = LocalizedResources.water;
            // var _lineArtWb = BitmapFactory.New(400, 400).FromResource("/Assets/groups/3/lres/coccodrillo lineart.png");
            // StreamResourceInfo streamInfo = App.GetResourceStream(new Uri("/Image;component/Images/123.png", UriKind.Relative));

            // For content in external assemblies set the Build Action as "Resource".  
            //You should then be able to use a Uri format like: "[assemblyname];component/[filename]" to access the resource stream.
            //StreamResourceInfo streamInfo = App.GetResourceStream(new Uri("EasyPaint;component/Assets/groups/3/lres/diavolo_colore.png", 
            //    UriKind.RelativeOrAbsolute));

            IsSoundEnabled = true;

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
            InitApp();
        }

        private void InitApp()
        {
            AppSettings.LoadSettings();
            InitAudio();
            var vm = ViewModelLocator.GroupSelectorViewModelStatic;
         
        }

        private void InitAudio()
        {
            //countdown
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.three.wav"))
            {
                _sounds.Add("3", SoundEffect.FromStream(stream));
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.two.wav"))
            {
                _sounds.Add("2", SoundEffect.FromStream(stream));
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.one.wav"))
            {
                _sounds.Add("1", SoundEffect.FromStream(stream));
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.go.wav"))
            {
                _sounds.Add("0", SoundEffect.FromStream(stream));
            }
            //click
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.click.wav"))
            {
                _sounds.Add("click", SoundEffect.FromStream(stream));
            }
            //level ok
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.levelCompleted.wav"))
            {
                _sounds.Add("levelCompleted", SoundEffect.FromStream(stream));
            }
            //level ko
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.levelFailed.wav"))
            {
                _sounds.Add("levelFailed", SoundEffect.FromStream(stream));
            }
            //count
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.pointsCount.wav"))
            {
                _sounds.Add("pointsCount", SoundEffect.FromStream(stream));
            }
            //low time - alarm
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.fmbass.wav"))
            {
                _sounds.Add("lowtime", SoundEffect.FromStream(stream));
            }

            InitTracks();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            if (_wasApplicationTerminated)
            {
                // real tombstone, new App instance   
                InitApp();
            }
            else
            {
                //must have been a chooser that did not tombstone or a quick back. 
            }

            if (string.IsNullOrEmpty(_currentTrack))
            {
                _currentTrack = _tracks[TrackType.StandardBackground];
            }
            (Application.Current as App).PlayBackgroundMusic(_currentTrack);

        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            _wasApplicationTerminated = false;
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

        private void InitTracks()
        {
            _tracks.Add(TrackType.Paint, "Audio/mp3/HunterThemeLoop.mp3");
            _tracks.Add(TrackType.StandardBackground, "Audio/mp3/HunterMenuThemeLoop.mp3");
        }

        public static MediaElement GlobalMediaElement
        {
            get { return Current.Resources["GlobalMedia"] as MediaElement; }
        }

        public static bool BackgroundMusicAllowed()
        {
            bool allowed = true;
            ////you can check a stored property here and return false if you want to disable all bgm
            //if (!MediaPlayer.GameHasControl)
            //{
            //    MyMsgbox.Show(AppViewModel.CurrentPage, MsgboxMode.YesNo, LocalizedResources.WarningMusic, response =>
            //    {
            //        if (response == MsgboxResponse.No)
            //        {
            //            allowed = false;
            //        }
            //    });
            //}
            return allowed;
        }

        public void ToggleIsMute()
        {
            GlobalMediaElement.IsMuted = !GlobalMediaElement.IsMuted;

            IsSoundEnabled = !GlobalMediaElement.IsMuted;

            if (MediaStateChanged != null)
            {
                MediaStateChanged(GlobalMediaElement.IsMuted);
            }

            //if (GlobalMediaElement.CurrentState != MediaElementState.Playing)
            //{
            //    PlayBackgroundMusic(type);
            //}
            //else
            //{
            //    PauseBackgroundMusic();
            //}
        }

        public void PauseBackgroundMusic()
        {
            GlobalMediaElement.Pause();
        }

        internal void StopBackgroundMusic()
        {
            GlobalMediaElement.Stop();
        }

        public void PlayBackgroundMusic(TrackType type)
        {
            PlayBackgroundMusic(_tracks[type]);
        }

        private void PlayBackgroundMusic(string trackName)
        {
            if (!BackgroundMusicAllowed())
            {
                return;
            }

            bool trackChanged = false;

            if (trackName != _currentTrack)
            {
                _currentTrack = trackName;
                trackChanged = true;
            }

            if (trackChanged || GlobalMediaElement.CurrentState == MediaElementState.Closed)
            {
                InitTrack(trackName);
            }
            else
            {
                switch (GlobalMediaElement.CurrentState)
                {
                    case MediaElementState.Paused:
                        GlobalMediaElement.Play();
                        break;
                    case MediaElementState.Playing:
                    //track already playing
                    default:
                        break;
                }
                //if (GlobalMediaElement.CurrentState != MediaElementState.Playing)
                //{
                //    GlobalMediaElement.Play();
                //}
                //else
                //{
                //}
            }

            if (MediaStateChanged != null)
            {
                MediaStateChanged(GlobalMediaElement.IsMuted);
            }
        }

        private void InitTrack(string trackName)
        {
            MediaPlayer.Stop(); //stop to clear any existing bg music
            GlobalMediaElement.Source = new Uri(trackName, UriKind.Relative);
            GlobalMediaElement.CurrentStateChanged -= GlobalMediaElement_CurrentStateChanged;
            GlobalMediaElement.CurrentStateChanged += GlobalMediaElement_CurrentStateChanged;
            GlobalMediaElement.MediaOpened -= MediaElement_MediaOpened;
            GlobalMediaElement.MediaOpened += MediaElement_MediaOpened;
            GlobalMediaElement.MediaFailed -= GlobalMediaElement_MediaFailed;
            GlobalMediaElement.MediaFailed += GlobalMediaElement_MediaFailed;
        }

        void GlobalMediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {

        }

        void GlobalMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            throw e.ErrorException;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            App.GlobalMediaElement.Play();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (GlobalMediaElement.CurrentState != MediaElementState.Playing)
            {
                //loop bg music
                GlobalMediaElement.Play();
            }
        }
        #endregion




    }
}