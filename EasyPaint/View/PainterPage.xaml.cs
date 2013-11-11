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

namespace EasyPaint.View
{
    public partial class PainterPage : PhoneApplicationPage
    {
        const int TotalTime = 30;

        // Constructor
        SimzzDev.DrawingBoard _myBoard;

        WriteableBitmap _lineArtPicture = null;

        private const string tmpFName = "tmp.png";
        WriteableBitmap _reducedColorsPicture = null;
        DispatcherTimer _dt = new DispatcherTimer();
        DispatcherTimer _countDownTimer = new DispatcherTimer();
        bool _gameInProgress = false;

        int _availableTimeValue = 0;

        Storyboard _storyboardSubjectImageFading;
        Storyboard _storyboardCountDown;
        Storyboard _storyboardShowPalette;

        private Dictionary<int, SoundEffect> _sounds = new Dictionary<int, SoundEffect>();

        public PainterPage()
        {
            InitializeComponent();
            //Tester.CheckImagesTester();
            _myBoard = new SimzzDev.DrawingBoard(ink);
            _dt.Interval = TimeSpan.FromSeconds(1);
            _dt.Tick += dt_Tick;
            TryPlayBackgroundMusic();
            LoadSounds();
            InitAnimations();

        }

        #region audio
        public MediaElement PaintMediaElement
        {
            get { return Resources["PaintMedia"] as MediaElement; }
        }


        public static bool BackgroundMusicAllowed()
        {
            //disabilitata temporaneamente musica

            bool allowed = true;

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

                App.GlobalMediaElement.Stop();

                PaintMediaElement.Source = new Uri("../Audio/mp3/Alegria.mp3", UriKind.Relative);
                PaintMediaElement.MediaOpened += MediaElement_MediaOpened; //wait until Media is ready before calling .Play()
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            PaintMediaElement.Play();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (PaintMediaElement.CurrentState != System.Windows.Media.MediaElementState.Playing)
            {
                //loop  music
                PaintMediaElement.Play();
            }
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
            TextBlockCountDown.Text = count.ToString();
            SoundHelper.PlaySound(_sounds[count]);
            count--;
         
            _storyboardCountDown.Begin();
            _storyboardCountDown.Completed += (sender, ev) =>
            {
                if (count == 0)
                {
                    TextBlockCountDown.Text = "go!!!";
                    SoundHelper.PlaySound(_sounds[count]);
                    _storyboardSubjectImageFading.Begin();
                    _storyboardSubjectImageFading.Completed += (sender1, ev1) =>
                    {
                        TextBlockCountDown.Visibility = Visibility.Collapsed;
                        _storyboardCountDown.Stop();
                        StartTimer();
                    };
                }
                else
                {
                    if (count == 1) {
                        _storyboardShowPalette.Begin();
                    }
                    TextBlockCountDown.Text = count.ToString();
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
            timerEllipse.Margin = new Thickness(leftMargin, -2, 0, 0);

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
            SetEllipseSize(_myBoard.BrushWidth);

            ImageMain.Visibility = System.Windows.Visibility.Visible;
            ImageTest.Visibility = System.Windows.Visibility.Collapsed;

            var selectedImage = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;

            if (selectedImage != null)
            {
                ImageMain.Source = new BitmapImage(selectedImage.ImageSource);
                //ImagesHelper.WriteContentImageToIsoStore(selectedImage.ReducedColorsResourceUri, tmpFName);
                //_reducedColorsPicture = new WriteableBitmap(ImagesHelper.GetBitmapImageFromIsoStore(tmpFName));

                _reducedColorsPicture = BitmapFactory.New(400, 400).FromResource(selectedImage.ReducedColorsResourcePath);
                _lineArtPicture = BitmapFactory.New(400, 400).FromResource(selectedImage.LineArtResourcePath);

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
                var btn = MyVisualTreeHelper.FindChild<Button>(Application.Current.RootVisual, "pc" + count);
                if (btn != null)
                {
                    btn.Background = new SolidColorBrush(color);
                }
                count++;
            }
            //#if DEBUG
            //                MessageBox.Show("colors detected : " + count);
            //#endif
            for (int i = count; i <= 6; i++)
            {
                var btn = MyVisualTreeHelper.FindChild<Button>(Application.Current.RootVisual, "pc" + i);
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
           // CheckDrawnPicture();

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

            ImageMain.Visibility = System.Windows.Visibility.Collapsed;
            ImageTest.Visibility = System.Windows.Visibility.Visible;

            //testImg.Source = userDrawnPicture;
            //MessageBox.Show("continue");

            //merge con immagine lineart
            userDrawnPicture.Blit(new Rect(0, 0, userDrawnPicture.PixelWidth, userDrawnPicture.PixelHeight),
                                  _lineArtPicture,
                                  new Rect(0, 0, _lineArtPicture.PixelWidth, _lineArtPicture.PixelHeight),
                                  WriteableBitmapExtensions.BlendMode.Alpha);
            ImageTest.Source = userDrawnPicture;

            //_lineArtPicture.Blit(new Rect(0, 0, userDrawnPicture.PixelWidth, userDrawnPicture.PixelHeight),
            //                      userDrawnPicture,
            //                      new Rect(0, 0, _lineArtPicture.PixelWidth, _lineArtPicture.PixelHeight),
            //                      WriteableBitmapExtensions.BlendMode.Alpha);
            //testImg.Source = _lineArtPicture;

            int diffPixels = ImagesHelper.GetNumberOfDifferentPixels(_reducedColorsPicture, userDrawnPicture);
            int diffPixelsPercentage = ImagesHelper.GetPercentageOfDifferentPixels(_reducedColorsPicture, userDrawnPicture);
            // textPrecision.Text = diffPixelsPercentage + "%";

            MessageBox.Show(string.Format("Precision: {0}%", diffPixelsPercentage));
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
            curSize += 2;
            if (curSize <= 6)
            {
                _myBoard.BrushWidth = curSize;
                _myBoard.BrushHeight = curSize;
            }
            else
            {
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

        private void pc1_Tap(object sender, GestureEventArgs e)
        {

        }


    }
}