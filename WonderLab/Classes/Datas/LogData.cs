using System;
using WonderLab.Classes.Enums;

namespace WonderLab.Classes.Datas;

/// <summary>
/// 日志详细数据
/// </summary>
public sealed record LogData
{
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// 日志等级
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// 日志内容
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 日志时间
    /// </summary>
    public DateTime Timestamp { get; set; }
}