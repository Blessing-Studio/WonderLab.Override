using System;
using System.Threading;
using System.Net.Sockets;
using BlessingStudio.Wrap;
using Waher.Networking.UPnP;
using System.Threading.Tasks;
using WonderLab.Services.Wrap;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

using UPnPService = WonderLab.Services.Wrap.UPnPService;
using Waher.Script.Constants;

namespace WonderLab.ViewModels.Pages;

public sealed partial class MultiplayerPageViewModel : ViewModelBase {
    private const string UPNP_WANIP_SRVICE = "urn:schemas-upnp-org:service:WANIPConnection:1";
    private const string UPNP_WANPPP_SERVICE = "urn:schemas-upnp-org:service:WANPPPConnection:1";

    private UPnP _uPnP;

    private readonly TimeSpan _timeOutSpan;

    private readonly WrapService _wrapService;
    private readonly UPnPService _upnPService;
    private readonly ILogger<MultiplayerPageViewModel> _logger;

    [ObservableProperty] private bool _isuPnPLoading;

    public MultiplayerPageViewModel(
        WrapService wrapService, 
        UPnPService uPnPService,
        ILogger<MultiplayerPageViewModel> logger) {
        _logger = logger;
        _wrapService = wrapService;
        _upnPService = uPnPService;

        IsuPnPLoading = true;

        _timeOutSpan = TimeSpan.FromSeconds(30);
        _ = InitializeAsync();
    }

    private async ValueTask InitializeAsync() {
        await Task.Run(async () => {
            try {
                _upnPService.Init();
                _upnPService.Search();
                _logger.LogInformation("开始查找 UPnP 设备");
                var now = DateTime.Now;
                while ((DateTime.Now - now) < _timeOutSpan && _upnPService.UPnPDeviceLocations.IsEmpty) {
                    await Task.Delay(5);
                }

                IsuPnPLoading = false;
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
                var natType = await _upnPService.GetNatTypeAsync(_uPnP);
                _logger.LogInformation("NAT 类型为：{Type}", natType);
            } catch (Exception ex) {
                _logger.LogError(ex, "查找 uPnP 设备或 NAT 类型时出现了错误");
            }
        });
    }
}