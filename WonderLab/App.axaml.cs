using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
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
using CommunityToolkit.Mvvm.Messaging;
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

    public override void RegisterServices() {
        base.RegisterServices();

        var bulider = CreateHostBuilder();
        _host = bulider.Build();
        _host.Start();
    }

    public override async void OnFrameworkInitializationCompleted() {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var isInitialize = ServiceProvider.GetRequiredService<SettingService>().IsInitialize;

            Window window = isInitialize ? GetService<OobeWindow>() : GetService<MainWindow>();
            StorageProvider = window.StorageProvider;
            desktop.MainWindow = window;

            await Task.Delay(TimeSpan.FromMilliseconds(50));
            window.DataContext = isInitialize ? GetService<OobeWindowViewModel>() : GetService<MainWindowViewModel>();

            desktop.Exit += (sender, args) => _host.StopAsync();
        }

        base.OnFrameworkInitializationCompleted();

        T GetService<T>() {
            return ServiceProvider.GetRequiredService<T>();
        }
    }

    private static IHostBuilder CreateHostBuilder() {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureServices(ConfigureView)
            .ConfigureServices(services => {
                services.AddSingleton<JavaFetcher>();
                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton(_ => Dispatcher.UIThread);
            });
        
        return builder;
    }

    private static void ConfigureView(IServiceCollection services) {
        ConfigureViewModel(services);

        //Pages
        services.AddTransient<HomePage>();
        
        services.AddSingleton<OobeWelcomePage>();
        services.AddSingleton<OobeAccountPage>();
        services.AddSingleton<OobeLanguagePage>();

        services.AddSingleton<SettingNavigationPage>();
        services.AddSingleton<DownloadNavigationPage>();

        services.AddSingleton<AboutPage>();
        services.AddSingleton<DetailSettingPage>();
        services.AddSingleton<LaunchSettingPage>();
        services.AddSingleton<NetworkSettingPage>();
        services.AddSingleton<AccountSettingPage>();

        //Windows
        services.AddSingleton<LogWindow>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<OobeWindow>();

        //Dialog
        services.AddTransient<TestUserCheckDialog>();
        services.AddTransient<ChooseAccountTypeDialog>();
        services.AddTransient<OfflineAuthenticateDialog>();
        services.AddTransient<YggdrasilAuthenticateDialog>();
        services.AddTransient<MicrosoftAuthenticateDialog>();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddTransient<GameService>();

        services.AddSingleton<LogService>();
        services.AddSingleton<TaskService>();
        services.AddSingleton<SkinService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<WindowService>();
        services.AddSingleton<SettingService>();
        services.AddSingleton<AccountService>();
        services.AddSingleton<LanguageService>();
        services.AddSingleton<DownloadService>();
        services.AddSingleton<NotificationService>();
        services.AddSingleton<OobeNavigationService>();
        services.AddSingleton<HostNavigationService>();
        services.AddSingleton<SettingNavigationService>();

        services.AddHostedService<QueuedHostedService>();
        services.AddHostedService<SettingBackgroundService>();

        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
            => new BackgroundTaskQueue(100)));

        services.AddSingleton((Func<IServiceProvider, IBackgroundNotificationQueue>)((IServiceProvider _)
            => new BackgroundNotificationQueue(200)));
        //services.AddScoped<UpdateService>();
        //services.AddScoped<TelemetryService>();
    }
    
    private static void ConfigureViewModel(IServiceCollection services) {
        services.AddTransient<HomePageViewModel>();

        //Window
        services.AddSingleton<LogWindowViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<OobeWindowViewModel>();

        //Oobe Page
        services.AddSingleton<OobeWelcomePageViewModel>();
        services.AddSingleton<OobeAccountPageViewModel>();
        services.AddSingleton<OobeLanguagePageViewModel>();

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
        services.AddTransient<MicrosoftAuthenticateDialogViewModel>();
    }
}