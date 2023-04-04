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
    public class JsonUtils
    {
        public static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab");
        public static string UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "user");
        public static string TempPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "temp");

        public static async void CraftLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.json");
            DirectoryCheck();

            if (!File.Exists(jsonPath)) {
                await File.WriteAllTextAsync(jsonPath,new LaunchInfoDataModel().ToJson());
                App.LaunchInfoData = new();
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            App.LaunchInfoData = json.ToJsonEntity<LaunchInfoDataModel>();
        }

        public static async void WriteLaunchInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launchdata.json");
            DirectoryCheck();

            await File.WriteAllTextAsync(jsonPath, App.LaunchInfoData.ToJson() ?? new(""));
        }

        public static async void CraftLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.json");
            DirectoryCheck();

            if (!File.Exists(jsonPath)) {           
                await File.WriteAllTextAsync(jsonPath, new LauncherDataModel().ToJson());
                App.LauncherData = new();
                return;
            }

            var json = await File.ReadAllTextAsync(jsonPath);
            App.LauncherData = json.ToJsonEntity<LauncherDataModel>();
        }

        public static async void WriteLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.json");
            DirectoryCheck();

            await File.WriteAllTextAsync(jsonPath, App.LauncherData.ToJson() ?? new(""));
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
