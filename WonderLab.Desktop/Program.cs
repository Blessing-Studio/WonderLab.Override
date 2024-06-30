using System;
using System.Runtime.InteropServices;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Extensions;
using WonderLab.Services;

namespace WonderLab.Desktop;

public sealed class Program {
    [STAThread]
    public static void Main(string[] args) {
        try {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        } catch (Exception ex) {
            var logService = App.ServiceProvider.GetService<LogService>();
            logService!.Error(nameof(Program), $"程序遭遇了致命性错误，信息堆栈：{ex}");
            logService.Finish();
        }
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
        }).With(new SkiaOptions {
            MaxGpuResourceSizeBytes = 1073741824L
        });
}