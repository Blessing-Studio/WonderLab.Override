namespace WonderLab.Classes.Datas;

public class DownloadItemData {
    public string Url { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }

    public int Size { get; set; }
    public int DownloadedBytes { get; set; }

    public bool IsCompleted { get; set; }
    public bool IsPartialContentSupported { get; set; }
}