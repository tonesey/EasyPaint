using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using EasyPaint.Tester.Resources;
using System.Windows.Media;
using EasyPaint.Helpers;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyPaint.Tester
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            var res = string.Format("Assets/3/reduced_10/{0}", new string[] { "coccodrillo colore.png" });
            //var res = string.Format("Assets/3/{0}", new string[] { "diavolo_colore.png" });
            var _reducedColorsPicture = BitmapFactory.New(400, 400).FromResource(res);

            ImageTest.Source = _reducedColorsPicture;

        //    List<Color> imageColors = ImagesHelper.GetColors(_reducedColorsPicture).ToList();
            List<MyColor> imageColors = ImagesHelper.GetColors(_reducedColorsPicture, true, false);

            //imageColors.Sort(delegate(Color left, Color right)
            //{
            //    return MyColor.GetBrightness(left).CompareTo(MyColor.GetBrightness(right));
            //});

            imageColors.Sort(delegate(MyColor left, MyColor right)
            {
                return left.Count.CompareTo(right.Count);
            });

            imageColors.Reverse();

            int count = 1;
            foreach (var color in imageColors)
            {
                Rectangle r = MyVisualTreeHelper.FindChild<Rectangle>(StackPanelOrigColors, "c" + count);
                if (r != null)
                {
                    r.Fill = new SolidColorBrush(color.MainColor);
                }

                TextBlock t = MyVisualTreeHelper.FindChild<TextBlock>(StackPanelOrigColors, "c" + count + "Name");
                if (t != null)
                {
                    t.Text = color.ToString();
                }
                count++;
            }

            for (int i = count; i <= 10; i++)
            {
                Rectangle r = MyVisualTreeHelper.FindChild<Rectangle>(StackPanelOrigColors, "c" + i);
                r.Visibility = Visibility.Collapsed;

                TextBlock t = MyVisualTreeHelper.FindChild<TextBlock>(StackPanelOrigColors, "c" + i + "Name");
                t.Visibility = Visibility.Collapsed;
            }

            //List<MyColor> reducedColor = ReduceColors(imageColors, 4);
            ////colori ridotti
            //count = 1;
            //foreach (var color in reducedColor)
            //{
            //    //main color
            //    Rectangle r = MyVisualTreeHelper.FindChild<Rectangle>(StackPanelReducedColors, "cr" + count);
            //    if (r != null)
            //    {
            //        r.Fill = new SolidColorBrush(color.MainColor);
            //    }
            //    TextBlock t = MyVisualTreeHelper.FindChild<TextBlock>(StackPanelReducedColors, "cr" + count + "Name");
            //    if (t != null)
            //    {
            //        t.Text = color.MainColor.ToString();
            //    }

            //    //twins
            //    int twCounter = 1;
            //    foreach (var twin in color.Twins)
            //    {
            //        Rectangle r1 = MyVisualTreeHelper.FindChild<Rectangle>(StackPanelReducedColors, "cr" + count + "tw" + twCounter);
            //        if (r1 != null)
            //        {
            //            r1.Fill = new SolidColorBrush(twin);
            //        }
            //        twCounter++;
            //    }
            //    for (int j = twCounter; j <= 3; j++)
            //    {
            //        Rectangle r2 = MyVisualTreeHelper.FindChild<Rectangle>(StackPanelReducedColors, "cr" + count + "tw" + twCounter);
            //        r2.Visibility = Visibility.Collapsed;
            //        twCounter++;
            //    }
            //    count++;
            //}
        }


        //private List<MyColor> ReduceColors(List<Color> imageColors, int maxColors)
        //{
        //    List<Color> colors = new List<Color>(imageColors);
        //    colors.Sort(delegate(Color left, Color right)
        //    {
        //        return MyColor.GetBrightness(left).CompareTo(MyColor.GetBrightness(right));
        //    });

        //    List<MyColor> reducedColors = new List<MyColor>();
        //    foreach (var item in colors)
        //    {
        //        reducedColors.Add(new MyColor(item));
        //    }

        //    int step = 0;
        //    do
        //    {
        //        int minDiff = Int16.MaxValue;
        //        int minDiffIndex = -1;

        //        int[] brightnessDiffs = new int[reducedColors.Count - 1];
        //        for (int i = 0; i < reducedColors.Count - 1; i++)
        //        {
        //            brightnessDiffs[i] = Math.Abs(reducedColors.ElementAt(i + 1).Brightness - reducedColors.ElementAt(i).Brightness);
        //            if (brightnessDiffs[i] < minDiff)
        //            {
        //                minDiff = brightnessDiffs[i];
        //                minDiffIndex = i;
        //            }
        //        }

        //        int twinColorIndex = -1;
        //        twinColorIndex = minDiffIndex + 1;

        //        MyColor myColor = reducedColors.ElementAt(minDiffIndex);
        //        var nextColor = reducedColors.ElementAt(twinColorIndex).MainColor;
        //        myColor.Twins.Add(nextColor); //imposto come colore gemello il successivo
        //        reducedColors.RemoveAt(twinColorIndex);

        //        step++;

        //    } while (reducedColors.Count > maxColors);

        //    return reducedColors;
        //}


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