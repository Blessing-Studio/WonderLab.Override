using System;
using System.Linq;
using System.Threading;

using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using wonderlab.Class;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab;

class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        try {
            var builder = BuildAvaloniaApp();
            builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex) {
            App.Logger.Error(ex.ToString());
            _ = App.Logger.EncapsulateLogsToFileAsync();
            var model = new ExceptionModel() {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Exception = ex.GetType().Name,
            };
        }
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseSystemFont()
            .LogToTrace()
            .UseReactiveUI()
            .UsePlatformDetect();
}
