using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string ImagePath { get; set; }
    }
}
