using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Classes.Enums;
using MinecraftLaunch.Classes.Models.Auth;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WonderLab.ViewModels.Dialogs.Setting;

public sealed partial class RefreshAccountDialogViewModel : DialogViewModelBase {
    private readonly DialogService _dialogService;

    [ObservableProperty] private Account _account;

    public RefreshAccountDialogViewModel(DialogService dialogService) { 
        _dialogService = dialogService;
        Account = Parameter as Account;
    }

    [RelayCommand]
    private void Continue() {
        _dialogService.CloseContentDialog();

        switch (Account.Type) {
            case AccountType.Microsoft:
                _dialogService.ShowContentDialog<MicrosoftAuthenticateDialogViewModel>();
                break;
            case AccountType.Yggdrasil:
                _dialogService.ShowContentDialog<YggdrasilAuthenticateDialogViewModel>(Account);
                break;
        }
    }
}