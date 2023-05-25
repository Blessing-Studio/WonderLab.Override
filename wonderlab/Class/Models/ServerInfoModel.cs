using Newtonsoft.Json;
using System.Collections.Generic;

namespace wonderlab.Class.Models
{
    public record ServerInfoModel {   
        public PingPayload Response { get; set; }

        public long Latency { get; set; }
    }

    public record PlayersPayload {   
        [JsonProperty("max")]
        public int Max { get; set; }

        [JsonProperty("online")]
        public int Online { get; set; }

        [JsonProperty("sample")]
        public List<Player> Sample { get; set; }
    }

    public record PingPayload {   
        [JsonProperty("version")]
        public VersionPayload Version { get; set; }

        [JsonProperty("players")]
        public PlayersPayload Players { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("modinfo")]
        public ServerPingModInfo ModInfo { get; set; }

        [JsonProperty("favicon")] 
        public string Icon { get; set; }
    }

    public record Player {   
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")] 
        public string Id { get; set; }
    }

    public record VersionPayload {   
        [JsonProperty("protocol")] 
        public int Protocol { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }
    }

    public record ServerPingModInfo {   
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("modList")] 
        public List<ModInfo> ModList { get; set; }
    }

    public record ModInfo {
        [JsonProperty("modid")]
        public string ModId { get; set; }

        [JsonProperty("version")] 
        public string Version { get; set; }
    }
}
