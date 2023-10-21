using System.Text.Json;
using System.Text.Json.Serialization;

namespace MinecraftOAuth.Module.Models {
    public class DisplayClaimsModel {
        [JsonPropertyName("xui")]
        public List<JsonElement> Xui { get; set; }
    }
}
