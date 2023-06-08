using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace wonderlab.Class.Models {
    public class WonderServerModel {
        [JsonProperty("isOfficial")]
        public bool IsOfficial { get; set; }

        [JsonProperty("isWelfare")]
        public bool IsWelfare { get; set; }

        [JsonProperty("hasModpacks")]
        public bool HasModpacks { get; set; }

        [JsonProperty("hasClient")]
        public bool HasClient { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("serverIp")]
        public string ServerIp { get; set; }

        [JsonProperty("serverPort")]
        public int ServerPort { get; set; }

        [JsonProperty("mcVersion")]
        public string McVersion { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("downloadMethod")]
        public IEnumerable<JObject> DownloadMethod { get; set; }
    }
}
