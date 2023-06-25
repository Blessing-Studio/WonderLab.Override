using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinecaftOAuth.Module.Models
{
    public class OwnershipItem
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("signature")] public string Signature { get; set; }
    }

    public class GameHasCheckResponseModel
    {
        [JsonPropertyName("items")] public List<OwnershipItem> Items { get; set; }

        [JsonPropertyName("signature")] public string Signature { get; set; }

        [JsonPropertyName("keyId")] public string KeyId { get; set; }
    }
}
