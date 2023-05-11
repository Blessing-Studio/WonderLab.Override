using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
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

        [JsonProperty("yggdrasilUrl")]
        public string YggdrasilUrl { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("userType")]
        public AccountType UserType { get; set; } = AccountType.Offline;
    }
}
