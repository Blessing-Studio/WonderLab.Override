using System;
using System.Linq;
using System.Threading;

using Avalonia;
using Avalonia.ReactiveUI;

namespace wonderlab;

class Program {
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args) {
        var builder = BuildAvaloniaApp();
        return builder.StartWithClassicDesktopLifetime(args);
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    private static void SilenceConsole() {
        new Thread(() => {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        }) { IsBackground = true }.Start();
    }
}
