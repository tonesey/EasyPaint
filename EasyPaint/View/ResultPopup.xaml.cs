using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;


namespace EasyPaint.View
{

    public enum GameAction { 
        Undefined,
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

        BackgroundWorker _bw = new BackgroundWorker();

        public event PopupClosedEventHandler PopupClosedEvent;
        public event ActionPerformedEventHandler ActionPerformedEvent;

        private readonly Popup _popup;

        public int Percentage { get; set; }

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
            TextBlockResult.Text = string.Empty;
            mainGrid.DataContext = this;

            _bw.DoWork -= _bw_DoWork;
            _bw.DoWork += _bw_DoWork;
            _bw.ProgressChanged -= _bw_ProgressChanged;
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.WorkerReportsProgress = true;

            //ProgressBarResult.
        }

        void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TextBlockResult.Text = string.Format("{0}%", e.ProgressPercentage);
            MyProgressBarResult.Value = e.ProgressPercentage;
        }

        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = 1; i <= (int)e.Argument; i++)
            {
                Thread.Sleep(50);
                worker.ReportProgress(i);
            }
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
                if (PopupClosedEvent != null) {
                    PopupClosedEvent();
                }
            }
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
            _bw.RunWorkerAsync(Percentage);

            //for (int i = 0; i <= Percentage; i++)
            //{
            //    //Dispatcher.BeginInvoke(() =>
            //    //{
            //    TextBlockResult.Text = string.Format("{0}%", i);
            //    Thread.Sleep(10);
            //    //});
            //}

            //ButtonRedo.Visibility = Visibility.Collapsed;
            //ButtonNext.Visibility = Visibility.Collapsed;
            //await RenderPercentage(Percentage);
            //ButtonRedo.Visibility = Visibility.Visible;
            //ButtonNext.Visibility = Visibility.Visible;
        }
    }
}