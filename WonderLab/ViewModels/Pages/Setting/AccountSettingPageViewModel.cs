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

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountSettingPageViewModel : ViewModelBase {
    private readonly DialogService _dialogService;
    private readonly SettingService _settingService;

    [ObservableProperty] private object _activeAccount;
    [ObservableProperty] private ObservableCollection<AccountViewData> _accounts;

    public AccountSettingPageViewModel(DialogService dialogService, SettingService settingService) {
        _dialogService = dialogService;
        _settingService = settingService;

        Accounts = _settingService.Data.Accounts
            .Select(x => new AccountViewData(x))
            .ToObservableList();

        WeakReferenceMessenger.Default.Register<AccountMessage>(this, Handle);
    }

    [RelayCommand]
    private void OpenDialog() {
        _dialogService.ShowContentDialog<ChooseAccountTypeDialogViewModel>();
    }

    private async void Handle(object obj, AccountMessage accountMessage) {
        await Task.Run(() => {
            foreach (var item in accountMessage.Accounts.Select(x => new AccountViewData(x))) {
                Accounts.Add(item);
            }
        });
    }
}