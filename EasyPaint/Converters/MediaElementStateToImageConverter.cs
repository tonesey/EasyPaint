using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace EasyPaint.Converters
{
    public class MediaElementStateToImageConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {

            if (value == null)
            {
                return new BitmapImage(new Uri("/EasyPaint;component/Assets/buttons/sound_off.png", UriKind.RelativeOrAbsolute));
            }

            //var mediastate = (MediaElementState)value;
            //switch (mediastate)
            //{
            //    //case MediaElementState.AcquiringLicense:
            //    //case MediaElementState.Buffering:
            //    //case MediaElementState.Individualizing:
            //    //case MediaElementState.Opening:
            //    //case MediaElementState.Closed:
            //    //case MediaElementState.Stopped:
            //    //case MediaElementState.Paused:
            //    //    return "/EasyPaint;component/Assets/buttons/sound_off.png";
            //    case MediaElementState.Playing:
            //        return new BitmapImage(new Uri("/EasyPaint;component/Assets/buttons/sound_on.png", UriKind.RelativeOrAbsolute));
            //}


            bool isMuted = (bool)value;

            if (isMuted)
                return new BitmapImage(new Uri("/EasyPaint;component/Assets/buttons/sound_off.png", UriKind.RelativeOrAbsolute));

            return new BitmapImage(new Uri("/EasyPaint;component/Assets/buttons/sound_on.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}