using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class HitokotoModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("hitokoto")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("from_who")]
        public string FromWho { get; set; }

        [JsonProperty("creator")]
        public string Creator { get; set; }

        [JsonProperty("creator_uid")]
        public int CreatorUid { get; set; }

        [JsonProperty("reviewer")]
        public int Reviewer { get; set; }

        [JsonProperty("commit_from")]
        public string CommitFrom { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }
    }

}
