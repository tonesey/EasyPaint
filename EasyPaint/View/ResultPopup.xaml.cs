using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using EasyPaint.ViewModel;
using System.Windows.Media.Animation;
using EasyPaint.Helpers;
using System.Windows.Controls;


namespace EasyPaint.View
{
    public enum GameAction
    {
        Undefined,
        Menu,
        Redo,
        Ahead
    }

    public enum ScoreType
    {
        Percentage,
        DrawScore,
        TimeScore,
        TotalScore
    }

    public delegate void PopupClosedEventHandler();
    public delegate void ActionPerformedEventHandler(GameAction action);

    public partial class ResultPopup : INotifyPropertyChanged
    {
        public static DependencyProperty PageOrientationProperty =
        DependencyProperty.Register("PageOrientation",
                                        typeof(PageOrientation),
                                        typeof(ResultPopup),
                                        new PropertyMetadata(PageOrientation.LandscapeLeft));

        private bool _cancellationPending = false;

        public event PopupClosedEventHandler PopupClosedEvent;
        public event ActionPerformedEventHandler ActionPerformedEvent;

        private readonly Popup _popup;
        private Storyboard _sbShowTextResult;

        public PageOrientation PageOrientation
        {
            set { SetValue(PageOrientationProperty, value); }
            get { return (PageOrientation)GetValue(PageOrientationProperty); }
        }

        public int InputUserPercentage { get; set; }
        public int InputSavedTime { get; set; }


