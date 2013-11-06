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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using EasyPaint.Tests;
using System.Windows.Media.Imaging;
using EasyPaint.Helpers;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;
using System.Windows.Threading;
using EasyPaint;
using System.Globalization;
using System.Threading;
using EasyPaint.ViewModel;

namespace EasyPaint.View
{
    public partial class PainterPage : PhoneApplicationPage
    {
        const int TotalTime = 60;

        // Constructor
        SimzzDev.DrawingBoard _myBoard;

        WriteableBitmap _lineArtPicture = null;

        private const string tmpFName = "tmp.png";
        WriteableBitmap _reducedColorsPicture = null;
        DispatcherTimer _dt = new DispatcherTimer();
        bool _gameInProgress = false;
   
        int _availableTimeValue = 0;

        Storyboard _sb = null;

        public PainterPage()
        {
            InitializeComponent();
            //Tester.CheckImagesTester();
            _myBoard = new SimzzDev.DrawingBoard(ink);
            //_dt.Interval = TimeSpan.FromSeconds(1);
            //_dt.Tick += dt_Tick;

            CultureInfo cc, cuic;
            cc = Thread.CurrentThread.CurrentCulture;
            cuic = Thread.CurrentThread.CurrentUICulture;

            _sb = (Storyboard)App.Current.Resources["FadeOutAnimation"];
            Storyboard.SetTarget(_sb.Children.ElementAt(0) as DoubleAnimation, timerEllipse);
        }

        #region timer
        void dt_Tick(object sender, EventArgs e)
        {
            if (_availableTimeValue - 1 > 0)
            {
                _availableTimeValue--;
            }
            else
            {
                StopTimer();
                CheckDrawnPicture();
                return;
            }

            //leftMargin / 380 = _curTimeValue / totaltime;
            int totMargin = (int)timerCanvas.Width - (int)timerEllipse.Width; //ora 380

            //inizio: _availableTimeValue = totaltime -> timerEllipse.Margin = totMargin
            //fine:   _availableTimeValue = 0         -> timerEllipse.Margin = 0
            int leftMargin = _availableTimeValue * totMargin / TotalTime;
            timerEllipse.Margin = new Thickness(leftMargin, 5, 0, 0);
        }

        private void StartTimer()
        {
            _gameInProgress = true;
            _availableTimeValue = TotalTime;
            _dt.Start();
            _sb.Begin();
        }

        private void StopTimer()
        {
            _gameInProgress = false;
            _availableTimeValue = 0;
            _dt.Stop();
            _sb.Stop();
        }
        #endregion

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            SetEllipseSize(_myBoard.BrushWidth);

            mainImg.Visibility = System.Windows.Visibility.Visible;
            testImg.Visibility = System.Windows.Visibility.Collapsed;

            var selectedImage = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;

            ImagesHelper.WriteContentImageToIsoStore(selectedImage.ReducedColorsResourceUri, tmpFName);

            _reducedColorsPicture = new WriteableBitmap(ImagesHelper.GetBitmapImageFromIsoStore(tmpFName));

            _lineArtPicture = BitmapFactory.New(_reducedColorsPicture.PixelWidth, _reducedColorsPicture.PixelHeight).FromResource(selectedImage.LineArtResourcePath);
            //_lineArtPicture = new WriteableBitmap(new BitmapImage(selectedImage.LineArtResourceUri));

            mainImg.Source = new BitmapImage(selectedImage.ImageSource);

            //ink.Height = _origPicture.PixelHeight;
            //ink.Width = _origPicture.PixelWidth;

            List<Color> c1 = ImagesHelper.GetColors(_reducedColorsPicture);
            int count = 1;
            foreach (var color in c1)
            {
                var btn = MyVisualTreeHelper.FindChild<Button>(Application.Current.RootVisual, "pc" + count);
                if (btn != null)
                {
                    btn.Background = new SolidColorBrush(color);
                }
                count++;
            }
#if DEBUG
            MessageBox.Show("colors detected : " + count);
#endif
            for (int i = count; i <= 6; i++)
            {
                var btn = MyVisualTreeHelper.FindChild<Button>(Application.Current.RootVisual, "pc" + count);
                btn.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult mb = MessageBox.Show("Sei sicuro di voler uscire?", "Attenzione", MessageBoxButton.OKCancel);
            //if (mb != MessageBoxResult.OK)
            //{
            //    e.Cancel = true;
            //}
            StopTimer();
        }

