using EasyPaint.Model;
using EasyPaint.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class GroupViewModel : ImageAndTextItem
    {
        private Group _group;
        public Group Group
        {
            get { return _group; }
        }

        //public List<ItemViewModel> _items = null;
        public ObservableCollection<ItemViewModel> Items { get; set; }

        public bool IsLocked
        {
            get
            {
//#if DEBUG
//                return false;
//#endif
                bool isLocked = !GroupHasAtLeastOneItemUnlocked();
                return isLocked;
            }
        }

        private bool GroupHasAtLeastOneItemUnlocked()
        {
            //return _group.Items.Where(i => i.Score >= Item.MINIMUM_UNLOCK_PERCENTAGE_REQUIRED).Count() >= 1;
            return _group.Items.Where(i => !i.IsLocked).Count() >= 1;
        }

        protected Uri _bckImage;
        public Uri BckImage
        {
            get
            {
                return _bckImage;
            }
            set
            {
                if (_bckImage != value)
                {
                    _bckImage = value;
                    this.OnPropertyChanged("BckImage");
                }
            }
        }

        #region protagonist image
        protected Uri _ProtagonistImage;
        public Uri ProtagonistImage
        {
            get
            {
                return _ProtagonistImage;
            }
            set
            {
                if (_ProtagonistImage != value)
                {
                    _ProtagonistImage = value;
                    this.OnPropertyChanged("ProtagonistImage");
                }
            }
        }


        public string ProtagonistImageName { get; set; }
        #endregion


        public GroupViewModel()
        {
        }

        public GroupViewModel(Group gr)
        {
            try
            {
                _group = gr;
                _id = gr.Id;
                _key = gr.Key;

                ImageSource = new Uri(string.Format("../Assets/{0}/groups/{1}.png", AppSettings.AppRes, _group.Id), UriKind.RelativeOrAbsolute);
                BckImage = new Uri(string.Format("../Assets/{0}/groups/{1}_bck.jpg", AppSettings.AppRes, _group.Id), UriKind.RelativeOrAbsolute);
                ProtagonistImage = new Uri(string.Format("../Assets/{0}/groups/{1}/{2}", new string[] { AppSettings.AppRes, _group.Id, _group.ProtagonistImageName }), UriKind.RelativeOrAbsolute);

                Items = new ObservableCollection<ItemViewModel>();
                foreach (var item in gr.Items)
                {
                    //ItemViewModel itemVm = new ItemViewModel(gr, item);
                    ItemViewModel itemVm = new ItemViewModel(item);
                    Items.Add(itemVm);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        


    }
}

