using Avalonia;
using System;

namespace WonderLab;

internal sealed class Program {
    [STAThread]
    public static void Main(string[] args) {
        try {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("glyphTypeface") || ex.Message.Contains("font"))
        {
            Console.WriteLine("字体加载失败。");
            Console.WriteLine("可能原因：系统中存在 X11 位图字体（如 Adobe Courier PCF 格式）");
            Console.WriteLine("请卸载您系统中 xorg-fonts-100dpi xorg-fonts-75dpi 两个包（不同系统包名不同）");
            Console.WriteLine("随后 fc-cache -fv。");
            Console.WriteLine();
            Console.WriteLine($"原始错误：{ex.Message}");
            throw; // 孩子们我打断点打了半天发现不是 avalonia 的问题
        }
        catch (Exception ex) {
            Console.WriteLine($"未预期的错误：{ex.Message}");
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}