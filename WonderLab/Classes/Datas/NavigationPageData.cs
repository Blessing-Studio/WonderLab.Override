namespace WonderLab.Classes.Datas;

public sealed record NavigationPageData {
    public required object Page {  get; init; }

    public required string PageKey { get; init; }
}
