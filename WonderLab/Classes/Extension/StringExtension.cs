using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonderLab.Classes.Extension {
    public static class StringExtension {
        public static int ToInt(this string text) {
            return Convert.ToInt32(text);
        }
    }
}
