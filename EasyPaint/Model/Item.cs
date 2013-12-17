using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public class Item
    {
        public const int MINIMUM_UNLOCK_PERCENTAGE_REQUIRED = 70;

        public string Key { get; set; }
        public string ImgFilename { get; set; }

        //public int CurrentScore { get; set; }
        public int UserMaximumScore { get; set; }

        public Item() {
            Key = string.Empty;
            ImgFilename = string.Empty;
            UserMaximumScore = 0;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
