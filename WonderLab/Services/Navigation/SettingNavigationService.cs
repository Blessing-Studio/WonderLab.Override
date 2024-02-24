using System;
using Avalonia.Controls;
using WonderLab.ViewModels;
using System.Collections.Generic;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Setting;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

/// <summary>
/// 设置页导航服务
/// </summary>
public sealed class SettingNavigationService(LogService logService) : INavigationService {
    private readonly LogService _logService = logService;
    private readonly Dictionary<string, object> _pageObjects = new() {
        { nameof(LaunchSettingPage), App.ServiceProvider.GetRequiredService<LaunchSettingPage>() },
        { nameof(DetailSettingPage), App.ServiceProvider.GetRequiredService<DetailSettingPage>() },
        { nameof(AccountSettingPage), App.ServiceProvider.GetRequiredService<AccountSettingPage>() },
        { nameof(NetworkSettingPage), App.ServiceProvider.GetRequiredService<NetworkSettingPage>() },
    };
    
    public Action<object>? NavigationRequest { get; set; }

    public void NavigationTo<TViewModel>() where TViewModel : ViewModelBase {
        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");
        _logService.Info(nameof(SettingNavigationService), $"The name of the page to be navigated is {viewName}");
        
        if (_pageObjects.TryGetValue(viewName, out var pageObject)) {
            (pageObject as UserControl)!.DataContext = App.ServiceProvider.GetRequiredService<TViewModel>();
            NavigationRequest?.Invoke(pageObject);
        }
    }
}