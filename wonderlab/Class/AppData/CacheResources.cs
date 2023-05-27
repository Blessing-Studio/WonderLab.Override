using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.Class.AppData {
    public class CacheResources {
        public static List<New> MojangNews { get; set; } = new();

        public IEnumerable<MinecraftLaunchResponse>? GameProcesses { get; set; }

        public static Dictionary<string, WebModpackInfoModel> WebModpackInfoDatas { get; set; } = new();

        public static void GetWebModpackInfoData() {
            StreamReader reader = new(AvaloniaUtils.GetAssetsStream("ModpackInfos.json"));
            var infos = reader.ReadToEnd().ToJsonEntity<List<WebModpackInfoModel>>();
            if (infos is not null && infos.Any()) {
                infos.ForEach(x => {
                    if (x.CurseForgeId != null) {
                        WebModpackInfoDatas.Add(x.CurseForgeId, x);
                    }
                });

                WebModpackInfoDatas.Values.ToList().ForEach(x => {
                    if (x.Chinese.Contains("*"))
                        x.Chinese = x.Chinese.Replace("*", string.Empty);
                });
            }
        }
    }
}
