using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Telerik.Windows.Controls;

namespace EasyPaint.ViewModel
{
    public class CharacterViewModel : MyViewModelBase
    {
        private Character _character;

        private ObservableCollection<PicViewModel> _charPics;
        public ObservableCollection<PicViewModel> Pics
        {
            get
            {
                return this._charPics;
            }
            private set
            {
                this._charPics = value;
            }
        }

        public CharacterViewModel(Character c)
        {
            _character = c;
            this.InitializePics();
        }

        public string Id
        {
            get
            {
                return _character.Id;
            }
        }

        public string FriendlyName
        {
            get
            {
                return LocalizedResources.ResourceManager.GetString(_character.Id);
            }
        }

        public Uri CharThumbnailSource
        {
            get
            {
                //return new Uri(string.Format("Assets/Packages/{0}/thumb.png", _character.Id), UriKind.RelativeOrAbsolute);
                return new Uri(string.Format("Assets/packages/{0}/c1.png", _character.Id), UriKind.RelativeOrAbsolute);
            }
        }


        public List<string> Cultures
        {
            get
            {
                return _character.Cultures;
            }
        }


        private void InitializePics()
        {
            this._charPics = new ObservableCollection<PicViewModel>();
            foreach (var pic in _character.Pics)
            {
                this._charPics.Add(new PicViewModel()
                {
                    CharacterId = _character.Id,
                    FileName = pic.FileName,
                    ImageSource = new Uri(string.Format("Assets/packages/{0}/{1}", _character.Id, pic.FileName), UriKind.RelativeOrAbsolute),
                    ImageThumbnailSource = new Uri(string.Format("Assets/packages/{0}/{1}", _character.Id, pic.FileName), UriKind.RelativeOrAbsolute)
                    //ImageThumbnailSource = new Uri("Images/FrameThumbnail.png", UriKind.RelativeOrAbsolute),
                    //Title = "Title " + i,
                    //Information = "Information " + i,
                    //Group = (i % 2 == 0) ? "EVEN" : "ODD"
                });
            }
            OnPropertyChanged("Pics");
        }
    }
}
