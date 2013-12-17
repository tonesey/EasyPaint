using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using EasyPaint.Model;


namespace EasyPaint.Converters
{
    public class PercentageToResultConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {


            int val = int.Parse(value.ToString());

            if (val < Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED) return LocalizedResources.message_level_failed;
            return LocalizedResources.message_level_completed;

        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}