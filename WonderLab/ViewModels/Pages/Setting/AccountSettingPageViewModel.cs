using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.ViewModels.Dialogs.Setting;
using WonderLab.Services;
using System.Collections.ObjectModel;
using WonderLab.Classes.Datas.ViewData;
using System.Linq;
using WonderLab.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.MessageData;
using System.Threading.Tasks;
using WonderLab.Classes.Datas.TaskData;
using Avalonia.Controls.Notifications;
using MinecraftLaunch.Classes.Models.Auth;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountSettingPageViewModel : ViewModelBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private object _activeAccount;
    [ObservableProperty] private ObservableCollection<AccountViewData> _accounts = [];

    public AccountSettingPageViewModel(DialogService dialogService, SettingService settingService, NotificationService notificationService, TaskService taskService) {
        _dialogService = dialogService;
        _settingService = settingService;
        _notificationService = notificationService;

        if (_settingService.Data.Accounts.Count != 0) {
            taskService.QueueJob(new AccountLoadTask(_settingService.Data.Accounts));
        }

        WeakReferenceMessenger.Default.Register<AccountMessage>(this, AccountHandle);
        WeakReferenceMessenger.Default.Register<AccountViewMessage>(this, AccountViewHandle);
        WeakReferenceMessenger.Default.Register<AccountChangeNotificationMessage>(this, AccountChangeHandle);
    }

    [RelayCommand]
    private void OpenDialog() {
        _dialogService.ShowContentDialog<ChooseAccountTypeDialogViewModel>();
    }

    private async void AccountHandle(object obj, AccountMessage accountMessage) {
        await Task.Run(async () => {
            foreach (var item in accountMessage.Accounts.Select(x => new AccountViewData(x))) {
                Accounts.Add(item);
                await Task.Delay(5);
            }
        });
    }

    private async void AccountViewHandle(object obj, AccountViewMessage accountMessage) {
        foreach (var item in accountMessage.Accounts) {
            Accounts.Add(item);
            await Task.Delay(5);
        }
    }

    private void AccountChangeHandle(object obj, AccountChangeNotificationMessage accountMessage) {
        if (Accounts.Remove(accountMessage.Account)) {
            _notificationService.QueueJob(new NotificationViewData {
                Title = "成功",
                Content = $"已成功将账户 {accountMessage.Account.Account.Name} 移除！",
                NotificationType = NotificationType.Success
            });
        }
    }
}