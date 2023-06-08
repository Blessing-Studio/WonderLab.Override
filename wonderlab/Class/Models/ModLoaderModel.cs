using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Installer;
using MinecraftLaunch.Modules.Models.Install;
using MinecraftLaunch.Modules.Models.Launch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class ModLoaderModel {
        public string GameCoreVersion { get; set; }

        public string Id { get; set; }

        public ModLoaderType ModLoaderType { get; set; }

        public string ModLoader => ModLoaderType.ToString();
        
        public object ModLoaderBuild { get; set; }

        public DateTime Time { get; set; }
    }
}
