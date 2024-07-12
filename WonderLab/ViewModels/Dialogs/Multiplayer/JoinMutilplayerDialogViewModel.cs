using System;
using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace WonderLab.ViewModels.Dialogs.Multiplayer;

public sealed partial class JoinMutilplayerDialogViewModel : DialogViewModelBase {
    private readonly WrapService _wrapService;
    private readonly DialogService _dialogService;
    private readonly ILogger<JoinMutilplayerDialogViewModel> _logger;

    [ObservableProperty] private bool _isMakedRequest;
    [ObservableProperty] private string _processInfoText;

    public JoinMutilplayerDialogViewModel(DialogService dialogService, WrapService wrapService, ILogger<JoinMutilplayerDialogViewModel> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _dialogService = dialogService;

        ProcessInfoText = "等待创建连接方批准请求";
        wrapService.ReconnectPeer += (s, e) => ProcessInfoText = $"开始与 {e.UserToken} 反向打洞";
        wrapService.RequestInvalidated += (s, e) => ProcessInfoText = $"给 {e.Requester} 发出的请求已失效";

        wrapService.ConnectFailed += (s, e) => {
            ProcessInfoText = $"与 {e.UserToken} 连接失败";
            _dialogService.CloseContentDialog();
        };
        wrapService.ConnectPeerSuccessfully += (s, e) => {
            ProcessInfoText = $"与 {e.UserToken} 连接成功 映射端口为 {e.Port}";
            _dialogService.CloseContentDialog();
        };
    }

    [RelayCommand]
    private void MakeRequest(string token) {
        IsMakedRequest = true;

        RunBackgroundWork(() => {
            try {
                _wrapService.MakeRequest(token);
            } catch (Exception ex) {
                _logger.LogError(ex, "在请求加入 {Token} 的房间时出现了异常", token);
            }
        });
    }
}