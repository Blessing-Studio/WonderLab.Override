using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Media;
using MinecraftLaunch.Utilities;

namespace WonderLab.Extensions;

/// <summary>
/// 字体扩展类
/// </summary>
public static class FontExtension {
    public static AppBuilder UseSystemFont(this AppBuilder builder) {
        if (EnvironmentUtil.IsMac) {
            return builder;
        }
        
        var fonts = new Dictionary<string, string> {
            { "Windows", "Microsoft YaHei UI, Microsoft YaHei" },
            { "Linux", "DejaVu Sans, Noto Sans CJK SC , WenQuanYi Micro Hei, WenQuanYi Zen Hei" },
        };

        var os = EnvironmentUtil.IsWindow 
            ? "Windows" : "Linux";
        
        var font = fonts[os];
        return builder.With(new FontManagerOptions {
            DefaultFamilyName = font,
        });
    }
}