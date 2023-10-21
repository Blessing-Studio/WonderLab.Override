using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models {
    public class AccountSkinModel {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("properties")]
        public List<SkinInfo> Properties { get; set; }
    }

    public class SkinInfo {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class SKIN {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class CAPE {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class Textures {
        [JsonPropertyName("SKIN")]
        public SKIN Skin { get; set; }

        [JsonPropertyName("CAPE")]
        public CAPE Cape { get; set; }
    }

    public class SkinMoreInfo {
        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; }

        [JsonPropertyName("profileId")]
        public string ProfileId { get; set; }

        [JsonPropertyName("profileName")]
        public string ProfileName { get; set; }

        [JsonPropertyName("textures")]
        public Textures Textures { get; set; }
    }
}
