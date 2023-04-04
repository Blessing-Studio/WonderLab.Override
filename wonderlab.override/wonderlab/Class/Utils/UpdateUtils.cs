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

        public const int Version = 127;

        const string API = "http://api.2018k.cn/getExample?id=f08e3a0d2d8f47d6b5aee68ec2499a21&data=version|notice|url|remark|lasttime";

        public static async ValueTask<UpdateInfo> GetLatestUpdateInfoAsync() {
            try {           
                var responseMessage = await HttpWrapper.HttpGetAsync(API);                
                var json = await responseMessage.Content.ReadAsStringAsync();
                var texts = json.Split('|');

                return new()
                {
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

        public static async void UpdateAsync(UpdateInfo info,Action<float> action, Action ok) {
            if (info.CanUpdate()) {
                var downloadResponse = await HttpWrapper.HttpDownloadAsync(info.DownloadUrl, Directory.GetCurrentDirectory(), (p, s) =>
                {
                    action(p);
                }, "WonderLab.update");

                if (downloadResponse.HttpStatusCode is System.Net.HttpStatusCode.OK) {
                    ok();
                }
            }
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

