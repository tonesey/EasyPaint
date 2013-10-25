using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public class Item
    {
        public string Key { get; set; }
        public string ImgFilename { get; set; }

        public override string ToString()
        {
            return Key;
        }
    }
}
