using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using EasyPaint.ViewModel;
using EasyPaint.Model.UI;
using System.IO;

namespace EasyPaint.Converters
{
    public class LockedToImageConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ItemViewModel).IsLocked ?  (value as ItemViewModel).ImageSourceHidden : (value as ItemViewModel).ImageSource;


            //real time image process - KO
            //ItemViewModel p = (value as ItemViewModel);
            //BitmapImage tn = new BitmapImage();
            //tn.SetSource(Application.GetResourceStream(p.ImageSource).Stream);
            //if (p.IsLocked)
            //{
            //    WriteableBitmap wb = new WriteableBitmap(tn);
            //    wb.Resize(tn.PixelWidth, tn.PixelHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
            //    var wb2 = WriteableBitmapExtensions.Convolute(wb, WriteableBitmapExtensions.KernelGaussianBlur3x3);
            //    BitmapImage bmp = new BitmapImage();
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        wb2.SaveJpeg(ms, (int)tn.PixelWidth, (int)tn.PixelHeight, 0, 100);
            //        bmp.SetSource(ms);
            //    }
            //    return bmp;
            //}
            //return tn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        #endregion
    }
}
