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
using EasyPaint.Model;
using System.Windows.Ink;
using EasyPaint.Settings;

namespace EasyPaint.View
{
    public partial class PainterPage : PhoneApplicationPage
    {
        const int TotalTime = 30;

        // Constructor
        SimzzDev.DrawingBoard _drawingboard = null;

        // private const string tmpFName = "tmp.png";

        WriteableBitmap _lineArtPicture = null;
        WriteableBitmap _reducedColorsPicture = null;
        WriteableBitmap _reducedColorsLineArtPicture = null;

        //DispatcherTimer _dt = new DispatcherTimer();
        //DispatcherTimer _countDownTimer = new DispatcherTimer();

        bool _gameInProgress = false;

        int _curAvailableTimeValue = 0;
        int _lastAvailableTimeValue = 0;

        Storyboard _storyboardSubjectImageFading;
        Storyboard _storyboardCountDown;
        Storyboard _storyboardShowPalette;
        Storyboard _storyboardColorSelected;
        Storyboard _storyboardSmallCountDownAnimation;
        EventHandler _storyboardSmallCountDownAnimationHandler;

        Popup _resultPopup = null;
        ResultPopup _resultPopupChild = null;

        //private const int MAX_PALETTE_COLORS = 4;
        //List<MyColor> _paletteColors = new List<MyColor>();
        //List<MyColor> _ignoredColors = new List<MyColor>();

        List<Color> _paletteColors = new List<Color>();
 
    

        public PainterPage()
        {
            InitializeComponent();
            //Tester.CheckImagesTester();
            _drawingboard = new SimzzDev.DrawingBoard(InkPresenterElement);
            //_dt.Interval = TimeSpan.FromSeconds(1);
            //_dt.Tick += dt_Tick;

            InitAnimations();
            AssignEventHandlers();
            InitPopup();

            BorderPalette.Visibility = Visibility.Collapsed;
        }

        private void AssignEventHandlers()
        {
            UnassignEventHandlers();
        }

        private void UnassignEventHandlers()
        {
        }

        //void ImageOverlay_MouseMove(object sender, MouseEventArgs e)
        //{
        //    _drawingboard.Ink_MouseMove(sender, e);
        //}

        //void ImageOverlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    ImageOverlay.ReleaseMouseCapture();
        //    _drawingboard.Ink_MouseLeftButtonUp(sender, e);
        //}

        //void ImageOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ImageOverlay.CaptureMouse();
        //    _drawingboard.Ink_MouseLeftButtonDown(sender, e);
        //}

        #region audio
        public void TryPlayBackgroundMusic()
        {
            //App.GlobalMediaElement.Stop();
            //App.GlobalMediaElement.Source = new Uri("../Audio/mp3/Alegria.mp3", UriKind.RelativeOrAbsolute);
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
            _storyboardColorSelected = (Storyboard)Resources["TappedColorSb"];
            _storyboardSmallCountDownAnimation = (Storyboard)Resources["StoryboardCountDownSmallAnimation"];
            //Storyboard.SetTarget(_storyboardImageFading.Children.ElementAt(0) as DoubleAnimation, ImageMain);
            //Storyboard.SetTarget(_storyboardCountDown.Children.ElementAt(0) as DoubleAnimation, TextBlockCountDown);
            //Storyboard.SetTarget(_storyboardShowPalette.Children.ElementAt(0) as DoubleAnimation, TextBlockCountDown);
        }

