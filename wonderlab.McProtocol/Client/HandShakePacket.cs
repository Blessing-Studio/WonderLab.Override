using Craft.Net.Common;
using MinecraftProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client
{
    public class HandShakePacket : ClientPacket
    {
        public int protocolVersion;
        public string serverAddress;
        public ushort port;
        public State nextState;
        public HandShakePacket(byte[] data)
        {
            using MinecraftStream stream = new MinecraftStream(new MemoryStream(data));
            int len = stream.ReadVarInt();
            if(GetId() != stream.ReadVarInt()){ throw new Exception("你确定这是一个handshake包?"); }
            protocolVersion = stream.ReadVarInt();
            serverAddress = stream.ReadString();
            port = stream.ReadUInt16();
            nextState = (State)stream.ReadVarInt();
        }
        public HandShakePacket(int ProtocolVersion, string ServerAddress, ushort Port, State NextState = State.Status) 
        {
            protocolVersion = ProtocolVersion;
            serverAddress = ServerAddress;
            port = Port;
            nextState = NextState;
        }
        public byte[] GetBytes()
        {
            using MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteVarInt(protocolVersion);
            minecraftStream.WriteString(serverAddress);
            minecraftStream.WriteUInt16(port);
            minecraftStream.WriteVarInt((int)nextState);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x00;
        }
    }
}
