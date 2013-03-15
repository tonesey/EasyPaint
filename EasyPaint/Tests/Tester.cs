using EasyPaint.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace EasyPaint.Tests
{
    class Tester
    {
        public static void CheckImagesTester()
        {
            WriteableBitmap img1 = ImagesHelper.GetTestImage("img_orig.png");

            WriteableBitmap img2 = ImagesHelper.GetTestImage("img_empty.png");
            int count = ImagesHelper.GetNumberOfDifferentPixels(img1, img2); //attesi: 144 pixel
            Assert.IsEqual(count, 144);

            img2 = ImagesHelper.GetTestImage("img_equal_shape_andColor.png");
            count = ImagesHelper.GetNumberOfDifferentPixels(img1, img2); //attesi: 0 pixel
            Assert.IsEqual(count, 0);

            img2 = ImagesHelper.GetTestImage("img_equal_shape_notColor.png");
            count = ImagesHelper.GetNumberOfDifferentPixels(img1, img2); //attesi: 144 pixel
            Assert.IsEqual(count, 144);

            img2 = ImagesHelper.GetTestImage("img_equal_shape_andColor_but1pixel.png");
            count = ImagesHelper.GetNumberOfDifferentPixels(img1, img2); //attesi: 1 pixel
            Assert.IsEqual(count, 1);

            img2 = ImagesHelper.GetTestImage("img_equal_shape_andColor_but10pixel.png");
            count = ImagesHelper.GetNumberOfDifferentPixels(img1, img2); //attesi: 10 pixel
            Assert.IsEqual(count, 10);

            img2 = ImagesHelper.GetTestImage("img_equal_shape_andColor_NotPosition.png");
            count = ImagesHelper.GetNumberOfDifferentPixels(img1, img2); //attesi: 144 * 2 pixel - img spostata 
            Assert.IsEqual(count, 144 * 2);

        }
    }
}
