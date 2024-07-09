using System.Net;
using STUN.Enums;
using System.Linq;
using STUN.StunResult;
using BlessingStudio.Wrap;
using Waher.Networking.UPnP;
using System.Threading.Tasks;
using BlessingStudio.Wrap.Utils;
using System.Collections.Concurrent;
using BlessingStudio.Wrap.Interfaces;

using static BlessingStudio.Wrap.Interfaces.IUPnPService;

namespace WonderLab.Services.Wrap;

public sealed class UPnPService {
    public UPnPClient Client { get; private set; }
    public ConcurrentBag<UPnPDevice> UPnPDeviceLocations { get; private set; }

    public event UPnPDeviceLocationEventHandler DeviceFounded;

    public void Init() {
        Client = [];
        UPnPDeviceLocations = [];
        Client.OnDeviceFound += OnDeviceFound;
    }

    public void Search() {
        Client.StartSearch();
    }

    public async Task<NatType> GetNatTypeAsync(IUPnPService upnp) {
        IPEndPoint endPoint = new(new IPAddress(new byte[] { 0, 0, 0, 0 }), RandomUtils.GetRandomPort());
        upnp.AddPortMapping(endPoint.Port, SocketProtocol.UDP, endPoint.Port, "Wrap NAT test");
        ClassicStunResult result = await StunUtils.GetClassicStunResultAsync(endPoint);
        upnp.DeletePortMapping(endPoint.Port, SocketProtocol.UDP);
        return result.NatType;
    }

    public IUPnPService GetUPnPService() {
        var uPnPs = UPnPDeviceLocations.Select(UPnPDeviceLocation => {
            if (UPnPDeviceLocation != null) {
                Waher.Networking.UPnP.UPnPService natService = UPnPDeviceLocation.GetService("urn:schemas-upnp-org:service:WANIPConnection:1"); // 获取WAN IP连接服务
                natService ??= UPnPDeviceLocation.GetService("urn:schemas-upnp-org:service:WANPPPConnection:1");
                if (natService != null) {
                    return new UPnP(natService);
                }
            }

            return null;
        });

        return uPnPs.FirstOrDefault(x => x != null);
    }

    private async void OnDeviceFound(object Sender, DeviceLocationEventArgs e) {
        if (e.Location.GetDevice().Device.DeviceType != "urn:schemas-upnp-org:device:InternetGatewayDevice:1")
            return;

        if (e.RemoteEndPoint.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
            return;

        DeviceFounded?.Invoke(Sender, e);
        UPnPDeviceLocations.Add((await e.Location.GetDeviceAsync()).Device);
    }
}
