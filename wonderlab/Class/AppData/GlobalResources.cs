using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.AppData {
    public class GlobalResources {
        [SupportedOSPlatform("OSX")]
        public const string MacJavaHomePath = "/Library/Java/JavaVirtualMachines";

        public const string CurseforgeToken = "$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6";

        public const string ClientId = "9fd44410-8ed7-4eb3-a160-9f1cc62c824c";

        public const string WonderApi = "http://43.136.86.16:14514/api/ServerInfo";

        public const string UpdateApi = "http://api.2018k.cn/getExample?id=f08e3a0d2d8f47d6b5aee68ec2499a21&data=version|notice|url|remark|lasttime";

        public const string MojangNewsApi = "https://launchercontent.mojang.com/news.json";

        public const string HitokotoApi = "https://v1.hitokoto.cn/";

        public static CurseForgeToolkit CurseForgeToolkit { get; } = new(CurseforgeToken);

        public static LaunchInfoDataModel LaunchInfoData { get; set; } = new();

        public static LauncherDataModel LauncherData { get; set; } = new();

        public static LaunchInfoDataModel DefaultLaunchInfoData { get; } = new();

        public static LauncherDataModel DefaultLauncherData { get; } = new();
    }
}
