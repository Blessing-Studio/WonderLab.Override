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
using WonderLab.Views.Pages.Download;
using WonderLab.Classes.Handlers;
using DialogHostAvalonia;
using WonderLab.Views.Dialogs;

namespace WonderLab.ViewModels.Windows {
    public class MainWindowViewModel : ViewModelBase {
        private UpdateHandler _updateHandler;

        private NotificationManager _notificationManager;

        public MainWindowViewModel(HomePage page, UpdateHandler updateHandler,
            NotificationManager manager) {
            CurrentPage = page;
            _updateHandler = updateHandler;
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

        public async void Init() {
            var result = await _updateHandler.CheckAsync();
            if (!result) {
                await Dispatcher.UIThread.InvokeAsync(async () => {
                    var content = App.ServiceProvider.GetRequiredService<UpdateDialogContent>();
                    await DialogHost.Show(content, "dialogHost");
                });
            }
        }

        public void NavigationSettingPage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<SettingPage>();
        }

        public void NavigationHomePage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<HomePage>();
        }

        public void NavigationDownloadPageCommand() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<DownloadPage>();
        }
    }
}
