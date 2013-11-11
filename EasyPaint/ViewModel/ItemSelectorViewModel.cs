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

        private GroupViewModel _CurrentGroup = null;
        public GroupViewModel CurrentGroup
        {
            get
            {
                return _CurrentGroup;
            }
            set
            {
                _CurrentGroup = value;
                OnPropertyChanged("CurrentGroup");
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

        public void SetCurrentGroup(GroupViewModel g)
        {
            _CurrentGroup = g;
            var itemsVm = new ObservableCollection<ItemViewModel>();
            foreach (var item in g.Group.Items)
            {
                ItemViewModel itemVm = new ItemViewModel(g.Group, item);
                itemsVm.Add(itemVm);
            }
            Items = itemsVm;
        }
    }
}
