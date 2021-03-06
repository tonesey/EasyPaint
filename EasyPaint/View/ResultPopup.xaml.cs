﻿using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using EasyPaint.ViewModel;
using System.Windows.Media.Animation;
using EasyPaint.Helpers;


namespace EasyPaint.View
{
    public enum GameAction
    {
        Undefined,
        Menu,
        Redo,
        Ahead
    }

    public delegate void PopupClosedEventHandler();
    public delegate void ActionPerformedEventHandler(GameAction action);

    public partial class ResultPopup
    {
        public static readonly DependencyProperty PageOrientationProperty =
            DependencyProperty.Register("PageOrientation",
                                        typeof(PageOrientation),
                                        typeof(ResultPopup),
                                        new PropertyMetadata(PageOrientation.LandscapeLeft));

        public static readonly DependencyProperty PercentageProperty =
        DependencyProperty.Register("Percentage",
                                    typeof(int),
                                    typeof(ResultPopup),
                                    new PropertyMetadata(0));

        BackgroundWorker _bw = new BackgroundWorker();

        private bool _cancellationPending = false;

        public event PopupClosedEventHandler PopupClosedEvent;
        public event ActionPerformedEventHandler ActionPerformedEvent;

        private readonly Popup _popup;
        private Storyboard _sbShowTextResult;

        public int UserPercentage { get; set; }
        public int AvailableTime { get; set; }

        public int Percentage
        {
            set { SetValue(PercentageProperty, value); }
            get { return (int)GetValue(PercentageProperty); }
        }

        public PageOrientation PageOrientation
        {
            set { SetValue(PageOrientationProperty, value); }
            get { return (PageOrientation)GetValue(PageOrientationProperty); }
        }

        public ResultPopup(Popup popup)
        {
            InitializeComponent();
            LocalizeUI();
            _popup = popup;

            _cancellationPending = false;
            _sbShowTextResult = (Storyboard)Resources["StoryboardShowTextResult"];

            DataContext = this;
            //TextBlockResult.Text = string.Empty;

            _bw.DoWork -= _bw_DoWork;
            _bw.DoWork += _bw_DoWork;
            _bw.ProgressChanged -= _bw_ProgressChanged;
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.RunWorkerCompleted -= _bw_RunWorkerCompleted;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
            _bw.WorkerReportsProgress = true;

            _sbShowTextResult.Completed -= _sbShowTextResult_Completed;
            _sbShowTextResult.Completed += _sbShowTextResult_Completed;
        }

        void _sbShowTextResult_Completed(object sender, System.EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                ButtonRedo.IsEnabled = true;
                ButtonMenu.IsEnabled = true;
            });
        }

        void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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

        void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Percentage = e.ProgressPercentage;
            //TextBlockResult.Text = string.Format("{0}%", e.ProgressPercentage);
            //MyProgressBarResult.Value = e.ProgressPercentage;
        }

        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            SoundHelper.PlaySound(App.Current.Sounds["pointsCount"]);
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 1; i <= (int)e.Argument; i++)
            {
                if (_cancellationPending) return;

                Thread.Sleep(50);
                worker.ReportProgress(i);
                if (i % 5 == 0) {
                    SoundHelper.PlaySound(App.Current.Sounds["pointsCount"]);
                }
            }
            Thread.Sleep(500);
        }

        //private async Task RenderPercentage(int percentage)
        //{
        //    for (int i = 0; i <= percentage; i++)
        //    {
        //        //Dispatcher.BeginInvoke(() =>
        //        //{
        //        TextBlockResult.Text = string.Format("{0}%", i);
        //        Thread.Sleep(10);
        //        //});
        //    }
        //}

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlockResultText.Visibility = System.Windows.Visibility.Collapsed;
            TextBlockResultText.Opacity = 0.1;
            _bw.RunWorkerAsync(UserPercentage);
            ButtonRedo.IsEnabled = false;
            ButtonMenu.IsEnabled = false;
            ImageResult.Source = ResImg;

            //await RenderPercentage(Percentage);
            //ButtonRedo.Visibility = Visibility.Visible;
            //ButtonNext.Visibility = Visibility.Visible;
        }


        public System.Windows.Media.Imaging.WriteableBitmap ResImg { get; set; }

        internal void Close()
        {
            _cancellationPending = true;
        }
    }
}