using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace EasyPaint.ViewModel
{
    public class ImageAndTextItem : AppViewModel
    {
        protected string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set {
                _id = value;
            }
        }

        protected string _key;
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        protected Uri _imageSource;
        public Uri ImageSource
        {
            get
            {
                return this._imageSource;
            }
            set
            {
                if (this._imageSource != value)
                {
                    this._imageSource = value;
                    OnPropertyChanged("ImageSource");
                }
            }
        }


        public string LocalizedName
        {
            get
            {
                return LocalizedResources.ResourceManager.GetString(Key);
            }
        }

      


    }
}
