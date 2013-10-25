using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPaint.Model
{
    public interface IDataService
    {
        void GetData(Action<AppData, Exception> callback);
    }
}
