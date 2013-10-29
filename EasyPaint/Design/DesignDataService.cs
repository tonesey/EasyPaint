using EasyPaint.Data;
using EasyPaint.Model;
using System;

namespace EasyPaint.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<AppData, Exception> callback)
        {
            // Use this to create design time data
            var item = AppData.GetInstance("design-time data");
            callback(item, null);
        }
    }
}