        private void StartCountDown()
        {
            TextBlockCountDownBig.Visibility = Visibility.Visible;
            int count = 3;
            TextBlockCountDownBig.Text = count.ToString();
            SoundHelper.PlaySound(App.Current.Sounds[count.ToString()]);
            count--;

            DisablePage();

            //http://stackoverflow.com/questions/4303922/removing-anonymous-event-handler
            EventHandler handler1 = null;
            EventHandler handler2 = null;

            handler1 = (s, e) =>
            {
                if (count == 0)
                {
                    TextBlockCountDownBig.Text = "go!!!";
                    SoundHelper.PlaySound(App.Current.Sounds[count.ToString()]);
                    _storyboardCountDown.Completed -= handler1;
                    _storyboardSubjectImageFading.Begin();
                }
                else
                {
                    if (count == 1)
                    {
                        _storyboardShowPalette.Begin();
                    }
                    TextBlockCountDownBig.Text = count.ToString();
                    SoundHelper.PlaySound(App.Current.Sounds[count.ToString()]);
                    _storyboardCountDown.Begin();
                    count--;
                }
            };

            handler2 = (s1, e1) =>
            {
                _storyboardSubjectImageFading.Completed -= handler2;
                TextBlockCountDownBig.Visibility = Visibility.Collapsed;
                _storyboardCountDown.Stop();

                EnablePage();
                StartTimer();
            };

            _storyboardCountDown.Completed += handler1;
            _storyboardSubjectImageFading.Completed += handler2;

            _storyboardCountDown.Begin();

        }

        void _storyboardCountDown_Completed(object sender, EventArgs e)
        {
        }

