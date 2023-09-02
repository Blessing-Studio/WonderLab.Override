using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftOAuth.Module.Models {
    public class YggdrasilResponse {
        /// <summary>
        /// 验证令牌
        /// </summary>
        [JsonPropertyName("accessToken")] public string AccessToken { get; init; }

        [JsonPropertyName("clientToken")] public string ClientToken { get; init; }
        /// <summary>
        /// 用户在皮肤站注册的所有游戏账号
        /// </summary>
        [JsonPropertyName("availableProfiles")] public List<AvailableProfiles> UserAccounts { get; init; }
    }
}

public class AvailableProfiles {
    /// <summary>
    /// 游戏角色Uuid
    /// </summary>
    [JsonPropertyName("id")] public string Uuid { get; init; }
    /// <summary>
    /// 游戏角色名
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; init; }
}
