using HarfBuzzSharp;
using MinecraftProtocol.Client;
using MinecraftProtocol.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wonderlab.Class.Models;

namespace wonderlab.Class.Utils
{
    public class ServerUtils {
        private static List<byte>? _buffer;
        private static int _offset;
        private static NetworkStream? _stream;

        /// <summary>
        /// 服务器信息获取方法
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        public async static ValueTask<ServerInfoModel> GetServerInfoAsync(string Address, ushort Port, int VersionId = 0) {
            using var client = new TcpClient {           
                SendTimeout = 5000,
                ReceiveTimeout = 5000
            };
            var sw = new Stopwatch();
            var timeOut = TimeSpan.FromSeconds(3);
            using var cts = new CancellationTokenSource(timeOut);

            sw.Start();
            cts.CancelAfter(timeOut);

            try {           
                await client.ConnectAsync(Address, Port, cts.Token);
            }
            catch (TaskCanceledException) {           
                throw new OperationCanceledException($"服务器连接失败，连接超时 ({timeOut.Seconds}s)。", cts.Token);
            }

            sw.Stop();

            if (!client.Connected) {           
                return default;
            }

            _buffer = new List<byte>();
            _stream = client.GetStream();

            WriteVarInt(VersionId == 0 ? 47 : VersionId);
            WriteString(Address);
            WriteShort(Port);
            WriteVarInt(1);
            await Flush(0);
            await Flush(0);

            var batch = new byte[1024];
            await using var ms = new MemoryStream();
            var remaining = 0;
            var flag = false;

            var latency = sw.ElapsedMilliseconds;

            do {           
                var readLength = await _stream.ReadAsync(batch.AsMemory());
                await ms.WriteAsync(batch.AsMemory(0, readLength));
                if (!flag)
                {
                    var packetLength = ReadVarInt(ms.ToArray());
                    remaining = packetLength - _offset;
                    flag = true;
                }

                if (readLength == 0 && remaining != 0)
                    continue;

                remaining -= readLength;
            } while (remaining > 0);

            var buffer = ms.ToArray();
            _offset = 0;
            var length = ReadVarInt(buffer);
            var packet = ReadVarInt(buffer);
            var jsonLength = ReadVarInt(buffer);

            var json = ReadString(buffer, jsonLength);
            var ping = JsonConvert.DeserializeObject<PingPayload>(json);

            return new ServerInfoModel
            {
                Latency = latency,
                Response = ping
            };
        }

        private static byte ReadByte(IReadOnlyList<byte> buffer)
        {
            var b = buffer[_offset];
            _offset += 1;
            return b;
        }

        private static byte[] Read(byte[] buffer, int length)
        {
            var data = new byte[length];
            Array.Copy(buffer, _offset, data, 0, length);
            _offset += length;
            return data;
        }

        private static int ReadVarInt(IReadOnlyList<byte> buffer)
        {
            var value = 0;
            var size = 0;
            int b;
            while (((b = ReadByte(buffer)) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++ * 7);
                if (size > 5) throw new IOException("This VarInt is an imposter!");
            }

            return value | ((b & 0x7F) << (size * 7));
        }

        private static string ReadString(byte[] buffer, int length)
        {
            var data = Read(buffer, length);
            return Encoding.UTF8.GetString(data);
        }

        private static void WriteVarInt(int value)
        {
            while ((value & 128) != 0)
            {
                _buffer.Add((byte)((value & 127) | 128));
                value = (int)(uint)value >> 7;
            }

            _buffer.Add((byte)value);
        }

        private static void WriteShort(ushort value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        private static void WriteString(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer.Length);
            _buffer.AddRange(buffer);
        }

        private static async ValueTask Flush(int id = -1)
        {
            var buffer = _buffer.ToArray();
            _buffer.Clear();

            var add = 0;
            var packetData = new[] { (byte)0x00 };
            if (id >= 0)
            {
                WriteVarInt(id);
                packetData = _buffer.ToArray();
                add = packetData.Length;
                _buffer.Clear();
            }

            WriteVarInt(buffer.Length + add);
            var bufferLength = _buffer.ToArray();
            _buffer.Clear();

            await _stream.WriteAsync(bufferLength.AsMemory());
            await _stream.WriteAsync(packetData.AsMemory());
            await _stream.WriteAsync(buffer.AsMemory());
        }
    }
}
