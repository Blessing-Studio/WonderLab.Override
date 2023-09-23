using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using wonderlab.Class;
using wonderlab.Class.Utils;
using wonderlab.control;
using wonderlab.Views.Windows;
using MainWindow = wonderlab.Views.Windows.MainWindow;

namespace wonderlab;

public partial class App : Application {
    public static MainWindow CurrentWindow { get; protected set; } = null!;
    public static Logger Logger { get; protected set; }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            JsonUtils.CreateLauncherInfoJson();
            JsonUtils.CreateLaunchInfoJson();
            desktop.MainWindow = new MainWindow {
                DataContext = MainWindow.ViewModel = new()
            };

            Manager.Current = CurrentWindow = (desktop.MainWindow as MainWindow)!;
            Logger = Logger.LoadLogger(CurrentWindow);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
