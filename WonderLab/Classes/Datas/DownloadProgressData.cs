namespace WonderLab.Classes.Datas;

public sealed record DownloadProgressData {
    public double Speed { get; set; }

    public int TotalCount { get; set; }
    public int TotalBytes { get; set; }
    public int FailedCount { get; set; }
    public int CompletedCount { get; set; }
    public int DownloadedBytes { get; set; }
}