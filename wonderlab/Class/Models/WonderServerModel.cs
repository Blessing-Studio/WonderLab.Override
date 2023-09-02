using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Text.Json;

namespace wonderlab.Class.Models {
    public class WonderServerModel {
        [JsonPropertyName("isOfficial")]
        public bool IsOfficial { get; set; }

        [JsonPropertyName("isWelfare")]
        public bool IsWelfare { get; set; }

        [JsonPropertyName("hasModpacks")]
        public bool HasModpacks { get; set; }

        [JsonPropertyName("hasClient")]
        public bool HasClient { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("serverIp")]
        public string ServerIp { get; set; }

        [JsonPropertyName("serverPort")]
        public int ServerPort { get; set; }

        [JsonPropertyName("mcVersion")]
        public string McVersion { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("downloadMethod")]
        public IEnumerable<JsonElement> DownloadMethod { get; set; }
    }
}
