using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using MinecraftLaunch.Modules.Models.Launch;
using System.Runtime.Versioning;
using MinecraftLaunch.Modules.Utils;

namespace wonderlab.Class.Utils {
    /// <summary>
    /// Java 工具类，针对 Windows 以外的系统
    /// </summary>
    [SupportedOSPlatform("OSX")]
    [SupportedOSPlatform("LINUX")]
    [Obsolete("由 MinecraftLaunch 所替代")]
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
                    yield return JavaUtil.GetJavaInfo($"{i}/Contents/Home/bin/java");
                }
            }
        }

        private static IEnumerable<JavaInfo> GetLinuxJavas() {
            //包管理器目录下已安装的java
            foreach (var LinuxJavaHomePath in GlobalResources.LinuxJavaHomePaths.AsParallel()) {
                if (!Directory.Exists(LinuxJavaHomePath))
                    continue;
                foreach (var jvmPath in Directory.EnumerateDirectories(LinuxJavaHomePath).AsParallel())
                    if ($"{jvmPath}/bin/java".IsFile())
                        yield return JavaUtil.GetJavaInfo($"{jvmPath}/bin/java");
            }
            //设置了环境变量的java
            using var cmd = new System.Diagnostics.Process {
                StartInfo = new("which", "java") {
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                }
            };
            cmd.Start();
            var envJvmPath = cmd.StandardError.ReadToEnd();
            cmd.Close();
            if (envJvmPath.IsFile())
                yield return JavaUtil.GetJavaInfo(envJvmPath);
        }

        private static IEnumerable<JavaInfo> GetJavaInOfficialGameCorePath() {
            var paths = new[] { "java-runtime-alpha", "java-runtime-beta", "jre-legacy" };

            return paths.Select(path => Path.Combine(GameCoreUtils.GetOfficialGameCorePath().FullName, path, "bin", "java"))
                .Where(File.Exists)
                .Select(JavaUtil.GetJavaInfo);
        }
    }
}
