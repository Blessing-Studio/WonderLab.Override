using System;
using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using BlessingStudio.Wrap.Client;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace WonderLab.ViewModels.Dialogs.Multiplayer;

public sealed partial class JoinMutilplayerRequestDialogViewModel : DialogViewModelBase {
    private readonly WrapService _wrapService;
    private readonly DialogService _dialogService;
    private readonly ILogger<JoinMutilplayerRequestDialogViewModel> _logger;

    [ObservableProperty] private bool _isAllow;
    [ObservableProperty] private string _processInfoText;

    public JoinMutilplayerRequestDialogViewModel(DialogService dialogService, WrapService wrapService, ILogger<JoinMutilplayerRequestDialogViewModel> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _dialogService = dialogService;

        ProcessInfoText = "连接中";
        wrapService.ConnectFailed += (s, e) => ProcessInfoText = $"与 {e.UserToken} 连接失败";
        wrapService.ReconnectPeer += (s, e) => ProcessInfoText = $"开始与 {e.UserToken} 反向打洞";
        wrapService.RequestInvalidated += (s, e) => ProcessInfoText = $"给 {e.Requester} 发出的请求已失效";
        wrapService.ConnectPeerSuccessfully += (s, e) => ProcessInfoText = $"与 {e.UserToken} 连接成功 映射端口为 {e.Port}";
    }

    [RelayCommand]
    private void Allow() {
        IsAllow = true;

        RunBackgroundWork(() => {
            var info = Parameter as RequestInfo;
            try {
                _wrapService.AcceptRequest(info);
            } catch (Exception ex) {
                _logger.LogError(ex, "在允许 {Token} 加入的房间时出现了异常", info.Requester);
            }
        }, () => _dialogService.CloseContentDialog());
    }
}