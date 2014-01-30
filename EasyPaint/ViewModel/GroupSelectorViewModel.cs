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
    public class GroupSelectorViewModel : AppViewModel
    {
        public RelayCommand GotoHomepageCommand { get; private set; }

        private ObservableCollection<ItemViewModel> _allItemsList;
        private ObservableCollection<ItemViewModel> AllItemsList {
            get {
                return _allItemsList;
            }
        }

        private ObservableCollection<GroupViewModel> _groups;
        public ObservableCollection<GroupViewModel> Groups
        {
            get
            {
                return this._groups;
            }
            private set
            {
                this._groups = value;
                OnPropertyChanged("Groups");
            }
        }

        private GroupViewModel _SelectedGroup = null;
        public GroupViewModel SelectedGroup
        {
            get
            {
                return _SelectedGroup;
            }
            set
            {
                _SelectedGroup = value;
                OnPropertyChanged("SelectedGroup");
            }
        }

        public RelayCommand GroupSelectedCommand { get; private set; }

        public GroupSelectorViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        throw new Exception("invalid data source: " + error.Message);
                    }
                    InitGroups(item.CfgData.Groups);
                });

            GroupSelectedCommand = new RelayCommand(() => NavigateToSelectedGroupCommand());
            GotoHomepageCommand = new RelayCommand(() => GotoHomepage());
        }

        private object GotoHomepage()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_Homepage);
            return null;
        }

        private object NavigateToSelectedGroupCommand()
        {
            ViewModelLocator.NavigationServiceStatic.NavigateTo(ViewModelLocator.View_ItemSeletor);
            return null;
        }

        private void InitGroups(List<Group> groups)
        {
            var groupsVm = new ObservableCollection<GroupViewModel>();
            foreach (var group in groups)
            {
                GroupViewModel groupVm = new GroupViewModel(group);
                groupsVm.Add(groupVm);
            }
            Groups = groupsVm;
            _allItemsList = new ObservableCollection<ItemViewModel>(Groups.SelectMany(g => g.Items));
        }

        public ItemViewModel GetNextItem(ItemViewModel current) {
            int curIndex = _allItemsList.IndexOf(current);
            if ((curIndex + 1) < _allItemsList.Count) {
                return _allItemsList.ElementAt(curIndex + 1);
            }
            return null;
        }
    }
}
