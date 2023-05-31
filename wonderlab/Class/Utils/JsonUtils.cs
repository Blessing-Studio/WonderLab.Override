using MinecraftLaunch.Modules.Toolkits;
using System;
using System.IO;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils {
    public static class JsonUtils {
        public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab");

        public static string UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "user");

        public static string TempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "temp");

        public static async void CreateLaunchInfoJson() {
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

        public static void WriteLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.wld");
            DirectoryCheck();

            jsonPath.WriteCompressedText(GlobalResources.LaunchInfoData.ToJson());
        }

        public static async void CreateLauncherInfoJson() {
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
