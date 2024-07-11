using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using CommunityToolkit.Mvvm.Input;
using BlessingStudio.Wrap.Client;
using System.Threading.Tasks;

namespace WonderLab.ViewModels.Dialogs.Multiplayer;

public sealed partial class JoinMutilplayerRequestDialogViewModel : DialogViewModelBase {
    private readonly WrapService _wrapService;
    private readonly DialogService _dialogService;

    public JoinMutilplayerRequestDialogViewModel(DialogService dialogService, WrapService wrapService) {
        _wrapService = wrapService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async void Allow() {
        await Task.Run(() => {
            _wrapService.AcceptRequest(Parameter as RequestInfo);
        });

        _dialogService.CloseContentDialog();
    }
}