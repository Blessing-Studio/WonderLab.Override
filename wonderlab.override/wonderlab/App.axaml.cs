using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using wonderlab.Class.Models;
using wonderlab.Views.Windows;

namespace wonderlab
{
    public partial class App : Application
    {
        public static LaunchInfoDataModel LaunchInfoData { get; set; } = new();

        public static LauncherDataModel LauncherData { get; set; } = new();

        public static Window Host { get; set; }

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
