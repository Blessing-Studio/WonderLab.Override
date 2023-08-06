using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server.v1_18_R2
{
    public class KeepAliveServerPacket : ServerPacket
    {
        public long Id;
        public KeepAliveServerPacket(byte[] data)
        {
            using MinecraftStream stream = new MinecraftStream(new MemoryStream(data));
            if (GetId() != stream.ReadVarInt()) { throw new Exception("你确定这是一个keepAlive包?"); }
            Id = stream.ReadInt64();
        }
        public KeepAliveServerPacket(long id)
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
            return 0x21;
        }
    }
}
