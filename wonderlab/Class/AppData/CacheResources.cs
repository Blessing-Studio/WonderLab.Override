using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;
using wonderlab.Class.ViewData;

namespace wonderlab.Class.AppData {
    public class CacheResources {
        public static List<New> MojangNews { get; set; } = new();

        public static List<ModLoaderModel> Quilts { get; set; } = new();

        public static List<ModLoaderModel> Forges { get; set; } = new();

        public static List<ModLoaderModel> Fabrics { get; set; } = new();

        public static List<ModLoaderModel> Optifines { get; set; } = new();

        public static GameCoreEmtity GameCoreInstallInfo { get; set; } = default!;

        public static ObservableCollection<MinecraftProcessViewData> GameProcesses { get; set; } = new();

        public static ObservableCollection<AccountViewData> Accounts { get; set; } = new();

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
