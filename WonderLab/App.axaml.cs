using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using MinecraftLaunch.Components.Fetcher;

using System;
using Serilog;
using System.IO;

using WonderLab.Services;
using WonderLab.Services.Auxiliary;
using WonderLab.Services.Download;
using WonderLab.Services.Game;
using WonderLab.Services.Navigation;
using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using WonderLab.ViewModels.Dialogs;
using WonderLab.ViewModels.Dialogs.Setting;
using WonderLab.Classes.Interfaces;
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
using WonderLab.Classes;
using WonderLab.Views.Dialogs.Multiplayer;
using WonderLab.ViewModels.Dialogs.Multiplayer;

namespace WonderLab;

public sealed partial class App : Application {
    private const string CONNECTION_STRING = "InstrumentationKey=2fd6d1c2-c40c-4a49-87bf-6883f625a901;IngestionEndpoint=https://australiaeast-1.in.applicationinsights.azure.com/;LiveEndpoint=https://australiaeast.livediagnostics.monitor.azure.com/;ApplicationId=bb052d56-b930-4bcd-94dc-97fe2b6111f4";

    private static IHost _host = default!;
    public static IServiceProvider ServiceProvider => _host.Services;

    public override void RegisterServices() {
        base.RegisterServices();

        var bulider = CreateHostBuilder();
        _host = bulider.Build();
        _host.Start();
    }

    public override async void OnFrameworkInitializationCompleted() {
        BindingPlugins.DataValidators.RemoveAt(0);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            Window window = SettingService.IsInitialize ? GetService<OobeWindow>() : GetService<MainWindow>();
            desktop.MainWindow = window;

            window.DataContext = SettingService.IsInitialize ? GetService<OobeWindowViewModel>() : GetService<MainWindowViewModel>();
            desktop.Exit += async (sender, args) => await _host.StopAsync();
        }

        base.OnFrameworkInitializationCompleted();

        T GetService<T>() {
            return ServiceProvider.GetRequiredService<T>();
        }
    }

    private static IHostBuilder CreateHostBuilder() {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureApplicationInsights)
            .ConfigureServices(ConfigureServices)
            .ConfigureServices(ConfigureView)
            .ConfigureServices(services => {
                services.AddSingleton<JavaFetcher>();
                services.AddSingleton<WeakReferenceMessenger>();
                services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
                services.AddSingleton(_ => Dispatcher.UIThread);
            })
            .ConfigureLogging(builder => {
                builder.ClearProviders();
                Log.Logger = new LoggerConfiguration()
                .Enrich
                .FromLogContext()
                .WriteTo.ApplicationInsights(new TelemetryConfiguration() {
                    ConnectionString = CONNECTION_STRING,
                }, TelemetryConverter.Traces)
                .WriteTo.File(Path.Combine("logs", $"WonderLog.log"),
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] ({SourceContext}): {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

                builder.AddSerilog(Log.Logger);
            });

        return builder;
    }

    private static void ConfigureApplicationInsights(IServiceCollection services) {
        services.AddApplicationInsightsTelemetryWorkerService(option => {
            option.ConnectionString = CONNECTION_STRING;
            option.EnableDebugLogger = false;
        });
    }

    private static void ConfigureView(IServiceCollection services) {
        ConfigureViewModel(services);

        //Pages
        services.AddTransient<HomePage>();
        services.AddSingleton<MultiplayerPage>();

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
        services.AddTransient<RecheckToOobeDialog>();
        services.AddTransient<JoinMutilplayerDialog>();
        services.AddTransient<CreateMutilplayerDialog>();
        services.AddTransient<ChooseAccountTypeDialog>();
        services.AddTransient<OfflineAuthenticateDialog>();
        services.AddTransient<YggdrasilAuthenticateDialog>();
        services.AddTransient<MicrosoftAuthenticateDialog>();
        services.AddTransient<JoinMutilplayerRequestDialog>();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddTransient<GameService>();

        services.AddSingleton<TaskService>();
        services.AddSingleton<WrapService>();
        services.AddSingleton<SkinService>();
        services.AddSingleton<ThemeService>();
        services.AddSingleton<UPnPService>();
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

        services.AddHostedService<QueuedHostedService>();
        services.AddHostedService<SettingBackgroundService>();

        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
            => new BackgroundTaskQueue(100)));

        services.AddSingleton((Func<IServiceProvider, IBackgroundNotificationQueue>)((IServiceProvider _)
            => new BackgroundNotificationQueue(200)));
        //services.AddScoped<TelemetryService>();
    }

    private static void ConfigureViewModel(IServiceCollection services) {
        services.AddTransient<HomePageViewModel>();
        services.AddSingleton<MultiplayerPageViewModel>();

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
        services.AddTransient<RecheckToOobeDialogViewModel>();
        services.AddTransient<JoinMutilplayerDialogViewModel>();
        services.AddTransient<CreateMutilplayerDialogViewModel>();
        services.AddTransient<ChooseAccountTypeDialogViewModel>();
        services.AddTransient<OfflineAuthenticateDialogViewModel>();
        services.AddTransient<YggdrasilAuthenticateDialogViewModel>();
        services.AddTransient<MicrosoftAuthenticateDialogViewModel>();
        services.AddTransient<JoinMutilplayerRequestDialogViewModel>();
    }
}