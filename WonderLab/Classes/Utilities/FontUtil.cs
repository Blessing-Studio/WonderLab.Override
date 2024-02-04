using Avalonia;
using Avalonia.Media;
using MinecraftLaunch.Utilities;
using System.Text;

namespace WonderLab.Classes.Utilities;

public static class FontUtil
{
    public static AppBuilder UseSystemFont(this AppBuilder builder)
    {
        var font = new StringBuilder();
        //"resm:wonderlab.Assets.Fonts.DinPro.ttf?assembly=wonderlab#DIN Pro"
        if (EnvironmentUtil.IsWindow)
        {
            font.Append("Microsoft YaHei UI, Microsoft YaHei");
        }
        else if (EnvironmentUtil.IsLinux)
        {
            font.Append("DejaVu Sans, Noto Sans CJK SC , WenQuanYi Micro Hei, WenQuanYi Zen Hei");
        }
        else
        {
            font.Append("苹方-简, 萍方-简");
        }

        return builder.With(new FontManagerOptions()
        {
            DefaultFamilyName = font.ToString(),
        });
    }
}
