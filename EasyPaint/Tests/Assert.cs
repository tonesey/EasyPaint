using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Tests
{
    class Assert
    {
        internal static void IsEqual(int actual, int expected)
        {
            if (actual != expected)
            {
                throw new Exception(string.Format("expected = {0} - actual = {1}", expected, actual)); 
            }
        }
    }
}
