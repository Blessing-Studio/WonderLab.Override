using System.Linq;
using WonderLab.Services;
using WonderLab.Services.UI;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using WonderLab.Classes.Datas.TaskData;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Classes.Datas.MessageData;
using WonderLab.ViewModels.Dialogs.Setting;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountSettingPageViewModel : ViewModelBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;
    private readonly NotificationService _notificationService;

    [ObservableProperty] private AccountViewData _activeAccount;
    [ObservableProperty] private ObservableCollection<AccountViewData> _accounts = [];

    public AccountSettingPageViewModel(
        DialogService dialogService,
        SettingService settingService,
        NotificationService notificationService,
        TaskService taskService) {
        _dialogService = dialogService;
        _settingService = settingService;
        _notificationService = notificationService;

        if (_settingService.Data.Accounts.Count != 0) {
            RunBackgroundWork(() => taskService.QueueJob(new AccountLoadTask(_settingService.Data.Accounts)));
        }

        WeakReferenceMessenger.Default.Register<AccountMessage>(this, AccountHandle);
        WeakReferenceMessenger.Default.Register<AccountViewMessage>(this, AccountViewHandle);
        WeakReferenceMessenger.Default.Register<AccountChangeNotificationMessage>(this, AccountChangeHandle);
    }

    [RelayCommand]
    private void OpenDialog() {
        _dialogService.ShowContentDialog<ChooseAccountTypeDialogViewModel>();
    }

    partial void OnActiveAccountChanged(AccountViewData value) {
        _settingService.Data.ActiveAccount = value?.Account;
    }

    private void AccountHandle(object obj, AccountMessage accountMessage) {
        RunBackgroundWork(async () => {
            foreach (var item in accountMessage.Accounts.Select(x => new AccountViewData(x))) {
                Accounts.Add(item);
                await Task.Delay(5);
            }
        });
    }

    private void AccountViewHandle(object obj, AccountViewMessage accountMessage) {
        RunBackgroundWork(async () => {
            foreach (var item in accountMessage.Accounts) {
                Accounts.Add(item);
                await Task.Delay(5);
            }
        });
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