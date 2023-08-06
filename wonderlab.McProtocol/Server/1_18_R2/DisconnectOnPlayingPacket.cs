using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server.v1_18_R2
{
    public class DisconnectOnPlayingPacket : ServerPacket
    {
        public string Reason;
        public DisconnectOnPlayingPacket(string reason)
        {
            Reason = reason;
        }
        public DisconnectOnPlayingPacket(byte[] data) 
        {
            MinecraftStream minecraftStream = new(new MemoryStream(data));
            int id = minecraftStream.ReadVarInt();
            if (id != GetId())
            {
                throw new Exception("你确定这是一个Disconnect包?");
            }
            Reason = minecraftStream.ReadString();
        }
        public Chat GetChatReason()
        {
            return Chat.FromJson(Reason);
        }
        public byte[] GetBytes()
        {
            MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(Reason);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x1A;
        }
    }
}
