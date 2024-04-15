using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Data.Core.Plugins;
using Microsoft.Extensions.Hosting;
using WonderLab.Services;
using WonderLab.Services.Game;
using WonderLab.Services.Navigation;
using WonderLab.Views.Windows;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Pages;
using WonderLab.Views.Pages.Navigation;
using WonderLab.Views.Pages.Setting;
using WonderLab.ViewModels.Pages.Setting;
using MinecraftLaunch.Components.Fetcher;
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
        var dataService = ServiceProvider.GetRequiredService<SettingService>();
        var themeService = ServiceProvider.GetRequiredService<ThemeService>();
        var windowService = ServiceProvider.GetRequiredService<WindowService>();
        var languageService = ServiceProvider.GetRequiredService<LanguageService>();

        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            StorageProvider = mainWindow.StorageProvider;
            desktop.MainWindow = mainWindow;
            Init();

            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.Exit += (sender, args) => dataService.Save();
        }

        base.OnFrameworkInitializationCompleted();

        void Init() {
            themeService.SetCurrentTheme(dataService.Data.ThemeIndex);
            languageService.SetLanguage(dataService.Data.LanguageIndex);
            windowService.SetBackground(dataService.Data.BackgroundIndex);
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
        //services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
        //    => new BackgroundTaskQueue(100)));

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

        //DialogContent
        //services.AddTransient<UpdateDialogContent>();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddScoped<JavaFetcher>();

        services.AddSingleton<LogService>();
        services.AddSingleton<GameService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<WindowService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<SettingService>();
        services.AddSingleton<LanguageService>();
        services.AddSingleton<HostNavigationService>();
        services.AddSingleton<SettingNavigationService>();

        //services.AddScoped<TaskService>();
        //services.AddScoped<UpdateService>();
        //services.AddScoped<DownloadService>();
        //services.AddScoped<TelemetryService>();
        //services.AddScoped<GameEntryService>();
        //services.AddScoped<NavigationService>();
        //services.AddScoped<NotificationService>();
        //services.AddHostedService<QueuedHostedService>();
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
    }
}