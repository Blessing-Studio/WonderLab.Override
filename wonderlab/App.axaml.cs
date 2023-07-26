using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using wonderlab.Class;
using wonderlab.control;
using MainWindow = wonderlab.Views.Windows.MainWindow;

namespace wonderlab {
    public partial class App : Application {
        public static MainWindow CurrentWindow { get; protected set; } = null!;
        public static Logger Logger { get; protected set; }

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = CurrentWindow = new MainWindow();
                Manager.Current = CurrentWindow;
            }

            Logger = Logger.LoadLogger();
            base.OnFrameworkInitializationCompleted();
        }
    }
}