using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class MojangNewsModel {
        [JsonProperty("version")]
        public int Version { get; set; }
        
        [JsonProperty("entries")]
        public List<New>? Entries { get; set; }
    }

    public class New {
        [JsonProperty("title")]
        public string? Title { get; set; }
      
        [JsonProperty("tag")]
        public string? Tag { get; set; }

        [JsonProperty("category")]
        public string? Category { get; set; }

        [JsonProperty("date")]
        public string? Date { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }

        [JsonProperty("playPageImage")]
        public PlayPageBitMap? PlayPageImage { get; set; }

        [JsonProperty("newsPageImage")]
        public NewsBitMap? NewsPageImage { get; set; }

        [JsonProperty("readMoreLink")]
        public string? ReadMoreLink { get; set; }

        [JsonProperty("cardBorder")]
        public string? CardBorder { get; set; }

        [JsonProperty("newsType")]
        public List<string>? NewsType { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

    }

    public class PlayPageBitMap {   
        [JsonProperty("id")]
        public string? title { get; set; }

        [JsonProperty("id")]
        public string? url { get; set; }
    }

    public class Dimensions {   
        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class NewsBitMap {   
        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }

        [JsonProperty("dimensions")]
        public Dimensions? Dimensions { get; set; }
    }
}
