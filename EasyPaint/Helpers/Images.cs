using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows_Phone_Sig_Capture;

namespace EasyPaint.Helpers
{
    class ImagesHelper
    {

        public static WriteableBitmap GetTestImage(string resName)
        {
            return GetImgFromEmbeddedResource("EasyPaint.Assets.testImages", resName);
        }

        public static WriteableBitmap GetImgFromEmbeddedResource(string basePath, string resName)
        {
            WriteableBitmap bmp = null;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", basePath, resName)))
            {
                BitmapImage img = new BitmapImage();
                img.SetSource(s);
                bmp = new WriteableBitmap(img);
            }
            return bmp;
        }


         public static List<Color> GetColors(WriteableBitmap bmp)
         {
             List<Color> cols = new List<Color>();
             foreach (var pixel in bmp.Pixels)
             {
                 var bytes = BitConverter.GetBytes(pixel);
                 Color c = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                 if (!cols.Contains(c)) {
                     cols.Add(c);
                 }
             }
             return cols;
         }



        public static int GetNumberOfDifferentPixels(WriteableBitmap bmp1, WriteableBitmap bmp2)
        {
            int count = 0;
            for (int i = 0; i < bmp1.Pixels.Count(); i++)
            {
                if (bmp1.Pixels[i] != bmp2.Pixels[i])
                {
                    count++;
                }
            }
            return count;
        }

        internal static int GetPercentageOfDifferentPixels(WriteableBitmap bmp1, WriteableBitmap bmp2)
        {
            int diff = GetNumberOfDifferentPixels(bmp1, bmp2);
            int totPixels = bmp1.Pixels.Count();
            int percentage = 100 - ((diff * 100) / totPixels);
            return percentage;
        }
       

        //public static void WriteContentImageToIsoStore(string contentPath, string isoStoreFileName) {

        public static void WriteContentImageToIsoStore(Uri imgUri, string isoStoreFileName)
        {

           // var pngStream = Application.GetResourceStream(new Uri(contentPath, UriKind.RelativeOrAbsolute)).Stream;
            var pngStream = Application.GetResourceStream(imgUri).Stream;
            int counter;
            byte[] buffer = new byte[1024];
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(isoStoreFileName, FileMode.Create, isf))
                {
                    counter = 0;
                    while (0 < (counter = pngStream.Read(buffer, 0, buffer.Length)))
                    {
                        isfs.Write(buffer, 0, counter);
                    }
                    pngStream.Close();
                }
            }
        }


        public static BitmapImage GetBitmapImageFromIsoStore(string fileName)
        {
            byte[] streamData;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    streamData = new byte[isfs.Length];
                    isfs.Read(streamData, 0, streamData.Length);
                }
            }

            MemoryStream ms = new MemoryStream(streamData);
            BitmapImage bmpImage = null;
            try
            {
                 bmpImage = new BitmapImage();
                bmpImage.SetSource(ms);
            }
            catch (Exception ex)
            {
                throw;
            }

            return bmpImage;
        }


        internal static int GetNotBlankPixels(WriteableBitmap bmp)
        {
            return bmp.Pixels.Where(p => p != 0).Count();
        }

        public static BitmapImage GetImageFromInkPresenter(InkPresenter inkPresenter)
        {

            WriteableBitmap wbBitmap = new WriteableBitmap(inkPresenter, new TranslateTransform());
            EditableImage eiImage = new EditableImage(wbBitmap.PixelWidth, wbBitmap.PixelHeight);

            try
            {
                for (int y = 0; y < wbBitmap.PixelHeight; ++y)
                {
                    for (int x = 0; x < wbBitmap.PixelWidth; ++x)
                    {
                        int pixel = wbBitmap.Pixels[wbBitmap.PixelWidth * y + x];
                        eiImage.SetPixel(x, y,
                        (byte)((pixel >> 16) & 0xFF),
                        (byte)((pixel >> 8) & 0xFF),
                        (byte)(pixel & 0xFF), (byte)((pixel >> 24) & 0xFF)
                        );
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("Cannot print images from other domains");
            }

            // Save it to disk
            Stream streamPNG = eiImage.GetStream();
            StreamReader srPNG = new StreamReader(streamPNG);
            byte[] baBinaryData = new Byte[streamPNG.Length];
            long bytesRead = streamPNG.Read(baBinaryData, 0, (int)streamPNG.Length);
            IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream("temp.png", FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication());
            isfStream.Write(baBinaryData, 0, baBinaryData.Length);
            isfStream.Close();

            //Show to image
            isfStream = new IsolatedStorageFileStream("temp.png", FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication());
            BitmapImage biImage = new BitmapImage();
            biImage.SetSource(isfStream);
            isfStream.Close();

            return biImage;
        }
    }
}
