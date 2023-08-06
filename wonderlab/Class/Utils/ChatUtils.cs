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

namespace wonderlab.Class.Utils {
    public class ChatUtils {
        public readonly ServerConnection connection;

        public ChatUtils(string ip, ushort port = 25565) {
            connection = new ServerConnection(ip, port);
        }

        public ChatUtils(ServerConnection connection) {
            this.connection = connection;
        }

        public ChatUtils(TcpClient tcpClient) {
            connection = new(tcpClient);
        }

        public void HandShake(string userName, string ip, ushort port = 25565) {
            lock (connection) {
                connection.SendPacket(new HandShakePacket(758, ip, port));
                connection.SendPacket(new LoginPacket(userName));
            }
        }

        public void Send(string message) {
            lock (connection) {
                connection.SendPacket(new ChatPacket(message));
            }
        }

        public Chat Recieve() {
            while (true) {
                lock (connection) {
                    ServerPacket serverPacket = connection.ReceivePacket();
                    if (serverPacket is SystemChatMessagePacket packet) {
                        return packet.GetChatMessage();
                    }
                }
            }
        }
    }
}
