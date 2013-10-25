using EasyPaint.Model;
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
   public class ItemViewModel : ViewModelBase
    {

        private Item _item;
        private Group _belongingGroup;

        public string ImgFilename { get; set; }
        public string Id { get; set; }

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

        public ItemViewModel(Group g, Item item)
        {
            _item = item;
            _belongingGroup = g;

            if (item != null) {
                ImgFilename = item.ImgFilename;
                ImageSource = new Uri(string.Format("../Assets/groups/{0}/{1}", _belongingGroup.Id, item.ImgFilename), UriKind.RelativeOrAbsolute);
            }
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
