using System;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Material.Icons;

namespace WonderLab.UI.Controls;

[StyledProperty(typeof(string), "Title")]
[StyledProperty(typeof(string), "SupportingText")]
[StyledProperty(typeof(object), "CloseButtonContent", "Cancel")]
[StyledProperty(typeof(bool), "IsCloseButtonVisible", true)]
[StyledProperty(typeof(MaterialIconKind?), "IconKind")]
[StyledProperty(typeof(AvaloniaList<Button>), "ExtendButtons")]
[TemplatePart(Name = "PART_CloseButton", Type = typeof(Button), IsRequired = true)]
public partial class DialogContentControl : UserControl {
    private const string PART_DialogHost = "PART_DialogHost";
    
    protected override Type StyleKeyOverride => typeof(DialogContentControl);

    public DialogContentControl() => ExtendButtons = [];
}