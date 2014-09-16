﻿using System;
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
using EasyPaint.Data;
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
using Microsoft.Phone.Tasks;
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
       // const int TotalTime = 30;

        // Constructor
        SimzzDev.DrawingBoard _drawingboard = null;
        bool _drawing = true;

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

        private TranslateTransform _transform_Move = new TranslateTransform();
        private ScaleTransform _transform_Scale = new ScaleTransform();
        private TransformGroup _transformGroup = new TransformGroup();
        private Brush _stationaryBrush;
        private Brush _transformingBrush = new SolidColorBrush(Colors.Orange);

        public PainterPage()
        {
            InitializeComponent();

            //ItemName1.Visibility = Visibility.Visible;
            ScoreStackPanel.Visibility = Visibility.Visible;
            //ItemName2.Visibility = Visibility.Visible;
            BorderPalette.Visibility = Visibility.Collapsed;
            BorderTools.Visibility = Visibility.Collapsed;


            LocalizeUI();

            Loaded += PainterPage_Loaded;
            Unloaded += PainterPage_Unloaded;
        }

        private void LocalizeUI()
        {
            TextBlockScoreDescr.Text = LocalizedResources.PainterPageCurrentScore;
            TextBlockRecordDescr.Text = LocalizedResources.PainterPageRecord;
        }

        private void BuildTransformGroup()
        {
            ClearTransformGroup();
            _transformGroup.Children.Add(_transform_Move);
            _transformGroup.Children.Add(_transform_Scale);
        }

        private void ClearTransformGroup()
        {
            if (_transformGroup.Children.Any())
            {
                _transformGroup.Children.Clear();
            }
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
            _curAvailableTimeValue = GetAvailableTime();


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
            _drawing = true;
            _drawingboard.IsEnabled = true;

            BuildTransformGroup();
            GridMainContainer.RenderTransform = _transformGroup;
            GridMainContainer.RenderTransformOrigin = new Point(0.5, 0.5);
            _transform_Move.X = 0;
            _transform_Move.Y = 0;
            _transform_Scale.ScaleX = 1;
            _transform_Scale.ScaleY = 1;


            stopTimeBtn.Visibility = System.Windows.Visibility.Visible;

            //#if DEBUG
            //            stopTimeBtn.Visibility = System.Windows.Visibility.Visible;
            //#else
            //            switch (App.Current.GameMode)
            //            {
            //                case GameMode.Arcade:
            //                    stopTimeBtn.Visibility = System.Windows.Visibility.Collapsed;
            //                    break;
            //                case GameMode.Gallery:
            //                    stopTimeBtn.Visibility = System.Windows.Visibility.Visible;
            //                    break;
            //            }
            //#endif

            BorderPalette.Visibility = Visibility.Collapsed;
            TextBlockCountDownSmall.Text = GetAvailableTime().ToString();
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
            ImageThumb.Visibility = System.Windows.Visibility.Visible;
            InkPresenterElement.Visibility = System.Windows.Visibility.Visible;
            ImageOverlay.Visibility = System.Windows.Visibility.Visible;

            _drawingboard.Clear();

            ItemViewModel currentItem = GetUserSelectedItem();

            if (currentItem != null)
            {
                ImageMain.Source = new BitmapImage(currentItem.ImageSource);
                ImageThumb.Source = new BitmapImage(currentItem.ImageSource);

                //ItemName1.Visibility = Visibility.Visible;

                switch (App.Current.GameMode)
                {
                    case GameMode.Gallery:
                        ScoreStackPanel.Visibility = Visibility.Collapsed;
                        break;
                    case GameMode.Arcade:
                        ScoreStackPanel.Visibility = Visibility.Visible;
                        break;
                }
                UpdateScores();


                // ItemName1.Text = currentItem.LocalizedName;
                //ItemName2.Visibility = Visibility.Visible;
                //ItemName2.Text = currentItem.LatinName;

                //if (currentItem.IsLocked)
                //{
                //    currentItem.IsLocked = false;
                //    AppSettings.SaveSettings(true);
                //}

                _reducedColorsPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.ReducedColorsResourcePath);
                _reducedColorsLineArtPicture = BitmapFactory.New(ViewModelLocator.PainterPageViewModelStatic.DrawingboardWidth, ViewModelLocator.PainterPageViewModelStatic.DrawingboardHeigth).FromResource(currentItem.ReducedColorLineArtResourcePath);

                ImageOverlay.Source = _reducedColorsLineArtPicture;
                ImageOverlay.Opacity = 0.5;

                BorderPalette.Visibility = Visibility.Visible;
                BorderTools.Visibility = Visibility.Visible;
                InitPalette();

                StartCountDown();
            }
        }

        private int GetAvailableTime()
        {
            int value = 0;
            switch (AppSettings.GameLevel)
            {
                case GameLevel.Easy:
                    value = 60;
                    break;
                case GameLevel.Medium:
                    value = 40;
                    break;
                case GameLevel.Hard:
                    value = 30;
                    break;
                default:
                    break;
            }
            return value;
        }

        private void UpdateScores()
        {
            TextBlockScoreValue.Text = AppDataManager.GetInstance().GetTotalPoints().ToString();
            //TextBlockRecordValue.Text = AppSettings.RecordScoreValue_HARD.ToString();

            switch (AppSettings.GameLevel)
            {
                case GameLevel.Easy:
                    TextBlockRecordValue.Text = AppSettings.RecordScoreValue_EASY.ToString();
                    break;
                case GameLevel.Medium:
                    TextBlockRecordValue.Text = AppSettings.RecordScoreValue_MEDIUM.ToString();
                    break;
                case GameLevel.Hard:
                    TextBlockRecordValue.Text = AppSettings.RecordScoreValue_HARD.ToString();
                    break;
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
            ImageThumb.Visibility = System.Windows.Visibility.Collapsed;

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
            _popupChild.InputUserPercentage = percentage;
            _popupChild.InputSavedTime = availTime;
            _popupChild.PageOrientation = Orientation;
            _popup.IsOpen = true;
        }

        private void InitPopup()
        {
            if (_popup == null)
            {
                _popup = new Popup();
                _popupChild = new ResultPopup(_popup);
                _popupChild.Height = 460;
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
                            FlurryWP8SDK.Api.LogEvent("new element selected: " + curEl.LocalizedDescr);
                            curEl.SetScore(_popupChild.GetItemTotalScore());

                            var nextEl = ViewModelLocator.GroupSelectorViewModelStatic.GetNextItem(curEl);
                            int unlockedItemsIngroup = ViewModelLocator.ItemSelectorViewModelStatic.GetUnlockedItemsCount();

                            if (App.Current.IsTrial && unlockedItemsIngroup > 2)
                            {
                                MessagingHelper.GetInstance().CurrentDispatcher.BeginInvoke(() => MyMsgbox.Show(this,
                                                                                                                MsgboxMode.YesNo,
                                                                                                                LocalizedResources.Trial_ArcadeGameLimitation,
                                                                                                                result =>
                                                                                                                {
                                                                                                                    switch (result)
                                                                                                                    {
                                                                                                                        case MsgboxResponse.Yes:
                                                                                                                            var marketplaceDetailTask = new MarketplaceDetailTask { ContentIdentifier = AppSettings.AppGuid };
                                                                                                                            marketplaceDetailTask.Show();
                                                                                                                            break;
                                                                                                                        case MsgboxResponse.No:
                                                                                                                            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GroupSeletor);
                                                                                                                            break;
                                                                                                                    }
                                                                                                                }));
                                return; //need to restart app
                            }

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
                                //switch continente
                                MyMsgbox.Show(this,
                                              MsgboxMode.Ok,
                                              string.Format(LocalizedResources.GroupCompleted, curEl.ParentGroupName, nextEl.ParentGroupName),
                                    async result =>
                                    {
                                        UnlockAndRestartWithItem(nextEl);
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

        private void GridMainContainer_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (_drawing) return;
            //_stationaryBrush = GridMainContainer.Background;
            //GridMainContainer.Background = _transformingBrush;
        }

        private void GridMainContainer_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_drawing) return;
            //GridMainContainer.Background = _stationaryBrush;
        }

        private void GridMainContainer_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (_drawing) return;

            _transform_Move.X += e.DeltaManipulation.Translation.X;
            _transform_Move.Y += e.DeltaManipulation.Translation.Y;

            double scaleFactor = (e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2;
            if (scaleFactor > 0)
            {
                var newScaleX = _transform_Scale.ScaleX * scaleFactor;
                if (newScaleX > 1 && newScaleX < 2)
                {
                    _transform_Scale.ScaleX *= scaleFactor;
                    _transform_Scale.ScaleY *= scaleFactor;
                }
            }
        }

        private void toggleModeBtn_Click(object sender, RoutedEventArgs e)
        {
            _drawing = !_drawing;
            _drawingboard.IsEnabled = _drawing;

            //TODO portare in un converter
            if (_drawing)
            {
                ZoomImage.Source = new BitmapImage(new Uri("../Assets/buttons/PanZoom1.png", UriKind.RelativeOrAbsolute));
            }
            else
            {
                ZoomImage.Source = new BitmapImage(new Uri("../Assets/buttons/lock.png", UriKind.RelativeOrAbsolute));
            }

        }

        private void AdControl_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {

        }

        private void AdControl_AdRefreshed(object sender, EventArgs e)
        {

        }

    }
}