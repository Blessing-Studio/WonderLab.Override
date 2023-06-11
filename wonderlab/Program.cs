using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab {
    internal class Program {
        [STAThread]
        public static async Task Main(string[] args) {
            try {
                //Environment.SetEnvironmentVariable("LC_CTYPE", "en_US.UTF-8");
                BuildAvaloniaApp()
                  .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e) {
                StringBuilder builder = new();
                builder.AppendLine("非常抱歉您的 WonderLab 又又又炸了，以下是此次崩溃的错误信息");
                builder.AppendLine("----------------------------------------------------------------------");
                builder.AppendLine($"系统平台：{SystemUtils.GetPlatformName()}");
                builder.AppendLine($"异常名：{e!.GetType().FullName}");
                builder.AppendLine("----------------------------------------------------------------------");
                builder.AppendLine($"异常堆栈信息：{e}");

                await File.WriteAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"小蓝瓶错误报告-{e!.GetType().FullName}.txt"), builder.ToString());
                JsonUtils.WriteLaunchInfoJson();
                JsonUtils.WriteLauncherInfoJson();
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .With(new Win32PlatformOptions())
                .With(new SkiaOptions());
    }
}
