using System;
using Avalonia.Markup.Xaml;

namespace WonderLab.UI.Markups;

public sealed class EnumBindingSourceExtension : MarkupExtension {
    private readonly Type _enumType;

    public EnumBindingSourceExtension(Type enumType) {
        _enumType = enumType;
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider) {
        return Enum.GetValues(_enumType);
    }
}