using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Text;
using EasyPaint.Model;
using Telerik.Windows.Controls;
using System.Threading;
using System.Collections.ObjectModel;
using EasyPaint.Data;

namespace EasyPaint.ViewModel
{
    public abstract class AppViewModel : MyViewModelBase
    {
        protected  IDataService _dataService = null;

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get
            {
                return _IsBusy;
            }
            set
            {
                _IsBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }
    }
}
