using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WonderLab.Services;
using WonderLab.Services.UI;
using WonderLab.ViewModels;
using WonderLab.ViewModels.Windows;
using WonderLab.Views;
using WonderLab.Views.Windows;

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
            var dataService = ServiceProvider.GetRequiredService<SettingService>();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            StorageProvider = mainWindow.StorageProvider;
            desktop.MainWindow = mainWindow;
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            
            desktop.Exit += (sender, args) => {
                dataService.Save();
            };
        }

        base.OnFrameworkInitializationCompleted();
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
        //services.AddScoped<HomePage>();
        //services.AddScoped<SettingPage>();
        //services.AddScoped<DownloadPage>();
        //services.AddScoped<TaskCenterPage>();
        //services.AddScoped<GameDownloadPage>();
        //services.AddTransient<LaunchSettingPage>();
        //services.AddScoped<NotificationCenterPage>();

        //Windows
        services.AddSingleton<MainWindow>();

        //DialogContent
        //services.AddTransient<UpdateDialogContent>();
    }

    private static void ConfigureServices(IServiceCollection services) {
        services.AddScoped<WindowService>();
        services.AddScoped<SettingService>();
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
        services.AddScoped<MainWindowViewModel>();
    }
}