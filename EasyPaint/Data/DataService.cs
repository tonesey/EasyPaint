using EasyPaint.Data;
using System;

namespace EasyPaint.Data
{
    public class DataService : IDataService
    {
        public void GetData(Action<AppDataManager, Exception> callback)
        {
            var item = AppDataManager.GetInstance("runtime data");
            callback(item, null);
        }
    }
}