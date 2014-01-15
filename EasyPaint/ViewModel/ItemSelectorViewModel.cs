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

        //internal ItemViewModel SelectNextItem()
        //{
        //    //go to next item in group
        //    int indexOfCurItem = _items.IndexOf(_SelectedItem);
        //    if (indexOfCurItem < _items.Count() - 1)
        //    {
        //        indexOfCurItem++;
        //        SelectedItem = _items.ElementAt(indexOfCurItem); //go to next Item
        //        return SelectedItem;
        //    }
            
        //    //skip to net group and get first item
        //    if (ViewModelLocator.GroupSelectorViewModelStatic.ExistsNextGroup()) {
        //        ViewModelLocator.GroupSelectorViewModelStatic.GotoNextGroup();
        //        SetCurrentGroup(ViewModelLocator.GroupSelectorViewModelStatic.SelectedGroup);
        //        SelectedItem = Items.First();
        //        return SelectedItem;
        //    }
        //    return null; //all levels completed!
        //}

        

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
            //var msg = new GoToPageMessage() { PageName = Constants.View_GroupSeletor };
            //Messenger.Default.Send<GoToPageMessage>(msg);
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_GroupSeletor);
            return null;
        }

        private object NavigateToSelectedItemCommand()
        {
            //var msg = new GoToPageMessage() { PageName = Constants.View_Painter };
            //Messenger.Default.Send<GoToPageMessage>(msg);
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Painter);
            return null;
        }

        public void SetCurrentGroup(GroupViewModel g)
        {
            _CurrentGroup = g;
            Items = g.Items;

            //var itemsVm = new ObservableCollection<ItemViewModel>();
            //foreach (var item in g.Group.Items)
            //{
            //    ItemViewModel itemVm = new ItemViewModel(g.Group, item);
            //    itemsVm.Add(itemVm);
            //}
            //Items = itemsVm;
        }


    }
}
