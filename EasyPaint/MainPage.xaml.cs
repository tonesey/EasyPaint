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
using EasyPaint.Tests;
using System.Windows.Media.Imaging;
using EasyPaint.Helpers;
using System.IO.IsolatedStorage;
using System.Windows.Resources;
using System.IO;

namespace DrawingBoard_Sample
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        SimzzDev.DrawingBoard _myBoard;
        private const string tmpFName = "tmp.png";
        WriteableBitmap _origPicture = null;


        public MainPage()
        {
            InitializeComponent();
            //Tester.CheckImagesTester();
            _myBoard = new SimzzDev.DrawingBoard(ink);
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            ImagesHelper.WriteContentImageToIsoStore("Assets/Packages/MickeyMouse/disegno-faccia-di-minnie-colorato-300x300.png", tmpFName);

            _origPicture = new WriteableBitmap(ImagesHelper.GetBitmapImageFromIsoStore(tmpFName));
            mainImg.Source = _origPicture;

            ink.Height = _origPicture.PixelHeight;
            ink.Width = _origPicture.PixelWidth;

            List<Color> c1 = ImagesHelper.GetColors(_origPicture);
            int count = 1;
            foreach (var color in c1)
            {
                var btn = MyVisualTreeHelper.FindChild<Button>(Application.Current.RootVisual, "pc" + count);
                if (btn != null)
                {
                    btn.Background = new SolidColorBrush(color);
                }
                count++;
            }
            for (int i = count; i <= 6; i++ )
            {
                var btn = MyVisualTreeHelper.FindChild<Button>(Application.Current.RootVisual, "pc" + count);
                btn.Visibility = Visibility.Collapsed;
            }
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
            _myBoard.InkMode = SimzzDev.DrawingBoard.PenMode.erase;
        }

        private void doneBtn_Click(object sender, RoutedEventArgs e)
        {
            //WriteableBitmap drawnPicture1 = new WriteableBitmap(myBoard.Ink, null);

            WriteableBitmap drawnPicture = new WriteableBitmap(ImagesHelper.GetImageFromInkPresenter(_myBoard.Ink));
            int test = ImagesHelper.GetNotBlankPixels(drawnPicture);

            //int diffPixels1 = ImagesHelper.GetNumberOfDifferentPixels(drawnPicture1, drawnPicture);

            //content
            //non riesco a caricarlo.... da sempre null
            //var bmp = new BitmapImage(new Uri("Assets/Packages/Pimpa/01.PNG", UriKind.Relative));
            //WriteableBitmap origPicture1 = new WriteableBitmap(bmp);

            //List<Color> c2 = ImagesHelper.GetColors(drawnPicture);

            int diffPixels = ImagesHelper.GetNumberOfDifferentPixels(_origPicture, drawnPicture);
            int diffPixelsPercentage = ImagesHelper.GetPercentageOfDifferentPixels(_origPicture, drawnPicture);
           // textPrecision.Text = diffPixelsPercentage + "%";

        }

        private void pc1_Click(object sender, RoutedEventArgs e)
        {
            _myBoard.InkMode = SimzzDev.DrawingBoard.PenMode.pen;
            Color selectedColor = ((sender as Button).Background as SolidColorBrush).Color;

            _myBoard.MainColor = selectedColor;
            _myBoard.OutlineColor = selectedColor;
        }

        private void pensizeBtn_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }

  

    }
}