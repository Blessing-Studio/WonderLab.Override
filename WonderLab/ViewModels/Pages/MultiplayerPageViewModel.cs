using Avalonia.Controls.Notifications;
using BlessingStudio.Wrap;
using BlessingStudio.Wrap.Client.Events;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using STUN.Enums;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Waher.Networking.UPnP;
using WonderLab.Classes.Datas.ViewData;
using WonderLab.Services;
using WonderLab.Services.UI;
using WonderLab.Services.Wrap;
using WonderLab.ViewModels.Dialogs.Multiplayer;
using UPnPService = WonderLab.Services.Wrap.UPnPService;

namespace WonderLab.ViewModels.Pages;

public sealed partial class MultiplayerPageViewModel : ViewModelBase {
    private const string UPNP_WANIP_SRVICE = "urn:schemas-upnp-org:service:WANIPConnection:1";
    private const string UPNP_WANPPP_SERVICE = "urn:schemas-upnp-org:service:WANPPPConnection:1";

    private UPnP _uPnP;


    private readonly TimeSpan _timeOutSpan;

    private readonly WrapService _wrapService;
    private readonly UPnPService _upnPService;
    private readonly DialogService _dialogService;
    private readonly WindowService _windowService;
    private readonly NotificationService _notificationService;
    private readonly ILogger<MultiplayerPageViewModel> _logger;

    [ObservableProperty] private NatType _natType;
    [ObservableProperty] private bool _isSupportUPnP;
    [ObservableProperty] private bool _isUPnPLoading;
    [ObservableProperty] private bool _isNatTypeLoading;
    [ObservableProperty] private string _userToken;
    [ObservableProperty] private string _minecraftPort;

    public MultiplayerPageViewModel(
        WrapService wrapService,
        UPnPService uPnPService,
        DialogService dialogService,
        WindowService windowService,
        NotificationService notificationService,
        ILogger<MultiplayerPageViewModel> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _upnPService = uPnPService;
        _windowService = windowService;
        _dialogService = dialogService;
        _notificationService = notificationService;

        IsUPnPLoading = true;
        IsNatTypeLoading = true;

        _timeOutSpan = TimeSpan.FromSeconds(10);

        MinecraftPort = "25565";
        _ = InitializeAsync();
    }

    [RelayCommand]
    private void Create() {
        _dialogService.ShowContentDialog<CreateMutilplayerDialogViewModel>();
    }

    [RelayCommand]
    private void Join() {
        _dialogService.ShowContentDialog<JoinMutilplayerDialogViewModel>();
    }

    [RelayCommand]
    private void CopyToken() {
        _windowService.CopyText(UserToken);
    }

    private async ValueTask InitializeAsync() {
        _wrapService.NewRequest += OnNewRequest;
        _wrapService.LoginedSuccessfully += OnLoginedSuccessfully;
        _wrapService.ConnectPeerSuccessfully += OnConnectPeerSuccessfully;

        await Task.Run(async () => {
            try {
                _upnPService.Init();
                _upnPService.Search();
                _logger.LogInformation("开始查找 UPnP 设备");
                var now = DateTime.Now;
                while ((DateTime.Now - now) < _timeOutSpan && _upnPService.UPnPDeviceLocations.Count is 0) {
                    await Task.Delay(5);
                }

                IsUPnPLoading = false;
                foreach (UPnPDevice uPnPDevice in _upnPService.UPnPDeviceLocations) {
                    if (uPnPDevice != null) {
                        Waher.Networking.UPnP.UPnPService natService = uPnPDevice.GetService(UPNP_WANIP_SRVICE);
                        natService ??= uPnPDevice.GetService(UPNP_WANPPP_SERVICE);

                        if (natService != null) {
                            _uPnP = new UPnP(natService);
                            _logger.LogInformation("UPnP已支持");
                            break;
                        }
                    }
                }

                _logger.LogInformation("开始查找 NAT 类型，是否支持 uPnP：{IsSupportUPnP}", _uPnP is not null);

                IsSupportUPnP = _uPnP is not null;

                if (IsSupportUPnP)
                    NatType = await _upnPService.GetNatTypeAsync(_uPnP);
                else
                    NatType = await _wrapService.GetNatTypeAsync();

                IsNatTypeLoading = false;

                _logger.LogInformation("NAT 类型为：{Type}", NatType);
            } catch (Exception ex) {
                _logger.LogError(ex, "查找 uPnP 设备或 NAT 类型时出现了错误");
                NatType = NatType.Unknown;
                IsNatTypeLoading = false;
                IsSupportUPnP = false;
            } finally {
                _wrapService.Init();
                _wrapService.Connect(_wrapService.ServerIP);
                _wrapService.Start();
            }
        });
    }

    private void OnConnectPeerSuccessfully(object sender, ConnectPeerSuccessfullyEvent e) {
        _notificationService.QueueJob(new NotificationViewData {
            Title = "成功",
            Content = $"与 {e.UserToken} 连接成功 映射端口为 {e.Port}",
            NotificationType = NotificationType.Success
        });
    }

    private void OnNewRequest(object sender, NewRequestEvent e) {
        _dialogService.ShowContentDialog<JoinMutilplayerRequestDialogViewModel>(e.RequestInfo);
    }

    private void OnLoginedSuccessfully(object sender, LoginedSuccessfullyEvent e) {
        _notificationService.QueueJob(new NotificationViewData {
            Title = "成功",
            Content = $"已成功与打洞服务器建立连接，您的用户令牌为：{e.UserToken}",
            NotificationType = NotificationType.Success
        });
        UserToken = e.UserToken;
    }
}