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
        public RelayCommand GoToGroupSelectorCommand { get; private set; }
        
        public ItemSelectorViewModel()
        {
            ItemSelectedCommand = new RelayCommand(() => NavigateToSelectedItemCommand());
            GoToGroupSelectorCommand = new RelayCommand(() => GoToGroupSelector());
        }

        private object GoToGroupSelector()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GroupSeletor);
            return null;
        }

        private object NavigateToSelectedItemCommand()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Painter);
            return null;
        }

        public void SetCurrentGroup(GroupViewModel g)
        {
            _CurrentGroup = g;
            Items = g.Items;
        }

        internal int GetUnlockedItemsCount()
        {
            return Items.Count(it => !it.IsLocked);
        }
    }
}
