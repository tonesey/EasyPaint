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
//using EasyPaint.Tests;
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
using System.Reflection;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Windows.Controls.Primitives;

namespace EasyPaint.View
{
    public partial class PainterPage : PhoneApplicationPage
    {
        const int TotalTime = 60;

        // Constructor
        SimzzDev.DrawingBoard _drawingboard;

        // private const string tmpFName = "tmp.png";

        WriteableBitmap _lineArtPicture = null;
        WriteableBitmap _reducedColorsPicture = null;
        DispatcherTimer _dt = new DispatcherTimer();
        DispatcherTimer _countDownTimer = new DispatcherTimer();
        bool _gameInProgress = false;

        int _availableTimeValue = 0;

        Storyboard _storyboardSubjectImageFading;
        Storyboard _storyboardCountDown;
        Storyboard _storyboardShowPalette;
        Popup _resultPopup = null;
        ResultPopup _resultPopupChild = null;

        private Dictionary<int, SoundEffect> _sounds = new Dictionary<int, SoundEffect>();

        public PainterPage()
        {
            InitializeComponent();
            //Tester.CheckImagesTester();
            _drawingboard = new SimzzDev.DrawingBoard(InkPresenterElement);
            _dt.Interval = TimeSpan.FromSeconds(1);
            _dt.Tick += dt_Tick;
            //TryPlayBackgroundMusic();
            LoadSounds();
            InitAnimations();
            AssignEventHandlers();
            InitPopup();
        }

        private void AssignEventHandlers()
        {
            UnassignEventHandlers();
            ImageOverlay.MouseLeftButtonDown += ImageOverlay_MouseLeftButtonDown;
            ImageOverlay.MouseLeftButtonUp += ImageOverlay_MouseLeftButtonUp;
            ImageOverlay.MouseMove += ImageOverlay_MouseMove;
            ImageOverlay.MouseLeave += ImageOverlay_MouseLeave;
        }

        private void UnassignEventHandlers()
        {
            ImageOverlay.MouseLeftButtonDown -= ImageOverlay_MouseLeftButtonDown;
            ImageOverlay.MouseLeftButtonUp -= ImageOverlay_MouseLeftButtonUp;
            ImageOverlay.MouseMove -= ImageOverlay_MouseMove;
            ImageOverlay.MouseLeave -= ImageOverlay_MouseLeave;
        }

        void ImageOverlay_MouseLeave(object sender, MouseEventArgs e)
        {
            _drawingboard.Ink_MouseLeave(sender, e);
        }

        void ImageOverlay_MouseMove(object sender, MouseEventArgs e)
        {
            _drawingboard.Ink_MouseMove(sender, e);
        }

        void ImageOverlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImageOverlay.ReleaseMouseCapture();
            _drawingboard.Ink_MouseLeftButtonUp(sender, e);
        }

