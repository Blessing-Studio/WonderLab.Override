using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Text;

namespace Craft.Net.Common
{
    // This file only has members of Stream
    public partial class MinecraftStream : Stream
    {
        public MinecraftStream(Stream baseStream)
        {
            BaseStream = baseStream;
        }

        public Stream BaseStream { get; set; }

        public override bool CanRead { get { return BaseStream.CanRead; } }

        public override bool CanSeek { get { return BaseStream.CanSeek; } }

        public override bool CanWrite { get { return BaseStream.CanWrite; } }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Length
        {
            get { return BaseStream.Length; }
        }

        public override long Position
        {
            get { return BaseStream.Position; }
            set { BaseStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
        }
    }
}
namespace Craft.Net.Common
{
    /// <summary>
    /// A big-endian stream for reading/writing Minecraft data types.
    /// </summary>
    public partial class MinecraftStream
    {
        static MinecraftStream()
        {
            StringEncoding = Encoding.UTF8;
        }

        public static Encoding StringEncoding;

        /// <summary>
        /// Reads a variable-length integer from the stream.
        /// </summary>
        public int ReadVarInt()
        {
            uint result = 0;
            int length = 0;
            while (true)
            {
                byte current = ReadUInt8();
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt may not be longer than 28 bits.");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }

        /// <summary>
        /// Reads a variable-length integer from the stream.
        /// </summary>
        /// <param name="length">The actual length, in bytes, of the integer.</param>
        public int ReadVarInt(out int length)
        {
            uint result = 0;
            length = 0;
            while (true)
            {
                byte current = ReadUInt8();
                result |= (current & 0x7Fu) << length++ * 7;
                if (length > 5)
                    throw new InvalidDataException("VarInt may not be longer than 60 bits.");
                if ((current & 0x80) != 128)
                    break;
            }
            return (int)result;
        }

        /// <summary>
        /// Writes a variable-length integer to the stream.
        /// </summary>
        public void WriteVarInt(int _value)
        {
            uint value = (uint)_value;
            while (true)
            {
                if ((value & 0xFFFFFF80u) == 0)
                {
                    WriteUInt8((byte)value);
                    break;
                }
                WriteUInt8((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }
        }

        /// <summary>
        /// Writes a variable-length integer to the stream.
        /// </summary>
        /// <param name="length">The actual length, in bytes, of the written integer.</param>
        public void WriteVarInt(int _value, out int length)
        {
            uint value = (uint)_value;
            Console.WriteLine(value.ToString("X"));
            length = 0;
            while (true)
            {
                length++;
                if ((value & 0xFFFFFF80u) == 0)
                {
                    WriteUInt8((byte)value);
                    break;
                }
                WriteUInt8((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }
        }

        public static int GetVarIntLength(int _value)
        {
            uint value = (uint)_value;
            int length = 0;
            while (true)
            {
                length++;
                if ((value & 0xFFFFFF80u) == 0)
                    break;
                value >>= 7;
            }
            return length;
        }

        public byte ReadUInt8()
        {
            int value = BaseStream.ReadByte();
            if (value == -1)
                throw new EndOfStreamException();
            return (byte)value;
        }

        public void WriteUInt8(byte value)
        {
            WriteByte(value);
        }

        public sbyte ReadInt8()
        {
            return (sbyte)ReadUInt8();
        }

        public void WriteInt8(sbyte value)
        {
            WriteUInt8((byte)value);
        }

        public ushort ReadUInt16()
        {
            return (ushort)(
                (ReadUInt8() << 8) |
                ReadUInt8());
        }

        public void WriteUInt16(ushort value)
        {
            Write(new[]
            {
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 2);
        }

        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        public void WriteInt16(short value)
        {
            WriteUInt16((ushort)value);
        }

        public uint ReadUInt32()
        {
            return (uint)(
                (ReadUInt8() << 24) |
                (ReadUInt8() << 16) |
                (ReadUInt8() << 8) |
                 ReadUInt8());
        }

        public void WriteUInt32(uint value)
        {
            Write(new[]
            {
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 4);
        }

        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        public void WriteInt32(int value)
        {
            WriteUInt32((uint)value);
        }

        public ulong ReadUInt64()
        {
            return unchecked(
                   ((ulong)ReadUInt8() << 56) |
                   ((ulong)ReadUInt8() << 48) |
                   ((ulong)ReadUInt8() << 40) |
                   ((ulong)ReadUInt8() << 32) |
                   ((ulong)ReadUInt8() << 24) |
                   ((ulong)ReadUInt8() << 16) |
                   ((ulong)ReadUInt8() << 8) |
                    (ulong)ReadUInt8());
        }

        public void WriteUInt64(ulong value)
        {
            Write(new[]
            {
                (byte)((value & 0xFF00000000000000) >> 56),
                (byte)((value & 0xFF000000000000) >> 48),
                (byte)((value & 0xFF0000000000) >> 40),
                (byte)((value & 0xFF00000000) >> 32),
                (byte)((value & 0xFF000000) >> 24),
                (byte)((value & 0xFF0000) >> 16),
                (byte)((value & 0xFF00) >> 8),
                (byte)(value & 0xFF)
            }, 0, 8);
        }

        public long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        public void WriteInt64(long value)
        {
            WriteUInt64((ulong)value);
        }

        public byte[] ReadUInt8Array(int length)
        {
            var result = new byte[length];
            if (length == 0) return result;
            int n = length;
            while (true)
            {
                n -= Read(result, length - n, n);
                if (n == 0)
                    break;
                System.Threading.Thread.Sleep(1);
            }
            return result;
        }

        public void WriteUInt8Array(byte[] value)
        {
            Write(value, 0, value.Length);
        }

        public void WriteUInt8Array(byte[] value, int offset, int count)
        {
            Write(value, offset, count);
        }

        public sbyte[] ReadInt8Array(int length)
        {
            return (sbyte[])(Array)ReadUInt8Array(length);
        }

        public void WriteInt8Array(sbyte[] value)
        {
            Write((byte[])(Array)value, 0, value.Length);
        }

        public ushort[] ReadUInt16Array(int length)
        {
            var result = new ushort[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt16();
            return result;
        }

        public void WriteUInt16Array(ushort[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt16(value[i]);
        }

        public short[] ReadInt16Array(int length)
        {
            return (short[])(Array)ReadUInt16Array(length);
        }

        public void WriteInt16Array(short[] value)
        {
            WriteUInt16Array((ushort[])(Array)value);
        }

        public uint[] ReadUInt32Array(int length)
        {
            var result = new uint[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt32();
            return result;
        }

        public void WriteUInt32Array(uint[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt32(value[i]);
        }

        public int[] ReadInt32Array(int length)
        {
            return (int[])(Array)ReadUInt32Array(length);
        }

        public void WriteInt32Array(int[] value)
        {
            WriteUInt32Array((uint[])(Array)value);
        }

        public ulong[] ReadUInt64Array(int length)
        {
            var result = new ulong[length];
            if (length == 0) return result;
            for (int i = 0; i < length; i++)
                result[i] = ReadUInt64();
            return result;
        }

        public void WriteUInt64Array(ulong[] value)
        {
            for (int i = 0; i < value.Length; i++)
                WriteUInt64(value[i]);
        }

        public long[] ReadInt64Array(int length)
        {
            return (long[])(Array)ReadUInt64Array(length);
        }

        public void WriteInt64Array(long[] value)
        {
            WriteUInt64Array((ulong[])(Array)value);
        }

        public unsafe float ReadSingle()
        {
            uint value = ReadUInt32();
            return *(float*)&value;
        }

        public unsafe void WriteSingle(float value)
        {
            WriteUInt32(*(uint*)&value);
        }

        public unsafe double ReadDouble()
        {
            ulong value = ReadUInt64();
            return *(double*)&value;
        }

        public unsafe void WriteDouble(double value)
        {
            WriteUInt64(*(ulong*)&value);
        }

        public bool ReadBoolean()
        {
            return ReadUInt8() != 0;
        }

        public void WriteBoolean(bool value)
        {
            WriteUInt8(value ? (byte)1 : (byte)0);
        }

        public string ReadString()
        {
            long length = ReadVarInt();
            if (length == 0) return string.Empty;
            var data = ReadUInt8Array((int)length);
            return StringEncoding.GetString(data);
        }

        public void WriteString(string value)
        {
            WriteVarInt(StringEncoding.GetByteCount(value));
            if (value.Length > 0)
                WriteUInt8Array(StringEncoding.GetBytes(value));
        }
        public UUID ReadUUID()
        {
            return new UUID(ReadInt64(), ReadInt64());
        }

        public void WriteUUID(UUID value)
        {
            WriteInt64(value.High);
            WriteInt64(value.Low);
        }
    }
}
public class Chat
{
    public string JsonData;
    public bool Bold
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var bold = obj["bold"];
            if(bold != null)
            {
                return ((bool)bold);
            }
            return false;
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["bold"] = value;
            JsonData = obj.ToString();
        }
    }
    public bool Italic
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var italic = obj["italic"];
            if (italic != null)
            {
                return ((bool)italic);
            }
            return false;
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["italic"] = value;
            JsonData = obj.ToString();
        }
    }
    public bool Underlined
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var underlined = obj["underlined"];
            if (underlined != null)
            {
                return ((bool)underlined);
            }
            return false;
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["underlined"] = value;
            JsonData = obj.ToString();
        }
    }
    public bool Strikethrough
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var strikethrough = obj["strikethrough"];
            if (strikethrough != null)
            {
                return ((bool)strikethrough);
            }
            return false;
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["strikethrough"] = value;
            JsonData = obj.ToString();
        }
    }
    public bool Obfuscated
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var obfuscated = obj["obfuscated"];
            if (obfuscated != null)
            {
                return ((bool)obfuscated);
            }
            return false;
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["obfuscated"] = value;
            JsonData = obj.ToString();
        }
    }
    public string Color
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var color = obj["color"];
            if (color != null)
            {
                return color.ToString();
            }
            return ChatColor.white.ToString();
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["color"] = value;
            JsonData = obj.ToString();
        }
    }
    public string Text
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            var text = obj["text"];
            if (text != null)
            {
                return text.ToString();
            }
            return string.Empty;
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            obj["text"] = value;
            JsonData = obj.ToString();
        }
    }
    public List<Chat> Extra
    {
        get
        {
            JObject obj = JObject.Parse(JsonData);
            if (obj.ContainsKey("extra"))
            {
                var tmp = obj["extra"];
                if (tmp != null)
                {
                    JArray jarr = JArray.Parse(tmp.ToString());
                    List<Chat> list = new List<Chat>();
                    foreach (var extraText in jarr)
                    {
                        list.Add(Chat.FromJson(extraText.ToString()));
                    }
                    return list;
                }
            }
            return new();
        }
        set
        {
            JObject obj = JObject.Parse(JsonData);
            List<string> list = new List<string>();
            foreach(var chat in value)
            {
                list.Add(chat.JsonData);
            }
            JArray jarr = JArray.FromObject(list);
            obj["extra"] = jarr;
            JsonData = obj.ToString();
        }
    }
    public void Add(Chat chat)
    {
        var tmp = Extra;
        tmp.Add(chat);
        Extra = tmp;
    }
    public static Chat FromJson(string jsonData)
    {
        Chat ret = new Chat("");
        ret.JsonData = jsonData;
        return ret;
    }
    public string ToColorfulString()
    {
        Color color = ChatColorUtils.GetColor(Color);
        string buffer = "\x1b[38;2;" + color.R.ToString() + ";" + color.G.ToString() + ";" + color.B.ToString() + "m" + Text;
        foreach (var extraText in Extra)
        {
            buffer += extraText.ToColorfulString();
        }
        return buffer;
    }
    public string ToColorfulSytledString()
    {
        Color color = ChatColorUtils.GetColor(Color);
        string sytleBuffer = "";
        if (Bold)
        {
            sytleBuffer += "\x1b[1m";
        }
        if (Italic)
        {
            sytleBuffer += "\x1b[3m";
        }
        if (Underlined)
        {
            sytleBuffer += "\x1b[4m";
        }
        if (Strikethrough)
        {
            sytleBuffer += "\x1b[9m";
        }
        string buffer = sytleBuffer + "\x1b[38;2;" + color.R.ToString() + ";" + color.G.ToString() + ";" + color.B.ToString() + "m" + Text + "\x1b[0m";
        foreach (var extraText in Extra)
        {
            buffer += extraText.ToColorfulSytledString();
        }
        return buffer;
    }
    public Chat(string text)
    {
        JObject obj = new JObject();
        obj["text"] = text;
        JsonData = obj.ToString();
    }
    public Chat(Chat chat)
    {
        JsonData = chat.JsonData;
    }
    public override string ToString()
    {
        string buffer = Text;
        foreach (var extraText in Extra)
        {
            buffer += extraText.ToString();
        }
        return buffer;
    }
    public static Chat operator +(Chat left, Chat right)
    {
        Chat tmp = new(left);
        tmp.Add(right);
        return tmp;
    }
    public enum ChatColor
    {
        black,
        dark_blue,
        dark_green,
        dark_aqua,
        dark_red,
        dark_purple,
        gold,
        gray,
        dark_gray,
        blue,
        green,
        aqua,
        red,
        light_purple,
        yellow,
        white
    }
    public class ChatColorUtils
    {
        public static Dictionary<ChatColor, string> HtmlColors = new Dictionary<ChatColor, string>
        {
            {ChatColor.black, "#000000"},
            {ChatColor.dark_blue, "#0000aa"},
            {ChatColor.dark_green, "#00aa00"},
            {ChatColor.dark_aqua, "#00aaaa"},
            {ChatColor.dark_red, "#aa0000"},
            {ChatColor.dark_purple, "#aa00aa"},
            {ChatColor.gold, "#ffaa00"},
            {ChatColor.gray, "#aaaaaa"},
            {ChatColor.dark_gray, "#555555"},
            {ChatColor.blue, "#5555ff"},
            {ChatColor.green, "#55ff55"},
            {ChatColor.aqua, "#55ffff"},
            {ChatColor.red, "#ff5555"},
            {ChatColor.light_purple, "#ff55ff"},
            {ChatColor.yellow, "#ffff55"},
            {ChatColor.white, "#ffffff"},
        };
        public static Color GetColor(string color)
        {
            if (color.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(color);
            }
            return ColorTranslator.FromHtml(HtmlColors[(ChatColor)Enum.Parse(typeof(ChatColor), value: color)]);
        }
    }
}
public class UUID
{
    public long High;
    public long Low;
    public UUID(long high, long low)
    {
        High = high;
        Low = low;
    }
}