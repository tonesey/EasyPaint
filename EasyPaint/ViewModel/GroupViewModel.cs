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

        public GroupViewModel(Group gr)
        {
            try
            {
                _group = gr;
                _id = gr.Id;
                _key = gr.Key;

                ImageSource = new Uri(string.Format("../Assets/{0}/groups/{1}.png", AppSettings.AppRes, _group.Id), UriKind.RelativeOrAbsolute);
                //BckImage = new Uri(string.Format("EasyPaint;component/Assets/groups/" + AppSettings.AppRes + "/{0}_bck.png", _group.Id), UriKind.RelativeOrAbsolute);
                BckImage = new Uri(string.Format("../Assets/{0}/groups/{1}_bck.png", AppSettings.AppRes, _group.Id), UriKind.RelativeOrAbsolute);
                //ImageSource = new Uri(string.Format("EasyPaint;component/Assets/groups/" + AppSettings.AppRes + "/{0}.png", _group.Id), UriKind.RelativeOrAbsolute);
            }
            catch (Exception)
            {
                throw;
            }

            
        }
    }
}

