using Craft.Net.Common;
using MinecraftProtocol.Client;
using MinecraftProtocol.Server;
using MinecraftProtocol.Server.v1_18_R2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol
{
    public class ClientPacketDecoder : PacketDecoder
    {
        public Packet GetPacket(byte[] data, State state)
        {
            MinecraftStream stream = new(new MemoryStream(data));
            int len = stream.ReadVarInt();
            int id = stream.ReadVarInt();
            if (id == 0)
            {
                if (len == 1 && state == State.Status)
                {
                    return new PingPacket();
                }
                //    if (state == State.Login)
                //    {
                //        string playerName = stream.ReadString();
                //        bool hasUUID = stream.ReadBoolean();
                //        if (hasUUID)
                //        {
                //            UUID uuid = new(stream.ReadInt64(), stream.ReadInt64());
                //            return new LoginPacket(playerName, hasUUID, uuid);
                //        }
                //        return new LoginPacket(playerName);
                //    }
                if (state == State.HandShaking)
                {
                    return new HandShakePacket(data);
                }
            }
            return new UnknownPacket(data);
        }
    }
    public class ServerPacketDecoder : PacketDecoder
    {
        public Packet GetPacket(byte[] data, State state)
        {
            MinecraftStream stream = new(new MemoryStream(data));
            int id = stream.ReadVarInt();
            if(id == 0x00 && state == State.Status)
            {
                return new StatusPacket(data);
            }
            if(id == 0x00 && state == State.Login)
            {
                //return new LoginPacket(data);
            }
            if(id == 0x03)
            {
                return new SetCompressionPacket(data);
            }
            if(id == 0x1A)
            {
                return new DisconnectOnPlayingPacket(data);
            }
            if(id == 0x21)
            {
                return new KeepAliveServerPacket(data);
            }
            if(id == 0x5F)
            {
                return new TabPacket(data);
            }
            if(id == 0x0F)
            {
                return new SystemChatMessagePacket(data);
            }
            return new UnknownPacket(data);
        }
    }
}
