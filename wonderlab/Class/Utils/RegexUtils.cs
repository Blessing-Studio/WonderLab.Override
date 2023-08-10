using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils {
    public class RegexUtils {
        public static Regex EmailCheck { get; } = new(@"^[a-zA-Z0-9_.-]+@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(com|cn|net)$");

        public static Regex Int32Check { get; } = new(@"^[0-9]+$");
    }
}
