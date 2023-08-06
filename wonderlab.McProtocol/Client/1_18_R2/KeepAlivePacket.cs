using Craft.Net.Common;
using MinecraftProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client.v1_18_R2
{
    public class KeepAlivePacket : ClientPacket
    {
        public long Id;
        public KeepAlivePacket(byte[] data)
        {
            using MinecraftStream stream = new MinecraftStream(new MemoryStream(data));
            if (GetId() != stream.ReadVarInt()) { throw new Exception("你确定这是一个keepAlive包?"); }
            Id = stream.ReadInt64();
        }
        public KeepAlivePacket(long id)
        {
            Id = id;
        }
        public byte[] GetBytes()
        {
            using MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteInt64(Id);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x0F;
        }
    }
}
