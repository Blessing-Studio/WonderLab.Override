using System;
using Avalonia.Controls;
using Avalonia.Threading;
using WonderLab.ViewModels;
using WonderLab.Classes.Datas;
using System.Collections.Generic;
using WonderLab.Classes.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;
public abstract class NavigationServiceBase : INavigationService {
    private readonly Dispatcher _dispatcher;

    public Action<NavigationPageData> NavigationRequest { get; set; }
    public virtual Dictionary<string, Func<object>> FuncPages { get; }

    public NavigationServiceBase(Dispatcher dispatcher) {
        _dispatcher = dispatcher;
    }

    public virtual async void NavigationTo<TViewModel>() where TViewModel : ViewModelBase {
        var viewName = typeof(TViewModel).Name.Replace("ViewModel", "");

        await _dispatcher.InvokeAsync(() => {
            if (FuncPages.TryGetValue(viewName, out var pageFunc)) {
                object pageObject = pageFunc();
                (pageObject as UserControl)!.DataContext = App.ServiceProvider!.GetRequiredService<TViewModel>();
                NavigationRequest?.Invoke(new NavigationPageData {
                    Page = pageObject,
                    PageKey = viewName,
                });
            }
        });
    }
}