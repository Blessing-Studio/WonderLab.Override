using MinecraftLaunch.Modules.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class UserModel {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("userToken")]
        public string UserToken { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("yggdrasilskinUrl")]
        public string YggdrasilSkinUrl { get; set; }

        [JsonProperty("userType")]
        public AccountType UserType { get; set; } = AccountType.Offline;
    }
}
