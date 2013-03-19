using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public class Character
    {
        public string Id { get; set; }
        public string FriendlyName { get; set; }
        public List<string> Cultures { get; set; }
        public List<MyPicture> Pics { get; set; }

        public Character() {
            Id = string.Empty;
            Cultures = new List<string>();
            Pics = new List<MyPicture>();
        }

        public override string ToString()
        {
            return Id;
        }

      
    }
}
