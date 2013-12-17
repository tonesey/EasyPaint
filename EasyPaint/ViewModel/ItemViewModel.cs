﻿using EasyPaint.Model;
using EasyPaint.Settings;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class ItemViewModel : ImageAndTextItem
    {
        private Item _item;
        private Group _belongingGroup;

        protected string _resourcePath;
        public string ReducedColorsResourcePath
        {
            get
            {
                return this._resourcePath;
            }
            set
            {
                if (this._resourcePath != value)
                {
                    this._resourcePath = value;
                    this.OnPropertyChanged("ReducedColorsResourcePath");
                }
            }
        }

        public bool IsLocked
        {
            get
            {
                return _item.UserMaximumScore < Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED;
            }
        }

        protected string _lineArtResourcePath;
        public string LineArtResourcePath
        {
            get
            {
                return _lineArtResourcePath;
            }
            set
            {
                if (_lineArtResourcePath != value)
                {
                    _lineArtResourcePath = value;
                    this.OnPropertyChanged("LineArtResourceUri");
                }
            }
        }

        public ItemViewModel(Group g, Item item)
        {
            _item = item;
            _belongingGroup = g;
            _key = item.Key;
            ImageSource = new Uri(string.Format("../Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, _belongingGroup.Id, item.ImgFilename }), UriKind.RelativeOrAbsolute);
            LineArtResourcePath = string.Format("Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, _belongingGroup.Id, item.ImgFilename.Replace("colore", "lineart")});
            ReducedColorsResourcePath = string.Format("Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, _belongingGroup.Id, item.ImgFilename });
        }
    }
}
