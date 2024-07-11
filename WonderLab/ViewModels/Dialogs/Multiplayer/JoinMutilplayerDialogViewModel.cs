using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace WonderLab.ViewModels.Dialogs.Multiplayer;

public sealed partial class JoinMutilplayerDialogViewModel : DialogViewModelBase {
    private readonly WrapService _wrapService;
    private readonly DialogService _dialogService;

    public JoinMutilplayerDialogViewModel(DialogService dialogService, WrapService wrapService) {
        _wrapService = wrapService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async void MakeRequest(string token) {
        await Task.Run(() => {
            _wrapService.MakeRequest(token);
        });

        _dialogService.CloseContentDialog();
    }
}