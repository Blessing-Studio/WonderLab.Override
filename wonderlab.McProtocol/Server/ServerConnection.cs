using Craft.Net.Common;
using Ionic.Zlib;
using MinecraftProtocol.Client;
using MinecraftProtocol.Server.v1_18_R2;
using System.Net.Sockets;

namespace MinecraftProtocol.Server {
    public class ServerConnection {
        public int Threshold;
        public bool IsCompressed = false;
        public PacketDecoder packetDecoder = new ServerPacketDecoder();
        public readonly TcpClient client;
        public NetworkStream networkStream;
        public byte[] arrayBuffer = new byte[16384];
        public bool IsDisconnected = false;
        public State state = State.HandShaking;
        public Queue<ServerPacket> packets = new Queue<ServerPacket>();
        public ServerConnection(TcpClient tcpClient) {
            client = tcpClient;
            networkStream = tcpClient.GetStream();
        }
        public ServerConnection(string serverAddress, ushort port = 25565) {
            client = new TcpClient(serverAddress, port);
            networkStream = client.GetStream();
        }

        public void SendPacket(ClientPacket packet) {
            if (packet is HandShakePacket) {
                state = ((HandShakePacket)packet).nextState;
            }

            if (!IsCompressed) {
                byte[] buffer = zlib.GetFullBytes(packet.GetBytes(), packet.GetId());
                client.Client.Send(buffer);
            } else {
                MinecraftStream minecraftStream = new(new MemoryStream());
                MemoryStream memoryStream = new MemoryStream(); ;
                MinecraftStream minecraftStream1 = new(memoryStream);
                minecraftStream1.WriteVarInt(packet.GetId());
                minecraftStream1.WriteUInt8Array(packet.GetBytes());
                byte[] buffer = memoryStream.ToArray();
                if (buffer.Length >= Threshold) {
                    minecraftStream.WriteVarInt(buffer.Length);
                    buffer = zlib.Compress(buffer);
                } else {
                    minecraftStream.WriteVarInt(0);
                }
                minecraftStream.WriteUInt8Array(buffer);
                MinecraftStream stream = new(networkStream);
                stream.WriteVarInt((int)minecraftStream.Length);
                stream.WriteUInt8Array(((MemoryStream)minecraftStream.BaseStream).ToArray());
            }
        }

        public ServerPacket ReceivePacket() {
            MinecraftStream minecraftStream = new(networkStream);
            int packetLen = minecraftStream.ReadVarInt();
            byte[] packetData = minecraftStream.ReadUInt8Array(packetLen);
            MinecraftStream packetStream = new(new MemoryStream(packetData));
            if (IsCompressed) {
                int dataLen = packetStream.ReadVarInt();
                if (dataLen != 0) {
                    byte[] toDeCompress = packetStream.ReadUInt8Array((int)(packetStream.Length - packetStream.Position));
                    packetData = zlib.Decompress(toDeCompress, dataLen);
                } else {
                    packetData = packetStream.ReadUInt8Array((int)(packetStream.Length - packetStream.Position));
                }
            }
            ServerPacket serverPacket = (ServerPacket)packetDecoder.GetPacket(packetData, state);
            if (serverPacket is SetCompressionPacket packet) {
                if (packet.Threshold > 0) {
                    IsCompressed = true;
                    Threshold = packet.Threshold;
                }
            }
            return serverPacket;
        }
    }

    public enum State {
        HandShaking = 0,
        Status = 1,
        Login = 2,
        Play = 3
    }
}
