﻿using EasyPaint.Model;
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

    public class DesignData : MyViewModelBase
    {
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

        private List<string> _sampleImagesFilenames = new List<string> { "canguro colore", "coccodrillo colore" };

        private GroupViewModel _currentGroup = null;
        public GroupViewModel CurrentGroup
        {
            get
            {
                Group g = new Group();
                g.Id = CurrentGroupId;
                _currentGroup = new GroupViewModel(g);
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
            (e.Item as PictureLoopingItem).Picture = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + CurrentGroupId + "/" + _sampleImagesFilenames[e.Index] + ".png", UriKind.RelativeOrAbsolute);
        }

        void _designDataDs_Items_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            e.Item = new PictureLoopingItem() { Picture = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + CurrentGroupId + "/" + _sampleImagesFilenames[e.Index] + ".png", UriKind.RelativeOrAbsolute) };
        }
        #endregion

        #region groups
        void ds_ItemUpdated_Groups(object sender, LoopingListDataItemEventArgs e)
        {
            (e.Item as PictureLoopingItem).Picture = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + e.Index + ".png", UriKind.RelativeOrAbsolute);
        }

        void ds_ItemNeeded_Groups(object sender, LoopingListDataItemEventArgs e)
        {
            e.Item = new PictureLoopingItem() { Picture = new Uri("../Assets/" + AppSettings.AppRes + "/groups/" + e.Index + ".png", UriKind.RelativeOrAbsolute) };
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
