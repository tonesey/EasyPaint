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

        public ItemSelectorViewModel() {

            ItemSelectedCommand = new RelayCommand(() => NavigateToSelectedItemCommand());
        }

        private object NavigateToSelectedItemCommand()
        {
            var msg = new GoToPageMessage() { PageName = Constants.View_Painter };
            Messenger.Default.Send<GoToPageMessage>(msg);
            return null;
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
