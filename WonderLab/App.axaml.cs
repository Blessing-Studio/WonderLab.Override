using System;
using Avalonia;
using WonderLab.Services;
using Avalonia.Markup.Xaml;
using WonderLab.Views.Pages;
using WonderLab.Services.UI;
using WonderLab.Services.Game;
using WonderLab.Views.Windows;
using Avalonia.Platform.Storage;
using WonderLab.ViewModels.Pages;
using Avalonia.Data.Core.Plugins;
using WonderLab.Services.Download;
using Microsoft.Extensions.Hosting;
using WonderLab.ViewModels.Windows;
using WonderLab.Services.Auxiliary;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Setting;
using WonderLab.Services.Navigation;
using WonderLab.Views.Dialogs.Setting;
using WonderLab.Views.Pages.Navigation;
using WonderLab.ViewModels.Pages.Setting;
using MinecraftLaunch.Components.Fetcher;
using WonderLab.ViewModels.Dialogs.Setting;
using WonderLab.ViewModels.Pages.Navigation;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab;

public sealed partial class App : Application {
    private static IHost _host = default!;

    public static IServiceProvider ServiceProvider => _host.Services;
    public static IStorageProvider StorageProvider { get; private set; }

    public override void Initialize() {
        base.Initialize();
        _host = Build().Build();
    }

    public override void OnFrameworkInitializationCompleted() {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var mainWindow = GetService<MainWindow>();
            StorageProvider = mainWindow.StorageProvider;
            desktop.MainWindow = mainWindow;
            Init();

            mainWindow.DataContext = GetService<MainWindowViewModel>();
            desktop.Exit += (sender, args) => GetService<SettingService>().Save();
        }

        base.OnFrameworkInitializationCompleted();

        void Init() {
            var dataService = GetService<SettingService>();
            GetService<ThemeService>().SetCurrentTheme(dataService.Data.ThemeIndex);
            GetService<LanguageService>().SetLanguage(dataService.Data.LanguageIndex);
            GetService<WindowService>().SetBackground(dataService.Data.BackgroundIndex);
        }

        T GetService<T>() {
            return ServiceProvider.GetRequiredService<T>();
        }
    }

    private static IHostBuilder Build() {
        return Host.CreateDefaultBuilder().ConfigureServices((_, services) => {
            ConfigureServices(services);
        }).ConfigureServices((_, services) => {
            ConfigureView(services);
        });
    }

    private static void ConfigureView(IServiceCollection services) {
        ConfigureViewModel(services);
        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
            => new BackgroundTaskQueue(100)));

        //Pages
        services.AddSingleton<HomePage>();
        
        services.AddSingleton<SettingNavigationPage>();
        services.AddSingleton<DownloadNavigationPage>();
        
        services.AddSingleton<AboutPage>();
        services.AddSingleton<DetailSettingPage>();
        services.AddSingleton<LaunchSettingPage>();
        services.AddSingleton<NetworkSettingPage>();
        services.AddSingleton<AccountSettingPage>();

        //Windows
        services.AddScoped<MainWindow>();

        //Dialog
        services.AddTransient<AuthenticateDialog>();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddScoped<JavaFetcher>();

        services.AddSingleton<LogService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<SkinService>();
        services.AddSingleton<GameService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<WindowService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<SettingService>();
        services.AddSingleton<LanguageService>();
        services.AddSingleton<DownloadService>();
        services.AddSingleton<HostNavigationService>();
        services.AddSingleton<SettingNavigationService>();

        services.AddHostedService<QueuedHostedService>();

        //services.AddScoped<UpdateService>();
        //services.AddScoped<TelemetryService>();
        //services.AddScoped<GameEntryService>();
        //services.AddScoped<NotificationService>();
    }
    
    private static void ConfigureViewModel(IServiceCollection services) {
        services.AddSingleton<HomePageViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        
        services.AddSingleton<SettingNavigationPageViewModel>();
        services.AddSingleton<DownloadNavigationPageViewModel>();

        services.AddSingleton<AboutPageViewModel>();
        services.AddTransient<DetailSettingPageViewModel>();
        services.AddTransient<LaunchSettingPageViewModel>();
        services.AddTransient<AccountSettingPageViewModel>();
        services.AddTransient<NetworkSettingPageViewModel>();
        services.AddTransient<AuthenticateDialogViewModel>();
    }
}