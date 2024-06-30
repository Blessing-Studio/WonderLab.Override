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
        
        return builder.With(new FontManagerOptions {
            DefaultFamilyName = "Microsoft YaHei UI, Microsoft YaHei",
        });
    }
}