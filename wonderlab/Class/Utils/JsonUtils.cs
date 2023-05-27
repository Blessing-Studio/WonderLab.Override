using MinecraftLaunch.Modules.Toolkits;
using System;
using System.IO;
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
                App.LaunchInfoData = new();
                return;
            }

            var json = await jsonPath.ReadCompressedText();
            App.LaunchInfoData = json.ToJsonEntity<LaunchInfoDataModel>();
        }

        public static void WriteLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.wld");
            DirectoryCheck();

            jsonPath.WriteCompressedText(App.LaunchInfoData.ToJson());
        }

        public static async void CreateLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
            DirectoryCheck();

            if (!File.Exists(jsonPath)) {
                jsonPath.WriteCompressedText(new LauncherDataModel().ToJson());
                App.LauncherData = new();
                return;
            }

            var json = await jsonPath.ReadCompressedText();
            App.LauncherData = json.ToJsonEntity<LauncherDataModel>();
        }

        public static void WriteLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
            DirectoryCheck();

            jsonPath.WriteCompressedText(App.LauncherData.ToJson());
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
