using MinecraftLaunch.Modules.Models.Launch;
using System.Text.Json.Serialization;
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
        [JsonPropertyName("gameDirectoryPath")]
        public string GameDirectoryPath { get; set; } = GameCoreUtils.GetOfficialGameCorePath().FullName;

        [JsonPropertyName("javaruntimePath")]
        public JavaInfo JavaRuntimePath { get; set; } = new();

        [JsonPropertyName("selectGameCore")]
        public string SelectGameCore { get; set; } = string.Empty;

        [JsonPropertyName("jvmArgument")]
        public string JvmArgument { get; set; } = string.Empty;

        [JsonPropertyName("maxMemory")]
        public int MaxMemory { get; set; } = 1024;

        [JsonPropertyName("windowHeight")]
        public int WindowHeight { get; set; } = 480;

        [JsonPropertyName("windowWidth")]
        public int WindowWidth { get; set; } = 854;

        [JsonPropertyName("isAutoSelectjava")]
        public bool IsAutoSelectJava { get; set; } = false;

        [JsonPropertyName("isAutoGetMemory")]
        public bool IsAutoGetMemory { get; set; } = false;

        [JsonPropertyName("javaRuntimes")]
        public List<JavaInfo> JavaRuntimes { get; set; } = new();

        [JsonPropertyName("gameDirectorys")]
        public List<string> GameDirectorys { get; set; } = new() { GameCoreUtils.GetOfficialGameCorePath().FullName };
    }
}
