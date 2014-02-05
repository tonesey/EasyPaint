using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaint.Data
{
    public interface IDataService
    {
        Task GetDataAsync(Action<AppDataManager, Exception> callback);
        void GetData(Action<AppDataManager, Exception> callback);
    }
}
