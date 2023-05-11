using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public static class DataUtil {   
        public static void CheckNull(object? obj) {       
            if(obj == null) {           
                throw new ArgumentNullException(nameof(obj));
            }
        }

        public static Dictionary<string, WebModpackInfoModel> WebModpackInfoDatas { get; set; } = new();
    }
}
