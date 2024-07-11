using WonderLab.Services.UI;
using CommunityToolkit.Mvvm.Input;

namespace WonderLab.ViewModels.Dialogs.Setting;

public sealed partial class ChooseAccountTypeDialogViewModel : DialogViewModelBase {
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
            case "Yggdrasil":
                _dialogService.ShowContentDialog<YggdrasilAuthenticateDialogViewModel>();
                break;
            case "Microsoft":
                _dialogService.ShowContentDialog<MicrosoftAuthenticateDialogViewModel>();
                break;
        }
    }
}