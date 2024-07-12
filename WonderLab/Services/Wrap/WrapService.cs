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

public sealed class WrapService : IWrapClient {
    TcpClient IWrapClient.Client => Client.Client;

    public const string SERVER_HOST = "47.113.149.130";

    public IPAddress ServerIP { get; set; }
    public IWrapClient Client { get; private set; }

    public string UserToken => Client.UserToken;
    public string DisconnectReason => Client.DisconnectReason;

    public bool IsDisposed => Client.IsDisposed;
    public bool IsConnected => Client.IsConnected;

    public IPEndPoint LocalIP => Client.LocalIP;
    public IPEndPoint RemoteIP => Client.RemoteIP;

    public Channel MainChannel => Client.MainChannel;
    public List<RequestInfo> Requests => Client.Requests;
    public IPeerManager PeerManager => Client.PeerManager;
    public IUPnPService UPnPService => Client.UPnPService;
    public Connection ServerConnection => Client.ServerConnection;

    public event EventHandler<NewRequestEvent> NewRequest;
    public event EventHandler<ReconnectPeerEvent> ReconnectPeer;
    public event EventHandler<ConnectPeerFailedEvent> ConnectFailed;
    public event EventHandler<ExpectedDisconnectEvent> ExpectedDisconnect;
    public event EventHandler<RequestInvalidatedEvent> RequestInvalidated;
    public event EventHandler<LoginedSuccessfullyEvent> LoginedSuccessfully;
    public event EventHandler<UnexpectedDisconnectEvent> UnexpectedDisconnect;
    public event EventHandler<ConnectPeerSuccessfullyEvent> ConnectPeerSuccessfully;

    event BlessingStudio.WonderNetwork.Events.EventHandler<ConnectPeerFailedEvent> IWrapClient.ConnectFailed {
        add => Client.ConnectFailed += value;
        remove => Client.ConnectFailed -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<ConnectPeerSuccessfullyEvent> IWrapClient.ConnectPeerSuccessfully {
        add => Client.ConnectPeerSuccessfully += value;
        remove => Client.ConnectPeerSuccessfully -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<ExpectedDisconnectEvent> IWrapClient.ExpectedDisconnect {
        add => Client.ExpectedDisconnect += value;
        remove => Client.ExpectedDisconnect -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<LoginedSuccessfullyEvent> IWrapClient.LoginedSuccessfully {
        add => Client.LoginedSuccessfully += value;
        remove => Client.LoginedSuccessfully -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<NewRequestEvent> IWrapClient.NewRequest {
        add => Client.NewRequest += value;
        remove => Client.NewRequest -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<ReconnectPeerEvent> IWrapClient.ReconnectPeer {
        add => Client.ReconnectPeer += value;
        remove => Client.ReconnectPeer -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<RequestInvalidatedEvent> IWrapClient.RequestInvalidated {
        add => Client.RequestInvalidated += value;
        remove => Client.RequestInvalidated -= value;
    }

    event BlessingStudio.WonderNetwork.Events.EventHandler<UnexpectedDisconnectEvent> IWrapClient.UnexpectedDisconnect {
        add => Client.UnexpectedDisconnect += value;
        remove => Client.UnexpectedDisconnect -= value;
    }

    /// <summary>
    /// 初始化 WrapService，包括创建新的 WrapClient 实例和注册事件处理器。
    /// </summary>
    public void Init() {
        // 创建新的WrapClient实例
        Client = new WrapClient();
        // 获取服务器IP地址
        ServerIP = Dns.GetHostAddresses(SERVER_HOST).First();

        // 注册各种事件处理器
        Client.NewRequest += e => {
            NewRequest?.Invoke(this, e);
        };

        Client.ExpectedDisconnect += e => {
            ExpectedDisconnect?.Invoke(this, e);
        };

        Client.UnexpectedDisconnect += e => {
            UnexpectedDisconnect?.Invoke(this, e);
        };

        Client.ConnectPeerSuccessfully += e => {
            ConnectPeerSuccessfully?.Invoke(this, e);
        };

        Client.RequestInvalidated += e => {
            RequestInvalidated?.Invoke(this, e);
        };

        Client.ReconnectPeer += e => {
            ReconnectPeer?.Invoke(this, e);
        };

        Client.ConnectFailed += e => {
            ConnectFailed?.Invoke(this, e);
        };

        Client.LoginedSuccessfully += e => {
            LoginedSuccessfully?.Invoke(this, e);
        };
    }

    /// <summary>
    /// 启动 WrapClient。
    /// </summary>
    public void Start() {
        Client.Start();
    }

    /// <summary>
    /// 获取 NAT 类型。
    /// </summary>
    /// <returns>NAT 类型。</returns>
    public NatType GetNatType() {
        return StunUtils.GetNatType();
    }

    /// <summary>
    /// 异步获取 NAT 类型。
    /// </summary>
    /// <returns>NAT 类型。</returns>
    public async Task<NatType> GetNatTypeAsync() {
        return await StunUtils.GetNatTypeAsync();
    }

    /// <summary>
    /// 接受请求。
    /// </summary>
    /// <param name="request">要接受的请求。</param>
    public void AcceptRequest(RequestInfo request) {
        Client.AcceptRequest(request);
    }

    /// <summary>
    /// 关闭 WrapClient。
    /// </summary>
    public void Close() {
        Client.Close();
    }

    /// <summary>
    /// 连接到指定的 IP 地址和端口。
    /// </summary>
    /// <param name="address">要连接的 IP 地址。</param>
    /// <param name="port">要连接的端口。</param>
    public void Connect(IPAddress address, int port = 38297) {
        Client.Connect(address, port);
    }

    /// <summary>
    /// 释放 WrapClient 的资源。
    /// </summary>
    public void Dispose() {
        Client.Dispose();
    }

    /// <summary>
    /// 发起请求。
    /// </summary>
    /// <param name="userToken">用户令牌。</param>
    public void MakeRequest(string userToken) {
        Client.MakeRequest(userToken);
    }

    /// <summary>
    /// 使用指定的用户令牌启动 WrapClient。
    /// </summary>
    /// <param name="userToken">用户令牌。</param>
    public void Start(string userToken = "_") {
        Client.Start(userToken);
    }
}