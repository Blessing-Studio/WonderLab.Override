using MinecraftLaunch.Modules.Utils;
using MinecraftProtocol.Server;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace wonderlab.Class.Models
{
    public record ServerInfoModel {   
        public PingPayload Response { get; set; }

        public long Latency { get; set; }
    }

    public record PlayersPayload {   
        [JsonPropertyName("max")]
        public int Max { get; set; }

        [JsonPropertyName("online")]
        public int Online { get; set; }

        [JsonPropertyName("sample")]
        public List<Player> Sample { get; set; }
    }

    public record PingPayload {   
        [JsonPropertyName("version")]
        public VersionPayload Version { get; set; }

        [JsonPropertyName("players")]
        public PlayersPayload Players { get; set; }

        [JsonPropertyName("description")]
        public object Description { get; set; }

        [JsonPropertyName("modinfo")]
        public ServerPingModInfo ModInfo { get; set; }

        [JsonPropertyName("favicon")] 
        public string Icon { get; set; }
        public static implicit operator Status(PingPayload ping)
        {
            return new(ping.ToJson());
        }
    }

    public record Player {   
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("id")] 
        public string Id { get; set; }
    }

    public record VersionPayload {   
        [JsonPropertyName("protocol")] 
        public int Protocol { get; set; }

        [JsonPropertyName("name")] 
        public string Name { get; set; }
    }

    public record ServerPingModInfo {   
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("modList")] 
        public List<ModInfo> ModList { get; set; }
    }

    public record ModInfo {
        [JsonPropertyName("modid")]
        public string ModId { get; set; }

        [JsonPropertyName("version")] 
        public string Version { get; set; }
    }
}
