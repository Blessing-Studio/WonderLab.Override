using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models {
    public class ModLoaderModel {
        public string GameCoreVersion { get; set; }

        public string Id { get; set; }

        public ModLoaderType ModLoaderType { get; set; }

        public string ModLoader => ModLoaderType.ToString();

        public object ModLoaderBuild { get; set; }

        public DateTime Time { get; set; }

        public string Type => GetModLoaderDescription();

        private string GetModLoaderDescription() {
            if (ModLoaderType is ModLoaderType.Forge) {
                return $"发布于 {Time.ToString("yyyy/MM/dd HH:mm")}";
            }

            return "正式版，发布时间未知";
        }
    }
}
