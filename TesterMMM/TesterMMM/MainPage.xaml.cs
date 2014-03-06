using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TesterMMM.Resources;
using System.Windows.Media;

namespace TesterMMM
{
    public partial class MainPage : PhoneApplicationPage
    {

        private TranslateTransform _transform_Move = new TranslateTransform();
        private ScaleTransform _transform_Scale = new ScaleTransform();

        private TransformGroup _transformGroup = new TransformGroup();

        private Brush _stationaryBrush;
        private Brush _transformingBrush = new SolidColorBrush(Colors.Orange);


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Combine the moving and resizing tranforms into one TransformGroup.
            // The rectangle's RenderTransform can only contain a single transform or TransformGroup.
            //_transformGroup.Children.Add(_transform_Move);
            //_transformGroup.Children.Add(_transform_Scale);
            //border.RenderTransform = _transformGroup;
            //border.RenderTransformOrigin = new Point(0.5, 0.5);
            
        }

        private void Border_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _stationaryBrush =border.Background;
            border.Background = _transformingBrush;
        }

        private void Border_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            // Move the rectangle.
            _transform_Move.X += e.DeltaManipulation.Translation.X;
            _transform_Move.Y += e.DeltaManipulation.Translation.Y;

            double scaleX = e.DeltaManipulation.Scale.X;
            double scaleY = e.DeltaManipulation.Scale.Y;

            double scaleFactor = Math.Min(scaleX, scaleY);

            if (scaleFactor > 0)
            {
                _transform_Scale.ScaleX *= scaleFactor;
                _transform_Scale.ScaleY *= scaleFactor;
            }
        }

        private void Border_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            // Restore the original color.
            border.Background = _stationaryBrush;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();
        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);
        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}