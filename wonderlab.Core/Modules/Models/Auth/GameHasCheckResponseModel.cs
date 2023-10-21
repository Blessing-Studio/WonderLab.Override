using System.Text.Json.Serialization;

namespace MinecraftOAuth.Module.Models {
    public class OwnershipItem {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("signature")] public string Signature { get; set; }
    }

    public class GameHasCheckResponseModel {
        [JsonPropertyName("items")] public List<OwnershipItem> Items { get; set; }

        [JsonPropertyName("signature")] public string Signature { get; set; }

        [JsonPropertyName("keyId")] public string KeyId { get; set; }
    }
}
