using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils {
    public static class SystemUtils {
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsWindows11 => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static string GetPlatformName() {
            if (IsWindows) {
                return "Windows";
            } else if (IsLinux) {
                return "Linux";
            } else {
                return "MacOS";
            }
        }
    }
}
