using EasyPaint.Helpers;
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
        public RelayCommand StartGameArcadeCommand { get; private set; }
        public RelayCommand StartGameGalleryCommand { get; private set; }
        public RelayCommand ShowCreditsCommand { get; private set; }
        public RelayCommand ShowHelpCommand { get; private set; }
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
            StartGameArcadeCommand = new RelayCommand(() => ExecStartGameCommand(GameMode.Arcade));
            StartGameGalleryCommand = new RelayCommand(() => ExecStartGameCommand(GameMode.Gallery));
            ShowCreditsCommand = new RelayCommand(() => ExecShowCreditsCommand());
            ShowHelpCommand = new RelayCommand(() => ExecShowHelpCommand());
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
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Credits);
            return null;
        }

        private object ExecShowHelpCommand()
        {

            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Help);
            return null;
        }

        private object ExecStartGameCommand(GameMode gameMode)
        {
            App.Current.GameMode = gameMode;
            switch (gameMode)
            {
                case GameMode.Arcade:
                   // ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GroupSeletor);
                    ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GameLevelSelector);
                    break;
                case GameMode.Gallery:
                    ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Gallery);
                    break;
            }
            return null;
        }

    }
}