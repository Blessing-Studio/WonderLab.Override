using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;

namespace wonderlab.Class.Utils {
    public static class UpdateUtils {
        public const string VersionType = "Lsaac";

        public static async ValueTask<UpdateInfo> GetLatestUpdateInfoAsync() {
            try {
                var responseMessage = await HttpWrapper.HttpGetAsync(GlobalResources.UpdateApi);
                var json = await responseMessage.Content.ReadAsStringAsync();
                var texts = json.Split('|');

                return new() {
                    Title = texts[1],
                    TagName = texts[0],
                    Message = texts[3],
                    DownloadUrl = texts[2],
                    CreatedAt = texts.Last(),
                };
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }

            return null!;
        }

        public static async void UpdateAsync(UpdateInfo info, Action<float> action) {
            if (info.CanUpdate()) {
                var downloadResponse = await HttpWrapper.HttpDownloadAsync(info.DownloadUrl, Directory.GetCurrentDirectory(), (p, s) => {
                    action(p);
                }, "WonderLab.update");

                if (downloadResponse.HttpStatusCode is HttpStatusCode.OK) {
                    try {
                        Process.Start(new ProcessStartInfo {
                            FileName = "powershell.exe",
                            Arguments = ArgumentsBuilding(),
                            WorkingDirectory = Directory.GetCurrentDirectory(),
                            UseShellExecute = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        });

                        var intVersion = Convert.ToInt32(info.TagName.Replace(".", string.Empty));
                        App.LauncherData.LauncherVersion = intVersion;
                        JsonUtils.WriteLauncherInfoJson();
                    }
                    catch (Exception) { }
                }
            }
        }

        public static string ArgumentsBuilding() {
            int currentPID = Process.GetCurrentProcess().Id;
            string name = Process.GetCurrentProcess().ProcessName, filename = $"{name}.exe";

            return $"Stop-Process -Id {currentPID} -Force;" +
                        $"Wait-Process -Id {currentPID} -ErrorAction SilentlyContinue;" +
                        "Start-Sleep -Milliseconds 500;" +
                        $"Remove-Item {filename} -Force;" +
                        $"Rename-Item WonderLab.update {filename};" +
                        $"Start-Process {name}.exe -Args updated;";
        }
    }

    public class UpdateInfo {
        public string TagName { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string CreatedAt { get; set; }

        public string DownloadUrl { get; set; }
    }

}

