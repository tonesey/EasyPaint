﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;
using Wp8Shared.Helpers;
using EasyPaint.Settings;

namespace EasyPaint.Controls
{
    public partial class FeedbackOverlay : UserControl
    {
        // Use this from XAML to control whether animation is on or off
        #region EnableAnimation Dependency Property

        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(FeedbackOverlay), new PropertyMetadata(true, null));

        public static void SetEnableAnimation(FeedbackOverlay element, bool value)
        {
            element.SetValue(EnableAnimationProperty, value);
        }

        public static bool GetEnableAnimation(FeedbackOverlay element)
        {
            return (bool)element.GetValue(EnableAnimationProperty);
        }

        #endregion

        // Use this for MVVM binding IsVisible
        #region IsVisible Dependency Property

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(FeedbackOverlay), new PropertyMetadata(false, null));

        public static void SetIsVisible(FeedbackOverlay element, bool value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(FeedbackOverlay element)
        {
            return (bool)element.GetValue(IsVisibleProperty);
        }

        #endregion

        // Use this for MVVM binding IsNotVisible
        #region IsNotVisible Dependency Property

        public static readonly DependencyProperty IsNotVisibleProperty =
            DependencyProperty.Register("IsNotVisible", typeof(bool), typeof(FeedbackOverlay), new PropertyMetadata(true, null));

        public static void SetIsNotVisible(FeedbackOverlay element, bool value)
        {
            element.SetValue(IsNotVisibleProperty, value);
        }

        public static bool GetIsNotVisible(FeedbackOverlay element)
        {
            return (bool)element.GetValue(IsNotVisibleProperty);
        }

        #endregion

        // Use this for detecting visibility change on code
        public event EventHandler VisibilityChanged = null;

        private PhoneApplicationFrame _rootFrame = null;

        public string Title
        {
            set
            {
                if (this.title.Text != value)
                {
                    this.title.Text = value;
                }
            }
        }

        public string Message
        {
            set
            {
                if (this.message.Text != value)
                {
                    this.message.Text = value;
                }
            }
        }

        public string NoText
        {
            set
            {
                if ((string)this.noButton.Content != value)
                {
                    this.noButton.Content = value;
                }
            }
        }

        public string YesText
        {
            set
            {
                if ((string)this.yesButton.Content != value)
                {
                    this.yesButton.Content = value;
                }
            }
        }

        public FeedbackOverlay()
        {
            InitializeComponent();

            this.yesButton.Click += yesButton_Click;
            this.noButton.Click += noButton_Click;
            this.Loaded += FeedbackOverlay_Loaded;
            this.hideContent.Completed += hideContent_Completed;
        }

        private void FeedbackOverlay_Loaded(object sender, RoutedEventArgs e)
        {
            this.AttachBackKeyPressed();

            if (FeedbackOverlay.GetEnableAnimation(this))
            {
                this.LayoutRoot.Opacity = 0;
                this.xProjection.RotationX = 90;
            }

            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                this.SetVisibility(true);
                this.SetupFirstMessage();

                if (FeedbackOverlay.GetEnableAnimation(this))
                    this.showContent.Begin();
            }
            else if (FeedbackHelper.Default.State == FeedbackState.SecondReview)
            {
                this.SetVisibility(true);
                this.SetupSecondMessage();

                if (FeedbackOverlay.GetEnableAnimation(this))
                    this.showContent.Begin();
            }
            else
            {
                this.SetVisibility(false);
            }
        }

        private void AttachBackKeyPressed()
        {
            // Detect back pressed
            if (this._rootFrame == null)
            {
                this._rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
                this._rootFrame.BackKeyPress += FeedbackOverlay_BackKeyPress;
            }
        }

        private void FeedbackOverlay_BackKeyPress(object sender, CancelEventArgs e)
        {
            // If back is pressed whilst open, close and cancel back to stop app exiting
            if (this.Visibility == System.Windows.Visibility.Visible)
            {
                this.OnNoClick();
                e.Cancel = true;
            }
        }

        private void SetupFirstMessage()
        {
            this.Title = string.Format(LocalizedResources.RatingTitle, AppSettings.AppName.ToUpper());
            this.Message = LocalizedResources.RatingMessage1;
            this.YesText = LocalizedResources.RatingYes;
            this.NoText = LocalizedResources.RatingNo;
        }

        private void SetupSecondMessage()
        {
            this.Title = string.Format(LocalizedResources.RatingTitle, AppSettings.AppName.ToUpper());
            this.Message = LocalizedResources.RatingMessage2;
            this.YesText = LocalizedResources.RatingYes;
            this.NoText = LocalizedResources.RatingNo;
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnNoClick();
        }

        private void OnNoClick()
        {
            if (FeedbackOverlay.GetEnableAnimation(this))
                this.hideContent.Begin();
        }

        private void hideContent_Completed(object sender, EventArgs e)
        {
            this.SetVisibility(false);
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetVisibility(false);

            if (FeedbackHelper.Default.State == FeedbackState.FirstReview)
            {
                this.Review();
            }
            else if (FeedbackHelper.Default.State == FeedbackState.SecondReview)
            {
                this.Review();
            }
        }

        private void Review()
        {
            FeedbackHelper.Default.Reviewed();
            var marketplace = new MarketplaceReviewTask();
            marketplace.Show();
        }

        private void SetVisibility(bool visible)
        {
            if (visible)
            {
                FeedbackOverlay.SetIsVisible(this, true);
                FeedbackOverlay.SetIsNotVisible(this, false);
                this.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                FeedbackOverlay.SetIsVisible(this, false);
                FeedbackOverlay.SetIsNotVisible(this, true);
                this.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.OnVisibilityChanged();
        }

        private void OnVisibilityChanged()
        {
            if (this.VisibilityChanged != null)
                this.VisibilityChanged(this, new EventArgs());
        }
    }
}
