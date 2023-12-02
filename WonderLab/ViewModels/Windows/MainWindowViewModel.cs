using ReactiveUI;
using Avalonia.Controls;
using System.Windows.Input;
using WonderLab.Views.Pages;
using ReactiveUI.Fody.Helpers;
using WonderLab.Classes.Managers;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Setting;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Threading;

namespace WonderLab.ViewModels.Windows {
    public class MainWindowViewModel : ViewModelBase {
        private NotificationManager _notificationManager;

        public MainWindowViewModel(HomePage page, NotificationManager manager) {
            CurrentPage = page;
            _notificationManager = manager;
        }

        [Reactive]
        public bool IsFullScreen { get; set; }

        [Reactive]
        public UserControl CurrentPage { get; set; }

        public ObservableCollection<INotification> Notifications
            => _notificationManager.Notifications;

        public ICommand NavigationHomePageCommand
            => ReactiveCommand.Create(NavigationHomePage);

        public ICommand NavigationSettingPageCommand
            => ReactiveCommand.Create(NavigationSettingPage);

        public async void NavigationSettingPage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<SettingPage>();
        }

        public async void NavigationHomePage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<HomePage>();
        }
    }
}
