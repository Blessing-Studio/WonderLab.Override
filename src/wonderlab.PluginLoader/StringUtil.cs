using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wonderlab.PluginLoader
{
    public static class StringUtil {   
        public static bool IsGuid(string strSrc) {       
            Regex reg = new Regex("^{[A-F0-9]{8}(-[A-F0-9]{4}){3}-[A-F0-9]{12}}$", RegexOptions.Compiled);
            return reg.IsMatch(strSrc);
        }

        public static string GetSubPath(string mainPath, string subPath) {       
            if (mainPath == null) {           
                return subPath;
            }
            
            if (subPath == null) {           
                return mainPath;
            }

            return Path.Combine(mainPath, subPath);
        }
    }
}
