using Craft.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol.Server.v1_18_R2
{
    public class TabPacket : ServerPacket
    {
        public string Header;
        public string Footer;
        public TabPacket(string header, string footer)
        {
            Header = header;
            Footer = footer;
        }
        public TabPacket(byte[] data)
        {
            MinecraftStream minecraftStream = new(new MemoryStream(data));
            int id = minecraftStream.ReadVarInt();
            if (id != GetId())
            {
                throw new Exception("你确定这是一个Tab包?");
            }
            Header = minecraftStream.ReadString();
            Footer = minecraftStream.ReadString();
        }
        public Chat GetChatHeader()
        {
            return Chat.FromJson(Header);
        }
        public Chat GetChatFooter()
        {
            return Chat.FromJson(Footer);
        }

        public byte[] GetBytes()
        {
            MinecraftStream minecraftStream = new(new MemoryStream());
            minecraftStream.WriteString(Header);
            minecraftStream.WriteString(Footer);
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        public int GetId()
        {
            return 0x5F;
        }
    }
}
