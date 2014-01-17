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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace EasyPaint.ViewModel
{
    public abstract class AppViewModel : ViewModelBase
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

        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            _isInDesignMode
                = (bool)DependencyPropertyDescriptor
                .FromProperty(prop, typeof(FrameworkElement))
                .Metadata.DefaultValue;
#endif
                }
                return _isInDesignMode.Value;
            }
        }


        public PhoneApplicationPage CurrentPage { get; set; }
    }
}
