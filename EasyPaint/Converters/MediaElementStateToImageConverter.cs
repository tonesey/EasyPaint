using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using System.Windows.Media;


namespace EasyPaint.Converters
{
    public class MediaElementStateToImageConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            var mediastate = (MediaElementState)value;
            switch (mediastate)
            {
                case MediaElementState.AcquiringLicense:
                case MediaElementState.Buffering:
                case MediaElementState.Individualizing:
                case MediaElementState.Opening:
                    break;
                case MediaElementState.Closed:
                case MediaElementState.Stopped:
                case MediaElementState.Paused:
                    return "/EasyPaint;component/Assets/buttons/sound_off.png";
                case MediaElementState.Playing:
                    return "/EasyPaint;component/Assets/buttons/sound_on.png";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}