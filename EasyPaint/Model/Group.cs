using System;
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

        //public string FriendlyName { get; set; }

        public List<Item> Items { get; set; }

        public Group()
        {
            Id = string.Empty;
            Items = new List<Item>();
        }

        public override string ToString()
        {
            return Id;
        }

    }
}
