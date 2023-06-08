using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client.v1_18_R2
{
    public class ChatPacket : ClientPacket
    {
        public string Message;
        public ChatPacket(byte[] data)
        {
            using MinecraftStream stream = new MinecraftStream(new MemoryStream(data));
            if (GetId() != stream.ReadVarInt()) { throw new Exception("你确定这是一个ChatMessage包?"); }
            Message = stream.ReadString();

        }
        public ChatPacket(string message)
        {
            Message = message;
        }
        public byte[] GetBytes()
        {
            using MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(Message);
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
