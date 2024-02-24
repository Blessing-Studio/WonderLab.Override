using System;
using Avalonia.Controls;
using WonderLab.ViewModels;
using WonderLab.Views.Pages;
using System.Collections.Generic;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

/// <summary>
/// 主体页面导航服务
/// </summary>
public sealed class HostNavigationService(LogService logService) : INavigationService {
    private readonly LogService _logService = logService;
    private readonly Dictionary<string, object> _pages = new() {
        { nameof(HomePage), App.ServiceProvider.GetRequiredService<HomePage>() },
        { nameof(DownloadNavigationPage), App.ServiceProvider.GetRequiredService<DownloadNavigationPage>() },
        { nameof(SettingNavigationService), App.ServiceProvider.GetRequiredService<SettingNavigationService>() },
    };
    
    public Action<object>? NavigationRequest { get; set; }

    public void NavigationTo<TViewModel>() where TViewModel : ViewModelBase {
        var viewName = typeof(TViewModel).Name;
        _logService.Info(nameof(HostNavigationService), $"The name of the page to be navigated is {viewName}");
        
        if (_pages.TryGetValue(viewName.Replace("ViewModel", ""), out var pageObject)) {
            (pageObject as UserControl)!.DataContext = App.ServiceProvider!.GetRequiredService<TViewModel>();
            NavigationRequest?.Invoke(pageObject);
        }
    }
}