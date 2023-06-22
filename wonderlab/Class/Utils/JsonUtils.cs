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
                    jsonPath.WriteCompressedText(new LaunchInfoDataModel().ToNewtonJson());
                    GlobalResources.LaunchInfoData = new();
                    return;
                }

                var json = await jsonPath.ReadCompressedText();
                GlobalResources.LaunchInfoData = json.ToNewtonJsonEntity<LaunchInfoDataModel>();

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

            jsonPath.WriteCompressedText(GlobalResources.LaunchInfoData.ToNewtonJson());
        }

        public static async void CreateLauncherInfoJson() {
            try {
                var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
                DirectoryCheck();

                if (!File.Exists(jsonPath)) {
                    jsonPath.WriteCompressedText(new LauncherDataModel().ToNewtonJson());
                    GlobalResources.LauncherData = new();
                    return;
                }

                var json = await jsonPath.ReadCompressedText();
                GlobalResources.LauncherData = json.ToNewtonJsonEntity<LauncherDataModel>();

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

            jsonPath.WriteCompressedText(GlobalResources.LauncherData.ToNewtonJson());
        }

        internal static void DirectoryCheck() {
            try {
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
            catch (Exception ex) {
                $"{ex}".ShowMessage("程序遭到了错误");
            }
        }
    }
}
