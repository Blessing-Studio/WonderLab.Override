using System;
using Avalonia.Controls;
using WonderLab.ViewModels;
using WonderLab.Views.Pages;
using System.Collections.Generic;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Navigation;
using Microsoft.Extensions.DependencyInjection;
using WonderLab.Classes.Datas;

namespace WonderLab.Services.Navigation;

/// <summary>
/// 主体页面导航服务
/// </summary>
public sealed class HostNavigationService(LogService logService) : INavigationService {
    private readonly LogService _logService = logService;
    private readonly Dictionary<string, Func<object>> _pages = new() {
        { nameof(HomePage), App.ServiceProvider.GetRequiredService<HomePage> },
        { nameof(DownloadNavigationPage), App.ServiceProvider.GetRequiredService<DownloadNavigationPage> },
        { nameof(SettingNavigationPage), App.ServiceProvider.GetRequiredService<SettingNavigationPage> },
    };
    
    public Action<NavigationPageData> NavigationRequest { get; set; }

    public void NavigationTo<TViewModel>() where TViewModel : ViewModelBase {
        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");
        _logService.Info(nameof(HostNavigationService), $"The name of the page to be navigated is {viewName}");
        
        if (_pages.TryGetValue(viewName, out var pageFunc)) {
            object pageObject = pageFunc();
            (pageObject as UserControl)!.DataContext = App.ServiceProvider!.GetRequiredService<TViewModel>();
            NavigationRequest?.Invoke(new NavigationPageData {
                Page = pageObject,
                PageKey = viewName,
            });
        }
    }
}