//using ColorMine.ColorSpaces;
//using ColorMine.ColorSpaces.Comparisons;
using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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


        public static List<Color> GetColors(WriteableBitmap bmp, bool excludeDarkColors = true, bool excludeBrightColors = true)
        {
            int darkThreshold = 30;
            int lightThreshold = 200;

            List<Color> cols = new List<Color>();
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
                if (!cols.Contains(c))
                {
                    cols.Add(c);
                }
            }
            return cols;
        }

        //public static List<MyColor> GetColors(WriteableBitmap bmp, bool excludeDarkColors = true, bool excludeBrightColors = true)
        //{
        //    int darkThreshold = 30;
        //    int lightThreshold = 200;

        //    List<MyColor> cols = new List<MyColor>();
        //    foreach (var pixel in bmp.Pixels)
        //    {
        //        var bytes = BitConverter.GetBytes(pixel);

        //        var a = bytes[3];
        //        if (a == 0)
        //        {
        //            continue; //skips transparency
        //        }

        //        var r = bytes[2];
        //        var g = bytes[1];
        //        var b = bytes[0];

        //        if (excludeDarkColors) //skips too dark colors
        //        {
        //            if (r < darkThreshold && g < darkThreshold && b < darkThreshold)
        //            {
        //                continue;
        //            }
        //        }

        //        if (excludeBrightColors) //skips too bright colors
        //        {
        //            if (r > lightThreshold && g > lightThreshold && b > lightThreshold)
        //            {
        //                continue;
        //            }
        //        }

        //        Color c = Color.FromArgb(a, r, g, b);
        //        var myColor = new MyColor(c);
        //        if (!cols.Contains(myColor))
        //        {
        //            cols.Add(myColor);
        //        }
        //        else
        //        {
        //            cols.First(col => col.Equals(myColor)).Count++;
        //        }
        //    }
        //    return cols;
        //}

        //public static List<MyColor> ReduceColors(List<MyColor> imageColors, int maxColors, out List<MyColor> discardedColors)
        //{
        //    List<MyColor> colors = new List<MyColor>(imageColors);
        //    discardedColors = new List<MyColor>();

        //    List<MyColor> reducedColors = new List<MyColor>();
        //    foreach (var item in colors)
        //    {
        //        reducedColors.Add(item);
        //    }

        //    int step = 1;
        //    do
        //    {
        //        double minDiff = Double.MaxValue;
        //        // Debug.WriteLine(">>>> step " + step + " - threshold = " + threshold);
        //        for (int i = 0; i < reducedColors.Count - 1; i++)
        //        {
        //            int indexToRemove = -1;
        //            var col1 = reducedColors.ElementAt(i);
        //            Debug.WriteLine("start check with : " + col1);

        //            for (int j = i; j < reducedColors.Count - 1; j++)
        //            {
        //                var nextColIndex = j + 1;
        //                var col2 = reducedColors.ElementAt(nextColIndex);

        //                //tutti KO, viene scartato un colore significativo!!!!!!!!!!!
        //                //var colorDiff = Math.Abs(col1.GrayColor - col2.GrayColor) * 100.0 / 256.0;
        //                //var colorDiff = Math.Abs(col1.Brightness - col2.Brightness) * 100.0 / 256.0;
        //                //var colorDiff = MyColor.CompareColors(col1.MainColor, col2.MainColor);

        //                //OK!!!!!!!!!!!!!
        //                var a = new Rgb { R = col1.MainColor.R, G = col1.MainColor.G, B = col1.MainColor.B };
        //                var b = new Rgb { R = col2.MainColor.R, G = col2.MainColor.G, B = col2.MainColor.B };
        //                var colorDiff = a.Compare(b, new Cie1976Comparison());

        //                Debug.WriteLine(j + ": difference between " + col1 + " and " + col2 + " = " + colorDiff);
        //                if (colorDiff < minDiff)
        //                {
        //                    minDiff = colorDiff;
        //                    indexToRemove = nextColIndex;
        //                }
        //            }
        //            if (indexToRemove != -1)
        //            {
        //                if (reducedColors.Count > maxColors)
        //                {
        //                    var colorToRemove = reducedColors.ElementAt(indexToRemove);
        //                    Debug.WriteLine("color to remove: " + colorToRemove);
        //                    discardedColors.Add(new MyColor(colorToRemove.MainColor));
        //                    reducedColors.RemoveAt(indexToRemove);
        //                }
        //            }
        //        }
        //        // threshold += 1;
        //        step++;
        //    } while (reducedColors.Count > maxColors);

        //    return reducedColors;
        //}


        public static Color HexStringToColor(string hexColor)
        {
            string hc = ExtractHexDigits(hexColor);
            if (hc.Length != 6)
            {
                // you can choose whether to throw an exception
                throw new ArgumentException("hexColor is not exactly 6 digits.");
            }
            string r = hc.Substring(0, 2);
            string g = hc.Substring(2, 2);
            string b = hc.Substring(4, 2);
            Color color = Colors.Black;
            try
            {
                int ri
                   = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi
                   = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi
                   = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb((byte)255, (byte)ri, (byte)gi, (byte)bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                throw new ArgumentException("Conversion failed.");
            }
            return color;
        }
        /// <summary>
        /// Extract only the hex digits from a string.
        /// </summary>
        public static string ExtractHexDigits(string input)
        {
            // remove any characters that are not digits (like #)
            Regex isHexDigit
               = new Regex("[abcdefABCDEF\\d]+", RegexOptions.Compiled);
            string newnum = "";
            foreach (char c in input)
            {
                if (isHexDigit.IsMatch(c.ToString()))
                    newnum += c.ToString();
            }
            return newnum;
        }


        internal static WriteableBitmap CheckPaletteCoverage(WriteableBitmap imageBase, WriteableBitmap imageLineart, List<Color> palette, out int coverage)
        {
            WriteableBitmap resultImage = new WriteableBitmap(imageBase.PixelWidth, imageBase.PixelHeight);
            int x, y;
            coverage = 0;

            int matchingPixels = 0;

            for (int i = 0; i < imageBase.Pixels.Count(); i++)
            {
                x = i % imageBase.PixelWidth;
                y = i / imageBase.PixelWidth;

                var imageBaseColorBytes = BitConverter.GetBytes(imageBase.Pixels[i]);

                byte a = (byte)imageBaseColorBytes[3];
                byte r = (byte)imageBaseColorBytes[2];
                byte g = (byte)imageBaseColorBytes[1];
                byte b = (byte)imageBaseColorBytes[0];

                Color c = Color.FromArgb(a, r, g, b);

                if ((byte)imageBaseColorBytes[3] != 255)
                {
                    resultImage.SetPixel(x, y, Colors.Yellow);
                    matchingPixels++; //il trasparente viene considerato buono
                    continue;
                }

                var imageLineartColorBytes = BitConverter.GetBytes(imageLineart.Pixels[i]);
                if ((byte)imageLineartColorBytes[3] == 255)
                {
                    //il pixel corrispondente della lineart non è trasparente: viene considerato buono, come se l'avesse disegnato l'utente
                    matchingPixels++;
                    resultImage.SetPixel(x, y, Colors.Green);
                    continue;
                }
                
                //if (palette.FirstOrDefault(c1 => c1.Equals(c)) == default(Color)) 
                if (!palette.Contains(c))
                {
                    //il colore non è presente nella palette
                    resultImage.SetPixel(x, y, Colors.Red);
                }
                else
                {
                    //il colore è presente nella palette
                    matchingPixels++; 
                    resultImage.SetPixel(x, y, Colors.Green);
                    //resultImage.SetPixel(x, y, c);
                }
            }

            coverage = ((matchingPixels * 100) / imageBase.Pixels.Count());
            return resultImage;
        }


        public static int GetNumberOfDifferentPixels(WriteableBitmap bmp1, WriteableBitmap bmp2, List<Color> colorsToIgnore, out int transparentPixelsCount, out WriteableBitmap resultImage)
        {
            int countDiff = 0;
            //  int countEq = 0;
            transparentPixelsCount = 0;
            
            resultImage = new WriteableBitmap(bmp1.PixelWidth, bmp1.PixelHeight);
            int x, y;

            for (int i = 0; i < bmp1.Pixels.Count(); i++)
            {
                var bytesbmp1 = BitConverter.GetBytes(bmp1.Pixels[i]);
                var bytesbmp2 = BitConverter.GetBytes(bmp2.Pixels[i]);
               
                x = i % bmp1.PixelWidth;
                y = i / bmp1.PixelWidth;
               
                if ((byte)bytesbmp1[3] != 255)
                {
                    transparentPixelsCount++;
                    resultImage.SetPixel(x, y, Colors.Transparent);
                    continue;
                }

                if (bmp1.Pixels[i] != bmp2.Pixels[i])
                {
                    byte a = (byte)bytesbmp2[3];
                    byte r = (byte)bytesbmp2[2];
                    byte g = (byte)bytesbmp2[1];
                    byte b = (byte)bytesbmp2[0];
                    Color c = Color.FromArgb(a, r, g, b);
                    if (!colorsToIgnore.Contains(c)) //il colore non è fra quelli da ignorare ed è diverso dall'immagine originale
                    {
                        countDiff++;
                        resultImage.SetPixel(x, y, Colors.Red);
                    }
                    else
                    {
                        resultImage.SetPixel(x, y, Colors.Yellow);
                    }
                }
                else
                {
                    resultImage.SetPixel(x, y, Colors.Green);
                }
            }
            return countDiff;
        }

        internal static int GetAccuracyPercentage(WriteableBitmap bmp1, WriteableBitmap bmp2, List<Color> colorsToIgnore, out WriteableBitmap resImg)
        {

            int transparentPixelsCount = 0;
            int diff = GetNumberOfDifferentPixels(bmp1, bmp2, colorsToIgnore, out transparentPixelsCount, out resImg);

            int totPixels = bmp1.Pixels.Count() - transparentPixelsCount;
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
