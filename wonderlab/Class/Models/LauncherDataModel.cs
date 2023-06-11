using Avalonia.Media;
using MinecraftLaunch.Modules.Models.Download;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Enum;

namespace wonderlab.Class.Models
{
    /// <summary>
    /// 启动器设置数据模型
    /// </summary>
    public class LauncherDataModel {
        [JsonProperty("accentColor")]
        public Color AccentColor { get; set; } = Color.FromRgb(255, 185, 0);
        
        [JsonProperty("bakgroundType")]
        public string BakgroundType { get; set; } = "主题色背景";

        [JsonProperty("themeType")]
        public string ThemeType { get; set; } = "亮色主题";

        [JsonProperty("parallaxType")]
        public string ParallaxType { get; set; } = "无";

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonProperty("issuingBranch")]
        public IssuingBranch IssuingBranch { get; set; } = IssuingBranch.Lsaac;

        [JsonProperty("currentdownloadAPI")]
        public DownloadApiType CurrentDownloadAPI { get; set; } = DownloadApiType.Mojang;

        [JsonProperty("languageType")]
        public string LanguageType { get; set; } = "zh-cn";

        [JsonProperty("launcherVersion")]
        public int LauncherVersion { get; set; } = 120;

        [JsonProperty("downloadCount")]
        public int DownloadCount { get; set; } = 64;
    }
}
