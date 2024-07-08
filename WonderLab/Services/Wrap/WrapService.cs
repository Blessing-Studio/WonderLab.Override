using BlessingStudio.WonderNetwork;
using BlessingStudio.Wrap.Client;
using BlessingStudio.Wrap.Client.Events;
using BlessingStudio.Wrap.Interfaces;
using BlessingStudio.Wrap.Utils;
using STUN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WonderLab.Services.Wrap;

public sealed class WrapService : IWrapClient
{
    public const string ServerHost = "wrap.blessing-studio.tech";
    public IPAddress ServerIP { get; set; }
    public IWrapClient Client { get; private set; }

    TcpClient IWrapClient.Client => Client.Client;

    public string DisconnectReason => Client.DisconnectReason;

    public bool IsConnected => Client.IsConnected;

    public bool IsDisposed => Client.IsDisposed;

    public IPEndPoint LocalIP => Client.LocalIP;

    public Channel MainChannel => Client.MainChannel;

    public IPeerManager PeerManager => Client.PeerManager;

    public IPEndPoint RemoteIP => Client.RemoteIP;

    public List<RequestInfo> Requests => Client.Requests;

    public Connection ServerConnection => Client.ServerConnection;

    public IUPnPService UPnPService => Client.UPnPService;

    public string UserToken => Client.UserToken;

    public event EventHandler<ConnectPeerFailedEvent> ConnectFailed;
    public event EventHandler<ConnectPeerSuccessfullyEvent> ConnectPeerSuccessfully;
    public event EventHandler<ExpectedDisconnectEvent> ExpectedDisconnect;
    public event EventHandler<LoginedSuccessfullyEvent> LoginedSuccessfully;
    public event EventHandler<NewRequestEvent> NewRequest;
    public event EventHandler<ReconnectPeerEvent> ReconnectPeer;
    public event EventHandler<RequestInvalidatedEvent> RequestInvalidated;
    public event EventHandler<UnexpectedDisconnectEvent> UnexpectedDisconnect;

    event BlessingStudio.WonderNetwork.Events.EventHandler<ConnectPeerFailedEvent> IWrapClient.ConnectFailed
    {
        add
        {
            Client.ConnectFailed += value;
        }

        remove
        {
            Client.ConnectFailed -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<ConnectPeerSuccessfullyEvent> IWrapClient.ConnectPeerSuccessfully
    {
        add
        {
            Client.ConnectPeerSuccessfully += value;
        }

        remove
        {
            Client.ConnectPeerSuccessfully -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<ExpectedDisconnectEvent> IWrapClient.ExpectedDisconnect
    {
        add
        {
            Client.ExpectedDisconnect += value;
        }

        remove
        {
            Client.ExpectedDisconnect -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<LoginedSuccessfullyEvent> IWrapClient.LoginedSuccessfully
    {
        add
        {
            Client.LoginedSuccessfully += value;
        }

        remove
        {
            Client.LoginedSuccessfully -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<NewRequestEvent> IWrapClient.NewRequest
    {
        add
        {
            Client.NewRequest += value;
        }

        remove
        {
            Client.NewRequest -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<ReconnectPeerEvent> IWrapClient.ReconnectPeer
    {
        add
        {
            Client.ReconnectPeer += value;
        }

        remove
        {
            Client.ReconnectPeer -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<RequestInvalidatedEvent> IWrapClient.RequestInvalidated
    {
        add
        {
            Client.RequestInvalidated += value;
        }

        remove
        {
            Client.RequestInvalidated -= value;
        }
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<UnexpectedDisconnectEvent> IWrapClient.UnexpectedDisconnect
    {
        add
        {
            Client.UnexpectedDisconnect += value;
        }

        remove
        {
            Client.UnexpectedDisconnect -= value;
        }
    }

    public void Init()
    {
        Client = new WrapClient();
        ServerIP = Dns.GetHostAddresses("wrap.blessing-studio.tech").First();

        Client.ExpectedDisconnect += e =>
        {
            ExpectedDisconnect(this, e);
        };

        Client.UnexpectedDisconnect += e =>
        {
            UnexpectedDisconnect(this, e);
        };

        Client.ConnectPeerSuccessfully += e =>
        {
            ConnectPeerSuccessfully(this, e);
        };

        Client.RequestInvalidated += e =>
        {
            RequestInvalidated(this, e);
        };

        Client.ReconnectPeer += e =>
        {
            ReconnectPeer(this, e);
        };

        Client.ConnectFailed += e =>
        {
            ConnectFailed(this, e);
        };

        Client.LoginedSuccessfully += e =>
        {
            LoginedSuccessfully(this, e);
        };
    }

    public void Start()
    {
        Client.Start();
    }

    public NatType GetNatType()
    {
        return StunUtils.GetNatType();
    }

    public async Task<NatType> GetNatTypeAsync()
    {
        return await StunUtils.GetNatTypeAsync();
    }

    public void AcceptRequest(RequestInfo request)
    {
        Client.AcceptRequest(request);
    }

    public void Close()
    {
        Client.Close();
    }

    public void Connect(IPAddress address, int port = 38297)
    {
        Client.Connect(address, port);
    }

    public void Dispose()
    {
        Client.Dispose();
    }

    public void MakeRequest(string userToken)
    {
        Client.MakeRequest(userToken);
    }

    public void Start(string userToken = "_")
    {
        Client.Start(userToken);
    }
}
