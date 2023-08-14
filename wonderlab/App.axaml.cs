using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using wonderlab.Class;
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
            var load = new LoadWindow();
            load.Show();

            var main = new MainWindow {
                DataContext = MainWindow.ViewModel = new()
            };

            Logger = Logger.LoadLogger(main);
            main.Hide();

            await Task.Delay(1000);
            desktop.MainWindow = main;

            main.Show();
            load.Close();

            CurrentWindow = (desktop.MainWindow as MainWindow)!;
            Manager.Current = CurrentWindow;
        }

        //Logger = Logger.LoadLogger();
        base.OnFrameworkInitializationCompleted();
    }
}
