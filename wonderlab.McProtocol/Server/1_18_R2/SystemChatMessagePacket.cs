using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server.v1_18_R2
{
    public class SystemChatMessagePacket : ServerPacket
    {
        public string Message;
        public MessageType MessageType;
        public UUID UUID;
        public SystemChatMessagePacket(string message, MessageType messageType = MessageType.ChatMessage, UUID? uuid = null)
        {
            Message = message;
            MessageType = messageType;
            if(uuid == null)
            {
                UUID = new(0L, 0L);
            }
            else
            {
                UUID = uuid;
            }
        }
        public SystemChatMessagePacket(byte[] data)
        {
            MinecraftStream minecraftStream = new(new MemoryStream(data));
            int id = minecraftStream.ReadVarInt();
            if (id != GetId())
            {
                throw new Exception("你确定这是一个System/Chat包?");
            }
            Message = minecraftStream.ReadString();
            byte tmp = (byte)minecraftStream.ReadByte();
            MessageType = (MessageType)tmp;
            UUID = minecraftStream.ReadUUID();
        }
        public Chat GetChatMessage()
        {
            return Chat.FromJson(Message);
        }

        public byte[] GetBytes()
        {
            MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(Message);
            minecraftStream.WriteByte((byte)MessageType);
            minecraftStream.WriteUUID(UUID);
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
    public enum MessageType
    {
        ChatMessage,
        SystemMessage,
        HotBar
    }
}
