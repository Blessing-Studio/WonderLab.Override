using STUN.Enums;
using System.Collections.Generic;
using System.Net.Sockets;
using Waher.Networking.UPnP;
using BlessingStudio.Wrap.Interfaces;
using System.Net;
using BlessingStudio.Wrap.Utils;
using STUN.StunResult;
using static BlessingStudio.Wrap.Interfaces.IUPnPService;
using System.Threading.Tasks;
using BlessingStudio.Wrap;
using System.Diagnostics.CodeAnalysis;

namespace WonderLab.Services.Wrap;

public sealed class UPnPService
{
    public UPnPClient Client { get; private set; }
    public IList<UPnPDevice> UPnPDeviceLocations { get; private set; }
    public event UPnPDeviceLocationEventHandler OnDeviceFound;
    public void Init()
    {
        Client = new();
        UPnPDeviceLocations = [];
        Client.OnDeviceFound += Client_OnDeviceFound;
    }
    public void StartSearch()
    {
        Client.StartSearch();
    }
    public NatType GetNatType(IUPnPService upnp)
    {
        IPEndPoint endPoint = new(new IPAddress(new byte[] { 0, 0, 0, 0 }), RandomUtils.GetRandomPort());
        upnp.AddPortMapping(endPoint.Port, SocketProtocol.UDP, endPoint.Port, "Wrap NAT test");
        ClassicStunResult result = StunUtils.GetClassicStunResultAsync(endPoint).GetAwaiter().GetResult();
        upnp.DeletePortMapping(endPoint.Port, SocketProtocol.UDP);
        return result.NatType;
    }
    public async Task<NatType> GetNatTypeAsync(IUPnPService upnp)
    {
        IPEndPoint endPoint = new(new IPAddress(new byte[] { 0, 0, 0, 0 }), RandomUtils.GetRandomPort());
        upnp.AddPortMapping(endPoint.Port, SocketProtocol.UDP, endPoint.Port, "Wrap NAT test");
        ClassicStunResult result = await StunUtils.GetClassicStunResultAsync(endPoint);
        upnp.DeletePortMapping(endPoint.Port, SocketProtocol.UDP);
        return result.NatType;
    }

    public IUPnPService GetUPnPService()
    {
        foreach (UPnPDevice UPnPDeviceLocation in UPnPDeviceLocations)
        {
            if (UPnPDeviceLocation != null)
            {
                Waher.Networking.UPnP.UPnPService natService = UPnPDeviceLocation.GetService("urn:schemas-upnp-org:service:WANIPConnection:1"); // 获取WAN IP连接服务
                natService ??= UPnPDeviceLocation.GetService("urn:schemas-upnp-org:service:WANPPPConnection:1");
                if (natService != null)
                {
                    return new UPnP(natService);
                }
            }
        }

        return null;
    }

    private void Client_OnDeviceFound(object Sender, DeviceLocationEventArgs e)
    {
        if (e.Location.GetDevice().Device.DeviceType != "urn:schemas-upnp-org:device:InternetGatewayDevice:1") return;
        if (e.RemoteEndPoint.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) return;
        OnDeviceFound(Sender, e);
        UPnPDeviceLocations.Add(e.Location.GetDevice().Device);
    }
}
