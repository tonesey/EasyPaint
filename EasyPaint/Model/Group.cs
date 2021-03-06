﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public class Group
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string ImgFilename { get; set; }
        public bool LicenseRequired { get; set; }

        //public string FriendlyName { get; set; }

        public List<Item> Items { get; set; }

        public Group()
        {
            Id = "not set";
            Items = new List<Item>();

            GridRow = 0;
            GridCol = 0;
            GridRowSpan = 1;
            GridColumnSpan = 1;

            SelectorGridRow = 1;

            IsSelectorCentered = true;

            LicenseRequired = false;
        }

        public override string ToString()
        {
            return Id;
        }


        public bool IsSelectorCentered { get; set; }

        #region protagonist image
        public string ProtagonistImageName { get; set; }

        public int GridRow { get; set; }

        public int GridCol { get; set; }

        public int GridRowSpan { get; set; }

        public int GridColumnSpan { get; set; }
        #endregion

        #region selector UI properties
        public int SelectorGridRow { get; set; }
        #endregion

       
    }
}
