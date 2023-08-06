using MinecraftProtocol.Client;
using MinecraftProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol
{
    public class ServerPing
    {
        public static string PingServer(string ip, ushort port)
        {
            ServerConnection server = new(ip, port);
            server.SendPacket(new HandShakePacket(761, ip, port));
            server.SendPacket(new PingPacket());
            server.IsDisconnected = true;
            return ((StatusPacket)server.ReceivePacket()).Status;
        }
    }
}
