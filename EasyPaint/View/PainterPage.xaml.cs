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
using Microsoft.Practices.ServiceLocation;
using Wp8Shared.UserControls;
using System.Threading.Tasks;

namespace EasyPaint.View
{
    public partial class PainterPage : PhoneApplicationPage
    {
        const int TotalTime = 30;

        // Constructor
        SimzzDev.DrawingBoard _drawingboard = null;

        //WriteableBitmap _lineArtPicture = null;
        WriteableBitmap _reducedColorsPicture = null;
        WriteableBitmap _reducedColorsLineArtPicture = null;

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

        Popup _popup = null;
        ResultPopup _popupChild = null;
        List<Color> _paletteColors = new List<Color>();

        public PainterPage()
        {
            InitializeComponent();

            ItemName1.Visibility = Visibility.Visible;
            ItemName2.Visibility = Visibility.Visible;

            Loaded += PainterPage_Loaded;
            Unloaded += PainterPage_Unloaded;
        }

        void PainterPage_Unloaded(object sender, RoutedEventArgs e)
        {
            PageLeftActions();
        }

        void PainterPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppViewModel.CurrentPage = this;
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

                switch (App.Current.GameMode)
                {
                    case GameMode.Arcade:
                        StartTimer();
                        break;
                    case GameMode.Gallery:
                        break;
                }
            };

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
                if (!_gameInProgress)
                {
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

                    if (_curAvailableTimeValue <= 10)
                    {
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

            switch (App.Current.GameMode)
            {
                case GameMode.Arcade:
                    stopTimeBtn.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case GameMode.Gallery:
                    stopTimeBtn.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

            BorderPalette.Visibility = Visibility.Collapsed;
            TextBlockCountDownSmall.Text = TotalTime.ToString();
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

            ItemViewModel currentItem = GetUserSelectedItem();

            if (currentItem != null)
            {
                ImageMain.Source = new BitmapImage(currentItem.ImageSource);

                ItemName1.Visibility = Visibility.Visible;
                ItemName2.Visibility = Visibility.Visible;

                ItemName1.Text = currentItem.LocalizedName;
                ItemName2.Text = currentItem.LatinName;

                //if (currentItem.IsLocked)
                //{
                //    currentItem.IsLocked = false;
                //    AppSettings.SaveSettings(true);
                //}

                _reducedColorsPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.ReducedColorsResourcePath);
                _reducedColorsLineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.ReducedColorLineArtResourcePath);

                ImageOverlay.Source = _reducedColorsLineArtPicture;
                ImageOverlay.Opacity = 0.5;
                InitPalette();

                StartCountDown();
            }
        }

        private static ItemViewModel GetUserSelectedItem()
        {
            ItemViewModel currentItem = null;
            switch (App.Current.GameMode)
            {
                case GameMode.Gallery:
                    currentItem = ViewModelLocator.GalleryViewModelStatic.SelectedItem;
                    break;
                case GameMode.Arcade:
                    currentItem = ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem;
                    break;
            }
            return currentItem;
        }

