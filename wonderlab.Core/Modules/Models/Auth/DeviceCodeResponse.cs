using System.Text.Json.Serialization;

namespace MinecraftOAuth.Module.Models;
/// <summary>
/// 设备代码响应模型类
/// </summary>
public class DeviceCodeResponse {
    /// <summary>
    /// 用户代码
    /// </summary>
    [JsonPropertyName("user_code")]
    public string UserCode { get; set; }

    /// <summary>
    /// 设备代码
    /// </summary>
    [JsonPropertyName("device_code")]
    public string DeviceCode { get; set; }

    /// <summary>
    /// 验证网址
    /// </summary>
    [JsonPropertyName("verification_uri")]
    public string VerificationUrl { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// 间隔
    /// </summary>
    [JsonPropertyName("interval")]
    public int Interval { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}