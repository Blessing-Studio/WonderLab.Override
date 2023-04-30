using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public static class JsonUtils
    {
        public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab");
        public static string UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "user");
        public static string TempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "temp");

        public static async void CraftLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.wld");
            DirectoryCheck();

            if (!File.Exists(jsonPath)) {
                await Task.Run(() => { FileUtils.WriteCompressedAllText(jsonPath, new LaunchInfoDataModel().ToJson()); });
                App.LaunchInfoData = new();
                return;
            }

            var json = FileUtils.ReadCompressedAllText(jsonPath);
            App.LaunchInfoData = json.ToJsonEntity<LaunchInfoDataModel>();
        }

        public static async void WriteLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.wld");
            DirectoryCheck();

            await Task.Run(() => { FileUtils.WriteCompressedAllText(jsonPath, App.LaunchInfoData.ToJson() ?? new("")); });
        }

        public static async void CraftLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
            DirectoryCheck();

            if (!File.Exists(jsonPath)) {
                await Task.Run(() => { FileUtils.WriteCompressedAllText(jsonPath, new LauncherDataModel().ToJson()); });
                App.LauncherData = new();
                return;
            }

            var json = await Task.Run(() => {return FileUtils.ReadCompressedAllText(jsonPath); });
            App.LauncherData = json.ToJsonEntity<LauncherDataModel>();
        }

        public static async void WriteLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
            DirectoryCheck();

            await Task.Run(() => { FileUtils.WriteCompressedAllText(jsonPath, App.LauncherData.ToJson() ?? new("")); });
        }

        internal static void DirectoryCheck() {
            if (!Directory.Exists(DataPath)) {
                Directory.CreateDirectory(DataPath);
            }

            if(!Directory.Exists(UserDataPath)) { 
                Directory.CreateDirectory(UserDataPath);
            }

            if (!Directory.Exists(TempPath)) {           
                Directory.CreateDirectory(TempPath);
            }
        }
    }
}
