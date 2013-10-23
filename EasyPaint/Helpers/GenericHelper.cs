using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Navigation;
using System.Windows.Threading;
using Wp7Shared.Helpers;

namespace EasyPaint.Helpers
{
    class GenericHelper
    {

        public static void Navigate(NavigationService ns, Dispatcher ds, string pageName)
        {
            StringBuilder sb = new StringBuilder("/View/");
            sb.Append(pageName);
            sb.Append(".xaml");
            NavigationHelper.SafeNavigateTo(ns, ds, sb.ToString());
        }

    }
}
