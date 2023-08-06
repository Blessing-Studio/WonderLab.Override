using Craft.Net.Common;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftProtocol {
    public static class zlib {
        /// <summary>
        /// Decompress a byte array into another byte array of a potentially unlimited size (!)
        /// </summary>
        /// <param name="to_decompress">Data to decompress</param>
        /// <returns>Decompressed data as byte array</returns>
        public static byte[] Decompress(byte[] to_decompress) {
            ZlibStream stream = new(new System.IO.MemoryStream(to_decompress, false), CompressionMode.Decompress);
            byte[] buffer = new byte[16 * 1024];
            using System.IO.MemoryStream decompressedBuffer = new();
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                decompressedBuffer.Write(buffer, 0, read);
            return decompressedBuffer.ToArray();
        }

        /// <summary>
        /// Compress a byte array into another bytes array using Zlib compression
        /// </summary>
        /// <param name="to_compress">Data to compress</param>
        /// <returns>Compressed data as a byte array</returns>
        public static byte[] Compress(byte[] to_compress) {
            byte[] data;
            using (System.IO.MemoryStream memstream = new()) {
                using (ZlibStream stream = new(memstream, CompressionMode.Compress)) {
                    stream.Write(to_compress, 0, to_compress.Length);
                }
                data = memstream.ToArray();
            }
            return data;
        }

        /// <summary>
        /// Decompress a byte array into another byte array of the specified size
        /// </summary>
        /// <param name="to_decompress">Data to decompress</param>
        /// <param name="size_uncompressed">Size of the data once decompressed</param>
        /// <returns>Decompressed data as a byte array</returns>
        public static byte[] Decompress(byte[] to_decompress, int size_uncompressed) {
            ZlibStream stream = new(new System.IO.MemoryStream(to_decompress, false), CompressionMode.Decompress);
            byte[] packetData_decompressed = new byte[size_uncompressed];
            stream.Read(packetData_decompressed, 0, size_uncompressed);
            stream.Close();
            return packetData_decompressed;
        }

        public static byte[] GetFullBytes(byte[] data, int id) {
            MinecraftStream minecraftStream = new(new MemoryStream(data));
            byte[] buffer = new byte[minecraftStream.Length];
            minecraftStream.Position = 0;
            minecraftStream.Read(buffer, 0, buffer.Length);
            using MinecraftStream stream = new MinecraftStream(new MemoryStream());
            stream.WriteVarInt((int)(minecraftStream.Length + 1));
            stream.WriteVarInt(id);
            stream.Write(buffer, 0, buffer.Length);
            buffer = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
