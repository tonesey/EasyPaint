using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EasyPaint.Model
{
    public class MyColor
    {
        // public List<Color> Twins { get; set; }
        public int Count { get; set; }
        public Color MainColor { get; set; }

        //public int Brightness
        //{
        //    get
        //    {
        //        var maxBrightness = -1;
        //        if (MainColor != null)
        //        {
        //            // if (!Twins.Any())
        //            {
        //                maxBrightness = GetBrightness(MainColor);
        //            }
        //            //else
        //            //{
        //            //    maxBrightness = Twins.Max(c => GetBrightness(c));
        //            //}
        //        }
        //        return maxBrightness;
        //    }
        //}

        //public static int CompareColors(Color a, Color b)
        //{
        //    return 100 * (int)(1.0 - ((double)(Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B)) / (256.0 * 3)));
        //}

        //public static double CompareColors(Color a, Color b)
        //{
        //    return ((double)(Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B)) / (256.0 * 3));
        //}

        //public double GrayColor
        //{
        //    get
        //    {
        //        return .11 * MainColor.B + .59 * MainColor.G + .30 * MainColor.R;
        //    }
        //}

        public MyColor()
        {
            //Twins = new List<Color>();
            Count = 1;
        }

        public MyColor(Color color)
        {
            MainColor = color;
            //Twins = new List<Color>();
            Count = 1;
        }

        //public static int GetBrightness(Color c)
        //{
        //    return (int)Math.Sqrt(
        //       c.R * c.R * .241 +
        //       c.G * c.G * .691 +
        //       c.B * c.B * .068);
        //}

        public override string ToString()
        {
            //if (Twins.Count() == 0) return MainColor.ToString();
            //return MainColor.ToString() + " : " + string.Join("-", Twins.Select(t => t.ToString()));
            return MainColor.ToString() + " : " + Count;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MyColor)) return false;
            return (obj as MyColor).MainColor.Equals(MainColor);
        }
    }
}
