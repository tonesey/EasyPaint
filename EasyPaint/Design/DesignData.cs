using EasyPaint.Data;
using EasyPaint.Model;
using EasyPaint.Model.UI;
using EasyPaint.Settings;
using EasyPaint.ViewModel;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace EasyPaint.Design
{
    public enum ListType
    {
        Group,
        Item,
        GalleryItem
    }

    public class DesignData : AppViewModel
    {
        private AppData _data = null;

        #region group and items selectors
        //list type
        public ListType ListId { get; set; }

        //for item selector only
        private string _CurrentGroupId = null;
        public string CurrentGroupId
        {
            get
            {
                return _CurrentGroupId;
            }
            set
            {
                _CurrentGroupId = value;
                OnPropertyChanged("CurrentGroup");
            }
        }

        private GroupViewModel _currentGroup = null;
        public GroupViewModel CurrentGroup
        {
            get
            {
                _currentGroup = new GroupViewModel(_data.Groups.FirstOrDefault(g => g.Id == CurrentGroupId));
                return _currentGroup;
            }
        }

        LoopingListDataSource _designDataDs_Groups = new LoopingListDataSource(3);
        LoopingListDataSource _designDataDs_Items = new LoopingListDataSource(3);
        LoopingListDataSource _designDataDs_ItemsUnlocked = new LoopingListDataSource(3);

        public LoopingListDataSource ListDs
        {
            get
            {
                switch (ListId)
                {
                    case ListType.Group:
                        return _designDataDs_Groups;
                    case ListType.Item:
                        return _designDataDs_Items;
                    case ListType.GalleryItem:
                        return _designDataDs_ItemsUnlocked;
                }
                return null;
            }
        }

        #region arcade mode
        #region items
        void _designDataDs_Items_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            (e.Item as PictureLoopingItem).DataContext = GetItemDataContext(e);
        }

        void _designDataDs_Items_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            e.Item = new PictureLoopingItem() { DataContext = GetItemDataContext(e) };
        }

        private ItemViewModel GetItemDataContext(LoopingListDataItemEventArgs e)
        {
            ItemViewModel itemVm = null;
            try
            {
                var el = _data.Groups.FirstOrDefault(g => g.Id == CurrentGroupId).Items.ElementAt(e.Index);
                itemVm = new ItemViewModel(el);
            }
            catch (Exception)
            {
                itemVm = new ItemViewModel() { ImageSource = new Uri("../Assets/ko.jpg", UriKind.RelativeOrAbsolute) };
            }
            return itemVm;
        }
        #endregion

        #region groups
        private GroupViewModel GetGroupDataContext(LoopingListDataItemEventArgs e)
        {
            GroupViewModel groupVm = null;
            try
            {
                var el = _data.Groups.ElementAt(e.Index);
                groupVm = new GroupViewModel(el);
            }
            catch (Exception)
            {
                groupVm = new GroupViewModel() { ImageSource = new Uri("../Assets/ko.jpg", UriKind.RelativeOrAbsolute) };
            }
            return groupVm;
        }

        void ds_ItemUpdated_Groups(object sender, LoopingListDataItemEventArgs e)
        {
            (e.Item as PictureLoopingItem).DataContext = GetGroupDataContext(e);
        }

        void ds_ItemNeeded_Groups(object sender, LoopingListDataItemEventArgs e)
        {
            e.Item = new PictureLoopingItem()
            {
                DataContext = GetGroupDataContext(e)
            };
        }
        #endregion
        #endregion

        #region gallery mode

        private ItemViewModel GetUnlockedItemsDataContext(LoopingListDataItemEventArgs e)
        {
            ItemViewModel itemVm = null;
            try
            {
                var el = _data.Groups.SelectMany(g => g.Items).Where(it => !it.IsLocked).ElementAt(e.Index);
                itemVm = new ItemViewModel(el);
            }
            catch (Exception)
            {
                itemVm = new ItemViewModel() { ImageSource = new Uri("../Assets/ko.jpg", UriKind.RelativeOrAbsolute) };
            }
            return itemVm;
        }

        void _designDataDs_ItemsUnlocked_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            (e.Item as PictureLoopingItem).DataContext = GetUnlockedItemsDataContext(e);
        }

        void _designDataDs_ItemsUnlocked_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            e.Item = new PictureLoopingItem() { DataContext = GetUnlockedItemsDataContext(e) };
        }
        #endregion

        #endregion

        #region result popup
        private int _percentage = 0;
        public int Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                OnPropertyChanged("Percentage");
            }
        }

        private PageOrientation _PageOrientation = PageOrientation.LandscapeLeft;
        public PageOrientation PageOrientation
        {
            get
            {
                return _PageOrientation;
            }
            set
            {
                _PageOrientation = value;
                OnPropertyChanged("PageOrientation");
            }
        }
        #endregion

        static DesignData()
        {
        }

        public DesignData()
        {
            if (!IsInDesignModeStatic)
            {
                return;
            }
            Init();
        }

        private void Init()
        {
            var instance = AppDataManager.GetInstance("design");
            _data = instance.CfgData;

            #region arcade mode
            //groups selector
            _designDataDs_Groups.ItemNeeded += ds_ItemNeeded_Groups;
            _designDataDs_Groups.ItemUpdated += ds_ItemUpdated_Groups;
            //items selector
            _designDataDs_Items.ItemNeeded += _designDataDs_Items_ItemNeeded;
            _designDataDs_Items.ItemUpdated += _designDataDs_Items_ItemUpdated;
            #endregion

            #region gallery mode
            _designDataDs_ItemsUnlocked.ItemNeeded += _designDataDs_ItemsUnlocked_ItemNeeded;
            _designDataDs_ItemsUnlocked.ItemUpdated += _designDataDs_ItemsUnlocked_ItemUpdated;
            #endregion

            //result popup
            Percentage = 100;
            PageOrientation = Microsoft.Phone.Controls.PageOrientation.LandscapeLeft;

        }
    }

}