        void ImageOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ImageOverlay.CaptureMouse();
            _drawingboard.Ink_MouseLeftButtonDown(sender, e);
        }

        #region audio
        public void TryPlayBackgroundMusic()
        {

            App.GlobalMediaElement.Stop();
            App.GlobalMediaElement.Source = new Uri("../Audio/mp3/Alegria.mp3", UriKind.RelativeOrAbsolute);

        }



        private void LoadSounds()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.three.wav"))
            {
                _sounds.Add(3, SoundEffect.FromStream(stream));
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.two.wav"))
            {
                _sounds.Add(2, SoundEffect.FromStream(stream));
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.one.wav"))
            {
                _sounds.Add(1, SoundEffect.FromStream(stream));
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyPaint.Audio.wav.go.wav"))
            {
                _sounds.Add(0, SoundEffect.FromStream(stream));
            }
        }

        #endregion

        private void ShowPalette()
        {
            _storyboardShowPalette.Begin();
        }

        private void InitAnimations()
        {
            _storyboardSubjectImageFading = (Storyboard)Resources["StoryboardSubjectFadeout"];
            _storyboardCountDown = (Storyboard)Resources["StoryboardCountdown"];
            _storyboardShowPalette = (Storyboard)Resources["StoryboardShowPalette"];
            //Storyboard.SetTarget(_storyboardImageFading.Children.ElementAt(0) as DoubleAnimation, ImageMain);
            //Storyboard.SetTarget(_storyboardCountDown.Children.ElementAt(0) as DoubleAnimation, TextBlockCountDown);
            //Storyboard.SetTarget(_storyboardShowPalette.Children.ElementAt(0) as DoubleAnimation, TextBlockCountDown);
        }

        private void StartCountDown()
        {
            int count = 3;
            TextBlockCountDownBig.Text = count.ToString();
            SoundHelper.PlaySound(_sounds[count]);
            count--;

            _storyboardCountDown.Begin();
            _storyboardCountDown.Completed += (sender, ev) =>
            {
                if (count == 0)
                {
                    TextBlockCountDownBig.Text = "go!!!";
                    SoundHelper.PlaySound(_sounds[count]);
                    _storyboardSubjectImageFading.Begin();
                    _storyboardSubjectImageFading.Completed += (sender1, ev1) =>
                    {
                        TextBlockCountDownBig.Visibility = Visibility.Collapsed;
                        _storyboardCountDown.Stop();
                        StartTimer();
                    };
                }
                else
                {
                    if (count == 1)
                    {
                        _storyboardShowPalette.Begin();
                    }
                    TextBlockCountDownBig.Text = count.ToString();
                    SoundHelper.PlaySound(_sounds[count]);
                    _storyboardCountDown.Begin();
                    count--;
                }
            };
        }

        #region timer
        void dt_Tick(object sender, EventArgs e)
        {


            if (_availableTimeValue - 1 > 0)
            {
                _availableTimeValue--;
                TextBlockCountDownSmall.Text = _availableTimeValue.ToString();
            }
            else
            {
                StopTimer();
                CheckDrawnPicture();
                return;
            }

            ////leftMargin / 380 = _curTimeValue / totaltime;
            //int totMargin = (int)timerCanvas.Width - (int)timerEllipse.Width; //ora 380
            ////inizio: _availableTimeValue = totaltime -> timerEllipse.Margin = totMargin
            ////fine:   _availableTimeValue = 0         -> timerEllipse.Margin = 0
            //int leftMargin = _availableTimeValue * totMargin / TotalTime;
            //timerEllipse.Margin = new Thickness(leftMargin, -2, 0, 0);

        }

        private void StartTimer()
        {
            _gameInProgress = true;
            _availableTimeValue = TotalTime;
            _dt.Start();
            //_storyboardSubjectImageFading.Begin();
        }

        private void StopTimer()
        {
            _gameInProgress = false;
            _availableTimeValue = 0;
            _dt.Stop();
            //_storyboardSubjectImageFading.Stop();
        }
        #endregion

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetEllipseSize(_drawingboard.BrushWidth);

            InkPresenterElement.Visibility = System.Windows.Visibility.Visible;
            ImageMain.Visibility = System.Windows.Visibility.Visible;
            ImageOverlay.Visibility = System.Windows.Visibility.Visible;
            ImageTest.Visibility = System.Windows.Visibility.Collapsed;

            var selectedImage = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;

            if (selectedImage != null)
            {
                ImageMain.Source = new BitmapImage(selectedImage.ImageSource);
                //ImagesHelper.WriteContentImageToIsoStore(selectedImage.ReducedColorsResourceUri, tmpFName);
                //_reducedColorsPicture = new WriteableBitmap(ImagesHelper.GetBitmapImageFromIsoStore(tmpFName));

                _reducedColorsPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(selectedImage.ReducedColorsResourcePath);
                _lineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(selectedImage.LineArtResourcePath);

                ImageOverlay.Source = _lineArtPicture;

                //_lineArtPicture = new WriteableBitmap(new BitmapImage(selectedImage.LineArtResourceUri));
                //ink.Height = _origPicture.PixelHeight;
                //ink.Width = _origPicture.PixelWidth;

                InitPalette();

                StartCountDown();

            }
        }

        private void InitPalette()
        {
            List<Color> imageColors = ImagesHelper.GetColors(_reducedColorsPicture);
            int count = 1;
            foreach (var color in imageColors)
            {
                var parentEl = MyVisualTreeHelper.FindChild<Viewbox>(Application.Current.RootVisual, "pc" + count);
                parentEl.Tag = color;
                var childEl = MyVisualTreeHelper.FindChild<System.Windows.Shapes.Path>(parentEl, "pc" + count + "_path");
                if (childEl != null)
                {
                    childEl.Fill = new SolidColorBrush(color);
                }
                count++;
            }
            //#if DEBUG
            //                MessageBox.Show("colors detected : " + count);
            //#endif
            for (int i = count; i <= 5; i++)
            {
                var el = MyVisualTreeHelper.FindChild<Viewbox>(Application.Current.RootVisual, "pc" + i);
                el.Visibility = Visibility.Collapsed;
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

        private void eraseBtn_Click(object sender, RoutedEventArgs e)
        {
            //Change it to 'erase mode'.
            _drawingboard.InkMode = SimzzDev.DrawingBoard.PenMode.Erase;
        }

        #region palette
        //private void pc1_Click(object sender, RoutedEventArgs e)
        //{
        //    _drawingboard.InkMode = SimzzDev.DrawingBoard.PenMode.Pen;
        //    Color selectedColor = ((sender as Button).Background as SolidColorBrush).Color;
        //    _drawingboard.OutlineColor = _drawingboard.MainColor = selectedColor;
        //}

        private void pc1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _drawingboard.InkMode = SimzzDev.DrawingBoard.PenMode.Pen;

            Viewbox senderViewBox = (sender as Viewbox);
            Color selectedColor = (Color)senderViewBox.Tag;
            _drawingboard.OutlineColor = _drawingboard.MainColor = selectedColor;
        }
        #endregion

        #region toolbox
        private void btnPensizeChange_Click(object sender, RoutedEventArgs e)
        {
            int curSize = _drawingboard.BrushWidth;
            curSize += 8;
            if (curSize <= 18)
            {
                _drawingboard.BrushWidth = curSize;
                _drawingboard.BrushHeight = curSize;
            }
            else
            {
                //torna al minimo
                _drawingboard.BrushWidth = 2;
                _drawingboard.BrushHeight = 2;
            }

            //stroke 2 -> width 10
            //stroke 4 -> width 20
            //stroke 6 -> width 30

            SetEllipseSize(_drawingboard.BrushWidth);
        }

        private void SetEllipseSize(int strokeWidth)
        {
            //ellipseStrokeSize.Width = ellipseStrokeSize.Height = strokeWidth * 5;

            switch (strokeWidth)
            {
                case 2:
                    ellipseStrokeSize.Width = ellipseStrokeSize.Height = 10;
                    break;
                case 10:
                    ellipseStrokeSize.Width = ellipseStrokeSize.Height = 20;
                    break;
                case 18:
                    ellipseStrokeSize.Width = ellipseStrokeSize.Height = 30;
                    break;
                default:
                    break;
            }
        }

        #endregion

        private void CheckDrawnPicture()
        {
            WriteableBitmap userDrawnPicture = null;

            //test1
            //BitmapImage imgFromInk = ImagesHelper.GetImageFromInkPresenter(_drawingboard.Ink);
            //userDrawnPicture = new WriteableBitmap(imgFromInk);

            userDrawnPicture = new WriteableBitmap((int)_drawingboard.Ink.Width, (int)_drawingboard.Ink.Height);
            userDrawnPicture.Render(_drawingboard.Ink, null);
            userDrawnPicture.Invalidate();

            //2. caricamento da disco
            //BitmapImage biImage = new BitmapImage();
            //using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream("temp.png", FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication()))
            //{
            //    biImage.SetSource(isfStream);
            //}
            //userDrawnPicture = new WriteableBitmap(biImage);

            //test2
           
            //Test1.Source = userDrawnPicture;

            //test3
            //WriteableBitmap userDrawnPicture = new WriteableBitmap((int)ImageOverlay.Width, (int)ImageOverlay.Height);
            //userDrawnPicture.Render(GridPainter, null);
            //userDrawnPicture.Invalidate();
            
            //Test1.Source = userDrawnPicture;
            //Test2.Source = _lineArtPicture;

            InkPresenterElement.Visibility = System.Windows.Visibility.Collapsed;
            ImageOverlay.Visibility = System.Windows.Visibility.Collapsed;
            ImageMain.Visibility = System.Windows.Visibility.Collapsed;
            ImageTest.Visibility = System.Windows.Visibility.Visible;

            //////////////////// 
            // merge 1
            ////////////////////
            //Grid g = new Grid();
            //g.Width = ImageOverlay.Width;
            //g.Height = ImageOverlay.Height;

            //Image inkImage1 = new Image();
            //inkImage1.Source = imgFromInk;
            ////Test1.Source = imgFromInk;

            //Image lineartImage1 = new Image();
            //lineartImage1.Source = _lineArtPicture;
            ////

            //g.Children.Add(lineartImage1);
            //g.Children.Add(inkImage1);

            //WriteableBitmap resultigImg = new WriteableBitmap((int)ImageOverlay.Width, (int)ImageOverlay.Height);
            //resultigImg.Render(g, null);
            //resultigImg.Invalidate();
            //ImageTest.Source = resultigImg;
            ////////////////////

            //////////////////// 
            // merge 2
            ////////////////////
            userDrawnPicture.Blit(new Rect(0, 0, userDrawnPicture.PixelWidth, userDrawnPicture.PixelHeight),
                                  _lineArtPicture,
                                  new Rect(0, 0, _lineArtPicture.PixelWidth, _lineArtPicture.PixelHeight),
                                  WriteableBitmapExtensions.BlendMode.Alpha);
            //ImageTest.Source = userDrawnPicture;

            ////_lineArtPicture.Blit(new Rect(0, 0, userDrawnPicture.PixelWidth, userDrawnPicture.PixelHeight),
            ////                      userDrawnPicture,
            ////                      new Rect(0, 0, _lineArtPicture.PixelWidth, _lineArtPicture.PixelHeight),
            ////                      WriteableBitmapExtensions.BlendMode.Alpha);
            ////testImg.Source = _lineArtPicture;


            //////////////////// 
            // merge 3
            ////////////////////
            //for (int y = 0; y < _lineArtPicture.PixelHeight; ++y)
            //{
            //    for (int x = 0; x < _lineArtPicture.PixelWidth; ++x)
            //    {
            //        userDrawnPicture.SetPixel(x, y, Colors.Red);
            //    }
            //}
            //userDrawnPicture.Invalidate();
            //ImageTest.Source = userDrawnPicture;


            int diffPixels = ImagesHelper.GetNumberOfDifferentPixels(_reducedColorsPicture, userDrawnPicture);
            int diffPixelsPercentage = ImagesHelper.GetPercentageOfDifferentPixels(_reducedColorsPicture, userDrawnPicture);

            ShowResultPopup(diffPixelsPercentage);
            //// textPrecision.Text = diffPixelsPercentage + "%";
            //MessageBox.Show(string.Format("Precision: {0}%", diffPixelsPercentage));
        }

        private void ShowResultPopup(int percentage)
        {
            
            //popup.VerticalOffset = 250;
            DisablePage();
            _resultPopupChild.Percentage = percentage;
            _resultPopupChild.PageOrientation = Orientation;
            _resultPopup.IsOpen = true;
        }

        private void InitPopup()
        {
            _resultPopup = new Popup();
            _resultPopup.Height = Application.Current.Host.Content.ActualHeight;
            _resultPopupChild = new ResultPopup(_resultPopup);
         
            _resultPopupChild.PopupClosedEvent -= exportPopup_PopupClosedEvent;
            _resultPopupChild.PopupClosedEvent += exportPopup_PopupClosedEvent;
            _resultPopupChild.ActionPerformedEvent -= exportPopup_ActionPerformedEvent;
            _resultPopupChild.ActionPerformedEvent += exportPopup_ActionPerformedEvent;
            //exportPopup.Width = Application.Current.Host.Content.ActualWidth;
            _resultPopup.Child = _resultPopupChild;
        }

        void exportPopup_ActionPerformedEvent(GameAction action)
        {
            //TODO inviare  messaggio 
            MessageBox.Show("TODO");
        }

        void exportPopup_PopupClosedEvent()
        {
            EnablePage();
        }

        private void DisablePage()
        {
            IsEnabled = false;
        }

        private void EnablePage()
        {
            IsEnabled = true;
        }

        private void TextBlockCountDownSmall_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

#if DEBUG
            StopTimer();
            CheckDrawnPicture();
#endif
        }



    }
}