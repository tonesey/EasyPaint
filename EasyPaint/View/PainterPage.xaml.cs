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

        Storyboard _storyboardImageFadingAndStartGame;
        Storyboard _storyboardBigCountdown;
        Storyboard _storyboardShowPalette;
        Storyboard _storyboardColorSelected;
        Storyboard _storyboardSmallCountDownAnimation;
        EventHandler _storyboardSmallCountDownAnimationHandler;

        EventHandler _storyboardBigCountdownHandler = null;
        EventHandler _storyboardImageFadingAndStartGameHandler = null;

        Popup _resultPopup = null;
        ResultPopup _resultPopupChild = null;
        List<Color> _paletteColors = new List<Color>();
 
        public PainterPage()
        {
            InitializeComponent();
            Loaded += PainterPage_Loaded;
            Unloaded += PainterPage_Unloaded;
        }

        void PainterPage_Unloaded(object sender, RoutedEventArgs e)
        {
            PageLeftActions();
        }

        void PainterPage_Loaded(object sender, RoutedEventArgs e)
        {
            _drawingboard = new SimzzDev.DrawingBoard(InkPresenterElement);
            InitAnimations();
            AssignEventHandlers();
            InitPopup();

            InitPage();
        }

        private void AssignEventHandlers()
        {
            UnassignEventHandlers();
        }

        private void UnassignEventHandlers()
        {
        }

        private void ShowPalette()
        {
            _storyboardShowPalette.Begin();
        }

        private void InitAnimations()
        {
            _storyboardImageFadingAndStartGame = (Storyboard)Resources["StoryboardSubjectFadeout"];
            _storyboardBigCountdown = (Storyboard)Resources["StoryboardCountdown"];
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


            _storyboardBigCountdownHandler = (s, e) =>
            {
                if (count == 0)
                {
                    TextBlockCountDownBig.Text = "go!!!";
                    (Application.Current as App).PlayBackgroundMusic(App.TrackType.Paint);
                    SoundHelper.PlaySound(App.Current.Sounds[count.ToString()]);
                    UnassignBigCountdownEventhandler();
                    _storyboardImageFadingAndStartGame.Begin();
                }
                else
                {
                    if (count == 1)
                    {
                        _storyboardShowPalette.Begin();
                    }
                    TextBlockCountDownBig.Text = count.ToString();
                    SoundHelper.PlaySound(App.Current.Sounds[count.ToString()]);
                    _storyboardBigCountdown.Begin();
                    count--;
                }
            };

            _storyboardImageFadingAndStartGameHandler = (s1, e1) =>
            {
                UnassignImageFadingAndStartGameEventHandler();
                TextBlockCountDownBig.Visibility = Visibility.Collapsed;
                _storyboardBigCountdown.Stop();
                EnablePage();
                StartTimer();
            };

            //_storyboardBigCountdown.Completed += _storyboardBigCountdownHandler;
            //_storyboardImageFadingAndStartGame.Completed += _storyboardImageFadingAndStartGameHandler;

            AssignBigCountdownEventhandler();
            AssignImageFadingAndStartGameEventHandler();

            _storyboardBigCountdown.Begin();

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
        }

        private void StartTimer()
        {
            BorderCountDownSmall.Visibility = System.Windows.Visibility.Visible;
            _gameInProgress = true;
            _curAvailableTimeValue = TotalTime;
            TextBlockCountDownSmall.Text = _curAvailableTimeValue.ToString();

            _storyboardSmallCountDownAnimationHandler = (s, e) =>
            {
                if (!_gameInProgress) {
                    UnassignSmallCountdownEventHandler();
                    return;
                }

                if (_curAvailableTimeValue == 0)
                {
                    UnassignSmallCountdownEventHandler();
                    StopTimer();
                    CheckDrawnPicture();
                }
                else
                {
                    _curAvailableTimeValue--;

                    if (_curAvailableTimeValue <= 10) {
                        SoundHelper.PlaySound(App.Current.Sounds["lowtime"]);
                    }
                    
                    Dispatcher.BeginInvoke(() =>
                    {
                        TextBlockCountDownSmall.Text = _curAvailableTimeValue.ToString();
                    });
                    _storyboardSmallCountDownAnimation.Begin();
                }
            };

            UnassignSmallCountdownEventHandler();
            AssignSmallCountdownEventHandler();

            _storyboardSmallCountDownAnimation.Begin();
        }

        #region small count
        private void UnassignSmallCountdownEventHandler()
        {
            _storyboardSmallCountDownAnimation.Completed -= _storyboardSmallCountDownAnimationHandler;
        }

        private void AssignSmallCountdownEventHandler()
        {
            _storyboardSmallCountDownAnimation.Completed += _storyboardSmallCountDownAnimationHandler;
        }
        #endregion

        #region big count
        private void AssignBigCountdownEventhandler()
        {
            _storyboardBigCountdown.Completed += _storyboardBigCountdownHandler;
        }

        private void UnassignBigCountdownEventhandler()
        {
            _storyboardBigCountdown.Completed -= _storyboardBigCountdownHandler;
        }
        #endregion

        #region image fading + start game
        private void AssignImageFadingAndStartGameEventHandler()
        {
            _storyboardImageFadingAndStartGame.Completed += _storyboardImageFadingAndStartGameHandler;
        }

        private void UnassignImageFadingAndStartGameEventHandler()
        {
            _storyboardImageFadingAndStartGame.Completed -= _storyboardImageFadingAndStartGameHandler;
        }
        #endregion

        private void StopTimer()
        {
            _gameInProgress = false;
            BorderCountDownSmall.Visibility = System.Windows.Visibility.Collapsed;
            _lastAvailableTimeValue = _curAvailableTimeValue;
            _curAvailableTimeValue = 0;

            UnassignSmallCountdownEventHandler();
            _storyboardSmallCountDownAnimation.Stop();

            UnassignBigCountdownEventhandler(); ;
            _storyboardBigCountdown.Stop();

            UnassignImageFadingAndStartGameEventHandler();
            _storyboardImageFadingAndStartGame.Stop();
        }

        #endregion

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
            (Application.Current as App).PlayBackgroundMusic(App.TrackType.StandardBackground);

            BorderPalette.Visibility = Visibility.Collapsed;
            TextBlockCountDownSmall.Text = TotalTime.ToString();
            StackPanelTest.Visibility = Visibility.Collapsed;
            GridPainter.Visibility = System.Windows.Visibility.Visible;

            StopTimer();

            _storyboardImageFadingAndStartGame.Stop();
            _storyboardBigCountdown.Stop();
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

            var currentItem = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;

            if (currentItem != null)
            {
                ImageMain.Source = new BitmapImage(currentItem.ImageSource);

                if (currentItem.IsLocked)
                {
                    currentItem.IsLocked = false;
                    AppSettings.SaveSettings(true);
                }

                _reducedColorsPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.ReducedColorsResourcePath);
                _lineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.LineArtResourcePath);
                _reducedColorsLineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.ReducedColorLineArtResourcePath);
                
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
                    childEl.Fill = new SolidColorBrush(color);
                    if (count == 1) {
                        //sets drawingboard color
                        _drawingboard.OutlineColor = _drawingboard.MainColor = color;
                    }
                }
                count++;
            }
     
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
                _resultPopupChild.Close();
                _resultPopup.IsOpen = false;
                _resultPopup = null;
            }
            StopTimer();
            (Application.Current as App).PlayBackgroundMusic(App.TrackType.StandardBackground);
         
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