using System;
using WonderLab.Services;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Services.Auxiliary;
using Avalonia.Controls.Notifications;
using WonderLab.Classes.Datas.ViewData;
using MinecraftLaunch.Components.Authenticator;
using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;

namespace WonderLab.ViewModels.Dialogs.Setting;

public sealed partial class OfflineAuthenticateDialogViewModel : DialogViewModelBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly AccountService _accountService;
    private readonly NotificationService _notificationService;

    public OfflineAuthenticateDialogViewModel(
        DialogService dialogService,
        AccountService accountService,
        SettingService settingService,
        NotificationService notificationService) {
        _dialogService = dialogService;
        _accountService = accountService;
        _settingService = settingService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task Authenticate(string name) {
        try {
            _accountService.InitializeComponent(new OfflineAuthenticator(name));
            var accounts = await _accountService.AuthenticateAsync(1);
            _settingService.Data.Accounts.AddRange(accounts);

            _notificationService.QueueJob(new NotificationViewData {
                Title = "成功",
                Content = $"已成功将账户 {name} 添加至 WonderLab！",
                NotificationType = NotificationType.Success
            });

            WeakReferenceMessenger.Default.Send(new AccountMessage(accounts));
            if (_dialogService.IsDialogOpen) {
                _dialogService.CloseContentDialog();
            }
        } catch (Exception ex) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "错误",
                Content = $"{ex.Message}",
                NotificationType = NotificationType.Error
            });
        }
    }
}