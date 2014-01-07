using EasyPaint.Helpers;
using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Data
{
    public class AppDataManager
    {
        private static AppDataManager _curInstance = null;

        private string _tag;

        private AppData _appData;
        public AppData CfgData
        {
            get { return _appData; }
        }

        //private UserData _userData;
        //public UserData UserData
        //{
        //    get { return _userData; }
        //}

        //public AppData(string tag, CfgData cfgData, UserData userData)
        public AppDataManager(string tag)
        {
            this._tag = tag;
            //_groups = data.Groups;

            _appData = ModelHelper.BuildAppData();

            if (tag.Contains("design"))
            {
                //creare / aggiungere qua altri dati cablati per fare le prove a design time
            }
            //else
            //{
            //    _cfgData = ModelHelper.BuildAppData();
            //}
        }

        public string GetUserScoreStrValue()
        {
            return ModelHelper.GetUserScoreValue(_appData);
        }

        internal static AppDataManager GetInstance()
        {
            return GetInstance(string.Empty);
        }

        internal static AppDataManager GetInstance(string tag)
        {
            if (_curInstance == null)
            {
                _curInstance = new AppDataManager(tag);
            }
            return _curInstance;
        }
    }
}
