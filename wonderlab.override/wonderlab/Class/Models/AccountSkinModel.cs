using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class AccountSkinModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public List<SkinInfo> Properties { get; set; }
    }

    public class SkinInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class SKIN
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class CAPE
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class Textures
    {
        [JsonProperty("SKIN")]
        public SKIN Skin { get; set; }

        [JsonProperty("CAPE")]
        public CAPE Cape { get; set; }
    }

    public class SkinMoreInfo
    {
        [JsonProperty("timestamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("profileId")]
        public string ProfileId { get; set; }

        [JsonProperty("profileName")]
        public string ProfileName { get; set; }

        [JsonProperty("textures")]
        public Textures Textures { get; set; }
    }
}
