﻿using EasyPaint.Helpers;
using EasyPaint.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Media;
using Wp8Shared.UserControls;

namespace EasyPaint.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class HomePageViewModel : AppViewModel
    {
        public RelayCommand StartGameCommand { get; private set; }
        public RelayCommand ShowCreditsCommand { get; private set; }
        public RelayCommand ShowSettingsCommand { get; private set; }
        public RelayCommand ToggleSoundCommand { get; private set; }
        
        private bool _isMuted = false;
        public bool IsMuted
        {
            get
            {
                return _isMuted;
            }
            private set
            {
                if (value != _isMuted)
                {
                    _isMuted = value;
                    OnPropertyChanged("IsMuted");
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the HomepageViewModel class.
        /// </summary>
        public HomePageViewModel()
        {
            StartGameCommand = new RelayCommand(() => ExecStartGameCommand());
            ShowCreditsCommand = new RelayCommand(() => ExecShowCreditsCommand());
            ShowSettingsCommand = new RelayCommand(() => ExecShowSettingsCommand());
            ToggleSoundCommand = new RelayCommand(() => ExecToggleSoundCommand());

            App.Current.MediaStateChanged -= Current_MediaStateChanged;
            App.Current.MediaStateChanged += Current_MediaStateChanged;
        }

        void Current_MediaStateChanged(bool state)
        {
            IsMuted = state;
        }

        private object ExecToggleSoundCommand()
        {
            var msg = new ToggleSoundMessage();
            Messenger.Default.Send<ToggleSoundMessage>(msg);
            return null;
        }

        private object ExecShowCreditsCommand()
        {
            MyMsgbox.Show(CurrentPage, MsgboxMode.Ok, "App engineering: Davide Tentori\nMusic: Raphael Ruis\nGraphics: MArta Todeschini, Davide Tentori\n\n Special Thanks To: Isa, Ele, Albi, Luca");
            return null;
        }

        private object ExecShowSettingsCommand()
        {
            return null;
        }

        private object ExecStartGameCommand()
        {
            //navigation 
            //http://geekswithblogs.net/lbugnion/archive/2011/01/06/navigation-in-a-wp7-application-with-mvvm-light.aspx
            //string navigationString = string.Format("/Info.xaml?appName={0}&appVer={1}&isTrial={2}", new string[] { 
            //    _appName, 
            //    _appVer, 
            //    ((App)Application.Current).IsTrial.ToString()}
            //   );

            //var msg = new GoToPageMessage() { PageName = Constants.View_GroupSeletor };
            //Messenger.Default.Send<GoToPageMessage>(msg);
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GroupSeletor);
            return null;
        }

    }
}