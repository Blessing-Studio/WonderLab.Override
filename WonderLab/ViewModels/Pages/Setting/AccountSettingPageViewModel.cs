using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WonderLab.ViewModels.Dialogs.Setting;

namespace WonderLab.ViewModels.Pages.Setting;

public sealed partial class AccountSettingPageViewModel : ViewModelBase {
    private readonly DialogService _dialogService;

    [ObservableProperty] private object _activeAccount;

    public AccountSettingPageViewModel(DialogService dialogService) {
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void OpenDialog() {
        _dialogService.ShowContentDialog<AuthenticateDialogViewModel>();
    }
}