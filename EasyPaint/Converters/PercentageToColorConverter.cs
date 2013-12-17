using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using System.Windows.Media;


namespace EasyPaint.Converters
{
    public class PercentageToColorConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var scb = new SolidColorBrush(Colors.Black);

            if (value == null) return scb;
            if (string.IsNullOrEmpty(value.ToString())) return scb;
                    
            int i = int.Parse(value.ToString());

            var red = i < 50
                ? 255
                : 255 - (256.0 / 100 * ((i - 50) * 2));
           
            var green = i < 50
                ? 256.0 / 100 * (i * 2)
                : 255;
            
            return new SolidColorBrush(Color.FromArgb(255, (byte)red, (byte)green, 0));
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}