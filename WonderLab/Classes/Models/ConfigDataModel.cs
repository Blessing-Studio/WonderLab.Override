using WonderLab.Classes.Enums;
using System.Text.Json.Serialization;
using MinecraftLaunch.Modules.Models.Launch;
using System.Collections.Generic;

namespace WonderLab.Classes.Models {
    public class ConfigDataModel {
        [JsonPropertyName("isFullscreen")]
        public bool IsFullscreen { get; set; }

        [JsonPropertyName("backgroundType")]
        public BackgroundType BackgroundType { get; set; }

        [JsonPropertyName("javaPath")]
        public JavaInfo JavaPath { get; set; }

        [JsonPropertyName("javaPaths")]
        public List<JavaInfo> JavaPaths { get; set; }

        [JsonPropertyName("isAutoSelectJava")]
        public bool IsAutoSelectJava { get; set; }

        [JsonPropertyName("isAutoMemory")]
        public bool IsAutoMemory { get; set; }

        [JsonPropertyName("maxMemory")]
        public int MaxMemory { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("gameFolder")]
        public string GameFolder { get; set; }

        [JsonPropertyName("gameFolders")]
        public List<string> GameFolders { get; set; }
    }
}
