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
                    InitGroups(item.Groups);
                });

           GroupSelectedCommand = new RelayCommand(() => NavigateToSelectedGroupCommand());
        }

        private object NavigateToSelectedGroupCommand()
        {
            var msg = new GoToPageMessage() { PageName = Constants.View_ItemSeletor };
            Messenger.Default.Send<GoToPageMessage>(msg);
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
        }
    }
}
