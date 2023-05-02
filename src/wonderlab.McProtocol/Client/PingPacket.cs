using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client
{
    public class PingPacket : ClientPacket
    {
        public byte[] GetBytes()
        {
            return new byte[] { };
            
        }

        public int GetId()
        {
            return 0x00;
        }
    }
}
