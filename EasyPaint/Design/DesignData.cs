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
using Telerik.Windows.Controls;

namespace EasyPaint.Design
{
    public enum ListType
    {
        Group,
        Item
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

        ///private List<string> _sampleImagesFilenames = new List<string> { "canguro colore", "coccodrillo colore" };

        private GroupViewModel _currentGroup = null;
        public GroupViewModel CurrentGroup
        {
            get
            {
                //Group g = new Group();
                //g.Id = CurrentGroupId;
                _currentGroup = new GroupViewModel(_data.Groups.FirstOrDefault(g => g.Id == CurrentGroupId));
                return _currentGroup;
            }
        }

        LoopingListDataSource _designDataDs_Groups = new LoopingListDataSource(2);
        LoopingListDataSource _designDataDs_Items = new LoopingListDataSource(2);

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
                }
                return null;
            }
        }

        #region items
        void _designDataDs_Items_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            Uri uri = null;
            string text = "none";
            try
            {
                // var imgFileName = _sampleImagesFilenames[e.Index] + ".png";
                var el = _data.Groups.FirstOrDefault(g => g.Id == CurrentGroupId).Items.ElementAt(e.Index);
                string imgFileName = el.ImgFilename;
                uri = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + CurrentGroupId + "/" + imgFileName, UriKind.RelativeOrAbsolute);
                text = el.Key + " " + LocalizedResources.ResourceManager.GetString(el.Key);
            }
            catch (Exception)
            {
                uri = new Uri("../Assets/ko.jpg", UriKind.RelativeOrAbsolute);
            }

            (e.Item as PictureLoopingItem).Picture = uri;
            (e.Item as PictureLoopingItem).Text = text;
        }

        void _designDataDs_Items_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            Uri uri = null;
            string text = "none";
            try
            {
                var el = _data.Groups.FirstOrDefault(g => g.Id == CurrentGroupId).Items.ElementAt(e.Index);
                string imgFileName = el.ImgFilename;
                uri = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + CurrentGroupId + "/" + imgFileName, UriKind.RelativeOrAbsolute);
                text = el.Key + " " + LocalizedResources.ResourceManager.GetString(el.Key);
            }
            catch (Exception)
            {
                uri = new Uri("../Assets/ko.jpg", UriKind.RelativeOrAbsolute);
            }

            e.Item = new PictureLoopingItem() { Picture = uri, Text = text };
        }
        #endregion

        #region groups
        void ds_ItemUpdated_Groups(object sender, LoopingListDataItemEventArgs e)
        {
            var imgFileName = e.Index + ".png";
            (e.Item as PictureLoopingItem).Picture = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + imgFileName, UriKind.RelativeOrAbsolute);
            (e.Item as PictureLoopingItem).Text = _data.Groups.ElementAt(e.Index).Key;
        }

        void ds_ItemNeeded_Groups(object sender, LoopingListDataItemEventArgs e)
        {
            var imgFileName = e.Index + ".png";
            e.Item = new PictureLoopingItem() { 
                Picture = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + imgFileName, UriKind.RelativeOrAbsolute),
                Text = _data.Groups.ElementAt(e.Index).Key
            };
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

        public DesignData()
        {
            if (!IsInDesignModeStatic)
            { 
                return;
            }

            _data = AppDataManager.GetInstance("design").CfgData;

            //groups selector
            _designDataDs_Groups.ItemNeeded += ds_ItemNeeded_Groups;
            _designDataDs_Groups.ItemUpdated += ds_ItemUpdated_Groups;

            //items selector
            _designDataDs_Items.ItemNeeded += _designDataDs_Items_ItemNeeded;
            _designDataDs_Items.ItemUpdated += _designDataDs_Items_ItemUpdated;

            //result popup
            Percentage = 100;
            PageOrientation = Microsoft.Phone.Controls.PageOrientation.LandscapeLeft;
        }




    }

}
