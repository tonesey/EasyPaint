/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:EasyPaint"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace EasyPaint.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                // SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<HomepageViewModel>();
        }

        #region mainviemodel
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public HomepageViewModel HomepageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HomepageViewModel>();
            }
        }

        private static HomepageViewModel _HomepageViewModelStatic;
        public static HomepageViewModel HomepageViewModelStatic
        {
            get
            {
                if (_HomepageViewModelStatic == null)
                {
                    return _HomepageViewModelStatic = ServiceLocator.Current.GetInstance<HomepageViewModel>();
                }
                return _HomepageViewModelStatic;
            }
        }
        #endregion
    }
}