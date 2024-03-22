using System;
using System.Runtime.InteropServices;
using Avalonia;
using WonderLab.Extensions;

namespace WonderLab.Desktop;

public sealed class Program {
    [STAThread]
    public static void Main(string[] args) {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .UseSystemFont()
        .LogToTrace()
        .With(new Win32PlatformOptions() {
            RenderingMode = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64 
            ? [Win32RenderingMode.Wgl] 
            : [Win32RenderingMode.AngleEgl, Win32RenderingMode.Software]!,
        })
        .With(new MacOSPlatformOptions() {
            DisableDefaultApplicationMenuItems = true,
        })
        .With(new X11PlatformOptions() { 
            OverlayPopups = true,
        });
}
