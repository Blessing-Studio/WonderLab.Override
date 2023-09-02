using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class WebModpackInfoModel {   
        [JsonPropertyName("En")] public string CurseForgeId { get; set; }
        [JsonPropertyName("Zh")] public string Chinese { get; set; }
        [JsonPropertyName("MCModWikiId")] public int McModId { get; set; }
        [JsonPropertyName("MCBBSId")] public int McbbsId { get; set; }
    }
}
