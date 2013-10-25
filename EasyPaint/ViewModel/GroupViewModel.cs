using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class GroupViewModel : MyViewModelBase
    {
        private Group _group;

        public string Id
        {
            get
            {
                return _group.Id;
            }
        }

        private Uri _imageSource;
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
                    this.OnPropertyChanged("ImageSource");
                }
            }
        }

        public string FriendlyName
        {
            get
            {
                return LocalizedResources.ResourceManager.GetString(_group.Key);
            }
        }

        //private ObservableCollection<ItemViewModel> _items;
        //public ObservableCollection<ItemViewModel> Items
        //{
        //    get
        //    {
        //        return this._items;
        //    }
        //    private set
        //    {
        //        this._items = value;
        //    }
        //}

        public GroupViewModel(Group c)
        {
            _group = c;
            ImageSource = new Uri(string.Format("../Assets/groups/{0}.png", _group.Id), UriKind.RelativeOrAbsolute);
            //this.InitializeItems();
        }


        //private void InitializeItems()
        //{
        //    this._items = new ObservableCollection<ItemViewModel>();
        //    foreach (var item in _group.Items)
        //    {
        //        this._items.Add(new ItemViewModel()
        //        {
        //            Id = _group.Id,
        //            ImgFilename = item.ImgFilename,
        //            ImageSource = new Uri(string.Format("Assets/groups/{0}/{1}", _group.Id, item.ImgFilename), UriKind.RelativeOrAbsolute),
        //            //ImageThumbnailSource = new Uri("Images/FrameThumbnail.png", UriKind.RelativeOrAbsolute),
        //            //Title = "Title " + i,
        //            //Information = "Information " + i,
        //            //Group = (i % 2 == 0) ? "EVEN" : "ODD"
        //        });
        //    }
        //    OnPropertyChanged("Items");
        //}
    }
}
