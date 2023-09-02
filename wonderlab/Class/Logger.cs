using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using wonderlab.Class.Utils;
using MinecraftLaunch.Modules.Utils;

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

        public static Logger LoadLogger(Window window) {
            Logger logger = new Logger();
            window.Closed += async (o, ctx) => {
                await logger.EncapsulateLogsToFileAsync();
                Environment.Exit(0);
            };

            return logger.Info("日志记录器已加载");
        }

        private async ValueTask EncapsulateLogsToFileAsync() {
            if (!LogsPath.IsDirectory()) {
                Directory.CreateDirectory(LogsPath);
            }

            var today = DateTime.Now;
            await File.WriteAllLinesAsync(Path.Combine(LogsPath, $"运行日志漂流瓶{today:yyyy-MM-dd-HH-mm-ss}.log"), Logs);
        }
    }
}
