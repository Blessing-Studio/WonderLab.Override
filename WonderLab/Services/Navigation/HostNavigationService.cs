using System;
using WonderLab.Views.Pages;
using System.Collections.Generic;
using WonderLab.Views.Pages.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Threading;

namespace WonderLab.Services.Navigation;

/// <summary>
/// 主体页面导航服务
/// </summary>
public sealed class HostNavigationService(Dispatcher dispatcher) : NavigationServiceBase(dispatcher) {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(HomePage), App.ServiceProvider.GetRequiredService<HomePage> },
        { nameof(MultiplayerPage), App.ServiceProvider.GetRequiredService<MultiplayerPage> },
        { nameof(SettingNavigationPage), App.ServiceProvider.GetRequiredService<SettingNavigationPage> },
        { nameof(DownloadNavigationPage), App.ServiceProvider.GetRequiredService<DownloadNavigationPage> },
    };
}