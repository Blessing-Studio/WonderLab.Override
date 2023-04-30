using MinecraftProtocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Utils
{
    public static class FileUtils
    {
        public static void WriteCompressedAllText(string path, string? contents)
        {
            if(!File.Exists(path)) return;
            if(contents == null) contents = string.Empty;
            byte[] tmp = zlib.Compress(Encoding.UTF8.GetBytes(contents));
            File.WriteAllBytes(path, tmp);
        }
        public static string ReadCompressedAllText(string path)
        {
            if (!File.Exists(path)) return string.Empty;
            return Encoding.UTF8.GetString(zlib.Decompress(File.ReadAllBytes(path)));
        }
    }
}
