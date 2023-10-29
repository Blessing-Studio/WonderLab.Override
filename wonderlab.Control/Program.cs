using Avalonia;
using System;
using System.Timers;

namespace wonderlab.control {
    internal class Program {
        private static Timer collectTimer = new(TimeSpan.FromSeconds(8));

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            collectTimer.Elapsed += CollectElapsed;

            collectTimer.Start();
        }

        private static void CollectElapsed(object? sender, ElapsedEventArgs e) {
            GC.Collect();
        }


        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
    }
}