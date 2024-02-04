using WonderLab.Libs.Compression;

namespace WonderLab.Libs;

public static class Zlib
{
    /// <summary>
    /// Decompress a byte array into another byte array of a potentially unlimited size (!)
    /// </summary>
    /// <param name="to_decompress">Data to decompress</param>
    /// <returns>Decompressed data as byte array</returns>
    public static byte[] Decompress(byte[] to_decompress)
    {
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
    public static byte[] Compress(byte[] to_compress)
    {
        byte[] data;
        using (System.IO.MemoryStream memstream = new())
        {
            using (ZlibStream stream = new(memstream, CompressionMode.Compress))
            {
                stream.Write(to_compress, 0, to_compress.Length);
            }
            data = memstream.ToArray();
        }
        return data;
    }
}
