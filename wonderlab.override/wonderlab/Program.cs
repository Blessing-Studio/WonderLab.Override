using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using MinecraftLaunch.Modules.Toolkits;
using System;
using System.Diagnostics;
using System.Text;
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
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                CurseForgeToolkit.Key = "$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6";

                BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args, ShutdownMode.OnLastWindowClose);
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

            result.With(new FontManagerOptions {           
                DefaultFamilyName = "resm:wonderlab.Assets.Fonts.MiSans-Normal.ttf?assembly=wonderlab#MiSans",
            });

            //if (SystemUtils.IsMacOS)
            //    result.With(new AvaloniaNativePlatformOptions {               
            //        UseGpu = true
            //    });

            //if (SystemUtils.IsLinux)
            //    result.With(new X11PlatformOptions {               
            //        UseGpu = true
            //    });

            //if (SystemUtils.IsWindows) {           
            //    result.With(new Win32PlatformOptions {               
            //        UseWgl = true,
            //        AllowEglInitialization = true,
            //    });
            //    result.With(new SkiaOptions
            //    {
            //        MaxGpuResourceSizeBytes = 1024000000,
            //    });
            //}
            result.LogToTrace();
            return result;
        }
    }
}