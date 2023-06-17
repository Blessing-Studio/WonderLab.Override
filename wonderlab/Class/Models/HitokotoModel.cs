using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class HitokotoModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("hitokoto")]
        public string Text { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("from_who")]
        public string FromWho { get; set; }

        [JsonPropertyName("creator")]
        public string Creator { get; set; }

        [JsonPropertyName("creator_uid")]
        public int CreatorUid { get; set; }

        [JsonPropertyName("reviewer")]
        public int Reviewer { get; set; }

        [JsonPropertyName("commit_from")]
        public string CommitFrom { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }
    }

}
