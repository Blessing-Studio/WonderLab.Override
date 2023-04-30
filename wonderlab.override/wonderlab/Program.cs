using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using HarmonyLib;
using MinecraftLaunch.Modules.Toolkits;
using PluginLoader;
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
            PluginLoader.PluginLoader.LoadAllFromPlugin();
            PluginLoader.PluginLoader.EnableAll();
            Harmony harmony = new("wonderlab");
            harmony.PatchAll();
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
            PluginLoader.PluginLoader.DisableAll();
            PluginLoader.PluginLoader.UnloadAll();
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