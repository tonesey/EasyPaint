using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Data
{
    public class CfgData
    {
        private List<Group> _groups;
        public List<Group> Groups
        {
            get { return _groups; }
            set { _groups = value; }
        }

        public string UIMode { get; set; }
    }
}
