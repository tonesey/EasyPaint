using EasyPaint.Data;
using EasyPaint.Model;
using System;

namespace EasyPaint.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<AppDataManager, Exception> callback)
        {
            // Use this to create design time data
            var item = AppDataManager.GetInstance("design");
            callback(item, null);
        }
    }
}