using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace EasyPaint.Model.UI
{
    public class PictureLoopingItem : LoopingListDataItem
    {
        private object _DataContext = null;
        public object DataContext
        {
            get
            {
                return _DataContext;
            }
            set
            {
                if (value != _DataContext)
                {
                    _DataContext = value;
                    this.OnPropertyChanged("DataContext");
                }
            }
        }
    }
}
