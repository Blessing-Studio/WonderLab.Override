using Craft.Net.Common;
using MinecraftProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client
{
    public class ClientConnection
    {
        public PacketDecoder packetDecoder = new ClientPacketDecoder();
        public readonly Socket client;
        public Stream buffer = new MemoryStream();
        public byte[] arrayBuffer = new byte[1024 * 1024 * 16];
        public Thread networkThread = new Thread(new ParameterizedThreadStart(receiveBytes));
        public bool IsDisconnected = false;
        public State state = State.HandShaking;
        public Queue<ClientPacket> packets = new Queue<ClientPacket>();
        public ClientConnection(Socket tcpClient)
        {
            client = tcpClient;
            networkThread.Start(this);
        }
        private static void receiveBytes(object? arg)
        {

            if (arg != null && arg is ClientConnection)
            {
                ClientConnection client = (ClientConnection)arg;
                try
                {
                    while (!client.IsDisconnected)
                    {
                        Thread.Sleep(2);
                        int bytes1Len = client.client.Receive(client.arrayBuffer);
                        client.buffer.Write(client.arrayBuffer, 0, bytes1Len);
                        lock (client.buffer)
                        {
                            while (!client.IsDisconnected)
                            {
                                if (client.buffer.Length > 0)
                                {
                                    client.buffer.Position = 0;
                                    MinecraftStream minecraftStream = new(client.buffer);
                                    int packetLen = minecraftStream.ReadVarInt();
                                    if (minecraftStream.Length >= packetLen + minecraftStream.Position)
                                    {
                                        byte[] packetByte = new byte[packetLen + minecraftStream.Position];
                                        minecraftStream.Position = 0;
                                        minecraftStream.Read(packetByte, 0, packetByte.Length);
                                        ClientPacketDecoder decoder = new();
                                        ClientPacket packet = (ClientPacket)client.packetDecoder.GetPacket(packetByte, client.state);
                                        byte[] bufferBytes = new byte[client.buffer.Length - minecraftStream.Position];
                                        client.buffer.Read(bufferBytes, 0, bufferBytes.Length);
                                        client.buffer.SetLength(0);
                                        client.buffer.Write(bufferBytes, 0, bufferBytes.Length);
                                        if (packet is HandShakePacket hand)
                                        {
                                            client.state = hand.nextState;
                                        }
                                        lock (client.packets)
                                        {
                                            client.packets.Enqueue(packet);
                                        }
                                    }
                                    else
                                    {
                                        client.buffer.Position = client.buffer.Length;
                                        break;
                                    }
                                    if (client.buffer.Length == 0)
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                            }
                        }
                    }
                    client.client.Close();
                }
                catch { client.IsDisconnected = true; }
            }
        }
        public void SendPacket(ServerPacket packet)
        {
            if (packet is HandShakePacket)
            {
                state = ((HandShakePacket)packet).nextState;
            }
            MinecraftStream minecraftStream = new(new MemoryStream(packet.GetBytes()));
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            using MinecraftStream stream = new MinecraftStream(new MemoryStream());
            stream.WriteVarInt((int)(minecraftStream.Length + 1));
            stream.WriteVarInt(packet.GetId());
            stream.Write(buffer, 0, buffer.Length);
            buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            client.Send(buffer);
        }
        public ClientPacket ReceivePacket()
        {
            while (true)
            {
                if (packets.Count == 0)
                {
                    Thread.Sleep(5);
                }
                else
                {
                    lock (packets)
                    {
                        return packets.Dequeue();
                    }
                }
            }
        }
    }
}
