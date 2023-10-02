using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Utils;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using wonderlab.Class.AppData;
using wonderlab.Class.Models;
using wonderlab.Class.ViewData;
using wonderlab.Views.Converters;

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

                var json = jsonPath.ReadCompressedText();
                GlobalResources.LaunchInfoData = json.ToJsonEntity<LaunchInfoDataModel>();

                if (GlobalResources.LaunchInfoData.IsNull()) {
                    GlobalResources.LaunchInfoData = GlobalResources.DefaultLaunchInfoData;
                }
            }
            catch (Exception) {
                await Task.Delay(500);
                WriteLaunchInfoJson();
                "WonderLab在加载数据文件时出现了异常，初步判定为数据文件损坏或格式更新，我们已为您重新创建了新的数据文件，原先的数据已丢失，在此深表歉意"
                    .ShowInfoDialog("程序遭遇了异常");
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

                var json = jsonPath.ReadCompressedText();
                JsonSerializerOptions options = new();
                options.Converters.Add(new JsonToColorConverter());
                
                GlobalResources.LauncherData = JsonSerializer.Deserialize<LauncherDataModel>(json, options)!;

                if (GlobalResources.LauncherData.IsNull()) {
                    GlobalResources.LauncherData = GlobalResources.DefaultLauncherData;
                }
            }
            catch (Exception) {
                await Task.Delay(500);
                WriteLauncherInfoJson();
                "WonderLab在加载数据文件时出现了异常，初步判定为数据文件损坏或格式更新，我们已为您重新创建了新的数据文件，原先的数据已丢失，在此深表歉意"
                    .ShowInfoDialog("程序遭遇了异常");
            }
        }

        public static void WriteLauncherInfoJson() {
            var jsonPath = Path.Combine(DataPath, "launcherdata.wld");
            DirectoryCheck();

            jsonPath.WriteCompressedText(GlobalResources.LauncherData.ToJson());
        }

        public static SingleCoreModel WriteSingleGameCoreJson(GameCore core) {
            DirectoryCheck();
            var file = Path.Combine(core.GetGameCorePath(true), $"singleConfig.wlcd");

            if (!file.IsFile()) {
                file.WriteCompressedText(new SingleCoreModel().ToJson());
                return new SingleCoreModel();
            }

            return new();
        }

        public static async ValueTask<SingleCoreModel> ReadSingleGameCoreJsonAsync(GameCore core) {
            string path = Path.Combine(core.GetGameCorePath(true), $"singleConfig.wlcd");
            var json = await path.ReadCompressedTextAsync();

            if (!path.IsFile()) {
                return WriteSingleGameCoreJson(core);
            }

            json.ShowLog();
            var data = json.ToJsonEntity<SingleCoreModel>();

            return data;
        }

        public static GameCoreViewData SaveSingleGameCoreJson(GameCoreViewData config) {
            if (!config.IsNull()) {
                string path = Path.Combine(config.Data.GetGameCorePath(true), $"singleConfig.wlcd");

                if (path.IsFile()) {
                    var json = config.SingleConfig.ToJson();
                    path.WriteCompressedText(json);
                }
            }

            return config;
        }

        public static void DirectoryCheck() {
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
