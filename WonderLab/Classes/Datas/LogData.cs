using System;
using WonderLab.Classes.Enums;

namespace WonderLab.Classes.Datas;

/// <summary>
/// 日志详细数据
/// </summary>
public sealed record LogData {
    public string Tag { get; set; }

    public LogLevel Level { get; set; }
    
    public string Message { get; set; }

    public DateTime Timestamp { get; set; }
}