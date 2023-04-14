using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class WebModpackInfoModel {   
        [JsonProperty("En")] public string CurseForgeId { get; set; }
        [JsonProperty("Zh")] public string Chinese { get; set; }
        [JsonProperty("MCModWikiId")] public int McModId { get; set; }
        [JsonProperty("MCBBSId")] public int McbbsId { get; set; }
    }
}
