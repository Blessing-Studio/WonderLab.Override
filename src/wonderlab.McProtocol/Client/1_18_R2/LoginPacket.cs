using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Client.v1_18_R2
{
    public class LoginPacket : ClientPacket
    {
        public string PlayerName;
        public LoginPacket(string playerName)
        {
            PlayerName = playerName;
        }
        public byte[] GetBytes()
        {
            using MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(PlayerName);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x00;
        }
    }
}
