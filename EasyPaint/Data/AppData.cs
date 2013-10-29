using EasyPaint.Helpers;
using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Data
{
    public class AppData
    {
        private static AppData _curInstance = null;

        private string _tag;

        private CfgData _cfgData;
        public CfgData CfgData
        {
            get { return _cfgData; }
        }

        private UserData _userData;
        public UserData UserData
        {
            get { return _userData; }
        }

        public AppData(string tag, CfgData cfgData, UserData userData)
        {
            this._tag = tag;
           //_groups = data.Groups;
            _cfgData = cfgData;
            _userData = userData;

            if (tag.Contains("design"))
            {
                //aggiungere qua altri dati cablati per fare le prove a design time
            }
        }

        internal static AppData GetInstance(string tag)
        {
            if (_curInstance == null)
            {
                _curInstance = new AppData(tag, ModelHelper.BuildDataFromCfg(), new UserData("TODO"));
            }
            return _curInstance;
        }
    }
}
