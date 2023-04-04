using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class DataUtil
    {
        public static void CheckNull(object? obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }
    }
}
