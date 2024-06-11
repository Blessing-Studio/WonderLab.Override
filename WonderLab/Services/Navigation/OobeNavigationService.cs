using System;
using System.Collections.Generic;
using WonderLab.Views.Pages.Oobe;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.Services.Navigation;

public sealed class OobeNavigationService : NavigationServiceBase {
    public override Dictionary<string, Func<object>> FuncPages { get; } = new() {
        { nameof(OobeWelcomePage), App.ServiceProvider.GetRequiredService<OobeWelcomePage> },
        { nameof(OobeAccountPage), App.ServiceProvider.GetRequiredService<OobeAccountPage> },
        { nameof(OobeLanguagePage), App.ServiceProvider.GetRequiredService<OobeLanguagePage> },
    };
}