        //private void redBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    //Change the color to red.
        //    _myBoard.MainColor = Colors.Red;
        //    _myBoard.OutlineColor = Colors.Red;
        //}

        //private void blackBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    //Change the color to black.
        //    _myBoard.MainColor = Colors.Black;
        //    _myBoard.OutlineColor = Colors.Black;
        //}

        //private void penBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    //Change it to 'draw mode'.
        //    _myBoard.InkMode = SimzzDev.DrawingBoard.PenMode.pen;
        //}

        private void eraseBtn_Click(object sender, RoutedEventArgs e)
        {
            //Change it to 'erase mode'.
            _myBoard.InkMode = SimzzDev.DrawingBoard.PenMode.Erase;
        }

        private void startOrStopBtn_Click(object sender, RoutedEventArgs e)
        {
            CheckDrawnPicture();

            //if (_gameInProgress)
            //{
            //    //started: now stop
            //    StopTimer();
            //    CheckDrawnPicture();
            //}
            //else
            //{
            //    //stopped: now started
            //    _myBoard.Clear();
            //    StartTimer();
            //}
        }


        private void CheckDrawnPicture()
        {
            //WriteableBitmap drawnPicture1 = new WriteableBitmap(myBoard.Ink, null);
            WriteableBitmap userDrawnPicture = new WriteableBitmap(ImagesHelper.GetImageFromInkPresenter(_myBoard.Ink));

            mainImg.Visibility = System.Windows.Visibility.Collapsed;
            testImg.Visibility = System.Windows.Visibility.Visible;

            //merge con immagine lineart
            userDrawnPicture.Blit(
                     new Rect(0, 0, userDrawnPicture.PixelWidth, userDrawnPicture.PixelHeight), 
                     _lineArtPicture,
                     new Rect(0, 0, _lineArtPicture.PixelWidth, _lineArtPicture.PixelHeight),
                     WriteableBitmapExtensions.BlendMode.Alpha);

            testImg.Source = userDrawnPicture;

            int test = ImagesHelper.GetNotBlankPixels(userDrawnPicture);

            //int diffPixels1 = ImagesHelper.GetNumberOfDifferentPixels(drawnPicture1, drawnPicture);

            //content
            //non riesco a caricarlo.... da sempre null
            //var bmp = new BitmapImage(new Uri("Assets/Packages/Pimpa/01.PNG", UriKind.Relative));
            //WriteableBitmap origPicture1 = new WriteableBitmap(bmp);

            //List<Color> c2 = ImagesHelper.GetColors(drawnPicture);

            int diffPixels = ImagesHelper.GetNumberOfDifferentPixels(_reducedColorsPicture, userDrawnPicture);
            int diffPixelsPercentage = ImagesHelper.GetPercentageOfDifferentPixels(_reducedColorsPicture, userDrawnPicture);
            // textPrecision.Text = diffPixelsPercentage + "%";

            MessageBox.Show(string.Format("Precisione: {0}%", diffPixelsPercentage));
        }

        private void pc1_Click(object sender, RoutedEventArgs e)
        {
            _myBoard.InkMode = SimzzDev.DrawingBoard.PenMode.Pen;
            Color selectedColor = ((sender as Button).Background as SolidColorBrush).Color;
            _myBoard.OutlineColor = _myBoard.MainColor = selectedColor;
        }

        private void btnPensizeChange_Click(object sender, RoutedEventArgs e)
        {
            int curSize = _myBoard.BrushWidth;
            curSize+=2;
            if (curSize <= 6)
            {
                _myBoard.BrushWidth = curSize;
                _myBoard.BrushHeight = curSize;
            }
            else {
                //torna al minimo
                _myBoard.BrushWidth = 2;
                _myBoard.BrushHeight = 2;
            }

            //stroke 2 -> width 10
            //stroke 4 -> width 20
            //stroke 6 -> width 30

            SetEllipseSize(_myBoard.BrushWidth);
        }
      
        private void SetEllipseSize(int strokeWidth)
        {
            ellipseStrokeSize.Width = ellipseStrokeSize.Height = strokeWidth * 5;
        }

        private void SaveAndDisablePen()
        {
        }

        private void RestorePen()
        {
        }

        private void Grid_MouseLeave_1(object sender, MouseEventArgs e)
        {

        }

        private void Grid_MouseMove_1(object sender, MouseEventArgs e)
        {
           // var pos = e.GetPosition(sender as UIElement);
            //if (pos.X < 0 || pos.Y < 0)
            //{
            //}

        }


    }
}