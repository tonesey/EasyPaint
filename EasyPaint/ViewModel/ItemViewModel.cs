using EasyPaint.Model;
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

        protected Uri _resourceUri;
        public Uri ReducedColorsResourceUri
        {
            get
            {
                return this._resourceUri;
            }
            set
            {
                if (this._resourceUri != value)
                {
                    this._resourceUri = value;
                    this.OnPropertyChanged("ResourceUri");
                }
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
            ImageSource = new Uri(string.Format("../Assets/groups/{0}/" + AppSettings.AppRes + "/{1}", _belongingGroup.Id, item.ImgFilename), UriKind.RelativeOrAbsolute);
            ReducedColorsResourceUri = new Uri(string.Format("EasyPaint;component/Assets/groups/{0}/" + AppSettings.AppRes + "/reduced/{1}", _belongingGroup.Id, item.ImgFilename), UriKind.RelativeOrAbsolute);
            //LineArtResourceUri = new Uri(string.Format("EasyPaint;component/Assets/groups/{0}/" + AppSettings.AppRes + "/{1}", _belongingGroup.Id, item.ImgFilename.Replace("colore", "lineart")), UriKind.RelativeOrAbsolute);
            LineArtResourcePath = string.Format("/Assets/groups/{0}/" + AppSettings.AppRes + "/{1}", _belongingGroup.Id, item.ImgFilename.Replace("colore", "lineart"));
        }
    }
}
