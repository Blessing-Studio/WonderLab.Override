using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models {
    public class SingleCoreModel {
        [JsonProperty("height")]
        public int GameWindowHeight { get; set; } = 480;

        [JsonProperty("width")]
        public int GameWindowWidth { get; set; } = 854;

        [JsonProperty("isfullscreen")]
        public bool IsFullScreen { get; set; } = false;

        [JsonProperty("issingleconfigenabled")]
        public bool IsSingleConfigEnabled { get; set; } = false;
    }
}
