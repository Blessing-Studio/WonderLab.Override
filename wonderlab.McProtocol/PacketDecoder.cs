using Craft.Net.Common;
using MinecraftProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol
{
    public interface PacketDecoder
    {

        public Packet GetPacket(byte[] data, State state);
    }
}
