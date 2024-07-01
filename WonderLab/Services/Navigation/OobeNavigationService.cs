using System;
using Avalonia.Threading;
using System.Collections.Generic;
using WonderLab.Views.Pages.Oobe;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

public sealed class OobeNavigationService(Dispatcher dispatcher) : NavigationServiceBase(dispatcher) {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(OobeWelcomePage), App.ServiceProvider.GetRequiredService<OobeWelcomePage> },
        { nameof(OobeAccountPage), App.ServiceProvider.GetRequiredService<OobeAccountPage> },
        { nameof(OobeLanguagePage), App.ServiceProvider.GetRequiredService<OobeLanguagePage> },
    };
}