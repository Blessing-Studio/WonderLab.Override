using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using WonderLab.Views.Pages.Settings;

namespace WonderLab.ViewModels.Pages.Settings {
    public class SettingPageViewModel : ViewModelBase {
        public SettingPageViewModel() {
            var page = App.ServiceProvider
                .GetRequiredService<LaunchSettingPage>();
            Current = page;
        }

        [Reactive]
        public object Current {  get; set; }
    }
}
