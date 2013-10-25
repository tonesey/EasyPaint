﻿using EasyPaint.Model;
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

        public ItemViewModel(Group g, Item item)
        {
            _item = item;
            _belongingGroup = g;
            _key = item.Key;
            ImageSource = new Uri(string.Format("../Assets/groups/{0}/{1}", _belongingGroup.Id, item.ImgFilename), UriKind.RelativeOrAbsolute);
        }
    }
}
