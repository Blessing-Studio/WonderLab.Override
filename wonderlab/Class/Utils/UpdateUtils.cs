using Flurl.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using MinecraftLaunch.Modules.Downloaders;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace wonderlab.Class.Utils {
    public static class UpdateUtils {
        public static string LocalVersion => 
            $"{Regex.Replace(AssemblyUtil.Version, @"\.\d+$", "")} {(AssemblyUtil.Build is 0 ? "" : $"Pre {AssemblyUtil.Build}")}";

        //public const string Local

        private const string UpdateUrl = "http://43.136.86.16:14514/api/update/";

        public static async ValueTask<JsonNode> GetLatestVersionInfoAsync() {
            try {
                using var responseMessage = await UpdateUrl
                    .GetAsync();

                var json = await responseMessage
                    .GetStringAsync();

                return JsonNode.Parse(json);        
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }

            return null!;
        }

        public static bool Check(JsonNode node) {
            int localVersion = AssemblyUtil.Version
                .Replace(".", "")
                .ToInt32();

            int newVersion = node["version"].GetValue<string>()
                .Replace(".", "")
                .ToInt32();

            return (localVersion < newVersion) && SystemUtils.IsWindows;
        }

        public static async void UpdateAsync(JsonNode info, Action<double> action) {
            var version = info["version"].GetValue<string>();
            Match match = Regex.Match(version, @"(?<=1\.2\.7\-preview)\d+");
            if (match.Success) {

            }
            
            using var downloader = FileDownloader.Build(new() {
                Url = $"https://ghproxy.com/{info["windows_file_url"].GetValue<string>()}",
                Directory = Directory.GetCurrentDirectory().ToDirectory()!,
                FileName = "launcher.zip"
            });

            downloader.DownloadProgressChanged += (_, raw) => {
                action(raw.Progress);
            };

            downloader.BeginDownload();
            var result = await downloader.CompleteAsync();

            if (result.HttpStatusCode is HttpStatusCode.OK) {
                try {
                    using var zip = ZipFile.OpenRead(result.Result.FullName);
                    var entry = zip.GetEntry("wonderlab.exe");
                    if (entry is not null) {
                        entry.ExtractToFile("wlo.exe");
                    }
                    await Task.Delay(1000);
                    JsonUtils.WriteLauncherInfoJson();
                    Process.Start(new ProcessStartInfo {
                        FileName = "powershell.exe",
                        Arguments = ArgumentsBuilding(),
                        WorkingDirectory = Directory.GetCurrentDirectory(),
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    })!.Dispose();
                }
                catch (Exception ex) {
                    App.Logger.Error(ex.ToString());
                }
            }
        }

        public static string ArgumentsBuilding() {
            int currentPID = Process.GetCurrentProcess().Id;
            string name = Process.GetCurrentProcess().ProcessName, filename = $"{name}.exe";
            return $"Stop-Process -Id {currentPID} -Force;" +
                   $"Wait-Process -Id {currentPID} -ErrorAction SilentlyContinue;" +
                   $"Start-Sleep -Milliseconds 500;" +
                   $"Remove-Item launcher.zip -Force;" +
                   $"Remove-Item {filename} -Force;" +
                   $"Rename-Item wlo.exe {filename};" +
                   $"Start-Process {name}.exe -Args updated;";
        }
    }
}

