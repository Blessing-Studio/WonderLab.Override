﻿using MinecraftLaunch.Modules.Toolkits;
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
        static string DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab");
        static string UserDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "wonderlab", "user");

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

        internal static void DirectoryCheck() {
            if (!Directory.Exists(DataPath)) {
                Directory.CreateDirectory(DataPath);
            }
        }
    }
}
