using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server
{
    public class StatusPacket : ServerPacket
    {
        public string Status;
        public StatusPacket(string status)
        {
            Status = status;
        }
        public StatusPacket(byte[] data)
        {
            MinecraftStream minecraftStream = new(new MemoryStream(data));
            int id = minecraftStream.ReadVarInt();
            if(id != GetId())
            {
                throw new Exception("你确定这是一个Status包?");
            }
            Status = minecraftStream.ReadString();
        }
        public byte[] GetBytes()
        {
            MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(Status);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x00;
        }
        public Status GetStatus() 
        { 
            return new Status(Status);
        }
    }
}
