using System;
using Avalonia.Controls;
using WonderLab.ViewModels;
using WonderLab.Classes.Datas;
using System.Collections.Generic;
using WonderLab.Views.Pages.Oobe;
using WonderLab.Classes.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

public sealed class OobeNavigationService(LogService logService) : INavigationService {
    private readonly LogService _logService = logService;
    private readonly Dictionary<string, Func<object>> _pages = new() {
        { nameof(OobeWelcomePage), App.ServiceProvider.GetRequiredService<OobeWelcomePage> },
    };

    public Action<NavigationPageData> NavigationRequest { get; set; }

    public void NavigationTo<TViewModel>() where TViewModel : ViewModelBase {
        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");
        _logService.Info(nameof(OobeNavigationService), $"The name of the page to be navigated is {viewName}");

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