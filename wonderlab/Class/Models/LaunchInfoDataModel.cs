using MinecraftLaunch.Modules.Models.Launch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Utils;

namespace wonderlab.Class.Models
{
    /// <summary>
    /// 启动信息数据模型
    /// </summary>
    public class LaunchInfoDataModel {
        [JsonProperty("gameDirectoryPath")]
        public string GameDirectoryPath { get; set; } = GameCoreUtils.GetOfficialGameCorePath().FullName;

        [JsonProperty("javaruntimePath")]
        public JavaInfo JavaRuntimePath { get; set; } = new();

        [JsonProperty("selectGameCore")]
        public string SelectGameCore { get; set; } = string.Empty;

        [JsonProperty("jvmArgument")]
        public string JvmArgument { get; set; } = string.Empty;

        [JsonProperty("maxMemory")]
        public int MaxMemory { get; set; } = 1024;

        [JsonProperty("windowHeight")]
        public int WindowHeight { get; set; } = 480;

        [JsonProperty("windowWidth")]
        public int WindowWidth { get; set; } = 854;

        [JsonProperty("isAutoSelectjava")]
        public bool IsAutoSelectJava { get; set; } = false;

        [JsonProperty("isAutoGetMemory")]
        public bool IsAutoGetMemory { get; set; } = false;

        [JsonProperty("javaRuntimes")]
        public List<JavaInfo> JavaRuntimes { get; set; } = new();

        [JsonProperty("gameDirectorys")]
        public List<string> GameDirectorys { get; set; } = new() { GameCoreUtils.GetOfficialGameCorePath().FullName };
    }
}
