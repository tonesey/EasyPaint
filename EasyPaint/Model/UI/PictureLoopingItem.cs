using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace EasyPaint.Model.UI
{
    public class PictureLoopingItem : LoopingListDataItem
    {

        public object DataContext { get; set; }

//#if COLORSCHECK
//        private BitmapSource _picture;
//        public BitmapSource Picture
//        {
//            get
//            {
//                return this._picture;
//            }
//            set
//            {
//                if (value != this._picture)
//                {
//                    this._picture = value;
//                    this.OnPropertyChanged("Picture");
//                }
//            }
//        }
//#else

        private Uri _picture;
        public Uri Picture
        {
            get
            {
                return this._picture;
            }
            set
            {
                if (value != this._picture)
                {
                    this._picture = value;
                    this.OnPropertyChanged("Picture");
                }
            }
        }


        private bool _IsLocked;
        public bool IsLocked
        {
            get
            {
                return _IsLocked;
            }
            set
            {
                if (value != _IsLocked)
                {
                    _IsLocked = value;
                    this.OnPropertyChanged("IsLocked");
                }
            }
        }

    }
}
