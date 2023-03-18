using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class UpdateUtils
    {
        public const string VersionType = "Lsaac";

        public const int Version = 1023;

        const string API = "https://gitee.com/api/v5/repos/baka_hs/xilu-baka/releases/latest";

        public static async ValueTask<UpdateInfo> GetLatestUpdateInfoAsync() {
            try {           
                var responseMessage = await HttpWrapper.HttpGetAsync(API);
                
                var json = await responseMessage.Content.ReadAsStringAsync();
                if (json.StartsWith("403")) {//访问过快导致的问题
                    return null!;
                }
                
                return json.ToJsonEntity<UpdateInfo>();
            }
            catch (Exception ex) {
                $"网络异常，{ex.Message}".ShowMessage("错误");
            }

            return null!;
        }

        public static async void UpdateAsync(UpdateInfo info,Action<float> action, Action ok) {
            if (info.CanUpdate()) {
                var downloadResponse = await HttpWrapper.HttpDownloadAsync(info.Assets.First().DownloadUrl, Directory.GetCurrentDirectory(), (p, s) =>
                {
                    action(p);
                }, "WonderLab.update");

                if (downloadResponse.HttpStatusCode is System.Net.HttpStatusCode.OK) {
                    ok();
                }
            }
        }
    }
    
    public class Author {   
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("followers_url")]
        public string FollowersUrl { get; set; }

        [JsonProperty("following_url")]
        public string FollowingUrl { get; set; }

        [JsonProperty("gists_url")]
        public string GistsUrl { get; set; }
 
        [JsonProperty("starred_url")]
        public string StarredUrl { get; set; }

        [JsonProperty("subscriptions_url")]
        public string SubscriptionsUrl { get; set; }

        [JsonProperty("organizations_url")]
        public string OrganizationsUrl { get; set; }

        [JsonProperty("repos_url")]
        public string ReposUrl { get; set; }

        [JsonProperty("events_url")]
        public string EventsUrl { get; set; }

        [JsonProperty("received_events_url")]
        public string ReceivedEventsUrl { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class UpdateAsset {   
        [JsonProperty("browser_download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class UpdateInfo {   
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("target_commitish")]
        public string TargetCommitish { get; set; }

        [JsonProperty("prerelease")]
        public string PreRelease { get; set; }

        [JsonProperty("name")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Message { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("assets")]
        public List<UpdateAsset> Assets { get; set; }
    }

}

