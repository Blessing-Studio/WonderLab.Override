using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Models.Auth;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wonderlab.Class.Models
{
    public class UserModel {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("userToken")]
        public string UserToken { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("yggdrasilUrl")]
        public string YggdrasilUrl { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("userType")]
        public AccountType UserType { get; set; } = AccountType.Offline;
    }
}
