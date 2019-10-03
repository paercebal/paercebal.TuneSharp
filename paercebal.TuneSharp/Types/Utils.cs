using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace paercebal.TuneSharp.Types
{
    public class Utils
    {
        public static T Clamp<T>(T value, T min, T max)
            where T : IComparable<T>
        {
            if (Comparer<T>.Default.Compare(value, min) < 0)
                return min;
            if (Comparer<T>.Default.Compare(value, max) > 0)
                return max;

            return value;
        }
    }
}
