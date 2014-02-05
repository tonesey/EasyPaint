using EasyPaint.Data;
using EasyPaint.Model;
using System;
using System.Threading.Tasks;

namespace EasyPaint.Design
{
    public class DesignDataService : IDataService
    {
        public async Task GetDataAsync(Action<AppDataManager, Exception> callback)
        {
            // Use this to create design time data
            var item = await AppDataManager.GetInstanceAsync("design");
            callback(item, null);
        }

        public void GetData(Action<AppDataManager, Exception> callback)
        {
            // Use this to create design time data
            var item = AppDataManager.GetInstance("design");
            callback(item, null);
        }
    }
}