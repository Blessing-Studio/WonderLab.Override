using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class ModrinthJsonModel {
        [JsonProperty("formatVersion")]
        public int FormatVersion { get; set; }

        [JsonProperty("game")]
        public string Game { get; set; }

        [JsonProperty("versionId")]
        public string VersionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("files")]
        public IEnumerable<Files> Files { get; set; }

        [JsonProperty("dependencies")]
        public Dependencies Dependencies { get; set; }
    }

    public class Dependencies {      
        [JsonProperty("minecraft")]
        public string Minecraft { get; set; }

        [JsonProperty("quilt-loader")]
        public string QuiltLoader { get; set; } = string.Empty;

        [JsonProperty("fabric-loader")]
        public string FabricLoader { get; set; } = string.Empty;
    }

    public class Files {
        [JsonProperty("fileSize")]
        public int FileSize { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("hashes")]
        public Hashes Hashes { get; set; }

        [JsonProperty("env")]
        public Env Env { get; set; }

        [JsonProperty("downloads")]
        public string[] Downloads { get; set; }

    }

    public class Hashes {
        [JsonProperty("sha1")]
        public string Sha1 { get; set; }

        [JsonProperty("sha512")]
        public string Sha512 { get; set; }
    }

    public class Env {
        [JsonProperty("client")]
        public string Client { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }
    }
}
