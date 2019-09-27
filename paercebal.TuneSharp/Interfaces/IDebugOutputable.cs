using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paercebal.TuneSharp.Interfaces
{
    public interface IDebugOutputable
    {
        void Log(string key, string text);
        void Log(string key, string format, params object[] o);
    }
}
