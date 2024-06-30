using System;
using Avalonia.Threading;
using System.Collections.Generic;
using WonderLab.Views.Pages.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

/// <summary>
/// 设置页导航服务
/// </summary>
public sealed class SettingNavigationService(Dispatcher dispatcher) : NavigationServiceBase(dispatcher) {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(AboutPage), App.ServiceProvider.GetRequiredService<AboutPage> },
        { nameof(LaunchSettingPage), App.ServiceProvider.GetRequiredService<LaunchSettingPage> },
        { nameof(DetailSettingPage), App.ServiceProvider.GetRequiredService<DetailSettingPage> },
        { nameof(AccountSettingPage), App.ServiceProvider.GetRequiredService<AccountSettingPage> },
        { nameof(NetworkSettingPage), App.ServiceProvider.GetRequiredService<NetworkSettingPage> },
    };
}