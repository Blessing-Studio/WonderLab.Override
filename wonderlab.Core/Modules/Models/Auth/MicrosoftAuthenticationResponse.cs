using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace MinecraftOAuth.Module.Models {
    public class MicrosoftAuthenticationResponse {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("skins")]
        public List<SkinModel> Skins { get; set; }

        [JsonPropertyName("capes")]
        public JsonElement Capes { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }

        public string Time { get; set; }
    }

    public class SkinModel {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("variant")]
        public string Variant { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }
    }
}
