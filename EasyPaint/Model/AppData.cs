using EasyPaint.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public class AppData
    {
        private static AppData _curInstance = null;

        private string _tag;
        private List<Group> _groups;

        public List<Group> Groups
        {
            get { return _groups; }
        }

        public AppData(string tag, List<Group> groups)
        {
            this._tag = tag;
            this._groups = groups;

            if (tag.Contains("design"))
            {
                //aggiungere qua altri dati cablati per fare le prove a design time
            }
        }

        internal static AppData GetInstance(string tag)
        {
            if (_curInstance == null)
            {
                _curInstance = new AppData(tag, ModelHelper.BuildDataFromCfg());
            }
            return _curInstance;
        }
    }
}
