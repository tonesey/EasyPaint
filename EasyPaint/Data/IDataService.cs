using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Data
{
    public interface IDataService
    {
        void GetData(Action<AppDataManager, Exception> callback);
    }
}
