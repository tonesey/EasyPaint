using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.Model.UI
{
    public class PictureLoopingItem : LoopingListDataItem
    {

        public bool IsLocked { get; set; }

        public object DataContext { get; set; }

        private Uri pictureSource;
        public Uri Picture
        {
            get
            {
                return this.pictureSource;
            }
            set
            {
                if (value != this.pictureSource)
                {
                    this.pictureSource = value;
                    this.OnPropertyChanged("Picture");
                }
            }
        }



    }
}
