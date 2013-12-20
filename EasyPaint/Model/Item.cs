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

        public int Score { get; set; }
        public int RecordScore { get; set; }

        private bool _isLocked = false;
        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public Item() {
            Key = string.Empty;
            ImgFilename = string.Empty;
            RecordScore = 0;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
