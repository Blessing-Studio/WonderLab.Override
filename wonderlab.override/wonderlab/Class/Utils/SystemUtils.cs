using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class SystemUtils {
        public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsWindows11 => (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.OSVersion.Version.Build >= 22000);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        [Obsolete]//屁玩意啥用没有
        public static T TryRun<T>(Func<T> action) {
            try {
                return action();
            }
            catch (Exception ex) {
                "焯，这辣鸡启动器又炸了".ShowMessage("错误");
            }

            return default!;
        }
    }
}
