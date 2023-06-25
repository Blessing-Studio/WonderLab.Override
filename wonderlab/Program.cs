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
using wonderlab.Class.AppData;
using wonderlab.Class.Utils;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace wonderlab {
    internal class Program {
        [STAThread]
        public static async Task Main(string[] args) {
            try {
                Environment.SetEnvironmentVariable("LC_CTYPE", "en_US.UTF-8");
                BuildAvaloniaApp()
                  .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e) {
                var str = e.ToString();
                StreamReader reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(str)));
                List<string> lines = new();
                while (reader.Peek() != -1) {
                    lines.Add(reader.ReadLine()!);
                }

                StringBuilder builder = new();
                builder.Append("非常抱歉您的 WonderLab 又又又炸了，以下是此次崩溃的错误信息\n");
                builder.Append("----------------------------------------------------------------------\n");
                builder.Append($"系统平台：{SystemUtils.GetPlatformName()}\n");
                builder.Append($"异常名：{e!.GetType().FullName}\n");
                builder.Append("----------------------------------------------------------------------\n");
                builder.Append($"异常堆栈信息：{string.Join("\n",lines)}");

                await Task.Run(async () => {
                    var json = new Model(e!.GetType().FullName!, builder.ToString())!.ToJson(false)!;
                    json.ShowLog();
                    var result = await HttpWrapper.HttpPostAsync($"{GlobalResources.WonderApi}error", json);
                    (await result.Content.ReadAsStringAsync()).ShowLog();
                });

                //await Task.Run(async () => {
                //    await File.WriteAllTextAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"小蓝瓶错误报告-{e!.GetType().FullName}.txt"), builder.ToString());
                //    JsonUtils.WriteLaunchInfoJson();
                //    JsonUtils.WriteLauncherInfoJson();
                //});
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .With(new Win32PlatformOptions())
                .With(new SkiaOptions());

        record Model {
            public Model(string et,string ei) {
                ErrorType = et;
                ErrorInfo = ei;
            }

            [JsonProperty("errorType")]
            public string ErrorType { get; set; } = string.Empty;

            [JsonProperty("errorInfo")]
            public string ErrorInfo { get; set; } = string.Empty;
        }
    }
}
