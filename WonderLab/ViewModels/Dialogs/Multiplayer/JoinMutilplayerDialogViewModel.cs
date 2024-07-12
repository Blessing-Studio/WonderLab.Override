using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using System;

namespace WonderLab.ViewModels.Dialogs.Multiplayer;

public sealed partial class JoinMutilplayerDialogViewModel : DialogViewModelBase {
    private readonly WrapService _wrapService;
    private readonly DialogService _dialogService;
    private readonly ILogger<JoinMutilplayerDialogViewModel> _logger;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(MakeRequestCommand))]
    private bool _isClick;

    public JoinMutilplayerDialogViewModel(DialogService dialogService, WrapService wrapService, ILogger<JoinMutilplayerDialogViewModel> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _dialogService = dialogService;
    }

    [RelayCommand(CanExecute = nameof(CanClick))]
    private void MakeRequest(string token) {
        IsClick = true;

        RunBackgroundWork(() => {
            try {
                _wrapService.MakeRequest(token);
            } catch (Exception ex) {
                _logger.LogError(ex, "在请求加入 {Token} 的房间时出现了异常", token);
            }
        }, () => {
            _dialogService.CloseContentDialog();
        });
    }

    private bool CanClick() => !IsClick;
}