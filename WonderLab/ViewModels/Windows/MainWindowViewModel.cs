using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Threading.Tasks;
using System.Windows.Input;
using WonderLab.Classes.Managers;
using WonderLab.Views.Pages;
using WonderLab.Views.Pages.Settings;

namespace WonderLab.ViewModels.Windows
{
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel(TaskManager manager, HomePage page) {
            CurrentPage = page;
        }

        [Reactive]
        public bool IsFullScreen { get; set; }

        [Reactive]
        public UserControl CurrentPage { get; set; }

        public ICommand NavigationHomePageCommand
            => ReactiveCommand.Create(NavigationHomePage);

        public ICommand NavigationSettingPageCommand
            => ReactiveCommand.Create(NavigationSettingPage);

        public async void NavigationSettingPage() {
            IsFullScreen = true;
            CurrentPage = App.ServiceProvider.GetRequiredService<SettingPage>();
            await Task.Delay(1000)
                .ContinueWith(x => IsFullScreen = false);
        }

        public void NavigationHomePage() {
            CurrentPage = App.ServiceProvider.GetRequiredService<HomePage>();
        }
    }
}
