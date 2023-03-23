using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using wonderlab.Class.Models;

namespace wonderlab
{
    public partial class App : Application
    {
        public static LaunchInfoDataModel LaunchInfoData { get; set; } = new();

        public static LauncherDataModel LauncherData { get; set; } = new();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
