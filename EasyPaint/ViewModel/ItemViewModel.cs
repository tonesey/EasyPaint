using EasyPaint.Model;
using EasyPaint.Settings;
using System;
using System.Collections.Generic;
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
#if DEBUG
                return false;
#endif
                return _item.IsLocked;
            }
            set
            {
                if (_item.IsLocked != value)
                {
                    _item.IsLocked = value;
                    this.OnPropertyChanged("IsLocked");
                }
            }
        }

        public string LatinName
        {
            get
            {
                return _item.LatinName;
            }
        }

        protected string _reducedColorlineArtResourcePath;
        public string ReducedColorLineArtResourcePath
        {
            get
            {
                return _reducedColorlineArtResourcePath;
            }
            set
            {
                if (_reducedColorlineArtResourcePath != value)
                {
                    _reducedColorlineArtResourcePath = value;
                    this.OnPropertyChanged("ReducedColorLineArtResourcePath");
                }
            }
        }

        public ItemViewModel(Group g, Item item)
        {
            _item = item;
            _belongingGroup = g;
            _key = item.Key;

            //full colors
            ImageSource = new Uri(string.Format("../Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, _belongingGroup.Id, item.ImgFilename }), UriKind.RelativeOrAbsolute);
           
            //reduced colors
            ReducedColorsResourcePath = string.Format("Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, _belongingGroup.Id, item.ImgFilename });
            ReducedColorLineArtResourcePath = string.Format("Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, _belongingGroup.Id, item.ImgFilename.Replace("colore", "lineart") });
          
           


        }

        internal void SetScore(int value)
        {
            _item.Score = value;
            if (value > _item.RecordScore)
            {
                _item.RecordScore = value; // new record!
            }
        }

        public List<Color> PaletteColors { get { return _item.PaletteColors ; } }
    }
}
