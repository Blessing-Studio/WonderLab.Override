using Avalonia.Controls;
using System.Windows.Input;
using WonderLab.Views.Pages;
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.ViewModels.Windows {
    public partial class MainWindowViewModel : ViewModelBase {
        private UpdateHandler _updateHandler;

        private NotificationManager _notificationManager;

        public MainWindowViewModel(HomePage page, UpdateHandler updateHandler,
            NotificationManager manager) {
            CurrentPage = page;
            _updateHandler = updateHandler;
            _notificationManager = manager;
        }

        [ObservableProperty]
        public bool isFullScreen;

        [ObservableProperty]
        public UserControl currentPage;

        public ObservableCollection<INotification> Notifications
            => _notificationManager.Notifications;

        public async void Init() {
            var result = await _updateHandler.CheckAsync();
            if (result) {
                await Dispatcher.UIThread.InvokeAsync(async () => {
                    var content = App.ServiceProvider.GetRequiredService<UpdateDialogContent>();
                    await DialogHost.Show(content, "dialogHost");
                });
            }
        }

        [RelayCommand]
        public void NavigationSettingPage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<SettingPage>();
        }

        [RelayCommand]
        public void NavigationHomePage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<HomePage>();
        }

        [RelayCommand]
        public void NavigationDownloadPage() {
            CurrentPage = App.ServiceProvider
                .GetRequiredService<DownloadPage>();
        }
    }
}
