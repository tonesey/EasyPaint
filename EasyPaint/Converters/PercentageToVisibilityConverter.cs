using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows;


namespace EasyPaint.Converters
{
    public class PercentageToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {


            int val = int.Parse(value.ToString());

            if (val < 70) return Visibility.Collapsed;
            return Visibility.Visible;

        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}