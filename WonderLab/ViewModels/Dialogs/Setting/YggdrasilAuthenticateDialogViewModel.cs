using System;
using System.Linq;
using WonderLab.Services;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using WonderLab.Services.Auxiliary;
using MinecraftLaunch.Classes.Enums;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Classes.Datas.MessageData;
using CommunityToolkit.Mvvm.ComponentModel;
using MinecraftLaunch.Components.Authenticator;

namespace WonderLab.ViewModels.Dialogs.Setting;

public sealed partial class YggdrasilAuthenticateDialogViewModel : DialogViewModelBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly AccountService _accountService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private string _url;
    [ObservableProperty] private string _email;
    [ObservableProperty] private string _password;

    public YggdrasilAuthenticateDialogViewModel(
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
    private async Task Authenticate() {
        try {
            _accountService.InitializeComponent(new YggdrasilAuthenticator(Url, Email, Password), AccountType.Yggdrasil);
            var accounts = await _accountService.AuthenticateAsync(3);
            _settingService.Data.Accounts.AddRange(accounts);

            _notificationService.QueueJob(new NotificationViewData {
                Title = "成功",
                Content = $"已成功将账户 {string.Join(",", accounts.Select(x => x.Name))} 添加至 WonderLab！",
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