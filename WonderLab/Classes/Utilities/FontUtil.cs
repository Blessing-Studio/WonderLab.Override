using System.Text;
using Avalonia;
using Avalonia.Media;
using MinecraftLaunch.Modules.Utilities;

namespace WonderLab.Classes.Utilities {
    public static class FontUtil {
        public static AppBuilder UseSystemFont(this AppBuilder builder) {
            var font = new StringBuilder();
            //"resm:wonderlab.Assets.Fonts.DinPro.ttf?assembly=wonderlab#DIN Pro"
            if (EnvironmentUtil.IsWindow) {
                font.Append("Microsoft YaHei UI");
            } else if (EnvironmentUtil.IsLinux) {
                font.Append("dejavu, wqy-zenhei, wqy-microhei");
            } else {
                font.Append("苹方-简, 萍方-简");
            }

            return builder.With(new FontManagerOptions() {
                DefaultFamilyName = font.ToString(),
            });
        }
    }
}
