using System;
using Avalonia;
using Avalonia.Controls;
using WonderLab.Services;
using Avalonia.Markup.Xaml;
using WonderLab.Views.Pages;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using WonderLab.Services.Game;
using WonderLab.Views.Windows;
using WonderLab.Views.Dialogs;
using Avalonia.Platform.Storage;
using WonderLab.Views.Pages.Oobe;
using WonderLab.ViewModels.Pages;
using Avalonia.Data.Core.Plugins;
using WonderLab.Services.Download;
using WonderLab.ViewModels.Dialogs;
using Microsoft.Extensions.Hosting;
using WonderLab.ViewModels.Windows;
using WonderLab.Services.Auxiliary;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Setting;
using WonderLab.Services.Navigation;
using WonderLab.ViewModels.Pages.Oobe;
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

    public override async void Initialize() {
        base.Initialize();
        _host = Build().Build();
        await _host.RunAsync();
    }

    public override void OnFrameworkInitializationCompleted() {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var settingService = GetService<SettingService>();
            var isInitialize = settingService.IsInitialize;

            Window window = isInitialize ? GetService<OobeWindow>() : GetService<MainWindow>();
            window.DataContext = isInitialize ? GetService<OobeWindowViewModel>() : GetService<MainWindowViewModel>();

            StorageProvider = window.StorageProvider;
            desktop.MainWindow = window;
            if (window is MainWindow) {
                Init();
            }

            desktop.Exit += async (sender, args) => await Task.Run(() => settingService.Save());
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

        //Pages
        services.AddTransient<HomePage>();
        
        services.AddSingleton<OobeWelcomePage>();

        services.AddSingleton<SettingNavigationPage>();
        services.AddSingleton<DownloadNavigationPage>();

        services.AddSingleton<AboutPage>();
        services.AddSingleton<DetailSettingPage>();
        services.AddSingleton<LaunchSettingPage>();
        services.AddSingleton<NetworkSettingPage>();
        services.AddSingleton<AccountSettingPage>();

        //Windows
        services.AddSingleton<MainWindow>();
        services.AddSingleton<OobeWindow>();

        //Dialog
        services.AddTransient<TestUserCheckDialog>();
        services.AddTransient<ChooseAccountTypeDialog>();
        services.AddTransient<OfflineAuthenticateDialog>();
        services.AddTransient<YggdrasilAuthenticateDialog>();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddHostedService<QueuedHostedService>();
        services.AddScoped<JavaFetcher>();

        services.AddTransient<GameService>();

        services.AddSingleton<LogService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<SkinService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<WindowService>();
        services.AddSingleton<AccountService>();
        services.AddSingleton<SettingService>();
        services.AddSingleton<LanguageService>();
        services.AddSingleton<DownloadService>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<OobeNavigationService>();
        services.AddSingleton<HostNavigationService>();
        services.AddSingleton<SettingNavigationService>();

        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
            => new BackgroundTaskQueue(100)));

        services.AddSingleton((Func<IServiceProvider, IBackgroundNotificationQueue>)((IServiceProvider _)
            => new BackgroundNotificationQueue(200)));
        //services.AddScoped<UpdateService>();
        //services.AddScoped<TelemetryService>();
        //services.AddScoped<GameEntryService>();
    }
    
    private static void ConfigureViewModel(IServiceCollection services) {
        services.AddTransient<HomePageViewModel>();

        //Window
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<OobeWindowViewModel>();

        //Oobe Page
        services.AddSingleton<OobeWelcomePageViewModel>();

        //Navigation Page
        services.AddSingleton<SettingNavigationPageViewModel>();
        services.AddSingleton<DownloadNavigationPageViewModel>();

        //Setting Page
        services.AddSingleton<AboutPageViewModel>();
        services.AddSingleton<DetailSettingPageViewModel>();
        services.AddSingleton<LaunchSettingPageViewModel>();
        services.AddSingleton<AccountSettingPageViewModel>();
        services.AddSingleton<NetworkSettingPageViewModel>();

        //Dialog
        services.AddTransient<TestUserCheckDialogViewModel>();
        services.AddTransient<ChooseAccountTypeDialogViewModel>();
        services.AddTransient<OfflineAuthenticateDialogViewModel>();
        services.AddTransient<YggdrasilAuthenticateDialogViewModel>();
    }
}