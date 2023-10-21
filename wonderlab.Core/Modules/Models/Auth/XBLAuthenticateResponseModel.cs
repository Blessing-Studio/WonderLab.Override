using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinecraftOAuth.Module.Models {
    public class XBLAuthenticateResponseModel {
        [JsonPropertyName("IssueInstant")]
        public string IssueInstant { get; set; }

        [JsonPropertyName("NotAfter")]
        public string NotAfter { get; set; }

        [JsonPropertyName("Token")]
        public string Token { get; set; }

        [JsonPropertyName("DisplayClaims")]
        public DisplayClaimsModel DisplayClaims { get; set; }
    }
}
