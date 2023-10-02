using System;
using System.Linq;
using System.Reflection;

namespace wonderlab.Class.Utils {
    public static class AssemblyUtil {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        public static string Version =>
            (Attribute.GetCustomAttribute(_assembly, typeof(AssemblyFileVersionAttribute), false)
            as AssemblyFileVersionAttribute).Version;

        public static int Build => int.Parse(Version.Split('.').Last());
    }
}
