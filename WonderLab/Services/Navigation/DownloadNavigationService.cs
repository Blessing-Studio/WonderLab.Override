using System;
using Avalonia.Threading;
using WonderLab.Views.Download;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

public sealed class DownloadNavigationService(Dispatcher dispatcher) : NavigationServiceBase(dispatcher) {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(SearchPage), App.ServiceProvider.GetRequiredService<SearchPage> },
    };
}