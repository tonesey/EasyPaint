using EasyPaint.Data;
using EasyPaint.Helpers;
using EasyPaint.Messages;
using EasyPaint.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class GalleryViewModel : AppViewModel
    {
        private ObservableCollection<ItemViewModel> _items = new ObservableCollection<ItemViewModel>();
        public ObservableCollection<ItemViewModel> Items
        {
            get
            {
                return this._items;
            }
            private set
            {
                this._items = value;
                OnPropertyChanged("Items");
            }
        }

        private ItemViewModel _SelectedItem = null;
        public ItemViewModel SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        public RelayCommand ItemSelectedCommand { get; private set; }
        public RelayCommand GotoHomepageCommand { get; private set; }

        public GalleryViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        throw new Exception("invalid data source: " + error.Message);
                    }
                    InitGalleryItems(item.CfgData.Groups);
                });

            ItemSelectedCommand = new RelayCommand(() => NavigateToSelectedItemCommand());
            GotoHomepageCommand = new RelayCommand(() => GotoHomepage());
        }

        private void InitGalleryItems(List<Group> groups)
        {
            foreach (var group in groups)
            {
                foreach (var item in group.Items.Where(it => !it.IsLocked))
                {
                    Items.Add(new ItemViewModel(group, item));
                }
            }
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }

        private object NavigateToSelectedItemCommand()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Painter);
            return null;
        }

    }
}
