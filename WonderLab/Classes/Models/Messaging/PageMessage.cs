namespace WonderLab.Classes.Models.Messaging;

public record PageMessage
{
    public bool IsChildrenPage { get; set; }

    public object Page { get; set; }

    public string PageName { get; set; }
}