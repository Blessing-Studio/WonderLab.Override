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
using NotificationService = WonderLab.Services.UI.NotificationService;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Windows;

public sealed partial class MainWindowViewModel : ViewModelBase {
    private readonly UpdateService _updateService;
    private readonly NavigationService _navigationService;
    private readonly NotificationService _notificationService;

    public MainWindowViewModel(
        UpdateService updateService,
        NavigationService navigationService,
        NotificationService notificationService) {
        _updateService = updateService;
        _navigationService = navigationService;
        _notificationService = notificationService;

        _navigationService.Navigation("HomePage");
    }

    [ObservableProperty]
    private bool isFullScreen;

    public ObservableCollection<INotification> Notifications
        => _notificationService.Notifications;

    public async void Init() {
        _notificationService.Info("开始初始化基本服务项，可能会有些许卡顿，不必担心一切正常！");
        var result = await _updateService.CheckAsync();
        var d = App.ServiceProvider.GetRequiredService<DownloadPage>();
        if (result) {
            await Dispatcher.UIThread.InvokeAsync(async () => {
                var content = App.ServiceProvider.GetRequiredService<UpdateDialogContent>();
                await DialogHost.Show(content, "dialogHost");
            });
        }
    }

    [RelayCommand]
    private void NavigationSettingPage() {
        _navigationService.Navigation("Setting.SettingPage");
    }

    [RelayCommand]
    private void NavigationHomePage() {
        _navigationService.Navigation("HomePage");
    }

    [RelayCommand]
    private void NavigationDownloadPage() {
        _navigationService.Navigation("Download.DownloadPage");
    }
}