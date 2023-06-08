using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using MinecraftLaunch.Modules.Toolkits;
using MinecraftLaunch.Modules.Models.Launch;
using System.Runtime.Versioning;

namespace wonderlab.Class.Utils {
    /// <summary>
    /// Java 工具类，针对 Windows 以外的系统
    /// </summary>
    [SupportedOSPlatform("OSX")]
    [SupportedOSPlatform("LINUX")]
    public class JavaUtils {        
        public static async IAsyncEnumerable<JavaInfo> GetJavas() {
            var result = await Task.Run(() => {
                var cache = new List<JavaInfo>();

                if (SystemUtils.IsMacOS) {
                    cache.AddRange(GetMacJavas());
                } else if (SystemUtils.IsLinux) {
                    cache.AddRange(GetLinuxJavas());
                }

                cache.AddRange(GetJavaInOfficialGameCorePath());
                return cache;
            });

            foreach (var java in result) {
                yield return java;
            }
        }

        private static IEnumerable<JavaInfo> GetMacJavas() {
            foreach (var i in Directory.EnumerateDirectories(GlobalResources.MacJavaHomePath).AsParallel()) {
                if (!Directory.Exists(i + "/Contents/Home/bin"))
                    continue;

                if ($"{i}/Contents/Home/bin/java".IsFile()) {
                    yield return JavaToolkit.GetJavaInfo(i);
                }
            }
        }

        private static IEnumerable<JavaInfo> GetLinuxJavas() {
            return Array.Empty<JavaInfo>();
        }

        private static IEnumerable<JavaInfo> GetJavaInOfficialGameCorePath() {
            var paths = new[] { "java-runtime-alpha", "java-runtime-beta", "jre-legacy" };

            return paths.Select(path => Path.Combine(GameCoreUtils.GetOfficialGameCorePath().FullName, path, "bin", "java"))
                .Where(File.Exists)
                .Select(JavaToolkit.GetJavaInfo);
        }
    }
}
