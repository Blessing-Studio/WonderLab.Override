using Natsurainko.Toolkits.Network;
using Natsurainko.Toolkits.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.AppData;

namespace wonderlab.Class.Utils {
    public static class UpdateUtils {
        public const string LocalVersion = "1.2.7";

        private const string BaseUpdateUrl = "https://yangspring114.github.io/wonderlab.update/";

        private const string UpdateUrl = "http://43.136.86.16:14514/api/lsaac/download";

        public static readonly string IndexUrl = $"{BaseUpdateUrl}{GlobalResources.LauncherData.IssuingBranch.ToString().ToLower()}/index.json";

        public static readonly string VersionInfoUrl = $"{BaseUpdateUrl}{GlobalResources.LauncherData.IssuingBranch.ToString().ToLower()}/files/window/*/info.json";

        public static Index Index { get; private set; }

        public static async ValueTask<VersionInfo> GetLatestVersionInfoAsync() {
            try {
                var responseMessage = await HttpWrapper.HttpGetAsync(IndexUrl);
                IndexUrl.ShowLog();
                var json = await responseMessage.Content.ReadAsStringAsync();
                Index = json.ToJsonEntity<Index>();

                var versionInfoUrl = VersionInfoUrl.Replace("*",$"{Index.Type}{Index.Latest}");
                VersionInfoUrl.Replace("*", $"{Index.Type}{Index.Latest}").ShowLog();

                var responseMessage1 = await HttpWrapper.HttpGetAsync(versionInfoUrl);
                var versionInfoJson = await responseMessage1.Content.ReadAsStringAsync();

                return versionInfoJson.ToJsonEntity<VersionInfo>();
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }

            return null!;
        }

        public static async void UpdateAsync(VersionInfo info, Action<float> action) {
            if (Index.Latest.Replace(".","").ToInt32() > LocalVersion.Replace(".", "").ToInt32()) {
                var downloadResponse = await HttpWrapper.HttpDownloadAsync(string.Format(info.DownloadUrl,$"x64"), Directory.GetCurrentDirectory(), (p, s) => {
                    action(p);
                }, "WonderLab.zip");

                if (downloadResponse.HttpStatusCode is HttpStatusCode.OK) {
                    try {
                        using var zip = ZipFile.OpenRead(downloadResponse.FileInfo.FullName);
                        zip.ExtractToDirectory(downloadResponse.FileInfo.Directory.FullName);

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
                   $"Remove-Item WonderLab.zip -Force;" +
                   $"Remove-Item {filename} -Force;" +
                   $"Rename-Item launcher.exe {filename};" +
                   $"Start-Process {name}.exe -Args updated;";
        }
    }

    public record Index {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("latest")]
        public string Latest { get; set; }
    }

    public record VersionInfo {
        [JsonProperty("url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("message")]
        public IEnumerable<string> UpdateMessage { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
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

