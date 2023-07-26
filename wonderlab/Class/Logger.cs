using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab.Class {
    public class Logger {
        private List<string> Logs = new();

        private readonly string LogsPath = Path.Combine(Directory.GetCurrentDirectory(), "logs");

        public Logger Log(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public Logger Info(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}][信息] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public Logger Error(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}][错误] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public Logger Warning(string message) {
            string log = $"[{SystemUtils.GetPlatformName()}][警告] {message}";
            Logs.Add(log);
            Trace.WriteLine(log);
            return this;
        }

        public static Logger LoadLogger() {
            Logger logger = new Logger();
            AssemblyLoadContext.Default.Unloading += async (ctx) => {
                Trace.WriteLine("closing");
                await logger.EncapsulateLogsToFileAsync();
            };

            return logger.Info("日志记录器已加载");
        }

        private async ValueTask EncapsulateLogsToFileAsync() {
            if (!LogsPath.IsDirectory()) {
                Directory.CreateDirectory(LogsPath);
            }

            var today = DateTime.Now;
            await File.WriteAllLinesAsync(Path.Combine(LogsPath, $"运行日志漂流瓶 - {today.Day}{today.Hour}{today.Minute}{today.Second}.log"), Logs);
        }
    }
}
