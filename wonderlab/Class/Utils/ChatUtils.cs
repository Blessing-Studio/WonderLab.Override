using MinecraftProtocol;
using MinecraftProtocol.Client;
using MinecraftProtocol.Client.v1_18_R2;
using MinecraftProtocol.Server;
using MinecraftProtocol.Server.v1_18_R2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public class Chater
    {
        public readonly ServerConnection connection;
        public Chater(string ip, ushort port = 25565)
        {
            connection = new ServerConnection(ip, port);
        }
        public Chater(ServerConnection connection)
        {
            this.connection = connection;
        }
        public Chater(TcpClient tcpClient)
        {
            connection = new(tcpClient);
        }
        public void HandShake(string userName, string ip, ushort port = 25565)
        {
            lock(connection)
            {
                connection.SendPacket(new HandShakePacket(758, ip, port));
                connection.SendPacket(new LoginPacket(userName));
            }
        }
        public void SendMessage(string message)
        {
            lock(connection)
            {
                connection.SendPacket(new ChatPacket(message));
            }
        }
        public Chat RecieveMessage()
        {
            while (true)
            {
                lock (connection)
                {
                    ServerPacket serverPacket = connection.ReceivePacket();
                    if(serverPacket is SystemChatMessagePacket packet)
                    {
                        return packet.GetChatMessage();
                    }
                }
            }
        }
    }
}
