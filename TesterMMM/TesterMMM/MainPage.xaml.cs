using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TesterMMM.Resources;
using System.Windows.Media;
using System.Windows.Ink;

namespace TesterMMM
{
    public partial class MainPage : PhoneApplicationPage
    {

        private TranslateTransform _transform_Move = new TranslateTransform();
        private ScaleTransform _transform_Scale = new ScaleTransform();

        private TransformGroup _transformGroup = new TransformGroup();

        private Brush _stationaryBrush;
        private Brush _transformingBrush = new SolidColorBrush(Colors.Orange);
        private double scaleX;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Combine the moving and resizing tranforms into one TransformGroup.
            // The rectangle's RenderTransform can only contain a single transform or TransformGroup.
            _transformGroup.Children.Add(_transform_Move);
            _transformGroup.Children.Add(_transform_Scale);
            border.RenderTransform = _transformGroup;
            border.RenderTransformOrigin = new Point(0.5, 0.5);

            Touch.FrameReported += Touch_FrameReported;
        }

        private bool _drawing = true;



        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            
            TouchPointCollection tpc = e.GetTouchPoints(border);
            //TouchPointCollection tpc1 = e.GetTouchPoints(MyIP);

            //if (tpc.Count != tpc1.Count)
            //{
            //    _drawing = false;
            //}
    
            int numberOfTouchPoint = tpc.Count;
            Debug.WriteLine("Touch_FrameReported : " + numberOfTouchPoint);

            //http://msdn.microsoft.com/IT-IT/library/system.windows.input.touchframeeventargs(v=vs.110).aspx

            foreach (TouchPoint touchPoint in tpc)
            {
            }

            if (numberOfTouchPoint > 1)
            {
                _drawing = false;
                //AssignBorderEvents();
                //UnassignInkEvents();
            }
            else
            {
                _drawing = true;
                //AssignInkEvents();
                //UnassignBorderEvents();
            }
        }
       
        private void Border_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            if (_drawing)
            {
                return;
            }
            Debug.WriteLine("Border_ManipulationStarted");
            _stationaryBrush = border.Background;
            border.Background = _transformingBrush;
        }

        private void Border_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
          
            if (_drawing)
            {
                return;
            }
            Debug.WriteLine("Border_ManipulationDelta");

            // Move the rectangle.
            _transform_Move.X += e.DeltaManipulation.Translation.X;
            _transform_Move.Y += e.DeltaManipulation.Translation.Y;

            double scaleX = e.DeltaManipulation.Scale.X;
            double scaleY = e.DeltaManipulation.Scale.Y;

            //Debug.WriteLine("> _transform_Scale.ScaleX: " + _transform_Scale.ScaleX);
            //Debug.WriteLine("> _transform_Scale.ScaleY: " + _transform_Scale.ScaleY);

            //if (_transform_Scale.ScaleX < 1 || _transform_Scale.ScaleY < 1)
            //{
            //    e.Handled = true;
            //    return;
            //}

            //if (_transform_Scale.ScaleX > 2 || _transform_Scale.ScaleY > 2)
            //{
            //    e.Handled = true;
            //    return;
            //}

            double scaleFactor = Math.Min(scaleX, scaleY);

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

        private void Border_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
          
            if (_drawing)
            {
                return;
            }
            Debug.WriteLine("Border_ManipulationCompleted");
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
        Stroke NewStroke;

        //A new stroke object named MyStroke is created. MyStroke is added to the StrokeCollection of the InkPresenter named MyIP
        private void MyIP_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (!_drawing)
            {
                return;
            }
            Debug.WriteLine("MyIP_MouseLeftButtonDown");
            MyIP.CaptureMouse();
            StylusPointCollection MyStylusPointCollection = new StylusPointCollection();
            MyStylusPointCollection.Add(e.StylusDevice.GetStylusPoints(MyIP));
            NewStroke = new Stroke(MyStylusPointCollection);
            MyIP.Strokes.Add(NewStroke);
        }

        //StylusPoint objects are collected from the MouseEventArgs and added to MyStroke. 
        private void MyIP_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_drawing)
            {
                return;
            }
            Debug.WriteLine("MyIP_MouseMove");
            if (NewStroke != null)
                NewStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(MyIP));
        }

        //MyStroke is completed
        private void MyIP_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!_drawing)
            {
                return;
            }
            Debug.WriteLine("MyIP_LostMouseCapture");
            NewStroke = null;
        }

        private void UIElement_OnTap(object sender, GestureEventArgs e)
        {
            MyIP.Strokes.Clear();
        }
    }
}