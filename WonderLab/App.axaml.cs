using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinecraftLaunch.Components.Fetcher;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Services;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Download;
using WonderLab.Services.Game;
using WonderLab.Services.Navigation;
using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using WonderLab.ViewModels.Dialogs;
using WonderLab.ViewModels.Dialogs.Setting;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.Navigation;
using WonderLab.ViewModels.Pages.Oobe;
using WonderLab.ViewModels.Pages.Setting;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Dialogs;
using WonderLab.Views.Dialogs.Setting;
using WonderLab.Views.Pages;
using WonderLab.Views.Pages.Navigation;
using WonderLab.Views.Pages.Oobe;
using WonderLab.Views.Pages.Setting;
using WonderLab.Views.Windows;

namespace WonderLab;

public sealed partial class App : Application
{
    private static IHost _host = default!;

    public static IServiceProvider ServiceProvider => _host.Services;

    public static IStorageProvider StorageProvider { get; private set; }

    public override void RegisterServices()
    {
        base.RegisterServices();

        var bulider = CreateHostBuilder();
        _host = bulider.Build();
        _host.Start();
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var isInitialize = ServiceProvider.GetRequiredService<SettingService>().IsInitialize;

            Window window = isInitialize ? GetService<OobeWindow>() : GetService<MainWindow>();
            StorageProvider = window.StorageProvider;
            desktop.MainWindow = window;

            await Task.Delay(TimeSpan.FromMilliseconds(50));
            window.DataContext = isInitialize ? GetService<OobeWindowViewModel>() : GetService<MainWindowViewModel>();

            desktop.Exit += (sender, args) => _host.StopAsync();
        }

        base.OnFrameworkInitializationCompleted();

        T GetService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }
    }

    private static IHostBuilder CreateHostBuilder()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .ConfigureServices(ConfigureView)
            .ConfigureServices(services =>
            {
                services.AddSingleton<JavaFetcher>();
                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton(_ => Dispatcher.UIThread);
            })
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                Log.Logger = new LoggerConfiguration()
                .Enrich
                .FromLogContext()
                .WriteTo.File(Path.Combine("logs", $"WonderLog.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] ({SourceContext}): {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

                builder.AddSerilog(Log.Logger);
            });

        return builder;
    }

    private static void ConfigureView(IServiceCollection services)
    {
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
        services.AddSingleton<MainWindow>();
        services.AddSingleton<OobeWindow>();

        //Dialog
        services.AddTransient<TestUserCheckDialog>();
        services.AddTransient<ChooseAccountTypeDialog>();
        services.AddTransient<OfflineAuthenticateDialog>();
        services.AddTransient<YggdrasilAuthenticateDialog>();
        services.AddTransient<MicrosoftAuthenticateDialog>();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<GameService>();

        services.AddSingleton<TaskService>();
        services.AddSingleton<SkinService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<UpdateService>();
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
        services.AddSingleton<UPnPService>();
        services.AddSingleton<WrapService>();

        services.AddHostedService<QueuedHostedService>();
        services.AddHostedService<SettingBackgroundService>();

        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
            => new BackgroundTaskQueue(100)));

        services.AddSingleton((Func<IServiceProvider, IBackgroundNotificationQueue>)((IServiceProvider _)
            => new BackgroundNotificationQueue(200)));
        //services.AddScoped<TelemetryService>();
    }

    private static void ConfigureViewModel(IServiceCollection services)
    {
        services.AddTransient<HomePageViewModel>();

        //Window
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