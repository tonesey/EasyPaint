using System;

namespace EasyPaint.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<AppData, Exception> callback)
        {
            var item = AppData.GetInstance("runtime data");
            callback(item, null);
        }
    }
}