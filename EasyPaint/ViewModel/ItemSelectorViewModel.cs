using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class ItemSelectorViewModel : AppViewModel
    {
        private ObservableCollection<ItemViewModel> _items;
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

        private Item _SelectedItem = null;
        public Item SelectedItem
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

        public ItemSelectorViewModel() { 
        
        }

        //public ItemSelectorViewModel(IDataService dataService)
        //{
        //    _dataService = dataService;
        //    _dataService.GetData(
        //        (item, error) =>
        //        {
        //            if (error != null)
        //            {
        //                // Report error here
        //                return;
        //            }
        //            //_model = item;
        //            //InitItems(item.Groups);
        //        });
        //    //GotoMainPageCommand = new RelayCommand(() => NavigateToMainPageCommand());
        //}

        public void SetGroupItems(Group g)
        {
            var itemsVm = new ObservableCollection<ItemViewModel>();
            foreach (var item in g.Items)
            {
                ItemViewModel itemVm = new ItemViewModel(g, item);
                itemsVm.Add(itemVm);
            }
            Items = itemsVm;
        }
    }
}