        #region timer
        void dt_Tick(object sender, EventArgs e)
        {
            if (_curAvailableTimeValue - 1 > 0)
            {
                _curAvailableTimeValue--;

                Dispatcher.BeginInvoke(() =>
                {
                    TextBlockCountDownSmall.Text = _curAvailableTimeValue.ToString();
                });
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
            BorderCountDownSmall.Visibility = System.Windows.Visibility.Visible;
            _gameInProgress = true;
            _curAvailableTimeValue = TotalTime;
            TextBlockCountDownSmall.Text = _curAvailableTimeValue.ToString();

            _storyboardSmallCountDownAnimationHandler = (s, e) =>
            {
                if (!_gameInProgress) return;

                if (_curAvailableTimeValue == 0)
                {
                    _storyboardSmallCountDownAnimation.Completed -= _storyboardSmallCountDownAnimationHandler;
                    StopTimer();
                    CheckDrawnPicture();
                }
                else
                {
                    _curAvailableTimeValue--;
                    Dispatcher.BeginInvoke(() =>
                    {
                        TextBlockCountDownSmall.Text = _curAvailableTimeValue.ToString();
                    });
                    _storyboardSmallCountDownAnimation.Begin();
                }
            };

            _storyboardSmallCountDownAnimation.Completed -= _storyboardSmallCountDownAnimationHandler;
            _storyboardSmallCountDownAnimation.Completed += _storyboardSmallCountDownAnimationHandler;
            _storyboardSmallCountDownAnimation.Begin();
        }

        private void StopTimer()
        {
            BorderCountDownSmall.Visibility = System.Windows.Visibility.Collapsed;
            _gameInProgress = false;
            _lastAvailableTimeValue = _curAvailableTimeValue;
            _curAvailableTimeValue = 0;
            _storyboardSmallCountDownAnimation.Stop();
            //_dt.Stop();
        }
        #endregion

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitPage();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            PageLeftActions();
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void InitPage()
        {
            TextBlockCountDownSmall.Text = TotalTime.ToString();

            StackPanelTest.Visibility = Visibility.Collapsed;
            GridPainter.Visibility = System.Windows.Visibility.Visible;

            StopTimer();

            _storyboardSubjectImageFading.Stop();
            _storyboardCountDown.Stop();
            _storyboardShowPalette.Stop();

            if (_drawingboard != null) SetEllipseSize(_drawingboard.BrushWidth);

            BorderPalette.Visibility = Visibility.Visible;

            (pc1.RenderTransform as CompositeTransform).TranslateX = ViewModelLocator.PainterPageViewModelStatic.PaletteItemTranslateX;
            (pc2.RenderTransform as CompositeTransform).TranslateX = ViewModelLocator.PainterPageViewModelStatic.PaletteItemTranslateX;
            (pc3.RenderTransform as CompositeTransform).TranslateX = ViewModelLocator.PainterPageViewModelStatic.PaletteItemTranslateX;
            (pc4.RenderTransform as CompositeTransform).TranslateX = ViewModelLocator.PainterPageViewModelStatic.PaletteItemTranslateX;
            (pc5.RenderTransform as CompositeTransform).TranslateX = ViewModelLocator.PainterPageViewModelStatic.PaletteItemTranslateX;

            _drawingboard.SetBoundary();

            ImageMain.Visibility = System.Windows.Visibility.Visible;
            InkPresenterElement.Visibility = System.Windows.Visibility.Visible;
            ImageOverlay.Visibility = System.Windows.Visibility.Visible;

            _drawingboard.Clear();

            var selectedImage = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;

            if (selectedImage != null)
            {
                ImageMain.Source = new BitmapImage(selectedImage.ImageSource);

                _reducedColorsPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(selectedImage.ReducedColorsResourcePath);
                _lineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(selectedImage.LineArtResourcePath);
                _reducedColorsLineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(selectedImage.ReducedColorLineArtResourcePath);
                ImageOverlay.Source = _lineArtPicture;
                ImageOverlay.Opacity = 0.4;
                InitPalette();
                StartCountDown();
            }
        }

        private void InitPalette()
        {


            //var imageColors = ImagesHelper.GetColors(_reducedColorsPicture, true, true);
            //_paletteColors = ImagesHelper.ReduceColors(imageColors, MAX_PALETTE_COLORS, out _ignoredColors);

            _paletteColors = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem.PaletteColors;

            int count = 1;
            foreach (var color in _paletteColors)
            {
                var parentEl = MyVisualTreeHelper.FindChild<Viewbox>(Application.Current.RootVisual, "pc" + count);
                parentEl.Tag = color;
                parentEl.Visibility = System.Windows.Visibility.Visible;
                var childEl = MyVisualTreeHelper.FindChild<System.Windows.Shapes.Path>(parentEl, "pc" + count + "_path");
                if (childEl != null)
                {
                    //childEl.Fill = new SolidColorBrush(color.MainColor);
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
            PageLeftActions();
        }

        private void PageLeftActions()
        {
            if (_resultPopup != null)
            {
                _resultPopup.IsOpen = false;
            }
            StopTimer();
        }

        private void eraseBtn_Click(object sender, RoutedEventArgs e)
        {
            //Change it to 'erase mode'.
            _drawingboard.InkMode = SimzzDev.DrawingBoard.PenMode.Erase;
            SoundHelper.PlaySound(App.Current.Sounds["click"]);
        }

        #region palette


        private void pc1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _drawingboard.InkMode = SimzzDev.DrawingBoard.PenMode.Pen;

            Viewbox senderViewBox = (sender as Viewbox);
            Color selectedColor = (Color)senderViewBox.Tag;

            _storyboardColorSelected.Stop();
            _storyboardColorSelected.SetValue(Storyboard.TargetNameProperty, senderViewBox.Name);
            _storyboardColorSelected.Begin();

            _drawingboard.OutlineColor = _drawingboard.MainColor = selectedColor;

            SoundHelper.PlaySound(App.Current.Sounds["click"]);
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
            //stroke 10 -> width 20
            //stroke 18 -> width 30

            SetEllipseSize(_drawingboard.BrushWidth);
            SoundHelper.PlaySound(App.Current.Sounds["click"]);
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
            userDrawnPicture = new WriteableBitmap((int)_drawingboard.Ink.Width, (int)_drawingboard.Ink.Height);
            userDrawnPicture.Render(_drawingboard.Ink, null);
            userDrawnPicture.Invalidate();

            ImageOverlay.Visibility = System.Windows.Visibility.Collapsed;
            ImageMain.Visibility = System.Windows.Visibility.Collapsed;

            userDrawnPicture.Blit(new Rect(0, 0, userDrawnPicture.PixelWidth, userDrawnPicture.PixelHeight),
                                  _reducedColorsLineArtPicture,
                                  new Rect(0, 0, _reducedColorsLineArtPicture.PixelWidth, _reducedColorsLineArtPicture.PixelHeight),
                                  WriteableBitmapExtensions.BlendMode.Alpha);

            //int tPixels = userDrawnPicture.Pixels.Where(p => p != 0).Count();

            //GridPainter.Visibility = Visibility.Collapsed;
            //StackPanelTest.Visibility = Visibility.Visible;
            //Img1.Source = userDrawnPicture;
            //Img2.Source = _reducedColorsPicture;

            WriteableBitmap resImg = null;

            int accuracyPercentage = ImagesHelper.GetAccuracyPercentage(_reducedColorsPicture,
                                                                        userDrawnPicture,
                                                                        new List<MyColor>(),
                                                                        out resImg);

            //#if DEBUG
            //            accuracyPercentage = 80;
            //#endif
            ShowResultPopup(accuracyPercentage, _lastAvailableTimeValue, resImg);
        }

        private void ShowResultPopup(int percentage, int availTime, WriteableBitmap resImg)
        {
            //popup.VerticalOffset = 250;
            DisablePage();
            InitPopup();
            _resultPopupChild.ResImg = resImg;
            _resultPopupChild.UserPercentage = percentage;
            _resultPopupChild.AvailableTime = availTime;
            _resultPopupChild.PageOrientation = Orientation;
            _resultPopup.IsOpen = true;
        }

        private void InitPopup()
        {
            if (_resultPopup == null)
            {
                _resultPopup = new Popup();
                _resultPopupChild = new ResultPopup(_resultPopup);
                _resultPopupChild.Height = 400;
                _resultPopupChild.Width = 400;
                _resultPopup.VerticalOffset = 200;
                _resultPopup.HorizontalOffset = 30;
                _resultPopupChild.PopupClosedEvent -= exportPopup_PopupClosedEvent;
                _resultPopupChild.PopupClosedEvent += exportPopup_PopupClosedEvent;
                _resultPopupChild.ActionPerformedEvent -= exportPopup_ActionPerformedEvent;
                _resultPopupChild.ActionPerformedEvent += exportPopup_ActionPerformedEvent;
                _resultPopup.Child = _resultPopupChild;
            }
        }

        void exportPopup_ActionPerformedEvent(GameAction action)
        {
            switch (action)
            {
                case GameAction.Menu:
                    NavigationService.Navigate(new Uri("/View/HomePage.xaml", UriKind.RelativeOrAbsolute));
                    break;
                case GameAction.Redo:
                    InitPage();
                    break;
                case GameAction.Ahead:
                    //LIVELLO COMPLETATO
                    var curEl = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;
                    curEl.SetScore(_resultPopupChild.UserPercentage);
                    AppSettings.SaveSettings(true);
                    var nextEl = ViewModelLocator.GroupSelectorViewModelStatic.GetNextItem(curEl);
                    if (nextEl != null)
                    {
                        nextEl.IsLocked = false;
                        ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem = nextEl;
                        InitPage();
                    }
                    else
                    {
                        MessageBox.Show("TODO - tutti i livelli completati!");
                    }
                    break;
                default:
                    break;
            }
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

        private void stopTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            StopTimer();
            CheckDrawnPicture();
            SoundHelper.PlaySound(App.Current.Sounds["click"]);
        }

        private void TextBlockCountDownSmall_Tap(object sender, GestureEventArgs e)
        {

#if DEBUG
             StopTimer();
            int accuracyPercentage = 80;
            ShowResultPopup(accuracyPercentage, _lastAvailableTimeValue, null);
#endif
        }

    }
}