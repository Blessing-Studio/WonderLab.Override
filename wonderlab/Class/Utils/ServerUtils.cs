using HarfBuzzSharp;
using MinecraftProtocol.Client;
using MinecraftProtocol.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public class ServerUtils {
        public string Address { get; init; }
        public ushort Port { get; init; }
        public int VersionId { get; init; }

        public ServerUtils(string Address, ushort Port, int VersionId = 0) {
            this.Address = Address;
            this.Port = Port;
            this.VersionId = VersionId;
        }

        /// <summary>
        /// 服务器信息获取方法
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        public async Task<ServerInfoModel> GetServerInfoAsync() {
            using var client = new TcpClient {
                SendTimeout = 5000,
                ReceiveTimeout = 5000
            };
            var sw = new Stopwatch();
            var timeOut = TimeSpan.FromSeconds(3);
            using var cts = new CancellationTokenSource(timeOut);

            sw.Start();
            cts.CancelAfter(timeOut);

            try {
                await client.ConnectAsync(Address, Port, cts.Token);
            }
            catch (TaskCanceledException) {
                throw new OperationCanceledException($"服务器 {this} 连接失败，连接超时 ({timeOut.Seconds}s)。", cts.Token);
            }

            sw.Stop();
            ServerConnection serverConnection = new(client);
            serverConnection.SendPacket(new HandShakePacket(761, Address, Port));
            serverConnection.SendPacket(new PingPacket());
            StatusPacket statusPacket = (StatusPacket)serverConnection.ReceivePacket();
            var ping = JsonConvert.DeserializeObject<PingPayload>(statusPacket.Status);

            return new ServerInfoModel {
                Latency = sw.ElapsedMilliseconds,
                Response = ping
            };
        }
    }
}
