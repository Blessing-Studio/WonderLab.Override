using Craft.Net.Common;
using MinecraftProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client.v1_18_R2
{
    public class PluginMessagePacket : ClientPacket
    {
        public string PluginChannel;
        public byte[] Data;
        public PluginMessagePacket(byte[] data)
        {
            using MinecraftStream stream = new MinecraftStream(new MemoryStream(data));
            if (GetId() != stream.ReadVarInt()) { throw new Exception("你确定这是一个keepAlive包?"); }
            PluginChannel = stream.ReadString();
            Data = stream.ReadUInt8Array((int)(stream.Length - stream.Position));
        }
        public PluginMessagePacket(string pluginChannel, byte[] data)
        {
            PluginChannel = pluginChannel;
            Data = data;
        }
        public byte[] GetBytes()
        {
            using MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(PluginChannel);
            minecraftStream.WriteUInt8Array(Data);
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
