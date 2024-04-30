using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WonderLab.Classes.Datas;
using WonderLab.Classes.Enums;
using System.Threading.Tasks.Dataflow;

namespace WonderLab.Services;

/// <summary>
///  日志记录服务
/// </summary>
public sealed class LogService {
    private readonly StreamWriter _writer;
    private readonly ActionBlock<LogData> _outputJobs;
    private readonly string _logFile = $"WonderLog-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log";
    
    public LogService() {
        _outputJobs = new ActionBlock<LogData>(async logMessage => {
            await WriteLogAsync(logMessage);
        });

        _writer = new StreamWriter(_logFile);
    }
    
    public void Info(string tag, string message) {
        _outputJobs.Post(new LogData {
            Tag = tag,
            Message = message, 
            Level = LogLevel.Info,
            Timestamp = DateTime.Now
        });
    }

    public void Debug(string tag, string message) {
        _outputJobs.Post(new LogData {
            Tag = tag,
            Message = message, 
            Level = LogLevel.Debug,
            Timestamp = DateTime.Now
        });    
    }

    public void Warn(string tag, string message) {
        _outputJobs.Post(new LogData {
            Tag = tag,
            Message = message, 
            Level = LogLevel.Warn,
            Timestamp = DateTime.Now
        });
    }

    public void Error(string tag, string message) {
        _outputJobs.Post(new LogData {
            Tag = tag,
            Message = message, 
            Level = LogLevel.Error,
            Timestamp = DateTime.Now
        });
    }

    public void Fatal(string tag, string message) {
        _outputJobs.Post(new LogData {
            Tag = tag,
            Message = message, 
            Level = LogLevel.Fatal,
            Timestamp = DateTime.Now
        });
    }
    
    public void Finish() {
        _outputJobs.Complete();
        _outputJobs.Completion.Wait();
        _writer.Flush();
        _writer.Dispose();
    }
    
    public string ReadLogs() {
        _writer.Flush();

        using var fs = File.Open(_logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs);
        return reader.ReadToEnd();
    }
    
    private async ValueTask WriteLogAsync(LogData logMessage) {
        var builder = new StringBuilder(1024);
        builder.Append($"[{logMessage.Timestamp:HH:mm:ss}] ");
        builder.Append($"[{logMessage.Level.ToString().ToUpper()}] ");
        builder.Append($"[{logMessage.Tag}] ");
        builder.Append(logMessage.Message);

        await _writer.WriteLineAsync(builder.ToString());
#if DEBUG
        System.Diagnostics.Debug.WriteLine(builder.ToString());
#endif
    }
}