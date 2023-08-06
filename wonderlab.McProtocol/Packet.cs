using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Craft.Net.Common;

namespace MinecraftProtocol
{
    public interface Packet
    {
        public byte[] GetBytes();
        public int GetId();
    }
    public interface ClientPacket : Packet
    {

    }
    public interface ServerPacket : Packet
    {

    }
}
