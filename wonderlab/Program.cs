using System;
using System.Linq;
using System.Threading;

using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using wonderlab.Class.Models;

namespace wonderlab;

class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args) {
        try {
            var builder = BuildAvaloniaApp();
            builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex) {
            var model = new ExceptionModel() {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Exception = ex.GetType().Name,
            };
        }

        return 114514;
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .With(new FontManagerOptions() {
                DefaultFamilyName = "resm:wonderlab.Assets.Fonts.DinPro.ttf?assembly=wonderlab#DIN Pro"
            })
            .LogToTrace()
            .UseReactiveUI()
            .UsePlatformDetect();
}
