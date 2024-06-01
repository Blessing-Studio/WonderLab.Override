using CommunityToolkit.Mvvm.Input;
using WonderLab.Services.UI;

namespace WonderLab.ViewModels.Dialogs.Setting;

public sealed partial class ChooseAccountTypeDialogViewModel : ViewModelBase {
    private readonly DialogService _dialogService;

    public ChooseAccountTypeDialogViewModel(DialogService dialogService) {
        _dialogService = dialogService;
    }

    [RelayCommand]
    private void ChooseAccountType(string type) {
        _dialogService.CloseContentDialog();

        switch (type) {
            case "Offline":
                _dialogService.ShowContentDialog<OfflineAuthenticateDialogViewModel>();
                break;
        }
    }
}