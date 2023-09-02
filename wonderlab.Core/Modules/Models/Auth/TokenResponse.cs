using System.Text.Json.Serialization;

namespace MinecraftOAuth.Module.Models;

public class TokenResponse {
    /// <summary>
    /// 到期时间
    /// </summary>
    [JsonPropertyName("expires_in")] 
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// 刷新令牌
    /// </summary>
    [JsonPropertyName("refresh_token")] 
    public string RefreshToken { get; set; }
}