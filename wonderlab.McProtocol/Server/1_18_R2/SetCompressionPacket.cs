using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server.v1_18_R2
{
    public class SetCompressionPacket : ServerPacket
    {
        public int Threshold;
        public SetCompressionPacket(int threshold)
        {
            Threshold = threshold;
        }
        public SetCompressionPacket(byte[] data)
        {
            MinecraftStream minecraftStream = new(new MemoryStream(data));
            int id = minecraftStream.ReadVarInt();
            if (id != GetId())
            {
                throw new Exception("你确定这是一个SetCompression包?");
            }
            Threshold = minecraftStream.ReadVarInt();
        }
        public byte[] GetBytes()
        {
            MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteVarInt(Threshold);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x03;
        }
    }
}
