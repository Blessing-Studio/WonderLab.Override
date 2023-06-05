using MinecraftLaunch.Modules.Toolkits;
using System;
using System.IO;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils {
    public static class JsonUtils {
        public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab");

        public static string UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "user");

        public static string TempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "temp");

        public static async void CreateLaunchInfoJson() {
            try {
                var jsonPath = Path.Combine(DataPath, "launchdata.wld");
                DirectoryCheck();

                if (!File.Exists(jsonPath)) {
                    File.Create(jsonPath).Close();
                    jsonPath.WriteCompressedText(new LaunchInfoDataModel().ToJson());
                    GlobalResources.LaunchInfoData = new();
                    return;
                }

                var json = await jsonPath.ReadCompressedText();
                GlobalResources.LaunchInfoData = json.ToJsonEntity<LaunchInfoDataModel>();

                if (GlobalResources.LaunchInfoData.IsNull()) {
                    GlobalResources.LaunchInfoData = GlobalResources.DefaultLaunchInfoData;
                }
            }
            catch (Exception) {
                await Task.Delay(500);
                WriteLaunchInfoJson();
                "WonderLab在加载数据文件时出现了异常，初步判定为数据文件损坏或格式更新，我们已为您重新创建了新的数据文件，原先的数据已丢失，在此深表歉意".ShowInfoDialog("程序遭遇了异常");
            }
        }

        public static void WriteLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.wld");
            DirectoryCheck();

            jsonPath.WriteCompressedText(GlobalResources.LaunchInfoData.ToJson());
        }

        public static async void CreateLauncherInfoJson() {
            try {
                var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
                DirectoryCheck();

                if (!File.Exists(jsonPath)) {
                    jsonPath.WriteCompressedText(new LauncherDataModel().ToJson());
                    GlobalResources.LauncherData = new();
                    return;
                }

                var json = await jsonPath.ReadCompressedText();
                GlobalResources.LauncherData = json.ToJsonEntity<LauncherDataModel>();

                if (GlobalResources.LauncherData.IsNull()) {
                    GlobalResources.LauncherData = GlobalResources.DefaultLauncherData;
                }
            }
            catch (Exception) {
                await Task.Delay(500);
                WriteLauncherInfoJson();
                "WonderLab在加载数据文件时出现了异常，初步判定为数据文件损坏或格式更新，我们已为您重新创建了新的数据文件，原先的数据已丢失，在此深表歉意".ShowInfoDialog("程序遭遇了异常");
            }
        }

        public static void WriteLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
            DirectoryCheck();

            jsonPath.WriteCompressedText(GlobalResources.LauncherData.ToJson());
        }

        internal static void DirectoryCheck() {
            if (!Directory.Exists(DataPath)) {
                Directory.CreateDirectory(DataPath);
            }

            if (!Directory.Exists(UserDataPath)) {
                Directory.CreateDirectory(UserDataPath);
            }

            if (!Directory.Exists(TempPath)) {
                Directory.CreateDirectory(TempPath);
            }
        }
    }
}
