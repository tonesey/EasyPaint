using EasyPaint.Helpers;
using EasyPaint.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public AppDataManager(string tag)
        {
            this._tag = tag;
        }

        public async Task BuildDataAsync()
        {
            _appData = await ModelHelper.BuildAppDataAsync();
            if (_tag.Contains("design"))
            {
                //creare / aggiungere qua altri dati cablati per fare le prove a design time
            }
        }

        public void BuildData()
        {
            _appData = ModelHelper.BuildAppData();
            if (_tag.Contains("design"))
            {
                //creare / aggiungere qua altri dati cablati per fare le prove a design time

                //sblocco di alcuni animali per modalità gallery
                _appData.Groups.ElementAt(0).Items.ElementAt(1).IsLocked  =true;
                _appData.Groups.ElementAt(0).Items.ElementAt(2).IsLocked = false;
                _appData.Groups.ElementAt(0).Items.ElementAt(3).IsLocked = false;

                _appData.Groups.ElementAt(1).Items.ElementAt(0).IsLocked = false;
                _appData.Groups.ElementAt(1).Items.ElementAt(1).IsLocked = false;
            }
        }

        public string GetUserScoreStrValue()
        {
            return ModelHelper.GetUserScoreValue(_appData);
        }

        internal long GetTotalPoints()
        {
            return _appData.Groups.SelectMany(g => g.Items).Sum(item => item.Score);
        }

        internal static async Task<AppDataManager> GetInstanceAsync()
        {
            return await GetInstanceAsync(string.Empty);
        }

        internal static async Task<AppDataManager> GetInstanceAsync(string tag)
        {
            if (_curInstance == null)
            {
                _curInstance = new AppDataManager(tag);
                await _curInstance.BuildDataAsync();
            }
            return _curInstance;
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
                _curInstance.BuildData();
            }
            return _curInstance;
        }

        
    }
}
