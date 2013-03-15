using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public class MyPicture
    {
        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}
