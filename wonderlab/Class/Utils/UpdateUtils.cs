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
                return json.ToJsonEntity<UpdateInfo>();
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }

            return null!;
        }

        public static async void UpdateAsync(UpdateInfo info, Action<float> action) {
            if (info.CanUpdate()) {
                var downloadResponse = await HttpWrapper.HttpDownloadAsync(info.Assets.First().Url, Directory.GetCurrentDirectory(), (p, s) => {
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

                        var intVersion = Convert.ToInt32(info.Version.Replace(".", string.Empty));
                        GlobalResources.LauncherData.LauncherVersion = intVersion;
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
                   $"Start-Sleep -Milliseconds 500;" +
                   $"Remove-Item {filename} -Force;" +
                   $"Rename-Item WonderLab.update {filename};" +
                   $"Start-Process {name}.exe -Args updated;";
        }
    }

    public class UpdateInfo {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tag_name")]
        public string Version { get; set; }

        [JsonProperty("target_commitish")]
        public string Sha1 { get; set; }

        [JsonProperty("prerelease")]
        public bool IsPreview { get; set; }

        [JsonProperty("name")]
        public string Branch { get; set; }

        [JsonProperty("body")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("assets")]
        public List<Assets> Assets { get; set; }

    }

    public class Author {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string OwnerName { get; set; }

        [JsonProperty("name")]
        public string UserName { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("url")]
        public string UserInfoUrl { get; set; }

        [JsonProperty("html_url")]
        public string UserHomeUrl { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("followers_url")]
        public string followers_url { get; set; }

        [JsonProperty("following_url")]
        public string following_url { get; set; }

        [JsonProperty("gists_url")]
        public string gists_url { get; set; }

        [JsonProperty("starred_url")]
        public string starred_url { get; set; }

        [JsonProperty("subscriptions_url")]
        public string subscriptions_url { get; set; }

        [JsonProperty("organizations_url")]
        public string organizations_url { get; set; }

        [JsonProperty("repos_url")]
        public string repos_url { get; set; }

        [JsonProperty("events_url")]
        public string events_url { get; set; }

        [JsonProperty("received_events_url")]
        public string received_events_url { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class Assets {
        [JsonProperty("browser_download_url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string FileName { get; set; }
    }
}

