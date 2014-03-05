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
        private ScaleTransform _transform_Resize = new ScaleTransform();
        private TransformGroup _transformGroup = new TransformGroup();

        private Brush _stationaryBrush;
        private Brush _transformingBrush = new SolidColorBrush(Colors.Orange);


        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Combine the moving and resizing tranforms into one TransformGroup.
            // The rectangle's RenderTransform can only contain a single transform or TransformGroup.
            _transformGroup.Children.Add(_transform_Move);
            _transformGroup.Children.Add(_transform_Resize);
            _border.RenderTransform = _transformGroup;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Border_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _stationaryBrush = _border.Background;
            _border.Background = _transformingBrush;
        }

        private void Border_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            // Move the rectangle.
            _transform_Move.X += e.DeltaManipulation.Translation.X;
            _transform_Move.Y += e.DeltaManipulation.Translation.Y;

            // Resize the rectangle.
            if (e.DeltaManipulation.Scale.X > 0 && e.DeltaManipulation.Scale.Y > 0)
            {
                // Scale the rectangle.
                _transform_Resize.ScaleX *= e.DeltaManipulation.Scale.X;
                _transform_Resize.ScaleY *= e.DeltaManipulation.Scale.Y;
            }
        }

        private void Border_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            // Restore the original color.
            _border.Background = _stationaryBrush;
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