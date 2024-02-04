using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;
using WonderLab.Classes.Utilities;
using WonderLab.Services;
using WonderLab.Services.UI;
using WonderLab.ViewModels.Dialogs;
using WonderLab.ViewModels.Pages;
using WonderLab.ViewModels.Pages.ControlCenter;
using WonderLab.ViewModels.Pages.Download;
using WonderLab.ViewModels.Pages.Setting;
using WonderLab.ViewModels.Windows;
using WonderLab.Views.Dialogs;
using WonderLab.Views.Pages;
using WonderLab.Views.Pages.ControlCenter;
using WonderLab.Views.Pages.Download;
using WonderLab.Views.Pages.Setting;
using WonderLab.Views.Windows;
using NotificationService = WonderLab.Services.UI.NotificationService;

namespace WonderLab;

public partial class App : Application
{
    private static IHost _host = default!;

    public static IServiceProvider ServiceProvider => _host.Services;

    public static IStorageProvider StorageProvider { get; private set; }

    public override async void Initialize()
    {
        base.Initialize();
        _host = HostBuilder().Build();
        await _host.RunAsync();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dataService = ServiceProvider.GetRequiredService<DataService>();
            dataService.Load();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            StorageProvider = mainWindow.StorageProvider;
            desktop.MainWindow = mainWindow;
            desktop.Exit += async (sender, args) =>
            {
                await dataService.SaveAsync();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }


    public static async ValueTask StopHostAsync()
    {
        using (_host)
        {
            await _host.StopAsync();
        }
    }

    private static IHostBuilder HostBuilder()
    {
        return Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
        {
            ConfigureServices(services);
        }).ConfigureServices((_, services) =>
        {
            ConfigureView(services);
        });
    }

    private static void ConfigureView(IServiceCollection services)
    {
        ConfigureViewModels(services);
        services.AddSingleton((Func<IServiceProvider, IBackgroundTaskQueue>)((IServiceProvider _)
            => new BackgroundTaskQueue(100)));

        //Pages
        services.AddScoped<HomePage>();
        services.AddScoped<SettingPage>();
        services.AddScoped<DownloadPage>();
        services.AddScoped<TaskCenterPage>();
        services.AddScoped<GameDownloadPage>();
        services.AddTransient<LaunchSettingPage>();
        services.AddScoped<NotificationCenterPage>();

        //Windows
        services.AddWindowFactory<MainWindow>();

        //DialogContent
        services.AddTransient<UpdateDialogContent>();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<DataService>();
        services.AddScoped<TaskService>();
        services.AddScoped<UpdateService>();
        services.AddScoped<DownloadService>();
        services.AddScoped<TelemetryService>();
        services.AddScoped<GameEntryService>();
        services.AddScoped<NavigationService>();
        services.AddScoped<NotificationService>();
        services.AddHostedService<QueuedHostedService>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        //Pages
        services.AddScoped<HomePageViewModel>();
        services.AddScoped<SettingPageViewModel>();
        services.AddScoped<DownloadPageViewModel>();
        services.AddScoped<TaskCenterPageViewModel>();
        services.AddScoped<GameDownloadPageViewModel>();
        services.AddScoped<LaunchSettingPageViewModel>();
        services.AddScoped<NotificationCenterPageViewModel>();

        //Windows
        services.AddScoped<MainWindowViewModel>();

        //Dialog
        services.AddTransient<UpdateDialogContentViewModel>();
    }
}
