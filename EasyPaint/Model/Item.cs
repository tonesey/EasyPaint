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

        //private Item _next;
        //public Item Next
        //{
        //    get { return _next; }
        //    set { _next = value; }
        //}

        //private Item _prev;
        //public Item Prev
        //{
        //    get { return _prev; }
        //    set { _prev = value; }
        //}

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
            //_prev = null;
            //_next = null;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}
