using Avalonia.Controls;
using Avalonia.Threading;
using DialogHostAvalonia;
using WonderLab.Services;
using WonderLab.Views.Pages;
using WonderLab.Views.Dialogs;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Classes.Interfaces;
using WonderLab.Views.Pages.Setting;
using System.Collections.ObjectModel;
using WonderLab.Views.Pages.Download;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace WonderLab.ViewModels.Windows {
    public partial class MainWindowViewModel : ViewModelBase {
        private UpdateService _updateService;
        private NotificationService _notificationService;

        public MainWindowViewModel(HomePage page,
            UpdateService updateService,
            NotificationService notificationService) {
            CurrentPage = page;
            _updateService = updateService;
            _notificationService = notificationService;
        }

        [ObservableProperty]
        public bool isFullScreen;

        [ObservableProperty]
        public UserControl currentPage;

        public ObservableCollection<INotification> Notifications
            => _notificationService.Notifications;

        public async void Init() {
            var result = await _updateService.CheckAsync();
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
