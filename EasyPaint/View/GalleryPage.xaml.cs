using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model.UI;
using EasyPaint.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace EasyPaint.View
{
    public partial class GalleryPage : PhoneApplicationPage
    {
        //private GalleryViewModel _vm;
        //private Dictionary<string, LoopingListDataItem> _dict = new Dictionary<string, LoopingListDataItem>();
        //LoopingListDataSource _listDs = new LoopingListDataSource(0);
        //public LoopingListDataSource ListDs
        //{
        //    get { return _listDs; }
        //}

        public GalleryPage()
        {
            InitializeComponent();
            Loaded += ItemSelectorPage_Loaded;
            Unloaded += ItemSelectorPage_Unloaded;
        }

        void ItemSelectorPage_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void ItemSelectorPage_Loaded(object sender, RoutedEventArgs e)
        {
            AppViewModel.CurrentPage = this;
            MessagingHelper.GetInstance().CurrentDispatcher = Dispatcher;
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ViewModelLocator.GalleryViewModelStatic.SelectedItem = (((sender as Grid).DataContext as PictureLoopingItem).DataContext as ItemViewModel);
            ViewModelLocator.GalleryViewModelStatic.ItemSelectedCommand.Execute(null);
        }

    }
}