using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MinecraftProtocol.Client;
using MinecraftProtocol.Server;

namespace MinecraftProtocol
{
    public class ProtocolHacker
    {
        public string ServerIp = "127.0.0.1";
        public ushort ServerPort = 25565;
        public ushort ConnectPort = 35565;
        public Socket? socketWatch;
        public void Start()
        {
            //定义一个套接字用于监听客户端发来的信息 包含3个参数(IP4寻址协议,流式连接,TCP协议)
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);          
            IPAddress ipaddress = IPAddress.Parse("0.0.0.0");               
            IPEndPoint endpoint = new IPEndPoint(ipaddress, ConnectPort);
            socketWatch.Bind(endpoint);
            //将套接字的监听队列长度限制为20
            socketWatch.Listen(20);
            Watching(this);
        }
        private static void Watching(object? obj)
        {
            if (obj is ProtocolHacker watch)
            {
                while (true)
                {
                    Socket socket = watch.socketWatch.Accept();
                    ClientConnection clientConnection = new(socket);
                    ServerConnection serverConnection = new(watch.ServerIp);
                    Thread thread1 = new Thread(new ParameterizedThreadStart(CToS));
                    Thread thread2 = new Thread(new ParameterizedThreadStart(SToC));
                    thread1.Start((clientConnection, serverConnection, watch));
                    thread2.Start((serverConnection, clientConnection, watch));
                }
            }
        }
        private static void CToS(object? obj)
        {
            while (true)
            {
                (ClientConnection, ServerConnection, ProtocolHacker) connections = ((ClientConnection, ServerConnection, ProtocolHacker))obj;
                var tmp = connections.Item1.ReceivePacket();
                if (tmp is HandShakePacket packet)
                {
                    packet.port = connections.Item3.ServerPort;
                    packet.serverAddress = connections.Item3.ServerIp;
                    connections.Item2.SendPacket(packet);
                }
                else
                {
                    connections.Item2.SendPacket(tmp);
                }
            }
        }
        private static void SToC(object? obj)
        {
            while (true)
            {
                (ServerConnection, ClientConnection, ProtocolHacker) connections = ((ServerConnection, ClientConnection, ProtocolHacker))obj;
                connections.Item2.SendPacket(connections.Item1.ReceivePacket());
            }
        }
    }
}