        private void InitPalette()
        {
            _paletteColors = GetUserSelectedItem().PaletteColors;
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
                    if (count == 1)
                    {
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
            ClosePopup();
            StopTimer();
            (Application.Current as App).PlayBackgroundMusic(App.TrackType.StandardBackground);
        }

        private void ClosePopup()
        {
            if (_popup != null)
            {
                _popupChild.Close();
                _popup.IsOpen = false;
                _popup = null;
            }
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

            SetEllipseSize(_drawingboard.BrushWidth);
            SoundHelper.PlaySound(App.Current.Sounds["click"]);
        }

        private void SetEllipseSize(int strokeWidth)
        {
            //stroke 2 -> width 10
            //stroke 10 -> width 20
            //stroke 18 -> width 30
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

            WriteableBitmap resImg = null;

            var colorsToIgnore = ImagesHelper.GetColors(_reducedColorsPicture, false, false);
            colorsToIgnore = colorsToIgnore.Except(GetUserSelectedItem().PaletteColors).ToList();
            colorsToIgnore = colorsToIgnore.Except(ImagesHelper.GetColors(_reducedColorsLineArtPicture, false, false)).ToList();
            
          //  var colorsToIgnore = new List<Color>();
            int accuracyPercentage = ImagesHelper.GetAccuracyPercentage(_reducedColorsPicture,
                                                                        userDrawnPicture,
                                                                        colorsToIgnore,
                                                                        out resImg);
            ShowResultPopup(accuracyPercentage, _lastAvailableTimeValue, resImg);
        }

        private void ShowResultPopup(int percentage, int availTime, WriteableBitmap resImg)
        {
            DisablePage();
            InitPopup();
            _popupChild.ResImg = resImg;
            _popupChild.UserPercentage = percentage;
            _popupChild.AvailableTime = availTime;
            _popupChild.PageOrientation = Orientation;
            _popup.IsOpen = true;
        }

        private void InitPopup()
        {
            if (_popup == null)
            {
                _popup = new Popup();
                _popupChild = new ResultPopup(_popup);
                _popupChild.Height = 400;
                _popupChild.Width = 400;
                _popupChild.PopupClosedEvent -= exportPopup_PopupClosedEvent;
                _popupChild.PopupClosedEvent += exportPopup_PopupClosedEvent;
                _popupChild.ActionPerformedEvent -= exportPopup_ActionPerformedEvent;
                _popupChild.ActionPerformedEvent += exportPopup_ActionPerformedEvent;
                _popup.Child = _popupChild;
                _popup.VerticalOffset = 200;
                _popup.HorizontalOffset = 30;
            }
        }

        void exportPopup_ActionPerformedEvent(GameAction action)
        {

            ClosePopup();

            switch (action)
            {
                case GameAction.Menu:
                    ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
                    break;
                case GameAction.Redo:
                    InitPage();
                    break;
                case GameAction.Ahead:

                    switch (App.Current.GameMode)
                    {
                        case GameMode.Arcade:
                            //livello completato
                            var curEl = GetUserSelectedItem();
                            curEl.SetScore(_popupChild.UserPercentage);
                            var nextEl = ViewModelLocator.GroupSelectorViewModelStatic.GetNextItem(curEl);
                            if (nextEl.ParentGroupId == curEl.ParentGroupId)
                            {
                                if (nextEl != null)
                                {
                                    UnlockAndRestartWithItem(nextEl);
                                }
                                else
                                {
                                    //tutti i livelli completati
                                    MyMsgbox.Show(this, MsgboxMode.Ok, LocalizedResources.MessageAllLevelsCompleted);
                                }
                            }
                            else
                            {
                                //res = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(AppSettings.IapCompleteGameProductId, false);

                                //switch continente
                                MyMsgbox.Show(this,
                                              MsgboxMode.Ok,
                                              string.Format(LocalizedResources.GroupCompleted, curEl.ParentGroupName, nextEl.ParentGroupName),
                                    async result =>
                                    {
                                        //verifica se è presente la licenza, nel caso sia richiesta
                                        if (AppSettings.ProductLicensed || !nextEl.ParentGroupRequiresLicense)
                                        {
                                            UnlockAndRestartWithItem(nextEl);
                                        }
                                        else
                                        {
                                            MyMsgbox.Show(this, MsgboxMode.YesNo,
                                                string.Format(LocalizedResources.NeedPaidAppQuestion, nextEl.ParentGroupName),
                                                async result1 =>
                                            {
                                                switch (result1)
                                                {
                                                    case MsgboxResponse.Yes:

                                                        string res = null;
                                                        try
                                                        {
#if DEBUG
                                                            res = await MockIAPLib.CurrentApp.RequestProductPurchaseAsync(AppSettings.IapCompleteGameProductId, false);
#else
                                                            res = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(AppSettings.IapCompleteGameProductId, true);
#endif
                                                        }
                                                        catch (Exception)
                                                        {
                                                            //capita anche se l'utente fa "Annulla" sull'acquisto
                                                            res = null;
                                                        }

                                                        if (res == null)
                                                        {
                                                            //acquisto KO
                                                            InitPage();
                                                            return; //se non si vuole acquistare resta sullo stesso item
                                                        }
                                                        else
                                                        {
                                                            //acquisto OK
                                                            AppSettings.ProductLicensed = true;
                                                            //si prosegue normalmente nel flusso
                                                            UnlockAndRestartWithItem(nextEl);
                                                        }
                                                        break;
                                                    case MsgboxResponse.No:
                                                        InitPage();
                                                        return; //se non si vuole acquistare resta sullo stesso item
                                                }
                                            });
                                        }
                                    });
                            }

                            break;
                        case GameMode.Gallery:
                            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Gallery);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        private void UnlockAndRestartWithItem(ItemViewModel nextEl)
        {
            nextEl.IsLocked = false;
            ViewModelLocator.ItemSelectorViewModelStatic.SelectedItem = nextEl;
            InitPage();
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

        private void ItemName1_Tap(object sender, GestureEventArgs e)
        {
#if DEBUG
            stopTimeBtn_Click(null, null);
#endif
        }

    }
}