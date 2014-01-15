/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:EasyPaint"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using EasyPaint.Data;
using EasyPaint.Helpers;
using EasyPaint.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using Wp8Shared.Helpers;

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

        #region views uris
        public static readonly Uri View_Painter = new Uri("/View/PainterPage.xaml", UriKind.RelativeOrAbsolute);
        public static readonly Uri View_Homepage = new Uri("/View/Homepage.xaml", UriKind.RelativeOrAbsolute);
        public static readonly Uri View_GroupSeletor = new Uri("/View/GroupSelectorPage.xaml", UriKind.RelativeOrAbsolute);
        public static readonly Uri View_ItemSeletor = new Uri("/View/ItemSelectorPage.xaml", UriKind.RelativeOrAbsolute);
        public static readonly Uri View_Credits = new Uri("/View/CreditsPage.xaml", UriKind.RelativeOrAbsolute);
        #endregion

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }

            SimpleIoc.Default.Register<INavigationService, NavigationHelper>();

            SimpleIoc.Default.Register<HomePageViewModel>();
            SimpleIoc.Default.Register<GroupSelectorViewModel>();
            SimpleIoc.Default.Register<ItemSelectorViewModel>();
            SimpleIoc.Default.Register<PainterPageViewModel>();
        }

        private static INavigationService _NavigationServiceStatic;
        public static INavigationService NavigationServiceStatic
        {
            get
            {
                if (_NavigationServiceStatic == null)
                {
                    return _NavigationServiceStatic = ServiceLocator.Current.GetInstance<INavigationService>();
                }
                return _NavigationServiceStatic;
            }
        }

        #region homepage
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public HomePageViewModel HomepageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HomePageViewModel>();
            }
        }
        #endregion

        #region group selector
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public GroupSelectorViewModel GroupSelectorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GroupSelectorViewModel>();
            }
        }

        private static GroupSelectorViewModel _GroupSelectorViewModelStatic;
        public static GroupSelectorViewModel GroupSelectorViewModelStatic
        {
            get
            {
                if (_GroupSelectorViewModelStatic == null)
                {
                    return _GroupSelectorViewModelStatic = ServiceLocator.Current.GetInstance<GroupSelectorViewModel>();
                }
                return _GroupSelectorViewModelStatic;
            }
        }
        #endregion

        #region item selector
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public ItemSelectorViewModel ItemSelectorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ItemSelectorViewModel>();
            }
        }

        private static ItemSelectorViewModel _ItemSelectorViewModelStatic;
        public static ItemSelectorViewModel ItemSelectorViewModelStatic
        {
            get
            {
                if (_ItemSelectorViewModelStatic == null)
                {
                    return _ItemSelectorViewModelStatic = ServiceLocator.Current.GetInstance<ItemSelectorViewModel>();
                }
                return _ItemSelectorViewModelStatic;
            }
        }
        #endregion

        #region painter page
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
        public PainterPageViewModel PainterPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PainterPageViewModel>();
            }
        }

        private static PainterPageViewModel _PainterPageViewModelStatic;
        public static PainterPageViewModel PainterPageViewModelStatic
        {
            get
            {
                if (_PainterPageViewModelStatic == null)
                {
                    return _PainterPageViewModelStatic = ServiceLocator.Current.GetInstance<PainterPageViewModel>();
                }
                return _PainterPageViewModelStatic;
            }
        }
        #endregion

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }

    }
}