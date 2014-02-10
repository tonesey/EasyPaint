using EasyPaint.Helpers;
using EasyPaint.Model;
using EasyPaint.Settings;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Windows.Storage;
using Wp8Shared.Utility;

namespace EasyPaint.ViewModel
{
    public class ItemViewModel : ImageAndTextItem
    {
        private Item _item;

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

        public bool IsLocked
        {
            get
            {
//#if DEBUG
//                return false;
//#endif
                return _item.IsLocked;
            }
            set
            {
                if (_item.IsLocked != value)
                {
                    _item.IsLocked = value;
                    this.OnPropertyChanged("IsLocked");
                    this.OnPropertyChanged("ImageSource");
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

        public string ParentGroupId
        {
            get
            {
                return _item.ParentGroup.Id;
            }
        }

        public string ParentGroupName
        {
            get
            {
                return LocalizedResources.ResourceManager.GetString(_item.ParentGroup.Key);
            }
        }

        public bool ParentGroupRequiresLicense
        {
            get
            {
                return _item.ParentGroup.LicenseRequired;
            }
        }

        public ItemViewModel()
        {
        }

        public ItemViewModel(Item item)
        {
            _item = item;
            _key = item.Key;

            //full colors
            //ImageSource = new Uri(string.Format("../Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename }), UriKind.RelativeOrAbsolute);
            //reduced colors
            ImageSource = new Uri(string.Format("../Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename }), UriKind.RelativeOrAbsolute);
            ImageSourceHidden = new Uri(string.Format("../Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename.Replace("colore", "hidden") }), UriKind.RelativeOrAbsolute);

            //---reduced colors
            ReducedColorsResourcePath = string.Format("Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename });
            ReducedColorLineArtResourcePath = string.Format("Assets/{0}/groups/{1}/reduced_10/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename.Replace("colore", "lineart") });
            //---full color
            //ReducedColorsResourcePath = string.Format("Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename });
            //ReducedColorLineArtResourcePath = string.Format("Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, item.ParentGroup.Id, item.ImgFilename.Replace("colore", "lineart") });

            ////HIDDEN IMAGE
            //WriteableBitmap hiddenImage = BitmapFactory.New(400, 400).FromResource(ReducedColorsResourcePath);
            //using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            //{
            //    string fileName = "hidden_" + item.ImgFilename;
            //    if (myIsolatedStorage.FileExists(fileName))
            //    {
            //        myIsolatedStorage.DeleteFile(fileName);
            //    }
            //    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName);
            //    Extensions.SaveJpeg(hiddenImage, fileStream, hiddenImage.PixelWidth, hiddenImage.PixelHeight, 0, 100);
            //    fileStream.Close();
            //    GetImageSourceFromIsoStore(fileName);
            //}

#if COLORSCHECK
            //if (_item.ParentGroup.Id == "0")
            //{
            WriteableBitmap bmpBase = BitmapFactory.New(400, 400).FromResource(ReducedColorsResourcePath);
            WriteableBitmap bmpLineart = BitmapFactory.New(400, 400).FromResource(ReducedColorLineArtResourcePath);

            //elimina la lineart dall'immagine base, per poter calcolare la % di copertura dei colori definiti nella palette
            //bmpBase.Blit(new Rect(0, 0, bmpBase.PixelWidth, bmpBase.PixelHeight),
            //             bmpLineart,
            //             new Rect(0, 0, bmpBase.PixelWidth, bmpBase.PixelHeight),
            //             WriteableBitmapExtensions.BlendMode.Subtractive);

            WriteableBitmap _checkImage = ImagesHelper.CheckPaletteCoverage(bmpBase, bmpLineart, item.PaletteColors, out _coverage);
            //WriteableBitmap _checkImage = bmpBase;

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string fileName = "checkimage_" + _key + ".jpg";
                if (myIsolatedStorage.FileExists(fileName))
                {
                    myIsolatedStorage.DeleteFile(fileName);
                }
                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(fileName);
                Extensions.SaveJpeg(_checkImage, fileStream, _checkImage.PixelWidth, _checkImage.PixelHeight, 0, 100);
                fileStream.Close();
                GetImageSourceFromIsoStore(fileName);
            }
            //}
#endif
        }

       

        public async void GetImageSourceFromIsoStore(string filename)
        {
            Windows.Storage.StorageFile storageFile = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(filename);
            ImageSource = new Uri(GenericUtility.GetIsolatedStorageFullImagePath(storageFile));
        }


#if COLORSCHECK
        //protected WriteableBitmap _checkImage;
        //public WriteableBitmap CheckImage
        //{
        //    get
        //    {
        //        return _checkImage;
        //    }
        //    set
        //    {
        //        if (_checkImage != value)
        //        {
        //            _checkImage = value;
        //            OnPropertyChanged("CheckImage");
        //        }
        //    }
        //}

        protected int _coverage;
        public int PaletteCoverage
        {
            get
            {
                return _coverage;
            }
            set
            {
                if (_coverage != value)
                {
                    _coverage = value;
                    OnPropertyChanged("PaletteCoverage");
                }
            }
        }
#endif

        internal void SetScore(int value)
        {
            _item.Score = value;
            if (value > _item.RecordScore)
            {
                _item.RecordScore = value; // new record!
            }
        }

        public List<Color> PaletteColors { get { return _item.PaletteColors; } }

        public override string ToString()
        {
            if (_item != null) return _item.ToString();
            return "empty element";
        }
    }
}
