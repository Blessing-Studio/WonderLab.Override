using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Hosting;
using System;
using WonderLab.ViewModels;
using WonderLab.Views;

namespace WonderLab;

public sealed partial class App : Application {
    private static IHost _host = default!;

    public static IServiceProvider ServiceProvider => _host.Services;
    public static IStorageProvider StorageProvider { get; private set; }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}