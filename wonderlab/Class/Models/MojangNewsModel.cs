using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class MojangNewsModel {
        [JsonPropertyName("version")]
        public int Version { get; set; }
        
        [JsonPropertyName("entries")]
        public List<New> Entries { get; set; }
    }

    public class New {
        [JsonPropertyName("title")]
        public string Title { get; set; }
      
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("playPageImage")]
        public PlayPageBitMap PlayPageImage { get; set; }

        [JsonPropertyName("newsPageImage")]
        public NewsBitMap NewsPageImage { get; set; }

        [JsonPropertyName("readMoreLink")]
        public string ReadMoreLink { get; set; }

        [JsonPropertyName("newsType")]
        public List<string> NewsType { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

    }

    public class PlayPageBitMap {   
        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("url")]
        public string url { get; set; }
    }

    public class Dimensions {   
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class NewsBitMap {   
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("dimensions")]
        public Dimensions? Dimensions { get; set; }
    }
}
