using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Diagnostics;
using wonderlab.Class.Utils;

namespace wonderlab
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            }
            catch (Exception)
            {
                JsonUtils.WriteLaunchInfoJson();
                JsonUtils.WriteLauncherInfoJson();
            }
        }

        public static AppBuilder BuildAvaloniaApp() {
            var result = AppBuilder.Configure<App>()
                   .UsePlatformDetect();

            //if (SystemUtils.IsMacOS)
            //    result.With(new AvaloniaNativePlatformOptions
            //    {
            //        UseGpu = true
            //    });

            //if (SystemUtils.IsLinux)
            //    result.With(new X11PlatformOptions
            //    {
            //        UseGpu = true
            //    });

            //if (SystemUtils.IsWindows)
            //{
            //    result.With(new Win32PlatformOptions
            //    {
            //        UseWgl = true,
            //        AllowEglInitialization = true,
            //    });
            //    result.With(new SkiaOptions
            //    {
            //        MaxGpuResourceSizeBytes = long.MaxValue,
            //    });
            //}
            result.LogToTrace();
            return result;
        }
    }
}