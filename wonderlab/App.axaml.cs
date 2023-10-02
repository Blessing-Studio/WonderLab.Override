using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Linq;
using System.Reflection;
using System;
using System.Threading.Tasks;
using wonderlab.Class;
using wonderlab.Class.Utils;
using wonderlab.control;
using wonderlab.Views.Windows;
using MainWindow = wonderlab.Views.Windows.MainWindow;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Diagnostics;
using wonderlab.Class.AppData;

namespace wonderlab;

public partial class App : Application {
    public static MainWindow CurrentWindow { get; protected set; } = null!;
    public static Logger Logger { get; protected set; }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
        RxApp.MainThreadScheduler.Schedule(LoadAllDataAsync);
    }

    public override async void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow {
                DataContext = MainWindow.ViewModel = new()
            };

            Manager.Current = CurrentWindow = (desktop.MainWindow as MainWindow)!;
            Logger = Logger.LoadLogger(CurrentWindow);
            Logger.Info($"启动器版本：{AssemblyUtil.Version}");
        }

        base.OnFrameworkInitializationCompleted();
    }

    public async void LoadAllDataAsync() {
        ThemeUtils.Init();
        ThemeUtils.SetAccentColor(GlobalResources.LauncherData.AccentColor);

        JsonUtils.CreateLauncherInfoJson();
        JsonUtils.CreateLaunchInfoJson();
        await Task.Run(async () => {
            CacheResources.Accounts = (await AccountUtils.GetAsync(true)
                 .ToListAsync())
                 .ToObservableCollection();
        });

        CacheResources.GetWebModpackInfoData();
    }
}



