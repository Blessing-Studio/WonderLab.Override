using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models {
    public class SingleCoreModel {
        [JsonPropertyName("height")]
        public int GameWindowHeight { get; set; } = 480;

        [JsonPropertyName("width")]
        public int GameWindowWidth { get; set; } = 854;

        [JsonPropertyName("isfullscreen")]
        public bool IsFullScreen { get; set; } = false;

        [JsonPropertyName("issingleconfigenabled")]
        public bool IsSingleConfigEnabled { get; set; } = false;
    }
}
