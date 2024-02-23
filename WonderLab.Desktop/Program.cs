using System;
using Avalonia;
using WonderLab.Extensions;

namespace WonderLab.Desktop;

public sealed class Program {
    [STAThread]
    public static void Main(string[] args) {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseSystemFont()
            .LogToTrace();
}