        #region scores to draw
        private int _percentage = 0;
        public int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                NotifyPropertyChanged("Percentage");
            }
        }

        private int _totalScore = 0;
        public int TotalScore
        {
            get
            {
                return _totalScore;
            }
            set
            {
                _totalScore = value;
                NotifyPropertyChanged("TotalScore");
            }
        }

        private int _userScoreDrawValue = 0;
        public int UserScoreDrawValue
        {
            get
            {
                return _userScoreDrawValue;
            }
            set
            {
                _userScoreDrawValue = value;
                NotifyPropertyChanged("UserScoreDrawValue");
            }
        }

        private int _userScoreTimeValue = 0;
        public int UserScoreTimeValue
        {
            get
            {
                return _userScoreTimeValue;
            }
            set
            {
                _userScoreTimeValue = value;
                NotifyPropertyChanged("UserScoreTimeValue");
            }
        }
        #endregion

        public ResultPopup(Popup popup)
        {
            InitializeComponent();
            LocalizeUI();
            _popup = popup;
            HideElements();

            _cancellationPending = false;
            _sbShowTextResult = (Storyboard)Resources["StoryboardShowTextResult"];

            DataContext = this;

            _sbShowTextResult.Completed -= _sbShowTextResult_Completed;
            _sbShowTextResult.Completed += _sbShowTextResult_Completed;
        }

        private async Task RenderScore(ScoreType scoreType, int value)
        {
            await Task.Run(() =>
            {
                int increment = 0;
                switch (scoreType)
                {
                    case ScoreType.Percentage:
                        increment = 1;
                        break;
                    case ScoreType.DrawScore:
                        increment = 10;
                        break;
                    case ScoreType.TimeScore:
                        increment = 5;
                        break;
                    case ScoreType.TotalScore:
                        increment = 5;
                        break;
                }

                for (int i = 0; i < value; i += increment)
                {
                    if (_cancellationPending) return;
                    Dispatcher.BeginInvoke(() =>
                    {
                        switch (scoreType)
                        {
                            case ScoreType.Percentage:
                                Percentage = i;
                                break;
                            case ScoreType.DrawScore:
                                UserScoreDrawValue = i;
                                break;
                            case ScoreType.TimeScore:
                                UserScoreTimeValue = i;
                                break;
                            case ScoreType.TotalScore:
                                TotalScore = i;
                                break;
                        }
                    });

                    int soundInterval = 0;
                    switch (scoreType)
                    {
                        case ScoreType.Percentage:
                            soundInterval = 5;
                            Thread.Sleep(40);
                            break;
                        case ScoreType.DrawScore:
                            soundInterval = 10 * 5;
                            Thread.Sleep(5);
                            break;
                        case ScoreType.TimeScore:
                            soundInterval = 5 * 5;
                            Thread.Sleep(5);
                            break;
                        case ScoreType.TotalScore:
                            soundInterval = 10 * 5;
                            Thread.Sleep(1);
                            break;
                    }

                    if (i % soundInterval == 0)
                    {
                        SoundHelper.PlaySound(App.Current.Sounds["pointsCount"]);
                    }
                }
                Thread.Sleep(500);
            });
        }


        private void LocalizeUI()
        {
        }

        private void ClosePopup()
        {
            if (_popup != null)
            {
                _popup.IsOpen = false;
                if (PopupClosedEvent != null)
                {
                    PopupClosedEvent();
                }
            }
        }

        private void ButtonMenu_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ActionPerformedEvent != null)
            {
                ActionPerformedEvent(GameAction.Menu);
            }
            ClosePopup();
        }

        private void ButtonRedo_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ActionPerformedEvent != null)
            {
                ActionPerformedEvent(GameAction.Redo);
            }
            ClosePopup();
        }

        private void ButtonAhead_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ActionPerformedEvent != null)
            {
                ActionPerformedEvent(GameAction.Ahead);
            }
            ClosePopup();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            HideElements();

            TextBlockResultText.Opacity = 0.1;
            ButtonRedo.IsEnabled = false;
            ButtonMenu.IsEnabled = false;
            ImageResult.Source = ResImg;

            await RenderScore(ScoreType.Percentage, InputUserPercentage);

            if (_cancellationPending) return;

            Dispatcher.BeginInvoke(() =>
            {
                TextBlockResultText.Visibility = System.Windows.Visibility.Visible;
            });

            _sbShowTextResult.Begin();
            if (ButtonNext.Visibility == System.Windows.Visibility.Collapsed)
            {
                SoundHelper.PlaySound(App.Current.Sounds["levelFailed"]);
            }
            else
            {
                SoundHelper.PlaySound(App.Current.Sounds["levelCompleted"]);
            }
        }

        async void _sbShowTextResult_Completed(object sender, System.EventArgs e)
        {
          
            Thread.Sleep(1000);

            Dispatcher.BeginInvoke(() =>
            {
                ButtonRedo.IsEnabled = true;
                ButtonMenu.IsEnabled = true;
            });

            bool levelPassed = ButtonNext.Visibility == System.Windows.Visibility.Visible;

            //draw score
            int drawScore = InputUserPercentage * 10;
            Dispatcher.BeginInvoke(() =>
            {
                StackPanelDrawScore.Visibility = Visibility.Visible;
            });
            await RenderScore(ScoreType.DrawScore, drawScore);

            //time score
            int timeScore = InputSavedTime * 5;
            Dispatcher.BeginInvoke(() =>
            {
                StackPanelTimeScore.Visibility = Visibility.Visible;
            });
            await RenderScore(ScoreType.TimeScore, timeScore);

            //total
            int totalScore = drawScore + timeScore;
            Dispatcher.BeginInvoke(() =>
            {
                StackPanelTotalScore.Visibility = Visibility.Visible;
            });
            await RenderScore(ScoreType.TotalScore, totalScore);


        }

        private void HideElements()
        {
            TextBlockResultText.Visibility = System.Windows.Visibility.Collapsed;
            StackPanelDrawScore.Visibility = Visibility.Collapsed;
            StackPanelTimeScore.Visibility = Visibility.Collapsed;
            StackPanelTotalScore.Visibility = Visibility.Collapsed;
        }

        public System.Windows.Media.Imaging.WriteableBitmap ResImg { get; set; }

        internal void Close()
        {
            _cancellationPending = true;
        }

        internal long GetTotalScore()
        {
            return TotalScore;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}