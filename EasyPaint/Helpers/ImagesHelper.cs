﻿using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;
using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
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

        //public static List<Color> GetColors(WriteableBitmap bmp)
        //{
        //    List<Color> cols = new List<Color>();
        //    foreach (var pixel in bmp.Pixels)
        //    {
        //        var bytes = BitConverter.GetBytes(pixel);
        //        Color c = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        //        if (!cols.Contains(c) && (c.A != 0))
        //        {
        //            cols.Add(c);
        //        }
        //    }
        //    return cols;
        //}

        public static List<MyColor> GetColors(WriteableBitmap bmp, bool excludeDarkColors = true, bool excludeBrightColors = true)
        {
            int darkThreshold = 30;
            int lightThreshold = 200;

            List<MyColor> cols = new List<MyColor>();
            foreach (var pixel in bmp.Pixels)
            {
                var bytes = BitConverter.GetBytes(pixel);

                var a = bytes[3];
                if (a == 0)
                {
                    continue; //skips transparency
                }

                var r = bytes[2];
                var g = bytes[1];
                var b = bytes[0];

                if (excludeDarkColors) //skips too dark colors
                {
                    if (r < darkThreshold && g < darkThreshold && b < darkThreshold)
                    {
                        continue;
                    }
                }

                if (excludeBrightColors) //skips too bright colors
                {
                    if (r > lightThreshold && g > lightThreshold && b > lightThreshold)
                    {
                        continue;
                    }
                }

                Color c = Color.FromArgb(a, r, g, b);
                var myColor = new MyColor(c);
                if (!cols.Contains(myColor))
                {
                    cols.Add(myColor);
                }
                else
                {
                    cols.First(col => col.Equals(myColor)).Count++;
                }
            }
            return cols;
        }

        public static List<MyColor> ReduceColors(List<MyColor> imageColors, int maxColors, out List<MyColor> discardedColors)
        {
            List<MyColor> colors = new List<MyColor>(imageColors);
            discardedColors = new List<MyColor>();

            List<MyColor> reducedColors = new List<MyColor>();
            foreach (var item in colors)
            {
                reducedColors.Add(item);
            }

            int step = 1;
            do
            {
                double minDiff = Double.MaxValue;
                // Debug.WriteLine(">>>> step " + step + " - threshold = " + threshold);
                for (int i = 0; i < reducedColors.Count - 1; i++)
                {
                    int indexToRemove = -1;
                    var col1 = reducedColors.ElementAt(i);
                    Debug.WriteLine("start check with : " + col1);

                    for (int j = i; j < reducedColors.Count - 1; j++)
                    {
                        var nextColIndex = j + 1;
                        var col2 = reducedColors.ElementAt(nextColIndex);

                        //tutti KO, viene scartato un colore significativo!!!!!!!!!!!
                        //var colorDiff = Math.Abs(col1.GrayColor - col2.GrayColor) * 100.0 / 256.0;
                        //var colorDiff = Math.Abs(col1.Brightness - col2.Brightness) * 100.0 / 256.0;
                        //var colorDiff = MyColor.CompareColors(col1.MainColor, col2.MainColor);

                        //OK!!!!!!!!!!!!!
                        var a = new Rgb { R = col1.MainColor.R, G = col1.MainColor.G, B = col1.MainColor.B };
                        var b = new Rgb { R = col2.MainColor.R, G = col2.MainColor.G, B = col2.MainColor.B };
                        var colorDiff = a.Compare(b, new Cie1976Comparison());

                        Debug.WriteLine(j + ": difference between " + col1 + " and " + col2 + " = " + colorDiff);
                        if (colorDiff < minDiff)
                        {
                            minDiff = colorDiff;
                            indexToRemove = nextColIndex;
                        }
                    }
                    if (indexToRemove != -1)
                    {
                        if (reducedColors.Count > maxColors)
                        {
                            var colorToRemove = reducedColors.ElementAt(indexToRemove);
                            Debug.WriteLine("color to remove: " + colorToRemove);
                            discardedColors.Add(new MyColor(colorToRemove.MainColor));
                            reducedColors.RemoveAt(indexToRemove);
                        }
                    }
                }
                // threshold += 1;
                step++;
            } while (reducedColors.Count > maxColors);

            return reducedColors;
        }

        public static int GetNumberOfDifferentPixels(WriteableBitmap bmp1, WriteableBitmap bmp2, List<MyColor> colorsToIgnore)
        {
            int count = 0;
            for (int i = 0; i < bmp1.Pixels.Count(); i++)
            {
                if (bmp1.Pixels[i] != bmp2.Pixels[i])
                {
                    byte a = (byte)bmp2.Pixels[3];
                    byte r = (byte)bmp2.Pixels[2];
                    byte g = (byte)bmp2.Pixels[1];
                    byte b = (byte)bmp2.Pixels[0];
                    Color c = Color.FromArgb(a, r, g, b);
                    if (colorsToIgnore.FirstOrDefault(c1 => c1.MainColor.Equals(c)) == null) //il colore non è fra quelli da ignorare ed è diverso dall'immagine originale
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        internal static int GetAccuracyPercentage(WriteableBitmap bmp1, WriteableBitmap bmp2, List<MyColor> colorsToIgnore)
        {
            int diff = GetNumberOfDifferentPixels(bmp1, bmp2, colorsToIgnore);
            int totPixels = bmp1.Pixels.Count();
            int percentage = 100 - ((diff * 100) / totPixels);
            return percentage;
        }


        //public static void WriteContentImageToIsoStore(string contentPath, string isoStoreFileName) {

        public static void WriteContentImageToIsoStore(Uri imgUri, string isoStoreFileName)
        {

            // var pngStream = Application.GetResourceStream(new Uri(contentPath, UriKind.RelativeOrAbsolute)).Stream;

            StreamResourceInfo streamInfo = App.GetResourceStream(imgUri);
            if (streamInfo != null)
            {
            }

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
                        (byte)(pixel & 0xFF), 
                        (byte)((pixel >> 24) & 0xFF)
                        );
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("Cannot print images from other domains");
            }

            BitmapImage biImage = new BitmapImage();
            // Save it to disk
            using (Stream streamPNG = eiImage.GetStream())
            {
                StreamReader srPNG = new StreamReader(streamPNG);
                byte[] baBinaryData = new Byte[streamPNG.Length];
                long bytesRead = streamPNG.Read(baBinaryData, 0, (int)streamPNG.Length);
                
                using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream("temp.png", FileMode.Create, IsolatedStorageFile.GetUserStoreForApplication()))
                {
                    isfStream.Write(baBinaryData, 0, baBinaryData.Length);
                }

                using (IsolatedStorageFileStream isfStream = new IsolatedStorageFileStream("temp.png", FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication()))
                {
                    biImage.SetSource(isfStream);
                }
            }

            return biImage;
        }
    }
}
