using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol
{
    public class UnknownPacket : ClientPacket, ServerPacket
    {
        public int Id;
        public byte[] Data;
        public UnknownPacket(byte[] packetData) 
        {
            
            MinecraftStream minecraftStream = new(new MemoryStream(packetData));
            Id = minecraftStream.ReadVarInt();
            Data = new byte[minecraftStream.Length - minecraftStream.Position];
            minecraftStream.Read(Data, 0, Data.Length);
        }
        public byte[] GetBytes()
        {
            return Data;
        }

        public int GetId()
        {
            return Id;
        }
    }
}
