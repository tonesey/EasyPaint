using EasyPaint.Data;
using System;
using System.Threading.Tasks;

namespace EasyPaint.Data
{
    public class DataService : IDataService
    {
        public async Task GetDataAsync(Action<AppDataManager, Exception> callback)
        {
            var item = await AppDataManager.GetInstanceAsync(); //"runtime data"
            callback(item, null);
        }

        public void GetData(Action<AppDataManager, Exception> callback)
        {
            var item = AppDataManager.GetInstance(); //"runtime data"
            callback(item, null);
        }
    }
}