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

        public GroupViewModel(Group gr)
        {
            try
            {
                _group = gr;
                _id = gr.Id;
                _key = gr.Key;

                ImageSource = new Uri(string.Format("../Assets/groups/" + AppSettings.AppRes + "/{0}.png", _group.Id), UriKind.RelativeOrAbsolute);
                //ImageSource = new Uri(string.Format("EasyPaint;component/Assets/groups/" + AppSettings.AppRes + "/{0}.png", _group.Id), UriKind.RelativeOrAbsolute);
            }
            catch (Exception)
            {
                throw;
            }

            
        }
    }
}

