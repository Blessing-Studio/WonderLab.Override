using System;
using Avalonia;
using ReactiveUI;
using Avalonia.Markup.Xaml;
using WonderLab.Views.Pages;
using System.Threading.Tasks;
using WonderLab.Views.Windows;
using Avalonia.Platform.Storage;
using WonderLab.Classes.Handlers;
using WonderLab.Classes.Managers;
using WonderLab.ViewModels.Pages;
using System.Reactive.Concurrency;
using WonderLab.Classes.Utilities;
using Microsoft.Extensions.Hosting;
using WonderLab.Classes.Interfaces;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Pages.Setting;
using WonderLab.ViewModels.Pages.Setting;
using WonderLab.Views.Pages.ControlCenter;
using Avalonia.Controls.ApplicationLifetimes;
using WonderLab.ViewModels.Pages.ControlCenter;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab;

public partial class App : Application {
    private static IHost _host = default!;    

    public static IServiceProvider ServiceProvider => _host.Services;

    public static IStorageProvider StorageProvider { get; private set; }

    public override async void Initialize() {
        base.Initialize();
        _host = HostBuilder().Build();
        await _host.RunAsync();
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var dataHandler = ServiceProvider.GetRequiredService<ConfigDataHandler>();
            RxApp.MainThreadScheduler.Schedule(dataHandler
                .Load);

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            StorageProvider = mainWindow.StorageProvider;
            desktop.MainWindow = mainWindow;
            desktop.Exit += async (sender, args) => {
                await dataHandler.SaveAsync();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static async ValueTask StopHostAsync() {
        using (_host) {
            await _host.StopAsync();
        }
    }

    private static IHostBuilder HostBuilder() {
        return Host.CreateDefaultBuilder().ConfigureServices((_, services) => {
            ConfigureHandlers(services);
        }).ConfigureServices((_, services) => {
            ConfigureLogging(services);
        }).ConfigureServices((_, services) => {
            ConfigureView(services);
        });
    }

    private static void ConfigureLogging(IServiceCollection services) {

    }

    private static void ConfigureView(IServiceCollection services) {
        ConfigureViewModels(services);
        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _) 
            => new BackgroundTaskQueue(100)));

        services.AddScoped<HomePage>();
        services.AddScoped<SettingPage>();
        services.AddScoped<TaskCenterPage>();
        services.AddWindowFactory<MainWindow>();
        services.AddScoped<LaunchSettingPage>();
        services.AddScoped<NotificationCenterPage>();

        ConfigureManagers(services);
    }

    private static void ConfigureHandlers(IServiceCollection services) {
        services.AddScoped<ConfigDataHandler>();
        services.AddHostedService<QueuedHostedHandler>();
    }

    private static void ConfigureViewModels(IServiceCollection services) {
        services.AddScoped<HomePageViewModel>();
        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<SettingPageViewModel>();
        services.AddScoped<TaskCenterPageViewModel>();
        services.AddScoped<LaunchSettingPageViewModel>();
        services.AddScoped<NotificationCenterPageViewModel>();
    }

    private static void ConfigureManagers(IServiceCollection services) {
        services.AddScoped<DataManager>();
        services.AddScoped<ThemeManager>();
        services.AddSingleton<TaskManager>();
        services.AddScoped<NotificationManager>();
    }
}
